# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.6.0] - 2026-08-13

### Changed

- Adopted **ETL core 0.22.0** — `Wolfgang.Etl.Abstractions` 0.21.0 -> 0.22.0, along with the
  test-only `Wolfgang.Etl.TestKit` / `Wolfgang.Etl.TestKit.Xunit` references. 0.22.0 is the release in
  which the TestKit packages were folded into the ETL-Abstractions repository and now build and ship
  from there. The public API of all four core packages is unchanged.
- Inherited from Abstractions 0.22.0: the `await foreach` sites in `ExtractorBase` and
  `TransformerBase` now use `ConfigureAwait(false)`, removing a sync-over-async deadlock risk for
  consumers on the `net462` and `netstandard2.0` targets that drive the pipeline from a
  synchronization context. No behavioural change on the modern targets.

## [0.5.0] - 2026-08-11

### Added

- Polymorphic extraction — a `CsvExtractor<TRecord>` can now bind each row to a different concrete
  type chosen by a discriminator column, so a single file can mix record shapes (#12). Set
  `Discriminator` to a `CsvDiscriminator<TBase>` built with `CsvDiscriminatorBuilder<TBase>` — a
  fluent, trim/AOT-safe builder that names each concrete type generically and optionally applies
  per-type `CsvColumnMap`s. The discriminator is read by column index or header name; an unmapped
  value is handled per `CsvDiscriminatorAction` (`Throw`, `Skip`, or `YieldAsBase`). While a
  discriminator is set, missing trailing fields are tolerated so narrower row shapes bind cleanly.
- Polymorphic loading — `CsvLoader<TRecord>` now accepts the same `Discriminator` and writes each
  record using the per-type mapping chosen by its runtime type, so a mixed file round-trips through
  the extractor (#13). No header row is written while a discriminator is set (the shapes share no
  common header); a record whose runtime type is unmapped is handled per `CsvDiscriminatorAction`.
- Streaming per-record validation for `CsvExtractor<TRecord>` (#8). Set `Validators` to a list of
  `CsvValidator<TRecord>` rules run after each row binds; a failing record is counted
  (`CsvExtractorProgress.CurrentInvalidItemCount`), passed to `InvalidRecordHandler`, then handled per
  `OnValidationFailure` — `Continue` (yield anyway), `Skip` (drop it), or `Stop` (raise
  `CsvValidationException`, the default). Built-in validators via the `CsvValidator` factory:
  `NotNullOrEmpty`, `GreaterThan`, `InRange`, `MaxLength`, `Matches`, and `Custom`; multiple rules'
  failures aggregate into a single `CsvInvalidRecord<TRecord>`. Async validators and unifying
  type-conversion errors into the same stream are tracked as follow-ups.
- The same validation trio (`Validators`, `OnValidationFailure`, `InvalidRecordHandler`) and
  `CsvLoaderProgress.CurrentInvalidItemCount` on `CsvLoader<TRecord>`, validating each record before
  it is written (#8).
- Runnable examples and cookbook docs for the new features: `examples/…PolymorphicRows` and
  `…RecordValidation`, with matching guides under `docs/cookbook/` (`polymorphic-rows.md`,
  `record-validation.md`) linked from the README.

## [0.4.0] - 2026-08-09

### Added

- `CsvLoader<TRecord>` now implements `ISupportDryRun` (#127). Set `IsDryRun = true` to run the
  full load pipeline against real data — enumerate the source, honor `SkipRecordCount` /
  `MaxRecordCount`, increment progress counters, fire progress reports, and log — but write
  **nothing** to the output (neither the header nor any records). Use it to validate a pipeline
  without producing output. Defaults to `false`.
- `CsvCheckpointExtensions` — atomic-write sugar for the resumable-extraction pattern (#11):
  `ReadCheckpointAsync` (returns `0` for a missing file; throws `FormatException` on non-integer
  content — corruption is loud), `WriteCheckpointAsync` (atomic: writes `path + ".tmp"` then
  renames over the target), and `extractor.ResumeFromCheckpointAsync(path)` (sets
  `SkipRecordCount` from the checkpoint and returns the count). Thin sugar over `SkipRecordCount`
  — checkpoint policy (when/where to acknowledge) stays the caller's. Runnable example under
  `examples/Wolfgang.Etl.Csv.Examples.ResumableExtraction/`.

## [0.3.0] - 2026-08-07

### Added

- CSV pipeline extensions for the generic `EtlPipeline` chain (#14):
  - `EtlPipeline.Create().CsvExtractor<T>(...)` source factories over a file path, a
    caller-supplied `StreamReader`, or a pre-built `CsvExtractor<T>`, returning
    `ICsvExtractorBuilder<T>` for inline fluent configuration.
  - `pipeline.CsvLoader<T>(...)` sink terminators over a file path, a caller-supplied
    `StreamWriter`, or a pre-built `CsvLoader<T>`, returning `ICsvLoaderBuilder<T>`.
  - Fluent setters on both builders map 1:1 to the underlying `CsvExtractor<T>` /
    `CsvLoader<T>` properties (delimiter, quote, escape, encoding, header handling,
    record windowing, trim options, column maps, bad-data / reading-exception callbacks,
    `ShouldQuote`, `NewLine`). The first pipeline operator narrows the builder to
    `IEtlPipeline<T>`, dropping the configuration surface — no explicit `Build()` step.
  - Path-based factories own the file stream they open and dispose it after the run
    (success and failure); caller-supplied streams and pre-built extractors/loaders are
    left to the caller. `.Encoding(...)` binds the actual stream encoding.

### Changed

- Bumped the `Wolfgang.Etl.Abstractions` dependency to **0.21.0** and the test-only
  `Wolfgang.Etl.TestKit` / `Wolfgang.Etl.TestKit.Xunit` packages to **0.14.0**. Adopts
  the expanded `LoaderBase` / `ExtractorBase` contract-test suites (cancellation,
  disposal, error-handling, allocation-budget) — removed the now-redundant hand-written
  `CsvLoader` null-`items` guard test that the contract base now covers.
- `CsvExtractor` row-level failures (parse and type-conversion) now route through the unified
  `ErrorPolicy` (Abstractions 0.21). Assign an `ErrorPolicy` that returns `ItemErrorAction.Skip`
  (e.g. `ErrorPolicy = _ => ItemErrorAction.Skip`) to skip a failed row and continue
  (`CurrentErrorItemCount` / `CsvExtractorProgress.CurrentErrorItemCount` track skipped failures);
  the default is fail-fast (the first failure aborts the run, as before).

### Deprecated

- `CsvExtractor.ReadingExceptionOccurred` and `CsvExtractor.BadDataFound` — use `ErrorPolicy`
  for the skip-vs-abort decision. Both callbacks still fire for observation; parse/type
  failures are now governed by `ErrorPolicy` and bad data remains tolerated
  (`CurrentBadDataCount`).

### Fixed

- `CsvLoader.LoadAsync` now honors a token that is **already cancelled before the first
  record is read** — it reads nothing and throws `OperationCanceledException` immediately,
  matching `CsvExtractor` and the `LoaderBase` cancellation contract (TestKit 0.14).
  Previously it read (and wrote) one record before observing cancellation.


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
