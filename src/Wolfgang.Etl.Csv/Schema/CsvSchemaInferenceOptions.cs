using System.Globalization;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Options controlling <see cref="CsvSchema.InferAsync"/>.
/// </summary>
public sealed class CsvSchemaInferenceOptions
{
    /// <summary>The maximum number of data rows to sample. Defaults to <c>100</c>.</summary>
    public int SampleRows { get; set; } = 100;



    /// <summary>Whether the first row is a header of column names. Defaults to <c>true</c>.</summary>
    public bool HasHeaderRecord { get; set; } = true;



    /// <summary>
    /// The field delimiter. When <c>null</c> (the default) it is auto-detected from the first non-blank
    /// line by counting <c>,</c>, tab, <c>|</c>, and <c>;</c> and picking the most frequent (tie goes to comma).
    /// </summary>
    public string? Delimiter { get; set; }



    /// <summary>The culture used to parse numbers and dates while classifying columns. Defaults to <see cref="CultureInfo.InvariantCulture"/>.</summary>
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
}
