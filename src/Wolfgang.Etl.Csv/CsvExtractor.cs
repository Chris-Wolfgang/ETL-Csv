using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
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
/// Extracts records of type <typeparamref name="TRecord"/> from a CSV stream
/// using <see href="https://joshclose.github.io/CsvHelper/">CsvHelper</see>.
/// </summary>
/// <typeparam name="TRecord">The type of records to extract. Must be <c>notnull</c>.</typeparam>
/// <example>
/// <code>
/// using var reader = new StreamReader("people.csv");
/// var extractor = new CsvExtractor&lt;Person&gt;(reader);
/// await foreach (var person in extractor.ExtractAsync(cancellationToken))
/// {
///     Console.WriteLine(person.Name);
/// }
/// </code>
/// </example>
public sealed class CsvExtractor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TRecord>
    : ExtractorBase<TRecord, CsvExtractorProgress>
    where TRecord : notnull
{
    private static readonly string OperationName = $"CSV extraction of {typeof(TRecord).Name}";

    private readonly StreamReader _reader;
    private readonly ILogger _logger;
    private readonly IProgressTimer? _progressTimer;
    private int _progressTimerWired;

    private int _currentLineNumber;
    private int _currentBadDataCount;
    private int _currentInvalidItemCount;



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <exception cref="ArgumentNullException"><paramref name="streamReader"/> is <c>null</c>.</exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public setters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvExtractor
    (
        StreamReader streamReader
    )
    {
        _reader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _logger = NullLogger.Instance;
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class with diagnostic logging.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="streamReader"/> or <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public setters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvExtractor
    (
        StreamReader streamReader,
        ILogger<CsvExtractor<TRecord>> logger
    )
    {
        _reader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class with an
    /// injected progress timer for testing.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="logger">An optional logger instance for diagnostic output.</param>
    /// <param name="timer">The progress timer to inject.</param>
    internal CsvExtractor
    (
        StreamReader streamReader,
        ILogger? logger,
        IProgressTimer timer
    )
    {
        _reader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _logger = logger ?? NullLogger.Instance;
        _progressTimer = timer ?? throw new ArgumentNullException(nameof(timer));
    }



    /// <summary>Gets or sets a value indicating whether comment lines are allowed.</summary>
    public bool AllowComments { get; set; }



    /// <summary>
    /// Gets or sets a callback invoked when the underlying parser detects bad data.
    /// Use this to log, count, or quarantine bad records as they're encountered.
    /// </summary>
    /// <remarks>
    /// When <c>null</c> (the default), bad data is silently counted via
    /// <see cref="CsvExtractorProgress.CurrentBadDataCount"/> but not logged — the
    /// library deliberately does not write CSV record contents to your logger,
    /// since CSV records frequently contain PII. Wire this to your logger if you
    /// want diagnostic output, e.g.:
    /// <code>
    /// extractor.BadDataFound = info => _logger.LogDebug("Bad CSV data on line {Line}: {Field}", info.LineNumber, info.Field);
    /// </code>
    /// Extraction always continues after a bad-data event. To abort, throw from
    /// the callback or trip the <see cref="System.Threading.CancellationToken"/>
    /// passed to <c>ExtractAsync</c>.
    /// </remarks>
    [Obsolete("Deprecated in favor of the unified ErrorPolicy for failure handling. This observation callback still fires (and CurrentBadDataCount still counts) when the parser reports malformed data; bad data remains tolerated, while parse/type-conversion failures are governed by ErrorPolicy.")]
    public Action<CsvBadDataInfo>? BadDataFound { get; set; }



    /// <summary>
    /// Gets or sets a callback invoked when the underlying parser raises a
    /// recoverable parse exception (typically a type-conversion failure on a
    /// specific field). Use this to log, count, or send to telemetry.
    /// </summary>
    /// <remarks>
    /// When <c>null</c> (the default), the exception is not logged by the library.
    /// The exception always propagates out of <c>ExtractAsync</c> regardless of
    /// the callback — this hook is for observation only. Wrap your <c>await foreach</c>
    /// in <c>try / catch</c> if you want to swallow individual row failures.
    /// <code>
    /// extractor.ReadingExceptionOccurred = info =>
    ///     _logger.LogDebug(info.Exception, "Parse error on line {Line} column {Col}: {Value}",
    ///         info.LineNumber, info.ColumnNumber, info.ColumnValue);
    /// </code>
    /// </remarks>
    [Obsolete("Deprecated in favor of the unified ErrorPolicy. Parse / type-conversion failures now route through HandleItemError so ErrorPolicy governs skip-vs-abort; ItemErrorContext.Exception carries the failure. This callback still fires for observation only.")]
    public Action<CsvReadingExceptionInfo>? ReadingExceptionOccurred { get; set; }



    /// <summary>Gets or sets the character used to mark a comment line.</summary>
    public char Comment { get; set; } = '#';



    /// <summary>Gets or sets the field delimiter. Default is <c>","</c>.</summary>
    public string Delimiter { get; set; } = ",";



    /// <summary>Gets or sets the character used to escape the quote character within a field.</summary>
    public char Escape { get; set; } = '"';



    /// <summary>
    /// Gets or sets the encoding forwarded to the underlying parser configuration.
    /// </summary>
    /// <remarks>
    /// This value is informational only — it is passed to CsvHelper's
    /// <c>CsvConfiguration.Encoding</c> but does <b>not</b> control how bytes are
    /// decoded into characters. Decoding happens inside the <see cref="StreamReader"/>
    /// supplied to the constructor, and that reader's <see cref="StreamReader.CurrentEncoding"/>
    /// is authoritative. To read a non-default encoding, construct the
    /// <see cref="StreamReader"/> with the encoding you want and ignore this property.
    /// </remarks>
    public Encoding Encoding { get; set; } = Encoding.UTF8;



    /// <summary>Gets or sets a value indicating whether the CSV has a header record.</summary>
    public bool HasHeaderRecord { get; set; } = true;



    /// <summary>Gets or sets a value indicating whether blank lines are skipped.</summary>
    public bool IgnoreBlankLines { get; set; } = true;



    /// <summary>
    /// Gets or sets the 1-based index of the first line the parser will consume from
    /// the source. Lines before this index are read and discarded.
    /// </summary>
    /// <remarks>
    /// The line at <see cref="InitialRecordIndex"/> is treated as the <i>first line read</i>,
    /// not necessarily the first data row. When <see cref="HasHeaderRecord"/> is <c>true</c>,
    /// it becomes the header row (column names); when <see cref="HasHeaderRecord"/> is
    /// <c>false</c>, it becomes the first data row. Set this to skip metadata or banner
    /// lines that precede the header / data section.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">value is less than 1.</exception>
    public int InitialRecordIndex
    {
        get => _initialRecordIndex;
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "InitialRecordIndex must be 1 or greater.");
            }

            _initialRecordIndex = value;
        }
    }
    private int _initialRecordIndex = 1;



    /// <summary>
    /// Gets or sets a value indicating whether the underlying stream should be left open
    /// after the parser is disposed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c> because the caller owns the <see cref="StreamReader"/>
    /// passed into the constructor.
    /// </remarks>
    public bool LeaveOpen { get; set; } = true;



    /// <summary>Gets or sets the quote character used to wrap fields.</summary>
    public char Quote { get; set; } = '"';



    /// <summary>Gets or sets the trimming options applied while reading.</summary>
    public CsvTrimOptions TrimOptions { get; set; } = CsvTrimOptions.None;



    /// <summary>
    /// Gets or sets a runtime column-map collection that overrides any
    /// <see cref="CsvColumnAttribute"/> / <see cref="CsvIgnoreAttribute"/> decorations
    /// on <typeparamref name="TRecord"/>.
    /// </summary>
    /// <remarks>
    /// Use this property when the CSV layout isn't known at compile time — for example
    /// when the column positions for a record type are loaded from configuration or a
    /// database "template" row. When non-null and non-empty, the runtime maps are the
    /// only source of property-to-column bindings; attribute-based mapping is bypassed.
    /// </remarks>
    public IReadOnlyList<CsvColumnMap>? ColumnMaps { get; set; }



    /// <summary>
    /// When set, each row is bound to a concrete record type chosen by a discriminator column, so a
    /// single file can mix multiple <typeparamref name="TRecord"/> shapes. Build one with
    /// <see cref="CsvDiscriminatorBuilder{TBase}"/> (trim/AOT-safe). When set, missing trailing fields
    /// are tolerated so narrower row shapes bind cleanly, and an unmapped discriminator value is handled
    /// per <see cref="CsvDiscriminator{TBase}.UnknownDiscriminator"/>.
    /// </summary>
    public CsvDiscriminator<TRecord>? Discriminator { get; set; }



    /// <summary>
    /// Optional per-record validators run after each row is bound. A record that fails one or more of
    /// them is counted in <see cref="CsvExtractorProgress.CurrentInvalidItemCount"/>, passed to
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
    /// Gets or sets the number of records to skip before yielding results.
    /// This is an alias for <see cref="ExtractorBase{TSource,TProgress}.SkipItemCount"/>.
    /// </summary>
    public int SkipRecordCount
    {
        get => SkipItemCount;
        set => SkipItemCount = value;
    }



    /// <summary>
    /// Gets or sets the maximum number of records to extract.
    /// This is an alias for <see cref="ExtractorBase{TSource,TProgress}.MaximumItemCount"/>.
    /// </summary>
    public int MaxRecordCount
    {
        get => MaximumItemCount;
        set => MaximumItemCount = value;
    }



    private CsvConfiguration BuildConfiguration()
    {
#pragma warning disable CS0618 // ErrorPolicy supersedes the decision; the callback is read for observation only.
        var callerBadDataFound = BadDataFound;
#pragma warning restore CS0618

        return new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            AllowComments = AllowComments,
            BadDataFound = args =>
            {
                _ = Interlocked.Increment(ref _currentBadDataCount);
                callerBadDataFound?.Invoke(ToCsvBadDataInfo(args));
            },
            Comment = Comment,
            Delimiter = Delimiter,
            Escape = Escape,
            Encoding = Encoding,
            HasHeaderRecord = HasHeaderRecord,
            // A polymorphic file's header describes the discriminator column, not the base type's members,
            // and each row is bound per concrete type — so don't validate the header against TRecord.
            HeaderValidated = Discriminator is not null ? null : ConfigurationFunctions.HeaderValidated,
            IgnoreBlankLines = IgnoreBlankLines,
            // A polymorphic file mixes row shapes of differing widths; tolerate missing trailing
            // fields so a narrower concrete type binds without tripping CsvHelper's default throw.
            MissingFieldFound = Discriminator is not null ? null : ConfigurationFunctions.MissingFieldFound,
            Quote = Quote,
            ReadingExceptionOccurred = OnReadingExceptionOccurred,
            TrimOptions = TrimOptions.ToCsvHelper(),
        };
    }



    private static CsvBadDataInfo ToCsvBadDataInfo(BadDataFoundArgs args)
    {
        // CsvHelper invokes BadDataFound from its parser/reader chain, so Context.Reader
        // and Context.Parser are guaranteed non-null here. Using `!` instead of `?.`
        // removes defensive null branches that are unreachable through the public API.
        var rawColumnIndex = args.Context.Reader!.CurrentIndex;
        var columnNumber = rawColumnIndex >= 0 ? rawColumnIndex + 1 : -1;

        return new CsvBadDataInfo
        (
            args.Context.Parser!.RawRow,
            columnNumber,
            args.Field,
            args.RawRecord
        );
    }



    private bool OnReadingExceptionOccurred(ReadingExceptionOccurredArgs args)
    {
        // Fire the deprecated observation callback, then return true so CsvHelper
        // rethrows out of GetRecord — the extract loop's try/catch routes the
        // failure through the ErrorPolicy (which owns the skip-vs-abort decision).
#pragma warning disable CS0618 // observation-only; ErrorPolicy owns the decision.
        ReadingExceptionOccurred?.Invoke(ToCsvReadingExceptionInfo(args));
#pragma warning restore CS0618
        return true;
    }



    private static CsvReadingExceptionInfo ToCsvReadingExceptionInfo(ReadingExceptionOccurredArgs args)
    {
        var ctx = args.Exception.Context;
        var columnIndex = ctx?.Reader?.CurrentIndex ?? -1;
        var headerRecord = ctx?.Reader?.HeaderRecord;
        var currentRecord = ctx?.Parser?.Record;

        // Bounds-check both index lookups. The original parsing exception is often
        // caused by a record with the wrong number of fields, and indexing into a
        // shorter record from the handler would itself throw IndexOutOfRangeException
        // and mask the underlying error.
        var columnName = headerRecord is not null && columnIndex >= 0 && columnIndex < headerRecord.Length
            ? headerRecord[columnIndex]
            : null;
        var columnValue = currentRecord is not null && columnIndex >= 0 && columnIndex < currentRecord.Length
            ? currentRecord[columnIndex]
            : null;
        var columnNumber = columnIndex >= 0 ? columnIndex + 1 : -1;

        return new CsvReadingExceptionInfo
        (
            ctx?.Parser?.RawRow ?? -1,
            columnNumber,
            columnName,
            columnValue,
            args.Exception
        );
    }



    /// <inheritdoc />
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Cohesive async extraction loop — the per-row read / error-policy / yield flow cannot be split across the yield boundary without hurting readability.")]
    protected override async IAsyncEnumerable<TRecord> ExtractWorkerAsync
    (
        [EnumeratorCancellation] CancellationToken token
    )
    {
        CsvLogMessages.StartingOperation(_logger, OperationName, null);

        var configuration = BuildConfiguration();

#pragma warning disable CA2007, MA0004
        using var csvReader = new CsvReader(_reader, configuration, LeaveOpen);
#pragma warning restore CA2007, MA0004

        RegisterRecordMap(csvReader.Context);

        await PrepareReaderAsync(csvReader).ConfigureAwait(false);

        // Manual ReadAsync / GetRecord loop instead of GetRecordsAsync so we can check
        // MaximumItemCount BEFORE materializing the next row. With GetRecordsAsync the
        // (N+1)th row would be parsed and type-converted (firing BadDataFound and
        // ReadingExceptionOccurred for it) before the limit check could stop us, and
        // MaximumItemCount = 0 would still consume one record.
        while (true)
        {
            token.ThrowIfCancellationRequested();

            if (CurrentItemCount >= MaximumItemCount)
            {
                CsvLogMessages.ReachedMaximumItemCount(_logger, MaximumItemCount, null);
                yield break;
            }

            bool hasRow;
            try
            {
                hasRow = await csvReader.ReadAsync().ConfigureAwait(false);
            }
            catch (CsvHelperException ex)
            {
                // Parse-level failure while reading the raw row. Route through the
                // ErrorPolicy so it governs the same skip-vs-abort decision as a
                // mapping failure below.
                if (RouteItemError(csvReader, ex) == ItemErrorAction.Abort)
                {
                    throw;
                }

                continue;
            }

            if (!hasRow)
            {
                break;
            }

            UpdateLineNumber(csvReader);

            TRecord? record;
            bool unknownSkip;
            try
            {
                if (Discriminator is not null)
                {
                    record = GetDiscriminatedRecord(csvReader, out unknownSkip);
                }
                else
                {
                    record = csvReader.GetRecord<TRecord>();
                    unknownSkip = false;
                }
            }
            catch (CsvHelperException ex)
            {
                // Type-conversion / mapping failure. The ErrorPolicy owns skip-vs-abort:
                // Skip continues to the next row (CurrentErrorItemCount++); the default
                // fail-fast policy aborts — re-throw preserving the original stack (the
                // prior behavior for an unhandled reading exception).
                if (RouteItemError(csvReader, ex) == ItemErrorAction.Abort)
                {
                    throw;
                }

                continue;
            }

            if (unknownSkip)
            {
                // An unmapped discriminator value under CsvDiscriminatorAction.Skip: count as
                // skipped so totals reconcile, then move on. (Throw surfaces as an exception
                // from GetDiscriminatedRecord; YieldAsBase returns a bound base record.)
                IncrementCurrentSkippedItemCount();
                CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
                continue;
            }

            if (record is null)
            {
                // CsvHelper can produce null for reference-typed TRecord when its
                // type converters resolve every column to null. Count as skipped
                // so totals reconcile against the raw row count instead of
                // silently dropping the row.
                IncrementCurrentSkippedItemCount();
                CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
                continue;
            }

            if (!TryValidate(record, out var invalid))
            {
                _ = Interlocked.Increment(ref _currentInvalidItemCount);
                InvalidRecordHandler?.Invoke(invalid!);

                if (OnValidationFailure == CsvValidationFailureAction.Skip)
                {
                    continue;
                }

                if (OnValidationFailure == CsvValidationFailureAction.Stop)
                {
                    throw new CsvValidationException(invalid!.LineNumber, invalid.Failures);
                }

                // Continue: fall through and yield the invalid record anyway.
            }

            IncrementCurrentItemCount();
            CsvLogMessages.ExtractedItem(_logger, CurrentItemCount, null);

            yield return record;
        }

        CsvLogMessages.ExtractionCompleted(_logger, CurrentItemCount, CurrentSkippedItemCount, null);
    }



    // Routes a failed row through the base ErrorPolicy and returns its decision. On Skip,
    // HandleItemError has already incremented CurrentErrorItemCount; the caller re-throws
    // on Abort so the stack is preserved.
    private ItemErrorAction RouteItemError(CsvReader csvReader, Exception exception)
    {
        var rawRow = csvReader.Parser.RawRecord;
        return HandleItemError(new ItemErrorContext(csvReader.Parser.RawRow, exception, () => rawRow));
    }



    private async Task PrepareReaderAsync(CsvReader csvReader)
    {
        // Skip lines before InitialRecordIndex (1-based).
        while (csvReader.Parser.RawRow < InitialRecordIndex - 1 && await csvReader.ReadAsync().ConfigureAwait(false))
        {
            UpdateLineNumber(csvReader);
            CsvLogMessages.IgnoredRow(_logger, csvReader.Parser.RawRow, null);
        }

        // Read the header record if present.
        if (HasHeaderRecord && await csvReader.ReadAsync().ConfigureAwait(false))
        {
            UpdateLineNumber(csvReader);
            csvReader.ReadHeader();
            csvReader.ValidateHeader<TRecord>();
        }

        // Honour SkipItemCount.
        while (CurrentSkippedItemCount < SkipItemCount && await csvReader.ReadAsync().ConfigureAwait(false))
        {
            UpdateLineNumber(csvReader);
            IncrementCurrentSkippedItemCount();
            CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
        }
    }



    private void UpdateLineNumber(CsvReader csvReader)
    {
        // Use Volatile.Write so the timer thread that calls CreateProgressReport
        // (which uses Volatile.Read on this field) sees a consistent snapshot.
        // Context.Parser is guaranteed non-null here — UpdateLineNumber is only
        // called after a successful ReadAsync(), which sets the Parser.
        Volatile.Write(ref _currentLineNumber, csvReader.Context.Parser!.RawRow);
    }



    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "TRecord is annotated with PublicProperties; CsvClassMapFactory reflects only public properties of TRecord.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "TRecord is annotated with PublicProperties; CsvClassMapFactory reflects only public properties of TRecord.")]
    // Reads the discriminator field for the current row, resolves the concrete type, and binds the
    // record to it. Type-conversion failures surface as CsvHelperException (routed through ErrorPolicy
    // by the caller). An unmapped value follows Discriminator.UnknownDiscriminator: Skip returns
    // (default, true); YieldAsBase binds to TRecord; Throw raises a hard error that bypasses ErrorPolicy.
    private TRecord? GetDiscriminatedRecord(CsvReader csvReader, out bool skip)
    {
        skip = false;
        var discriminator = Discriminator!;

        var value = discriminator.ColumnName is not null
            ? csvReader.GetField(discriminator.ColumnName)
            : csvReader.GetField(discriminator.ColumnIndex);

        if (value is not null && discriminator.TryResolveType(value, out var type))
        {
            return (TRecord?)csvReader.GetRecord(type);
        }

        switch (discriminator.UnknownDiscriminator)
        {
            case CsvDiscriminatorAction.Skip:
                skip = true;
                return default;

            case CsvDiscriminatorAction.YieldAsBase:
                return csvReader.GetRecord<TRecord>();

            default:
                throw new InvalidOperationException
                (
                    $"No record type is mapped for discriminator value '{value}'."
                );
        }
    }



    private void RegisterRecordMap(CsvContext context)
    {
        // Always register the base TRecord map (from ColumnMaps or attributes). When a discriminator
        // is set this backs CsvDiscriminatorAction.YieldAsBase — GetRecord<TRecord>() then binds
        // through the intended base mapping instead of CsvHelper's default conventions. The map is
        // null for an attribute-less type with no ColumnMaps, in which case CsvHelper auto-maps.
        var baseMap = ColumnMaps is { Count: > 0 }
            ? CsvClassMapFactory.BuildFromColumnMaps<TRecord>(ColumnMaps)
            : CsvClassMapFactory.GetMap<TRecord>();

        if (baseMap is not null)
        {
            context.RegisterClassMap(baseMap);
        }

        Discriminator?.RegisterClassMaps(context);
    }



    /// <inheritdoc />
    protected override CsvExtractorProgress CreateProgressReport() =>
        new
        (
            CurrentItemCount,
            CurrentSkippedItemCount,
            Volatile.Read(ref _currentLineNumber),
            Volatile.Read(ref _currentBadDataCount),
            CurrentErrorItemCount,
            Volatile.Read(ref _currentInvalidItemCount)
        );



    // Runs every configured validator against the bound record, aggregating failures. Returns true
    // when the record is valid (or no validators are configured); otherwise false with the failure
    // detail. The line number is the source row the record was read from.
    private bool TryValidate(TRecord record, out CsvInvalidRecord<TRecord>? invalid)
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
            var result = validator(record);
            if (!result.IsValid)
            {
                failures ??= new List<string>();
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                // CsvValidationResult.Failures is declared non-null, but the ctor takes it as a
                // primary-record parameter and a caller can pass `null!` to bypass the check.
                // ExtractAsync_tolerates_a_validator_that_returns_null_failures explicitly verifies
                // this defensive branch — removing it produces an ArgumentNullException from
                // List<string>.AddRange. Keep the runtime check even though the nullable contract
                // says it's redundant.
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

        // Hand the record a snapshot, not the live list, so a handler can't mutate what a later
        // observer (or the CsvValidationException) sees.
        invalid = new CsvInvalidRecord<TRecord>(record, Volatile.Read(ref _currentLineNumber), failures.ToArray());
        return false;
    }



    /// <inheritdoc />
    protected override IProgressTimer CreateProgressTimer(IProgress<CsvExtractorProgress> progress)
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
