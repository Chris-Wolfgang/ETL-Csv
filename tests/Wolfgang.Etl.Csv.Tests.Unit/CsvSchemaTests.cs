using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvSchemaTests
{
    private static StreamReader Reader(string csv, Encoding? encoding = null)
    {
        var enc = encoding ?? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        var stream = new MemoryStream(enc.GetPreamble().Concat(enc.GetBytes(csv)).ToArray());
        return new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
    }



    private const string TypedCsv =
        "flag,count,big,amount,when,id,note\n" +
        "true,5,3000000000,1.5,2023-01-15,550e8400-e29b-41d4-a716-446655440000,hello\n" +
        "false,7,4000000000,2.75,2023-02-20,6ba7b810-9dad-11d1-80b4-00c04fd430c8,world\n";



    [Fact]
    public async Task InferAsync_classifies_each_column_down_the_type_ladder()
    {
        using var reader = Reader(TypedCsv);

        var schema = await CsvSchema.InferAsync(reader);

        Type TypeOf(string name) => schema.Columns.Single(c => string.Equals(c.Name, name, StringComparison.Ordinal)).InferredType;

        Assert.Equal(typeof(bool), TypeOf("flag"));
        Assert.Equal(typeof(int), TypeOf("count"));
        Assert.Equal(typeof(long), TypeOf("big"));
        Assert.Equal(typeof(decimal), TypeOf("amount"));
        Assert.Equal(typeof(DateTime), TypeOf("when"));
        Assert.Equal(typeof(Guid), TypeOf("id"));
        Assert.Equal(typeof(string), TypeOf("note"));
    }



    [Fact]
    public async Task InferAsync_records_a_date_format_and_column_indices()
    {
        using var reader = Reader(TypedCsv);

        var schema = await CsvSchema.InferAsync(reader);

        var when = schema.Columns.Single(c => string.Equals(c.Name, "when", StringComparison.Ordinal));
        Assert.Equal("yyyy-MM-dd", when.Format);
        Assert.Equal(4, when.Index);
    }



    [Fact]
    public async Task InferAsync_widens_a_mixed_column_to_string()
    {
        using var reader = Reader("value\n1\nabc\n3\n");

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Equal(typeof(string), Assert.Single(schema.Columns).InferredType);
    }



    [Fact]
    public async Task InferAsync_marks_a_column_nullable_when_a_sampled_value_is_blank()
    {
        using var reader = Reader("a,b\n1,x\n2,\n3,z\n");

        var schema = await CsvSchema.InferAsync(reader);

        Assert.False(schema.Columns.Single(c => string.Equals(c.Name, "a", StringComparison.Ordinal)).Nullable);
        Assert.True(schema.Columns.Single(c => string.Equals(c.Name, "b", StringComparison.Ordinal)).Nullable);
    }



    [Theory]
    [InlineData("a,b,c\n1,2,3\n", ",")]
    [InlineData("a\tb\tc\n1\t2\t3\n", "\t")]
    [InlineData("a|b|c\n1|2|3\n", "|")]
    [InlineData("a;b;c\n1;2;3\n", ";")]
    public async Task InferAsync_auto_detects_the_delimiter(string csv, string expectedDelimiter)
    {
        using var reader = Reader(csv);

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Equal(expectedDelimiter, schema.Delimiter);
        Assert.Equal(3, schema.Columns.Count);
    }



    [Fact]
    public async Task InferAsync_without_a_header_generates_positional_column_names()
    {
        using var reader = Reader("1,2,3\n4,5,6\n");

        var schema = await CsvSchema.InferAsync(reader, new CsvSchemaInferenceOptions { HasHeaderRecord = false });

        Assert.Equal(new[] { "Column1", "Column2", "Column3" }, schema.Columns.Select(c => c.Name));
        Assert.Equal(typeof(int), schema.Columns[0].InferredType);
    }



    [Fact]
    public async Task InferAsync_detects_utf8_without_a_bom()
    {
        using var reader = Reader(TypedCsv, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Equal("utf-8", schema.Encoding.WebName);
    }



    [Fact]
    public async Task InferAsync_detects_a_utf16_bom()
    {
        using var reader = Reader(TypedCsv, Encoding.Unicode);   // UTF-16 LE, emits a BOM

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Equal("utf-16", schema.Encoding.WebName);
    }



    [Fact]
    public async Task InferAsync_rejects_a_null_reader()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => CsvSchema.InferAsync(null!));
    }



    [Fact]
    public async Task InferAsync_treats_an_all_blank_column_as_nullable_string()
    {
        using var reader = Reader("a,b\n1,\n2,\n");

        var schema = await CsvSchema.InferAsync(reader);

        var b = schema.Columns.Single(c => string.Equals(c.Name, "b", StringComparison.Ordinal));
        Assert.Equal(typeof(string), b.InferredType);
        Assert.True(b.Nullable);
    }



    [Fact]
    public async Task InferAsync_leaves_Format_null_for_a_date_no_candidate_pattern_matches()
    {
        using var reader = Reader("when\nJanuary 5, 2023\nMarch 12, 2023\n");

        var schema = await CsvSchema.InferAsync(reader);

        var when = Assert.Single(schema.Columns);
        Assert.Equal(typeof(DateTime), when.InferredType);
        Assert.Null(when.Format);
    }



    [Fact]
    public async Task InferAsync_on_empty_input_yields_no_columns_and_the_default_delimiter()
    {
        using var reader = Reader(string.Empty);

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Empty(schema.Columns);
        Assert.Equal(",", schema.Delimiter);
    }



    [Fact]
    public void FromJson_rejects_null_and_a_json_null_literal()
    {
        Assert.Throws<ArgumentNullException>(() => CsvSchema.FromJson(null!));
        Assert.Throws<FormatException>(() => CsvSchema.FromJson("null"));
    }



    [Fact]
    public void FromJson_wraps_malformed_json_in_a_FormatException()
    {
        Assert.Throws<FormatException>(() => CsvSchema.FromJson("{ not valid json"));
    }



    [Fact]
    public async Task InferAsync_falls_back_to_a_positional_name_for_a_blank_header_cell()
    {
        using var reader = Reader("a,,c\n1,2,3\n");

        var schema = await CsvSchema.InferAsync(reader);

        Assert.Equal(new[] { "a", "Column2", "c" }, schema.Columns.Select(c => c.Name));
    }



    [Fact]
    public async Task InferAsync_ignores_blank_lines_between_records()
    {
        using var reader = Reader("n\n1\n\n2\n\n3\n");

        var schema = await CsvSchema.InferAsync(reader);

        var column = Assert.Single(schema.Columns);
        Assert.Equal(typeof(int), column.InferredType);
        Assert.False(column.Nullable);   // the blank lines were skipped, not treated as empty values
    }



    [Fact]
    public async Task Schema_round_trips_through_json()
    {
        using var reader = Reader(TypedCsv);
        var schema = await CsvSchema.InferAsync(reader);

        var json = schema.ToJson();
        var restored = CsvSchema.FromJson(json);

        Assert.Equal(schema.Delimiter, restored.Delimiter);
        Assert.Equal(schema.HasHeaderRecord, restored.HasHeaderRecord);
        Assert.Equal(schema.Encoding.WebName, restored.Encoding.WebName);
        Assert.Equal(schema.Columns.Count, restored.Columns.Count);
        Assert.Equal(typeof(Guid), restored.Columns.Single(c => string.Equals(c.Name, "id", StringComparison.Ordinal)).InferredType);
        Assert.Equal("yyyy-MM-dd", restored.Columns.Single(c => string.Equals(c.Name, "when", StringComparison.Ordinal)).Format);
    }



    [Fact]
    public async Task ToColumnMaps_seeds_an_extractor_that_reads_the_source()
    {
        const string csv = "FirstName,LastName,Age\nAlice,Smith,30\nBob,Jones,25\n";
        CsvSchema schema;
        using (var schemaReader = Reader(csv))
        {
            schema = await CsvSchema.InferAsync(schemaReader);
        }

        using var dataReader = Reader(csv);
        var extractor = new CsvExtractor<Person>(dataReader)
        {
            ColumnMaps = schema.ToColumnMaps(),
        };

        var people = new List<Person>();
        await foreach (var person in extractor.ExtractAsync())
        {
            people.Add(person);
        }

        Assert.Equal(2, people.Count);
        Assert.Equal("Alice", people[0].FirstName);
        Assert.Equal(30, people[0].Age);
        Assert.Equal("Jones", people[1].LastName);
    }



    [ExcludeFromCodeCoverage]
    public record Person
    {
        public string FirstName { get; set; } = string.Empty;



        public string LastName { get; set; } = string.Empty;



        public int Age { get; set; }
    }
}
