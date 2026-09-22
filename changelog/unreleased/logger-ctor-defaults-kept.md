type: internal

The `(reader|writer, logger = null)` constructor defaults on `CsvExtractor` / `CsvLoader` stay as shipped in 0.9; their removal (S3427) is scheduled for the 2026-12-15 wave (#379), with the rule silenced for those two files until then, so this release carries no public-signature change.
