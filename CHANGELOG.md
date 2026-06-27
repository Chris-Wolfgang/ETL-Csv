# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security



## [0.2.0] - 2026-06-27

Post-0.1.0 polish: governance docs, security infrastructure, real `net462`/`netstandard` deadlock fix in `CsvLoader`, ABI / allocation tracking activated. No public API additions or removals.

### Added

- `docs/adr/` — Architecture Decision Records (with template and `0001-csvhelper-as-internal-parser.md`).
- `docs/migration/` — placeholder for per-major-version migration guides.
- `docs/disaster-recovery.md` — runbook for NuGet / GitHub account / repo compromise scenarios.
- `docs/abi-stability.md` + `scripts/check-api-compat.ps1` — SemVer policy + one-shot ApiCompat diff against the latest published NuGet.
- `docs/license-audit.md` — 2026-06-20 snapshot of transitive dependency licenses (all MIT-compatible).
- `docs/reproducible-build-verification.md` — manual recipe to verify a tagged build reproduces byte-for-byte against the published `.nupkg`.
- `.github/workflows/scorecard.yaml` — OSSF Scorecard automated security-posture analysis.
- `.github/workflows/workflow-security.yaml` — actionlint + zizmor lint of every workflow YAML.
- `.github/workflows/benchmarks.yaml` — already-shipping BDN → `dev/bench/` chart workflow now in main release line.
- `src/Wolfgang.Etl.Csv/PublicAPI.Shipped.txt` populated with the 0.1.0 baseline. `Microsoft.CodeAnalysis.PublicApiAnalyzers` now enforces `RS0016`/`RS0017` against accidental API additions/removals.
- `tests/Wolfgang.Etl.Csv.Tests.Unit/CsvAllocationProfileTests.cs` — `GC.GetAllocatedBytesForCurrentThread` regression guards for the extract / load hot paths.
- `tests/Wolfgang.Etl.Csv.Tests.Unit/CsvCultureInvarianceTests.cs` — `tr-TR` / `de-DE` / `ja-JP` culture matrix.
- `tests/Wolfgang.Etl.Csv.Tests.Unit/CsvXmlDocExampleRotTests.cs` — Roslyn-parses every `<example><code>` block in the generated XML doc, fails on rot.

### Changed

- `CsvLoader.cs` `await using var csvWriter = new CsvWriter(...)` split into construction + explicit `ConfiguredAsyncDisposable`. **Closes a real `SynchronizationContext`-capture deadlock surface on `net462` / `netstandard2.0` consumers using sync-over-async** — the previous `#pragma warning disable CA2007, MA0004` silenced the analyzer but did not eliminate the underlying risk.
- `CsvLogMessages.cs` annotated with the per-category `EventId` numbering scheme and the never-reuse-retired-IDs policy.
- `Wolfgang.Etl.Csv.csproj` — `<AssemblyVersion>` / `<FileVersion>` removed; SDK derives both from `<Version>` so they cannot drift.
- README target frameworks table replaced with the literal 5 TFMs the `.nupkg` ships; prerequisite corrected to `.NET 10.0 SDK`.
- `ETL-Csv.slnx` fully re-synchronized with the working tree (5 stale references removed, 17 missing files added).
- `actions/checkout@v6` → `@v7` standardized across all workflows.

### Removed

- `docs/SECURITY.md` (duplicate of root `SECURITY.md`).



## [0.1.0] - 2026-06-20

Initial release.

### Added

- `CsvExtractor<TRecord>` — async streaming CSV reader implementing `Wolfgang.Etl.Abstractions.ExtractorBase<TRecord, CsvExtractorProgress>`. Yields strongly-typed records over `IAsyncEnumerable<TRecord>` with cancellation, progress reporting (`CurrentItemCount`, `CurrentSkippedItemCount`, `CurrentLineNumber`, `CurrentBadDataCount`), `InitialRecordIndex` / `SkipRecordCount` / `MaxRecordCount` controls, opt-in `BadDataFound` / `ReadingExceptionOccurred` callbacks (library logs nothing by default — PII privacy).
- `CsvLoader<TRecord>` — async streaming CSV writer implementing `LoaderBase<TRecord, CsvLoaderProgress>`. Accepts `IAsyncEnumerable<TRecord>` with cancellation, progress reporting, `SkipRecordCount` / `MaxRecordCount` controls, opt-in `ShouldQuote` callback.
- Parser-agnostic public surface: `CsvTrimOptions`, `CsvBadDataInfo`, `CsvShouldQuoteContext`, `CsvReadingExceptionInfo`, `CsvColumnAttribute`, `CsvIgnoreAttribute`, `CsvColumnMap`, `CsvExtractorProgress`, `CsvLoaderProgress`. No CsvHelper types leak through the public API.
- Compile-time column mapping via `[CsvColumn]` / `[CsvIgnore]` attributes; runtime column mapping via `CsvColumnMap` / `ColumnMaps` property; both cached for repeated use.
- Customizable delimiter, quote, escape, comment, header presence, trim options, encoding (informational — `StreamReader.CurrentEncoding` is authoritative).
- Multi-targets `net462; netstandard2.0; netstandard2.1; net8.0; net10.0`.

### Notes

- `CsvExtractor<T>` and `CsvLoader<T>` are annotated `[RequiresUnreferencedCode]` — the library is not trim/NativeAOT safe.
- See [`docs/abi-stability.md`](docs/abi-stability.md) for the SemVer commitment, [`docs/license-audit.md`](docs/license-audit.md) for the 0.1.0 dependency snapshot, [`docs/reproducible-build-verification.md`](docs/reproducible-build-verification.md) for the build-repro recipe.
