type: internal

The options constructor assigns the stage's backing fields directly instead of going through the deprecated setters, so the `CS0618` suppressions that covered those writes are gone. The `InitialRecordIndex` guard (must be 1 or greater) now also runs on `CsvExtractorOptions<TRecord>.InitialRecordIndex`'s init accessor, with a test. (#339)
