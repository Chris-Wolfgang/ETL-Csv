using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Options for the <see cref="CsvExtractor{TRecord}"/> constructors.
/// </summary>
/// <remarks>
/// Supplied as the second constructor parameter, ahead of the optional logger. When the whole
/// options object is <see langword="null"/>, or an individual property is left unset, the
/// documented defaults below apply — defaults live on the property initializers here rather than
/// in constructor bodies, so no constructor can accidentally diverge from them.
/// <para>
/// This record deliberately carries no <c>Encoding</c> property. Decoding is performed by the
/// <see cref="System.IO.StreamReader"/> supplied to the constructor, whose
/// <see cref="System.IO.StreamReader.CurrentEncoding"/> is authoritative; the former
/// <see cref="CsvExtractor{TRecord}.Encoding"/> property never influenced it and is
/// <see cref="ObsoleteAttribute">obsolete</see>. To read a non-default encoding, construct the
/// <see cref="System.IO.StreamReader"/> with the encoding you want.
/// </para>
/// <para>
/// <c>BadDataFound</c> and <c>ReadingExceptionOccurred</c> are intentionally absent: both are
/// already <see cref="ObsoleteAttribute">obsolete</see> on <see cref="CsvExtractor{TRecord}"/>,
/// superseded by the unified error policy. Carrying them onto a new options record would
/// reintroduce deprecated surface.
/// </para>
/// </remarks>
/// <typeparam name="TRecord">The record type the extractor produces.</typeparam>
public sealed record CsvExtractorOptions<TRecord>
    where TRecord : notnull
{
    /// <summary>
    /// Gets a value indicating whether lines beginning with <see cref="Comment"/> are treated as
    /// comments and skipped. Defaults to <see langword="false"/>.
    /// </summary>
    public bool AllowComments { get; init; }





    /// <summary>
    /// Gets the character that marks a comment line when <see cref="AllowComments"/> is
    /// <see langword="true"/>. Defaults to <c>'#'</c>.
    /// </summary>
    public char Comment { get; init; } = '#';



    /// <summary>
    /// Gets the field delimiter. Defaults to <c>","</c>.
    /// </summary>
    public string Delimiter { get; init; } = ",";



    /// <summary>
    /// Gets the escape character. Defaults to <c>'"'</c>.
    /// </summary>
    public char Escape { get; init; } = '"';



    /// <summary>
    /// Gets a value indicating whether the first line is treated as a header record.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool HasHeaderRecord { get; init; } = true;



    /// <summary>
    /// Gets a value indicating whether blank lines are skipped rather than surfaced as records.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool IgnoreBlankLines { get; init; } = true;



    /// <summary>
    /// Gets a value indicating whether the supplied reader is left open when the extractor is
    /// disposed. Defaults to <see langword="true"/>.
    /// </summary>
    public bool LeaveOpen { get; init; } = true;



    /// <summary>
    /// Gets the quote character. Defaults to <c>'"'</c>.
    /// </summary>
    public char Quote { get; init; } = '"';



    /// <summary>
    /// Gets the whitespace-trimming behaviour applied to parsed fields. Defaults to
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
    /// <see langword="null"/>, meaning every row maps to <typeparamref name="TRecord"/>.
    /// </summary>
    public CsvDiscriminator<TRecord>? Discriminator { get; init; }



    /// <summary>
    /// Gets the validators applied to each extracted record. Defaults to <see langword="null"/>,
    /// meaning no validation.
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
    /// Gets the one-based index of the first record to read. When left unset the extractor's
    /// default of <c>1</c> applies.
    /// </summary>
    /// <remarks>
    /// Nullable so that "unset" is distinguishable from an explicit value. The underlying property
    /// rejects values below <c>1</c>, so a non-nullable option defaulting to <c>0</c> would make
    /// every options-constructed extractor throw.
    /// </remarks>
    public int? InitialRecordIndex { get; init; }



    /// <summary>
    /// Gets the number of records to skip before extracting. Alias for
    /// <c>ExtractorBase.SkipItemCount</c>. When left unset the base default applies.
    /// </summary>
    public int? SkipRecordCount { get; init; }



    /// <summary>
    /// Gets the maximum number of records to extract. Alias for
    /// <c>ExtractorBase.MaximumItemCount</c>. When left unset the base default applies.
    /// </summary>
    public int? MaxRecordCount { get; init; }
}
