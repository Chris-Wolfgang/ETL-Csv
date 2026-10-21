type: internal

The trim, AOT and single-file analyzers now run on the net8.0+ builds (`IsAotCompatible` / `IsTrimmable`), so the existing `[RequiresUnreferencedCode]` annotations are checked for completeness. The library is still not trim-safe: CsvHelper reflects over the record type.
