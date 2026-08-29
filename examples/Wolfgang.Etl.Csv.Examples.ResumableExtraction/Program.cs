using System.Text;
using Wolfgang.Etl.Csv;
using Wolfgang.Etl.Csv.Examples.ResumableExtraction;
// These files still configure via the deprecated property setters in places where the value is
// applied after construction, so it cannot travel through the options constructor without
// restructuring the test. They keep exercising the setter path until the setters are removed.
#pragma warning disable CS0618


// Resumable extraction: persist a record counter as a checkpoint, and on the next run skip past
// the records already acknowledged. CsvCheckpointExtensions covers the mechanical bits — the atomic
// write and the read-with-default — while WHEN to checkpoint (here: after each row) stays your policy.

var dir = Path.Combine(Path.GetTempPath(), "csv-resumable-demo");
Directory.CreateDirectory(dir);
var csvPath = Path.Combine(dir, "orders.csv");
var checkpointPath = Path.Combine(dir, "orders.checkpoint");

await File.WriteAllTextAsync(csvPath, BuildCsv(rows: 10));
if (File.Exists(checkpointPath))
{
    File.Delete(checkpointPath);
}

Console.WriteLine("Run 1 — processes 4 rows, checkpointing after each, then 'crashes':");
await ProcessAsync(csvPath, checkpointPath, stopAfter: 4);

Console.WriteLine();
Console.WriteLine("Run 2 — resumes from the checkpoint and finishes the rest:");
await ProcessAsync(csvPath, checkpointPath, stopAfter: int.MaxValue);

static async Task ProcessAsync(string csvPath, string checkpointPath, int stopAfter)
{
    using var reader = new StreamReader(csvPath);
    var extractor = new CsvExtractor<OrderRow>(reader);

    // Sets SkipRecordCount to the persisted count so already-processed rows are skipped.
    var alreadyDone = await extractor.ResumeFromCheckpointAsync(checkpointPath);
    if (alreadyDone > 0)
    {
        Console.WriteLine($"  resuming past {alreadyDone} already-processed row(s)");
    }

    var processed = alreadyDone;
    await foreach (var order in extractor.ExtractAsync())
    {
        Console.WriteLine($"  processed order {order.Id}");
        processed++;

        // Acknowledge forward: crash-safe because the write is atomic.
        await CsvCheckpointExtensions.WriteCheckpointAsync(checkpointPath, processed);

        if (processed - alreadyDone >= stopAfter)
        {
            Console.WriteLine("  ...crash!");
            break;
        }
    }
}

static string BuildCsv(int rows)
{
    var builder = new StringBuilder("Id\n");
    for (var i = 1; i <= rows; i++)
    {
        builder.Append(i).Append('\n');
    }

    return builder.ToString();
}
