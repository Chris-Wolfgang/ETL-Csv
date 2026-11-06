namespace Wolfgang.Etl.Csv.Examples.PolymorphicRows;

// A file that mixes three row shapes, all deriving from a common base. The first column is the
// discriminator: HDR = batch header, PMT = payment, TRL = trailer.

public record LedgerRow
{
    public string RecordType { get; set; } = string.Empty;
}

public record HeaderRow : LedgerRow
{
    public string BatchId { get; set; } = string.Empty;

    public string BatchDate { get; set; } = string.Empty;
}

public record PaymentRow : LedgerRow
{
    public string Account { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}

public record TrailerRow : LedgerRow
{
    public int Count { get; set; }
}
