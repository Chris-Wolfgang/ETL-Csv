# 0007 — CsvHelper 34, with record binding owned by the library

- **Status:** Accepted
- **Date:** 2026-11-26
- **Decision maker(s):** @Chris-Wolfgang (adopt CsvHelper 34 now, #514, 2026-11-20)

## Context

CsvHelper 34 is a rewrite. It is SIMD-accelerated and allocation-free, and it has no `CsvHelper.Configuration` namespace: no `ClassMap`, `CsvConfiguration`, `CsvContext` or type converters. It maps a record type to columns in only two ways. One is a mapper that its source generator writes for a type marked `[CsvRecord]`. The other is a hand-written `CsvRecordMapper<T>`.

ETL-Csv takes its consumers' plain record types as a generic argument. It maps them by convention, by `[CsvColumn]` / `[CsvIgnore]`, by a runtime `CsvColumnMap` list, or per type through a `CsvDiscriminator`. None of these can be expressed as a generated mapper, because the library does not own the types. On CsvHelper 33 the library built a `ClassMap` per type by reflection (`CsvClassMapFactory`).

ADR [0001](0001-csvhelper-as-internal-parser.md) kept CsvHelper types out of the public surface. That still holds, so the migration changes no public signature.

## Decision

We will read and write CSV with CsvHelper 34's field-level API (`CsvReader.GetString`, `CsvWriter.WriteField`), and the library will bind records itself:

- `CsvRecordBinding` is the column layout of a type, built once by reflection and cached. It comes from the type's public properties and attributes, or from a `CsvColumnMap` list. It replaces the `ClassMap`.
- `CsvBindingReader` resolves the columns against the header once per run and builds each record. It sets public setters, or calls the constructor with the most parameters for a type without a parameterless one. `CsvBindingWriter` writes the header and each record's fields.
- `CsvValueConverter` converts field text with the rules CsvHelper 33's default converters used: the current culture, an empty field as `null` for nullable and reference types, `Format` as the exact parse and write format, enums by name ignoring case, and booleans as `true`/`false`/`1`/`0`.

The behaviour the test suite pinned on CsvHelper 33 is kept, including column order, line numbers that count a quoted line break, the bad-data column, the header and missing-field checks, and quoting of padded fields. The existing tests pass. The ones changed are those of removed internals (`CsvClassMapFactory`, the `TrimOptions` mapping, the captured discriminator writers), the expected exception types, the backslash-escape test and the allocation measurement, which now re-primes CsvHelper's pooled buffers after forcing a GC.

## Alternatives considered

- **Hold on CsvHelper 33.x with a semver-major ignore.** Rejected by the owner (#514). It leaves the library on a line that gets no more features.
- **Generate a `CsvRecordMapper<T>` per consumer type.** That is the source-generator work in #5. It needs consumers to opt in at compile time, and it does not cover runtime `CsvColumnMap` lists or the direct-init discriminator. It stays a possible later addition and is not a replacement for runtime binding.
- **Register a hand-written `CsvRecordMapper<T>` per type, built by reflection.** CsvHelper's mapper registry is static and per type. A runtime `ColumnMaps` list or a discriminator's per-type maps would then change global state shared by every extractor of that type. Owning the binding keeps each extractor's layout its own.

## Consequences

- **Breaking, for consumers:**
  - Failures that were CsvHelper 33 exceptions are now `CsvHelper.CsvFormatException`. That covers type conversion (`TypeConverterException`), a header without a required column (`HeaderValidationException`), a missing field (`MissingFieldException`) and the `CsvHelperException` base.
  - A property getter that throws while a record is written now surfaces as the getter's own exception, not wrapped in `WriterException`.
- **Breaking, for options:**
  - `Delimiter`, `Quote` and `Comment` must each be a single ASCII character.
  - `NewLine` must be `"\r\n"`, `"\n"` or `"\r"`.
  - `Escape` must equal `Quote` or stay at its default `'"'`. CsvHelper 34 reads and writes RFC 4180 quoting, where a quote is escaped by doubling it, so a backslash escape is refused at construction.
- **Changed detail:** `CsvBadDataInfo.Field` is now the malformed field found by re-scanning the record. CsvHelper 34 reports a malformed record, not the field.
- **Header matching** now follows CsvHelper 34's `TryGetFieldIndex`. An exact match wins, and otherwise a name matches ignoring case and surrounding whitespace. CsvHelper 33 matched exactly.
- **Target frameworks:** CsvHelper 34 ships net10.0, net8.0, net47, netstandard2.1 and netstandard2.0. The library's net462 build uses CsvHelper's netstandard2.0 assembly.
- **Name clash:** CsvHelper 34 has its own `CsvColumnAttribute`, `CsvIgnoreAttribute` and `CsvValidationException`. A consumer file with both `using CsvHelper;` and `using Wolfgang.Etl.Csv;` must qualify those names.
- **Trimming and AOT:** CsvHelper's own reflection is gone, so the reflection left is the library's binding. It reads the record type's public properties and constructors. The entry points keep `[RequiresUnreferencedCode]` (ADR [0003](0003-trim-and-native-aot-annotations.md)).
- **Maintenance:** the library now owns a small type-conversion table. A member type it has no rule for falls back to the type's `TypeConverter`.
