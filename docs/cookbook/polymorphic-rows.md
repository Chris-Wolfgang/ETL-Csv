# Cookbook: polymorphic rows (mixed shapes in one file)

Some CSV files aren't one flat table — they interleave **different record shapes** keyed by a
discriminator column. A classic example is a batch file with a header row, many detail rows, and a
trailer row:

```
HDR,B001,2026-01-15
PMT,ACC-100,250
PMT,ACC-205,75
TRL,2
```

Each row's first column says what it is (`HDR` / `PMT` / `TRL`), and each shape has different columns.
A `CsvDiscriminator<TBase>` binds each row to the right concrete type on read, and writes each record
back by its runtime type on load — so the file round-trips.

Runnable version: [`examples/Wolfgang.Etl.Csv.Examples.PolymorphicRows`](../../examples/Wolfgang.Etl.Csv.Examples.PolymorphicRows).

## 1. Model the shapes

Give the shapes a common base (concrete, so `YieldAsBase` can fall back to it if you use that policy):

```csharp
public record LedgerRow { public string RecordType { get; set; } = ""; }
public record HeaderRow  : LedgerRow { public string BatchId { get; set; } = ""; public string BatchDate { get; set; } = ""; }
public record PaymentRow : LedgerRow { public string Account { get; set; } = ""; public decimal Amount { get; set; } }
public record TrailerRow : LedgerRow { public int Count { get; set; } }
```

## 2. Build the discriminator

Use `CsvDiscriminatorBuilder<TBase>` — it names each concrete type generically, so property metadata is
preserved for trimming/AOT. Because the shapes share no header, bind each type's columns by index:

```csharp
var discriminator = new CsvDiscriminatorBuilder<LedgerRow>(columnIndex: 0)
    .Map<HeaderRow>("HDR", new[]
    {
        new CsvColumnMap(nameof(HeaderRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(HeaderRow.BatchId))    { Index = 1 },
        new CsvColumnMap(nameof(HeaderRow.BatchDate))  { Index = 2 },
    })
    .Map<PaymentRow>("PMT", new[]
    {
        new CsvColumnMap(nameof(PaymentRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(PaymentRow.Account))    { Index = 1 },
        new CsvColumnMap(nameof(PaymentRow.Amount))     { Index = 2 },
    })
    .Map<TrailerRow>("TRL", new[]
    {
        new CsvColumnMap(nameof(TrailerRow.RecordType)) { Index = 0 },
        new CsvColumnMap(nameof(TrailerRow.Count))      { Index = 1 },
    })
    .Build();
```

To key off a **named** header column instead of an index, construct the builder with
`new CsvDiscriminatorBuilder<LedgerRow>("Kind")` and leave `HasHeaderRecord = true`.

## 3. Read

```csharp
var extractor = new CsvExtractor<LedgerRow>(reader)
{
    HasHeaderRecord = false,
    Discriminator = discriminator,
};

await foreach (var row in extractor.ExtractAsync())
{
    // row is a HeaderRow / PaymentRow / TrailerRow — switch on the concrete type
}
```

While a discriminator is set, missing trailing fields are tolerated, so narrower shapes (like the
two-column `TRL`) bind cleanly.

## 4. Write it back

The same discriminator drives the loader, which dispatches by each record's runtime type:

```csharp
var loader = new CsvLoader<LedgerRow>(writer) { Discriminator = discriminator };
await loader.LoadAsync(rows);
```

No header row is written while a discriminator is set (the shapes have no common header). The
discriminator **value** travels as a normal mapped property (here `RecordType`), which is what makes the
read → write round-trip reproduce the original bytes.

## 5. Unknown values

A discriminator value with no mapping is handled by `CsvDiscriminatorAction`:

| Action | On read | On write |
| --- | --- | --- |
| `Throw` *(default)* | raises for the unmapped value | raises for the unmapped runtime type |
| `Skip` | drops the row, bumps the skipped count | drops the record |
| `YieldAsBase` | binds the row to `TBase` via its base mapping | writes the record as `TBase` |

Set it with `.OnUnknown(CsvDiscriminatorAction.Skip)` on the builder. For `YieldAsBase`, assign the
extractor/loader `ColumnMaps` (or decorate `TBase` with attributes) so the base binding is meaningful.
