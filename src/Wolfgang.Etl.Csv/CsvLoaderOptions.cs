using System;
using System.Collections.Generic;

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
/// <para>
/// It also carries no <c>IsDryRun</c> property. That member implements
/// <see cref="Wolfgang.Etl.Abstractions.ISupportDryRun.IsDryRun"/>, which declares a
/// <see langword="set"/> accessor, so it cannot become <see langword="init"/>-only while that
/// interface stands. Set it on the loader after construction until the interface changes.
/// </para>
/// </remarks>
/// <typeparam name="TRecord">The record type the loader writes.</typeparam>
public sealed record CsvLoaderOptions<TRecord>
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
    /// Gets the number of records to skip before loading. Alias for
    /// <c>LoaderBase.SkipItemCount</c>. When left unset the base default applies.
    /// </summary>
    public int? SkipRecordCount { get; init; }



    /// <summary>
    /// Gets the maximum number of records to load. Alias for
    /// <c>LoaderBase.MaximumItemCount</c>. When left unset the base default applies.
    /// </summary>
    public int? MaxRecordCount { get; init; }
}
