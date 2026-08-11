using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Thrown when a record fails validation and <c>OnValidationFailure</c> is
/// <see cref="CsvValidationFailureAction.Stop"/>. Carries the source line and the aggregated
/// failure reasons.
/// </summary>
public sealed class CsvValidationException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="CsvValidationException"/> class.</summary>
    /// <param name="lineNumber">The 1-based line the failing record came from.</param>
    /// <param name="failures">The failure reasons. A null collection is treated as empty.</param>
    public CsvValidationException(int lineNumber, IReadOnlyList<string> failures)
        : base(BuildMessage(lineNumber, failures))
    {
        LineNumber = lineNumber;
        Failures = failures ?? Array.Empty<string>();
    }



    /// <summary>Gets the 1-based line the failing record came from.</summary>
    public int LineNumber { get; }



    /// <summary>Gets the aggregated reasons the record failed validation.</summary>
    public IReadOnlyList<string> Failures { get; }



    private static string BuildMessage(int lineNumber, IReadOnlyList<string>? failures)
    {
        var reasons = failures is { Count: > 0 }
            ? string.Join("; ", failures)
            : "(no reasons provided)";

        return $"Record on line {lineNumber} failed validation: {reasons}";
    }
}
