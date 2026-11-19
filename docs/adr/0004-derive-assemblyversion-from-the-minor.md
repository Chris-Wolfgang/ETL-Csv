# 0004 — Derive AssemblyVersion as 0.{Minor}.0.0

- **Status:** Accepted
- **Date:** 2026-11-17
- **Decision maker(s):** @Chris-Wolfgang

## Context

`Wolfgang.Etl.Csv` targets `net462`, so it runs on the .NET Framework CLR, which binds a reference by its full four-part `AssemblyVersion`. .NET 5 and later ignore the value for binding.

There are two ways to get this wrong. If `AssemblyVersion` follows the package version, every patch release changes the binding identity and every .NET Framework consumer needs a binding redirect for no behavioural reason. If it is frozen, a breaking `0.x` minor release is invisible at bind time: a consumer who swaps in the new package without recompiling loads it and fails at run time.

The ETL family has a policy for this (recorded in ETL-Abstractions as "`<AssemblyVersion>` policy for the ETL family"): a library that targets .NET Framework and is not already pinned at `1.0.0.0` derives `0.{Minor}.0.0` during `0.x`. The project file here already does that and explains it in a comment; there was no ADR.

## Decision

`<AssemblyVersion>` is derived from `<Version>` in `src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj`:

```xml
<AssemblyVersion>0.$([System.Version]::Parse($(Version.Split('-')[0])).Minor).0.0</AssemblyVersion>
```

It changes on every minor release, which is where SemVer puts a breaking change before 1.0, and stays fixed across patch releases. `<FileVersion>` carries the full release version.

## Consequences

- A patch release keeps the binding identity, so it needs no binding redirect.
- A minor release changes it. A .NET Framework consumer who upgrades across a minor without recompiling gets a load-time version mismatch that names the problem, and needs a redirect or a rebuild.
- A release cannot forget the change, because the value is computed.
- The formula yields `0.{Minor}.0.0` only while the major version is 0. It has to be replaced at 1.0 (the family policy says `{Major}.0.0.0`), and that change should be recorded as a new ADR.
- `1.0.0.0` is not used during `0.x`, so the real `1.0.0` release brings a new binding identity.

## Alternatives considered

- **Pin at `1.0.0.0`** — rejected: no bind-time signal for a breaking `0.x` minor, and the `1.0.0.0` identity is spent before 1.0. Several older libraries in the family ship this way and stay that way, because moving a pinned library down would be a downgrade.
- **Let the SDK derive `AssemblyVersion` from `<Version>`** — rejected: a new binding identity on every patch release.
- **A hand-maintained `0.{Minor}.0.0`** — rejected: it depends on every release preparation remembering one more edit.

## Related

- Sibling records of the same rule: ETL-SqlBulkCopy ADR 0007, ETL-Transformers ADR-0004.
- Recorded after the fact: the derivation was already in force when this ADR was written.
