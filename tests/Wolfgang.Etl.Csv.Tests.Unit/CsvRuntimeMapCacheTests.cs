using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvRuntimeMapCacheTests
{
    public sealed record Pair
    {
        public string Name { get; set; } = string.Empty;

        public string Other { get; set; } = string.Empty;
    }



    [Fact]
    public async Task ExtractAsync_when_ColumnMaps_list_is_repopulated_uses_the_new_layout()
    {
        var maps = new List<CsvColumnMap>
        {
            new("Name") { Index = 0 },
            new("Other") { Index = 1 },
        };

        var first = await ExtractAsync("a,b\n", maps);

        maps.Clear();
        maps.Add(new CsvColumnMap("Name") { Index = 1 });
        maps.Add(new CsvColumnMap("Other") { Index = 0 });

        var second = await ExtractAsync("a,b\n", maps);

        Assert.Equal("a", first.Name);
        Assert.Equal("b", first.Other);
        Assert.Equal("b", second.Name);
        Assert.Equal("a", second.Other);
    }



    [Fact]
    public void BuildFromColumnMaps_when_list_is_repopulated_returns_a_new_map()
    {
        var maps = new List<CsvColumnMap> { new("Name") { Index = 0 } };

        var first = CsvClassMapFactory.BuildFromColumnMaps<Pair>(maps);
        maps[0] = new CsvColumnMap("Name") { Index = 1 };
        var second = CsvClassMapFactory.BuildFromColumnMaps<Pair>(maps);

        Assert.NotSame(first, second);
    }



    [Fact]
    public void BuildFromColumnMaps_when_list_is_unreachable_the_cache_does_not_keep_it_alive()
    {
        var weak = BuildAndForget();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(weak.TryGetTarget(out _));
    }



    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static WeakReference<List<CsvColumnMap>> BuildAndForget()
    {
        var maps = new List<CsvColumnMap> { new("Name") { Index = 0 } };
        CsvClassMapFactory.BuildFromColumnMaps<Pair>(maps);
        return new WeakReference<List<CsvColumnMap>>(maps);
    }



    private static async Task<Pair> ExtractAsync(string csv, IReadOnlyList<CsvColumnMap> maps)
    {
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv)));
        var extractor = new CsvExtractor<Pair>
        (
            reader,
            new CsvExtractorOptions<Pair> { HasHeaderRecord = false, ColumnMaps = maps }
        );

        var items = new List<Pair>();
        await foreach (var item in extractor.ExtractAsync())
        {
            items.Add(item);
        }

        return Assert.Single(items);
    }
}
