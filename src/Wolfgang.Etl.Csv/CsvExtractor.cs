using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Extracts records of type <typeparamref name="TRecord"/> from a CSV stream
/// using <see href="https://joshclose.github.io/CsvHelper/">CsvHelper</see>.
/// </summary>
/// <typeparam name="TRecord">The type of records to extract. Must be <c>notnull</c>.</typeparam>
/// <remarks>
/// Enumerating <c>ExtractAsync</c> can throw: <see cref="CsvValidationException"/> when a record fails a
/// validator and <c>OnValidationFailure</c> is <see cref="CsvValidationFailureAction.Stop"/> (the default);
/// <see cref="InvalidOperationException"/> for an unmapped discriminator value when the discriminator's
/// <c>OnUnknown</c> is <see cref="CsvDiscriminatorAction.Throw"/>; and CsvHelper's own parse or
/// conversion exception when a row fails under the default fail-fast <c>ErrorPolicy</c>.
/// </remarks>
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

    // Lines consumed by the InitialRecordIndex preamble skip. The parser numbers rows from 1 after
    // the skip, so every reported line number adds this to stay a physical line in the source.
    private int _preambleLineCount;
    private int _currentBadDataCount;
    private int _currentInvalidItemCount;



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class.
    /// </summary>
    /// <remarks>
    /// Retained for binary compatibility with assemblies compiled before the optional-logger overload existed:
    /// <c>new CsvExtractor&lt;T&gt;(streamReader)</c> in such an assembly is bound to this exact signature, and removing it
    /// would fail at runtime with <see cref="MissingMethodException"/> with no compile-time signal. Hidden from IntelliSense;
    /// source code binds here too, so nothing changes for callers. New code has no reason to name this overload.
    /// </remarks>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <exception cref="ArgumentNullException"><paramref name="streamReader"/> is <c>null</c>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public setters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvExtractor
    (
        StreamReader streamReader
    )
        : this(streamReader, logger: null, options: null)
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class with diagnostic logging.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="logger">
    /// An optional logger instance for diagnostic output. When <c>null</c> — or omitted —
    /// <see cref="NullLogger.Instance"/> is used and logging is disabled.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="streamReader"/> is <c>null</c>.
    /// </exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public setters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvExtractor
    (
        StreamReader streamReader,
        ILogger<CsvExtractor<TRecord>>? logger = null
    )
        : this(streamReader, logger, options: null)
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class with an
    /// injected progress timer for testing.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="timer">The progress timer to inject.</param>
    /// <param name="logger">
    /// An optional logger instance for diagnostic output. When <c>null</c> — or omitted —
    /// <see cref="NullLogger.Instance"/> is used and logging is disabled.
    /// </param>
    internal CsvExtractor
    (
        StreamReader streamReader,
        IProgressTimer timer,
        ILogger? logger = null
    )
        : this(streamReader, logger, options: null)
    {
        _progressTimer = timer ?? throw new ArgumentNullException(nameof(timer));
    }



    /// <summary>
    /// The single initialization path for <see cref="CsvExtractor{TRecord}"/>. Every other
    /// constructor chains into this one, so the shared fields are assigned in exactly one place.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="logger">
    /// The logger to use, or <c>null</c> for <see cref="NullLogger.Instance"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="streamReader"/> is <c>null</c>.
    /// </exception>
    /// <param name="options">
    /// The options record forwarded to the base constructor, which applies the inherited
    /// <see cref="ExtractorOptions"/> settings before this class applies its own; <c>null</c> keeps the defaults.
    /// </param>
    private CsvExtractor
    (
        StreamReader streamReader,
        ILogger? logger,
        CsvExtractorOptions<TRecord>? options
    )
        : base(options)
    {
        _reader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
        _logger = logger ?? NullLogger.Instance;
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvExtractor{TRecord}"/> class configured from
    /// an options record.
    /// </summary>
    /// <param name="streamReader">The <see cref="StreamReader"/> to read CSV data from.</param>
    /// <param name="options">
    /// The configuration to apply. When <c>null</c>, the documented defaults apply.
    /// </param>
    /// <param name="logger">
    /// An optional logger instance for diagnostic output. When <c>null</c> — or omitted —
    /// <see cref="NullLogger.Instance"/> is used and logging is disabled.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="streamReader"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="options"/> sets both <c>Schema</c> and a non-empty <c>ColumnMaps</c>, or its
    /// <c>Schema</c> was built for a record type other than <typeparamref name="TRecord"/>.
    /// </exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over TRecord's members beyond what DynamicallyAccessedMembers can express (type converter constructors, non-public setters in some flows). The library is not trim/NativeAOT safe.")]
    public CsvExtractor
    (
        StreamReader streamReader,
        CsvExtractorOptions<TRecord>? options,
        ILogger<CsvExtractor<TRecord>>? logger = null
    )
        : this(streamReader, logger, options)
    {
        ApplyOptions(options);
    }



    /// <summary>
    /// Copies <paramref name="options"/> onto this instance. A <c>null</c> options object leaves
    /// every property at its default.
    /// </summary>
    /// <param name="options">The configuration to apply, or <c>null</c>.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="options"/> holds an invalid combination or value (see the message for the option at fault).
    /// </exception>
    internal void ApplyOptions(CsvExtractorOptions<TRecord>? options)
    {
        if (options is null)
        {
            return;
        }

        CsvSchema.ValidateOptions(options.Schema, options.ColumnMaps, typeof(TRecord), nameof(options));
        ValidateOptionValues(options);

        _allowComments = options.AllowComments;
        _comment = options.Comment;
        _delimiter = options.Delimiter;
        _escape = options.Escape;
        _hasHeaderRecord = options.HasHeaderRecord;
        _ignoreBlankLines = options.IgnoreBlankLines;
        _leaveOpen = options.LeaveOpen;
        _quote = options.Quote;
        _trimOptions = options.TrimOptions;
        _columnMaps = options.ColumnMaps;
        _schema = options.Schema;
        _discriminator = options.Discriminator;
        _validators = options.Validators;
        _onValidationFailure = options.OnValidationFailure;
        _invalidRecordHandler = options.InvalidRecordHandler;
        _initialRecordIndex = options.InitialRecordIndex;
    }



    // Rejects option values the parser would otherwise fail on mid-enumeration with an error that does
    // not name the option: a Delimiter, Quote or Comment CsvHelper 34 cannot use, and a name-based
    // Discriminator without a header row, whose lookups would fail on every record.
    private static void ValidateOptionValues(CsvExtractorOptions<TRecord> options)
    {
        CsvOptionRules.Validate(options.Delimiter, options.Quote, options.Escape, nameof(options));
        if (options.AllowComments)
        {
            _ = CsvOptionRules.ToAscii(options.Comment, nameof(options));
        }

        if (options.Discriminator is { ColumnName: not null } && !options.HasHeaderRecord)
        {
            throw new ArgumentException
            (
                "A Discriminator that selects its column by ColumnName requires HasHeaderRecord = true; use ColumnIndex without a header row.",
                nameof(options)
            );
        }
    }



    private bool _allowComments;

    /// <summary>Gets or sets a value indicating whether comment lines are allowed.</summary>
    public bool AllowComments { get => _allowComments; [Obsolete("Configure AllowComments through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _allowComments = value; }



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
    /// This hook is for observation only. <see cref="ExtractorBase{TSource, TProgress}.ErrorPolicy"/>
    /// decides skip-vs-abort: under the default fail-fast policy the exception propagates out of
    /// <c>ExtractAsync</c>; a <c>Skip</c> policy drops the row and extraction continues.
    /// <code>
    /// extractor.ReadingExceptionOccurred = info =>
    ///     _logger.LogDebug(info.Exception, "Parse error on line {Line} column {Col}: {Value}",
    ///         info.LineNumber, info.ColumnNumber, info.ColumnValue);
    /// </code>
    /// </remarks>
    [Obsolete("Deprecated in favor of the unified ErrorPolicy. Parse / type-conversion failures now route through HandleItemError so ErrorPolicy governs skip-vs-abort; ItemErrorContext.Exception carries the failure. This callback still fires for observation only.")]
    public Action<CsvReadingExceptionInfo>? ReadingExceptionOccurred { get; set; }



    private char _comment = '#';

    /// <summary>Gets or sets the character used to mark a comment line.</summary>
    public char Comment { get => _comment; [Obsolete("Configure Comment through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _comment = value; }



    private string _delimiter = ",";

    /// <summary>Gets or sets the field delimiter. Default is <c>","</c>.</summary>
    public string Delimiter { get => _delimiter; [Obsolete("Configure Delimiter through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _delimiter = value; }



    private char _escape = '"';

    /// <summary>Gets or sets the character used to escape the quote character within a field.</summary>
    public char Escape { get => _escape; [Obsolete("Configure Escape through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _escape = value; }



    private Encoding _encoding = Encoding.UTF8;

    /// <summary>
    /// Gets or sets an encoding that the extractor does not use.
    /// </summary>
    /// <remarks>
    /// This value is informational only — it does <b>not</b> control how bytes are
    /// decoded into characters. Decoding happens inside the <see cref="StreamReader"/>
    /// supplied to the constructor, and that reader's <see cref="StreamReader.CurrentEncoding"/>
    /// is authoritative. To read a non-default encoding, construct the
    /// <see cref="StreamReader"/> with the encoding you want and ignore this property.
    /// </remarks>
    public Encoding Encoding { get => _encoding; [Obsolete("This property is inert - it never controlled how bytes are decoded or encoded. Construct the StreamReader with the encoding you want. This setter will be removed in a future release.")] set => _encoding = value; }



    private bool _hasHeaderRecord = true;

    /// <summary>Gets or sets a value indicating whether the CSV has a header record.</summary>
    public bool HasHeaderRecord { get => _hasHeaderRecord; [Obsolete("Configure HasHeaderRecord through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _hasHeaderRecord = value; }



    private bool _ignoreBlankLines = true;

    /// <summary>Gets or sets a value indicating whether blank lines are skipped.</summary>
    public bool IgnoreBlankLines { get => _ignoreBlankLines; [Obsolete("Configure IgnoreBlankLines through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _ignoreBlankLines = value; }



    private int _initialRecordIndex = 1;

    /// <summary>
    /// Gets or sets the 1-based index of the first line the parser will consume from
    /// the source. Lines before this index are read and discarded.
    /// </summary>
    /// <remarks>
    /// The line at <see cref="InitialRecordIndex"/> is treated as the <i>first line read</i>,
    /// not necessarily the first data row. When <see cref="HasHeaderRecord"/> is <c>true</c>,
    /// it becomes the header row (column names); when <see cref="HasHeaderRecord"/> is
    /// <c>false</c>, it becomes the first data row. Set this to skip metadata or banner
    /// lines that precede the header / data section. The skipped lines are discarded as raw
    /// text, not parsed as CSV, so blank lines, comment lines and quote characters in them have no effect
    /// and each counts as exactly one line (a quoted field cannot span a line break inside the skipped block).
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">value is less than 1.</exception>
    public int InitialRecordIndex
    {
        get => _initialRecordIndex;
        [Obsolete("Configure InitialRecordIndex through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")]
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "InitialRecordIndex must be 1 or greater.");
            }

            _initialRecordIndex = value;
        }
    }



    private bool _leaveOpen = true;

    /// <summary>
    /// Gets or sets a value indicating whether the underlying stream should be left open
    /// after the parser is disposed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c> because the caller owns the <see cref="StreamReader"/>
    /// passed into the constructor.
    /// </remarks>
    public bool LeaveOpen { get => _leaveOpen; [Obsolete("Configure LeaveOpen through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _leaveOpen = value; }



    private char _quote = '"';

    /// <summary>Gets or sets the quote character used to wrap fields.</summary>
    public char Quote { get => _quote; [Obsolete("Configure Quote through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _quote = value; }



    private CsvTrimOptions _trimOptions = CsvTrimOptions.None;

    /// <summary>Gets or sets the trimming options applied while reading.</summary>
    public CsvTrimOptions TrimOptions { get => _trimOptions; [Obsolete("Configure TrimOptions through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _trimOptions = value; }



    private IReadOnlyList<CsvColumnMap>? _columnMaps;

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
    public IReadOnlyList<CsvColumnMap>? ColumnMaps { get => _columnMaps; [Obsolete("Configure ColumnMaps through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _columnMaps = FrozenColumnMaps.Freeze(value); }



    private CsvSchema? _schema;

    /// <summary>
    /// Gets the code-first schema supplied through <see cref="CsvExtractorOptions{TRecord}.Schema"/>, or
    /// <see langword="null"/> when mapping is driven by <see cref="ColumnMaps"/> or attributes.
    /// </summary>
    public CsvSchema? Schema => _schema;



    private CsvDiscriminator<TRecord>? _discriminator;

    /// <summary>
    /// When set, each row is bound to a concrete record type chosen by a discriminator column, so a
    /// single file can mix multiple <typeparamref name="TRecord"/> shapes. Build one with
    /// <see cref="CsvDiscriminatorBuilder{TBase}"/> (trim/AOT-safe). When set, missing trailing fields
    /// are tolerated so narrower row shapes bind cleanly, and an unmapped discriminator value is handled
    /// per <see cref="CsvDiscriminator{TBase}.UnknownDiscriminator"/>.
    /// </summary>
    public CsvDiscriminator<TRecord>? Discriminator { get => _discriminator; [Obsolete("Configure Discriminator through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _discriminator = value; }



    private IReadOnlyList<CsvValidator<TRecord>>? _validators;

    /// <summary>
    /// Optional per-record validators run after each row is bound. A record that fails one or more of
    /// them is counted in <see cref="CsvExtractorProgress.CurrentInvalidItemCount"/>, passed to
    /// <see cref="InvalidRecordHandler"/>, and then handled per <see cref="OnValidationFailure"/>.
    /// </summary>
    public IReadOnlyList<CsvValidator<TRecord>>? Validators { get => _validators; [Obsolete("Configure Validators through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _validators = value; }



    private CsvValidationFailureAction _onValidationFailure = CsvValidationFailureAction.Stop;

    /// <summary>
    /// How a record that fails validation is handled. Defaults to <see cref="CsvValidationFailureAction.Stop"/>
    /// (the first invalid record raises a <see cref="CsvValidationException"/>).
    /// </summary>
    public CsvValidationFailureAction OnValidationFailure { get => _onValidationFailure; [Obsolete("Configure OnValidationFailure through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _onValidationFailure = value; }



    private Action<CsvInvalidRecord<TRecord>>? _invalidRecordHandler;

    /// <summary>
    /// Optional callback invoked for each record that fails validation, before <see cref="OnValidationFailure"/>
    /// is applied. Use it to log or quarantine invalid rows.
    /// </summary>
    public Action<CsvInvalidRecord<TRecord>>? InvalidRecordHandler { get => _invalidRecordHandler; [Obsolete("Configure InvalidRecordHandler through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")] set => _invalidRecordHandler = value; }



    /// <summary>
    /// Gets or sets the number of records to skip before yielding results.
    /// This is an alias for <see cref="ExtractorBase{TSource,TProgress}.SkipItemCount"/>.
    /// </summary>
    public int SkipRecordCount
    {
        get => SkipItemCount;
        [Obsolete("Configure SkipItemCount through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")]
        set => SkipItemCount = value;
    }



    /// <summary>
    /// Gets or sets the maximum number of records to extract.
    /// This is an alias for <see cref="ExtractorBase{TSource,TProgress}.MaximumItemCount"/>.
    /// </summary>
    public int MaxRecordCount
    {
        get => MaximumItemCount;
        [Obsolete("Configure MaximumItemCount through CsvExtractorOptions<TRecord> passed to the constructor instead. This setter will be removed in a future release.")]
        set => MaximumItemCount = value;
    }



    private CsvReaderOptions BuildReaderOptions(CultureInfo culture)
    {
#pragma warning disable CS0618 // ErrorPolicy supersedes the decision; the callback is read for observation only.
        var callerBadDataFound = BadDataFound;
#pragma warning restore CS0618

        var separator = CsvOptionRules.ToSeparator(Delimiter);
        var quote = CsvOptionRules.ToQuote(Quote, Escape);

        return new CsvReaderOptions
        {
            CommentPrefix = AllowComments ? CsvOptionRules.ToAscii(Comment, nameof(Comment)) : null,
            // Malformed quoting is counted and reported, and the record is kept: the library's
            // "bad data is tolerated" contract. Records of differing widths are allowed (a polymorphic
            // file mixes them); a field a mapped member needs but the record lacks is the binding's error.
            FieldCountMismatch = CsvFieldCountHandling.Allow,
            FormatProvider = culture,
            HasHeader = HasHeaderRecord,
            IncludeValuesInExceptions = true,
            MalformedData = CsvMalformedDataHandling.Recover,
            MissingField = CsvMissingFieldHandling.ReturnEmpty,
            OnMalformedRecord = record =>
            {
                _ = Interlocked.Increment(ref _currentBadDataCount);
                callerBadDataFound?.Invoke(ToCsvBadDataInfo(record, separator, quote));
            },
            Quote = quote,
            Separator = separator,
            SkipEmptyLines = IgnoreBlankLines,
            TrackLineNumbers = true,
            TrimInsideQuotes = (TrimOptions & CsvTrimOptions.InsideQuotes) == CsvTrimOptions.InsideQuotes,
            TrimWhitespace = (TrimOptions & CsvTrimOptions.Trim) == CsvTrimOptions.Trim,
        };
    }



    private CsvBadDataInfo ToCsvBadDataInfo(CsvMalformedRecord record, char separator, char quote)
    {
        // CsvHelper 34 reports the malformed record, not the field, so the field is found here by
        // re-scanning the raw record. A malformed header has no column (-1), as with CsvHelper 33.
        var column = -1;
        var field = record.RawRecord;
        if (!HasHeaderRecord || record.RecordNumber != 1)
        {
            column = CsvMalformedField.Find(record.RawRecord, separator, quote, out var badField);
            field = column >= 0 ? badField : record.RawRecord;
        }

        return new CsvBadDataInfo
        (
            (int)record.LineNumber + CsvMalformedField.CountLineBreaks(record.RawRecord) + _preambleLineCount,
            column >= 0 ? column + 1 : -1,
            field,
            record.RawRecord
        );
    }



    // Fires the deprecated observation callback for a record the binding could not read; the caller then
    // routes the failure through the ErrorPolicy (which owns the skip-vs-abort decision).
    private void OnReadingException(CsvReader csvReader, CsvBindingReader bindingReader, Exception exception)
    {
#pragma warning disable CS0618 // observation-only; ErrorPolicy owns the decision.
        var callback = ReadingExceptionOccurred;
#pragma warning restore CS0618
        if (callback is null)
        {
            return;
        }

        var column = bindingReader.FailedColumn;
        callback
        (
            new CsvReadingExceptionInfo
            (
                CurrentLine(csvReader),
                column >= 0 ? column + 1 : -1,
                bindingReader.FailedColumnName,
                bindingReader.FailedValue,
                exception
            )
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

        ResetRunCounters();

        var culture = CultureInfo.CurrentCulture;
        var options = BuildReaderOptions(culture);

        await SkipPreambleAsync().ConfigureAwait(false);

        var csvReader = CsvReader.Create(_reader, options, LeaveOpen);
        try
        {
            var readers = await PrepareReaderAsync(csvReader, culture, token).ConfigureAwait(false);

            // Read one record at a time so MaximumItemCount is checked BEFORE the next row is parsed
            // and converted (an enumerator would read the (N+1)th row before the limit could stop it).
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
                    hasRow = await csvReader.ReadAsync(token).ConfigureAwait(false);
                }
                catch (CsvFormatException ex)
                {
                    // Parse-level failure while reading the raw row. Route through the ErrorPolicy so it
                    // governs the same skip-vs-abort decision as a mapping failure below.
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

                TRecord record;
                bool unknownSkip;
                var bindingReader = readers.Base;
                try
                {
                    if (Discriminator is not null)
                    {
                        record = GetDiscriminatedRecord(csvReader, readers, out unknownSkip, out bindingReader)!;
                    }
                    else
                    {
                        record = (TRecord)bindingReader.Read(csvReader);
                        unknownSkip = false;
                    }
                }
                catch (CsvFormatException ex)
                {
                    // Type-conversion / mapping failure. The ErrorPolicy owns skip-vs-abort: Skip
                    // continues to the next row (CurrentErrorItemCount++); the default fail-fast policy
                    // aborts — re-throw preserving the original stack.
                    OnReadingException(csvReader, bindingReader, ex);
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

                if (!TryValidate(record, out var invalid))
                {
                    _ = Interlocked.Increment(ref _currentInvalidItemCount);
                    InvalidRecordHandler?.Invoke(invalid);

                    if (OnValidationFailure == CsvValidationFailureAction.Skip)
                    {
                        continue;
                    }

                    if (OnValidationFailure == CsvValidationFailureAction.Stop)
                    {
                        throw new CsvValidationException(invalid.LineNumber, invalid.Failures);
                    }

                    // Continue: fall through and yield the invalid record anyway.
                }

                IncrementCurrentItemCount();
                CsvLogMessages.ExtractedItem(_logger, CurrentItemCount, null);

                yield return record;
            }
        }
        finally
        {
            // Disposing returns the reader's pooled buffers; it closes the StreamReader only when
            // LeaveOpen is false.
            await csvReader.DisposeAsync().ConfigureAwait(false);
        }

        CsvLogMessages.ExtractionCompleted(_logger, CurrentItemCount, CurrentSkippedItemCount, null);
    }



    // Routes a failed row through the base ErrorPolicy and returns its decision. On Skip,
    // HandleItemError has already incremented CurrentErrorItemCount; the caller re-throws
    // on Abort so the stack is preserved. The ItemErrorContext ordinal is the physical line the
    // record starts on, not a record ordinal: a CSV row can span several lines, and the line is
    // what a caller needs to find the row in the source. CsvExtractorProgress.CurrentLineNumber and
    // CsvBadDataInfo use the same numbering.
    private ItemErrorAction RouteItemError(CsvReader csvReader, Exception exception)
    {
        var rawRow = Encoding.UTF8.GetString(csvReader.RawRecord.ToArray());
        return HandleItemError(new ItemErrorContext(CurrentLine(csvReader), exception, () => rawRow));
    }



    // Discards the lines before InitialRecordIndex (1-based) on the raw reader, before the CSV parser
    // sees them: banner text is not CSV, so a blank line, a comment line or a stray quote in it must
    // not be interpreted (a blank line would swallow the next record, a quote the rest of the file).
    private async Task SkipPreambleAsync()
    {
        _preambleLineCount = 0;

        while (_preambleLineCount < InitialRecordIndex - 1
            && await _reader.ReadLineAsync().ConfigureAwait(false) is not null)
        {
            _preambleLineCount++;
            CsvLogMessages.IgnoredRow(_logger, _preambleLineCount, null);
        }

        Volatile.Write(ref _currentLineNumber, _preambleLineCount);
    }



    // The readers of one run: the base TRecord's, and, with a Discriminator, one per mapped type,
    // created when the type is first read.
    private sealed class RunReaders
    {
        public RunReaders(CsvBindingReader baseReader, IReadOnlyDictionary<Type, CsvRecordBinding>? bindings, bool hasHeader, CultureInfo culture)
        {
            Base = baseReader;
            Bindings = bindings;
            HasHeader = hasHeader;
            Culture = culture;
        }



        public CsvBindingReader Base { get; }



        public IReadOnlyDictionary<Type, CsvRecordBinding>? Bindings { get; }



        public bool HasHeader { get; }



        public CultureInfo Culture { get; }



        public Dictionary<Type, CsvBindingReader> ByType { get; } = new();
    }



    private async Task<RunReaders> PrepareReaderAsync(CsvReader csvReader, CultureInfo culture, CancellationToken token)
    {
        // Read the header record, if present. The base binding is resolved against it: a missing
        // required column fails here, before any record is read — unless a Discriminator is set, whose
        // header describes the discriminator column and each row type, not the base type's members.
        var headerRead = false;
        if (HasHeaderRecord)
        {
            await csvReader.ReadHeaderAsync(token).ConfigureAwait(false);

            // Empty input has no header row to validate (or line to report).
            headerRead = csvReader.Headers.Count > 0;
            if (headerRead)
            {
                Volatile.Write(ref _currentLineNumber, CurrentLine(csvReader, csvReader.LineNumber > 0 ? csvReader.LineNumber : 1));
            }
        }

        var bindings = Discriminator?.GetBindings();
        var baseReader = new CsvBindingReader
        (
            GetBaseBinding(),
            csvReader,
            HasHeaderRecord,
            validateHeader: headerRead && Discriminator is null,
            tolerateMissingFields: Discriminator is not null,
            culture
        );
        var readers = new RunReaders(baseReader, bindings, HasHeaderRecord, culture);

        // Honour SkipItemCount.
        while (CurrentSkippedItemCount < SkipItemCount && await csvReader.ReadAsync(token).ConfigureAwait(false))
        {
            UpdateLineNumber(csvReader);
            IncrementCurrentSkippedItemCount();
            CsvLogMessages.SkippedItem(_logger, CurrentSkippedItemCount, SkipItemCount, null);
        }

        return readers;
    }



    // The physical line the current record ends on, as CsvHelper 33's RawRow counted it: the line it
    // starts on (CsvHelper 34's LineNumber) plus the line breaks inside its quoted fields, plus the
    // InitialRecordIndex preamble the parser never saw.
    private int CurrentLine(CsvReader csvReader) =>
        CurrentLine(csvReader, csvReader.LineNumber);



    private int CurrentLine(CsvReader csvReader, long startLine) =>
        (int)startLine + CsvMalformedField.CountLineBreaks(csvReader.RawRecord) + _preambleLineCount;



    private void UpdateLineNumber(CsvReader csvReader)
    {
        // Use Volatile.Write so the timer thread that calls CreateProgressReport
        // (which uses Volatile.Read on this field) sees a consistent snapshot.
        Volatile.Write(ref _currentLineNumber, CurrentLine(csvReader));
    }



    // Reads the discriminator field for the current row, resolves the concrete type, and binds the
    // record to it. Type-conversion failures surface as CsvFormatException (routed through ErrorPolicy
    // by the caller). An unmapped value follows Discriminator.UnknownDiscriminator: Skip returns
    // (default, true); YieldAsBase binds to TRecord; Throw raises a hard error that bypasses ErrorPolicy.
    private TRecord? GetDiscriminatedRecord(CsvReader csvReader, RunReaders readers, out bool skip, out CsvBindingReader bindingReader)
    {
        skip = false;
        bindingReader = readers.Base;
        var discriminator = Discriminator!;

        var index = discriminator.ColumnIndex;
        if (discriminator.ColumnName is not null && !csvReader.TryGetFieldIndex(discriminator.ColumnName, out index))
        {
            index = -1;
        }

        var value = index >= 0 && index < csvReader.FieldCount ? csvReader.GetString(index) : null;

        if (value is not null && discriminator.TryResolveType(value, out var type))
        {
            if (!readers.ByType.TryGetValue(type, out var typeReader))
            {
                typeReader = new CsvBindingReader
                (
                    readers.Bindings![type],
                    csvReader,
                    readers.HasHeader,
                    validateHeader: false,
                    tolerateMissingFields: true,
                    readers.Culture
                );
                readers.ByType.Add(type, typeReader);
            }

            bindingReader = typeReader;
            return (TRecord)typeReader.Read(csvReader);
        }

        switch (discriminator.UnknownDiscriminator)
        {
            case CsvDiscriminatorAction.Skip:
                skip = true;
                return default;

            case CsvDiscriminatorAction.YieldAsBase:
                return (TRecord)readers.Base.Read(csvReader);

            default:
                throw new InvalidOperationException
                (
                    $"No record type is mapped for discriminator value '{value}'."
                );
        }
    }



    // The base TRecord layout, from Schema, ColumnMaps, or attributes. With a discriminator it backs
    // CsvDiscriminatorAction.YieldAsBase. The requirement cannot move further out: the only caller is
    // ExtractWorkerAsync, an override of an unannotated ExtractorBase member, where
    // [RequiresUnreferencedCode] would be IL2046. It is surfaced instead at every public CsvExtractor
    // constructor (and the EtlPipeline factory extensions), so no instance reaches this method without
    // the consumer seeing the warning.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Reached only through a CsvExtractor instance, whose public constructors carry [RequiresUnreferencedCode]; TRecord is annotated with PublicProperties and the binding reflects only over public members of TRecord.")]
    private CsvRecordBinding GetBaseBinding() =>
        CsvRecordBindingFactory.Resolve(typeof(TRecord), _schema is not null ? _schema.Columns : ColumnMaps);



    private void ResetRunCounters()
    {
        Volatile.Write(ref _currentLineNumber, 0);
        Volatile.Write(ref _currentBadDataCount, 0);
        Volatile.Write(ref _currentInvalidItemCount, 0);
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
    private bool TryValidate(TRecord record, [NotNullWhen(false)] out CsvInvalidRecord<TRecord>? invalid)
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
                // Failures is non-nullable, but a validator can still return `with { Failures = null! }`;
                // skip it rather than throw from AddRange.
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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
