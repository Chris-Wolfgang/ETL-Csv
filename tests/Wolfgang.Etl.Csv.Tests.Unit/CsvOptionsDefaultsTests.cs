using System;
using System.IO;
using System.Text;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Guards the options records' defaults against the base-class defaults they restate.
/// </summary>
/// <remarks>
/// <see cref="CsvExtractorOptions{TRecord}.MaxRecordCount"/> and
/// <see cref="CsvLoaderOptions{TRecord}.MaxRecordCount"/> declare <see cref="int.MaxValue"/>, but the
/// value is really owned by <c>ExtractorBase</c> / <c>LoaderBase</c>. Restating it means a change on
/// the base side would otherwise be silently overridden from here — constructing with any options
/// object would quietly pin the stale value. These tests make that a build failure instead.
/// </remarks>
public class CsvOptionsDefaultsTests
{
    private static StreamReader Reader() => new(new MemoryStream(Encoding.UTF8.GetBytes("A,B\r\n1,2\r\n")));


    private static StreamWriter Writer() => new(new MemoryStream());


    [Fact]
    public void Extractor_constructed_with_empty_options_keeps_the_base_defaults()
    {
        using var reader = Reader();

        var sut = new CsvExtractor<PersonRecord>(reader, new CsvExtractorOptions<PersonRecord>());

        Assert.Equal(int.MaxValue, sut.MaximumItemCount);
        Assert.Equal(0, sut.SkipItemCount);
        Assert.Equal(1, sut.InitialRecordIndex);
    }


    [Fact]
    public void Loader_constructed_with_empty_options_keeps_the_base_defaults()
    {
        using var writer = Writer();

        var sut = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord>());

        Assert.Equal(int.MaxValue, sut.MaximumItemCount);
        Assert.Equal(0, sut.SkipItemCount);
    }


    [Fact]
    public void Extractor_constructed_without_options_keeps_the_base_defaults()
    {
        using var reader = Reader();

        var sut = new CsvExtractor<PersonRecord>(reader);

        Assert.Equal(int.MaxValue, sut.MaximumItemCount);
        Assert.Equal(0, sut.SkipItemCount);
        Assert.Equal(1, sut.InitialRecordIndex);
    }


    [Fact]
    public void Options_records_default_to_skipping_blank_lines_and_leaving_the_stream_open()
    {
        var extractorOptions = new CsvExtractorOptions<PersonRecord>();
        var loaderOptions = new CsvLoaderOptions<PersonRecord>();

        Assert.True(extractorOptions.IgnoreBlankLines);
        Assert.True(extractorOptions.LeaveOpen);
        Assert.True(loaderOptions.LeaveOpen);
    }


    [Fact]
    public void ExtractorOptions_InitialRecordIndex_accepts_one()
    {
        var options = new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 1 };

        Assert.Equal(1, options.InitialRecordIndex);
    }


    [Fact]
    public void ExtractorOptions_InitialRecordIndex_below_one_reports_the_value_and_message()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new CsvExtractorOptions<PersonRecord> { InitialRecordIndex = 0 }
        );

        Assert.Equal(0, ex.ActualValue);
        Assert.StartsWith("InitialRecordIndex must be 1 or greater.", ex.Message, StringComparison.Ordinal);
    }
}
