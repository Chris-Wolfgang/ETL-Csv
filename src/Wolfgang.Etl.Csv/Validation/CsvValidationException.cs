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
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvValidationException"/> class with the
    /// default exception message.
    /// </summary>
    /// <remarks>
    /// <see cref="LineNumber"/> is <c>0</c> and <see cref="Failures"/> is empty. Prefer
    /// <see cref="CsvValidationException(int, IReadOnlyList{string})"/> when the failing line and
    /// reasons are known.
    /// </remarks>
    public CsvValidationException()
    {
        Failures = Array.Empty<string>();
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvValidationException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <remarks>
    /// <see cref="LineNumber"/> is <c>0</c> and <see cref="Failures"/> is empty.
    /// </remarks>
    public CsvValidationException(string? message)
        : base(message)
    {
        Failures = Array.Empty<string>();
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="CsvValidationException"/> class with a
    /// specified error message and a reference to the inner exception that caused it.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of this exception, or <see langword="null"/>.</param>
    /// <remarks>
    /// <see cref="LineNumber"/> is <c>0</c> and <see cref="Failures"/> is empty.
    /// </remarks>
    public CsvValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        Failures = Array.Empty<string>();
    }



    /// <summary>Initializes a new instance of the <see cref="CsvValidationException"/> class.</summary>
    /// <param name="lineNumber">The 1-based line the failing record came from.</param>
    /// <param name="failures">The failure reasons. Must not be <see langword="null"/>; a <see langword="null"/> passed from code without nullable annotations is treated as empty.</param>
    public CsvValidationException(int lineNumber, IReadOnlyList<string> failures)
        : base(BuildMessage(lineNumber, failures))
    {
        LineNumber = lineNumber;
        Failures = failures ?? Array.Empty<string>();
    }



    /// <summary>
    /// Gets the 1-based line the failing record came from, or <c>0</c> when the exception was
    /// created without line information.
    /// </summary>
    public int LineNumber { get; }



    /// <summary>
    /// Gets the aggregated reasons the record failed validation. Empty when the exception was
    /// created without failure reasons.
    /// </summary>
    public IReadOnlyList<string> Failures { get; }



    private static string BuildMessage(int lineNumber, IReadOnlyList<string>? failures)
    {
        var reasons = failures is { Count: > 0 }
            ? string.Join("; ", failures)
            : "(no reasons provided)";

        return $"Record on line {lineNumber} failed validation: {reasons}";
    }
}
