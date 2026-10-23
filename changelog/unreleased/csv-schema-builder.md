type: feature

Add `CsvSchemaBuilder<T>` and `CsvSchema` — a fluent, type-safe, code-first column mapping for record types you cannot decorate with `[CsvColumn]`, applied through the new `Schema` option on `CsvExtractorOptions<TRecord>` / `CsvLoaderOptions<TRecord>` (mutually exclusive with `ColumnMaps`); `CsvSchema.For<T>()` reifies a type's attribute-resolved mapping for inspection.
