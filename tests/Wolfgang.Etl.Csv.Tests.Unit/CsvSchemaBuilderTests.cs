using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvSchemaBuilderTests
{
    [Fact]
    public void Column_builds_a_map_with_every_field_populated()
    {
        var maps = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Column(p => p.Age, index: 2, format: "0", optional: true, @default: "0")
            .Build();

        Assert.Equal(2, maps.Count);

        Assert.Equal
        (
            new CsvColumnMap("FirstName") { Name = "first_name" },
            maps[0]
        );

        Assert.Equal
        (
            new CsvColumnMap("Age") { Index = 2, Format = "0", Optional = true, Default = "0" },
            maps[1]
        );
    }


    [Fact]
    public void Column_resolves_a_value_type_property_selected_through_boxing()
    {
        // p => p.Age is Func<PersonRecord, int>; selecting it here exercises the
        // Convert-unwrapping path (a value type boxed to the selector's result).
        var maps = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.Age, name: "age")
            .Build();

        Assert.Equal("Age", maps[0].PropertyName);
    }


    [Fact]
    public void Column_when_selector_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column<string>(null!)
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
    public async Task Code_built_schema_extracts_equivalently_to_the_attributed_record()
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
        var viaSchema = await ExtractAsync(new CsvExtractor<PersonRecord>(Reader(csv)) { ColumnMaps = schema });

        Assert.Equal(viaAttributes.Count, viaSchema.Count);
        for (var i = 0; i < viaAttributes.Count; i++)
        {
            Assert.Equal(viaAttributes[i].FirstName, viaSchema[i].FirstName);
            Assert.Equal(viaAttributes[i].LastName, viaSchema[i].LastName);
            Assert.Equal(viaAttributes[i].Age, viaSchema[i].Age);
        }
    }


    [Fact]
    public async Task Code_built_schema_loads_equivalently_to_the_attributed_record()
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

        var viaAttributes = await LoadToStringAsync(w => new CsvLoader<AttributedPersonRecord>(w) { LeaveOpen = true }, attributed);

        var schema = new CsvSchemaBuilder<PersonRecord>()
            .Column(p => p.FirstName, name: "first_name")
            .Column(p => p.LastName, name: "last_name")
            .Column(p => p.Age, name: "age")
            .Build();
        var viaSchema = await LoadToStringAsync(w => new CsvLoader<PersonRecord>(w) { LeaveOpen = true, ColumnMaps = schema }, plain);

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
}
