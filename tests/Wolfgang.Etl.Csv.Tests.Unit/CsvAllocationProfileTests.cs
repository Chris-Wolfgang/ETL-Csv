#if NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Hot-path allocation regression guards. Uses
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/> (available since
/// netcoreapp3.0; net5.0+ for our test matrix) to capture the per-thread
/// allocation delta across a known extraction or load.
///
/// These tests don't aim for zero allocation — CsvHelper allocates per
/// record by design, and the wrapper types contribute too. The aim is
/// to catch egregious regressions (e.g. someone accidentally adds a
/// <c>string.Format</c> per row, or removes a buffer pool and now we
/// allocate 100× more). The ceilings are intentionally generous:
/// they're regression alarms, not contracts.
///
/// If a baseline shifts because a real perf improvement reduced
/// allocations, tighten the ceiling at the same time as the
/// improvement. If a baseline shifts because of a regression, find
/// out why before relaxing the ceiling.
/// </summary>
public class CsvAllocationProfileTests
{
    private const string Csv =
        "FirstName,LastName,Age\r\n" +
        "Alice,Smith,30\r\n" +
        "Bob,Jones,25\r\n" +
        "Carol,White,35\r\n" +
        "Dave,Brown,40\r\n" +
        "Eve,Davis,28\r\n";



    private static StreamReader Reader() =>
        new(new MemoryStream(Encoding.UTF8.GetBytes(Csv)), Encoding.UTF8);



    private static void StabilizeBeforeMeasurement()
    {
        // Two GC sweeps + finalizer drain make the baseline reading
        // independent of allocations from previous tests / xUnit
        // scaffolding. Without this the noise floor would be tens of
        // KB and the per-record signal would drown.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }



    [Fact]
    public async Task ExtractAsync_when_run_over_5_record_CSV_stays_under_allocation_ceiling()
    {
        // Warm-up: JIT compilation, CsvHelper internal cache priming,
        // ClassMap construction. These happen exactly once per process
        // and should not count against per-record allocation budgets.
        await ExtractOnce();

        StabilizeBeforeMeasurement();
        var before = GC.GetAllocatedBytesForCurrentThread();

        var count = await ExtractOnce();

        var after = GC.GetAllocatedBytesForCurrentThread();
        var delta = after - before;

        Assert.Equal(5, count);

        // 5 records × ~3 KB per record = ~15 KB baseline (small but
        // realistic for CsvHelper + record allocation + string interning).
        // Ceiling of 100 KB allows ~6× headroom for environmental noise
        // and still catches an order-of-magnitude regression.
        const long Ceiling = 100_000;
        Assert.True
        (
            delta < Ceiling,
            $"ExtractAsync allocated {delta} bytes for 5 records (ceiling {Ceiling}). " +
            "If this is an intentional change, update the ceiling and explain why."
        );
    }



    [Fact]
    public async Task LoadAsync_when_writing_5_record_stream_stays_under_allocation_ceiling()
    {
        // Same warmup + measurement pattern, but for the write path.
        await LoadOnce();

        StabilizeBeforeMeasurement();
        var before = GC.GetAllocatedBytesForCurrentThread();

        await LoadOnce();

        var after = GC.GetAllocatedBytesForCurrentThread();
        var delta = after - before;

        // Same rationale as ExtractAsync. Write path tends to be a bit
        // higher because CsvHelper's per-record write buffer flushes
        // through StreamWriter's char buffer.
        const long Ceiling = 100_000;
        Assert.True
        (
            delta < Ceiling,
            $"LoadAsync allocated {delta} bytes for 5 records (ceiling {Ceiling}). " +
            "If this is an intentional change, update the ceiling and explain why."
        );
    }



    private static async Task<int> ExtractOnce()
    {
        var sut = new CsvExtractor<PersonRecord>(Reader());
        var count = 0;
        await foreach (var _ in sut.ExtractAsync().ConfigureAwait(false))
        {
            count++;
        }
        return count;
    }



    private static async Task LoadOnce()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true);
        var sut = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord>
        { LeaveOpen = true});
        var items = new List<PersonRecord>
        {
            new() { FirstName = "Alice", LastName = "Smith", Age = 30 },
            new() { FirstName = "Bob",   LastName = "Jones", Age = 25 },
            new() { FirstName = "Carol", LastName = "White", Age = 35 },
            new() { FirstName = "Dave",  LastName = "Brown", Age = 40 },
            new() { FirstName = "Eve",   LastName = "Davis", Age = 28 },
        };
        await sut.LoadAsync(items.ToAsyncEnumerable()).ConfigureAwait(false);
        await writer.FlushAsync().ConfigureAwait(false);
        await writer.DisposeAsync().ConfigureAwait(false);
    }
}

#endif
