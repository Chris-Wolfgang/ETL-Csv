#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsCheck;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Property / fuzz tests (CsCheck) for the <see cref="CsvExtractor{TRecord}"/> ⇄
/// <see cref="CsvLoader{TRecord}"/> round trip (#62). The short version runs in PR CI (CsCheck's
/// default ~100 cases); the long version runs in <c>fuzz.yaml</c>, which scales the case count via
/// the <c>CsCheck_Time</c> / <c>CsCheck_Iter</c> environment variables CsCheck reads at runtime.
///
/// Marked <c>[Trait("Category", "Fuzz")]</c> so <c>fuzz.yaml</c> can select just these tests.
/// Restricted to net8.0+ (see the csproj) — CsCheck targets net8.0.
/// </summary>
[Trait("Category", "Fuzz")]
public class CsvFuzzTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);


    // Printable ASCII (space..tilde) — exercises delimiter, quote and whitespace quoting/escaping,
    // and the empty string, without bare CR/LF/TAB whose CSV round trip is subject to newline
    // normalization (a distinct concern, not this invariant).
    private static readonly Gen<string> GenField =
        Gen.Char[' ', '~'].Array[0, 12].Select(chars => new string(chars));


    private static readonly Gen<PersonRecord> GenPerson =
        Gen.Select
        (
            GenField,
            GenField,
            Gen.Int[0, 130],
            (first, last, age) => new PersonRecord { FirstName = first, LastName = last, Age = age }
        );


    [Fact]
    public void Extract_after_Load_round_trips_every_record()
    {
        GenPerson.List[0, 40].Sample
        (
            records => SameRecords(records, ExtractFromString(LoadToString(records)))
        );
    }


    [Fact]
    public void SameRecords_when_lists_match_returns_true()
    {
        var records = new List<PersonRecord> { new() { FirstName = "A", LastName = "B", Age = 1 } };

        Assert.True(SameRecords(records, new List<PersonRecord>(records)));
    }


    [Fact]
    public void SameRecords_when_counts_differ_returns_false()
    {
        var records = new List<PersonRecord> { new() { FirstName = "A", LastName = "B", Age = 1 } };

        Assert.False(SameRecords(records, new List<PersonRecord>()));
    }


    [Theory]
    [InlineData("X", "B", 1)]
    [InlineData("A", "X", 1)]
    [InlineData("A", "B", 2)]
    public void SameRecords_when_a_field_differs_returns_false(string firstName, string lastName, int age)
    {
        var expected = new List<PersonRecord> { new() { FirstName = "A", LastName = "B", Age = 1 } };
        var actual = new List<PersonRecord> { new() { FirstName = firstName, LastName = lastName, Age = age } };

        Assert.False(SameRecords(expected, actual));
    }


    // The round-trip invariant: same count, and every record's fields equal (ordinal).
    private static bool SameRecords(IReadOnlyList<PersonRecord> expected, IReadOnlyList<PersonRecord> actual)
    {
        if (actual.Count != expected.Count)
        {
            return false;
        }

        for (var i = 0; i < expected.Count; i++)
        {
            if (!string.Equals(actual[i].FirstName, expected[i].FirstName, StringComparison.Ordinal)
                || !string.Equals(actual[i].LastName, expected[i].LastName, StringComparison.Ordinal)
                || actual[i].Age != expected[i].Age)
            {
                return false;
            }
        }

        return true;
    }


    // VSTHRD002/S5034 suppressions: CsCheck's `Sample` predicate is synchronous
    // (it takes a `Func<T, bool>`), and the round-trip test above calls these
    // helpers from inside that predicate. Making them async would require
    // switching to `SampleAsync` and rewriting the invariant check — deferred
    // until CsCheck standardizes on async predicates. There's no synchronization
    // context in the xunit test host, so the sync-over-async here can't deadlock.
#pragma warning disable VSTHRD002, S5034
    private static string LoadToString(IReadOnlyList<PersonRecord> records)
    {
        using var stream = new MemoryStream();

        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord>
        { LeaveOpen = true});

        loader.LoadAsync(ToAsync(records)).GetAwaiter().GetResult();
        writer.Flush();

        return Utf8NoBom.GetString(stream.ToArray());
    }


    private static List<PersonRecord> ExtractFromString(string csv)
    {
        using var reader = new StreamReader(new MemoryStream(Utf8NoBom.GetBytes(csv)), Utf8NoBom);
        var extractor = new CsvExtractor<PersonRecord>(reader);

        var results = new List<PersonRecord>();
        var enumerator = extractor.ExtractAsync().GetAsyncEnumerator();

        try
        {
            while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
            {
                results.Add(enumerator.Current);
            }
        }
        finally
        {
            enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        return results;
    }
#pragma warning restore VSTHRD002, S5034


    private static async IAsyncEnumerable<PersonRecord> ToAsync(IEnumerable<PersonRecord> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }

        await Task.CompletedTask;
    }
}

#endif
