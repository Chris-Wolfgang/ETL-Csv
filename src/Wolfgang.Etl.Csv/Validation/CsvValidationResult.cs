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
public sealed record CsvValidationResult
{
    /// <summary>
    /// Legacy positional constructor retained from the 0.6.x record-primary-ctor shape. Prefer
    /// <see cref="CsvValidationResult()"/> for success or <see cref="CsvValidationResult(IReadOnlyList{string})"/>
    /// for failure. This constructor still validates its inputs and throws on inconsistent state.
    /// </summary>
    /// <param name="IsValid"><c>true</c> when the record satisfied the rule; otherwise <c>false</c>.</param>
    /// <param name="Failures">The failure reasons. Empty when <paramref name="IsValid"/> is <c>true</c>; non-empty otherwise.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Failures"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="IsValid"/> is <c>true</c> and <paramref name="Failures"/> is non-empty,
    /// or <paramref name="IsValid"/> is <c>false</c> and <paramref name="Failures"/> is empty.
    /// </exception>
    [Obsolete("Use CsvValidationResult() for success or CsvValidationResult(IReadOnlyList<string>) for failure. This constructor will be removed in a future major version.")]
    public CsvValidationResult(bool IsValid, IReadOnlyList<string> Failures)
    {
        if (Failures is null)
        {
            throw new ArgumentNullException(nameof(Failures));
        }

        if (IsValid && Failures.Count > 0)
        {
            throw new ArgumentException
            (
                "A successful CsvValidationResult cannot carry failure reasons.",
                nameof(Failures)
            );
        }

        if (!IsValid && Failures.Count == 0)
        {
            throw new ArgumentException
            (
                "A failed CsvValidationResult must contain at least one failure reason.",
                nameof(Failures)
            );
        }

        this.Failures = Failures;
        this.IsValid = IsValid;
    }



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
    public bool IsValid { get; init; }



    /// <summary>
    /// The failure reasons. Empty (never <c>null</c>) when <see cref="IsValid"/> is <c>true</c>;
    /// non-empty otherwise.
    /// </summary>
    public IReadOnlyList<string> Failures { get; init; }



    /// <summary>
    /// Deconstructs the result into its <see cref="IsValid"/> and <see cref="Failures"/> components.
    /// Kept explicit because the record no longer carries a positional primary constructor — without
    /// this, the compiler wouldn't synthesize a <c>Deconstruct</c> at all and the shipped 0.6.x API
    /// would break.
    /// </summary>
    /// <param name="IsValid">Receives <see cref="IsValid"/>.</param>
    /// <param name="Failures">Receives <see cref="Failures"/>.</param>
    public void Deconstruct(out bool IsValid, out IReadOnlyList<string> Failures)
    {
        IsValid = this.IsValid;
        Failures = this.Failures;
    }



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
