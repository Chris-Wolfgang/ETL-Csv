using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Wolfgang.Etl.TestKit.Xunit;
using Xunit;

// The observation callbacks and the InitialRecordIndex setter are deprecated but still shipped, so
// they are still pinned here.
#pragma warning disable CS0618

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins observable details of <see cref="CsvExtractor{TRecord}"/> that the contract and round-trip
/// tests do not look at: what it logs, the line number it reports, the header and missing-field
/// checks, and what the observation callbacks receive.
/// </summary>
public class CsvExtractorWorkerTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    private static StreamReader ReaderOver(string content) => new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);



    private static async Task<List<T>> ReadAllAsync<T>(CsvExtractor<T> sut)
        where T : notnull
    {
        var list = new List<T>();
        await foreach (var item in sut.ExtractAsync())
        {
            list.Add(item);
        }

        return list;
    }



    [Fact]
    public async Task ExtractAsync_logs_ignored_rows_skips_and_the_operation_name()
    {
        using var reader = ReaderOver("banner\r\nFirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n");
        var logger = new RecordingLogger<CsvExtractor<PersonRecord>>();
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                InitialRecordIndex = 2,
                SkipItemCount = 1,
            },
            logger
        );

        var rows = await ReadAllAsync(sut);

        Assert.Equal("Bob", Assert.Single(rows).FirstName);
        Assert.Equal
        (
            new[]
            {
                "Starting CSV extraction of PersonRecord.",
                "Ignored row 1 before InitialRecordIndex.",
                "Skipped item 1 of 1.",
                "Extracted item 1.",
                "CSV extraction completed. Extracted: 1, skipped: 1.",
            },
            logger.Entries.Select(e => e.Message)
        );
    }



    [Theory]
    [InlineData("FirstName,LastName,Age\r\nAlice,Smith,30\r\n", 1, 0, 2)]
    [InlineData("FirstName,LastName,Age\r\n", 1, 0, 1)]
    [InlineData("banner\r\nmore banner\r\n", 3, 0, 2)]
    [InlineData("FirstName,LastName,Age\r\nAlice,Smith,30\r\n", 1, 1, 2)]
    public async Task ExtractAsync_reports_the_last_line_read(string csv, int initialRecordIndex, int skip, int expectedLine)
    {
        using var reader = ReaderOver(csv);
        var timer = new ManualProgressTimer();
        var sut = new CsvExtractor<PersonRecord>(reader, timer)
        {
            SkipItemCount = skip,
            InitialRecordIndex = initialRecordIndex,
        };
        var progress = new SyncProgress<CsvExtractorProgress>();

        await foreach (var _ in sut.ExtractAsync(progress))
        {
        }

        Assert.Equal(expectedLine, progress.LastValue!.CurrentLineNumber);
    }



    [Fact]
    public async Task ExtractAsync_with_an_injected_timer_reports_progress_each_time_it_fires()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n");
        var timer = new ManualProgressTimer();
        var sut = new CsvExtractor<PersonRecord>(reader, timer);
        var progress = new SyncProgress<CsvExtractorProgress>();
        var reportsPerFire = new List<int>();

        await foreach (var _ in sut.ExtractAsync(progress))
        {
            var before = progress.CallCount;
            timer.Fire();
            reportsPerFire.Add(progress.CallCount - before);
        }

        // Each Fire produced exactly one report: the Elapsed handler is wired once.
        Assert.Equal(new[] { 1, 1 }, reportsPerFire);
    }



    [Fact]
    public void Constructor_without_options_skips_blank_lines()
    {
        using var reader = ReaderOver(string.Empty);

        var sut = new CsvExtractor<PersonRecord>(reader);

        Assert.True(sut.IgnoreBlankLines);
    }



    [Fact]
    public void InitialRecordIndex_setter_accepts_one()
    {
        using var reader = ReaderOver(string.Empty);
        var sut = new CsvExtractor<PersonRecord>(reader);

        sut.InitialRecordIndex = 1;

        Assert.Equal(1, sut.InitialRecordIndex);
    }



    [Fact]
    public void InitialRecordIndex_setter_below_one_reports_the_value_and_message()
    {
        using var reader = ReaderOver(string.Empty);
        var sut = new CsvExtractor<PersonRecord>(reader);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => sut.InitialRecordIndex = 0);

        Assert.Equal(0, ex.ActualValue);
        Assert.StartsWith("InitialRecordIndex must be 1 or greater.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public async Task ExtractAsync_when_the_header_lacks_a_mapped_column_throws_HeaderValidationException()
    {
        using var reader = ReaderOver("FirstName,LastName\r\nAlice,Smith\r\n");
        var sut = new CsvExtractor<PersonRecord>(reader);

        await Assert.ThrowsAsync<CsvHelper.HeaderValidationException>(async () => await ReadAllAsync(sut));
    }



    [Fact]
    public async Task ExtractAsync_when_a_row_is_missing_a_field_throws_MissingFieldException()
    {
        using var reader = ReaderOver("Alice,Smith\r\n");
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                HasHeaderRecord = false,
            }
        );

        await Assert.ThrowsAsync<CsvHelper.MissingFieldException>(async () => await ReadAllAsync(sut));
    }



    [Fact]
    public async Task ExtractAsync_with_a_discriminator_tolerates_a_row_missing_trailing_fields()
    {
        var discriminator = new CsvDiscriminatorBuilder<PersonRecord>(0)
            .Map<PersonRecord>
            (
                "P",
                new[]
                {
                    new CsvColumnMap(nameof(PersonRecord.FirstName)) { Index = 0 },
                    new CsvColumnMap(nameof(PersonRecord.LastName)) { Index = 1 },
                }
            )
            .Build();
        using var reader = ReaderOver("P\r\n");
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                HasHeaderRecord = false,
                Discriminator = discriminator,
            }
        );

        var row = Assert.Single(await ReadAllAsync(sut));

        Assert.Equal("P", row.FirstName);
        Assert.Equal(string.Empty, row.LastName);
    }



    [Fact]
    public async Task ExtractAsync_by_name_discriminator_reads_the_named_column_wherever_it_is()
    {
        var discriminator = new CsvDiscriminatorBuilder<PersonRecord>("Kind")
            .Map<PersonRecord>
            (
                "P",
                new[]
                {
                    new CsvColumnMap(nameof(PersonRecord.FirstName)) { Name = "FirstName" },
                }
            )
            .Build();
        using var reader = ReaderOver("FirstName,Kind\r\nAlice,P\r\n");
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                Discriminator = discriminator,
            }
        );

        Assert.Equal("Alice", Assert.Single(await ReadAllAsync(sut)).FirstName);
    }



    [Fact]
    public async Task ExtractAsync_when_discriminator_value_is_unmapped_and_action_is_Throw_names_the_value()
    {
        var discriminator = new CsvDiscriminatorBuilder<PersonRecord>(0)
            .Map<PersonRecord>("P")
            .Build();
        using var reader = ReaderOver("Q,Smith,1\r\n");
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                HasHeaderRecord = false,
                Discriminator = discriminator,
            }
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await ReadAllAsync(sut));

        Assert.Equal("No record type is mapped for discriminator value 'Q'.", ex.Message);
    }



    [Fact]
    public async Task ExtractAsync_when_ColumnMaps_is_empty_uses_the_default_mapping()
    {
        // An empty list means "no runtime maps"; it must not reach BuildFromColumnMaps, which
        // rejects an empty list.
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                ColumnMaps = Array.Empty<CsvColumnMap>(),
            }
        );

        Assert.Equal(30, Assert.Single(await ReadAllAsync(sut)).Age);
    }



    [Fact]
    public async Task BadDataFound_receives_the_line_and_one_based_column()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Sm" + '"' + "ith,30\r\n");
        var sut = new CsvExtractor<PersonRecord>(reader);
        var seen = new List<CsvBadDataInfo>();
        sut.BadDataFound = seen.Add;

        await ReadAllAsync(sut);

        var info = Assert.Single(seen);
        Assert.Equal(2, info.LineNumber);
        Assert.Equal(2, info.ColumnNumber);
    }



    [Theory]
    [InlineData("FirstName,LastName,Age\r\nAl", "ice,Smith,30\r\n", 2, 1)]
    [InlineData("First", "Name,LastName,Age\r\nAlice,Smith,30\r\n", 1, -1)]
    public async Task BadDataFound_reports_the_column_of_the_bad_field(string before, string after, int expectedLine, int expectedColumn)
    {
        using var reader = ReaderOver(before + '"' + after);
        var sut = new CsvExtractor<PersonRecord>(reader);
        var seen = new List<CsvBadDataInfo>();
        sut.BadDataFound = seen.Add;

        try
        {
            await ReadAllAsync(sut);
        }
        catch (CsvHelper.CsvHelperException)
        {
            // A bad header also fails header validation; only the callback matters here.
        }

        var info = Assert.Single(seen);
        Assert.Equal(expectedLine, info.LineNumber);
        Assert.Equal(expectedColumn, info.ColumnNumber);
    }



    [Fact]
    public async Task ReadingExceptionOccurred_names_the_failing_column_and_its_value()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,old\r\n");
        var info = await CaptureReadingExceptionAsync(reader, columnMaps: null, hasHeader: true);

        Assert.Equal(2, info.LineNumber);
        Assert.Equal(3, info.ColumnNumber);
        Assert.Equal("Age", info.ColumnName);
        Assert.Equal("old", info.ColumnValue);
    }



    [Fact]
    public async Task ReadingExceptionOccurred_for_the_first_column_reports_column_one()
    {
        using var reader = ReaderOver("Age,FirstName,LastName\r\nold,Alice,Smith\r\n");
        var maps = new[]
        {
            new CsvColumnMap(nameof(PersonRecord.Age)) { Index = 0 },
            new CsvColumnMap(nameof(PersonRecord.FirstName)) { Index = 1 },
            new CsvColumnMap(nameof(PersonRecord.LastName)) { Index = 2 },
        };

        var info = await CaptureReadingExceptionAsync(reader, maps, hasHeader: true);

        Assert.Equal(1, info.ColumnNumber);
        Assert.Equal("Age", info.ColumnName);
        Assert.Equal("old", info.ColumnValue);
    }



    [Fact]
    public async Task ReadingExceptionOccurred_when_the_column_is_past_the_header_has_no_name()
    {
        using var reader = ReaderOver("FirstName,LastName\r\nAlice,Smith,old\r\n");
        var maps = new[]
        {
            new CsvColumnMap(nameof(PersonRecord.FirstName)) { Index = 0 },
            new CsvColumnMap(nameof(PersonRecord.LastName)) { Index = 1 },
            new CsvColumnMap(nameof(PersonRecord.Age)) { Index = 2 },
        };

        var info = await CaptureReadingExceptionAsync(reader, maps, hasHeader: true);

        Assert.Equal(3, info.ColumnNumber);
        Assert.Null(info.ColumnName);
        Assert.Equal("old", info.ColumnValue);
    }



    [Fact]
    public async Task ReadingExceptionOccurred_when_the_field_is_missing_has_no_value()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith\r\n");

        var info = await CaptureReadingExceptionAsync(reader, columnMaps: null, hasHeader: true);

        Assert.Equal(3, info.ColumnNumber);
        Assert.Equal("Age", info.ColumnName);
        Assert.Null(info.ColumnValue);
    }



    [Fact]
    public async Task ReadingExceptionOccurred_without_a_header_has_no_column_name()
    {
        using var reader = ReaderOver("Alice,Smith,old\r\n");

        var info = await CaptureReadingExceptionAsync(reader, columnMaps: null, hasHeader: false);

        Assert.Equal(3, info.ColumnNumber);
        Assert.Null(info.ColumnName);
        Assert.Equal("old", info.ColumnValue);
    }



    private static async Task<CsvReadingExceptionInfo> CaptureReadingExceptionAsync
    (
        StreamReader reader,
        IReadOnlyList<CsvColumnMap>? columnMaps,
        bool hasHeader
    )
    {
        var sut = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                ColumnMaps = columnMaps,
                HasHeaderRecord = hasHeader,
            }
        );
        var seen = new List<CsvReadingExceptionInfo>();
        sut.ReadingExceptionOccurred = seen.Add;

        await Assert.ThrowsAnyAsync<CsvHelper.CsvHelperException>(async () => await ReadAllAsync(sut));

        return Assert.Single(seen);
    }
}
