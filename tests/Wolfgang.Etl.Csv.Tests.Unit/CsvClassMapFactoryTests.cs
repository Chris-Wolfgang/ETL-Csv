using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CsvHelper.Configuration;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins what <see cref="CsvClassMapFactory"/> writes into the CsvHelper member maps, for both the
/// attribute path and the runtime <see cref="CsvColumnMap"/> path.
/// </summary>
/// <remarks>
/// <see cref="CsvClassMapFactory.GetMap{T}()"/> caches per type, so every attributed record type here
/// is used by one test only.
/// </remarks>
public class CsvClassMapFactoryTests
{
    [Fact]
    public void GetMap_when_the_type_has_no_attributes_returns_null()
    {
        Assert.Null(CsvClassMapFactory.GetMap<UnattributedRecord>());
    }



    [Fact]
    public void GetMap_when_the_only_attribute_is_CsvIgnore_ignores_that_property()
    {
        var map = CsvClassMapFactory.GetMap<IgnoreOnlyRecord>();

        Assert.NotNull(map);
        Assert.True(Member(map!, nameof(IgnoreOnlyRecord.Secret)).Data.Ignore);
        Assert.False(Member(map!, nameof(IgnoreOnlyRecord.Visible)).Data.Ignore);
    }



    [Fact]
    public void GetMap_when_a_property_has_CsvIgnore_and_CsvColumn_ignores_it_without_renaming()
    {
        var map = CsvClassMapFactory.GetMap<IgnoreAndColumnRecord>();

        var member = Member(map!, nameof(IgnoreAndColumnRecord.Both));
        Assert.True(member.Data.Ignore);
        Assert.False(member.Data.IsNameSet);
    }



    [Fact]
    public void GetMap_when_the_only_attributes_are_CsvColumn_applies_index_name_and_format()
    {
        var map = CsvClassMapFactory.GetMap<ColumnOnlyRecord>();

        Assert.NotNull(map);

        var first = Member(map!, nameof(ColumnOnlyRecord.First));
        Assert.True(first.Data.IsIndexSet);
        Assert.Equal(0, first.Data.Index);

        var second = Member(map!, nameof(ColumnOnlyRecord.Second));
        Assert.True(second.Data.IsIndexSet);
        Assert.Equal(1, second.Data.Index);
        Assert.Equal(new[] { "yyyy-MM-dd" }, second.Data.TypeConverterOptions.Formats);

        var named = Member(map!, nameof(ColumnOnlyRecord.Named));
        Assert.True(named.Data.IsNameSet);
        Assert.Equal("renamed", named.Data.Names.Single());
    }



    [Fact]
    public void GetMap_when_CsvColumn_has_an_empty_name_and_format_applies_neither()
    {
        var map = CsvClassMapFactory.GetMap<EmptyColumnSettingsRecord>();

        var empty = Member(map!, nameof(EmptyColumnSettingsRecord.EmptySettings));
        Assert.False(empty.Data.IsNameSet);
        Assert.False(empty.Data.IsIndexSet);
        Assert.True(IsEmpty(empty.Data.TypeConverterOptions.Formats));

        var bare = Member(map!, nameof(EmptyColumnSettingsRecord.NoSettings));
        Assert.False(bare.Data.IsNameSet);
        Assert.True(IsEmpty(bare.Data.TypeConverterOptions.Formats));
    }



    [Fact]
    public void BuildFromColumnMaps_applies_index_zero()
    {
        var map = CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>
        (
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)) { Index = 0 },
            }
        );

        var member = Member(map, nameof(ProbeRecord.A));
        Assert.True(member.Data.IsIndexSet);
        Assert.Equal(0, member.Data.Index);
    }



    [Fact]
    public void BuildFromColumnMaps_applies_a_non_empty_format_only()
    {
        var map = CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>
        (
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)) { Index = 0, Format = "yyyy" },
                new CsvColumnMap(nameof(ProbeRecord.B)) { Index = 1, Format = "" },
                new CsvColumnMap(nameof(ProbeRecord.C)) { Index = 2 },
            }
        );

        Assert.Equal(new[] { "yyyy" }, Member(map, nameof(ProbeRecord.A)).Data.TypeConverterOptions.Formats);
        Assert.True(IsEmpty(Member(map, nameof(ProbeRecord.B)).Data.TypeConverterOptions.Formats));
        Assert.True(IsEmpty(Member(map, nameof(ProbeRecord.C)).Data.TypeConverterOptions.Formats));
    }



    [Fact]
    public void BuildFromColumnMaps_without_index_ignores_a_null_or_empty_name()
    {
        var map = CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>
        (
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)),
                new CsvColumnMap(nameof(ProbeRecord.B)) { Name = "" },
            }
        );

        Assert.False(Member(map, nameof(ProbeRecord.A)).Data.IsNameSet);
        Assert.False(Member(map, nameof(ProbeRecord.B)).Data.IsNameSet);
    }



    [Fact]
    public void BuildFromColumnMaps_when_index_zero_is_used_twice_throws()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>
            (
                new[]
                {
                    new CsvColumnMap(nameof(ProbeRecord.A)) { Index = 0 },
                    new CsvColumnMap(nameof(ProbeRecord.B)) { Index = 0 },
                }
            )
        );

        Assert.Equal("columnMaps", ex.ParamName);
        Assert.StartsWith
        (
            "Duplicate column index 0 on property 'B' (already bound by 'A').",
            ex.Message,
            StringComparison.Ordinal
        );
    }



    [Fact]
    public void BuildFromColumnMaps_when_the_list_is_empty_reports_the_parameter_and_message()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>(Array.Empty<CsvColumnMap>())
        );

        Assert.Equal("columnMaps", ex.ParamName);
        Assert.StartsWith("columnMaps must contain at least one entry.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void BuildFromColumnMaps_when_a_property_is_unknown_names_it_and_the_type()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvClassMapFactory.BuildFromColumnMaps<ProbeRecord>
            (
                new[]
                {
                    new CsvColumnMap("Missing") { Index = 0 },
                }
            )
        );

        Assert.StartsWith
        (
            $"Property 'Missing' was not found on type '{typeof(ProbeRecord).FullName}'.",
            ex.Message,
            StringComparison.Ordinal
        );
    }



    private static MemberMap Member(ClassMap map, string propertyName) =>
        map.MemberMaps.Single(m => string.Equals(m.Data.Member!.Name, propertyName, StringComparison.Ordinal));



    private static bool IsEmpty(string[]? formats) => formats is null || formats.Length == 0;



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record UnattributedRecord
    {
        public string Value { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record IgnoreOnlyRecord
    {
        [CsvIgnore]
        public string Secret { get; set; } = string.Empty;



        public string Visible { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record IgnoreAndColumnRecord
    {
        [CsvIgnore]
        [CsvColumn(Name = "renamed")]
        public string Both { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record ColumnOnlyRecord
    {
        [CsvColumn(Index = 0)]
        public string First { get; set; } = string.Empty;



        [CsvColumn(Index = 1, Format = "yyyy-MM-dd")]
        public DateTime Second { get; set; }



        [CsvColumn(Name = "renamed")]
        public string Named { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record EmptyColumnSettingsRecord
    {
        [CsvColumn(Name = "", Format = "")]
        public string EmptySettings { get; set; } = string.Empty;



        [CsvColumn]
        public string NoSettings { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record ProbeRecord
    {
        public string A { get; set; } = string.Empty;



        public string B { get; set; } = string.Empty;



        public string C { get; set; } = string.Empty;
    }
}
