type: fix

The runtime `ColumnMaps` class-map cache no longer returns a stale map when the same list is repopulated with a different layout, and no longer pins every list it has seen for the life of the process: entries are held weakly by list instance and revalidated against a snapshot of the list's contents.
