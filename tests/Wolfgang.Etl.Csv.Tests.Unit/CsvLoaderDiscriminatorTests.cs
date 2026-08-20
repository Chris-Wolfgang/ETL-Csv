using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvLoaderDiscriminatorTests
{
    private static (CsvLoader<T> sut, MemoryStream stream) CreateLoader<T>(CsvDiscriminator<T> discriminator)
        where T : notnull
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen: true);
        var sut = new CsvLoader<T>(writer)
        {
            LeaveOpen = true,
            Discriminator = discriminator,
        };

        return (sut, stream);
    }



    private static async Task<string> LoadAndReadAsync<T>(CsvLoader<T> sut, MemoryStream stream, IEnumerable<T> items)
        where T : notnull
    {
        await sut.LoadAsync(items.ToAsyncEnumerable());

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }



    private static CsvDiscriminator<LedgerRow> BuildByIndex
    (
        CsvDiscriminatorAction onUnknown = CsvDiscriminatorAction.Throw
    ) =>
        new CsvDiscriminatorBuilder<LedgerRow>(0)
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
            .OnUnknown(onUnknown)
            .Build();



    private static readonly LedgerRow[] MixedRows =
    {
        new HeaderRow { RecordType = "HDR", BatchId = "B001", BatchDate = "2026-01-01" },
        new PaymentRow { RecordType = "PMT", Account = "ACC1", Amount = 100 },
        new TrailerRow { RecordType = "TRL", Count = 3 },
    };



    private const string MixedCsv = "HDR,B001,2026-01-01\r\nPMT,ACC1,100\r\nTRL,3\r\n";



    [Fact]
    public async Task LoadAsync_when_Discriminator_writes_each_record_by_its_runtime_type()
    {
        var (sut, stream) = CreateLoader(BuildByIndex());

        var text = await LoadAndReadAsync(sut, stream, MixedRows);

        Assert.Equal(MixedCsv, text);
    }



    [Fact]
    public async Task LoadAsync_when_Discriminator_writes_no_header_even_when_HasHeaderRecord_is_true()
    {
        var (sut, stream) = CreateLoader(BuildByIndex());
        sut.HasHeaderRecord = true;

        var text = await LoadAndReadAsync(sut, stream, MixedRows);

        Assert.StartsWith("HDR,", text);
        Assert.DoesNotContain("RecordType", text);
    }



    [Fact]
    public async Task LoadAsync_round_trips_a_polymorphic_file_through_the_extractor()
    {
        // Read a mixed file, then write it back — the bytes should match.
        var extractStream = new MemoryStream(Encoding.UTF8.GetBytes(MixedCsv));
        var extractor = new CsvExtractor<LedgerRow>(new StreamReader(extractStream, Encoding.UTF8))
        {
            HasHeaderRecord = false,
            Discriminator = BuildByIndex(),
        };

        var read = new List<LedgerRow>();
        await foreach (var row in extractor.ExtractAsync())
        {
            read.Add(row);
        }

        var (sut, stream) = CreateLoader(BuildByIndex());
        var text = await LoadAndReadAsync(sut, stream, read);

        Assert.Equal(MixedCsv, text);
    }



    [Fact]
    public async Task LoadAsync_when_runtime_type_is_unmapped_and_action_is_Throw_raises()
    {
        var (sut, stream) = CreateLoader(BuildByIndex());
        var items = new LedgerRow[] { new UnmappedRow { RecordType = "XXX" } };

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await LoadAndReadAsync(sut, stream, items));
    }



    [Fact]
    public async Task LoadAsync_when_runtime_type_is_unmapped_and_action_is_Skip_skips_the_record()
    {
        var (sut, stream) = CreateLoader(BuildByIndex(CsvDiscriminatorAction.Skip));
        var items = new LedgerRow[]
        {
            new PaymentRow { RecordType = "PMT", Account = "ACC1", Amount = 100 },
            new UnmappedRow { RecordType = "XXX" },
        };

        var text = await LoadAndReadAsync(sut, stream, items);

        Assert.Equal("PMT,ACC1,100\r\n", text);
        Assert.Equal(1, sut.CurrentItemCount);
        Assert.Equal(1, sut.CurrentSkippedItemCount);
    }



    [Fact]
    public async Task LoadAsync_when_runtime_type_is_unmapped_and_action_is_YieldAsBase_writes_the_base_shape()
    {
        var (sut, stream) = CreateLoader(BuildByIndex(CsvDiscriminatorAction.YieldAsBase));
        var items = new LedgerRow[] { new UnmappedRow { RecordType = "XXX", Note = "ignored" } };

        var text = await LoadAndReadAsync(sut, stream, items);

        // The base LedgerRow exposes only RecordType, so the Note column is not written.
        Assert.Equal("XXX\r\n", text);
    }



    [Fact]
    public async Task LoadAsync_when_dry_run_and_Discriminator_writes_nothing_but_counts()
    {
        var (sut, stream) = CreateLoader(BuildByIndex());
        sut.IsDryRun = true;

        var text = await LoadAndReadAsync(sut, stream, MixedRows);

        Assert.Equal(string.Empty, text);
        Assert.Equal(3, sut.CurrentItemCount);
    }



    [Fact]
    public async Task LoadAsync_surfaces_the_underlying_write_exception_not_the_reflection_wrapper()
    {
        var discriminator = new CsvDiscriminatorBuilder<LedgerRow>(0)
            .Map<ExplodingRow>
            (
                "BOOM",
                new[]
                {
                    new CsvColumnMap(nameof(ExplodingRow.RecordType)) { Index = 0 },
                    new CsvColumnMap(nameof(ExplodingRow.Detonate)) { Index = 1 },
                }
            )
            .Build();

        var (sut, stream) = CreateLoader(discriminator);
        var items = new LedgerRow[] { new ExplodingRow { RecordType = "BOOM" } };

        var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await LoadAndReadAsync(sut, stream, items));

        Assert.IsNotType<TargetInvocationException>(ex);
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
    public record UnmappedRow : LedgerRow
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // Set by test data; the CSV writer serializes public members so the setter
        // must exist even though no test reads Note back after construction.
        public string Note { get; set; } = string.Empty;
    }



    [ExcludeFromCodeCoverage]
    public record ExplodingRow : LedgerRow
    {
        // Instance property (not static): the test wires a bound ExplodingRow into
        // the loader; reading Detonate then triggers the throw through the
        // reflective writer. Static would break the test pattern.
#pragma warning disable S2325
        public string Detonate => throw new InvalidOperationException("boom");
#pragma warning restore S2325
    }
}
