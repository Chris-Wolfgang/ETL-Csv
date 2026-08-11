using System.Text;
using Wolfgang.Etl.Csv;
using Wolfgang.Etl.Csv.Examples.RecordValidation;

// Streaming validation: dirty rows shouldn't abort a nightly batch. Attach per-record validators and
// pick a failure policy — here Skip, which drops invalid rows, counts them, and routes each to a
// handler, while only the valid orders reach the loop.

const string csv =
    "OrderNumber,Quantity,Notes\n" +
    "A-1,5,ships today\n" +
    ",0,missing number and zero qty\n" +       // fails NotNullOrEmpty AND GreaterThan
    "A-3,3,ok\n" +
    "A-4,7,this note is deliberately way too long for the limit\n";  // fails MaxLength

using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv)));
var extractor = new CsvExtractor<Order>(reader)
{
    Validators =
    [
        CsvValidator.NotNullOrEmpty<Order>(o => o.OrderNumber, nameof(Order.OrderNumber)),
        CsvValidator.GreaterThan<Order>(o => o.Quantity, 0, nameof(Order.Quantity)),
        CsvValidator.MaxLength<Order>(o => o.Notes, 20, nameof(Order.Notes)),
    ],
    OnValidationFailure = CsvValidationFailureAction.Skip,
    InvalidRecordHandler = invalid =>
        Console.WriteLine($"  skipped row {invalid.LineNumber}: {string.Join("; ", invalid.Failures)}"),
};

// Pass a progress sink so the invalid-count is surfaced through CsvExtractorProgress at completion.
var progress = new LastValueProgress<CsvExtractorProgress>();

Console.WriteLine("Validating while reading (invalid rows are skipped and reported):");
var valid = new List<Order>();
await foreach (var order in extractor.ExtractAsync(progress))
{
    valid.Add(order);
}

Console.WriteLine();
Console.WriteLine($"Valid orders that made it through: {string.Join(", ", valid.Select(o => o.OrderNumber))}");
Console.WriteLine($"Invalid rows skipped: {progress.Value?.CurrentInvalidItemCount ?? 0}");
