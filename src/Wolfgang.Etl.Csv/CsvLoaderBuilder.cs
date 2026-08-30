using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Default <see cref="ICsvLoaderBuilder{T}"/> implementation. Records configuration up front, then
/// materializes a <see cref="CsvLoader{TRecord}"/> and terminates the upstream pipeline when
/// <see cref="RunAsync"/> is called.
/// </summary>
/// <typeparam name="T">The record type consumed by the loader.</typeparam>
internal sealed class CsvLoaderBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    : ICsvLoaderBuilder<T>
    where T : notnull
{
    private readonly IEtlPipeline<T> _pipeline;
    private readonly string? _path;
    private readonly StreamWriter? _writer;
    private readonly CsvLoader<T>? _existing;
    private readonly List<Action<CsvLoader<T>>> _mutations = new();
    private CsvLoaderOptions<T> _options = new();

    private Encoding _encoding = System.Text.Encoding.UTF8;


    private CsvLoaderBuilder(IEtlPipeline<T> pipeline, string? path, StreamWriter? writer, CsvLoader<T>? existing)
    {
        _pipeline = pipeline;
        _path = path;
        _writer = writer;
        _existing = existing;
    }


    internal static ICsvLoaderBuilder<T> FromPath(IEtlPipeline<T> pipeline, string path) => new CsvLoaderBuilder<T>(pipeline, path, writer: null, existing: null);


    internal static ICsvLoaderBuilder<T> FromWriter(IEtlPipeline<T> pipeline, StreamWriter writer) => new CsvLoaderBuilder<T>(pipeline, path: null, writer, existing: null);


    internal static ICsvLoaderBuilder<T> FromLoader(IEtlPipeline<T> pipeline, CsvLoader<T> loader) => new CsvLoaderBuilder<T>(pipeline, path: null, writer: null, loader);


#pragma warning disable CS0618
    // The builder is the supported replacement for these setters. It must still write them
    // directly for a caller-supplied instance, which has no constructor to route through.
    public ICsvLoaderBuilder<T> Delimiter(string delimiter)
    {
        if (delimiter is null)
        {
            throw new ArgumentNullException(nameof(delimiter));
        }

        return Set(o => o with { Delimiter = delimiter }, l => l.Delimiter = delimiter);
    }


    public ICsvLoaderBuilder<T> Quote(char quote) => Set(o => o with { Quote = quote }, l => l.Quote = quote);


    public ICsvLoaderBuilder<T> Escape(char escape) => Set(o => o with { Escape = escape }, l => l.Escape = escape);


    public ICsvLoaderBuilder<T> NewLine(string newLine)
    {
        if (newLine is null)
        {
            throw new ArgumentNullException(nameof(newLine));
        }

        return Set(o => o with { NewLine = newLine }, l => l.NewLine = newLine);
    }


    public ICsvLoaderBuilder<T> HasHeaderRecord(bool hasHeader) => Set(o => o with { HasHeaderRecord = hasHeader }, l => l.HasHeaderRecord = hasHeader);


    public ICsvLoaderBuilder<T> Encoding(Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        // Only _encoding matters: it is handed to the StreamWriter this builder opens for a
        // path sink, and that writer's encoding is authoritative. CsvLoader.Encoding is inert and
        // obsolete, so it is deliberately not mirrored here.
        _encoding = encoding;
        return this;
    }


    public ICsvLoaderBuilder<T> TrimOptions(CsvTrimOptions options) => Set(o => o with { TrimOptions = options }, l => l.TrimOptions = options);


    public ICsvLoaderBuilder<T> ShouldQuote(Func<CsvShouldQuoteContext, bool> shouldQuote)
    {
        if (shouldQuote is null)
        {
            throw new ArgumentNullException(nameof(shouldQuote));
        }

        return Set(o => o with { ShouldQuote = shouldQuote }, l => l.ShouldQuote = shouldQuote);
    }


    public ICsvLoaderBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps)
    {
        if (maps is null)
        {
            throw new ArgumentNullException(nameof(maps));
        }

        return Set(o => o with { ColumnMaps = maps }, l => l.ColumnMaps = maps);
    }


    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvLoader construction is unreferenced-code; the public terminator extension methods on IEtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]


#pragma warning restore CS0618


    public Task RunAsync
    (
        IProgress<EtlPipelineProgress>? progress = null,
        CancellationToken token = default
    )
    {
        CsvLoader<T> loader;
        StreamWriter? owned = null;

        if (_existing is not null)
        {
            // The instance already exists, so options cannot travel through its constructor;
            // apply them through the same code path the constructor uses.
            loader = _existing;
        }
        else if (_writer is not null)
        {
            loader = new CsvLoader<T>(_writer, _options);
        }
        else
        {
            // Path-based sink: the builder owns the writer it opens and disposes it after the run
            // (success or failure) via DisposingOwned, flushing the file to disk.
            owned = new StreamWriter(_path!, append: false, _encoding);
            loader = new CsvLoader<T>(owned, _options);
        }

        foreach (var mutation in _mutations)
        {
            mutation(loader);
        }

        var sink = _pipeline.To(loader);

        if (owned is not null)
        {
            sink = sink.DisposingOwned(owned);
        }

        return sink.RunAsync(progress, token);
    }


    /// <summary>
    /// Applies one configuration change by whichever route suits the sink.
    /// </summary>
    /// <remarks>
    /// For a sink this builder constructs, the change is folded into the options record handed to
    /// the constructor. For a caller-supplied loader there is no constructor to route through, and
    /// applying the whole options record would reset properties the caller had already set
    /// directly — so only this single assignment is recorded.
    /// </remarks>
    private ICsvLoaderBuilder<T> Set
    (
        Func<CsvLoaderOptions<T>, CsvLoaderOptions<T>> update,
        Action<CsvLoader<T>> mutate
    )
    {
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
    /// <see cref="CsvLoaderOptions{T}"/> — currently only <c>IsDryRun</c>, which implements an
    /// interface declaring a <see langword="set"/> accessor.
    /// </summary>
    private ICsvLoaderBuilder<T> Configure(Action<CsvLoader<T>> mutation)
    {
        _mutations.Add(mutation);
        return this;
    }
}
