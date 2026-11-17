# 0006 — Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-11-17
- **Decision maker(s):** @Chris-Wolfgang

## Context

`Wolfgang.Etl.Csv` makes choices that the code alone does not explain: CsvHelper sits behind a parser-agnostic surface, blocking APIs are banned by an analyzer, the library carries trim annotations without claiming to be trim-safe, the assembly version follows the minor version, and ownership of a reader, writer or file depends on who opened it. Without the reason next to it, each of these reads as an accident that a later change might "tidy up".

The repository has kept such records in `docs/adr` since ADR 0001, and [README.md](README.md) says when to write one and in what format. The practice itself was never recorded as a decision, which is why this record carries a later number than the decisions it describes.

## Decision

We record each significant, non-obvious design decision as an Architecture Decision Record in `docs/adr`, following [README.md](README.md): one Markdown file per decision, named `<NNNN>-<short-slug>.md` with the next number in sequence, started from [0000-template.md](0000-template.md), with a row in the README index. A decision that changes gets a new ADR, and the old one has its status changed to say what superseded it.

## Consequences

- A contributor can find out why something is the way it is without reading the history, and reversing a decision is explicit.
- A decision worth recording costs a few minutes to write up, and the index has to be kept in step with the files by hand.
- A record written after the fact, as this one and ADRs 0002 to 0005 are, reconstructs the reasoning and can miss part of it.

## Alternatives considered

- **Leave the reasoning in commit messages and pull requests** — rejected: it is far from the code and has to be dug out of history, if it was written down at all.

## Related

- The same practice is recorded in ETL-Abstractions ADR-0001, ETL-FixedWidth ADR 0001 and ETL-SqlBulkCopy ADR 0001.
