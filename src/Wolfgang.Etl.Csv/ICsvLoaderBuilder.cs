using System;
using System.Collections.Generic;
using System.Text;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Fluent builder for a <see cref="CsvLoader{TRecord}"/> that terminates the generic
/// <see cref="EtlPipeline"/> chain. Extends <see cref="IEtlPipelineSink"/>, so the pipeline is
/// runnable as soon as the terminator is called; the fluent setters simply refine the loader
/// before <see cref="IEtlPipelineSink.RunAsync(IProgress{EtlPipelineProgress}, System.Threading.CancellationToken)"/>
/// materializes and runs it.
/// </summary>
/// <typeparam name="T">The record type consumed by the loader.</typeparam>
/// <remarks>
/// Each setter maps 1:1 to a public property on <see cref="CsvLoader{TRecord}"/>; no new
/// configuration surface is introduced. Stream ownership is decided by the terminator overload,
/// not a setter: a path-based sink owns and disposes the file it opens (on success and failure),
/// while a caller-supplied <see cref="System.IO.StreamWriter"/> is left open for the caller.
/// </remarks>
public interface ICsvLoaderBuilder<T> : IEtlPipelineSink
    where T : notnull
{
    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.Delimiter"/>. Default: <c>","</c>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="delimiter"/> is <see langword="null"/>.</exception>
    ICsvLoaderBuilder<T> Delimiter(string delimiter);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.Quote"/>. Default: <c>"</c>.
    /// </summary>
    ICsvLoaderBuilder<T> Quote(char quote);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.Escape"/>. Default: <c>"</c>.
    /// </summary>
    ICsvLoaderBuilder<T> Escape(char escape);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.NewLine"/>. Default: <c>\r\n</c>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="newLine"/> is <see langword="null"/>.</exception>
    ICsvLoaderBuilder<T> NewLine(string newLine);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.HasHeaderRecord"/>. Default: <see langword="true"/>.
    /// </summary>
    ICsvLoaderBuilder<T> HasHeaderRecord(bool hasHeader);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.Encoding"/> and the encoding used to open a path-based
    /// sink. Default: <see cref="System.Text.Encoding.UTF8"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    ICsvLoaderBuilder<T> Encoding(Encoding encoding);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.TrimOptions"/>. Default: <see cref="CsvTrimOptions.None"/>.
    /// </summary>
    ICsvLoaderBuilder<T> TrimOptions(CsvTrimOptions options);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.ShouldQuote"/> — a predicate deciding, per field, whether
    /// the value is quoted.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="shouldQuote"/> is <see langword="null"/>.</exception>
    ICsvLoaderBuilder<T> ShouldQuote(Func<CsvShouldQuoteContext, bool> shouldQuote);


    /// <summary>
    /// Sets <see cref="CsvLoader{TRecord}.ColumnMaps"/> — explicit member-to-column mappings
    /// applied instead of convention-based binding.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
    ICsvLoaderBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps);
}
