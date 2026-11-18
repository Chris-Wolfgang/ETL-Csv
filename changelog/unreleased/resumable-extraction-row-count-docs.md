type: docs

The resumable-extraction cookbook, example and `CsvCheckpointExtensions` docs now explain that `SkipItemCount` skips raw rows rather than committed records, and give the counter to checkpoint when validators, an `ErrorPolicy` or a discriminator can drop rows. The pipeline-extensions guide's `Through` sample now compiles, and its configuration section and the README schema-builder section no longer overstate what the fluent builders and `CsvSchema` do.
