using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvExtractorDiscriminatorTests
{
    private static CsvExtractor<T> CreateExtractor<T>(string csv, CsvDiscriminator<T> discriminator, bool hasHeader)
        where T : notnull
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        return new CsvExtractor<T>(new StreamReader(stream, Encoding.UTF8))
        {
            HasHeaderRecord = hasHeader,
            Discriminator = discriminator,
        };
    }



    private static async Task<List<T>> ReadAllAsync<T>(CsvExtractor<T> sut)
        where T : notnull
    {
        var list = new List<T>();
        await foreach (var item in sut.ExtractAsync())
        {
            list.Add(item);
        }

        return list;
    }



    private static CsvDiscriminator<LedgerRow> BuildByIndex
    (
        CsvDiscriminatorAction onUnknown = CsvDiscriminatorAction.Throw,
        StringComparer? comparer = null
    )
    {
        var builder = new CsvDiscriminatorBuilder<LedgerRow>(0)
            .Map<HeaderRow>
            (
                "HDR",
                new[]
                {
                    new CsvColumnMap(nameof(HeaderRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(HeaderRow.BatchId)) { Index = 1 },
                    new CsvColumnMap(nameof(HeaderRow.BatchDate)) { Index = 2 },
                }
            )
            .Map<PaymentRow>
            (
                "PMT",
                new[]
                {
                    new CsvColumnMap(nameof(PaymentRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(PaymentRow.Account)) { Index = 1 },
                    new CsvColumnMap(nameof(PaymentRow.Amount)) { Index = 2 },
                }
            )
            .Map<TrailerRow>
            (
                "TRL",
                new[]
                {
                    new CsvColumnMap(nameof(TrailerRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(TrailerRow.Count)) { Index = 1 },
                }
            )
            .OnUnknown(onUnknown);

        if (comparer is not null)
        {
            builder = builder.WithComparer(comparer);
        }

        return builder.Build();
    }



    private const string ByIndexCsv =
        "HDR,B001,2026-01-01\n" +
        "PMT,ACC1,100\n" +
        "PMT,ACC2,25\n" +
        "TRL,3\n";



    [Fact]
    public async Task ExtractAsync_when_Discriminator_by_index_binds_each_row_to_its_concrete_type()
    {
        var sut = CreateExtractor(ByIndexCsv, BuildByIndex(), hasHeader: false);

        var rows = await ReadAllAsync(sut);

        Assert.Collection
        (
            rows,
            r => Assert.IsType<HeaderRow>(r),
            r => Assert.IsType<PaymentRow>(r),
            r => Assert.IsType<PaymentRow>(r),
            r => Assert.IsType<TrailerRow>(r)
        );
    }



    [Fact]
    public async Task ExtractAsync_when_Discriminator_by_index_maps_field_values()
    {
        var sut = CreateExtractor(ByIndexCsv, BuildByIndex(), hasHeader: false);

        var rows = await ReadAllAsync(sut);

        var header = Assert.IsType<HeaderRow>(rows[0]);
        Assert.Equal("B001", header.BatchId);

        var payment = Assert.IsType<PaymentRow>(rows[1]);
        Assert.Equal("ACC1", payment.Account);
        Assert.Equal(100m, payment.Amount);

        var trailer = Assert.IsType<TrailerRow>(rows[3]);
        Assert.Equal(3, trailer.Count);
    }



    [Fact]
    public async Task ExtractAsync_when_Discriminator_by_name_binds_using_the_header_column()
    {
        const string csv =
            "Kind,Account,Amount,Count\n" +
            "PMT,ACC1,100,\n" +
            "TRL,,,7\n";

        var discriminator = new CsvDiscriminatorBuilder<NamedRow>("Kind")
            .Map<NamedPayment>("PMT")
            .Map<NamedTrailer>("TRL")
            .Build();

        var rows = await ReadAllAsync(CreateExtractor(csv, discriminator, hasHeader: true));

        var payment = Assert.IsType<NamedPayment>(rows[0]);
        Assert.Equal("ACC1", payment.Account);
        Assert.Equal(100m, payment.Amount);

        var trailer = Assert.IsType<NamedTrailer>(rows[1]);
        Assert.Equal(7, trailer.Count);
    }



    [Fact]
    public async Task ExtractAsync_when_unknown_discriminator_and_action_is_Throw_raises()
    {
        const string csv = ByIndexCsv + "XXX,foo,bar\n";
        var sut = CreateExtractor(csv, BuildByIndex(CsvDiscriminatorAction.Throw), hasHeader: false);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await ReadAllAsync(sut));
    }



    [Fact]
    public async Task ExtractAsync_when_unknown_discriminator_and_action_is_Skip_skips_the_row()
    {
        const string csv = ByIndexCsv + "XXX,foo,bar\n";
        var sut = CreateExtractor(csv, BuildByIndex(CsvDiscriminatorAction.Skip), hasHeader: false);

        var rows = await ReadAllAsync(sut);

        Assert.Equal(4, rows.Count);
        Assert.Equal(1, sut.CurrentSkippedItemCount);
    }



    [Fact]
    public async Task ExtractAsync_when_unknown_discriminator_and_action_is_YieldAsBase_yields_base_type()
    {
        const string csv = ByIndexCsv + "XXX,foo,bar\n";
        var sut = CreateExtractor(csv, BuildByIndex(CsvDiscriminatorAction.YieldAsBase), hasHeader: false);

        var rows = await ReadAllAsync(sut);

        Assert.Equal(5, rows.Count);
        var fallback = rows[4];
        Assert.Equal(typeof(LedgerRow), fallback.GetType());
        Assert.Equal("XXX", fallback.RecordType);
    }



    [Fact]
    public async Task ExtractAsync_when_YieldAsBase_binds_the_base_row_through_the_extractor_ColumnMaps()
    {
        // The base LedgerRow's RecordType lives at index 1 here; without the base map being registered
        // the fallback would auto-map it to index 0. This locks in that YieldAsBase honors ColumnMaps.
        var sut = CreateExtractor("foo,XXX,bar\n", BuildByIndex(CsvDiscriminatorAction.YieldAsBase), hasHeader: false);
        sut.ColumnMaps = new[]
        {
            new CsvColumnMap(nameof(LedgerRow.RecordType)) { Index = 1 },
        };

        var rows = await ReadAllAsync(sut);

        var fallback = Assert.Single(rows);
        Assert.Equal(typeof(LedgerRow), fallback.GetType());
        Assert.Equal("XXX", fallback.RecordType);
    }



    [Fact]
    public async Task ExtractAsync_when_custom_comparer_is_case_sensitive_a_mismatched_case_is_unknown()
    {
        const string csv =
            "PMT,ACC1,100\n" +
            "pmt,ACC2,50\n";

        var sut = CreateExtractor
        (
            csv,
            BuildByIndex(CsvDiscriminatorAction.Skip, StringComparer.Ordinal),
            hasHeader: false
        );

        var rows = await ReadAllAsync(sut);

        var payment = Assert.IsType<PaymentRow>(Assert.Single(rows));
        Assert.Equal("ACC1", payment.Account);
        Assert.Equal(1, sut.CurrentSkippedItemCount);
    }



    [Fact]
    public async Task ExtractAsync_when_Discriminator_is_direct_init_with_per_type_column_maps_reflects_and_binds()
    {
        // Exercises the reflective RegisterClassMaps fallback (no PrebuiltClassMaps) and the
        // non-generic CsvClassMapFactory.BuildFromColumnMaps(Type, ...) bridge.
        var discriminator = new CsvDiscriminator<LedgerRow>
        {
            ColumnIndex = 0,
            Mapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["PMT"] = typeof(PaymentRow),
                ["TRL"] = typeof(TrailerRow),
            },
            PerTypeColumnMaps = new Dictionary<Type, IReadOnlyList<CsvColumnMap>>
            {
                [typeof(PaymentRow)] = new[]
                {
                    new CsvColumnMap(nameof(PaymentRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(PaymentRow.Account)) { Index = 1 },
                    new CsvColumnMap(nameof(PaymentRow.Amount)) { Index = 2 },
                },
                [typeof(TrailerRow)] = new[]
                {
                    new CsvColumnMap(nameof(TrailerRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(TrailerRow.Count)) { Index = 1 },
                },
            },
        };

        var rows = await ReadAllAsync(CreateExtractor("PMT,ACC9,42\nTRL,2\n", discriminator, hasHeader: false));

        var payment = Assert.IsType<PaymentRow>(rows[0]);
        Assert.Equal("ACC9", payment.Account);
        Assert.Equal(42m, payment.Amount);
        Assert.Equal(2, Assert.IsType<TrailerRow>(rows[1]).Count);
    }



    [Fact]
    public async Task ExtractAsync_when_Discriminator_is_direct_init_without_column_maps_uses_attribute_mapping()
    {
        // Exercises the reflective GetMap(Type) bridge (attribute-based mapping, no column maps).
        var discriminator = new CsvDiscriminator<NamedRow>
        {
            ColumnName = "Kind",
            Mapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["PMT"] = typeof(NamedPayment),
                ["TRL"] = typeof(NamedTrailer),
            },
        };

        const string csv =
            "Kind,Account,Amount,Count\n" +
            "PMT,ACC1,100,\n" +
            "TRL,,,7\n";

        var rows = await ReadAllAsync(CreateExtractor(csv, discriminator, hasHeader: true));

        Assert.Equal("ACC1", Assert.IsType<NamedPayment>(rows[0]).Account);
        Assert.Equal(7, Assert.IsType<NamedTrailer>(rows[1]).Count);
    }



    [Fact]
    public void TryResolveValue_returns_the_mapped_value_for_a_known_type()
    {
        var discriminator = BuildByIndex();

        var found = discriminator.TryResolveValue(typeof(PaymentRow), out var value);

        Assert.True(found);
        Assert.Equal("PMT", value);
    }



    [Fact]
    public void TryResolveValue_returns_false_for_an_unmapped_type()
    {
        var discriminator = BuildByIndex();

        Assert.False(discriminator.TryResolveValue(typeof(string), out _));
    }



    [Fact]
    public void CsvDiscriminatorBuilder_ctor_rejects_a_negative_column_index()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CsvDiscriminatorBuilder<LedgerRow>(-1));
    }



    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CsvDiscriminatorBuilder_ctor_rejects_a_blank_column_name(string columnName)
    {
        Assert.Throws<ArgumentException>(() => new CsvDiscriminatorBuilder<LedgerRow>(columnName));
    }



    [Fact]
    public void CsvDiscriminatorBuilder_Map_rejects_a_null_discriminator_value()
    {
        var builder = new CsvDiscriminatorBuilder<LedgerRow>(0);

        Assert.Throws<ArgumentNullException>(() => builder.Map<PaymentRow>(null!));
    }



    [Fact]
    public void CsvDiscriminatorBuilder_WithComparer_rejects_null()
    {
        var builder = new CsvDiscriminatorBuilder<LedgerRow>(0);

        Assert.Throws<ArgumentNullException>(() => builder.WithComparer(null!));
    }



    [Fact]
    public void CsvDiscriminatorBuilder_Build_rejects_a_duplicate_discriminator_value()
    {
        var builder = new CsvDiscriminatorBuilder<LedgerRow>(0)
            .Map<PaymentRow>("X")
            .Map<TrailerRow>("X");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }



    [Fact]
    public void CsvDiscriminatorBuilder_Build_rejects_the_same_type_mapped_twice()
    {
        var builder = new CsvDiscriminatorBuilder<LedgerRow>(0)
            .Map<PaymentRow>("A")
            .Map<PaymentRow>("B");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }



    [Fact]
    public async Task ExtractAsync_when_direct_init_maps_a_non_base_type_throws_a_clear_error()
    {
        var discriminator = new CsvDiscriminator<LedgerRow>
        {
            ColumnIndex = 0,
            Mapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["X"] = typeof(string),   // not assignable to LedgerRow
            },
        };

        var sut = CreateExtractor("X,foo\n", discriminator, hasHeader: false);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await ReadAllAsync(sut));
    }



    [ExcludeFromCodeCoverage]
    public record LedgerRow
    {
        public string RecordType { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    public record HeaderRow : LedgerRow
    {
        public string BatchId { get; set; } = string.Empty;



        public string BatchDate { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    public record PaymentRow : LedgerRow
    {
        public string Account { get; set; } = string.Empty;



        public decimal Amount { get; set; }
    }



    [ExcludeFromCodeCoverage]
    public record TrailerRow : LedgerRow
    {
        public int Count { get; set; }
    }



    [ExcludeFromCodeCoverage]
    public record NamedRow
    {
    }



    [ExcludeFromCodeCoverage]
    public record NamedPayment : NamedRow
    {
        [CsvColumn(Name = "Account")]
        public string Account { get; set; } = string.Empty;



        [CsvColumn(Name = "Amount")]
        public decimal Amount { get; set; }
    }



    [ExcludeFromCodeCoverage]
    public record NamedTrailer : NamedRow
    {
        [CsvColumn(Name = "Count")]
        public int Count { get; set; }
    }
}
