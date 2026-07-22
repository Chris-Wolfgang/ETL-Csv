using System;
using System.Collections.Generic;
using System.Text;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Fluent builder for a <see cref="CsvExtractor{TRecord}"/> hung off the generic
/// <see cref="EtlPipeline"/> chain. Extends <see cref="IEtlPipeline{T}"/>, so a consumer
/// transitions from configuring the extractor to appending pipeline operators simply by
/// calling one — no explicit <c>Build()</c> step. The first pipeline operator (for example
/// <c>Where</c>, <c>Select</c>, or a <see cref="IEtlPipeline{T}.Through{TOut}(ITransformAsync{T, TOut})"/>
/// stage) materializes the extractor and the configuration setters fall off the surface.
/// </summary>
/// <typeparam name="T">The record type produced by the extractor.</typeparam>
/// <remarks>
/// Each setter maps 1:1 to a public property on <see cref="CsvExtractor{TRecord}"/>; no new
/// configuration surface is introduced. Setters return the builder for chaining and take
/// effect when the pipeline is first enumerated. Stream ownership is decided by the factory
/// overload, not a setter: a path-based source owns and disposes the file it opens, while a
/// caller-supplied <see cref="System.IO.StreamReader"/> is left open for the caller to dispose.
/// </remarks>
public interface ICsvExtractorBuilder<T> : IEtlPipeline<T>
    where T : notnull
{
    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.Delimiter"/>. Default: <c>","</c>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="delimiter"/> is <see langword="null"/>.</exception>
    ICsvExtractorBuilder<T> Delimiter(string delimiter);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.Quote"/>. Default: <c>"</c>.
    /// </summary>
    ICsvExtractorBuilder<T> Quote(char quote);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.Escape"/>. Default: <c>"</c>.
    /// </summary>
    ICsvExtractorBuilder<T> Escape(char escape);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.Comment"/>. Default: <c>#</c>. Only honoured when
    /// <see cref="AllowComments(bool)"/> is enabled.
    /// </summary>
    ICsvExtractorBuilder<T> Comment(char comment);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.AllowComments"/>. Default: <see langword="false"/>.
    /// </summary>
    ICsvExtractorBuilder<T> AllowComments(bool allow);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.IgnoreBlankLines"/>. Default: <see langword="true"/>.
    /// </summary>
    ICsvExtractorBuilder<T> IgnoreBlankLines(bool ignore);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.HasHeaderRecord"/>. Default: <see langword="true"/>.
    /// </summary>
    ICsvExtractorBuilder<T> HasHeaderRecord(bool hasHeader);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.Encoding"/> and the encoding used to open a
    /// path-based source. Default: <see cref="System.Text.Encoding.UTF8"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    ICsvExtractorBuilder<T> Encoding(Encoding encoding);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.InitialRecordIndex"/> — the 1-based line treated as
    /// the first line read. Default: <c>1</c>.
    /// </summary>
    ICsvExtractorBuilder<T> InitialRecordIndex(int oneBasedIndex);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.SkipRecordCount"/> — the number of records to skip
    /// after the header before yielding.
    /// </summary>
    ICsvExtractorBuilder<T> SkipRecordCount(int count);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.MaxRecordCount"/> — the maximum number of records to
    /// yield.
    /// </summary>
    ICsvExtractorBuilder<T> MaxRecordCount(int count);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.TrimOptions"/>. Default: <see cref="CsvTrimOptions.None"/>.
    /// </summary>
    ICsvExtractorBuilder<T> TrimOptions(CsvTrimOptions options);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.ColumnMaps"/> — explicit column-to-member mappings
    /// applied instead of convention-based binding.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
    ICsvExtractorBuilder<T> ColumnMaps(IReadOnlyList<CsvColumnMap> maps);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.BadDataFound"/> — a callback invoked when the parser
    /// detects bad data. Extraction always continues after the callback.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    ICsvExtractorBuilder<T> BadDataFound(Action<CsvBadDataInfo> handler);


    /// <summary>
    /// Sets <see cref="CsvExtractor{TRecord}.ReadingExceptionOccurred"/> — a callback invoked when
    /// the parser raises a recoverable parse exception. The exception still propagates.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    ICsvExtractorBuilder<T> ReadingExceptionOccurred(Action<CsvReadingExceptionInfo> handler);
}
