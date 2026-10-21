using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Wolfgang.Etl.TestKit.Xunit;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins <see cref="CsvLoaderProgress.CurrentLineNumber"/> to its documented meaning: the 1-based
/// line most recently written, or 0 before any line is written.
/// </summary>
public class CsvLoaderLineNumberTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    private static readonly PersonRecord[] People =
    {
        new() { FirstName = "Alice", LastName = "Smith", Age = 30 },
        new() { FirstName = "Bob", LastName = "Jones", Age = 25 },
        new() { FirstName = "Carol", LastName = "White", Age = 35 },
    };



    [Fact]
    public async Task LoadAsync_reports_the_last_written_line_number()
    {
        var reported = await LoadAndReportAsync(People, hasHeader: true);

        // One header line plus three records.
        Assert.Equal(4, reported);
    }



    [Fact]
    public async Task LoadAsync_when_only_the_header_is_written_reports_line_one()
    {
        var reported = await LoadAndReportAsync(Array.Empty<PersonRecord>(), hasHeader: true);

        Assert.Equal(1, reported);
    }



    [Fact]
    public async Task LoadAsync_when_nothing_is_written_reports_zero()
    {
        var reported = await LoadAndReportAsync(Array.Empty<PersonRecord>(), hasHeader: false);

        Assert.Equal(0, reported);
    }



    private static async Task<int> LoadAndReportAsync(IEnumerable<PersonRecord> items, bool hasHeader)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var timer = new ManualProgressTimer();
        var sut = new CsvLoader<PersonRecord>(writer, timer)
        {
#pragma warning disable CS0618 // the timer-injecting constructor takes no options record
            HasHeaderRecord = hasHeader,
#pragma warning restore CS0618
        };
        var progress = new SyncProgress<CsvLoaderProgress>();

        // The loader disposes the timer when the load ends, so fire it from the source once the
        // loop has written every record and asks for the next one.
        await sut.LoadAsync(ThenFire(items, timer), progress);

        return progress.LastValue!.CurrentLineNumber;
    }



    private static async IAsyncEnumerable<PersonRecord> ThenFire(IEnumerable<PersonRecord> items, ManualProgressTimer timer)
    {
        foreach (var item in items)
        {
            yield return item;
        }

        await Task.Yield();
        timer.Fire();
    }
}
