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
