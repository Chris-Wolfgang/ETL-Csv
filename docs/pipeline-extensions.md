# CSV pipeline extensions

`Wolfgang.Etl.Csv` hangs class-named CSV source factories and sink terminators off the generic
`EtlPipeline` chain introduced in `Wolfgang.Etl.Abstractions` 0.16.0. A pipeline reads as one fluent
statement — start from a CSV source, append pipeline operators, terminate with a CSV sink, then run:

```csharp
await EtlPipeline
    .Create()
    .CsvExtractor<Order>("orders.csv")
    .Delimiter("|")
    .HasHeaderRecord(true)
    .InitialRecordIndex(3)
    .Where(o => o.Quantity > 0)
    .Select(o => new EnrichedOrder(o, Lookup(o.CustomerId)))
    .CsvLoader<EnrichedOrder>("enriched.csv")
    .RunAsync(progress, token);
```

## The two halves

### Source factories — `EtlPipeline.Create().CsvExtractor<T>(…)`

Three overloads begin a pipeline and return `ICsvExtractorBuilder<T>`:

| Overload | Ownership |
| --- | --- |
| `CsvExtractor<T>(string path)` | The factory **owns** the file stream: it is opened when the pipeline is enumerated and disposed when enumeration ends (success, failure or abandonment). Composing a pipeline that is never run holds no file handle. |
| `CsvExtractor<T>(StreamReader reader)` | The **caller** owns the reader. |
| `CsvExtractor<T>(CsvExtractor<T> extractor)` | The **caller** owns the extractor and its stream. |

### Sink terminators — `pipeline.CsvLoader<T>(…)`

Three overloads terminate a pipeline and return `ICsvLoaderBuilder<T>` (an `IEtlPipelineSink`):

| Overload | Ownership |
| --- | --- |
| `CsvLoader<T>(string path)` | The terminator **owns** the file stream and disposes it after the run. Output goes to a temporary file beside the destination that replaces it only when the run succeeds, so a failed or cancelled run leaves an existing file untouched. `RunAsync` can be called once per sink. |
| `CsvLoader<T>(StreamWriter writer)` | The **caller** owns the writer. |
| `CsvLoader<T>(CsvLoader<T> loader)` | The **caller** owns the loader and its stream. |

## Inline configuration

Each builder exposes fluent setters for the most common `CsvExtractor<T>` / `CsvLoader<T>` options
(listed below). The **first pipeline operator** you call (`Where`, `Select`, `Through`, `CsvLoader`, …)
narrows the builder to `IEtlPipeline<T>` — the configuration setters fall off the surface, so there is
no explicit `Build()` step.

- **Extractor:** `Delimiter`, `Quote`, `Escape`, `Comment`, `AllowComments`, `IgnoreBlankLines`,
  `HasHeaderRecord`, `Encoding`, `InitialRecordIndex`, `SkipRecordCount`, `MaxRecordCount`,
  `TrimOptions`, `ColumnMaps`, `BadDataFound`, `ReadingExceptionOccurred`.
- **Loader:** `Delimiter`, `Quote`, `Escape`, `NewLine`, `HasHeaderRecord`, `Encoding`, `TrimOptions`,
  `ShouldQuote`, `ColumnMaps`, `IsDryRun`.

Most setters map to the property of the same name; `Encoding(…)` is the exception — it selects the
encoding used to open a path-based stream, so it controls the bytes actually read from / written to disk,
and it has no effect on a caller-supplied reader or writer. `SkipRecordCount` / `MaxRecordCount` map to
`SkipItemCount` / `MaximumItemCount`.

The following options have **no** fluent setter. To use them, build the `CsvExtractor<T>` / `CsvLoader<T>`
yourself with the options record and pass it to the `CsvExtractor<T>(extractor)` / `CsvLoader<T>(loader)`
overloads:

- **Extractor:** `Schema`, `Discriminator`, `Validators`, `OnValidationFailure`, `InvalidRecordHandler`,
  `ErrorPolicy`, `LeaveOpen`.
- **Loader:** `Schema`, `Discriminator`, `Validators`, `OnValidationFailure`, `InvalidRecordHandler`,
  `LeaveOpen`.

## Where do `Where` / `Select` come from?

The pipeline core (`IEtlPipeline<T>`) provides only `Through` — the stream-to-stream primitive. The
LINQ-flavored operators (`Where`, `Select`, `Distinct`, `Take`, …) are extension methods shipped by
`Wolfgang.Etl.Transformers`. If you do not reference that package, express the same work inline with
`Through`:

```csharp
// C# does not allow `yield` inside a lambda, so the stage is a local (or static) iterator method.
static async IAsyncEnumerable<Order> PositiveQuantities(IAsyncEnumerable<Order> source)
{
    await foreach (var order in source)
    {
        if (order.Quantity > 0)
        {
            yield return order;
        }
    }
}

await EtlPipeline
    .Create()
    .CsvExtractor<Order>("orders.csv")
    .Through<Order>(PositiveQuantities)
    .CsvLoader<Order>("filtered.csv")
    .RunAsync();
```

## Caller-owned streams

When you already hold a `StreamReader` / `StreamWriter`, pass it in and keep control of its lifetime:

```csharp
using var reader = new StreamReader("orders.csv");
using var writer = new StreamWriter("out.csv");

await EtlPipeline
    .Create()
    .CsvExtractor<Order>(reader)
    .CsvLoader<Order>(writer)
    .RunAsync();

// reader and writer are still open here — you dispose them (the using blocks do).
```

## Cross-format pipelines

Because the source and sink are independent extension packages over the same `EtlPipeline` core, a CSV
source can feed any sibling sink (and vice versa). For example, with `Wolfgang.Etl.Json`'s terminators
referenced:

```csharp
await EtlPipeline
    .Create()
    .CsvExtractor<Order>("orders.csv")
    .Delimiter("|")
    .JsonLineLoader<Order>("orders.jsonl")
    .RunAsync();
```
