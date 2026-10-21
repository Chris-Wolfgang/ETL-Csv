using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Wolfgang.Etl.TestKit.Xunit;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins observable details of <see cref="CsvLoader{TRecord}"/>'s load loop that the contract and
/// round-trip tests do not look at: what it logs, how it aggregates
/// validator failures, and which shape it writes.
/// </summary>
public class CsvLoaderWorkerTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    private static readonly PersonRecord[] People =
    {
        new() { FirstName = "Alice", LastName = "Smith", Age = 30 },
        new() { FirstName = "Bob", LastName = "Jones", Age = 25 },
        new() { FirstName = "Carol", LastName = "White", Age = 35 },
    };



    [Fact]
    public async Task LoadAsync_logs_start_skip_load_limit_and_completion()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var logger = new RecordingLogger<CsvLoader<PersonRecord>>();
        var sut = new CsvLoader<PersonRecord>
        (
            writer,
            new CsvLoaderOptions<PersonRecord>
            {
                LeaveOpen = true,
                SkipItemCount = 1,
                MaximumItemCount = 1,
            },
            logger
        );

        await sut.LoadAsync(People.ToAsyncEnumerable());

        Assert.Equal
        (
            new[]
            {
                "Starting CSV loading of PersonRecord.",
                "Skipped item 1 of 1.",
                "Loaded item 1.",
                "Reached MaximumItemCount of 1. Stopping.",
                "CSV loading completed. Loaded: 1, skipped: 1.",
            },
            logger.Entries.Select(e => e.Message)
        );
    }



    [Fact]
    public void Constructor_without_options_leaves_the_writer_open()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);

        var sut = new CsvLoader<PersonRecord>(writer);

        Assert.True(sut.LeaveOpen);
    }



    [Fact]
    public async Task LoadAsync_aggregates_the_failures_of_every_failing_validator()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        CsvInvalidRecord<PersonRecord>? captured = null;
        var sut = new CsvLoader<PersonRecord>
        (
            writer,
            new CsvLoaderOptions<PersonRecord>
            {
                LeaveOpen = true,
                Validators = new CsvValidator<PersonRecord>[]
                {
                    _ => CsvValidationResult.Fail("first"),
                    _ => CsvValidationResult.Fail("second"),
                },
                OnValidationFailure = CsvValidationFailureAction.Skip,
                InvalidRecordHandler = invalid => captured = invalid,
            }
        );

        await sut.LoadAsync(People.Take(1).ToAsyncEnumerable());

        Assert.Equal(new[] { "first", "second" }, captured!.Failures);
    }



    [Fact]
    public async Task LoadAsync_when_ColumnMaps_is_empty_writes_the_default_shape()
    {
        // An empty list means "no runtime maps"; it must not reach BuildFromColumnMaps, which
        // rejects an empty list.
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var sut = new CsvLoader<PersonRecord>
        (
            writer,
            new CsvLoaderOptions<PersonRecord>
            {
                LeaveOpen = true,
                ColumnMaps = Array.Empty<CsvColumnMap>(),
            }
        );

        await sut.LoadAsync(People.Take(1).ToAsyncEnumerable());
        await writer.FlushAsync();

        Assert.Equal("FirstName,LastName,Age\r\nAlice,Smith,30\r\n", Utf8NoBom.GetString(stream.ToArray()));
    }



    [Fact]
    public async Task LoadAsync_without_a_discriminator_writes_a_derived_record_as_the_declared_type()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var sut = new CsvLoader<PersonRecord>
        (
            writer,
            new CsvLoaderOptions<PersonRecord>
            {
                LeaveOpen = true,
                HasHeaderRecord = false,
            }
        );
        var items = new PersonRecord[] { new EmployeeRecord { FirstName = "Dan", LastName = "Green", Age = 40, Badge = "B7" } };

        await sut.LoadAsync(items.ToAsyncEnumerable());
        await writer.FlushAsync();

        Assert.Equal("Dan,Green,40\r\n", Utf8NoBom.GetString(stream.ToArray()));
    }



    [Fact]
    public async Task LoadAsync_with_an_injected_timer_reports_progress_each_time_it_fires()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true);
        var timer = new ManualProgressTimer();
        var sut = new CsvLoader<PersonRecord>(writer, timer);
        var progress = new SyncProgress<CsvLoaderProgress>();
        var reportsPerFire = new List<int>();

        await sut.LoadAsync(FireBetweenRecords(People, timer, progress, reportsPerFire), progress);

        // Each Fire produced exactly one report: the Elapsed handler is wired once.
        Assert.Equal(new[] { 1, 1, 1 }, reportsPerFire);
    }



    private static async IAsyncEnumerable<PersonRecord> FireBetweenRecords
    (
        IEnumerable<PersonRecord> items,
        ManualProgressTimer timer,
        SyncProgress<CsvLoaderProgress> progress,
        List<int> reportsPerFire
    )
    {
        foreach (var item in items)
        {
            await Task.Yield();
            var before = progress.CallCount;
            timer.Fire();
            reportsPerFire.Add(progress.CallCount - before);
            yield return item;
        }
    }



    public record EmployeeRecord : PersonRecord
    {
        public string Badge { get; set; } = string.Empty;
    }
}
