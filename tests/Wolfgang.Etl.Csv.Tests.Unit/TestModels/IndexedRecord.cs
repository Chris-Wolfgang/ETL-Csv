using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.Etl.Csv.Tests.Unit.TestModels;

/// <summary>Two string columns bound by <see cref="CsvColumnAttribute.Index"/> (0 and 1).</summary>
[ExcludeFromCodeCoverage]
public sealed record IndexedRecord
{
    [CsvColumn(Index = 0)]
    public string A { get; set; } = string.Empty;

    [CsvColumn(Index = 1)]
    public string B { get; set; } = string.Empty;
}
