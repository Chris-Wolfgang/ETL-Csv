type: internal

`CsvExtractor` / `CsvLoader` constructor chains: the `(ILogger?)` casts are gone (the named `options:` argument already disambiguates) the `(reader/writer, logger = null)` overloads keep their default for now — dropping it (S3427; the single-argument constructor already wins `new X(reader)`) changes the recorded public signature and is scheduled for the 2026-12-15 wave (#379).
