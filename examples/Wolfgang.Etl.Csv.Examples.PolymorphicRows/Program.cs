using System.Text;
using Wolfgang.Etl.Csv;
using Wolfgang.Etl.Csv.Examples.PolymorphicRows;

// Polymorphic rows: one CSV mixes three shapes keyed by the first column. A CsvDiscriminator binds
// each row to the right concrete type on read, and writes each record back by its runtime type on load
// — so the file round-trips byte-for-byte.

const string csv =
    "HDR,B001,2026-01-15\n" +
    "PMT,ACC-100,250\n" +
    "PMT,ACC-205,75\n" +
    "TRL,2\n";

// Built with the fluent, trim/AOT-safe builder. Each Map<T>(value, columns) names the concrete type
// generically and pins its columns by index (there is no shared header across the shapes).
var discriminator = new CsvDiscriminatorBuilder<LedgerRow>(columnIndex: 0)
    .Map<HeaderRow>("HDR", new[]
    {
        new CsvColumnMap(nameof(HeaderRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(HeaderRow.BatchId)) { Index = 1 },
        new CsvColumnMap(nameof(HeaderRow.BatchDate)) { Index = 2 },
    })
    .Map<PaymentRow>("PMT", new[]
    {
        new CsvColumnMap(nameof(PaymentRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(PaymentRow.Account)) { Index = 1 },
        new CsvColumnMap(nameof(PaymentRow.Amount)) { Index = 2 },
    })
    .Map<TrailerRow>("TRL", new[]
    {
        new CsvColumnMap(nameof(TrailerRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(TrailerRow.Count)) { Index = 1 },
    })
    .Build();

Console.WriteLine("Reading a mixed-shape file — each row binds to its concrete type:");
var rows = new List<LedgerRow>();
using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv))))
{
    var extractor = new CsvExtractor<LedgerRow>(reader)
    {
        HasHeaderRecord = false,
        Discriminator = discriminator,
    };

    await foreach (var row in extractor.ExtractAsync())
    {
        rows.Add(row);
        Console.WriteLine($"  {row.GetType().Name,-10} {row}");
    }
}

Console.WriteLine();
Console.WriteLine("Writing them back — the loader dispatches by runtime type:");
using var buffer = new MemoryStream();
using (var writer = new StreamWriter(buffer, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true))
{
    var loader = new CsvLoader<LedgerRow>(writer) { Discriminator = discriminator };
    await loader.LoadAsync(ToAsync(rows));
    await writer.FlushAsync();
}

Console.Write(Encoding.UTF8.GetString(buffer.ToArray()));

static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> items)
{
    foreach (var item in items)
    {
        yield return item;
    }

    await Task.CompletedTask;
}
