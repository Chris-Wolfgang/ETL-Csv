using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the field conversions <see cref="CsvValueConverter"/> applies to reflection-mapped records:
/// the rules CsvHelper 33's default type converters used before the CsvHelper 34 migration.
/// </summary>
public class CsvValueConverterTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;



    public static TheoryData<string, Type, object> ParsedValues => new()
    {
        { "true", typeof(bool), true },
        { " False ", typeof(bool), false },
        { "1", typeof(bool), true },
        { "0", typeof(bool), false },
        { "x", typeof(char), 'x' },
        { " y ", typeof(char), 'y' },
        { "-5", typeof(sbyte), (sbyte)-5 },
        { "200", typeof(byte), (byte)200 },
        { "-300", typeof(short), (short)-300 },
        { "60000", typeof(ushort), (ushort)60000 },
        { "-70000", typeof(int), -70000 },
        { "4000000000", typeof(uint), 4000000000u },
        { "-9000000000", typeof(long), -9000000000L },
        { "18000000000000000000", typeof(ulong), 18000000000000000000UL },
        { "1.5", typeof(float), 1.5f },
        { "1,234.5", typeof(double), 1234.5d },
        { "1,234.50", typeof(decimal), 1234.50m },
        { "Second", typeof(Sample), Sample.Second },
        { "second", typeof(Sample), Sample.Second },
        { "2", typeof(Sample), Sample.Second },
        { "01:02:03", typeof(TimeSpan), new TimeSpan(1, 2, 3) },
        { "https://example.com/a", typeof(Uri), new Uri("https://example.com/a") },
    };



    [Theory]
    [MemberData(nameof(ParsedValues))]
    public void Parse_converts_the_text_to_the_member_type(string text, Type type, object expected)
    {
        Assert.Equal(expected, CsvValueConverter.Parse(text, type, format: null, Invariant));
    }



    [Fact]
    public void Parse_when_the_type_is_string_returns_the_text_unchanged()
    {
        Assert.Equal(" a ", CsvValueConverter.Parse(" a ", typeof(string), format: null, Invariant));
    }



    [Fact]
    public void Parse_when_the_type_is_string_and_the_field_is_empty_returns_an_empty_string()
    {
        Assert.Equal(string.Empty, CsvValueConverter.Parse(string.Empty, typeof(string), format: null, Invariant));
    }



    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_when_a_nullable_field_is_blank_returns_null(string text)
    {
        Assert.Null(CsvValueConverter.Parse(text, typeof(int?), format: null, Invariant));
    }



    [Fact]
    public void Parse_when_a_nullable_field_has_a_value_returns_it()
    {
        Assert.Equal(7, CsvValueConverter.Parse("7", typeof(int?), format: null, Invariant));
    }



    [Fact]
    public void Parse_when_a_reference_type_field_is_empty_returns_null()
    {
        Assert.Null(CsvValueConverter.Parse(string.Empty, typeof(Uri), format: null, Invariant));
    }



    [Fact]
    public void Parse_when_a_format_is_given_parses_a_DateTime_exactly()
    {
        Assert.Equal
        (
            new DateTime(2026, 11, 26),
            CsvValueConverter.Parse("26/11/2026", typeof(DateTime), "dd/MM/yyyy", Invariant)
        );
    }



    [Fact]
    public void Parse_without_a_format_parses_a_DateTime_with_the_culture()
    {
        Assert.Equal
        (
            new DateTime(2026, 11, 26),
            CsvValueConverter.Parse("26.11.2026", typeof(DateTime), format: null, CultureInfo.GetCultureInfo("de-DE"))
        );
    }



    [Fact]
    public void Parse_when_a_format_is_given_parses_a_DateTimeOffset_exactly()
    {
        Assert.Equal
        (
            new DateTimeOffset(2026, 11, 26, 0, 0, 0, TimeSpan.FromHours(2)),
            CsvValueConverter.Parse("2026-11-26 +02:00", typeof(DateTimeOffset), "yyyy-MM-dd zzz", Invariant)
        );
    }



    [Fact]
    public void Parse_without_a_format_parses_a_DateTimeOffset()
    {
        Assert.Equal
        (
            new DateTimeOffset(2026, 11, 26, 8, 0, 0, TimeSpan.Zero),
            CsvValueConverter.Parse("2026-11-26T08:00:00+00:00", typeof(DateTimeOffset), format: null, Invariant)
        );
    }



    [Fact]
    public void Parse_when_a_format_is_given_parses_a_TimeSpan_exactly()
    {
        Assert.Equal
        (
            new TimeSpan(0, 1, 30),
            CsvValueConverter.Parse("01-30", typeof(TimeSpan), @"mm\-ss", Invariant)
        );
    }



    [Fact]
    public void Parse_reads_a_Guid_with_surrounding_whitespace()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, CsvValueConverter.Parse(" " + guid + " ", typeof(Guid), format: null, Invariant));
    }



#if NET6_0_OR_GREATER
    [Fact]
    public void Parse_reads_DateOnly_with_and_without_a_format()
    {
        Assert.Equal(new DateOnly(2026, 11, 26), CsvValueConverter.Parse("2026-11-26", typeof(DateOnly), format: null, Invariant));
        Assert.Equal(new DateOnly(2026, 11, 26), CsvValueConverter.Parse("26112026", typeof(DateOnly), "ddMMyyyy", Invariant));
    }



    [Fact]
    public void Parse_reads_TimeOnly_with_and_without_a_format()
    {
        Assert.Equal(new TimeOnly(8, 30), CsvValueConverter.Parse("08:30", typeof(TimeOnly), format: null, Invariant));
        Assert.Equal(new TimeOnly(8, 30), CsvValueConverter.Parse("0830", typeof(TimeOnly), "HHmm", Invariant));
    }
#endif



    [Fact]
    public void Parse_falls_back_to_the_type_converter_for_other_types()
    {
        Assert.Equal(new Version(1, 2, 3), CsvValueConverter.Parse("1.2.3", typeof(Version), format: null, Invariant));
    }



    [Theory]
    [InlineData("maybe", typeof(bool))]
    [InlineData("ab", typeof(char))]
    [InlineData("Third", typeof(Sample))]
    [InlineData("x", typeof(int))]
    [InlineData("not a version", typeof(Version))]
    [InlineData("x", typeof(NoTextConversion))]
    public void Parse_when_the_text_is_not_a_value_of_the_type_throws_FormatException(string text, Type type)
    {
        Assert.Throws<FormatException>(() => CsvValueConverter.Parse(text, type, format: null, Invariant));
    }



    [Fact]
    public void Format_writes_null_as_an_empty_field()
    {
        Assert.Equal(string.Empty, CsvValueConverter.Format(null, format: null, Invariant));
    }



    [Fact]
    public void Format_writes_a_string_unchanged()
    {
        Assert.Equal(" a ", CsvValueConverter.Format(" a ", format: "ignored", Invariant));
    }



    [Fact]
    public void Format_applies_the_format_and_culture_to_a_formattable_value()
    {
        Assert.Equal("1.234,50", CsvValueConverter.Format(1234.5m, "N2", CultureInfo.GetCultureInfo("de-DE")));
    }



    [Fact]
    public void Format_without_a_format_uses_the_culture_default()
    {
        Assert.Equal("1234.5", CsvValueConverter.Format(1234.5m, format: null, Invariant));
    }



    [Fact]
    public void Format_writes_a_non_formattable_value_with_ToString()
    {
        Assert.Equal("True", CsvValueConverter.Format(true, format: null, Invariant));
    }



    [Fact]
    public void Format_writes_a_value_whose_ToString_returns_null_as_an_empty_field()
    {
        Assert.Equal(string.Empty, CsvValueConverter.Format(new NullText(), format: null, Invariant));
    }



    public enum Sample
    {
        First = 1,
        Second = 2,
    }



    [ExcludeFromCodeCoverage] // test POCO: only its type metadata is read
    [TypeConverter(typeof(TypeConverter))]
    public sealed class NoTextConversion
    {
    }



    [ExcludeFromCodeCoverage] // test POCO
    public sealed class NullText
    {
        public override string? ToString() => null;
    }
}
