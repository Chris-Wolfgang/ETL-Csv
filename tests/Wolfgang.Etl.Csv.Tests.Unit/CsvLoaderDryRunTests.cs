using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvLoaderDryRunTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private static readonly PersonRecord[] People =
    {
        new() { FirstName = "Alice", LastName = "Smith", Age = 30 },
        new() { FirstName = "Bob", LastName = "Jones", Age = 25 },
    };


    [Fact]
    public void IsDryRun_defaults_to_false()
    {
        using var writer = new StreamWriter(new MemoryStream());

        Assert.False(new CsvLoader<PersonRecord>(writer).IsDryRun);
    }


    [Fact]
    public void CsvLoader_implements_ISupportDryRun()
    {
        using var writer = new StreamWriter(new MemoryStream());

        Assert.IsAssignableFrom<ISupportDryRun>(new CsvLoader<PersonRecord>(writer));
    }


    [Fact]
    public async Task LoadAsync_when_IsDryRun_writes_nothing_to_the_output()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true))
        {
            var loader = new CsvLoader<PersonRecord>(writer) { LeaveOpen = true, IsDryRun = true };
            await loader.LoadAsync(ToAsync(People));
            await writer.FlushAsync();
        }

        // Not even the header is written.
        Assert.Empty(stream.ToArray());
    }


    [Fact]
    public async Task LoadAsync_when_IsDryRun_still_enumerates_and_counts_every_item()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<PersonRecord>(writer)
        {
            LeaveOpen = true,
            IsDryRun = true,
        };

        await loader.LoadAsync(ToAsync(People));

        // The full pipeline ran — items were enumerated and counted, just not written.
        Assert.Equal(People.Length, loader.CurrentItemCount);
    }


    [Fact]
    public async Task LoadAsync_when_IsDryRun_still_honors_SkipRecordCount()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<PersonRecord>(writer)
        {
            LeaveOpen = true,
            IsDryRun = true,
            SkipRecordCount = 1,
        };

        await loader.LoadAsync(ToAsync(People));

        Assert.Equal(1, loader.CurrentSkippedItemCount);
        Assert.Equal(People.Length - 1, loader.CurrentItemCount);
        Assert.Empty(stream.ToArray());
    }


    [Fact]
    public async Task LoadAsync_when_not_dry_run_writes_the_header_and_records()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true))
        {
            var loader = new CsvLoader<PersonRecord>(writer) { LeaveOpen = true };
            await loader.LoadAsync(ToAsync(People));
            await writer.FlushAsync();
        }

        var output = Utf8NoBom.GetString(stream.ToArray());

        Assert.Contains("FirstName", output);
        Assert.Contains("Alice", output);
    }


    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
