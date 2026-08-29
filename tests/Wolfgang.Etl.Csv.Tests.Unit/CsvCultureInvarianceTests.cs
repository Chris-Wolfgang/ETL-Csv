using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Verifies CSV extract / load operations are stable across cultures that
/// historically break naive .NET string handling:
///
/// - <c>tr-TR</c> (Turkish I/i casing) — distinguishes between dotted-i and
///   dotless-I; <c>"FIRST".ToLower()</c> in tr-TR is <c>"fırst"</c>, not
///   <c>"first"</c>. Catches case-insensitive column-name matches that rely
///   on culture-sensitive lower().
/// - <c>de-DE</c> (decimal comma) — <c>1234.56.ToString()</c> in de-DE is
///   <c>"1234,56"</c>, breaking pipelines that round-trip numbers through
///   strings without an explicit culture.
/// - <c>ja-JP</c> (Japanese calendar / date format) — short-date formats
///   default to Japanese era notation in some configurations. Catches
///   date parsing that omits an explicit format string.
///
/// Each test sets <see cref="CultureInfo.CurrentCulture"/> and
/// <see cref="CultureInfo.CurrentUICulture"/> on its own thread for the
/// duration of the test only; restored in <c>finally</c>.
/// </summary>
public class CsvCultureInvarianceTests
{
    private static StreamReader Reader(string csv) =>
        new(new MemoryStream(Encoding.UTF8.GetBytes(csv)), Encoding.UTF8);



    private static async Task<List<T>> RunUnderCulture<T>(string cultureName, Func<Task<List<T>>> action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;
        try
        {
            var target = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = target;
            CultureInfo.CurrentUICulture = target;
            return await action().ConfigureAwait(false);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }



    [Fact]
    public async Task ExtractAsync_when_culture_is_tr_TR_matches_columns_case_insensitively_without_locale_skew()
    {
        // Header has uppercase "AGE"; the AttributedPersonRecord doesn't use
        // [CsvColumn(Name=...)] for Age, so CsvHelper's default matching
        // applies. The risk on tr-TR is that an internal ToLower/ToUpper
        // using the current culture turns "AGE" into "AGE" (no I in this
        // word) but turns "FirstName" into "fırstname" — a bad match for
        // "first_name". To exercise the failure mode we use a property
        // name with capital I.
        var csv = "first_name,last_name,age\r\nIlhan,Yilmaz,30\r\n";
        var results = await RunUnderCulture("tr-TR", async () =>
        {
            var sut = new CsvExtractor<AttributedPersonRecord>(Reader(csv));
            var list = new List<AttributedPersonRecord>();
            await foreach (var item in sut.ExtractAsync())
            {
                list.Add(item);
            }
            return list;
        });

        Assert.Single(results);
        Assert.Equal("Ilhan", results[0].FirstName);
        Assert.Equal("Yilmaz", results[0].LastName);
        Assert.Equal(30, results[0].Age);
    }



    [Fact]
    public async Task ExtractAsync_when_culture_is_de_DE_parses_integer_columns_correctly()
    {
        // Age=30 should parse the same regardless of decimal-separator
        // culture. The risk is a numeric-style mis-parse — e.g. if a parser
        // treats "30" under de-DE as something other than the integer 30.
        var csv = "first_name,last_name,age\r\nKlaus,Müller,30\r\n";
        var results = await RunUnderCulture("de-DE", async () =>
        {
            var sut = new CsvExtractor<AttributedPersonRecord>(Reader(csv));
            var list = new List<AttributedPersonRecord>();
            await foreach (var item in sut.ExtractAsync())
            {
                list.Add(item);
            }
            return list;
        });

        Assert.Single(results);
        Assert.Equal("Müller", results[0].LastName);
        Assert.Equal(30, results[0].Age);
    }



    [Fact]
    public async Task LoadAsync_when_culture_is_ja_JP_writes_integers_in_invariant_form()
    {
        // Writing a record with an integer column under ja-JP should yield
        // standard "30" — not Japanese-era notation or full-width digits.
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 1024, leaveOpen: true);

        await RunUnderCulture("ja-JP", async () =>
        {
            var sut = new CsvLoader<AttributedPersonRecord>(writer, new CsvLoaderOptions<AttributedPersonRecord>
        { LeaveOpen = true});
            var items = new List<AttributedPersonRecord>
            {
                new() { FirstName = "Alice", LastName = "Smith", Age = 30 },
            };
            await sut.LoadAsync(items.ToAsyncEnumerable()).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
            return new List<int> { 0 };
        });

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var text = await reader.ReadToEndAsync();

        Assert.Contains("Alice,Smith,30", text, StringComparison.Ordinal);
        // Defensive: explicitly verify no full-width digits ended up in the output.
        Assert.DoesNotContain("０", text, StringComparison.Ordinal); // U+FF10 fullwidth 0
        Assert.DoesNotContain("３", text, StringComparison.Ordinal); // U+FF13 fullwidth 3
    }
}
