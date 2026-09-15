using System;
using System.Collections.Generic;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Options for the <see cref="CsvLoader{TRecord}"/> constructors.
/// </summary>
/// <remarks>
/// Supplied as the second constructor parameter, ahead of the optional logger. When the whole
/// options object is <see langword="null"/>, or an individual property is left unset, the
/// documented defaults below apply — defaults live on the property initializers here rather than
/// in constructor bodies, so no constructor can accidentally diverge from them.
/// <para>
/// This record deliberately carries no <c>Encoding</c> property. Byte encoding is performed by the
/// <see cref="System.IO.StreamWriter"/> supplied to the constructor, whose
/// <see cref="System.IO.StreamWriter.Encoding"/> is authoritative; the former
/// <see cref="CsvLoader{TRecord}.Encoding"/> property never influenced it and is
/// <see cref="ObsoleteAttribute">obsolete</see>. To write a non-default encoding, construct the
/// <see cref="System.IO.StreamWriter"/> with the encoding you want.
/// </para>
/// </remarks>
/// <typeparam name="TRecord">The record type the loader writes.</typeparam>
public sealed record CsvLoaderOptions<TRecord> : LoaderOptions
    where TRecord : notnull
{
    /// <summary>
    /// Gets the field delimiter. Defaults to <c>","</c>.
    /// </summary>
    public string Delimiter { get; init; } = ",";



    /// <summary>
    /// Gets the escape character. Defaults to <c>'"'</c>.
    /// </summary>
    public char Escape { get; init; } = '"';



    /// <summary>
    /// Gets a value indicating whether a header record is written before the data rows.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool HasHeaderRecord { get; init; } = true;



    /// <summary>
    /// Gets a value indicating whether the supplied writer is left open when the loader is
    /// disposed. Defaults to <see langword="true"/>.
    /// </summary>
    public bool LeaveOpen { get; init; } = true;



    /// <summary>
    /// Gets the newline sequence written between records. Defaults to <c>"\r\n"</c>.
    /// </summary>
    public string NewLine { get; init; } = "\r\n";



    /// <summary>
    /// Gets the quote character. Defaults to <c>'"'</c>.
    /// </summary>
    public char Quote { get; init; } = '"';



    /// <summary>
    /// Gets a predicate deciding whether a given field is quoted. Defaults to
    /// <see langword="null"/>, meaning CsvHelper's default quoting rules apply.
    /// </summary>
    public Func<CsvShouldQuoteContext, bool>? ShouldQuote { get; init; }



    /// <summary>
    /// Gets the whitespace-trimming behaviour applied to written fields. Defaults to
    /// <see cref="CsvTrimOptions.None"/>.
    /// </summary>
    public CsvTrimOptions TrimOptions { get; init; } = CsvTrimOptions.None;



    /// <summary>
    /// Gets the explicit column mappings. Defaults to <see langword="null"/>, meaning mapping is
    /// driven by attributes on <typeparamref name="TRecord"/>.
    /// </summary>
    public IReadOnlyList<CsvColumnMap>? ColumnMaps { get; init; }



    /// <summary>
    /// Gets the discriminator used to select a record shape per row. Defaults to
    /// <see langword="null"/>, meaning every record is written as <typeparamref name="TRecord"/>.
    /// </summary>
    public CsvDiscriminator<TRecord>? Discriminator { get; init; }



    /// <summary>
    /// Gets the validators applied to each record before it is written. Defaults to
    /// <see langword="null"/>, meaning no validation.
    /// </summary>
    public IReadOnlyList<CsvValidator<TRecord>>? Validators { get; init; }



    /// <summary>
    /// Gets the action taken when a record fails validation. Defaults to
    /// <see cref="CsvValidationFailureAction.Stop"/>.
    /// </summary>
    public CsvValidationFailureAction OnValidationFailure { get; init; } = CsvValidationFailureAction.Stop;



    /// <summary>
    /// Gets a callback invoked for each record that fails validation. Defaults to
    /// <see langword="null"/>, meaning no callback.
    /// </summary>
    public Action<CsvInvalidRecord<TRecord>>? InvalidRecordHandler { get; init; }


    /// <summary>
    /// Deprecated alias for <see cref="LoaderOptions.SkipItemCount"/>, which this record now inherits. Reads and writes
    /// forward to it; the two never diverge.
    /// </summary>
    [Obsolete("Configure SkipItemCount, inherited from LoaderOptions, instead. SkipRecordCount forwards to it and will be removed.")]
    public int SkipRecordCount
    {
        get => SkipItemCount;
        init => SkipItemCount = value;
    }



    /// <summary>
    /// Deprecated alias for <see cref="LoaderOptions.MaximumItemCount"/>, which this record now inherits. Reads and writes
    /// forward to it; the two never diverge.
    /// </summary>
    [Obsolete("Configure MaximumItemCount, inherited from LoaderOptions, instead. MaxRecordCount forwards to it and will be removed.")]
    public int MaxRecordCount
    {
        get => MaximumItemCount;
        init => MaximumItemCount = value;
    }



    /// <summary>
    /// Gets a value indicating whether the load runs as a dry run. When <see langword="true"/>, the
    /// loader enumerates the source, honours <see cref="SkipRecordCount"/> / <see cref="MaxRecordCount"/>,
    /// increments progress counters, fires progress reports and logs exactly as a real load would — but
    /// writes nothing to the underlying writer (neither the header nor any records). Use it to validate a
    /// pipeline against real data without producing output. Defaults to <see langword="false"/>.
    /// </summary>
    public bool IsDryRun { get; init; }
}
