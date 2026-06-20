# Migration Guides

Per-major-version upgrade guides for Wolfgang.Etl.Csv consumers.

## Status

No migration guides yet — Wolfgang.Etl.Csv is at **0.1.0**, the first public release.

The first migration guide will appear here when a 1.0 or 2.0 ships with breaking changes. Until then, this directory exists as scaffolding so the structure is in place before it's needed.

## Format

Each guide is a single markdown file named `<from>-to-<to>.md` (e.g. `0.x-to-1.0.md`, `1.x-to-2.0.md`).

A guide should include:

1. **Summary** — one paragraph describing the scope of the change
2. **Breaking changes** — table of removed/renamed/repurposed public API with before/after snippets
3. **Behavioural changes** — semantics that changed even where the API didn't
4. **Recommended migration order** — for guides covering multiple breaking changes, suggest a sequence consumers can follow incrementally
5. **Automated rewrites** — code-fix analyzer rules, `dotnet format` patterns, or regex substitutions where applicable
6. **Deprecations** — what to remove next, on what schedule

## Cross-references

When a release ships breaking changes, link the migration guide from:

- The release notes on the [GitHub Releases page](https://github.com/Chris-Wolfgang/ETL-Csv/releases)
- `CHANGELOG.md` under the relevant `[X.Y.Z]` section
- The NuGet package description (or the PackageReleaseNotes property in the csproj)
- The repo README's "Installation" section if the consumer-side change is more than a version bump
