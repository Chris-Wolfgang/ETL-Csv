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
    private bool _hasRun;


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


    public ICsvLoaderBuilder<T> IsDryRun(bool isDryRun) => Set(o => o with { IsDryRun = isDryRun }, l => l.IsDryRun = isDryRun);


    public ICsvLoaderBuilder<T> Encoding(Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        // Only _encoding matters: it is handed to the StreamWriter this builder opens for a
        // path sink, and that writer's encoding is authoritative. CsvLoader.Encoding is inert and
        // obsolete, so it is deliberately not mirrored here.
        EnsureNotRun();
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

#pragma warning restore CS0618



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
        // Mirrors CsvExtractorBuilder's materialization guard: configuration recorded after a run
        // would otherwise silently apply to the next run. The run itself is single-use too: the
        // loader and (for a path sink) its writer are consumed by the first run, and a second run
        // against the same destination would replace the first run's output.
        if (_hasRun)
        {
            throw new InvalidOperationException
            (
                "The loader has already been run; RunAsync can only be called once per builder."
            );
        }

        _hasRun = true;

        StreamWriter? owned = null;
        string? temporaryPath = null;
        IEtlPipelineSink sink;

        try
        {
            var loader = CreateLoader(ref owned, ref temporaryPath);

            foreach (var mutation in _mutations)
            {
                mutation(loader);
            }

            sink = _pipeline.To(loader);

            if (owned is not null)
            {
                sink = sink.DisposingOwned(owned);
            }
        }
        catch
        {
            // DisposingOwned has not taken the writer yet, so a throw above must release it here.
            // Nothing has been written to it, so the synchronous Dispose has no buffer to flush,
            // and this method is not async (DisposeAsync is also missing on net462/netstandard2.0).
#pragma warning disable CA1849
            owned?.Dispose();
#pragma warning restore CA1849
            DeleteQuietly(temporaryPath);
            throw;
        }

        return temporaryPath is null
            ? sink.RunAsync(progress, token)
            : RunAndCommitAsync(sink, _path!, temporaryPath, progress, token);
    }


    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "CsvLoader construction is unreferenced-code; the public terminator extension methods on IEtlPipeline that create this builder carry [RequiresUnreferencedCode]."
    )]
    private CsvLoader<T> CreateLoader(ref StreamWriter? owned, ref string? temporaryPath)
    {
        if (_existing is not null)
        {
            // The instance already exists, so options cannot travel through its constructor;
            // the recorded fluent mutations are replayed onto it by the caller (see Set).
            return _existing;
        }

        if (_writer is not null)
        {
            return new CsvLoader<T>(_writer, _options);
        }

        // Validate before opening: StreamWriter(append: false) truncates its target, so options the
        // constructor would reject must not cost the caller a file.
        CsvSchema.ValidateOptions(_options.Schema, _options.ColumnMaps, typeof(T), "options");

        // Path-based sink: the builder owns the writer it opens and disposes it after the run
        // (success or failure) via DisposingOwned, flushing the file to disk. The output goes to a
        // sibling temporary file that replaces the destination only when the run succeeds, so a run
        // that fails (for example because the source cannot be opened) or is cancelled leaves an
        // existing destination untouched.
        temporaryPath = TemporaryPathFor(_path!);
        owned = new StreamWriter(temporaryPath, append: false, _encoding);
        return new CsvLoader<T>(owned, _options);
    }


    private static string TemporaryPathFor(string path)
    {
        var full = Path.GetFullPath(path);
        return Path.Combine
        (
            Path.GetDirectoryName(full)!,
            Path.GetFileName(full) + "." + Guid.NewGuid().ToString("N") + ".tmp"
        );
    }


    private static async Task RunAndCommitAsync
    (
        IEtlPipelineSink sink,
        string path,
        string temporaryPath,
        IProgress<EtlPipelineProgress>? progress,
        CancellationToken token
    )
    {
        try
        {
            // DisposingOwned has closed the temporary file by the time this completes.
            await sink.RunAsync(progress, token).ConfigureAwait(false);

            if (File.Exists(path))
            {
                File.Replace(temporaryPath, path, destinationBackupFileName: null);
            }
            else
            {
                File.Move(temporaryPath, path);
            }
        }
        catch
        {
            DeleteQuietly(temporaryPath);
            throw;
        }
    }


    // Best-effort cleanup of the temporary file; the original failure is what the caller needs to see.
    private static void DeleteQuietly(string? temporaryPath)
    {
        if (temporaryPath is null)
        {
            return;
        }

        try
        {
            File.Delete(temporaryPath);
        }
        catch (IOException)
        {
            // Left behind only if the file is still locked; nothing more can be done here.
        }
        catch (UnauthorizedAccessException)
        {
            // As above.
        }
    }


    private void EnsureNotRun()
    {
        if (_hasRun)
        {
            throw new InvalidOperationException
            (
                "The loader has already been run; configuration setters can no longer be applied."
            );
        }
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
        EnsureNotRun();

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
}
