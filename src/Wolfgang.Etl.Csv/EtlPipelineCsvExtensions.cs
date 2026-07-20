using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Wolfgang.Etl.Abstractions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Class-named CSV source factories and sink terminators for the fluent <see cref="EtlPipeline"/>
/// chain. Source factories hang off <see cref="EtlPipeline"/> so a pipeline reads
/// <c>EtlPipeline.Create().CsvExtractor&lt;Order&gt;("orders.csv")</c>; sink terminators hang off
/// <see cref="IEtlPipeline{T}"/> so it ends <c>… .CsvLoader&lt;Order&gt;("out.csv").RunAsync()</c>.
/// </summary>
/// <remarks>
/// Path-based factories own the file stream they open and dispose it after the run (success or
/// failure). Factories that accept a caller-supplied <see cref="StreamReader"/>,
/// <see cref="StreamWriter"/>, or a pre-built extractor/loader do not dispose it — the caller owns
/// the lifecycle. The fluent configuration setters returned by these factories map 1:1 to the
/// underlying <c>CsvExtractor&lt;T&gt;</c> / <c>CsvLoader&lt;T&gt;</c> properties.
/// </remarks>
public static class EtlPipelineCsvExtensions
{
    /// <summary>
    /// Begins a pipeline from a CSV file. The factory owns the file stream and disposes it when
    /// enumeration finishes.
    /// </summary>
    /// <typeparam name="T">The record type produced by the extractor.</typeparam>
    /// <param name="pipeline">The builder returned by <see cref="EtlPipeline.Create"/>.</param>
    /// <param name="path">The path of the CSV file to read.</param>
    /// <returns>An <see cref="ICsvExtractorBuilder{T}"/> for inline configuration and chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="path"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvExtractorBuilder<T> CsvExtractor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this EtlPipeline pipeline,
        string path
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return CsvExtractorBuilder<T>.FromPath(path);
    }


    /// <summary>
    /// Begins a pipeline from a caller-supplied <see cref="StreamReader"/>. The caller owns and
    /// disposes the reader.
    /// </summary>
    /// <typeparam name="T">The record type produced by the extractor.</typeparam>
    /// <param name="pipeline">The builder returned by <see cref="EtlPipeline.Create"/>.</param>
    /// <param name="reader">The reader to read CSV data from.</param>
    /// <returns>An <see cref="ICsvExtractorBuilder{T}"/> for inline configuration and chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="reader"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvExtractorBuilder<T> CsvExtractor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this EtlPipeline pipeline,
        StreamReader reader
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return CsvExtractorBuilder<T>.FromReader(reader);
    }


    /// <summary>
    /// Begins a pipeline from a pre-built <c>CsvExtractor&lt;T&gt;</c>. The caller owns the
    /// extractor and any stream it wraps.
    /// </summary>
    /// <typeparam name="T">The record type produced by the extractor.</typeparam>
    /// <param name="pipeline">The builder returned by <see cref="EtlPipeline.Create"/>.</param>
    /// <param name="extractor">The extractor that seeds the pipeline.</param>
    /// <returns>An <see cref="ICsvExtractorBuilder{T}"/> for inline configuration and chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="extractor"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvExtractor uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvExtractorBuilder<T> CsvExtractor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this EtlPipeline pipeline,
        CsvExtractor<T> extractor
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (extractor is null)
        {
            throw new ArgumentNullException(nameof(extractor));
        }

        return CsvExtractorBuilder<T>.FromExtractor(extractor);
    }


    /// <summary>
    /// Terminates the pipeline, writing all records to a CSV file. The factory owns the file stream
    /// and disposes it after the run (success or failure).
    /// </summary>
    /// <typeparam name="T">The record type consumed by the loader.</typeparam>
    /// <param name="pipeline">The pipeline to terminate.</param>
    /// <param name="path">The path of the CSV file to write.</param>
    /// <returns>An <see cref="ICsvLoaderBuilder{T}"/> for inline configuration; call
    /// <see cref="IEtlPipelineSink.RunAsync(IProgress{EtlPipelineProgress}, System.Threading.CancellationToken)"/> to run.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="path"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvLoader uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvLoaderBuilder<T> CsvLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this IEtlPipeline<T> pipeline,
        string path
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return CsvLoaderBuilder<T>.FromPath(pipeline, path);
    }


    /// <summary>
    /// Terminates the pipeline, writing all records to a caller-supplied <see cref="StreamWriter"/>.
    /// The caller owns and disposes the writer.
    /// </summary>
    /// <typeparam name="T">The record type consumed by the loader.</typeparam>
    /// <param name="pipeline">The pipeline to terminate.</param>
    /// <param name="writer">The writer to write CSV data to.</param>
    /// <returns>An <see cref="ICsvLoaderBuilder{T}"/> for inline configuration; call
    /// <see cref="IEtlPipelineSink.RunAsync(IProgress{EtlPipelineProgress}, System.Threading.CancellationToken)"/> to run.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvLoader uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvLoaderBuilder<T> CsvLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this IEtlPipeline<T> pipeline,
        StreamWriter writer
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        return CsvLoaderBuilder<T>.FromWriter(pipeline, writer);
    }


    /// <summary>
    /// Terminates the pipeline with a pre-built <c>CsvLoader&lt;T&gt;</c>. The caller owns
    /// the loader and any stream it wraps.
    /// </summary>
    /// <typeparam name="T">The record type consumed by the loader.</typeparam>
    /// <param name="pipeline">The pipeline to terminate.</param>
    /// <param name="loader">The loader that consumes the pipeline output.</param>
    /// <returns>An <see cref="ICsvLoaderBuilder{T}"/> for inline configuration; call
    /// <see cref="IEtlPipelineSink.RunAsync(IProgress{EtlPipelineProgress}, System.Threading.CancellationToken)"/> to run.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="loader"/> is <see langword="null"/>.</exception>
    [RequiresUnreferencedCode("CsvLoader uses CsvHelper, which reflects over T's members. The library is not trim/NativeAOT safe.")]
    public static ICsvLoaderBuilder<T> CsvLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    (
        this IEtlPipeline<T> pipeline,
        CsvLoader<T> loader
    )
        where T : notnull
    {
        if (pipeline is null)
        {
            throw new ArgumentNullException(nameof(pipeline));
        }

        if (loader is null)
        {
            throw new ArgumentNullException(nameof(loader));
        }

        return CsvLoaderBuilder<T>.FromLoader(pipeline, loader);
    }
}
