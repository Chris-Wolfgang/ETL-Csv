type: docs

`CsvLoaderOptions<TRecord>` and the README now say that `CsvLoader<TRecord>` does not consult the inherited `ErrorPolicy` (a record that fails partway through cannot be withdrawn), and point at validators with `Skip` for dropping records before they are written.
