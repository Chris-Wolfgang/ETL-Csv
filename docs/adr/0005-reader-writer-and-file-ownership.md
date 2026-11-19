# 0005 — The caller owns the reader or writer it passes in; the path-based factories own the file they open

- **Status:** Accepted
- **Date:** 2026-11-17
- **Decision maker(s):** @Chris-Wolfgang

## Context

A stage can be given its data two ways: a `StreamReader` or `StreamWriter` the caller already has, or (through the `EtlPipeline` factories) a file path. Each raises the same questions: who closes it, when it is opened, and what is left behind when a run fails.

Closing something the caller still needs breaks a `MemoryStream` that is read back after the run. Opening a file earlier than necessary, or leaving it open, leaks the handle. Writing straight into the destination leaves a half-written file where a good one used to be.

The rules the code follows were in the XML documentation and in comments in the pipeline builders, not in one place.

## Decision

**A reader or writer the caller passes in belongs to the caller.**

- `CsvExtractor<T>` takes a `StreamReader` and `CsvLoader<T>` takes a `StreamWriter`.
- `LeaveOpen` on the options record defaults to `true`: the stage does not dispose it.
- A caller who sets `LeaveOpen` to `false` hands it over. It is then disposed with the CSV parser or writer at the end of the run, not when the stage is disposed.
- The pipeline factories that accept a `StreamReader`, a `StreamWriter` or a pre-built extractor or loader do not dispose it either.

**A file the library opens belongs to the library.**

- A path-based source is opened when enumeration starts, not when the pipeline is composed, and is closed when enumeration ends.
- A path-based sink is written to a temporary file beside the destination. The temporary file replaces the destination only when the run succeeds. A run that fails or is cancelled leaves the destination as it was.
- Both are disposed after the run, on success and on failure.

## Consequences

- A caller can read back a `MemoryStream`, or keep using a reader or writer, after the run.
- A pipeline that is composed and never run opens no file.
- A failed or cancelled load does not truncate or corrupt an existing destination file.
- The destination's directory must allow creating and renaming a file, and for a moment there are two files there.
- With `LeaveOpen = false` the reader or writer is gone after the first run, so the stage cannot be run on it again.
- The sibling libraries do not all make the same choice. ETL-Xml honours a `LeaveOpen` option on caller-supplied streams (ETL-Xml ADR-003); this library applies it to the `StreamReader` or `StreamWriter`.

## Alternatives considered

- **Dispose what the caller passes in by default** — rejected: the common cases (a `MemoryStream` read back in a test, a stream the caller goes on using) would break, and the caller already has a `using` for it.
- **Open the path-based source when the pipeline is composed** — rejected: a pipeline that is never enumerated would leak the handle.
- **Write the path-based sink straight into the destination** — rejected: opening a `StreamWriter` with `append: false` truncates the destination before anything is known about whether the run will succeed.

## Related

- Issues #700 and #701, which the lazy open and the temporary file answer.
- Recorded after the fact: the behaviour was already in force when this ADR was written.
