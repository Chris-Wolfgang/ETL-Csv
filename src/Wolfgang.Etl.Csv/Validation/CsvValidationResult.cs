using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The outcome of running a <see cref="CsvValidator{T}"/> against a record: whether it passed and, if
/// not, the human-readable reasons it failed. Illegal states (successful-with-failures, failed-without-
/// failures, null failures) throw at construction — the two named constructors below are the recommended
/// path; the record's positional constructor is retained for source-compat and marked
/// <see cref="ObsoleteAttribute"/>.
/// </summary>
/// <param name="IsValid"><c>true</c> when the record satisfied the rule; otherwise <c>false</c>.</param>
/// <param name="Failures">The failure reasons. Empty when <paramref name="IsValid"/> is <c>true</c>.</param>
public sealed record CsvValidationResult
{

    [Obsolete("Use CsvValidationResult() for success or CsvValidationResult(IReadOnlyList<string>) for failure. This constructor will be removed in a future major version.")]
    public CsvValidationResult(bool IsValid, IReadOnlyList<string> Failures)
    {
        ...
    }
    
    /// <summary>Constructs a successful validation result (no failures).</summary>
#pragma warning disable CS0618 // internal chain to the record's own [Obsolete] primary ctor is intentional
    public CsvValidationResult() : this(IsValid: true, Failures: Array.Empty<string>()) { }
#pragma warning restore CS0618



    /// <summary>Constructs a failed validation result carrying one or more failure reasons.</summary>
    /// <param name="failures">The failure reasons. Must be non-null and contain at least one entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="failures"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="failures"/> is empty.</exception>
#pragma warning disable CS0618 // internal chain to the record's own [Obsolete] primary ctor is intentional
    public CsvValidationResult(IReadOnlyList<string> failures)
        : this(IsValid: false, Failures: failures ?? throw new ArgumentNullException(nameof(failures)))
    {
        if (failures.Count == 0)
        {
            throw new ArgumentException
            (
                "A failed CsvValidationResult must contain at least one failure reason.",
                nameof(failures)
            );
        }
    }
#pragma warning restore CS0618



    /// <summary>
    /// The failure reasons. Empty (never <c>null</c>) when <see cref="IsValid"/> is <c>true</c>;
    /// non-empty otherwise. The initializer enforces the invariant so the primary constructor throws
    /// on inconsistent state whether callers use the new API or the deprecated positional one.
    /// </summary>
    public IReadOnlyList<string> Failures { get; init; } = ValidateFailures(IsValid, Failures);



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



    private static IReadOnlyList<string> ValidateFailures(bool isValid, IReadOnlyList<string> failures)
    {
        if (failures is null)
        {
            throw new ArgumentNullException(nameof(failures));
        }

        if (isValid && failures.Count > 0)
        {
            throw new ArgumentException
            (
                "A successful CsvValidationResult cannot carry failure reasons.",
                nameof(failures)
            );
        }

        if (!isValid && failures.Count == 0)
        {
            throw new ArgumentException
            (
                "A failed CsvValidationResult must contain at least one failure reason.",
                nameof(failures)
            );
        }

        return failures;
    }
}
