type: fix

`CsvCheckpointExtensions.WriteCheckpointAsync` writes each checkpoint through its own uniquely named temporary file (`<path>.<random>.tmp`) instead of a fixed `<path>.tmp`, so concurrent writers of the same checkpoint no longer collide and a stale temporary file is never reused.
