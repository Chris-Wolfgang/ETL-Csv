namespace Wolfgang.Etl.Csv.Examples.SchemaBuilder;

/// <summary>
/// A record you cannot (or do not want to) decorate — imagine it comes from a third-party
/// package or a code generator. It carries no <c>[CsvColumn]</c> attributes; the CSV layout
/// is described in code with <see cref="CsvSchemaBuilder{T}"/> instead.
/// </summary>
public sealed record Order
{
    public int Id { get; set; }

    public DateTime PlacedOn { get; set; }

    public decimal Total { get; set; }
}
