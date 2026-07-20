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


    public ICsvLoaderBuilder<T> Delimiter(string delimiter)
    {
        if (delimiter is null)
        {
            throw new ArgumentNullException(nameof(delimiter));
        }

        return Configure(l => l.Delimiter = delimiter);
    }


    public ICsvLoaderBuilder<T> Quote(char quote) => Configure(l => l.Quote = quote);


    public ICsvLoaderBuilder<T> Escape(char escape) => Configure(l => l.Escape = escape);


    public ICsvLoaderBuilder<T> NewLine(string newLine)
    {
        if (newLine is null)
        {
            throw new ArgumentNullException(nameof(newLine));
        }

        return Configure(l => l.NewLine = newLine);
    }


    public ICsvLoaderBuilder<T> HasHeaderRecord(bool hasHeader) => Configure(l => l.HasHeaderRecord = hasHeader);


    public ICsvLoaderBuilder<T> Encoding(Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        _encoding = encoding;
        return Configure(l => l.Encoding = encoding);
    }


    public ICsvLoaderBuilder<T> TrimOptions(CsvTrimOptions options) => Configure(l => l.TrimOptions = options);


    public ICsvLoaderBuilder<T> ShouldQuote(Func<CsvShouldQuoteContext, bool> shouldQuote)
    {
        if (shouldQuote is null)
        {
            throw new ArgumentNullException(nameof(shouldQuote));
        }

        return Configure(l => l.ShouldQuote = shouldQuote);
    }


    public ICsvLoaderBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps)
    {
        if (maps is null)
        {
            throw new ArgumentNullException(nameof(maps));
        }

        return Configure(l => l.ColumnMaps = maps);
    }


    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvLoader construction is unreferenced-code; the public terminator extension methods on IEtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]
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
            loader = _existing;
        }
        else if (_writer is not null)
        {
            loader = new CsvLoader<T>(_writer);
        }
        else
        {
            // Path-based sink: the builder owns the writer it opens and disposes it after the run
            // (success or failure) via DisposingOwned, flushing the file to disk.
            owned = new StreamWriter(_path!, append: false, _encoding);
            loader = new CsvLoader<T>(owned);
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


    private ICsvLoaderBuilder<T> Configure(Action<CsvLoader<T>> mutation)
    {
        _mutations.Add(mutation);
        return this;
    }
}
