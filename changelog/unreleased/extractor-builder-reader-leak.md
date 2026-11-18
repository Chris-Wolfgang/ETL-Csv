type: fix

The path-based `EtlPipeline.CsvExtractor<T>(path)` builder now disposes the `StreamReader` it opened if constructing the extractor or replaying a configuration step throws, so the file handle is not left open.
