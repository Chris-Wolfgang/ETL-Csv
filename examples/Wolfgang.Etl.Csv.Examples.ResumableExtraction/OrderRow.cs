namespace Wolfgang.Etl.Csv.Examples.ResumableExtraction;

/// <summary>A single row of the demo CSV.</summary>
public sealed record OrderRow
{
    public int Id { get; set; }
}
