using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Loads records of type <typeparamref name="TRecord"/> into a CSV stream
/// using <see href="https://joshclose.github.io/CsvHelper/">CsvHelper</see>.
/// </summary>
/// <typeparam name="TRecord">The type of records to load. Must be <c>notnull</c>.</typeparam>
/// <example>
/// <code>
/// using var writer = new StreamWriter("people.csv");
/// var loader = new CsvLoader&lt;Person&gt;(writer);
/// await loader.LoadAsync(items, cancellationToken);
/// </code>
/// </example>
public sealed class CsvLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TRecord>
    : LoaderBase<TRecord, CsvLoaderProgress>, ISupportDryRun
    where TRecord : notnull
{
    private static readonly string OperationName = $"CSV loading of {typeof(TRecord).Name}";

    private readonly StreamWriter _writer;
    private readonly ILogger _logger;
    private readonly IProgressTimer? _progressTimer;
    private int _progressTimerWired;

    private int _currentLineNumber;
    private int _currentInvalidItemCount;
    private int _currentRecordNumber;



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvLoader{TRecord}"/> class.
    /// </summary>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to write CSV data to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="streamWriter"/> is <c>null</c>.</exception>
    [RequiresUnreferencedCode("CsvLoader uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public getters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvLoader
    (
        StreamWriter streamWriter
    )
    {
        _writer = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        _logger = NullLogger.Instance;
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvLoader{TRecord}"/> class with diagnostic logging.
    /// </summary>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to write CSV data to.</param>
    /// <param name="logger">
    /// An optional logger instance for diagnostic output. When <c>null</c> — or omitted —
    /// <see cref="NullLogger.Instance"/> is used and logging is disabled.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="streamWriter"/> is <c>null</c>.
    /// </exception>
    [RequiresUnreferencedCode("CsvLoader uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public getters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvLoader
    (
        StreamWriter streamWriter,
        ILogger<CsvLoader<TRecord>>? logger = null
    )
    {
        _writer = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        _logger = logger ?? (ILogger)NullLogger.Instance;
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvLoader{TRecord}"/> class with an
    /// injected progress timer for testing.
    /// </summary>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to write CSV data to.</param>
    /// <param name="logger">An optional logger instance for diagnostic output.</param>
    /// <param name="timer">The progress timer to inject.</param>
    internal CsvLoader
    (
        StreamWriter streamWriter,
        ILogger? logger,
        IProgressTimer timer
    )
    {
        _writer = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
        _logger = logger ?? NullLogger.Instance;
        _progressTimer = timer ?? throw new ArgumentNullException(nameof(timer));
    }



    /// <summary>Gets or sets the field delimiter. Default is <c>","</c>.</summary>
    public string Delimiter { get; set; } = ",";



    /// <summary>Gets or sets the character used to escape the quote character within a field.</summary>
    public char Escape { get; set; } = '"';



    /// <summary>
    /// Gets or sets the encoding forwarded to CsvHelper's writer configuration.
    /// </summary>
    /// <remarks>
    /// This value is informational only — it is passed to CsvHelper's
    /// <c>CsvConfiguration.Encoding</c> but does <b>not</b> control how characters are
    /// encoded into bytes on the output stream. Byte encoding is performed by the
    /// <see cref="StreamWriter"/> supplied to the constructor, and that writer's
    /// <see cref="StreamWriter.Encoding"/> is authoritative. To write a non-default
    /// encoding, construct the <see cref="StreamWriter"/> with the encoding you want
    /// and ignore this property.
    /// </remarks>
    public Encoding Encoding { get; set; } = Encoding.UTF8;



    /// <summary>Gets or sets a value indicating whether a header record should be written.</summary>
    public bool HasHeaderRecord { get; set; } = true;



    /// <summary>
    /// Gets or sets a value indicating whether the underlying stream should be left open
    /// after the writer is disposed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c> because the caller owns the <see cref="StreamWriter"/>
    /// passed into the constructor.
    /// </remarks>
    public bool LeaveOpen { get; set; } = true;



    /// <summary>Gets or sets the line terminator written between records.</summary>
    public string NewLine { get; set; } = "\r\n";



    /// <summary>Gets or sets the quote character used to wrap fields when needed.</summary>
    public char Quote { get; set; } = '"';



    /// <summary>
    /// Gets or sets a callback that decides whether a field should be quoted.
    /// When <c>null</c>, the underlying parser's default policy is used.
    /// </summary>
    public Func<CsvShouldQuoteContext, bool>? ShouldQuote { get; set; }



    /// <summary>Gets or sets the trimming options applied while writing.</summary>
    public CsvTrimOptions TrimOptions { get; set; } = CsvTrimOptions.None;



    /// <summary>
    /// Gets or sets a runtime column-map collection that overrides any
    /// <see cref="CsvColumnAttribute"/> / <see cref="CsvIgnoreAttribute"/> decorations
    /// on <typeparamref name="TRecord"/>.
    /// </summary>
    /// <remarks>
    /// Use this property when the CSV layout isn't known at compile time. When non-null
    /// and non-empty, the runtime maps are the only source of property-to-column
    /// bindings; attribute-based mapping is bypassed.
    /// </remarks>
    public IReadOnlyList<CsvColumnMap>? ColumnMaps { get; set; }



    /// <summary>
    /// When set, each record is written using a per-type mapping chosen by its runtime type, so a
    /// single file can mix multiple <typeparamref name="TRecord"/> shapes — the write-side mirror of
    /// <see cref="CsvExtractor{TRecord}.Discriminator"/>. Build one with
    /// <see cref="CsvDiscriminatorBuilder{TBase}"/> (trim/AOT-safe). No header row is written while a
    /// discriminator is set (the shapes have no common header); a record whose runtime type is not
    /// mapped is handled per <see cref="CsvDiscriminator{TBase}.UnknownDiscriminator"/>.
    /// </summary>
    public CsvDiscriminator<TRecord>? Discriminator { get; set; }



    /// <summary>
    /// Optional per-record validators run before each record is written. A record that fails one or
    /// more of them is counted in <see cref="CsvLoaderProgress.CurrentInvalidItemCount"/>, passed to
    /// <see cref="InvalidRecordHandler"/>, and then handled per <see cref="OnValidationFailure"/>.
    /// </summary>
    public IReadOnlyList<CsvValidator<TRecord>>? Validators { get; set; }



    /// <summary>
    /// How a record that fails validation is handled. Defaults to <see cref="CsvValidationFailureAction.Stop"/>
    /// (the first invalid record raises a <see cref="CsvValidationException"/>).
    /// </summary>
    public CsvValidationFailureAction OnValidationFailure { get; set; } = CsvValidationFailureAction.Stop;



    /// <summary>
    /// Optional callback invoked for each record that fails validation, before <see cref="OnValidationFailure"/>
    /// is applied. Use it to log or quarantine invalid rows.
    /// </summary>
    public Action<CsvInvalidRecord<TRecord>>? InvalidRecordHandler { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether the load runs as a dry run. When <c>true</c>,
    /// the loader enumerates the source and honors <see cref="SkipRecordCount"/> /
    /// <see cref="MaxRecordCount"/>, increments progress counters, fires progress reports, and
    /// logs exactly as a real load would — but writes nothing to the underlying writer (neither
    /// the header nor any records). Use it to validate a pipeline against real data without
    /// producing output. Defaults to <c>false</c>.
    /// </summary>
    public bool IsDryRun { get; set; }



    /// <summary>
    /// Gets or sets the number of records to skip before writing.
    /// This is an alias for <see cref="LoaderBase{TDestination,TProgress}.SkipItemCount"/>.
    /// </summary>
    public int SkipRecordCount
    {
        get => SkipItemCount;
        set => SkipItemCount = value;
    }



    /// <summary>
    /// Gets or sets the maximum number of records to write.
    /// This is an alias for <see cref="LoaderBase{TDestination,TProgress}.MaximumItemCount"/>.
    /// </summary>
    public int MaxRecordCount
    {
        get => MaximumItemCount;
        set => MaximumItemCount = value;
    }



    private CsvConfiguration BuildConfiguration()
    {
        var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            Delimiter = Delimiter,
            Escape = Escape,
            Encoding = Encoding,
            NewLine = NewLine,
            Quote = Quote,
            TrimOptions = TrimOptions.ToCsvHelper(),
        };

        var callerShouldQuote = ShouldQuote;
        if (callerShouldQuote is not null)
        {
            configuration.ShouldQuote = args => callerShouldQuote
            (
                new CsvShouldQuoteContext(args.Field, args.FieldType)
            );
        }

        return configuration;
    }



    /// <inheritdoc />
    protected override async Task LoadWorkerAsync
    (
        IAsyncEnumerable<TRecord> items,
        CancellationToken token
    )
    {
        // LoaderBase null-checks `items` in all public LoadAsync overloads before
        // invoking this worker, so a null is unreachable here.

        CsvLogMessages.StartingOperation(_logger, OperationName, null);

        // The `await using var` form doesn't compose with ConfigureAwait(false) —
        // there's no way to ConfigureAwait the implicit DisposeAsync that runs at
        // scope exit. Split into construction + explicit ConfiguredAsyncDisposable
        // so the disposal continuation never captures the caller's
        // SynchronizationContext (real deadlock risk on net462/netstandard targets
        // where sync-over-async consumers exist in the wild).
        var csvWriter = new CsvWriter(_writer, BuildConfiguration(), LeaveOpen);
        await using var _csvWriterDisposal = csvWriter.ConfigureAwait(false);

        RegisterRecordMap(csvWriter.Context);

        await WriteHeaderIfNeededAsync(csvWriter).ConfigureAwait(false);

        // Honor a token that is already cancelled before we pull the first record
        // from the source — mirrors CsvExtractor's top-of-loop check so a
        // pre-cancelled load reads nothing (LoaderBase contract, TestKit 0.13).
        token.ThrowIfCancellationRequested();

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(false))
        {
            token.ThrowIfCancellationRequested();

            // 1-based ordinal of the record in the input sequence — the loader writes rather than reads,
            // so this is the meaningful "where did this record come from" for an invalid record.
            var recordNumber = ++_currentRecordNumber;

            if (CurrentSkippedItemCount < SkipItemCount)
            {
                IncrementCurrentSkippedItemCount();
                CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
                continue;
            }

            if (CurrentItemCount >= MaximumItemCount)
            {
                CsvLogMessages.ReachedMaximumItemCount(_logger, MaximumItemCount, null);
                break;
            }

            if (!await TryWriteRecordAsync(csvWriter, item, recordNumber).ConfigureAwait(false))
            {
                continue;
            }

            IncrementCurrentItemCount();
            CsvLogMessages.LoadedItem(_logger, CurrentItemCount, null);
        }

        await csvWriter.FlushAsync().ConfigureAwait(false);

        CsvLogMessages.LoadingCompleted(_logger, CurrentItemCount, CurrentSkippedItemCount, null);
    }



    // Validates, then writes a record. Returns false (the caller skips it, not counting it as loaded)
    // when validation drops it under Skip, or when a discriminator can't map its runtime type under Skip.
    private async Task<bool> TryWriteRecordAsync(CsvWriter csvWriter, TRecord item, int recordNumber)
    {
        if (!PassesValidationGate(item, recordNumber))
        {
            return false;
        }

        // A discriminator writes each record by its runtime type; an unmapped type is skipped,
        // written as the base shape, or raises, per UnknownDiscriminator.
        if (!TryResolveWriteMode(item, out var writeAsBase))
        {
            IncrementCurrentSkippedItemCount();
            CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
            return false;
        }

        await WriteRecordAsync(csvWriter, item, writeAsBase).ConfigureAwait(false);
        return true;
    }



    // Applies the validators to a record. Returns true to write it (valid, or invalid under Continue);
    // false to skip it (invalid under Skip). Throws under Stop. Invalid records are always counted and
    // routed to InvalidRecordHandler first.
    private bool PassesValidationGate(TRecord item, int recordNumber)
    {
        if (TryValidate(item, recordNumber, out var invalid))
        {
            return true;
        }

        _ = Interlocked.Increment(ref _currentInvalidItemCount);
        InvalidRecordHandler?.Invoke(invalid!);

        switch (OnValidationFailure)
        {
            case CsvValidationFailureAction.Skip:
                return false;

            case CsvValidationFailureAction.Stop:
                throw new CsvValidationException(invalid!.LineNumber, invalid.Failures);

            default:
                return true;
        }
    }



    private async Task WriteRecordAsync(CsvWriter csvWriter, TRecord item, bool writeAsBase)
    {
        // Dry run: skip the physical write; counters, progress, and logging still fire.
        if (IsDryRun)
        {
            return;
        }

        if (writeAsBase)
        {
            csvWriter.WriteRecord(item);
        }
        else
        {
            WriteRecordByRuntimeType(csvWriter, item);
        }

        await csvWriter.NextRecordAsync().ConfigureAwait(false);
        UpdateLineNumber(csvWriter);
    }



    // Runs every configured validator against the record, aggregating failures. Returns true when
    // the record is valid (or no validators are configured); otherwise false with the failure detail.
    private bool TryValidate(TRecord item, int recordNumber, out CsvInvalidRecord<TRecord>? invalid)
    {
        invalid = null;

        var validators = Validators;
        if (validators is null || validators.Count == 0)
        {
            return true;
        }

        List<string>? failures = null;
        foreach (var validator in validators)
        {
            var result = validator(item);
            if (!result.IsValid)
            {
                failures ??= new List<string>();
                if (result.Failures is not null)
                {
                    failures.AddRange(result.Failures);
                }
            }
        }

        if (failures is null)
        {
            return true;
        }

        // Validation runs before the write, so report the input record's ordinal (not the last-written
        // line), and hand over a snapshot the handler / CsvValidationException observer can't mutate.
        invalid = new CsvInvalidRecord<TRecord>(item, recordNumber, failures.ToArray());
        return false;
    }



    private async Task WriteHeaderIfNeededAsync(CsvWriter csvWriter)
    {
        // A dry run writes nothing to the output — not even the header. A polymorphic write has no
        // single header shared by every record type, so it is headerless as well.
        if (!HasHeaderRecord || IsDryRun || Discriminator is not null)
        {
            return;
        }

        csvWriter.WriteHeader<TRecord>();
        await csvWriter.NextRecordAsync().ConfigureAwait(false);
        UpdateLineNumber(csvWriter);
    }



    private void UpdateLineNumber(CsvWriter csvWriter)
    {
        // Use Volatile.Write so the timer thread that calls CreateProgressReport
        // (which uses Volatile.Read on this field) sees a consistent snapshot.
        Volatile.Write(ref _currentLineNumber, csvWriter.Row);
    }



    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "TRecord is annotated with PublicProperties; CsvClassMapFactory reflects only public properties of TRecord.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "TRecord is annotated with PublicProperties; CsvClassMapFactory reflects only public properties of TRecord.")]
    private void RegisterRecordMap(CsvContext context)
    {
        // Always register the base TRecord map (from ColumnMaps or attributes). When a discriminator is
        // set this backs CsvDiscriminatorAction.YieldAsBase — writing an unmapped record as TRecord then
        // uses the intended base mapping instead of CsvHelper's default conventions. The map is null for
        // an attribute-less type with no ColumnMaps, in which case CsvHelper auto-maps.
        var baseMap = ColumnMaps is { Count: > 0 }
            ? CsvClassMapFactory.BuildFromColumnMaps<TRecord>(ColumnMaps)
            : CsvClassMapFactory.GetMap<TRecord>();

        if (baseMap is not null)
        {
            context.RegisterClassMap(baseMap);
        }

        Discriminator?.RegisterClassMaps(context);
    }



    // Decides how a record is written under the active discriminator. Returns false when the
    // record's runtime type is unmapped and the policy is Skip (the caller skips it). Otherwise
    // returns true, with writeAsBase set when the plain WriteRecord<TRecord> path applies — either
    // there is no discriminator, or the type is unmapped under YieldAsBase. Throws under Throw.
    private bool TryResolveWriteMode(TRecord item, out bool writeAsBase)
    {
        if (Discriminator is null)
        {
            writeAsBase = true;
            return true;
        }

        if (Discriminator.TryResolveValue(item.GetType(), out _))
        {
            writeAsBase = false;
            return true;
        }

        switch (Discriminator.UnknownDiscriminator)
        {
            case CsvDiscriminatorAction.Skip:
                writeAsBase = false;
                return false;

            case CsvDiscriminatorAction.YieldAsBase:
                writeAsBase = true;
                return true;

            default:
                throw new InvalidOperationException
                (
                    $"No discriminator value is mapped for record type '{item.GetType()}'."
                );
        }
    }



    // CsvWriter.WriteRecord<T> binds the map from the static T, so writing a mixed file requires
    // dispatching to the record's runtime type. Cache the open generic method once, and each closed
    // generic per runtime type so high-volume loads don't re-pay MakeGenericMethod per row.
    private static readonly MethodInfo WriteRecordMethod =
        typeof(CsvWriter).GetMethod(nameof(CsvWriter.WriteRecord))!;

    private static readonly ConcurrentDictionary<Type, MethodInfo> WriteRecordByType = new();



    private void WriteRecordByRuntimeType(CsvWriter csvWriter, TRecord item)
    {
        var type = item.GetType();

        // Prefer the builder-captured write delegate — CsvWriter.WriteRecord<T> rooted at a compile-time
        // T, so this path is trim/AOT-safe. Only the direct-init discriminator (no captured writers) needs
        // the reflective fallback below.
        if (Discriminator is not null && Discriminator.TryGetWriter(type, out var writer))
        {
            writer(csvWriter, item);
            return;
        }

        WriteRecordByRuntimeTypeReflective(csvWriter, item, type);
    }



    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Only reached for a direct-init CsvDiscriminator (no builder-captured writer); CsvLoader is already [RequiresUnreferencedCode].")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Only reached for a direct-init CsvDiscriminator; the builder path uses a statically-rooted write delegate and does not hit MakeGenericMethod.")]
    private static void WriteRecordByRuntimeTypeReflective(CsvWriter csvWriter, object item, Type type)
    {
        var method = WriteRecordByType.GetOrAdd(type, static t => WriteRecordMethod.MakeGenericMethod(t));

        try
        {
            _ = method.Invoke(csvWriter, new[] { item });
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            // Surface the CsvHelper exception, not the reflection wrapper, preserving its stack.
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        }
    }



    /// <inheritdoc />
    protected override CsvLoaderProgress CreateProgressReport() =>
        new
        (
            CurrentItemCount,
            CurrentSkippedItemCount,
            Volatile.Read(ref _currentLineNumber),
            Volatile.Read(ref _currentInvalidItemCount)
        );



    /// <inheritdoc />
    protected override IProgressTimer CreateProgressTimer(IProgress<CsvLoaderProgress> progress)
    {
        if (_progressTimer is not null)
        {
            if (Interlocked.CompareExchange(ref _progressTimerWired, 1, 0) == 0)
            {
                _progressTimer.Elapsed += () => progress.Report(CreateProgressReport());
            }

            return _progressTimer;
        }

        return base.CreateProgressTimer(progress);
    }
}
