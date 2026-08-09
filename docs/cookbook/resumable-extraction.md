# Cookbook: resumable extraction

Resuming a large CSV extraction after a crash — without reprocessing everything — is **a discipline, not a feature**. Everything you need is already in the public API: `SkipRecordCount` (and `InitialRecordIndex`) to skip past what's done, plus a small durable counter. The optional `CsvCheckpointExtensions` helper (see below) covers the mechanical read/write so you don't hand-roll it — but *when* to acknowledge a record is application-specific, so that part stays yours.

## 1. The problem

You're streaming a multi-gigabyte file into a database. Two hours in, the process crashes. Starting over reprocesses everything you already committed — wasted hours, and duplicate work downstream.

You want to pick up where you left off.

## 2. The trap

The obvious-but-wrong approach is to bump a counter inside the loop as each record is *yielded*:

```csharp
var processed = 0;
await foreach (var order in extractor.ExtractAsync(ct))
{
    await SaveAsync(order, ct);   // may not have committed yet
    processed++;                  // ← WRONG place to acknowledge
    await WriteCheckpointAsync(processed);
}
```

A record being yielded by `ExtractAsync` does **not** mean it's durably committed downstream. If you checkpoint here and then crash *before* `SaveAsync`'s transaction commits, the resume skips a row that was never actually saved. **You lose records.**

## 3. The pattern

Acknowledge a record **only after the downstream side-effect has committed** — and count *committed* records, not *read* records.

```csharp
var processed = await CsvCheckpointExtensions.ReadCheckpointAsync(checkpointPath, ct);   // 0 if absent

using var reader = new StreamReader("orders.csv");
var extractor = new CsvExtractor<Order>(reader) { SkipRecordCount = processed };

await foreach (var order in extractor.ExtractAsync(ct))
{
    using var tx = await db.BeginTransactionAsync(ct);
    await SaveAsync(order, tx, ct);
    await tx.CommitAsync(ct);     // the side-effect is now durable

    processed++;                  // ← acknowledge AFTER the commit
    await CsvCheckpointExtensions.WriteCheckpointAsync(checkpointPath, processed, ct);
}
```

Checkpointing after every record is simplest and safest. If the per-record checkpoint write is a measurable cost, batch it (`if (processed % 1000 == 0) …`) plus a final write after the loop — but understand the trade-off in §8.

## 4. Atomic write (why the temp-file dance)

A checkpoint file must never be left **torn**. If you write the counter in place and crash mid-write, the file can contain a truncated or garbage value, and the next resume reads nonsense.

The fix is write-to-temp-then-rename: write the new value to `path + ".tmp"`, then atomically rename it over the target. A crash leaves either the *old* file intact or the *new* file complete — never a partial one, because the rename is atomic at the filesystem level.

`CsvCheckpointExtensions.WriteCheckpointAsync` does exactly this (and `ReadCheckpointAsync` returns `0` for a missing file and throws `FormatException` on non-integer content — corruption is loud, not silent). If you'd rather see the raw idiom:

```csharp
static async Task WriteCheckpointAtomicAsync(string path, int count, CancellationToken ct)
{
    var temp = path + ".tmp";
    await File.WriteAllTextAsync(temp, count.ToString(CultureInfo.InvariantCulture), ct);
    File.Move(temp, path, overwrite: true);   // atomic replace (net8+; File.Replace on older TFMs)
}
```

## 5. Resume

On startup, read the counter (defaulting to `0` when the file doesn't exist yet) and set `SkipRecordCount` to it. The convenience method does both:

```csharp
var extractor = new CsvExtractor<Order>(reader);
var resumedFrom = await extractor.ResumeFromCheckpointAsync(checkpointPath, ct);   // sets SkipRecordCount, returns the count
```

`SkipRecordCount` skips *data* records; if your file has metadata rows before the header, combine it with `InitialRecordIndex` (which positions the first line read) exactly as you would on a fresh run.

## 6. Where to put the acknowledgement

The ack must happen *after* the durable commit — and inside the same success path.

**SQL:** acknowledge after the transaction commits.

```csharp
using var tx = await db.BeginTransactionAsync(ct);
await SaveAsync(order, tx, ct);
await tx.CommitAsync(ct);
processed++;
await CsvCheckpointExtensions.WriteCheckpointAsync(checkpointPath, processed, ct);
```

**Message queue:** acknowledge after the broker confirms the publish.

```csharp
await publisher.PublishAsync(order, ct);   // completes only when the broker has accepted it
processed++;
await CsvCheckpointExtensions.WriteCheckpointAsync(checkpointPath, processed, ct);
```

If the commit/publish throws, you do **not** advance the checkpoint — the record will be re-read on the next run. That's the at-least-once guarantee (see §8).

## 7. Multi-source files

When you concatenate N files into one logical stream, a single record counter isn't enough — you need to know *which file* you were in. Persist a small `(fileIndex, recordsCommittedInThatFile)` pair instead of a bare integer, and on resume skip whole files up to `fileIndex`, then set `SkipRecordCount` on that source:

```csharp
for (var i = checkpoint.FileIndex; i < files.Count; i++)
{
    using var reader = new StreamReader(files[i]);
    var extractor = new CsvExtractor<Order>(reader)
    {
        SkipRecordCount = i == checkpoint.FileIndex ? checkpoint.RecordsInFile : 0,
    };

    await foreach (var order in extractor.ExtractAsync(ct))
    {
        // ... commit, then persist (i, ++recordsInFile) atomically ...
    }
}
```

## 8. What this does **not** give you

- **Exactly-once.** This is **at-least-once**: if you crash in the window between the downstream commit and the checkpoint write, the record is re-processed on resume. Achieving exactly-once requires the downstream to be **idempotent** (e.g. an upsert keyed by a natural id, or a dedup table) — the extractor can't provide it.
- **Mid-record resume.** CSV is line-based, so you always resume on a **row boundary**. A record that was half-read when the process died is simply read again from the start on resume.

## See also

- Runnable example: [`examples/Wolfgang.Etl.Csv.Examples.ResumableExtraction/`](../../examples/Wolfgang.Etl.Csv.Examples.ResumableExtraction/) — a crash-then-resume demo.
- Helper API: `CsvCheckpointExtensions` (`ReadCheckpointAsync`, `WriteCheckpointAsync`, `ResumeFromCheckpointAsync`).
