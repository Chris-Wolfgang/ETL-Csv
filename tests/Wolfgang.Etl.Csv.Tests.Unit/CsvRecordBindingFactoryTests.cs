using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the column layout <see cref="CsvRecordBindingFactory"/> builds, for both the attribute path
/// and the runtime <see cref="CsvColumnMap"/> path.
/// </summary>
public class CsvRecordBindingFactoryTests
{
    [Fact]
    public void GetBinding_when_the_type_has_no_attributes_maps_every_property_by_name()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(UnattributedRecord));

        var member = Assert.Single(binding.Members);
        Assert.Equal(nameof(UnattributedRecord.Value), member.Name);
        Assert.Equal(-1, member.Index);
    }



    [Fact]
    public void GetBinding_returns_the_cached_binding_for_the_same_type()
    {
        Assert.Same
        (
            CsvRecordBindingFactory.GetBinding(typeof(UnattributedRecord)),
            CsvRecordBindingFactory.GetBinding(typeof(UnattributedRecord))
        );
    }



    [Fact]
    public void GetBinding_when_the_only_attribute_is_CsvIgnore_leaves_that_property_out()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(IgnoreOnlyRecord));

        Assert.Equal(new[] { nameof(IgnoreOnlyRecord.Visible) }, Names(binding));
    }



    [Fact]
    public void GetBinding_when_a_property_has_CsvIgnore_and_CsvColumn_ignores_it()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(IgnoreAndColumnRecord));

        Assert.Empty(binding.Members);
    }



    [Fact]
    public void GetBinding_when_the_only_attributes_are_CsvColumn_applies_index_name_and_format()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(ColumnOnlyRecord));

        var first = Member(binding, nameof(ColumnOnlyRecord.First));
        Assert.Equal(0, first.Index);

        var second = Member(binding, nameof(ColumnOnlyRecord.Second));
        Assert.Equal(1, second.Index);
        Assert.Equal("yyyy-MM-dd", second.Format);

        var named = Member(binding, nameof(ColumnOnlyRecord.Named));
        Assert.Equal(-1, named.Index);
        Assert.Equal("renamed", named.Name);
    }



    [Fact]
    public void GetBinding_when_CsvColumn_has_an_empty_name_and_format_applies_neither()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(EmptyColumnSettingsRecord));

        var empty = Member(binding, nameof(EmptyColumnSettingsRecord.EmptySettings));
        Assert.Equal(nameof(EmptyColumnSettingsRecord.EmptySettings), empty.Name);
        Assert.Null(empty.Format);

        var bare = Member(binding, nameof(EmptyColumnSettingsRecord.NoSettings));
        Assert.Equal(nameof(EmptyColumnSettingsRecord.NoSettings), bare.Name);
        Assert.Null(bare.Format);
    }



    [Fact]
    public void GetBinding_puts_the_derived_type_properties_first()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(DerivedRecord));

        Assert.Equal(new[] { nameof(DerivedRecord.Extra), nameof(BaseRecord.Kind) }, Names(binding));
    }



    [Fact]
    public void GetBinding_binds_a_positional_record_through_its_constructor()
    {
        var binding = CsvRecordBindingFactory.GetBinding(typeof(PositionalRecord));

        Assert.Equal(2, binding.ConstructorParameters.Count);
        Assert.All(binding.ConstructorMembers, Assert.NotNull);
        Assert.Empty(binding.ReadMembers);
        Assert.Equal(2, binding.WriteMembers.Count);
    }



    [Fact]
    public void FromColumnMaps_applies_index_zero()
    {
        var binding = CsvRecordBindingFactory.FromColumnMaps
        (
            typeof(ProbeRecord),
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)) { Index = 0 },
            }
        );

        Assert.Equal(0, Member(binding, nameof(ProbeRecord.A)).Index);
    }



    [Fact]
    public void FromColumnMaps_applies_a_non_empty_format_only()
    {
        var binding = CsvRecordBindingFactory.FromColumnMaps
        (
            typeof(ProbeRecord),
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)) { Index = 0, Format = "yyyy" },
                new CsvColumnMap(nameof(ProbeRecord.B)) { Index = 1, Format = "" },
                new CsvColumnMap(nameof(ProbeRecord.C)) { Index = 2 },
            }
        );

        Assert.Equal("yyyy", Member(binding, nameof(ProbeRecord.A)).Format);
        Assert.Null(Member(binding, nameof(ProbeRecord.B)).Format);
        Assert.Null(Member(binding, nameof(ProbeRecord.C)).Format);
    }



    [Fact]
    public void FromColumnMaps_without_index_uses_the_property_name_for_a_null_or_empty_name()
    {
        var binding = CsvRecordBindingFactory.FromColumnMaps
        (
            typeof(ProbeRecord),
            new[]
            {
                new CsvColumnMap(nameof(ProbeRecord.A)),
                new CsvColumnMap(nameof(ProbeRecord.B)) { Name = "" },
            }
        );

        Assert.Equal(new[] { nameof(ProbeRecord.A), nameof(ProbeRecord.B) }, Names(binding));
    }



    [Fact]
    public void FromColumnMaps_when_the_same_list_is_reused_unchanged_returns_the_cached_binding()
    {
        var maps = new[] { new CsvColumnMap(nameof(ProbeRecord.A)) };

        Assert.Same
        (
            CsvRecordBindingFactory.FromColumnMaps(typeof(ProbeRecord), maps),
            CsvRecordBindingFactory.FromColumnMaps(typeof(ProbeRecord), maps)
        );
    }



    [Fact]
    public void FromColumnMaps_when_the_list_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => CsvRecordBindingFactory.FromColumnMaps(typeof(ProbeRecord), null!)
        );

        Assert.Equal("columnMaps", ex.ParamName);
    }



    [Fact]
    public void FromColumnMaps_when_a_property_is_mapped_twice_throws()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvRecordBindingFactory.FromColumnMaps
            (
                typeof(ProbeRecord),
                new[]
                {
                    new CsvColumnMap(nameof(ProbeRecord.A)),
                    new CsvColumnMap(nameof(ProbeRecord.A)),
                }
            )
        );

        Assert.StartsWith("Duplicate column map for property 'A'.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void FromColumnMaps_when_index_zero_is_used_twice_throws()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvRecordBindingFactory.FromColumnMaps
            (
                typeof(ProbeRecord),
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
    public void FromColumnMaps_when_the_list_is_empty_reports_the_parameter_and_message()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvRecordBindingFactory.FromColumnMaps(typeof(ProbeRecord), Array.Empty<CsvColumnMap>())
        );

        Assert.Equal("columnMaps", ex.ParamName);
        Assert.StartsWith("columnMaps must contain at least one entry.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void FromColumnMaps_when_a_property_is_unknown_names_it_and_the_type()
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => CsvRecordBindingFactory.FromColumnMaps
            (
                typeof(ProbeRecord),
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



    private static CsvMemberBinding Member(CsvRecordBinding binding, string propertyName) =>
        binding.Members.Single(m => string.Equals(m.Property.Name, propertyName, StringComparison.Ordinal));



    private static string[] Names(CsvRecordBinding binding) =>
        binding.Members.Select(m => m.Property.Name).ToArray();



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
    public record BaseRecord
    {
        public string Kind { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record DerivedRecord : BaseRecord
    {
        public string Extra { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record PositionalRecord(string Name, int Count);



    [ExcludeFromCodeCoverage] // test POCO: only its metadata is read, never an instance
    public sealed record ProbeRecord
    {
        public string A { get; set; } = string.Empty;



        public string B { get; set; } = string.Empty;



        public string C { get; set; } = string.Empty;
    }
}
