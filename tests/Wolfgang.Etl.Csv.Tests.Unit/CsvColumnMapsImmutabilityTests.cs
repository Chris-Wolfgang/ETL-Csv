using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvColumnMapsImmutabilityTests
{
    private static List<CsvColumnMap> MapsWithName(string name) =>
        new()
        {
            new CsvColumnMap(nameof(PersonRecord.FirstName)) { Name = name },
            new CsvColumnMap(nameof(PersonRecord.LastName)) { Name = "last" },
            new CsvColumnMap(nameof(PersonRecord.Age)) { Name = "age" },
        };



    [Fact]
    public async Task ExtractAsync_when_the_callers_list_is_edited_after_construction_keeps_the_original_mapping()
    {
        var maps = MapsWithName("first");
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("first,last,age\nAnn,Lee,30\n")));
        var sut = new CsvExtractor<PersonRecord>(reader, new CsvExtractorOptions<PersonRecord> { ColumnMaps = maps });

        maps[0] = new CsvColumnMap(nameof(PersonRecord.FirstName)) { Name = "given" };

        var items = new List<PersonRecord>();
        await foreach (var item in sut.ExtractAsync())
        {
            items.Add(item);
        }

        Assert.Equal("Ann", Assert.Single(items).FirstName);
    }



    [Fact]
    public async Task LoadAsync_when_the_callers_list_is_edited_after_construction_keeps_the_original_mapping()
    {
        var maps = MapsWithName("first");
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen: true);
        var sut = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord> { ColumnMaps = maps, LeaveOpen = true });

        maps[0] = new CsvColumnMap(nameof(PersonRecord.FirstName)) { Name = "given" };

        await sut.LoadAsync(new[] { new PersonRecord { FirstName = "Ann", LastName = "Lee", Age = 30 } }.ToAsyncEnumerable());
        await writer.FlushAsync();

        Assert.StartsWith("first,last,age", Encoding.UTF8.GetString(stream.ToArray()));
    }



    [Fact]
    public void ColumnMaps_when_read_back_is_not_a_mutable_collection()
    {
        var options = new CsvExtractorOptions<PersonRecord> { ColumnMaps = MapsWithName("first") };

        Assert.IsNotAssignableFrom<System.Collections.IList>(options.ColumnMaps);
        Assert.Equal(3, options.ColumnMaps.Count);
        Assert.Equal("first", options.ColumnMaps[0].Name);
    }



    [Fact]
    public void ColumnMaps_when_the_same_frozen_list_is_assigned_again_is_not_copied_again()
    {
        var first = new CsvLoaderOptions<PersonRecord> { ColumnMaps = MapsWithName("first") };
        var second = new CsvLoaderOptions<PersonRecord> { ColumnMaps = first.ColumnMaps };

        Assert.Same(first.ColumnMaps, second.ColumnMaps);
    }



    [Fact]
    public void ColumnMaps_when_null_stays_null()
    {
        Assert.Null(new CsvExtractorOptions<PersonRecord> { ColumnMaps = null }.ColumnMaps);
    }



    [Fact]
    public void PerTypeColumnMaps_when_the_callers_list_is_edited_after_assignment_keeps_the_original()
    {
        var list = MapsWithName("first");
        var dictionary = new Dictionary<Type, IReadOnlyList<CsvColumnMap>> { [typeof(PersonRecord)] = list };
        var discriminator = new CsvDiscriminator<object> { ColumnName = "kind", PerTypeColumnMaps = dictionary };

        list[0] = new CsvColumnMap(nameof(PersonRecord.FirstName)) { Name = "given" };
        dictionary.Clear();

        Assert.Equal("first", discriminator.PerTypeColumnMaps[typeof(PersonRecord)][0].Name);
        Assert.Null(new CsvDiscriminator<object> { ColumnName = "kind", PerTypeColumnMaps = null }.PerTypeColumnMaps);
    }



    [Fact]
    public void Enumerating_the_frozen_list_yields_every_map_in_order()
    {
        var options = new CsvExtractorOptions<PersonRecord> { ColumnMaps = MapsWithName("first") };

        Assert.Equal
        (
            new[] { "FirstName", "LastName", "Age" },
            options.ColumnMaps.Select(m => m.PropertyName).ToArray()
        );
        System.Collections.IEnumerable nonGeneric = options.ColumnMaps;
        var enumerator = nonGeneric.GetEnumerator();
        var count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }

        Assert.Equal(3, count);
    }
}
