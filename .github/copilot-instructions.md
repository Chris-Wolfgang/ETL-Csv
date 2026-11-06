# Copilot Instructions — Wolfgang.Etl.Csv

## What this repository is

`Wolfgang.Etl.Csv` is a NuGet library: a `CsvExtractor<TRecord>` and `CsvLoader<TRecord>` for streaming CSV into and
out of strongly-typed records. They derive from `ExtractorBase<,>` / `LoaderBase<,>` in
[Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions), and CsvHelper is used internally
without exposing its types on the public surface.

## Layout

- `src/Wolfgang.Etl.Csv/` — the library (multi-targeted: net462, netstandard2.0/2.1, net5.0–net8.0, net10.0)
- `tests/Wolfgang.Etl.Csv.Tests.Unit/` — xUnit tests, built on the `Wolfgang.Etl.TestKit.Xunit` contract base classes
- `tests/Wolfgang.Etl.Csv.AotSmoke/` — NativeAOT boundary check (Linux only, not in the solution)
- `tests/Wolfgang.Etl.Csv.Tests.SourceLink/` — symbol-package checks
- `benchmarks/`, `examples/` — BenchmarkDotNet suite and runnable examples
- `docs/` — ADRs, cookbook, workflow notes; `docfx_project/` — the DocFX site
- `changelog/unreleased/` — one changelog fragment per PR that changes `src/` (see its README)

## Conventions to follow in suggestions

- Configuration goes through `CsvExtractorOptions<TRecord>` / `CsvLoaderOptions<TRecord>` passed to the constructor.
  The settable properties on the stages are `[Obsolete]`; do not suggest them.
- Row failures are handled by `ErrorPolicy`. `BadDataFound` / `ReadingExceptionOccurred` are deprecated and only observe.
- All I/O is async. Synchronous APIs are banned by `BannedSymbols.txt`, and library code uses `ConfigureAwait(false)`.
- Allman braces, file-scoped namespaces, three blank lines between members, and multi-line argument lists with the
  opening parenthesis on its own line.
- Public API changes must be recorded in `PublicAPI.Unshipped.txt`. RS0016 / RS0037 are errors for `src/` in Release.
- Test names follow `MethodUnderTest_when_condition_expected_result`. Test assemblies are held to 100% line coverage,
  production code to 95%.
- Warnings are errors. Fix them rather than suppressing them.

## CI

`pr.yaml` runs on `pull_request`: Linux, then Windows, then macOS test stages, each with a coverage gate, plus
InspectCode, DevSkim and the other checks. `protected-files.yaml` fails any PR that mixes a protected file (workflows,
`.editorconfig`, `Directory.Build.*`, `BannedSymbols.txt`, `*.globalconfig`, `*.DotSettings`, `coverlet.runsettings`,
the CI scripts) with other changes. See `docs/WORKFLOW_SECURITY.md`.

To reproduce the PR gates locally, run `pwsh ./scripts/build-pr.ps1`.
