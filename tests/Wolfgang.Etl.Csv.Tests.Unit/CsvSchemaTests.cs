using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvSchemaTests
{
    [Fact]
    public void For_reifies_attributes_honouring_CsvColumn_and_omitting_CsvIgnore()
    {
        var schema = CsvSchema.For<AttributedPersonRecord>();

        Assert.Equal(typeof(AttributedPersonRecord), schema.RecordType);
        Assert.Equal(3, schema.ColumnCount);
        Assert.Equal
        (
            new[] { "FirstName", "LastName", "Age" },
            schema.Columns.Select(c => c.PropertyName).ToArray()
        );
        Assert.Equal
        (
            new[] { "first_name", "last_name", "age" },
            schema.Columns.Select(c => c.Name).ToArray()
        );
        Assert.DoesNotContain(schema.Columns, c => string.Equals(c.PropertyName, nameof(AttributedPersonRecord.ComputedDisplayName), StringComparison.Ordinal));
    }



    [Fact]
    public void For_when_type_has_no_attributes_binds_every_public_property_by_its_own_name()
    {
        var schema = CsvSchema.For<PersonRecord>();

        Assert.Equal(3, schema.ColumnCount);
        Assert.All(schema.Columns, c => Assert.Equal(c.PropertyName, c.Name));
    }



    [Fact]
    public async Task ExtractAsync_with_For_schema_is_equivalent_to_the_attribute_path()
    {
        var csv = "first_name,last_name,age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n";

        var viaAttributes = await ExtractAsync(new CsvExtractor<AttributedPersonRecord>(Reader(csv)));
        var viaSchema = await ExtractAsync
        (
            new CsvExtractor<AttributedPersonRecord>
            (
                Reader(csv),
                new CsvExtractorOptions<AttributedPersonRecord> { Schema = CsvSchema.For<AttributedPersonRecord>() }
            )
        );

        Assert.Equal(viaAttributes, viaSchema);
    }



    [Fact]
    public async Task ExtractAsync_with_For_schema_of_an_index_mapped_record_is_equivalent_to_the_attribute_path()
    {
        var csv = "x,y\r\n1,2\r\n";
        var attributeOptions = new CsvExtractorOptions<CsvAttributeMappingTests.IndexedRecord> { HasHeaderRecord = false };
        var schemaOptions = new CsvExtractorOptions<CsvAttributeMappingTests.IndexedRecord>
        {
            HasHeaderRecord = false,
            Schema = CsvSchema.For<CsvAttributeMappingTests.IndexedRecord>(),
        };

        var viaAttributes = await ExtractAsync(new CsvExtractor<CsvAttributeMappingTests.IndexedRecord>(Reader(csv), attributeOptions));
        var viaSchema = await ExtractAsync(new CsvExtractor<CsvAttributeMappingTests.IndexedRecord>(Reader(csv), schemaOptions));

        Assert.Equal(viaAttributes, viaSchema);
    }



    [Fact]
    public void Constructor_when_schema_was_built_for_a_different_record_type_throws_ArgumentException()
    {
        var schema = CsvSchema.For<PersonRecord>();

        var ex = Assert.Throws<ArgumentException>
        (
            () => new CsvExtractor<AttributedPersonRecord>(Reader(""), new CsvExtractorOptions<AttributedPersonRecord> { Schema = schema })
        );

        Assert.Contains(nameof(PersonRecord), ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void Constructor_when_both_Schema_and_ColumnMaps_are_set_throws_ArgumentException()
    {
        var options = new CsvExtractorOptions<PersonRecord>
        {
            Schema = CsvSchema.For<PersonRecord>(),
            ColumnMaps = new[] { new CsvColumnMap(nameof(PersonRecord.Age)) { Name = "age" } },
        };

        Assert.Throws<ArgumentException>(() => new CsvExtractor<PersonRecord>(Reader(""), options));
    }



    [Fact]
    public void LoaderConstructor_when_schema_was_built_for_a_different_record_type_throws_ArgumentException()
    {
        var options = new CsvLoaderOptions<AttributedPersonRecord> { Schema = CsvSchema.For<PersonRecord>() };

        Assert.Throws<ArgumentException>(() => new CsvLoader<AttributedPersonRecord>(Writer(), options));
    }



    [Fact]
    public void LoaderConstructor_when_both_Schema_and_ColumnMaps_are_set_throws_ArgumentException()
    {
        var options = new CsvLoaderOptions<PersonRecord>
        {
            Schema = CsvSchema.For<PersonRecord>(),
            ColumnMaps = new[] { new CsvColumnMap(nameof(PersonRecord.Age)) { Name = "age" } },
        };

        Assert.Throws<ArgumentException>(() => new CsvLoader<PersonRecord>(Writer(), options));
    }



    [Fact]
    public void Schema_property_exposes_the_schema_supplied_through_options()
    {
        var schema = CsvSchema.For<PersonRecord>();
        var extractor = new CsvExtractor<PersonRecord>(Reader(""), new CsvExtractorOptions<PersonRecord> { Schema = schema });
        var loader = new CsvLoader<PersonRecord>(Writer(), new CsvLoaderOptions<PersonRecord> { Schema = schema });

        Assert.Same(schema, extractor.Schema);
        Assert.Same(schema, loader.Schema);
    }



    [Fact]
    public void Schema_property_is_null_when_no_schema_was_supplied()
    {
        var extractor = new CsvExtractor<PersonRecord>(Reader(""));

        Assert.Null(extractor.Schema);
    }



    private static StreamReader Reader(string csv) =>
        new(new MemoryStream(Encoding.UTF8.GetBytes(csv)), Encoding.UTF8);



    private static StreamWriter Writer() =>
        new(new MemoryStream(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true);



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
}
