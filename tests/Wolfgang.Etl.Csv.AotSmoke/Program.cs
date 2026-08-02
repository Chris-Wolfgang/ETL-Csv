using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv;

namespace Wolfgang.Etl.Csv.AotSmoke;

/// <summary>
/// A simple, fully-settable record. <see cref="CsvExtractor{TRecord}"/> annotates its type
/// parameter with <c>[DynamicallyAccessedMembers(PublicProperties)]</c>, so the trimmer preserves
/// these properties even under Native AOT.
/// </summary>
public sealed class Person
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public int Age { get; set; }
}


internal static class Program
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);


    // The smoke DELIBERATELY calls the [RequiresUnreferencedCode] CsvExtractor / CsvLoader API to
    // observe whether it actually functions under Native AOT for a [DynamicallyAccessedMembers]-
    // preserved record. IL2026 (unreferenced code) and IL3050 (dynamic code) are therefore expected
    // at these call sites and suppressed with this justification, per the issue's guidance to
    // document required trim suppressions.
    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "Smoke test: intentionally exercises the [RequiresUnreferencedCode] extract/load path to verify AOT runtime behavior for a properties-preserved record."
    )]
    [UnconditionalSuppressMessage
    (
        "AOT",
        "IL3050",
        Justification = "Smoke test: intentionally exercises the reflection/expression mapping path to verify AOT runtime behavior."
    )]
    private static async Task<int> Main()
    {
        try
        {
            // Pure POCO / options surface — no reflection, must be AOT-safe.
            var map = new CsvColumnMap(nameof(Person.FirstName)) { Index = 0 };
            var trim = CsvTrimOptions.Trim;
            _ = new CsvExtractorProgress(0, 0, 0, 0);
            _ = new CsvLoaderProgress(0, 0, 0);

            var people = new[]
            {
                new Person { FirstName = "Alice", LastName = "Smith", Age = 30 },
                new Person { FirstName = "Bob", LastName = "Jones", Age = 25 },
            };

            // Load: CsvLoader writes the records to CSV text (reflection-mapped hot path).
            string csv;
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
                var loader = new CsvLoader<Person>(writer) { LeaveOpen = true };
                await loader.LoadAsync(ToAsync(people)).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
                csv = Utf8NoBom.GetString(stream.ToArray());
            }

            // Extract: CsvExtractor reads them back (reflection-mapped hot path).
            var readBack = new List<Person>();
            using (var reader = new StreamReader(new MemoryStream(Utf8NoBom.GetBytes(csv)), Utf8NoBom))
            {
                var extractor = new CsvExtractor<Person>(reader);
                await foreach (var person in extractor.ExtractAsync().ConfigureAwait(false))
                {
                    readBack.Add(person);
                }
            }

            if (readBack.Count != 2
                || !string.Equals(readBack[0].FirstName, "Alice", StringComparison.Ordinal)
                || !string.Equals(readBack[1].LastName, "Jones", StringComparison.Ordinal)
                || readBack[1].Age != 25)
            {
                Console.Error.WriteLine($"AOT smoke FAILED: round-trip mismatch (got {readBack.Count} records).");
                return 1;
            }

            Console.WriteLine
            (
                $"AOT smoke OK: round-tripped {readBack.Count} records under Native AOT " +
                $"(map={map.PropertyName}#{map.Index}, trim={trim})."
            );
            return 0;
        }
        catch (Exception ex)
        {
            // A MissingMethodException / NotSupportedException here is the exact failure #76 hunts for.
            Console.Error.WriteLine($"AOT smoke FAILED with {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }


    private static async IAsyncEnumerable<Person> ToAsync(IEnumerable<Person> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }

        await Task.CompletedTask;
    }
}
