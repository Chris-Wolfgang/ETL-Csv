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

/// <summary>
/// Closes test gaps that Stryker reported as surviving mutants (#404): exact guard messages
/// (string mutations), the boundary of the Schema-vs-ColumnMaps rule, the runtime-map cache
/// returning the cached instance, and <see cref="CsvColumnAttribute"/> inherited from an
/// overridden base property.
/// </summary>
public sealed class CsvMutationGapTests
{
    [Fact]
    public void Column_when_index_is_below_negative_one_reports_the_documented_message()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName, index: -2)
        );

        Assert.StartsWith
        (
            "Index must be -1 (bind by name) or a non-negative column index.",
            ex.Message,
            StringComparison.Ordinal
        );
    }



    [Fact]
    public void Column_when_name_is_blank_reports_the_documented_message()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName, name: " ")
        );

        Assert.StartsWith
        (
            "Name must be null (bind by the property's own name) or a non-blank column name.",
            ex.Message,
            StringComparison.Ordinal
        );
    }



    [Fact]
    public void Column_when_name_and_index_are_both_supplied_reports_the_documented_message()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvSchemaBuilder<PersonRecord>().Column(p => p.FirstName, name: "first_name", index: 0)
        );

        Assert.StartsWith
        (
            "A column binds by name or by index, not both; pass either name or a non-negative index.",
            ex.Message,
            StringComparison.Ordinal
        );
    }



    [Fact]
    public void Build_when_no_column_was_added_reports_the_documented_message()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new CsvSchemaBuilder<PersonRecord>().Build());

        Assert.Equal("Add at least one column before calling Build().", ex.Message);
    }



    [Fact]
    public void ValidateOptions_when_Schema_and_non_empty_ColumnMaps_are_set_reports_the_documented_message()
    {
        var schema = CsvSchema.For<PersonRecord>();

        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvSchema.ValidateOptions
            (
                schema,
                new[] { new CsvColumnMap(nameof(PersonRecord.Age)) },
                typeof(PersonRecord),
                "options"
            )
        );

        Assert.StartsWith("Set either Schema or ColumnMaps, not both.", ex.Message, StringComparison.Ordinal);
        Assert.Equal("options", ex.ParamName);
    }



    [Fact]
    public void ValidateOptions_when_Schema_is_set_and_ColumnMaps_is_empty_accepts_it()
    {
        var schema = CsvSchema.For<PersonRecord>();

        // An empty ColumnMaps list configures nothing, so it does not conflict with a schema.
        CsvSchema.ValidateOptions(schema, Array.Empty<CsvColumnMap>(), typeof(PersonRecord), "options");

        Assert.Equal(typeof(PersonRecord), schema.RecordType);
    }



    [Fact]
    public void BuildFromColumnMaps_when_called_again_with_the_same_list_returns_the_cached_map()
    {
        var maps = new[] { new CsvColumnMap(nameof(PersonRecord.FirstName)) { Index = 0 } };

        var first = CsvClassMapFactory.BuildFromColumnMaps<PersonRecord>(maps);
        var second = CsvClassMapFactory.BuildFromColumnMaps<PersonRecord>(maps);

        Assert.Same(first, second);
    }



    [Fact]
    public void For_when_an_override_inherits_CsvColumn_from_the_base_property_uses_the_inherited_name()
    {
        var schema = CsvSchema.For<DerivedNamedRecord>();

        var column = Assert.Single(schema.Columns, c => string.Equals(c.PropertyName, nameof(DerivedNamedRecord.Name), StringComparison.Ordinal));
        Assert.Equal("full_name", column.Name);
    }



    [Fact]
    public async Task ExtractAsync_when_an_override_inherits_CsvColumn_from_the_base_property_binds_the_inherited_name()
    {
        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("id,full_name\r\n7,Ann\r\n")), Encoding.UTF8);
        var extractor = new CsvExtractor<DerivedNamedRecord>(reader);

        var records = await extractor
            .ExtractAsync()
            .ToListAsync();

        var record = Assert.Single(records);
        Assert.Equal(7, record.Id);
        Assert.Equal("Ann", record.Name);
    }



    [Fact]
    public void For_when_every_property_is_ignored_reports_the_documented_message()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => CsvSchema.For<AllIgnoredRecord>());

        Assert.Equal($"{nameof(AllIgnoredRecord)} has no mappable public properties.", ex.Message);
    }



    [Fact]
    public void Constructor_when_columns_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new CsvSchema(typeof(PersonRecord), null!));

        Assert.Equal("columns", ex.ParamName);
    }



    [Fact]
    public void Constructor_when_columns_is_empty_reports_the_documented_message()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CsvSchema(typeof(PersonRecord), Array.Empty<CsvColumnMap>()));

        Assert.StartsWith("A schema must map at least one column.", ex.Message, StringComparison.Ordinal);
        Assert.Equal("columns", ex.ParamName);
    }



    [ExcludeFromCodeCoverage]
    public sealed class AllIgnoredRecord
    {
        [CsvIgnore]
        public string Hidden { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    public class BaseNamedRecord
    {
        [CsvColumn(Name = "full_name")]
        public virtual string Name { get; set; } = string.Empty;
    }



    // Name is overridden without an attribute, so its [CsvColumn] is reachable only through the
    // inherit: true lookup. Id carries its own attribute so the type takes the attribute path.
    [ExcludeFromCodeCoverage]
    public sealed class DerivedNamedRecord : BaseNamedRecord
    {
        [CsvColumn(Name = "id")]
        public int Id { get; set; }

        public override string Name { get; set; } = string.Empty;
    }
}
