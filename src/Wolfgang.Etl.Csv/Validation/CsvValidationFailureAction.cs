namespace Wolfgang.Etl.Csv;

/// <summary>
/// Chooses what a <see cref="CsvExtractor{TRecord}"/> (or <see cref="CsvLoader{TRecord}"/>) does with a
/// record that fails one or more <see cref="CsvValidator{T}"/>s. In every case the invalid record is
/// counted (<c>CurrentInvalidItemCount</c>) and the <c>InvalidRecordHandler</c>, if any, is invoked first.
/// </summary>
public enum CsvValidationFailureAction
{
    /// <summary>Yield / write the record anyway — the caller decides what to do with it.</summary>
    Continue,

    /// <summary>Drop the record: it is neither yielded nor written, and it does not count as extracted / loaded.</summary>
    Skip,

    /// <summary>Raise a <see cref="CsvValidationException"/> and end the operation.</summary>
    Stop,
}
