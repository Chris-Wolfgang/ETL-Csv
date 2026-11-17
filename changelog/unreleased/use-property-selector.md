type: internal

`CsvSchemaBuilder<T>.Column` now resolves its property selector with the shared `PropertySelector.Resolve` from Wolfgang.Etl.Abstractions 0.27 instead of a private copy. Accepted and rejected selectors are unchanged.
