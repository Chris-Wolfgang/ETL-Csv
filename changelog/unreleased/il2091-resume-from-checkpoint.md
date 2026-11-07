type: fix

`ResumeFromCheckpointAsync<TRecord>` now carries the same `DynamicallyAccessedMembers(PublicProperties)` annotation on `TRecord` as `CsvExtractor<TRecord>`, so trimmed and Native AOT callers no longer see an IL2091 warning flow through it.
