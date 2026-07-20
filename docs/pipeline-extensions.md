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
| `CsvExtractor<T>(string path)` | The factory **owns** the file stream and disposes it when the run ends (success or failure). |
| `CsvExtractor<T>(StreamReader reader)` | The **caller** owns the reader. |
| `CsvExtractor<T>(CsvExtractor<T> extractor)` | The **caller** owns the extractor and its stream. |

### Sink terminators — `pipeline.CsvLoader<T>(…)`

Three overloads terminate a pipeline and return `ICsvLoaderBuilder<T>` (an `IEtlPipelineSink`):

| Overload | Ownership |
| --- | --- |
| `CsvLoader<T>(string path)` | The terminator **owns** the file stream and disposes it after the run. |
| `CsvLoader<T>(StreamWriter writer)` | The **caller** owns the writer. |
| `CsvLoader<T>(CsvLoader<T> loader)` | The **caller** owns the loader and its stream. |

## Inline configuration

Each builder exposes fluent setters that map 1:1 to the underlying `CsvExtractor<T>` / `CsvLoader<T>`
properties. The **first pipeline operator** you call (`Where`, `Select`, `Through`, `CsvLoader`, …)
narrows the builder to `IEtlPipeline<T>` — the configuration setters fall off the surface, so there is
no explicit `Build()` step.

- **Extractor:** `Delimiter`, `Quote`, `Escape`, `Comment`, `AllowComments`, `IgnoreBlankLines`,
  `HasHeaderRecord`, `Encoding`, `InitialRecordIndex`, `SkipRecordCount`, `MaxRecordCount`,
  `TrimOptions`, `ColumnMaps`, `BadDataFound`, `ReadingExceptionOccurred`.
- **Loader:** `Delimiter`, `Quote`, `Escape`, `NewLine`, `HasHeaderRecord`, `Encoding`, `TrimOptions`,
  `ShouldQuote`, `ColumnMaps`.

`Encoding(…)` binds the encoding used to open a path-based stream, so it controls the bytes actually
read from / written to disk.

## Where do `Where` / `Select` come from?

The pipeline core (`IEtlPipeline<T>`) provides only `Through` — the stream-to-stream primitive. The
LINQ-flavored operators (`Where`, `Select`, `Distinct`, `Take`, …) are extension methods shipped by
`Wolfgang.Etl.Transformers`. If you do not reference that package, express the same work inline with
`Through`:

```csharp
await EtlPipeline
    .Create()
    .CsvExtractor<Order>("orders.csv")
    .Through(async source =>
    {
        await foreach (var order in source)
        {
            if (order.Quantity > 0)
            {
                yield return order;
            }
        }
    })
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
