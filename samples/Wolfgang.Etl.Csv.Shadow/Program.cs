// Shadow-testing sample consumer (#63).
//
// Synthetic unit / property tests prove the behavioural contract under tidy input.
// They do NOT catch performance or allocation regressions that only surface under
// realistic consumer traffic shapes — mixed row widths, concurrent enumeration,
// paged windowing, bursty small calls. This runner replays those shapes through the
// PUBLIC CsvExtractor / CsvLoader surface and emits a per-scenario allocation + GC +
// latency report. shadow.yaml runs it nightly and compares the report against a
// committed golden baseline: allocation regressions fail the run (deterministic),
// latency is advisory (CI wall-clock is noisy).
//
// It doubles as executable "real usage" documentation that XML-doc snippets can't.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv;

namespace Wolfgang.Etl.Csv.Shadow;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var outputPath = args.Length > 0 ? args[0] : "shadow-report.json";

        var scenarios = new (string Name, Func<Task<long>> Run)[]
        {
            ("round_trip_mixed", () => RoundTripMixedAsync(40_000)),
            ("concurrent_streaming", () => ConcurrentStreamingAsync(streams: 8, recordsPerStream: 15_000)),
            ("windowed_paging", () => WindowedPagingAsync(total: 60_000, page: 10_000)),
            ("bursty_small", () => BurstySmallAsync(iterations: 2_000, rowsEach: 25)),
        };

        var results = new Dictionary<string, ScenarioResult>(StringComparer.Ordinal);
        foreach (var (name, run) in scenarios)
        {
            var result = await MeasureAsync(run).ConfigureAwait(false);
            results[name] = result;
            Console.WriteLine
            (
                $"{name,-22} records={result.Records,10:N0}  alloc={result.AllocatedBytes,14:N0}B  " +
                $"gen0={result.Gen0,3} gen1={result.Gen1,3} gen2={result.Gen2,3}  {result.ElapsedMs,8:F1}ms"
            );
        }

        var report = new ShadowReport
        (
            Schema: 1,
            Commit: Environment.GetEnvironmentVariable("GITHUB_SHA") ?? "local",
            Scenarios: results
        );

        var json = JsonSerializer.Serialize(report, ShadowJson.Options);
        await File.WriteAllTextAsync(outputPath, json).ConfigureAwait(false);
        Console.WriteLine($"\nWrote {outputPath}");
        return 0;
    }


    // Measures allocation (process-wide, precise — covers the concurrent scenario's worker
    // threads), GC collections, and wall-clock around one steady-state run. A warm-up run
    // first pays the JIT / first-use costs so they don't land in the measured numbers.
    private static async Task<ScenarioResult> MeasureAsync(Func<Task<long>> scenario)
    {
        await scenario().ConfigureAwait(false);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocStart = GC.GetTotalAllocatedBytes(precise: true);
        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);

        var stopwatch = Stopwatch.StartNew();
        var records = await scenario().ConfigureAwait(false);
        stopwatch.Stop();

        return new ScenarioResult
        (
            AllocatedBytes: GC.GetTotalAllocatedBytes(precise: true) - allocStart,
            Gen0: GC.CollectionCount(0) - gen0,
            Gen1: GC.CollectionCount(1) - gen1,
            Gen2: GC.CollectionCount(2) - gen2,
            ElapsedMs: stopwatch.Elapsed.TotalMilliseconds,
            Records: records
        );
    }


    // Extract → materialize → load back out → re-extract. The classic ETL round-trip a
    // consumer runs when reshaping a file, over rows of deliberately varied width.
    private static async Task<long> RoundTripMixedAsync(int records)
    {
        var bytes = Encoding.UTF8.GetBytes(BuildCsv(records, vary: true));

        var buffer = new List<ShadowRecord>(records);
        using (var reader = new StreamReader(new MemoryStream(bytes, writable: false), Encoding.UTF8))
        {
            var extractor = new CsvExtractor<ShadowRecord>(reader);
            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                buffer.Add(record);
            }
        }

        using var output = new MemoryStream(bytes.Length);
        using (var writer = new StreamWriter(output, new UTF8Encoding(false), 1 << 16, leaveOpen: true))
        {
            var loader = new CsvLoader<ShadowRecord>(writer);
            await loader.LoadAsync(ToAsync(buffer)).ConfigureAwait(false);
        }

        output.Position = 0;
        long roundTripped = 0;
        using (var reader = new StreamReader(output, Encoding.UTF8))
        {
            var extractor = new CsvExtractor<ShadowRecord>(reader);
            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                if (record.Age < 0)
                {
                    throw new InvalidOperationException("round-trip corruption");
                }

                roundTripped++;
            }
        }

        return roundTripped;
    }


    // N extractors streaming concurrently off the same source bytes — the shape a service
    // hits under parallel request load. Precise process-wide allocation is what makes this
    // measurable across the worker threads.
    private static async Task<long> ConcurrentStreamingAsync(int streams, int recordsPerStream)
    {
        var bytes = Encoding.UTF8.GetBytes(BuildCsv(recordsPerStream, vary: true));

        var tasks = Enumerable.Range(0, streams).Select(async _ =>
        {
            using var reader = new StreamReader(new MemoryStream(bytes, writable: false), Encoding.UTF8);
            var extractor = new CsvExtractor<ShadowRecord>(reader);
            long count = 0;
            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                if (record.Age < 0)
                {
                    throw new InvalidOperationException("unexpected");
                }

                count++;
            }

            return count;
        });

        var counts = await Task.WhenAll(tasks).ConfigureAwait(false);
        return counts.Sum();
    }


    // Page through a large file in fixed windows via SkipRecordCount + MaxRecordCount — the
    // resumable / paged-consumption pattern, re-opening the source per page.
    private static async Task<long> WindowedPagingAsync(int total, int page)
    {
        var bytes = Encoding.UTF8.GetBytes(BuildCsv(total, vary: false));

        long seen = 0;
        for (var skip = 0; skip < total; skip += page)
        {
            using var reader = new StreamReader(new MemoryStream(bytes, writable: false), Encoding.UTF8);
            var extractor = new CsvExtractor<ShadowRecord>(reader)
            {
                SkipRecordCount = skip,
                MaxRecordCount = page,
            };

            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                seen++;
            }
        }

        return seen;
    }


    // Many tiny extract+load cycles back to back — bursty, short-lived call traffic where
    // per-call fixed overhead (buffers, parser setup) dominates and must stay bounded.
    private static async Task<long> BurstySmallAsync(int iterations, int rowsEach)
    {
        var bytes = Encoding.UTF8.GetBytes(BuildCsv(rowsEach, vary: false));

        long total = 0;
        for (var i = 0; i < iterations; i++)
        {
            var buffer = new List<ShadowRecord>(rowsEach);
            using (var reader = new StreamReader(new MemoryStream(bytes, writable: false), Encoding.UTF8))
            {
                var extractor = new CsvExtractor<ShadowRecord>(reader);
                await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
                {
                    buffer.Add(record);
                }
            }

            using var output = new MemoryStream(bytes.Length);
            using (var writer = new StreamWriter(output, new UTF8Encoding(false), 1024, leaveOpen: true))
            {
                var loader = new CsvLoader<ShadowRecord>(writer);
                await loader.LoadAsync(ToAsync(buffer)).ConfigureAwait(false);
            }

            total += buffer.Count;
        }

        return total;
    }


    // Deterministic (seeded) generator. `vary` widens some rows so the workload isn't a
    // single uniform shape — the realistic-traffic intent of #63.
    private static string BuildCsv(int records, bool vary)
    {
        var random = new Random(20260805);
        var cities = new[] { "Stroudsburg", "Bethlehem", "Allentown", "Easton", "Wilkes-Barre" };
        var builder = new StringBuilder(records * 32);
        builder.Append("FirstName,LastName,Age,City\r\n");

        for (var i = 0; i < records; i++)
        {
            var width = vary && (i % 7 == 0) ? random.Next(20, 60) : random.Next(4, 12);
            builder
                .Append("First").Append('X', width).Append(i)
                .Append(",Last").Append(i)
                .Append(',').Append(i % 100)
                .Append(',').Append(cities[i % cities.Length])
                .Append("\r\n");
        }

        return builder.ToString();
    }


    private static async IAsyncEnumerable<ShadowRecord> ToAsync(IEnumerable<ShadowRecord> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }
}


internal sealed record ShadowRecord(string FirstName, string LastName, int Age, string City);


internal sealed record ScenarioResult(long AllocatedBytes, int Gen0, int Gen1, int Gen2, double ElapsedMs, long Records);


internal sealed record ShadowReport(int Schema, string Commit, IReadOnlyDictionary<string, ScenarioResult> Scenarios);


internal static class ShadowJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
    };
}
