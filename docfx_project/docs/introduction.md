# Introduction

Welcome to Wolfgang.Etl.Csv!

## Overview

A `CsvExtractor<T>` and `CsvLoader<T>`, derived from `ExtractorBase<,>` and `LoaderBase<,>`, for ETLs built with
Wolfgang.Etl.Abstractions. They stream CSV into and out of strongly-typed records, and use
[CsvHelper](https://joshclose.github.io/CsvHelper/) internally without exposing its types.

## Key Features

- Async-first read and write of CSV streams, with cancellation throughout
- Progress reporting with item, skip, error, line-number and bad-data counts
- Compile-time column mapping (`[CsvColumn]`, `[CsvIgnore]`) and runtime mapping (`CsvColumnMap`)
- Polymorphic rows via `CsvDiscriminator<TBase>`, and streaming record validation
- `ErrorPolicy` to skip or abort on rows that fail to parse
- Resumable extraction through checkpoints
- Multi-targeted from net462 to net10.0

## Getting Help

If you need help with Wolfgang.Etl.Csv, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](../api/index.md)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/ETL-Csv)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/ETL-Csv/issues)
