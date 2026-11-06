using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A record that failed validation, together with where it came from and why. Passed to the
/// <c>InvalidRecordHandler</c> on the extractor / loader.
/// </summary>
/// <typeparam name="T">The record type.</typeparam>
/// <param name="Record">The record that failed. It is fully materialized — validation runs after binding.</param>
/// <param name="LineNumber">
/// Where the record came from: for a <see cref="CsvExtractor{TRecord}"/>, the 1-based source line it was
/// read from; for a <see cref="CsvLoader{TRecord}"/>, the 1-based ordinal of the record in the input
/// sequence (validation runs before the record is written).
/// </param>
/// <param name="Failures">The aggregated failure reasons across every validator that rejected the record.</param>
public sealed record CsvInvalidRecord<T>(T Record, int LineNumber, IReadOnlyList<string> Failures);
