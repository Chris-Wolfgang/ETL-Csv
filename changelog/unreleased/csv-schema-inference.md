type: feature

Add `CsvInferredSchema.InferAsync`, which samples a CSV and returns its column names, best-fit CLR types, nullability, date formats, delimiter and encoding; `ToColumnMaps()` seeds a `CsvExtractor<TRecord>`, and `ToJson()` / `FromJson()` persist the result.
