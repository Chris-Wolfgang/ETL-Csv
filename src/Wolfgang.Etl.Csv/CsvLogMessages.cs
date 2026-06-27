using System;
using Microsoft.Extensions.Logging;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Cached <see cref="LoggerMessage"/> delegates for high-performance structured logging
/// across the CSV extractor and loader.
/// </summary>
/// <remarks>
/// <para><b>EventId numbering scheme</b> — IDs are reserved by category so future maintainers
/// don't reuse retired IDs (log aggregators key alerts on EventId; reuse confuses dashboards):</para>
/// <list type="bullet">
///   <item><description><c>1–9</c>: Lifecycle / general (currently 1=StartingOperation, 2=SkippedItem, 3=ReachedMaximumItemCount; 4–9 reserved)</description></item>
///   <item><description><c>10–19</c>: Extractor-specific (10=ExtractedItem, 11=ExtractionCompleted, 12=IgnoredRow; 13 retired = removed default BadDataFound log per PII privacy; 14 retired = removed default ReadingExceptionOccurred log)</description></item>
///   <item><description><c>20–29</c>: Loader-specific (20=LoadedItem, 21=LoadingCompleted; 22–29 reserved)</description></item>
/// </list>
/// <para>When adding a new event, use the next free ID within the appropriate category. Never reuse
/// a retired ID — pick a fresh one even if the new event is conceptually similar.</para>
/// </remarks>
internal static class CsvLogMessages
{
    internal static readonly Action<ILogger, string, Exception?> StartingOperation =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, nameof(StartingOperation)), "Starting {Operation}.");

    internal static readonly Action<ILogger, int, int, Exception?> SkippedItem =
        LoggerMessage.Define<int, int>(LogLevel.Debug, new EventId(2, nameof(SkippedItem)), "Skipped item {SkippedCount} of {SkipTotal}.");

    internal static readonly Action<ILogger, int, Exception?> ReachedMaximumItemCount =
        LoggerMessage.Define<int>(LogLevel.Debug, new EventId(3, nameof(ReachedMaximumItemCount)), "Reached MaximumItemCount of {MaximumItemCount}. Stopping.");

    internal static readonly Action<ILogger, int, Exception?> ExtractedItem =
        LoggerMessage.Define<int>(LogLevel.Debug, new EventId(10, nameof(ExtractedItem)), "Extracted item {CurrentItemCount}.");

    internal static readonly Action<ILogger, int, int, Exception?> ExtractionCompleted =
        LoggerMessage.Define<int, int>(LogLevel.Information, new EventId(11, nameof(ExtractionCompleted)), "CSV extraction completed. Extracted: {ItemCount}, skipped: {SkippedCount}.");

    internal static readonly Action<ILogger, int, Exception?> IgnoredRow =
        LoggerMessage.Define<int>(LogLevel.Debug, new EventId(12, nameof(IgnoredRow)), "Ignored row {RawRowIndex} before InitialRecordIndex.");

    // Note: bad-data and reading-exception events are surfaced to callers via the
    // CsvExtractor.BadDataFound and CsvExtractor.ReadingExceptionOccurred Action<>
    // properties rather than being logged by the library. This keeps potentially-
    // sensitive CSV record contents out of application logs unless the caller
    // explicitly opts in by wiring those callbacks to their own logger.

    internal static readonly Action<ILogger, int, Exception?> LoadedItem =
        LoggerMessage.Define<int>(LogLevel.Debug, new EventId(20, nameof(LoadedItem)), "Loaded item {CurrentItemCount}.");

    internal static readonly Action<ILogger, int, int, Exception?> LoadingCompleted =
        LoggerMessage.Define<int, int>(LogLevel.Information, new EventId(21, nameof(LoadingCompleted)), "CSV loading completed. Loaded: {ItemCount}, skipped: {SkippedCount}.");
}
