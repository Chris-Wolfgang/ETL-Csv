using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

// These files still configure via the deprecated property setters in places where the value is
// applied after construction, so it cannot travel through the options constructor without
// restructuring the test. They keep exercising the setter path until the setters are removed.
#pragma warning disable CS0618

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvLoaderValidationTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    private static readonly Order[] Orders =
    {
        new() { OrderNumber = "A1", Quantity = 5, Notes = "ok" },
        new() { OrderNumber = "A2", Quantity = 0, Notes = "bad" },   // invalid
        new() { OrderNumber = "A3", Quantity = 3, Notes = "fine" },
    };



    private static CsvLoader<Order> CreateLoader(MemoryStream stream, out StreamWriter writer)
    {
        writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        return new CsvLoader<Order>(writer, new CsvLoaderOptions<Order>
        {
            LeaveOpen = true,
            Validators = new[] { CsvValidator.GreaterThan<Order>(o => o.Quantity, 0, "Quantity") },});
    }



    private static async Task<string> LoadAndReadAsync(CsvLoader<Order> loader, MemoryStream stream, StreamWriter writer, IEnumerable<Order> items)
    {
        await loader.LoadAsync(items.ToAsyncEnumerable());
        await writer.FlushAsync();

        return Utf8NoBom.GetString(stream.ToArray());
    }



    [Fact]
    public async Task LoadAsync_when_OnValidationFailure_is_Skip_does_not_write_invalid_records()
    {
        using var stream = new MemoryStream();
        var loader = CreateLoader(stream, out var writer);
        loader.OnValidationFailure = CsvValidationFailureAction.Skip;
        CsvInvalidRecord<Order>? captured = null;
        loader.InvalidRecordHandler = invalid => captured = invalid;

        var output = await LoadAndReadAsync(loader, stream, writer, Orders);
        writer.Dispose();

        Assert.Contains("A1", output);
        Assert.Contains("A3", output);
        Assert.DoesNotContain("A2", output);
        Assert.Equal(2, loader.CurrentItemCount);
        Assert.NotNull(captured);
        Assert.Equal("A2", captured.Record.OrderNumber);
    }



    [Fact]
    public async Task LoadAsync_reports_the_input_record_ordinal_as_the_invalid_LineNumber()
    {
        // A2 (Quantity 0) is the 2nd record in the input; its LineNumber should be 2, not the
        // last-written line (which would be 1 for the header, or 0 in dry-run).
        using var stream = new MemoryStream();
        var loader = CreateLoader(stream, out var writer);
        loader.OnValidationFailure = CsvValidationFailureAction.Skip;
        CsvInvalidRecord<Order>? captured = null;
        loader.InvalidRecordHandler = invalid => captured = invalid;

        await LoadAndReadAsync(loader, stream, writer, Orders);
        writer.Dispose();

        Assert.Equal(2, captured!.LineNumber);
    }



    [Fact]
    public async Task LoadAsync_when_OnValidationFailure_is_Continue_writes_invalid_records_too()
    {
        using var stream = new MemoryStream();
        var loader = CreateLoader(stream, out var writer);
        loader.OnValidationFailure = CsvValidationFailureAction.Continue;
        var handlerCalls = 0;
        loader.InvalidRecordHandler = _ => handlerCalls++;

        var output = await LoadAndReadAsync(loader, stream, writer, Orders);
        writer.Dispose();

        Assert.Contains("A2", output);
        Assert.Equal(3, loader.CurrentItemCount);
        Assert.Equal(1, handlerCalls);
    }



    [Fact]
    public async Task LoadAsync_when_OnValidationFailure_is_Stop_throws_after_invoking_the_handler()
    {
        using var stream = new MemoryStream();
        var loader = CreateLoader(stream, out var writer);   // Stop is the default
        var handlerCalls = 0;
        loader.InvalidRecordHandler = _ => handlerCalls++;

        var ex = await Assert.ThrowsAsync<CsvValidationException>(async () =>
            await LoadAndReadAsync(loader, stream, writer, Orders));
        writer.Dispose();

        Assert.Equal(1, handlerCalls);
        Assert.Contains("Quantity", Assert.Single(ex.Failures));
    }



    [Fact]
    public async Task LoadAsync_with_no_validators_writes_every_record()
    {
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<Order>(writer, new CsvLoaderOptions<Order>
        { LeaveOpen = true});

        var output = await LoadAndReadAsync(loader, stream, writer, Orders);
        writer.Dispose();

        Assert.Contains("A1", output);
        Assert.Contains("A2", output);
        Assert.Contains("A3", output);
        Assert.Equal(3, loader.CurrentItemCount);
    }



    [Fact]
    public async Task LoadAsync_reports_the_invalid_count_through_progress()
    {
        using var stream = new MemoryStream();
        var loader = CreateLoader(stream, out var writer);
        loader.OnValidationFailure = CsvValidationFailureAction.Skip;

        var progress = new SyncProgress<CsvLoaderProgress>();
        await loader.LoadAsync(Orders.ToAsyncEnumerable(), progress);
        writer.Dispose();

        Assert.Equal(1, progress.LastValue!.CurrentInvalidItemCount);
    }



    [Fact]
    public void CsvLoaderProgress_exposes_the_invalid_count_via_the_new_constructor()
    {
        var progress = new CsvLoaderProgress(5, 1, 9, 2);

        Assert.Equal(5, progress.CurrentItemCount);
        Assert.Equal(2, progress.CurrentInvalidItemCount);
    }



    [ExcludeFromCodeCoverage]
    public record Order
    {
        public string OrderNumber { get; set; } = string.Empty;



        public int Quantity { get; set; }



        public string Notes { get; set; } = string.Empty;
    }
}
