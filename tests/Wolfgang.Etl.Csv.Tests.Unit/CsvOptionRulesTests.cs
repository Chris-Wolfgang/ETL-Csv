using System;
using System.IO;
using System.Text;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the dialect limits CsvHelper 34 puts on the extractor and loader options.
/// </summary>
public class CsvOptionRulesTests
{
    [Theory]
    [InlineData(",", ',')]
    [InlineData("\t", '\t')]
    [InlineData("|", '|')]
    public void ToSeparator_returns_the_single_ASCII_character(string delimiter, char expected)
    {
        Assert.Equal(expected, CsvOptionRules.ToSeparator(delimiter));
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("::")]
    [InlineData("§")]
    public void ToSeparator_when_the_delimiter_is_not_one_ASCII_character_throws(string? delimiter)
    {
        var ex = Assert.Throws<ArgumentException>(() => CsvOptionRules.ToSeparator(delimiter));

        Assert.Equal("Delimiter", ex.ParamName);
    }



    [Theory]
    [InlineData('"', '"')]
    [InlineData('\'', '\'')]
    [InlineData('\'', '"')]
    public void ToQuote_accepts_the_quote_itself_or_the_default_escape(char quote, char escape)
    {
        Assert.Equal(quote, CsvOptionRules.ToQuote(quote, escape));
    }



    [Fact]
    public void ToQuote_when_the_escape_is_another_character_throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => CsvOptionRules.ToQuote('"', '\\'));

        Assert.Equal("Quote", ex.ParamName);
    }



    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    public void ToNewLine_returns_a_line_ending_CsvHelper_writes(string newLine)
    {
        Assert.Equal(newLine, CsvOptionRules.ToNewLine(newLine));
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\n\n")]
    [InlineData(";")]
    public void ToNewLine_when_the_line_ending_is_not_supported_throws(string? newLine)
    {
        var ex = Assert.Throws<ArgumentException>(() => CsvOptionRules.ToNewLine(newLine));

        Assert.Equal("NewLine", ex.ParamName);
    }



    [Fact]
    public void CsvLoader_constructor_when_NewLine_is_not_supported_rejects_it()
    {
        using var writer = new StreamWriter(new MemoryStream(), Encoding.UTF8);

        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord> { NewLine = "|" })
        );

        Assert.Equal("options", ex.ParamName);
    }



    [Fact]
    public void CsvLoader_constructor_when_Delimiter_has_two_characters_rejects_it()
    {
        using var writer = new StreamWriter(new MemoryStream(), Encoding.UTF8);

        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord> { Delimiter = "||" })
        );

        Assert.Equal("options", ex.ParamName);
    }
}
