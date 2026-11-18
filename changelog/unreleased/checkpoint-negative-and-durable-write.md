type: fix

`CsvCheckpointExtensions.WriteCheckpointAsync` now rejects a negative count with `ArgumentOutOfRangeException` and flushes the temporary file to disk before the atomic rename, so a power loss cannot leave an empty checkpoint. `ReadCheckpointAsync` reports a negative value in the file as `FormatException` (the documented failure) instead of letting it surface later from the skip-count setter.
