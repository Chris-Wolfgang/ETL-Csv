type: fix

`CsvExtractor` and `CsvLoader` now validate their options record at construction: a null or empty `Delimiter` (and, for the loader, `NewLine`) and a name-based `Discriminator` combined with `HasHeaderRecord = false` throw `ArgumentException` naming `options`, instead of failing mid-enumeration with an `IndexOutOfRangeException` or a `ReaderException` that a skip `ErrorPolicy` could swallow.
