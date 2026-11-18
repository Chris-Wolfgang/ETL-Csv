using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the resumable-extraction guidance in docs/cookbook/resumable-extraction.md: SkipItemCount skips
/// raw rows, so a count of committed records only resumes correctly when no row was dropped.
/// </summary>
public class CsvResumeRowCountTests
{
    private const string Csv = "FirstName,LastName,Age\nAnn,A,1\nBad,B,-1\nCy,C,3\nDee,D,4\n";



    [Fact]
    public async Task Resuming_with_a_committed_record_count_replays_a_record_when_a_row_was_dropped()
    {
        // Ann and Cy committed (Bad was dropped by validation): the committed count is 2, but three
        // raw rows were consumed, so SkipItemCount = 2 yields Cy again.
        var (resumed, _) = await RunAsync(skipItemCount: 2);

        Assert.Equal(new[] { "Cy", "Dee" }, resumed);
    }



    [Fact]
    public async Task Resuming_with_the_consumed_row_count_does_not_replay_a_committed_record()
    {
        // The documented counter: CurrentItemCount + CurrentSkippedItemCount + CurrentErrorItemCount
        // + records dropped by validation (counted via InvalidRecordHandler) at the moment Cy is yielded.
        var (firstRun, consumedAtCy) = await RunAsync(skipItemCount: 0, stopAfter: "Cy");

        Assert.Equal(new[] { "Ann", "Cy" }, firstRun);
        Assert.Equal(3, consumedAtCy);

        var (resumed, _) = await RunAsync(skipItemCount: consumedAtCy);

        Assert.Equal(new[] { "Dee" }, resumed);
    }



    private static async Task<(List<string> Names, int Consumed)> RunAsync(int skipItemCount, string? stopAfter = null)
    {
        var dropped = 0;
        var extractor = new CsvExtractor<PersonRecord>
        (
            new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(Csv)), Encoding.UTF8),
            new CsvExtractorOptions<PersonRecord>
            {
                SkipItemCount = skipItemCount,
                OnValidationFailure = CsvValidationFailureAction.Skip,
                Validators = new[] { CsvValidator.GreaterThan<PersonRecord>(p => p.Age, 0, "Age") },
                InvalidRecordHandler = _ => dropped++,
            }
        );

        var names = new List<string>();
        var consumed = 0;
        await foreach (var person in extractor.ExtractAsync())
        {
            names.Add(person.FirstName);
            consumed = extractor.CurrentItemCount
                + extractor.CurrentSkippedItemCount
                + extractor.CurrentErrorItemCount
                + dropped;

            if (string.Equals(person.FirstName, stopAfter, System.StringComparison.Ordinal))
            {
                break;
            }
        }

        return (names, consumed);
    }
}
