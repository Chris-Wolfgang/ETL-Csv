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


    private static async Task<int> Main()
    {
        // Phase 1 — the pure POCO / options surface has no reflection and MUST work under AOT.
        // A regression here (a value type that silently no-ops, a removed member) fails the smoke.
        try
        {
            var map = new CsvColumnMap(nameof(Person.FirstName)) { Index = 0 };
            var trim = CsvTrimOptions.Trim;
            _ = new CsvExtractorProgress(0, 0, 0, 0);
            _ = new CsvLoaderProgress(0, 0, 0);

            Console.WriteLine($"AOT-safe surface OK (map={map.PropertyName}#{map.Index}, trim={trim}).");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"AOT smoke FAILED: the trim-safe POCO surface threw {ex.GetType().Name}: {ex.Message}");
            return 1;
        }

        // Phase 2 — the extract/load hot path is [RequiresUnreferencedCode]: CsvHelper builds a
        // DefaultClassMap<T> via reflection the trimmer can't see, so under AOT it is EXPECTED to
        // throw MissingMethodException/NotSupportedException. Assert that documented boundary so a
        // silent change (either direction) is caught. Any OTHER exception type is unexpected and
        // propagates → non-zero exit → red.
        try
        {
            var count = await RoundTripAsync().ConfigureAwait(false);
            Console.WriteLine
            (
                $"NOTE: extract/load round trip SUCCEEDED under Native AOT ({count} records) — the " +
                "[RequiresUnreferencedCode] marker may now be removable; review whether the library " +
                "gained AOT support (e.g. a source-generated class map) and update this smoke."
            );
            return 0;
        }
        catch (Exception ex) when (ex is MissingMethodException or NotSupportedException)
        {
            Console.WriteLine
            (
                "OK: extract/load is unavailable under Native AOT exactly as documented " +
                $"([RequiresUnreferencedCode]) — {ex.GetType().Name}: {ex.Message}"
            );
            return 0;
        }
    }


    // Exercises the reflection-mapped hot path. IL2026/IL3050 are expected here (the API is
    // [RequiresUnreferencedCode]); suppressed with justification per the issue's guidance to
    // document required trim suppressions.
    [UnconditionalSuppressMessage
    (
        "Trimming",
        "IL2026",
        Justification = "Smoke test: intentionally exercises the [RequiresUnreferencedCode] extract/load path to verify AOT runtime behavior."
    )]
    [UnconditionalSuppressMessage
    (
        "AOT",
        "IL3050",
        Justification = "Smoke test: intentionally exercises the reflection/expression mapping path to verify AOT runtime behavior."
    )]
    private static async Task<int> RoundTripAsync()
    {
        var people = new[]
        {
            new Person { FirstName = "Alice", LastName = "Smith", Age = 30 },
            new Person { FirstName = "Bob", LastName = "Jones", Age = 25 },
        };

        string csv;
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true))
            {
                var loader = new CsvLoader<Person>(writer) { LeaveOpen = true };
                await loader.LoadAsync(ToAsync(people)).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            csv = Utf8NoBom.GetString(stream.ToArray());
        }

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
            || readBack[1].Age != 25)
        {
            throw new InvalidOperationException($"Round-trip mismatch: got {readBack.Count} records.");
        }

        return readBack.Count;
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
