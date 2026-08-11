using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A record that failed validation, together with where it came from and why. Passed to the
/// <c>InvalidRecordHandler</c> on the extractor / loader.
/// </summary>
/// <typeparam name="T">The record type.</typeparam>
/// <param name="Record">The record that failed. It is fully materialized — validation runs after binding.</param>
/// <param name="LineNumber">The 1-based source line the record came from, or the line most recently written.</param>
/// <param name="Failures">The aggregated failure reasons across every validator that rejected the record.</param>
public sealed record CsvInvalidRecord<T>(T Record, int LineNumber, IReadOnlyList<string> Failures);
