#if NET10_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Snapshot / approval tests (Verify) for <see cref="CsvLoader{TRecord}"/>'s text output (#73).
/// These lock the exact serialized shape — header row, delimiter, quoting/escaping of special
/// characters, and line terminators — so accidental format drift a targeted assertion would miss
/// fails loudly against the committed <c>Snapshots/*.verified.txt</c> baseline.
///
/// The CSV string is split on its <c>\r\n</c> terminators and the resulting line array is verified,
/// rather than the raw string. Verify then owns the on-disk serialization (LF-terminated), which
/// keeps the snapshot files git/OS-portable while still capturing terminator regressions — a
/// <c>\n</c>-instead-of-<c>\r\n</c> change alters how the string splits and so changes the snapshot.
///
/// Restricted to net10.0 (see the csproj) — the output is TFM-independent, so one modern-TFM pass
/// is sufficient and keeps a single shared snapshot per test.
/// </summary>
public class CsvSnapshotTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);


    private static readonly IReadOnlyList<PersonRecord> People = new[]
    {
        new PersonRecord { FirstName = "Alice", LastName = "Smith", Age = 30 },
        new PersonRecord { FirstName = "Bob", LastName = "Jones", Age = 25 },
        new PersonRecord { FirstName = "Carol", LastName = "White", Age = 42 },
    };


    [Fact]
    public async Task Default_settings_write_header_and_records()
    {
        var csv = await LoadToStringAsync(People);

        await Verifier.Verify(SplitLines(csv)).UseDirectory("Snapshots");
    }


    [Fact]
    public async Task Pipe_delimiter_and_no_header()
    {
        var csv = await LoadToStringAsync
        (
            People,
            loader =>
            {
                loader.Delimiter = "|";
                loader.HasHeaderRecord = false;
            }
        );

        await Verifier.Verify(SplitLines(csv)).UseDirectory("Snapshots");
    }


    [Fact]
    public async Task Fields_with_delimiter_quote_and_newline_are_quoted_and_escaped()
    {
        var tricky = new[]
        {
            new PersonRecord { FirstName = "Comma,Name", LastName = "Quote\"Name", Age = 1 },
            new PersonRecord { FirstName = "Line\r\nBreak", LastName = "Plain", Age = 2 },
            new PersonRecord { FirstName = "  Padded  ", LastName = "", Age = 3 },
        };

        var csv = await LoadToStringAsync(tricky);

        await Verifier.Verify(SplitLines(csv)).UseDirectory("Snapshots");
    }


    private static string[] SplitLines(string csv) => csv.Split(new[] { "\r\n" }, StringSplitOptions.None);


    private static async Task<string> LoadToStringAsync
    (
        IReadOnlyList<PersonRecord> records,
        Action<CsvLoader<PersonRecord>>? configure = null
    )
    {
        using var stream = new MemoryStream();

        var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<PersonRecord>(writer) { LeaveOpen = true };
        configure?.Invoke(loader);

        await loader.LoadAsync(ToAsync(records));
        await writer.FlushAsync();

        return Utf8NoBom.GetString(stream.ToArray());
    }


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
