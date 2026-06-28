# Transitive License Audit

Per-release sweep of `Wolfgang.Etl.Csv`'s transitive NuGet dependencies' licenses, to verify nothing in the dependency closure imposes obligations that conflict with this library's **MIT** distribution.

## Recipe

```bash
dotnet tool install -g dotnet-project-licenses
dotnet-project-licenses \
  --input src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj \
  --include-transitive \
  --use-project-assets-json
```

Run after every release. Update the table below.

## Last audited

- **Date:** 2026-06-20
- **Version:** 0.1.0
- **Tool:** `dotnet-project-licenses` 2.7.1

### Result

26 transitive dependencies, all MIT-compatible for our purposes.

| Reference | Version | License | Notes |
|---|---|---|---|
| AsyncFixer | 2.1.0 | Apache-2.0 | Analyzer — build-time only, not redistributed |
| CsvHelper | 33.1.0 | MS-PL OR Apache-2.0 | Both MIT-compatible. Internal parser, not exposed publicly. |
| Meziantou.Analyzer | 3.0.58 | MIT | Analyzer — build-time only |
| Microsoft.Bcl.AsyncInterfaces | 10.0.7 | MIT | Runtime dep |
| Microsoft.Bcl.HashCode | 1.1.1 | MIT | Runtime dep (transitive) |
| Microsoft.Build.Tasks.Git | 8.0.0 | MIT | SourceLink build helper |
| Microsoft.CodeAnalysis.BannedApiAnalyzers | 4.14.0 | MIT | Analyzer — build-time only |
| Microsoft.CodeAnalysis.PublicApiAnalyzers | 3.3.4 | MIT | Analyzer — build-time only |
| Microsoft.CSharp | 4.7.0 | MIT | Transitive |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.7 | MIT | Transitive (from Logging.Abstractions) |
| Microsoft.Extensions.Logging.Abstractions | 10.0.7 | MIT | Runtime dep |
| **Microsoft.NETCore.Platforms** | 1.1.0 | **MS-EULA** | SDK runtime helper. Standard Microsoft EULA for .NET runtime components. Covered by Microsoft's .NET SDK redistribution rights. Not a concern for downstream MIT distribution. |
| Microsoft.SourceLink.Common | 8.0.0 | MIT | SourceLink build helper |
| Microsoft.SourceLink.GitHub | 8.0.0 | MIT | SourceLink build helper |
| Microsoft.VisualStudio.Threading.Analyzers | 17.14.15 | MIT | Analyzer — build-time only |
| NETStandard.Library | 2.0.3 | (unspecified — MIT per Microsoft) | Standard .NET targeting pack |
| Roslynator.Analyzers | 4.15.0 | Apache-2.0 | Analyzer — build-time only |
| **SonarAnalyzer.CSharp** | 10.25.0.139117 | **Proprietary (free for OSS)** | Analyzer — build-time only. Sonar's standard license permits free use on open-source projects, which this repo is. |
| System.Buffers | 4.6.1 | MIT | Transitive |
| System.ComponentModel.Annotations | 5.0.0 | MIT | Transitive |
| System.Diagnostics.DiagnosticSource | 10.0.7 | MIT | Transitive |
| System.Memory | 4.6.3 | MIT | Transitive |
| System.Numerics.Vectors | 4.6.1 | MIT | Transitive |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | MIT | Transitive |
| System.Threading.Tasks.Extensions | 4.6.3 | MIT | Transitive |
| Wolfgang.Etl.Abstractions | 0.13.0 | MIT | First-party (Chris Wolfgang) |

## Conclusion

**No restrictive obligations.** The dependency closure is MIT-clean for redistribution. The two non-MIT entries (Microsoft.NETCore.Platforms / SonarAnalyzer) are either covered by the .NET SDK EULA (Microsoft component, not redistributed at runtime) or are build-time analyzers free for OSS use.

If a future dependency introduces a strong copyleft license (GPL, AGPL, LGPL) or a non-OSS commercial license, that's a release blocker — re-evaluate the dependency or remove it.
