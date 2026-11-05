# Getting Started

This guide will help you quickly get up and running with Wolfgang.Etl.Csv.

## Prerequisites

- A project targeting .NET Framework 4.6.2+, .NET Standard 2.0/2.1, or .NET 5.0+
- [Wolfgang.Etl.Abstractions](https://www.nuget.org/packages/Wolfgang.Etl.Abstractions/) comes in as a dependency of the package

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Etl.Csv
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Etl.Csv
```

## Quick Start

Define a record type, then read a CSV with `await foreach`:

```csharp
using System.IO;
using Wolfgang.Etl.Csv;

public sealed record Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public int    Age       { get; set; }
}

using var reader = new StreamReader("people.csv");
var extractor = new CsvExtractor<Person>(reader);

await foreach (var person in extractor.ExtractAsync(cancellationToken))
{
    Console.WriteLine($"{person.FirstName} {person.LastName} ({person.Age})");
}
```

Write records back out with `CsvLoader<T>`:

```csharp
using var writer = new StreamWriter("people.csv");
var loader = new CsvLoader<Person>(writer);

await loader.LoadAsync(GetPeopleAsync(), cancellationToken);
```

Delimiters, headers, column maps, validation and error handling are configured through
`CsvExtractorOptions<TRecord>` / `CsvLoaderOptions<TRecord>` passed to the constructor.

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed documentation
- Read the [Introduction](introduction.md) to learn more about Wolfgang.Etl.Csv
- Check out the runnable examples under `examples/` in the [GitHub repository](https://github.com/Chris-Wolfgang/ETL-Csv)

## Common Issues

- **Values parse differently on another machine:** parsing and formatting follow the host's current culture.
- **NativeAOT / trimming:** the extractor and loader are `[RequiresUnreferencedCode]`. CsvHelper reflects over the record type, so the library is not trim-safe.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/ETL-Csv)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/ETL-Csv/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/ETL-Csv/issues)
