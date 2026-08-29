# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **Third-party notices and a license-audit gate (#253).** `THIRD-PARTY-NOTICES.md` records every
  shipped runtime dependency with its version and license, and is packed into the NuGet output
  alongside `README.md`. A new `license-audit.yaml` workflow runs `dotnet-project-licenses` against
  the `src/` dependency graph on every PR touching a `.csproj`, plus weekly, gating against
  `licenses/allowed-licenses.json`.

  The allowlist carries the literal SPDX expression `MS-PL OR Apache-2.0` alongside the individual
  identifiers. CsvHelper is dual-licensed and declares that compound expression, which
  `dotnet-project-licenses` compares as text — an allowlist holding only `Apache-2.0` would fail the
  audit on it despite one of its alternatives being allowed. This package consumes CsvHelper under
  Apache-2.0.

  Analyzer packages are out of scope: they are `PrivateAssets=all` and never distributed.

### Changed

- **`logger` is now an optional trailing constructor parameter on `CsvExtractor<T>` and
  `CsvLoader<T>`.** `ILogger<T> logger` became `ILogger<T>? logger = null`, and passing `null` (or
  omitting it) now resolves to `NullLogger.Instance` instead of throwing `ArgumentNullException`.
  This aligns both types with the fleet-wide constructor convention — logger always last, always
  optional — already followed by `Etl-DbClient`.

  Not a breaking change: the parameter list is unchanged, so the emitted signature is identical and
  PackageValidation against the published baseline passes. Only the nullability annotation and the
  default were added. Existing calls that pass a logger continue to bind exactly as before.

  The single-argument `CsvExtractor(StreamReader)` / `CsvLoader(StreamWriter)` constructors are now
  redundant but are **deliberately retained** — deleting them would be a binary breaking change for
  already-compiled consumers, because optional-argument defaults are baked in at the caller's
  compile time. They are scheduled for `[Obsolete]` in the next minor and removal in the one after.

- **`CsvExtractor<T>` and `CsvLoader<T>` now have a single initialization path.** All three
  constructors previously assigned `_reader`/`_writer` and `_logger` independently — three copies of
  the same setup, each free to drift from the others. They now chain into one private constructor
  that assigns the shared fields in exactly one place.

  No API or behavior change: the signatures, the `ArgumentNullException` for a null reader/writer,
  and the order in which arguments are validated are all unchanged.

  This is a defect-prevention change. The identical triplicated-assignment shape produced two
  shipped bugs elsewhere in the fleet — a `LeaveOpen` flag one ETL-Xml constructor set and another
  didn't, and an ETL-FixedWidth internal constructor that hard-coded UTF-8 while its public
  counterpart honored the caller's encoding. In both cases the constructor that was easiest to
  overlook was the one that got it wrong.

## [0.7.1] - 2026-08-23

Analyzer-noise cleanup release. Zero consumer-visible API or behavior changes —
drop-in replacement for 0.7.0.

### Changed

- `CsvClassMapFactory` internal per-column configuration guards rewritten from
  `!string.IsNullOrEmpty(x) + x!` to `x is not null && x.Length > 0`, eliminating
  the null-forgiving operators without any suppression. Same semantic; behavior
  unchanged. Retires 4 SonarAnalyzer S8969 findings at the source. (#241)
- `CsvValidationResult` legacy `[Obsolete]` positional constructor now validates
  its inputs inline (the `ValidateFailures` helper was the only call site and was
  inlined). Same throw behavior; no consumer-visible change. (#240)
- Six record-synthesized members already present on the shipped assembly are now
  declared in `PublicAPI.Shipped.txt` — `operator ==` for `CsvBadDataInfo`,
  `CsvColumnMap`, `CsvReadingExceptionInfo`, `CsvShouldQuoteContext`; and
  `PrintMembers(StringBuilder)` for `CsvExtractorProgress`, `CsvLoaderProgress`.
  Documentation catch-up only — these members were callable in 0.7.0. (#239)

### Internal

- `.zizmor.yml` moved to `.github/zizmor.yml` (zizmor 1.5.2 auto-discovers only
  the `.github/` location) and rewritten to the schema version 1.5.2 requires.
  The existing `dangerous-triggers: ignore: [pr.yaml]` documented-suppression
  rule now actually takes effect. Retires 1 zizmor alert. (#242)
- `.github/workflows/scorecard.yaml` SARIF filter step extended to strip 9
  additional accepted-as-intentional Scorecard findings: `PinnedDependenciesID`
  for `dotnet`/`pip` invocations (no first-class hash-pin verify for either),
  and the four solo-maintainer structural rules `BranchProtectionID`,
  `CIIBestPracticesID`, `CodeReviewID`, `FuzzingID`. Full rationale documented
  inline. Existing `DangerousWorkflowID` filter for pr.yaml unchanged. (#243)
- `Validation/.editorconfig` gains three narrow file-scoped suppressions on
  `CsvValidationResult.cs` — S1133 on the `[Obsolete]` ctor and
  ParameterHidesMember on the explicit `Deconstruct(out bool IsValid, ...)` out
  params — both required to preserve the shipped API surface. Rationale
  documented alongside each entry. (#240)

Part of umbrella #201.

## [0.7.0] - 2026-08-22

### Added

- **`CsvValidationResult` two-constructor API**: two new explicit constructors
  that make illegal states (successful-with-failures, failed-without-failures,
  null failures) unrepresentable at the call site:
  - `new CsvValidationResult()` — successful result, no failures
  - `new CsvValidationResult(IReadOnlyList<string> failures)` — failed result;
    throws `ArgumentNullException` on null or `ArgumentException` on empty

  Recommended over the legacy positional constructor. `Pass` and `Fail(params)`
  are unchanged and still the shortest paths.

### Deprecated

- **`CsvValidationResult.CsvValidationResult(bool IsValid, IReadOnlyList<string> Failures)`**
  (the record's positional primary constructor) is marked `[Obsolete]`. It
  still works — and now validates its inputs, throwing on inconsistent state
  (previously any combination was silently accepted, including the `null!`
  bypass) — but the two named constructors above are the recommended path.
  Will be removed in a future major version. `Deconstruct` is not obsoleted;
  it's the standard record-synthesized shape and stays available.

  **Migration**:
  - `new CsvValidationResult(true, someList)` → `CsvValidationResult.Pass` or `new CsvValidationResult()`
  - `new CsvValidationResult(false, failuresList)` → `new CsvValidationResult(failuresList)`
  - `var (isValid, failures) = result` → `result.IsValid` and `result.Failures`
  - `CsvValidationResult.Fail("reason")` — unchanged

  No public API is removed in this release — consumers of the record's
  value-equality surface (`Equals`, `==`, `GetHashCode`, `IEquatable<T>`,
  `ToString`), `with` expressions, `Deconstruct`, and the legacy positional
  constructor all continue to work; the two deprecations only surface
  compile-time warnings pointing at the new constructors.

### Changed

- Retired the remaining ~26 Code Scanning alerts left after the 0.6.1 `#201`
  sweep. Two alerts became real code fixes (deleted dead `count` in
  `MemoryDeltaBenchmarks.cs`; moved internal `CsvClassMapFactory.cs` to root
  so folder matches the flat `Wolfgang.Etl.Csv` namespace). Remaining
  suppressions on the `Properties/*.cs` polyfills are now per-line
  `// ReSharper disable` comments at each site with rationale, replacing the
  earlier folder-scoped `.editorconfig`. Nothing hidden in an `.editorconfig`
  block.
- `CsvExtractor.TryValidate` drops the defensive `if (result.Failures is not null)`
  wrapper — the ctor guards make the check unreachable. This retires the last
  `ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract` suppression from
  Group A.

## [0.6.1] - 2026-08-19

### Security

- Adopted `Wolfgang.Etl.Abstractions` / `Wolfgang.Etl.TestKit` / `Wolfgang.Etl.TestKit.Xunit`
  **0.23.2**, which carries the ETL-Abstractions fleet security fixes from ETL-Abstractions#361.
  No behavioural or API change in ETL-Csv itself.
- Retired ~700 Code Scanning alerts across `InspectCode` / `zizmor` / `Scorecard` via the
  `#201` umbrella:
  - `PublicApiAnalyzer` is now gated on `Exists('PublicAPI.*.txt')` at `Directory.Build.props`
    so `RS0016 / RS0017 / RS0037` no longer flood `tests/` / `benchmarks/` / `examples/`.
  - Every GitHub Actions `uses:` is SHA-pinned with a `# vMAJOR.MINOR.PATCH` note (retires
    all `unpinned-uses` / `PinnedDependenciesID` findings).
  - `codeql.yaml` reads step outputs from an `env:` block instead of interpolating into the
    shell body (retires the `template-injection` findings).
  - `semgrep.yaml` / `pr-benchmarks.yaml` / `scorecard.yaml` narrow `security-events: write`,
    `pull-requests: write`, and `read-all` from workflow-level to job-level scope.
  - Repo-root `.zizmor.yml` documents the one accepted `pull_request_target` finding on
    `pr.yaml` with rationale.
- **New**: pin `<AssemblyVersion>` to `0.6.0.0` for the whole 0.6.x line and derive
  `<FileVersion>` from `<Version>` — so .NET Framework consumers no longer need a binding
  redirect between PATCH bumps. Before this change, `AssemblyVersion` was SDK-derived from
  `<Version>` and shifted on every release. Consumers who added a binding redirect after
  upgrading from earlier 0.x versions can leave it in place; new consumers on 0.6.1+ will
  not need one.

### Changed

- Adopted `Microsoft.Bcl.AsyncInterfaces` / `Microsoft.Extensions.Logging.Abstractions` 10.0.11.
- Bumped test-side deps: `Microsoft.NET.Test.Sdk` 18.9.0 (modern-TFM branch), `Microsoft.CodeAnalysis.CSharp` 5.9.0, `CsCheck` 4.8.0, `Roslynator.Analyzers` 4.16.1, `Meziantou.Analyzer` 3.0.163, `SonarAnalyzer.CSharp` 10.32.0.713, `Microsoft.SourceLink.GitHub` 10.0.400.

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
