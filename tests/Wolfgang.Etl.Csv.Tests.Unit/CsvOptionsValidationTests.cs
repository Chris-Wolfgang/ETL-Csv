using System;
using System.IO;
using System.Text;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvOptionsValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CsvExtractor_when_Delimiter_is_null_or_empty_throws_naming_options(string? delimiter)
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvExtractor<PersonRecord>(NewReader(), new CsvExtractorOptions<PersonRecord> { Delimiter = delimiter! })
        );

        Assert.Equal("options", ex.ParamName);
        Assert.Contains("Delimiter", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void CsvExtractor_when_a_name_based_Discriminator_has_no_header_record_throws_naming_options()
    {
        var discriminator = new CsvDiscriminatorBuilder<PersonRecord>("Kind")
            .Map<PersonRecord>("p")
            .Build();

        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvExtractor<PersonRecord>
            (
                NewReader(),
                new CsvExtractorOptions<PersonRecord> { HasHeaderRecord = false, Discriminator = discriminator }
            )
        );

        Assert.Equal("options", ex.ParamName);
        Assert.Contains("HasHeaderRecord", ex.Message, StringComparison.Ordinal);
    }



    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CsvLoader_when_Delimiter_is_null_or_empty_throws_naming_options(string? delimiter)
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvLoader<PersonRecord>(NewWriter(), new CsvLoaderOptions<PersonRecord> { Delimiter = delimiter! })
        );

        Assert.Equal("options", ex.ParamName);
        Assert.Contains("Delimiter", ex.Message, StringComparison.Ordinal);
    }



    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CsvLoader_when_NewLine_is_null_or_empty_throws_naming_options(string? newLine)
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvLoader<PersonRecord>(NewWriter(), new CsvLoaderOptions<PersonRecord> { NewLine = newLine! })
        );

        Assert.Equal("options", ex.ParamName);
        Assert.Contains("NewLine", ex.Message, StringComparison.Ordinal);
    }



    private static StreamReader NewReader() => new(new MemoryStream(Encoding.UTF8.GetBytes("a\n")));



    private static StreamWriter NewWriter() => new(new MemoryStream());
}
