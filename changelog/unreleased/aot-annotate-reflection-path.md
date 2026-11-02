type: internal

The library's own reflection path (the direct-init `CsvDiscriminator` maps, runtime-type writes, and the record map registration) is now annotated with `[RequiresUnreferencedCode]` / `[RequiresDynamicCode]` or covered at its seam, so a Native AOT publish reports no trim/AOT warnings from Wolfgang.Etl.Csv itself; the remaining warnings come from CsvHelper.
