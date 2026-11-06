namespace Wolfgang.Etl.Csv.Examples.RecordValidation;

public record Order
{
    public string OrderNumber { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Notes { get; set; } = string.Empty;
}
