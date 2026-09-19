type: internal

S1133 ("remove this deprecated code someday") is excluded per file for `CsvLoader.cs`, `CsvExtractorOptions.cs` and `CsvLoaderOptions.cs` (joining `CsvExtractor.cs`) in the nested `src/.editorconfig`: their `[Obsolete]` setters and record aliases are the deliberate markers for the 2026-12-15 removal wave (#283).
