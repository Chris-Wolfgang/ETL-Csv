using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The outcome of running a <see cref="CsvValidator{T}"/> against a record: whether it passed and, if
/// not, the human-readable reasons it failed.
/// </summary>
/// <param name="IsValid"><c>true</c> when the record satisfied the rule; otherwise <c>false</c>.</param>
/// <param name="Failures">The failure reasons. Empty when <paramref name="IsValid"/> is <c>true</c>.</param>
public sealed record CsvValidationResult(bool IsValid, IReadOnlyList<string> Failures)
{
    /// <summary>A shared, cached "passed" result with no failures.</summary>
    public static CsvValidationResult Pass { get; } = new(IsValid: true, Array.Empty<string>());



    /// <summary>Creates a failed result carrying one or more <paramref name="reasons"/>.</summary>
    /// <param name="reasons">The reasons the record failed. A null array is treated as empty.</param>
    public static CsvValidationResult Fail(params string[] reasons) =>
        new(IsValid: false, reasons ?? Array.Empty<string>());
}
