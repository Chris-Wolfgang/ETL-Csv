type: docs

`CsvExtractorOptions<TRecord>.LeaveOpen` and `CsvLoaderOptions<TRecord>.LeaveOpen` now say the reader/writer is closed (or left open) when the run completes, not when the stage is disposed.
