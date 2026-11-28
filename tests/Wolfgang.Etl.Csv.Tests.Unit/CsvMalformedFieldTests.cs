using System;
using System.Text;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvMalformedFieldTests
{
    [Theory]
    [InlineData("a,b\"c,d", 1, "b\"c")]
    [InlineData("\"a\"x,b", 0, "\"a\"x")]
    [InlineData("a,\"b", 1, "\"b")]
    [InlineData("\"a\"\"b\",c\"", 1, "c\"")]
    [InlineData("\"a\",\"b\"z", 1, "\"b\"z")]
    public void Find_returns_the_first_malformed_field(string raw, int expectedIndex, string expectedField)
    {
        var index = CsvMalformedField.Find(raw, ',', '"', out var field);

        Assert.Equal(expectedIndex, index);
        Assert.Equal(expectedField, field);
    }



    [Theory]
    [InlineData("a,b,c")]
    [InlineData("\"a,b\",c")]
    [InlineData("")]
    public void Find_when_no_field_is_malformed_returns_minus_one(string raw)
    {
        Assert.Equal(-1, CsvMalformedField.Find(raw, ',', '"', out var field));
        Assert.Equal(string.Empty, field);
    }



    [Theory]
    [InlineData("a", 0)]
    [InlineData("a\r\nb", 1)]
    [InlineData("a\nb\rc", 2)]
    [InlineData("a\r", 1)]
    [InlineData("\r\n\r\n", 2)]
    public void CountLineBreaks_counts_CRLF_LF_and_lone_CR_once(string text, int expected)
    {
        Assert.Equal(expected, CsvMalformedField.CountLineBreaks(text));
        Assert.Equal(expected, CsvMalformedField.CountLineBreaks(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(text))));
    }
}
