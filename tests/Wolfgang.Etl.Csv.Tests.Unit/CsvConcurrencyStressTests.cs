using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Concurrency / race stress tests (#70). Production use of an async library hits real
/// interleavings: many concurrent extractors/loaders sharing the process-wide class-map and
/// column-map caches, and cancellation arriving mid-<c>await</c>. These drive high-fan-out
/// <see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{Task})"/> workloads and assert
/// isolation, correctness, and prompt cancellation without hangs.
///
/// Plain xunit rather than Coyote — the library surface is <see cref="IAsyncEnumerable{T}"/>-based
/// and Coyote's async-stream scheduling support is too rough to instrument it cleanly. Marked
/// <c>[Trait("Category", "Concurrency")]</c> so a soak workflow can select and repeat them.
/// </summary>
[Trait("Category", "Concurrency")]
public class CsvConcurrencyStressTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);


    private const int Workers = 64;


    [Fact]
    public async Task Concurrent_extractions_of_the_same_record_type_stay_isolated_and_correct()
    {
        const string csv = "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\nCarol,White,42\r\n";

        var tasks = Enumerable.Range(0, Workers).Select(_ => Task.Run(async () =>
        {
            using var reader = ReaderOver(csv);
            var extractor = new CsvExtractor<PersonRecord>(reader);

            var got = new List<PersonRecord>();
            await foreach (var record in extractor.ExtractAsync().ConfigureAwait(false))
            {
                got.Add(record);
            }

            return got;
        }));

        var results = await Task.WhenAll(tasks);

        foreach (var got in results)
        {
            Assert.Equal(3, got.Count);
            Assert.Equal("Alice", got[0].FirstName);
            Assert.Equal("White", got[2].LastName);
            Assert.Equal(25, got[1].Age);
        }
    }


    [Fact]
    public async Task Concurrent_loads_produce_correct_independent_output()
    {
        var records = new[]
        {
            new PersonRecord { FirstName = "Alice", LastName = "Smith", Age = 30 },
            new PersonRecord { FirstName = "Bob", LastName = "Jones", Age = 25 },
        };
        const string expected = "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n";

        var tasks = Enumerable.Range(0, Workers).Select(_ => Task.Run(async () =>
        {
            using var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
            var loader = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord>
        { LeaveOpen = true});

            await loader.LoadAsync(ToAsync(records)).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);

            return Utf8NoBom.GetString(stream.ToArray());
        }));

        var outputs = await Task.WhenAll(tasks);

        Assert.All(outputs, output => Assert.Equal(expected, output));
    }


    [Fact]
    public async Task Cancellation_mid_concurrent_extraction_throws_and_never_hangs()
    {
        var builder = new StringBuilder("FirstName,LastName,Age\r\n");
        for (var i = 0; i < 2000; i++)
        {
            builder.Append("First").Append(i).Append(",Last").Append(i).Append(',').Append(i % 120).Append("\r\n");
        }

        var csv = builder.ToString();

        var tasks = Enumerable.Range(0, Workers).Select(_ => Task.Run(async () =>
        {
            using var cts = new CancellationTokenSource();
            using var reader = ReaderOver(csv);
            var extractor = new CsvExtractor<PersonRecord>(reader);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                var seen = 0;
                await foreach (var record in extractor.ExtractAsync(cts.Token).ConfigureAwait(false))
                {
                    // Defensive `record is not null` check: the extractor's
                    // nullable contract says non-null, but the concurrency stress
                    // test exercises interleavings we want to keep resilient.
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (record is not null && ++seen == 5)
                    {
                        cts.Cancel();
                    }
                }
            }).ConfigureAwait(false);
        }));

        var all = Task.WhenAll(tasks);
        var finished = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(30)));

        Assert.Same(all, finished);
        await all;
    }


    private static StreamReader ReaderOver(string content) =>
        new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);


    private static async IAsyncEnumerable<PersonRecord> ToAsync(IEnumerable<PersonRecord> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }

        await Task.CompletedTask;
    }
}
