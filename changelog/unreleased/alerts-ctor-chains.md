type: internal

`CsvExtractor` / `CsvLoader` constructor chains: the `(ILogger?)` casts are gone (the named `options:` argument already disambiguates) and the `(reader/writer, logger = null)` overloads drop their dead default — the hidden single-argument constructor already wins `new X(reader)` (S3427). Binary signature unchanged; PublicAPI text updated.
