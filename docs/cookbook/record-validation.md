# Cookbook: streaming record validation

Production CSVs are full of dirty data, and "stop on the first bad row" is the wrong default for a
nightly batch of millions of rows. `CsvExtractor<TRecord>` (and, symmetrically, `CsvLoader<TRecord>`)
can run **per-record validators** after each row binds and apply a configurable failure policy — so
invalid rows are skipped, counted, and reported without aborting the run.

Runnable version: [`examples/Wolfgang.Etl.Csv.Examples.RecordValidation`](../../examples/Wolfgang.Etl.Csv.Examples.RecordValidation).

## 1. Attach validators and a policy

```csharp
var extractor = new CsvExtractor<Order>(reader)
{
    Validators =
    [
        CsvValidator.NotNullOrEmpty<Order>(o => o.OrderNumber, nameof(Order.OrderNumber)),
        CsvValidator.GreaterThan<Order>(o => o.Quantity, 0, nameof(Order.Quantity)),
        CsvValidator.MaxLength<Order>(o => o.Notes, 500, nameof(Order.Notes)),
    ],
    OnValidationFailure = CsvValidationFailureAction.Skip,
    InvalidRecordHandler = invalid =>
        _logger.LogWarning("Skipped row {Line}: {Reasons}", invalid.LineNumber, string.Join("; ", invalid.Failures)),
};

await foreach (var order in extractor.ExtractAsync())
{
    // only valid orders reach here
}
```

## 2. Pick the failure policy

`OnValidationFailure` decides what happens to a record that fails one or more validators. In **every**
case the record is counted (`CurrentInvalidItemCount`) and passed to `InvalidRecordHandler` first.

| `CsvValidationFailureAction` | Effect |
| --- | --- |
| `Stop` *(default)* | raise a `CsvValidationException` and end the run |
| `Skip` | drop the record — it is neither yielded nor written, and doesn't count as extracted/loaded |
| `Continue` | yield/write the record anyway — the caller decides what to do with it |

## 3. Built-in validators

The `CsvValidator` factory covers the common rules; each takes a selector for the member under test:

| Factory | Fails when |
| --- | --- |
| `NotNullOrEmpty(selector)` | the string is null or empty |
| `GreaterThan(selector, threshold)` | the value is not `> threshold` |
| `InRange(selector, min, max)` | the value is outside `[min, max]` |
| `MaxLength(selector, n)` | the string is longer than `n` |
| `Matches(selector, regex)` | the string doesn't match |
| `Custom(predicate, message)` | your predicate returns `false` |

`GreaterThan`/`InRange` compare via `IComparable`, so a single type argument (the record type) is enough
at the call site: `CsvValidator.GreaterThan<Order>(o => o.Quantity, 0)`. If the selected value and the
threshold are of incompatible runtime types, the record simply fails validation rather than throwing.

Multiple failing validators aggregate all their messages into a single `CsvInvalidRecord<TRecord>`.

## 4. Reading the invalid count

`CurrentInvalidItemCount` is surfaced through the progress report. Pass an `IProgress<CsvExtractorProgress>`
(or `IProgress<CsvLoaderProgress>`) to see it:

```csharp
var progress = new LastValueProgress<CsvExtractorProgress>();
await foreach (var order in extractor.ExtractAsync(progress)) { /* ... */ }
Console.WriteLine($"Invalid rows: {progress.Value?.CurrentInvalidItemCount}");
```

## 5. Validating on write

The same trio (`Validators`, `OnValidationFailure`, `InvalidRecordHandler`) exists on
`CsvLoader<TRecord>` and runs **before** each record is written — so an invalid record is never written
under `Skip`, and `CsvInvalidRecord.LineNumber` reports the record's 1-based ordinal in the input.

## Not in scope (yet)

Async validators (`Func<T, ValueTask<CsvValidationResult>>`) and folding CsvHelper type-conversion
failures into the same invalid-record stream are tracked as follow-ups; today validators run
synchronously after a row has bound, and type-conversion failures are governed by `ErrorPolicy`.
