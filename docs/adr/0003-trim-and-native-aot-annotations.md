# 0003 — Trim and Native AOT: annotate the entry points, do not claim the library is trim-safe

- **Status:** Accepted
- **Date:** 2026-11-17
- **Decision maker(s):** @Chris-Wolfgang

## Context

[ADR 0001](0001-csvhelper-as-internal-parser.md) chose CsvHelper as the internal parser. CsvHelper maps columns to a record by reflecting over the record type at run time, in ways `DynamicallyAccessedMembers` cannot fully describe (type converter constructors, non-public setters in some flows). That makes it unsafe under trimming, and under Native AOT it needs run-time code generation.

A consumer who publishes with `PublishTrimmed` or `PublishAot` should be told this at build time, at the call that causes it, and not find out when the published app fails.

The pieces were already in place when this ADR was written. There was no record of what the library does and does not claim.

## Decision

We do not claim that the library is trim-safe. We make the limitation visible and keep it from growing.

- The public constructors of `CsvExtractor<T>` and `CsvLoader<T>`, and the other entry points that reach CsvHelper's reflection, carry `[RequiresUnreferencedCode]`, and `[RequiresDynamicCode]` where run-time code generation is involved. The record type parameter is annotated with `[DynamicallyAccessedMembers(PublicProperties)]` so that the trimmer keeps the properties being mapped.
- The project sets `IsAotCompatible` and `IsTrimmable` on the target frameworks compatible with `net8.0`. Here the flags do not mean "safe". They turn on the trim, AOT and single-file analyzers, so that the annotations are checked for completeness, and they mark the assembly so that a consumer's trimmer analyzes it instead of treating it as opaque.
- `tests/Wolfgang.Etl.Csv.AotSmoke` is published with Native AOT for `linux-x64` and run by `aot-smoke.yaml` on every pull request to the default branch. A trim or AOT warning that points at this library's code fails the publish. The warnings that come from inside `CsvHelper.dll` (`IL2070`, `IL2075`, `IL3050`) are left as warnings.

## Consequences

- A consumer who trims or AOT-publishes gets a warning at each call into this library that is not safe, with a message that says why.
- The smoke test shows that a simple record round-trips in a natively compiled app. It does not show that every record shape does: a type converter or a member the trimmer removed can still fail at run time.
- A new reflection or dynamic-code use in this library that is not annotated fails the Release build.
- The annotations are part of the public surface. Removing one later is safe; adding one breaks a caller who treats the warning as an error.
- A trim- and AOT-safe path needs a class map built at compile time, which would remove the CsvHelper reflection path. That is issue #5 and is not decided here.

## Alternatives considered

- **Declare nothing** — rejected: a consumer's trimmer would treat the assembly as opaque, and the consumer would get no warning at the unsafe calls.
- **Declare the library compatible and suppress the warnings** — rejected: it would be a false claim. The reflection is real and it is in a dependency.
- **Replace CsvHelper with a parser that does not reflect** — not pursued: ADR 0001 stands, and the public surface is parser-agnostic so that this stays possible.

## Related

- [ADR 0001](0001-csvhelper-as-internal-parser.md).
- Issue #5: source generator for a compile-time class map.
- Sibling records: ETL-FixedWidth ADR 0005, ETL-Xml ADR-004, Etl-DbClient ADR-0003, ETL-SqlBulkCopy ADR 0004 and 0006.
- Recorded after the fact: the annotations, the flags and the smoke test were already in force when this ADR was written.
