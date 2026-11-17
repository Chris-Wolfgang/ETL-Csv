type: fix

ColumnMaps (and CsvDiscriminator.PerTypeColumnMaps) are now copied into a private immutable list when assigned. Editing the list you passed in afterwards, for example `maps[0] = new CsvColumnMap(...)`, no longer affects an extractor or loader built from it, and the list read back cannot be cast to a mutable collection.
