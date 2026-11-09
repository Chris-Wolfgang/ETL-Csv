type: fix

A reused `CsvExtractor` / `CsvLoader` now resets its CSV-specific progress counters (`CurrentLineNumber`, `CurrentBadDataCount`, `CurrentInvalidItemCount`) and the loader's invalid-record ordinal at the start of every run, matching the base counters, instead of accumulating them across runs.
