using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvInheritedAttributeTests
{
    public class BaseRecord
    {
        [CsvIgnore]
        public string? Secret { get; set; }

        [CsvColumn(Name = "identifier")]
        public int Id { get; set; }
    }



    public class DerivedRecord : BaseRecord
    {
        public string? Extra { get; set; }
    }



    public abstract class VirtualBase
    {
        [CsvIgnore]
        public abstract string? Secret { get; set; }

        [CsvColumn(Name = "identifier")]
        public abstract int Id { get; set; }
    }



    public class OverridingRecord : VirtualBase
    {
        public override string? Secret { get; set; }

        public override int Id { get; set; }
    }



    [Fact]
    public async Task LoadAsync_when_ignored_property_is_inherited_does_not_write_it()
    {
        var record = new DerivedRecord { Secret = "s", Extra = "e", Id = 7 };
        Assert.Equal("s", record.Secret);

        var text = await WriteAsync(record);

        Assert.Equal("Extra,identifier\r\ne,7\r\n", text);
    }



    [Fact]
    public async Task ExtractAsync_when_ignored_property_is_inherited_does_not_require_its_header()
    {
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("Extra,identifier\ne,7\n")));
        var extractor = new CsvExtractor<DerivedRecord>(reader);

        var items = new List<DerivedRecord>();
        await foreach (var item in extractor.ExtractAsync())
        {
            items.Add(item);
        }

        var single = Assert.Single(items);
        Assert.Equal("e", single.Extra);
        Assert.Equal(7, single.Id);
    }



    [Fact]
    public async Task LoadAsync_when_attributes_are_on_overridden_base_properties_applies_them()
    {
        var record = new OverridingRecord { Secret = "s", Id = 7 };
        Assert.Equal("s", record.Secret);

        var text = await WriteAsync(record);

        Assert.Equal("identifier\r\n7\r\n", text);
    }



    [Fact]
    public void For_when_attributes_are_on_overridden_base_properties_applies_them()
    {
        var schema = CsvSchema.For<OverridingRecord>();

        var column = Assert.Single(schema.Columns);
        Assert.Equal("Id", column.PropertyName);
        Assert.Equal("identifier", column.Name);
    }



    private static async Task<string> WriteAsync<T>(T item)
        where T : notnull
    {
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true);
        var loader = new CsvLoader<T>(writer);

        await loader.LoadAsync(new[] { item }.ToAsyncEnumerable());
        await writer.FlushAsync();

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
