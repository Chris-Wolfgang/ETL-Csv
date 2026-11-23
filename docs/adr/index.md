# Architecture Decision Records

See [README.md](README.md) for the folder and [ADR 0000](0000-record-architecture-decisions.md) for the practice. Copy
[`TEMPLATE.md`](TEMPLATE.md) to add a record.

| # | Title | Status |
|---|---|---|
| [0000](0000-record-architecture-decisions.md) | Record architecture decisions | Accepted |
| [0001](0001-csvhelper-as-internal-parser.md) | CsvHelper as the internal parser, parser-agnostic public surface | Accepted |
| [0002](0002-async-only-io-via-banned-symbols.md) | Async-only I/O enforced through BannedSymbols.txt | Accepted |
| [0003](0003-trim-and-native-aot-annotations.md) | Trim and Native AOT: annotate the entry points, do not claim the library is trim-safe | Accepted |
| [0004](0004-derive-assemblyversion-from-the-minor.md) | Derive AssemblyVersion as 0.{Minor}.0.0 | Accepted |
| [0005](0005-reader-writer-and-file-ownership.md) | The caller owns the reader or writer it passes in; the path-based factories own the file they open | Accepted |
| 0006 | Record architecture decisions (moved to [0000](0000-record-architecture-decisions.md)) | Retired |
