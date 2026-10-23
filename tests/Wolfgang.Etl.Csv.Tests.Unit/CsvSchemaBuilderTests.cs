using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvSchemaBuilderTests
{
    [Fact]
    public void Build_maps_every_field_and_binds_the_schema_to_T()
    {
        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Column(p => p.Age, index: 2, format: "0", optional: true, @default: "0")
            .Build();

        Assert.Equal(typeof(PersonRecord), schema.RecordType);
        Assert.Equal(2, schema.ColumnCount);

        Assert.Equal
        (
            new CsvColumnMap("FirstName") { Name = "first_name" },
            schema.Columns[0]
        );

        Assert.Equal
        (
            new CsvColumnMap("Age") { Index = 2, Format = "0", Optional = true, Default = "0" },
            schema.Columns[1]
        );
    }



    [Fact]
    public void Build_when_no_columns_were_added_throws_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new CsvSchemaBuilder<PersonRecord>().Build());
    }



    [Fact]
    public void Build_returns_columns_that_cannot_be_cast_back_to_a_mutable_array()
    {
        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Build();

        Assert.IsNotType<CsvColumnMap[]>(schema.Columns);
    }



    [Fact]
    public void Column_resolves_a_value_type_property_selected_through_boxing()
    {
        // p => p.Age is Func<PersonRecord, int>; the selector resolves through the Convert-unwrapping path.
        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.Age, name: "age")
            .Build();

        Assert.Equal("Age", schema.Columns[0].PropertyName);
    }



    [Fact]
    public void Column_accepts_an_inherited_property()
    {
        var schema = new CsvSchemaBuilder<DerivedRecord>()
            .Column(d => d.BaseName, name: "base_name")
            .Build();

        Assert.Equal("BaseName", schema.Columns[0].PropertyName);
    }



    [Fact]
    public void Column_when_selector_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column<string>(null!)
        );
    }



    [Fact]
    public void Column_when_index_is_below_negative_one_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName, index: -2)
        );
    }



    [Theory]
    [MemberData(nameof(InvalidSelectors))]
    public void Column_when_selector_is_not_a_direct_property_throws_ArgumentException(Action act)
    {
        Assert.Throws<ArgumentException>(act);
    }



    public static IEnumerable<object[]> InvalidSelectors()
    {
        // Method call, not a member access.
        yield return new object[] { (Action)(() => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName.Trim())) };
        // Nested member (property of a property), not declared on the parameter.
        yield return new object[] { (Action)(() => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName.Length)) };
        // A field, not a property.
        yield return new object[] { (Action)(() => new CsvSchemaBuilder<WithField>().Column(w => w.Tag)) };
        // A constant, no member at all.
        yield return new object[] { (Action)(() => new CsvSchemaBuilder<PersonRecord>().Column(_ => "literal")) };
    }



    [Fact]
    public async Task ExtractAsync_with_a_code_built_schema_is_equivalent_to_the_attributed_record()
    {
        var csv = "first_name,last_name,age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n";

        // Attribute path: AttributedPersonRecord maps FirstName/LastName/Age to the snake_case columns.
        var viaAttributes = await ExtractAsync(new CsvExtractor<AttributedPersonRecord>(Reader(csv)));

        // Schema path: the SAME layout expressed on the undecorated PersonRecord via the builder.
        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Column(p => p.LastName, name: "last_name")
            .Column(p => p.Age, name: "age")
            .Build();
        var viaSchema = await ExtractAsync
        (
            new CsvExtractor<PersonRecord>(Reader(csv), new CsvExtractorOptions<PersonRecord> { Schema = schema })
        );

        Assert.Equal(viaAttributes.Count, viaSchema.Count);
        for (var i = 0; i < viaAttributes.Count; i++)
        {
            Assert.Equal(viaAttributes[i].FirstName, viaSchema[i].FirstName);
            Assert.Equal(viaAttributes[i].LastName, viaSchema[i].LastName);
            Assert.Equal(viaAttributes[i].Age, viaSchema[i].Age);
        }
    }



    [Fact]
    public async Task LoadAsync_with_a_code_built_schema_is_equivalent_to_the_attributed_record()
    {
        var attributed = new[]
        {
            new AttributedPersonRecord { FirstName = "Alice", LastName = "Smith", Age = 30 },
            new AttributedPersonRecord { FirstName = "Bob", LastName = "Jones", Age = 25 },
        };
        var plain = new[]
        {
            new PersonRecord { FirstName = "Alice", LastName = "Smith", Age = 30 },
            new PersonRecord { FirstName = "Bob", LastName = "Jones", Age = 25 },
        };

        var viaAttributes = await LoadToStringAsync
        (
            w => new CsvLoader<AttributedPersonRecord>(w, new CsvLoaderOptions<AttributedPersonRecord> { LeaveOpen = true }),
            attributed
        );

        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Column(p => p.LastName, name: "last_name")
            .Column(p => p.Age, name: "age")
            .Build();
        var viaSchema = await LoadToStringAsync
        (
            w => new CsvLoader<PersonRecord>(w, new CsvLoaderOptions<PersonRecord> { LeaveOpen = true, Schema = schema }),
            plain
        );

        Assert.Equal(viaAttributes, viaSchema);
    }



    private static StreamReader Reader(string csv) =>
        new(new MemoryStream(Encoding.UTF8.GetBytes(csv)), Encoding.UTF8);



    private static async Task<List<T>> ExtractAsync<T>(CsvExtractor<T> extractor)
        where T : notnull
    {
        var results = new List<T>();
        await foreach (var item in extractor.ExtractAsync().ConfigureAwait(false))
        {
            results.Add(item);
        }

        return results;
    }



    private static async Task<string> LoadToStringAsync<T>(Func<StreamWriter, CsvLoader<T>> create, IReadOnlyList<T> records)
        where T : notnull
    {
        using var stream = new MemoryStream();
        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        using (var writer = new StreamWriter(stream, utf8NoBom, 1024, leaveOpen: true))
        {
            var loader = create(writer);
            await loader.LoadAsync(ToAsync(records)).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }

        return utf8NoBom.GetString(stream.ToArray());
    }



    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }



    [ExcludeFromCodeCoverage]
    private sealed class WithField
    {
        public string Tag = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    private record BaseRecord
    {
        public string BaseName { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    private sealed record DerivedRecord : BaseRecord
    {
        public int Id { get; set; }
    }
}
