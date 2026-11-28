type: breaking

Move to CsvHelper 34 (#514). Record binding is now the library's own, so plain record types keep working unchanged, but CsvHelper 33 exceptions become `CsvHelper.CsvFormatException`, a throwing property getter surfaces unwrapped instead of as `WriterException`, `Delimiter` / `Quote` / `Comment` must be single ASCII characters, `NewLine` must be `\r\n`, `\n` or `\r`, and an `Escape` other than `Quote` (or its default `"`) is rejected at construction; see ADR 0007.
