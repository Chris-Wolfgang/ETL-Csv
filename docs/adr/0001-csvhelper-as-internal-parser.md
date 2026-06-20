# 0001 — CsvHelper as the internal parser, parser-agnostic public surface

- **Status:** Accepted
- **Date:** 2026-06-20
- **Decision maker(s):** @Chris-Wolfgang

## Context

Wolfgang.Etl.Csv 0.1.0 needs a CSV parser. The realistic options in the .NET ecosystem are:

1. **[CsvHelper](https://joshclose.github.io/CsvHelper/)** — mature (~12 years), 280M+ NuGet downloads, MS-PL / Apache 2.0 dual-license, broad RFC 4180 + non-standard CSV quirks coverage, type converters and mapping fluently composable, allocates more than strictly necessary for high-throughput scenarios.
2. **[Sep](https://github.com/nietras/Sep)** — modern (2023), Apache 2.0, allocation-free hot path, vectorized parsing, ~10× CsvHelper's throughput in benchmarks. Smaller surface than CsvHelper: limited bespoke-CSV-quirk coverage, less mature type-converter ecosystem.
3. **Roll our own** — full control over allocations and API surface but a perpetual maintenance burden for edge cases (escapes, quote handling, encoding, BOM, comments, multi-line fields).

The library has two constraints that shape the choice:

- **Drop-in compatible with Wolfgang.Etl.Abstractions's `ExtractorBase<T, TProgress>` / `LoaderBase<T, TProgress>` contracts.** The parser's record-iteration model needs to map cleanly onto the async-yield + progress + cancellation pattern those contracts define.
- **Should not lock consumers into the parser's API surface.** Consumers wire `CsvExtractor<T>` into their pipelines and shouldn't have to absorb a parser swap if we later need one.

## Decision

We will use **CsvHelper** as the internal parser for 0.1.0, behind a public surface that does **not leak any CsvHelper types**.

The wrapper types — `CsvTrimOptions`, `CsvBadDataInfo`, `CsvShouldQuoteContext`, `CsvReadingExceptionInfo`, `CsvColumnAttribute`, `CsvIgnoreAttribute`, `CsvColumnMap` — translate consumer intent to CsvHelper's configuration model at extraction/load time. CsvHelper itself is referenced as an internal package dependency; nothing in `public class CsvExtractor<T>` or `public class CsvLoader<T>` exposes a CsvHelper type, enum, or interface.

## Consequences

**Positive:**
- Day-one production-grade RFC 4180 + non-standard CSV coverage. Doesn't need its own parser test suite at the byte level.
- Type-converter ecosystem comes for free (DateTime formats, enums, custom converters per property).
- Mapping fluency (`ClassMap<T>`, attribute-driven, runtime-driven) is reusable inside our `CsvClassMapFactory` without inventing a parallel abstraction.
- License (MS-PL / Apache 2.0) is MIT-compatible — no consumer-side license fragmentation.
- Future parser swap (e.g. to Sep, for an allocation-free 1.0) is internal-only: zero consumer breakage.

**Negative:**
- Allocations per record are higher than a hand-tuned or Sep-based pipeline. Not appropriate as-is for hot-path streaming of millions of rows per second — consumers in that regime will need to roll their own or wait for a Sep-backed variant.
- CsvHelper is not trim/AOT-safe (reflects beyond `DynamicallyAccessedMembers(PublicProperties)` for type converters). The library inherits this constraint and surfaces it honestly via `[RequiresUnreferencedCode]` on public ctors and a "Trim / NativeAOT" subsection in the README (see [#105](https://github.com/Chris-Wolfgang/ETL-Csv/issues/105)).
- Wrapper types add a thin maintenance burden — when CsvHelper adds a new configuration knob worth exposing, the wrapper needs a corresponding addition.

**Neutral:**
- The wrapper-translation pattern adds a layer of indirection that's barely measurable in microbenchmarks but real in profile traces. For 0.1.0's use cases (typical ETL throughput in the hundreds of thousands of rows/sec), this is well within the noise.

## Alternatives considered

- **Sep** — rejected for 0.1.0 because its type-converter and configuration ecosystem is less mature than CsvHelper's, and the library needs to support attribute-driven mapping with custom formats / defaults / optional columns on day one. Reconsidering for a future 1.0 / 2.0 allocation-free variant is the entire point of keeping the public surface parser-agnostic.
- **Roll our own** — rejected because building a production-grade RFC 4180 parser plus type-converter system plus configurable encoding/BOM/comment handling is a multi-month effort that adds zero differentiated value over CsvHelper. The differentiation is in the ETL contract (progress, async-yield, cancellation, mapping factory), not in the parser.

## Related

- [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions) — the `ExtractorBase` / `LoaderBase` contracts this library implements.
- [CsvHelper docs](https://joshclose.github.io/CsvHelper/)
- [#105](https://github.com/Chris-Wolfgang/ETL-Csv/issues/105) — trim/AOT honesty contract.
