using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The outcome of running a <see cref="CsvValidator{T}"/> against a record: whether it passed and, if
/// not, the human-readable reasons it failed. Illegal states (successful-with-failures, failed-without-
/// failures) are unrepresentable — the type has two constructors, one for each outcome, and each
/// enforces its own invariant at construction time.
/// </summary>
/// <remarks>
/// Use <see cref="Pass"/> or the default constructor for a successful result, and either the
/// <see cref="CsvValidationResult(IReadOnlyList{string})"/> constructor or the
/// <see cref="Fail(string[])"/> factory for a failed result.
/// </remarks>
public sealed class CsvValidationResult
{
    /// <summary>Constructs a successful validation result (no failures).</summary>
    public CsvValidationResult()
    {
        IsValid = true;
        Failures = Array.Empty<string>();
    }



    /// <summary>Constructs a failed validation result carrying one or more failure reasons.</summary>
    /// <param name="failures">The failure reasons. Must be non-null and contain at least one entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="failures"/> is empty.</exception>
    public CsvValidationResult(IReadOnlyList<string> failures)
    {
        if (failures is null)
        {
            throw new ArgumentNullException(nameof(failures));
        }

        if (failures.Count == 0)
        {
            throw new ArgumentException
            (
                "A failed CsvValidationResult must contain at least one failure reason.",
                nameof(failures)
            );
        }

        IsValid = false;
        Failures = failures;
    }



    /// <summary><c>true</c> when the record satisfied the rule; otherwise <c>false</c>.</summary>
    public bool IsValid { get; }



    /// <summary>
    /// The failure reasons. Empty (never <c>null</c>) when <see cref="IsValid"/> is <c>true</c>;
    /// non-empty otherwise.
    /// </summary>
    public IReadOnlyList<string> Failures { get; }



    /// <summary>A shared, cached "passed" result with no failures.</summary>
    public static CsvValidationResult Pass { get; } = new();



    /// <summary>Creates a failed result carrying one or more <paramref name="reasons"/>.</summary>
    /// <param name="reasons">The reasons the record failed. Must be non-null and contain at least one entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="reasons"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="reasons"/> is empty.</exception>
    public static CsvValidationResult Fail(params string[] reasons)
    {
        if (reasons is null)
        {
            throw new ArgumentNullException(nameof(reasons));
        }

        if (reasons.Length == 0)
        {
            throw new ArgumentException
            (
                "At least one reason must be provided.",
                nameof(reasons)
            );
        }

        return new CsvValidationResult(reasons);
    }
}
