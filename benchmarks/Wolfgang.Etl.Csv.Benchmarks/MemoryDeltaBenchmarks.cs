using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Wolfgang.Etl.Csv.Benchmarks;

/// <summary>
/// Measures the bytes allocated (process-wide, <see cref="GC.GetTotalAllocatedBytes(bool)"/>)
/// between just before extraction starts and just after the final record is yielded.
/// No collection is forced inside the timed body, so the Mean column reflects extraction
/// alone; the BenchmarkDotNet <c>MemoryDiagnoser</c> attribute reports the same allocation
/// per operation in its "Allocated" column.
/// </summary>
[MemoryDiagnoser]
public class MemoryDeltaBenchmarks
{
    private byte[] _data = Array.Empty<byte>();



    [Params(0, 1, 1_000, 10_000, 100_000, 1_000_000)]
    public int RecordCount { get; set; }



    [GlobalSetup]
    public void Setup()
    {
        _data = BuildData(RecordCount);
    }



    private static byte[] BuildData(int count)
    {
        var sb = new StringBuilder(Math.Max(64, count * 50));
        sb.AppendLine("first_name,last_name,city,zip_code,age");
        for (var i = 0; i < count; i++)
        {
            sb.AppendLine("John,Smith,Seattle,98101,42");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }



    [Benchmark]
    public async Task<long> Extract_MemoryDelta()
    {
        var before = GC.GetTotalAllocatedBytes(precise: true);

        using var reader = new StreamReader(new MemoryStream(_data), Encoding.UTF8);
        var extractor = new CsvExtractor<BenchmarkRecord>(reader);

        await foreach (var _ in extractor.ExtractAsync())
        {
            // Drain the enumerable so every extraction allocation falls inside the
            // measurement window; the row isn't otherwise consumed.
        }

        var after = GC.GetTotalAllocatedBytes(precise: true);
        return after - before;
    }
}
