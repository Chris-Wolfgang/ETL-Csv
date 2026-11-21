type: feature

Add the standard `()`, `(string)` and `(string, Exception)` constructors to `CsvValidationException`; exceptions built with them report `LineNumber` 0 and an empty `Failures` list.
