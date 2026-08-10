using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvExtractorValidationTests
{
    private const string Csv =
        "OrderNumber,Quantity,Notes\n" +
        "A1,5,ok\n" +
        "A2,0,bad\n" +   // Quantity 0 -> invalid
        "A3,3,fine\n";



    private static CsvExtractor<Order> CreateExtractor(string csv)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        return new CsvExtractor<Order>(new StreamReader(stream, Encoding.UTF8))
        {
            Validators = new[] { CsvValidator.GreaterThan<Order>(o => o.Quantity, 0, "Quantity") },
        };
    }



    private static async Task<List<Order>> ReadAllAsync(CsvExtractor<Order> sut, IProgress<CsvExtractorProgress>? progress = null)
    {
        var list = new List<Order>();
        var stream = progress is null ? sut.ExtractAsync() : sut.ExtractAsync(progress);
        await foreach (var item in stream)
        {
            list.Add(item);
        }

        return list;
    }



    [Fact]
    public async Task ExtractAsync_with_no_validators_yields_every_record()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(Csv));
        var sut = new CsvExtractor<Order>(new StreamReader(stream, Encoding.UTF8));

        var rows = await ReadAllAsync(sut);

        Assert.Equal(3, rows.Count);
    }



    [Fact]
    public async Task ExtractAsync_tolerates_a_validator_that_returns_null_failures()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("OrderNumber,Quantity,Notes\nA1,5,ok\n"));
        var sut = new CsvExtractor<Order>(new StreamReader(stream, Encoding.UTF8))
        {
            OnValidationFailure = CsvValidationFailureAction.Skip,
            Validators = new CsvValidator<Order>[] { _ => new CsvValidationResult(false, null!) },
        };

        var rows = await ReadAllAsync(sut);

        Assert.Empty(rows);   // record failed (and was skipped) without an NRE from null Failures
    }



    [Fact]
    public async Task ExtractAsync_when_OnValidationFailure_is_Skip_drops_invalid_records()
    {
        CsvInvalidRecord<Order>? captured = null;
        var sut = CreateExtractor(Csv);
        sut.OnValidationFailure = CsvValidationFailureAction.Skip;
        sut.InvalidRecordHandler = invalid => captured = invalid;

        var progress = new SyncProgress<CsvExtractorProgress>();
        var rows = await ReadAllAsync(sut, progress);

        Assert.Equal(new[] { "A1", "A3" }, rows.ConvertAll(o => o.OrderNumber));
        Assert.Equal(1, progress.LastValue!.CurrentInvalidItemCount);
        Assert.NotNull(captured);
        Assert.Equal("A2", captured!.Record.OrderNumber);
        Assert.Contains("Quantity", Assert.Single(captured.Failures));
    }



    [Fact]
    public async Task ExtractAsync_when_OnValidationFailure_is_Continue_yields_invalid_records_but_still_counts_them()
    {
        var handlerCalls = 0;
        var sut = CreateExtractor(Csv);
        sut.OnValidationFailure = CsvValidationFailureAction.Continue;
        sut.InvalidRecordHandler = _ => handlerCalls++;

        var progress = new SyncProgress<CsvExtractorProgress>();
        var rows = await ReadAllAsync(sut, progress);

        Assert.Equal(3, rows.Count);
        Assert.Equal(1, handlerCalls);
        Assert.Equal(1, progress.LastValue!.CurrentInvalidItemCount);
    }



    [Fact]
    public async Task ExtractAsync_when_OnValidationFailure_is_Stop_throws_after_invoking_the_handler()
    {
        var handlerCalls = 0;
        var sut = CreateExtractor(Csv);          // Stop is the default
        sut.InvalidRecordHandler = _ => handlerCalls++;

        var ex = await Assert.ThrowsAsync<CsvValidationException>(async () => await ReadAllAsync(sut));

        Assert.Equal(1, handlerCalls);
        Assert.Contains("Quantity", Assert.Single(ex.Failures));
    }



    [Fact]
    public async Task ExtractAsync_aggregates_failures_from_every_validator()
    {
        CsvInvalidRecord<Order>? captured = null;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("OrderNumber,Quantity,Notes\n,0,toolong\n"));
        var sut = new CsvExtractor<Order>(new StreamReader(stream, Encoding.UTF8))
        {
            OnValidationFailure = CsvValidationFailureAction.Skip,
            Validators = new[]
            {
                CsvValidator.NotNullOrEmpty<Order>(o => o.OrderNumber, "OrderNumber"),
                CsvValidator.GreaterThan<Order>(o => o.Quantity, 0, "Quantity"),
                CsvValidator.MaxLength<Order>(o => o.Notes, 3, "Notes"),
            },
            InvalidRecordHandler = invalid => captured = invalid,
        };

        var rows = await ReadAllAsync(sut);

        Assert.Empty(rows);
        Assert.NotNull(captured);
        Assert.Equal(3, captured!.Failures.Count);
    }



    [ExcludeFromCodeCoverage]
    public record Order
    {
        public string OrderNumber { get; set; } = string.Empty;



        public int Quantity { get; set; }



        public string Notes { get; set; } = string.Empty;
    }
}
