# Third-Party Notices

`Wolfgang.Etl.Csv` ships the runtime dependencies listed below.
`license-audit.yaml` gates new dependencies against
`licenses/allowed-licenses.json` on every PR that touches a `.csproj`, plus
weekly. Regenerate this file's table by hand (from `dotnet-project-licenses`'s
console output — see the commands below) and commit it whenever the dependency
graph changes.

## Wolfgang.Etl.Csv

| Package | Version | License |
|---------|---------|---------|
| [CsvHelper](https://www.nuget.org/packages/CsvHelper/) | 33.1.0 | [MS-PL OR Apache-2.0](https://licenses.nuget.org/MS-PL%20OR%20Apache-2.0) |
| [Microsoft.Bcl.AsyncInterfaces](https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces/) | 10.0.11 | [MIT](https://licenses.nuget.org/MIT) |
| [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/) | 10.0.11 | [MIT](https://licenses.nuget.org/MIT) |

> **CsvHelper is dual-licensed.** Its package metadata declares the SPDX
> expression `MS-PL OR Apache-2.0`, meaning a consumer may take it under
> either. This package consumes it under **Apache-2.0**.
>
> The allowlist therefore contains the literal string `MS-PL OR Apache-2.0`
> rather than `Apache-2.0` alone: `dotnet-project-licenses` compares the
> declared expression as text, so an allowlist holding only the individual
> licence identifiers would fail the audit on a compound expression even
> though one of its alternatives is allowed.

> Microsoft.Bcl.AsyncInterfaces supplies `IAsyncEnumerable<T>` /
> `IAsyncDisposable` on the down-level targets; on net8.0+ those types are part
> of the framework.

## First-party dependencies

`Wolfgang.Etl.Abstractions` (MIT) is also a shipped runtime dependency, but it
is authored and published by this project's owner rather than a third party, so
it is recorded here for completeness rather than listed in the table above.

## Copyright

- CsvHelper — © Josh Close and contributors.
- Microsoft.Bcl.AsyncInterfaces, Microsoft.Extensions.Logging.Abstractions —
  © Microsoft Corporation. All rights reserved.

## Baseline scan

Generated from:

```
dotnet-project-licenses --input src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj
```

against the src project's shipped (non-analyzer, non-test) dependency graph.
Analyzer packages are `PrivateAssets=all` build-time-only and are never
distributed in the NuGet package, so they are deliberately out of scope.
