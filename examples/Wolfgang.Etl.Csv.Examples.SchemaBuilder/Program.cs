using System.Text;
using Wolfgang.Etl.Csv;
using Wolfgang.Etl.Csv.Examples.SchemaBuilder;

// The Order record has no [CsvColumn] attributes. Instead of decorating it, describe the CSV
// layout with a fluent, compile-time-checked schema. Property selectors (o => o.Id) are checked
// by the compiler and survive a rename, and the result is the same CsvColumnMap the attribute
// path produces — so a code-built schema behaves identically to an attributed record.
var schema = new CsvSchemaBuilder<Order>()
    .Column(o => o.Id, name: "order_id")
    .Column(o => o.PlacedOn, name: "placed_at", format: "yyyy-MM-dd")
    .Column(o => o.Total, name: "amount", format: "0.00")
    .Build();

const string csv =
    "order_id,placed_at,amount\n" +
    "1001,2026-01-15,42.50\n" +
    "1002,2026-02-03,199.00\n";

Console.WriteLine("Reading with a code-built schema (Order has no attributes):");
Console.WriteLine();

using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv))))
{
    var extractor = new CsvExtractor<Order>(reader) { ColumnMaps = schema };
    await foreach (var order in extractor.ExtractAsync())
    {
        Console.WriteLine($"  #{order.Id}  placed {order.PlacedOn:yyyy-MM-dd}  total {order.Total:0.00}");
    }
}

Console.WriteLine();
Console.WriteLine("Writing the same records back out with the same schema:");
Console.WriteLine();

var orders = new[]
{
    new Order { Id = 2001, PlacedOn = new DateTime(2026, 3, 1), Total = 12.00m },
    new Order { Id = 2002, PlacedOn = new DateTime(2026, 3, 2), Total = 8.75m },
};

using (var writer = new StreamWriter(Console.OpenStandardOutput(), leaveOpen: true))
{
    var loader = new CsvLoader<Order>(writer) { LeaveOpen = true, ColumnMaps = schema };
    await loader.LoadAsync(ToAsync(orders));
    await writer.FlushAsync();
}

static async IAsyncEnumerable<Order> ToAsync(IEnumerable<Order> items)
{
    foreach (var item in items)
    {
        yield return item;
    }

    await Task.CompletedTask;
}
