type: fix

`CsvLoaderProgress.CurrentLineNumber` now reports the line most recently written, as documented; it used to report one more (for example 5 after a header and three records).
