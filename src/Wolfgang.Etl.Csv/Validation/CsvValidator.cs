using System;
using System.Text.RegularExpressions;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// A synchronous, per-record validation rule. Runs after a record is bound and returns a
/// <see cref="CsvValidationResult"/>. Combine several on the extractor's (or loader's) <c>Validators</c>
/// collection; their failures aggregate.
/// </summary>
/// <typeparam name="T">The record type being validated.</typeparam>
/// <param name="record">The record to validate.</param>
/// <returns>The validation outcome.</returns>
public delegate CsvValidationResult CsvValidator<in T>(T record);



/// <summary>
/// Factory methods for the common built-in <see cref="CsvValidator{T}"/> rules. Each takes a selector
/// that reads the member under test, so the rules stay strongly typed without reflection.
/// </summary>
public static class CsvValidator
{
    /// <summary>Fails when the selected string is null or empty.</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="selector">Reads the string member under test.</param>
    /// <param name="memberName">Optional member name used in the failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    public static CsvValidator<T> NotNullOrEmpty<T>(Func<T, string?> selector, string? memberName = null)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return record =>
            string.IsNullOrEmpty(selector(record))
                ? CsvValidationResult.Fail($"{Label(memberName)} must not be null or empty.")
                : CsvValidationResult.Pass;
    }



    /// <summary>Fails when the selected value is not strictly greater than <paramref name="threshold"/>.</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="selector">Reads the comparable member under test.</param>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <param name="memberName">Optional member name used in the failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> or <paramref name="threshold"/> is null.</exception>
    public static CsvValidator<T> GreaterThan<T>(Func<T, IComparable> selector, IComparable threshold, string? memberName = null)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        if (threshold is null)
        {
            throw new ArgumentNullException(nameof(threshold));
        }

        return record =>
        {
            var value = selector(record);
            return value is not null && TryCompareTo(value, threshold, out var comparison) && comparison > 0
                ? CsvValidationResult.Pass
                : CsvValidationResult.Fail($"{Label(memberName)} must be greater than {threshold}.");
        };
    }



    /// <summary>Fails when the selected value is outside the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="selector">Reads the comparable member under test.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <param name="memberName">Optional member name used in the failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/>, <paramref name="min"/>, or <paramref name="max"/> is null.</exception>
    public static CsvValidator<T> InRange<T>(Func<T, IComparable> selector, IComparable min, IComparable max, string? memberName = null)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        if (min is null)
        {
            throw new ArgumentNullException(nameof(min));
        }

        if (max is null)
        {
            throw new ArgumentNullException(nameof(max));
        }

        return record =>
        {
            var value = selector(record);
            return value is not null
                && TryCompareTo(value, min, out var low) && low >= 0
                && TryCompareTo(value, max, out var high) && high <= 0
                ? CsvValidationResult.Pass
                : CsvValidationResult.Fail($"{Label(memberName)} must be between {min} and {max} inclusive.");
        };
    }



    /// <summary>Fails when the selected string is longer than <paramref name="maxLength"/>. A null string passes.</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="selector">Reads the string member under test.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="memberName">Optional member name used in the failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is negative.</exception>
    public static CsvValidator<T> MaxLength<T>(Func<T, string?> selector, int maxLength, string? memberName = null)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "Max length must be non-negative.");
        }

        return record =>
            (selector(record)?.Length ?? 0) <= maxLength
                ? CsvValidationResult.Pass
                : CsvValidationResult.Fail($"{Label(memberName)} must be at most {maxLength} characters.");
    }



    /// <summary>Fails when the selected string does not match <paramref name="pattern"/>. A null string is tested as empty.</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="selector">Reads the string member under test.</param>
    /// <param name="pattern">The regular expression the value must match.</param>
    /// <param name="memberName">Optional member name used in the failure message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> or <paramref name="pattern"/> is null.</exception>
    public static CsvValidator<T> Matches<T>(Func<T, string?> selector, Regex pattern, string? memberName = null)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        return record =>
            pattern.IsMatch(selector(record) ?? string.Empty)
                ? CsvValidationResult.Pass
                : CsvValidationResult.Fail($"{Label(memberName)} must match '{pattern}'.");
    }



    /// <summary>Fails when <paramref name="predicate"/> returns <c>false</c>, reporting <paramref name="failureMessage"/>.</summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="predicate">The rule the record must satisfy.</param>
    /// <param name="failureMessage">The message reported when the predicate fails.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="failureMessage"/> is null.</exception>
    public static CsvValidator<T> Custom<T>(Func<T, bool> predicate, string failureMessage)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (failureMessage is null)
        {
            throw new ArgumentNullException(nameof(failureMessage));
        }

        return record => predicate(record) ? CsvValidationResult.Pass : CsvValidationResult.Fail(failureMessage);
    }



    // IComparable.CompareTo throws ArgumentException when the runtime types are incompatible (e.g. an
    // int selector against a long threshold). Treat that as "did not satisfy the rule" — a validation
    // failure — rather than letting it abort the whole extraction/load.
    private static bool TryCompareTo(IComparable value, IComparable other, out int comparison)
    {
        try
        {
            comparison = value.CompareTo(other);
            return true;
        }
        catch (ArgumentException)
        {
            comparison = 0;
            return false;
        }
    }



    private static string Label(string? memberName) =>
        string.IsNullOrWhiteSpace(memberName) ? "Value" : memberName!;
}
