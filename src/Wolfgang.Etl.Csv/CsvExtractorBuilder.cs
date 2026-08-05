using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
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


    public ICsvExtractorBuilder<T> Delimiter(string delimiter)
    {
        if (delimiter is null)
        {
            throw new ArgumentNullException(nameof(delimiter));
        }

        return Configure(e => e.Delimiter = delimiter);
    }


    public ICsvExtractorBuilder<T> Quote(char quote) => Configure(e => e.Quote = quote);


    public ICsvExtractorBuilder<T> Escape(char escape) => Configure(e => e.Escape = escape);


    public ICsvExtractorBuilder<T> Comment(char comment) => Configure(e => e.Comment = comment);


    public ICsvExtractorBuilder<T> AllowComments(bool allow) => Configure(e => e.AllowComments = allow);


    public ICsvExtractorBuilder<T> IgnoreBlankLines(bool ignore) => Configure(e => e.IgnoreBlankLines = ignore);


    public ICsvExtractorBuilder<T> HasHeaderRecord(bool hasHeader) => Configure(e => e.HasHeaderRecord = hasHeader);


    public ICsvExtractorBuilder<T> Encoding(Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        _encoding = encoding;
        return Configure(e => e.Encoding = encoding);
    }


    public ICsvExtractorBuilder<T> InitialRecordIndex(int oneBasedIndex) => Configure(e => e.InitialRecordIndex = oneBasedIndex);


    public ICsvExtractorBuilder<T> SkipRecordCount(int count) => Configure(e => e.SkipRecordCount = count);


    public ICsvExtractorBuilder<T> MaxRecordCount(int count) => Configure(e => e.MaxRecordCount = count);


    public ICsvExtractorBuilder<T> TrimOptions(CsvTrimOptions options) => Configure(e => e.TrimOptions = options);


    public ICsvExtractorBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps)
    {
        if (maps is null)
        {
            throw new ArgumentNullException(nameof(maps));
        }

        return Configure(e => e.ColumnMaps = maps);
    }


    public ICsvExtractorBuilder<T> BadDataFound(Action<CsvBadDataInfo> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

#pragma warning disable CS0618 // forwards to the deprecated observation callback for back-compat.
        return Configure(e => e.BadDataFound = handler);
#pragma warning restore CS0618
    }


    public ICsvExtractorBuilder<T> ReadingExceptionOccurred(Action<CsvReadingExceptionInfo> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

#pragma warning disable CS0618 // forwards to the deprecated observation callback for back-compat.
        return Configure(e => e.ReadingExceptionOccurred = handler);
#pragma warning restore CS0618
    }


    public IEtlPipeline<TOut> Through<TOut>(ITransformAsync<T, TOut> transformer) where TOut : notnull => Pipeline().Through(transformer);


    public IEtlPipeline<TOut> Through<TOut>(ITransformWithCancellationAsync<T, TOut> transformer) where TOut : notnull => Pipeline().Through(transformer);


    public IEtlPipeline<TOut> Through<TOut>(Func<IAsyncEnumerable<T>, IAsyncEnumerable<TOut>> stage) where TOut : notnull => Pipeline().Through(stage);


    public IEtlPipeline<TOut> Through<TOut>(Func<IAsyncEnumerable<T>, CancellationToken, IAsyncEnumerable<TOut>> stage) where TOut : notnull => Pipeline().Through(stage);


    public IEtlPipelineSink To<TProgress>(LoaderBase<T, TProgress> loader) where TProgress : notnull => Pipeline().To(loader);


    public IAsyncEnumerable<T> AsAsyncEnumerable(CancellationToken token = default) => Pipeline().AsAsyncEnumerable(token);


    private ICsvExtractorBuilder<T> Configure(Action<CsvExtractor<T>> mutation)
    {
        if (_pipeline is not null)
        {
            throw new InvalidOperationException
            (
                "The extractor has already been materialized by a pipeline operator; configuration setters can no longer be applied."
            );
        }

        _mutations.Add(mutation);
        return this;
    }


    private IEtlPipeline<T> Pipeline() => _pipeline ??= EtlPipeline.Create().From(BuildExtractor());


    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvExtractor construction is unreferenced-code; the public factory extension methods on EtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]
    private CsvExtractor<T> BuildExtractor()
    {
        CsvExtractor<T> extractor;

        if (_existing is not null)
        {
            extractor = _existing;
        }
        else if (_reader is not null)
        {
            extractor = new CsvExtractor<T>(_reader);
        }
        else
        {
            // Path-based source: the builder owns the reader it opens, so LeaveOpen is false and
            // the CsvReader disposes it when enumeration finishes (success or failure).
            var reader = new StreamReader(_path!, _encoding, detectEncodingFromByteOrderMarks: true);
            extractor = new CsvExtractor<T>(reader) { LeaveOpen = false };
        }

        foreach (var mutation in _mutations)
        {
            mutation(extractor);
        }

        return extractor;
    }
}
