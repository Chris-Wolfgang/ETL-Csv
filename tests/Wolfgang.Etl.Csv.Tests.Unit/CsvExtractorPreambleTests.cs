using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvExtractorPreambleTests
{
    [Fact]
    public async Task ExtractAsync_when_preamble_has_a_blank_line_still_reads_the_header()
    {
        var items = await ExtractAsync
        (
            "banner one\nbanner two\n\nFirstName,LastName,Age\nAnn,Lee,1\nBob,Ray,2\n",
            new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 4 }
        );

        Assert.Equal(2, items.Count);
        Assert.Equal("Ann", items[0].FirstName);
    }



    [Fact]
    public async Task ExtractAsync_when_preamble_has_a_comment_line_still_reads_the_header()
    {
        var items = await ExtractAsync
        (
            "banner one\n#banner two\nFirstName,LastName,Age\nAnn,Lee,1\nBob,Ray,2\n",
            new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 3, AllowComments = true }
        );

        Assert.Equal(2, items.Count);
    }



    [Fact]
    public async Task ExtractAsync_when_preamble_has_an_unbalanced_quote_still_reads_the_records()
    {
        var items = await ExtractAsync
        (
            "\"Report for Q3\nsecond\nFirstName,LastName,Age\nAnn,Lee,1\nBob,Ray,2\n",
            new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 3 }
        );

        Assert.Equal(2, items.Count);
    }



    [Fact]
    public async Task ExtractAsync_when_preamble_is_skipped_line_numbers_stay_physical_source_lines()
    {
        var csv = "banner one\nbanner two\nFirstName,LastName,Age\nAnn,Lee,1\nBob,Ray,x\n";
        var info = default(CsvReadingExceptionInfo);
        var sut = new CsvExtractor<PersonRecord>
        (
            new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv)), Encoding.UTF8),
            new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 3 }
        )
        {
            ErrorPolicy = _ => ItemErrorAction.Skip,
        };
#pragma warning disable CS0618 // observation-only callback
        sut.ReadingExceptionOccurred = i => info = i;
#pragma warning restore CS0618

        await foreach (var _ in sut.ExtractAsync())
        {
        }

        Assert.NotNull(info);
        Assert.Equal(5, info!.LineNumber);
    }



    private static async Task<List<PersonRecord>> ExtractAsync(string csv, CsvExtractorOptions<PersonRecord> options)
    {
        var sut = new CsvExtractor<PersonRecord>
        (
            new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv)), Encoding.UTF8),
            options
        );

        var results = new List<PersonRecord>();
        await foreach (var item in sut.ExtractAsync())
        {
            results.Add(item);
        }

        return results;
    }
}
