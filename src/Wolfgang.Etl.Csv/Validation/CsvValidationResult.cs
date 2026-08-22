using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The outcome of running a <see cref="CsvValidator{T}"/> against a record: whether it passed and, if
/// not, the human-readable reasons it failed. Illegal states (successful-with-failures, failed-without-
/// failures, null failures) throw at construction — the two named constructors below are the recommended
/// path; the legacy positional constructor is retained for source-compat and marked
/// <see cref="ObsoleteAttribute"/>.
/// </summary>
/// <remarks>
/// Use <see cref="Pass"/> or the default constructor for a successful result, and either the
/// <see cref="CsvValidationResult(IReadOnlyList{string})"/> constructor or the
/// <see cref="Fail(string[])"/> factory for a failed result.
/// </remarks>
public sealed record CsvValidationResult
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



    /// <summary>
    /// Legacy positional constructor retained from the 0.6.x record-primary-ctor shape. Prefer
    /// <see cref="CsvValidationResult()"/> for success or <see cref="CsvValidationResult(IReadOnlyList{string})"/>
    /// for failure — those constructors make illegal states unrepresentable at the call site.
    /// This constructor still validates its inputs and throws on inconsistent state, but the
    /// callsite can't express the intent as clearly as the two named constructors.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="Failures"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="IsValid"/> is <c>true</c> and <paramref name="Failures"/> is non-empty,
    /// or <paramref name="IsValid"/> is <c>false</c> and <paramref name="Failures"/> is empty.
    /// </exception>
    [Obsolete("Use CsvValidationResult() for success or CsvValidationResult(IReadOnlyList<string>) for failure. This constructor will be removed in a future major version.")]
    public CsvValidationResult(bool IsValid, IReadOnlyList<string> Failures)
    {
        this.Failures = ValidateFailures(IsValid, Failures);
        this.IsValid = IsValid;
    }



    /// <summary><c>true</c> when the record satisfied the rule; otherwise <c>false</c>.</summary>
    public bool IsValid { get; init; }



    /// <summary>
    /// The failure reasons. Empty (never <c>null</c>) when <see cref="IsValid"/> is <c>true</c>;
    /// non-empty otherwise.
    /// </summary>
    public IReadOnlyList<string> Failures { get; init; }



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



    /// <summary>
    /// Legacy positional deconstruction retained from the 0.6.x record-primary-ctor shape. Prefer
    /// reading <see cref="IsValid"/> and <see cref="Failures"/> directly. Will be removed in a
    /// future major version alongside the positional constructor.
    /// </summary>
    [Obsolete("Read IsValid and Failures directly. Deconstruct will be removed in a future major version.")]
    public void Deconstruct(out bool IsValid, out IReadOnlyList<string> Failures)
    {
        IsValid = this.IsValid;
        Failures = this.Failures;
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
