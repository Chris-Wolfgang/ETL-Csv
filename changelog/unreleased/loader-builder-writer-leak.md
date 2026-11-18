type: fix

The path-based `CsvLoader(path)` pipeline terminator now validates the options before opening (and truncating) the target file, and disposes the `StreamWriter` it opened if constructing the loader or wiring the sink throws.
