# ABI Stability and Breaking-Change Detection

How Wolfgang.Etl.Csv tracks public API changes between releases.

## Policy

- **PATCH / MINOR releases (0.1.x, 0.x.0)** — must **not** break ABI. Adding new public members is fine; removing or changing existing public members is not.
- **MAJOR releases (1.0.0, 2.0.0, ...)** — may break ABI, but every break must be:
  - Listed in `CHANGELOG.md` under the `### Removed` or `### Changed` section
  - Covered by a migration guide in [`docs/migration/`](migration/) with before/after snippets
  - Surfaced in the release notes

## Detection

Run [`scripts/check-api-compat.ps1`](../scripts/check-api-compat.ps1) before publishing a release. It downloads the latest stable `Wolfgang.Etl.Csv` from NuGet.org, builds the local source, then runs [`Microsoft.DotNet.ApiCompat.Tool`](https://www.nuget.org/packages/Microsoft.DotNet.ApiCompat.Tool/) to diff the public API.

```powershell
# Diff against latest stable on NuGet.org
pwsh ./scripts/check-api-compat.ps1

# Or pin to a specific baseline
pwsh ./scripts/check-api-compat.ps1 -BaselineVersion 0.1.0
```

Exit code 0 = no breaks. Non-zero = ApiCompat surfaced at least one break; review the report inline.

## Why not on every PR?

Running on every PR would catch every accidental change but generate noise on PRs that deliberately add API surface (which is most non-trivial PRs while a library is below 1.0). The pre-release run is the right point because it answers the specific question: *"am I about to ship a breaking change in a release that shouldn't?"*

Once the library reaches a 1.x stability promise, **consider moving this check to `release.yaml`** as a release gate.

## What ApiCompat catches

- Removed types or members
- Changed method signatures (parameter types, return types, generic constraints)
- Visibility narrowing (e.g. `public` → `internal`)
- Removed interface implementations
- Members renamed without retaining the old name as `[Obsolete]`

## What ApiCompat does **not** catch

- Behaviour changes (same signature, different semantics — e.g. a method that used to return `null` now throws)
- Performance regressions
- Changes to internal types or private members (intentional — these aren't part of the public contract)
- XML-doc-only changes (the example in `<example>` is checked by [`CsvXmlDocExampleRotTests`](../tests/Wolfgang.Etl.Csv.Tests.Unit/CsvXmlDocExampleRotTests.cs); semantics changes are not detectable from metadata alone)

For behaviour changes, the test suite is the primary guard. For performance regressions, the benchmark chart at https://chris-wolfgang.github.io/ETL-Csv/dev/bench/ is the trend signal.

## PublicAPI baseline files

`src/Wolfgang.Etl.Csv/PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` are a complementary mechanism enforced at build time by [`Microsoft.CodeAnalysis.PublicApiAnalyzers`](https://www.nuget.org/packages/Microsoft.CodeAnalysis.PublicApiAnalyzers/) — when a new public symbol appears without a corresponding entry in either file, the build emits `RS0016`. This catches accidental API additions at the PR layer; ApiCompat catches accidental removals at the release layer.

The two are complementary: PublicApiAnalyzers prevents the surface from growing without an explicit author decision, ApiCompat prevents the surface from shrinking without an explicit author decision.

## Versioning

Wolfgang.Etl.Csv follows [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html):

- `<MAJOR>.<MINOR>.<PATCH>` for releases
- Pre-release identifiers (`-alpha`, `-beta`, `-rc`) reserved for unstable previews
- `0.x.x` — pre-1.0 development. MINOR may break ABI in this range (this is permitted by SemVer for the 0.x range, but the project chooses to honour the no-break-on-MINOR policy anyway to avoid surprising consumers).

The current shipped version is reported in `src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj` `<Version>` and surfaced on NuGet.org.
