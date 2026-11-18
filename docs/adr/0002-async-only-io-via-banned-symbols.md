# 0002 — Async-only I/O enforced through BannedSymbols.txt

- **Status:** Accepted
- **Date:** 2026-11-17
- **Decision maker(s):** @Chris-Wolfgang

## Context

`CsvExtractor<T>` and `CsvLoader<T>` are stages in an `IAsyncEnumerable<T>` pipeline. A blocking call on that path (`Task.Result`, `Thread.Sleep`, a synchronous `Stream.Read`) holds a thread-pool thread for the length of the I/O and can deadlock a consumer that runs under a single-threaded synchronization context. Such a call compiles without complaint and is easy to miss in review.

The repository already carries the mechanism: a `BannedSymbols.txt` at the root, read by `Microsoft.CodeAnalysis.BannedApiAnalyzers`. On the date of this ADR the list had the same 65 entries in all eight ETL repositories. What was missing was a record of the decision and of the places where the code is synchronous on purpose.

## Decision

We enforce the async-only rule with the banned-API analyzer instead of relying on review.

- `BannedSymbols.txt` bans `Task.Wait`, `Task.WaitAll`, `Task.WaitAny`, `Task<T>.Result`, `GetAwaiter().GetResult()`, `Thread.Sleep`, the synchronous `File` helpers, `Stream.Read` / `Write` / `Flush` / `CopyTo`, `Parallel.For` / `ForEach` and the blocking `Console` reads. Each entry names its replacement.
- `.editorconfig` sets `RS0030` to `error` for the shipped code and to `none` for tests, benchmarks and examples. The shipped code has no `RS0030` suppression.
- The stages use CsvHelper's asynchronous members for everything that touches the reader or writer: `CsvReader.ReadAsync` reads each row, and `CsvWriter.NextRecordAsync` and `FlushAsync` write rows out. `ReadHeader`, `GetRecord` and `WriteRecord` are synchronous, and work on the row CsvHelper holds in memory.

Two calls are synchronous on purpose. Each is suppressed for `CA1849` with a comment at the call:

- The checkpoint writer calls `FileStream.Flush(flushToDisk: true)` before it renames the checkpoint file, because there is no asynchronous flush-to-disk.
- The path-based loader builder disposes a writer it has not written to yet, on a failure path in a method that is not async.

## Consequences

- A direct call to a banned member cannot merge into shipped code, and the build error names the replacement.
- The rule covers the listed members only. A synchronous member that is not on the list, such as one on `TextReader` or `TextWriter`, is caught by review or not at all.
- What CsvHelper does inside its asynchronous members is CsvHelper's, not this library's.
- `BannedSymbols.txt` is a protected file, so a change to the list goes in its own pull request.

## Alternatives considered

- **Rely on code review and AsyncFixer** — rejected: AsyncFixer catches some patterns, not all of the banned APIs, and review misses them under time pressure.

## Related

- Sibling records of the same rule: ETL-SqlBulkCopy ADR 0003, ETL-Transformers ADR-0001.
- Recorded after the fact: the rule was already in force when this ADR was written.
