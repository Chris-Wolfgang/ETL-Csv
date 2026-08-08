#if NET8_0_OR_GREATER

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;
using Xunit.Abstractions;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Sustained-load GC / allocation profiling (#75). Streams a large synthetic dataset through
/// <see cref="CsvExtractor{TRecord}"/> and asserts the streaming path stays GC-healthy under
/// volume: bounded allocation per record, and — the load-bearing check — no unexpected gen2
/// promotion, which is what a retention / LOH-pressure regression would look like. Yielded
/// records are discarded immediately, so a well-behaved streaming extractor should churn gen0
/// and leave gen2 essentially flat no matter how many rows it processes.
///
/// The row count scales via the <c>GC_PROFILE_RECORDS</c> environment variable, so the same test
/// runs a fast pass in PR CI (default) and a long, high-volume pass in <c>gc-profiling.yaml</c>.
/// <c>[Trait("Category","GcProfile")]</c> lets the scheduled workflow select it.
///
/// This covers the sustained-load GC-behaviour intent of #75; the full PerfView/ETW analysis
/// (top allocation sites, per-type heap census) is heavier Windows-only tooling left as a
/// follow-up.
/// </summary>
[Trait("Category", "GcProfile")]
public class CsvSustainedLoadGcTests
{
    private readonly ITestOutputHelper _output;


    public CsvSustainedLoadGcTests(ITestOutputHelper output) => _output = output;


    [Fact]
    public async Task Streaming_a_large_dataset_stays_gc_healthy()
    {
        // GC.CollectionCount is process-wide, so the gen2 assertion is only reliable when this
        // test genuinely runs alone. That is asserted only when GC_PROFILE_ISOLATED=true, which
        // gc-profiling.yaml sets while filtering to Category=GcProfile (nothing else running).
        // The row count (GC_PROFILE_RECORDS) is orthogonal — setting it does not imply isolation,
        // so in the normal parallel PR run we assert only the per-thread allocation guard.
        var isolatedRun = string.Equals
        (
            Environment.GetEnvironmentVariable("GC_PROFILE_ISOLATED"),
            "true",
            StringComparison.OrdinalIgnoreCase
        );
        var records = ResolveRecordCount();

        // Build the source bytes OUTSIDE the measured region — the ~MB byte[] is itself an LOH
        // allocation and would otherwise pollute the extractor's gen2 reading.
        var bytes = Encoding.UTF8.GetBytes(BuildCsv(records));

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);
        // GetAllocatedBytesForCurrentThread is thread-affine — which is intended here. The source
        // is an in-memory MemoryStream, so every await in the loop below completes synchronously
        // and enumeration never leaves this thread; the start/end reads therefore measure the same
        // thread. (If this ever drove a genuinely async source that resumed on the thread pool,
        // this would need a thread-agnostic measure such as GC.GetTotalAllocatedBytes.)
        var allocatedStart = GC.GetAllocatedBytesForCurrentThread();

        var count = 0;
        using (var reader = new StreamReader(new MemoryStream(bytes, writable: false), Encoding.UTF8))
        {
            var extractor = new CsvExtractor<PersonRecord>(reader);
            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                // Touch a field so the JIT can't elide the mapping, then discard.
                if (record.Age < 0)
                {
                    throw new InvalidOperationException("unexpected");
                }

                count++;
            }
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedStart;
        var gen0Delta = GC.CollectionCount(0) - gen0;
        var gen1Delta = GC.CollectionCount(1) - gen1;
        var gen2Delta = GC.CollectionCount(2) - gen2;
        var perRecord = allocated / (double)records;

        _output.WriteLine
        (
            $"records={records:N0} allocated={allocated:N0}B perRecord={perRecord:F0}B " +
            $"gen0={gen0Delta} gen1={gen1Delta} gen2={gen2Delta}"
        );

        Assert.Equal(records, count);

        // Streaming must not retain: gen2 (and any LOH growth that would trigger it) stays flat
        // no matter the volume. A spike means rows are being kept alive somewhere. Only checked
        // in isolation (see the note above) since GC.CollectionCount is process-wide.
        if (isolatedRun)
        {
            Assert.True
            (
                gen2Delta <= 1,
                $"gen2 collections spiked ({gen2Delta}) over {records:N0} records — the streaming path may be retaining rows."
            );
        }

        // Generous per-record regression alarm (not a contract): CsvHelper allocates per row by design.
        Assert.True
        (
            perRecord < 2048,
            $"per-record allocation {perRecord:F0}B exceeds the 2KB regression ceiling over {records:N0} records."
        );
    }


    private static int ResolveRecordCount()
    {
        var raw = Environment.GetEnvironmentVariable("GC_PROFILE_RECORDS");
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n > 0
            ? n
            : 50_000;
    }


    private static string BuildCsv(int records)
    {
        var builder = new StringBuilder("FirstName,LastName,Age\r\n", records * 20);
        for (var i = 0; i < records; i++)
        {
            builder.Append("First").Append(i).Append(",Last").Append(i).Append(',').Append(i % 100).Append("\r\n");
        }

        return builder.ToString();
    }
}

#endif
