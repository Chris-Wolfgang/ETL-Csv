# Architecture Decision Records

This directory captures the **why** behind significant architectural choices in Wolfgang.Etl.Csv.

Code comments explain *what* a line of code does and *how* — ADRs explain *why* the larger decision was made, what alternatives were considered, and what the tradeoffs were. They survive refactors, package upgrades, and team turnover.

## When to write an ADR

Write one when:

- A choice locks in a tradeoff that's not obvious from the code
- A choice closes off a path that a future maintainer might reasonably want to take
- The decision was contentious or had non-trivial alternatives
- The rationale lives only in a PR thread, issue comment, or Slack message — places where it'll be hard to find later

Don't write one for tactical choices captured fine by code comments or PR descriptions.

## Format

Each ADR is a single markdown file named `<NNNN>-<short-slug>.md`, where `NNNN` is a 4-digit zero-padded sequence number starting at `0001`. Use `0000-template.md` as the starting point.

## Index

| # | Title | Status |
|---|---|---|
| [0001](0001-csvhelper-as-internal-parser.md) | CsvHelper as the internal parser, parser-agnostic public surface | Accepted |
| [0002](0002-async-only-io-via-banned-symbols.md) | Async-only I/O enforced through BannedSymbols.txt | Accepted |
| [0003](0003-trim-and-native-aot-annotations.md) | Trim and Native AOT: annotate the entry points, do not claim the library is trim-safe | Accepted |
| [0004](0004-derive-assemblyversion-from-the-minor.md) | Derive AssemblyVersion as 0.{Minor}.0.0 | Accepted |
| [0005](0005-reader-writer-and-file-ownership.md) | The caller owns the reader or writer it passes in; the path-based factories own the file they open | Accepted |
| [0006](0006-record-architecture-decisions.md) | Record architecture decisions | Accepted |
