using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Default <see cref="ICsvExtractorBuilder{T}"/> implementation. Records configuration until the
/// first pipeline operator, then materializes a <see cref="CsvExtractor{TRecord}"/> and delegates
/// to a generic <see cref="EtlPipeline"/> built from it.
/// </summary>
/// <typeparam name="T">The record type produced by the extractor.</typeparam>
internal sealed class CsvExtractorBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    : ICsvExtractorBuilder<T>
    where T : notnull
{
    private readonly string? _path;
    private readonly StreamReader? _reader;
    private readonly CsvExtractor<T>? _existing;
    private readonly List<Action<CsvExtractor<T>>> _mutations = new();
    private CsvExtractorOptions<T> _options = new();

    private Encoding _encoding = System.Text.Encoding.UTF8;
    private IEtlPipeline<T>? _pipeline;


    private CsvExtractorBuilder(string? path, StreamReader? reader, CsvExtractor<T>? existing)
    {
        _path = path;
        _reader = reader;
        _existing = existing;
    }


    internal static ICsvExtractorBuilder<T> FromPath(string path) => new CsvExtractorBuilder<T>(path, reader: null, existing: null);


    internal static ICsvExtractorBuilder<T> FromReader(StreamReader reader) => new CsvExtractorBuilder<T>(path: null, reader, existing: null);


    internal static ICsvExtractorBuilder<T> FromExtractor(CsvExtractor<T> extractor) => new CsvExtractorBuilder<T>(path: null, reader: null, extractor);


#pragma warning disable CS0618
    // The builder is the supported replacement for these setters. It must still write them
    // directly for a caller-supplied instance, which has no constructor to route through.
    public ICsvExtractorBuilder<T> Delimiter(string delimiter)
    {
        if (delimiter is null)
        {
            throw new ArgumentNullException(nameof(delimiter));
        }

        return Set(o => o with { Delimiter = delimiter }, e => e.Delimiter = delimiter);
    }


    public ICsvExtractorBuilder<T> Quote(char quote) => Set(o => o with { Quote = quote }, e => e.Quote = quote);


    public ICsvExtractorBuilder<T> Escape(char escape) => Set(o => o with { Escape = escape }, e => e.Escape = escape);


    public ICsvExtractorBuilder<T> Comment(char comment) => Set(o => o with { Comment = comment }, e => e.Comment = comment);


    public ICsvExtractorBuilder<T> AllowComments(bool allow) => Set(o => o with { AllowComments = allow }, e => e.AllowComments = allow);


    public ICsvExtractorBuilder<T> IgnoreBlankLines(bool ignore) => Set(o => o with { IgnoreBlankLines = ignore }, e => e.IgnoreBlankLines = ignore);


    public ICsvExtractorBuilder<T> HasHeaderRecord(bool hasHeader) => Set(o => o with { HasHeaderRecord = hasHeader }, e => e.HasHeaderRecord = hasHeader);


    public ICsvExtractorBuilder<T> Encoding(Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        // Only _encoding matters: it is handed to the StreamReader this builder opens for a
        // path source, and that reader's encoding is authoritative. CsvExtractor.Encoding is inert
        // and obsolete, so it is deliberately not mirrored here.
        EnsureNotMaterialized();
        _encoding = encoding;
        return this;
    }


    public ICsvExtractorBuilder<T> InitialRecordIndex(int oneBasedIndex)
    {
        // Validate here so both routes (options record and pre-built extractor, whose mutation is
        // deferred to materialization) fail at the call, naming the caller's parameter.
        if (oneBasedIndex < 1)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(oneBasedIndex),
                oneBasedIndex,
                "InitialRecordIndex must be 1 or greater."
            );
        }

        return Set(o => o with { InitialRecordIndex = oneBasedIndex }, e => e.InitialRecordIndex = oneBasedIndex);
    }


    public ICsvExtractorBuilder<T> SkipRecordCount(int count) => Set(o => o with { SkipItemCount = count }, e => e.SkipItemCount = count);


    public ICsvExtractorBuilder<T> MaxRecordCount(int count) => Set(o => o with { MaximumItemCount = count }, e => e.MaximumItemCount = count);


    public ICsvExtractorBuilder<T> TrimOptions(CsvTrimOptions options) => Set(o => o with { TrimOptions = options }, e => e.TrimOptions = options);


    public ICsvExtractorBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps)
    {
        if (maps is null)
        {
            throw new ArgumentNullException(nameof(maps));
        }

        return Set(o => o with { ColumnMaps = maps }, e => e.ColumnMaps = maps);
    }


    public ICsvExtractorBuilder<T> BadDataFound(Action<CsvBadDataInfo> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        // Forwards to the deprecated observation callback for back-compat.
        return Configure(e => e.BadDataFound = handler);
    }


    public ICsvExtractorBuilder<T> ReadingExceptionOccurred(Action<CsvReadingExceptionInfo> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        // Forwards to the deprecated observation callback for back-compat.
        return Configure(e => e.ReadingExceptionOccurred = handler);
    }


#pragma warning restore CS0618


    public IEtlPipeline<TOut> Through<TOut>(ITransformAsync<T, TOut> transformer) where TOut : notnull => Pipeline().Through(transformer);


    public IEtlPipeline<TOut> Through<TOut>(ITransformWithCancellationAsync<T, TOut> transformer) where TOut : notnull => Pipeline().Through(transformer);


    public IEtlPipeline<TOut> Through<TOut>(Func<IAsyncEnumerable<T>, IAsyncEnumerable<TOut>> stage) where TOut : notnull => Pipeline().Through(stage);


    public IEtlPipeline<TOut> Through<TOut>(Func<IAsyncEnumerable<T>, CancellationToken, IAsyncEnumerable<TOut>> stage) where TOut : notnull => Pipeline().Through(stage);


    public IEtlPipelineSink To<TProgress>(LoaderBase<T, TProgress> loader) where TProgress : notnull => Pipeline().To(loader);


    public IAsyncEnumerable<T> AsAsyncEnumerable(CancellationToken token = default) => Pipeline().AsAsyncEnumerable(token);


    private void EnsureNotMaterialized()
    {
        if (_pipeline is not null)
        {
            throw new InvalidOperationException
            (
                "The extractor has already been materialized by a pipeline operator; configuration setters can no longer be applied."
            );
        }
    }


    /// <summary>
    /// Applies one configuration change by whichever route suits the source.
    /// </summary>
    /// <remarks>
    /// For a source this builder constructs, the change is folded into the options record handed
    /// to the constructor. For a caller-supplied extractor there is no constructor to route
    /// through, and applying the whole options record would reset properties the caller had
    /// already set directly — so only this single assignment is recorded.
    /// </remarks>
    private ICsvExtractorBuilder<T> Set
    (
        Func<CsvExtractorOptions<T>, CsvExtractorOptions<T>> update,
        Action<CsvExtractor<T>> mutate
    )
    {
        EnsureNotMaterialized();

        if (_existing is not null)
        {
            _mutations.Add(mutate);
        }
        else
        {
            _options = update(_options);
        }

        return this;
    }


    /// <summary>
    /// Records a post-construction mutation. Reserved for members deliberately absent from
    /// <see cref="CsvExtractorOptions{T}"/> — currently the two obsolete observation callbacks.
    /// </summary>
    private ICsvExtractorBuilder<T> Configure(Action<CsvExtractor<T>> mutation)
    {
        EnsureNotMaterialized();
        _mutations.Add(mutation);
        return this;
    }


    private IEtlPipeline<T> Pipeline()
    {
        if (_pipeline is not null)
        {
            return _pipeline;
        }

        if (_path is not null)
        {
            // Options the extractor constructor would reject fail here, at composition, as they
            // always have — the constructor itself now runs only when the pipeline is enumerated.
            CsvSchema.ValidateOptions(_options.Schema, _options.ColumnMaps, typeof(T), "options");

            return _pipeline = EtlPipeline.Create().From(ReadPathAsync());
        }

        return _pipeline = EtlPipeline.Create().From(BuildExtractor());
    }


    // Path-based source: the file is opened when enumeration starts and closed when it ends
    // (success, failure or abandonment), so composing a pipeline never holds a file handle and the
    // pipeline can be enumerated more than once. Setters are locked once the pipeline exists, so
    // _options / _encoding / _mutations are stable by the time this runs.
    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvExtractor construction is unreferenced-code; the public factory extension methods on EtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]
    private async IAsyncEnumerable<T> ReadPathAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        using var reader = new StreamReader(_path!, _encoding, detectEncodingFromByteOrderMarks: true);
        var extractor = new CsvExtractor<T>(reader, _options with { LeaveOpen = true });

        foreach (var mutation in _mutations)
        {
            mutation(extractor);
        }

        await foreach (var item in extractor.ExtractAsync(token).WithCancellation(token).ConfigureAwait(false))
        {
            yield return item;
        }
    }


    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvExtractor construction is unreferenced-code; the public factory extension methods on EtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]
    private CsvExtractor<T> BuildExtractor()
    {
        // The instance already exists for a caller-supplied extractor, so options cannot travel
        // through its constructor; the recorded fluent mutations are replayed onto it (see Set).
        var extractor = _existing ?? new CsvExtractor<T>(_reader!, _options);

        foreach (var mutation in _mutations)
        {
            mutation(extractor);
        }

        return extractor;
    }
}
