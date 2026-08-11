namespace Wolfgang.Etl.Csv;

/// <summary>
/// How a polymorphic <see cref="CsvExtractor{TRecord}"/> (read) or <see cref="CsvLoader{TRecord}"/>
/// (write) handles a row whose discriminator value — or, on write, whose runtime type — is not in
/// the <see cref="CsvDiscriminator{TBase}"/> mapping.
/// </summary>
public enum CsvDiscriminatorAction
{
    /// <summary>Raise an exception for the unmapped discriminator value / record type.</summary>
    Throw,

    /// <summary>Skip the row/record and increment the skipped-item count.</summary>
    Skip,

    /// <summary>
    /// Read the row into (or write the record as) the base type <c>TBase</c> using its default
    /// (attribute- or <c>ColumnMaps</c>-based) mapping instead of a concrete-type mapping.
    /// </summary>
    YieldAsBase,
}
