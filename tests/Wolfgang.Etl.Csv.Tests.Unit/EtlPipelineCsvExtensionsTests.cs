using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

// These files still configure via the deprecated property setters in places where the value is
// applied after construction, so it cannot travel through the options constructor without
// restructuring the test. They keep exercising the setter path until the setters are removed.
#pragma warning disable CS0618

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Tests for the class-named <see cref="EtlPipelineCsvExtensions"/> source factories and sink
/// terminators that hang CSV extraction/loading off the generic <see cref="EtlPipeline"/> chain.
/// Pipeline operators are supplied as inline <c>Through</c> stages so the tests take no dependency
/// on the LINQ-flavored operators shipped by <c>Wolfgang.Etl.Transformers</c>.
/// </summary>
public sealed class EtlPipelineCsvExtensionsTests : IDisposable
{
    private readonly string _tempDir;


    public EtlPipelineCsvExtensionsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "etl-csv-pipeline-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }


    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup; a leaked handle would surface as its own test failure.
        }
    }


    [Fact]
    public async Task CsvExtractor_from_path_through_two_stages_to_CsvLoader_path_writes_expected_csv()
    {
        var source = WriteTempFile
        (
            "source.csv",
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\nCarol,White,35\r\n"
        );
        var target = Path.Combine(_tempDir, "target.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .Through(FilterAdults)
            .Through(UppercaseLastNames)
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,SMITH,30\r\nCarol,WHITE,35\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task CsvExtractor_from_reader_and_CsvLoader_to_writer_round_trips_without_disposing_caller_streams()
    {
        using var sourceReader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        using var targetStream = new MemoryStream();
        using var targetWriter = new StreamWriter(targetStream, Utf8NoBom, 1024, leaveOpen: true);

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(sourceReader)
            .CsvLoader(targetWriter)
            .RunAsync();

        targetWriter.Flush();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            Utf8NoBom.GetString(targetStream.ToArray())
        );

        // Caller-owned streams are left open — writing again must not throw.
        targetWriter.Write("still-open");
        targetWriter.Flush();
    }


    [Fact]
    public async Task CsvExtractor_from_existing_extractor_and_CsvLoader_from_existing_loader_run()
    {
        using var sourceReader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var extractor = new CsvExtractor<PersonRecord>(sourceReader);

        using var targetStream = new MemoryStream();
        using var targetWriter = new StreamWriter(targetStream, Utf8NoBom, 1024, leaveOpen: true);
        var loader = new CsvLoader<PersonRecord>(targetWriter, new CsvLoaderOptions<PersonRecord>
        { LeaveOpen = true});

        await EtlPipeline
            .Create()
            .CsvExtractor(extractor)
            .CsvLoader(loader)
            .RunAsync();

        targetWriter.Flush();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            Utf8NoBom.GetString(targetStream.ToArray())
        );
    }


    [Fact]
    public async Task Extractor_and_loader_delimiter_and_no_header_setters_round_trip()
    {
        var source = WriteTempFile("in.psv", "Alice|Smith|30\r\nBob|Jones|25\r\n");
        var target = Path.Combine(_tempDir, "out.psv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .Delimiter("|")
            .HasHeaderRecord(false)
            .CsvLoader(target)
            .Delimiter("|")
            .HasHeaderRecord(false)
            .RunAsync();

        Assert.Equal
        (
            "Alice|Smith|30\r\nBob|Jones|25\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Extractor_SkipRecordCount_and_MaxRecordCount_bound_the_window()
    {
        var source = WriteTempFile
        (
            "window.csv",
            "FirstName,LastName,Age\r\nA,A,1\r\nB,B,2\r\nC,C,3\r\nD,D,4\r\n"
        );
        var target = Path.Combine(_tempDir, "window-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .SkipRecordCount(1)
            .MaxRecordCount(2)
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nB,B,2\r\nC,C,3\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task CsvLoader_Encoding_setter_binds_the_output_stream_encoding()
    {
        var source = WriteTempFile("enc.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "enc-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .CsvLoader(target)
            .Encoding(new UTF8Encoding(encoderShouldEmitUTF8Identifier: true))
            .RunAsync();

        var bytes = File.ReadAllBytes(target);

        Assert.True
        (
            bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
            "Expected a UTF-8 BOM, proving the loader used the configured encoding to open the file."
        );
    }


    [Fact]
    public async Task Path_based_source_and_sink_release_their_file_handles_after_a_successful_run()
    {
        var source = WriteTempFile("release.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "release-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .CsvLoader(target)
            .RunAsync();

        // A locked file would throw IOException here.
        File.Delete(source);
        File.Delete(target);

        Assert.False(File.Exists(source));
        Assert.False(File.Exists(target));
    }


    [Fact]
    public async Task Path_based_source_releases_its_file_handle_after_a_faulted_run()
    {
        var source = WriteTempFile("fault.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "fault-out.csv");

        var run = EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .Through(ThrowOnFirst)
            .CsvLoader(target)
            .RunAsync();

        // Awaiting a Task not started in this context is the exact test shape
        // xunit intends for RunAsync-style faulted-run assertions.
#pragma warning disable VSTHRD003
        await Assert.ThrowsAsync<InvalidOperationException>(() => run);
#pragma warning restore VSTHRD003

        // The reader must have been disposed even though the pipeline threw.
        File.Delete(source);
        Assert.False(File.Exists(source));
    }


    [Fact]
    public void First_pipeline_operator_narrows_the_builder_off_the_configuration_surface()
    {
        var source = WriteTempFile("narrow.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");

        var builder = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);
        IEtlPipeline<PersonRecord> narrowed = builder.Through(FilterAdults);

        Assert.False(narrowed is ICsvExtractorBuilder<PersonRecord>);
    }


    [Fact]
    public void Configuring_a_caller_supplied_extractor_leaves_its_other_properties_alone()
    {
        // Regression guard: the builder folds configuration into an options record for sources it
        // constructs, but a caller-supplied extractor is already configured. Applying the whole
        // record to it would reset every property the caller set directly - here, Quote.
        var source = WriteTempFile("supplied.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        using var reader = new StreamReader(source);

        var extractor = new CsvExtractor<PersonRecord>(reader, new CsvExtractorOptions<PersonRecord>
        {
            Quote = '\'',
            HasHeaderRecord = false});

        _ = EtlPipeline
            .Create()
            .CsvExtractor(extractor)
            .Delimiter(";");

        Assert.Equal('\'', extractor.Quote);
        Assert.False(extractor.HasHeaderRecord);
    }


    [Fact]
    public void Configuring_the_extractor_after_it_is_materialized_throws()
    {
        var source = WriteTempFile("late.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");

        var builder = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);
        _ = builder.Through(FilterAdults);

        Assert.Throws<InvalidOperationException>(() => builder.Delimiter(";"));
    }


    [Fact]
    public void CsvExtractor_factories_reject_null_arguments()
    {
        var pipeline = EtlPipeline.Create();

        using var reader = ReaderOver("FirstName,LastName,Age\r\n");
        var extractor = new CsvExtractor<PersonRecord>(reader);

        // Null pipeline receiver — every overload.
        Assert.Throws<ArgumentNullException>(() => ((EtlPipeline)null!).CsvExtractor<PersonRecord>("x.csv"));
        Assert.Throws<ArgumentNullException>(() => ((EtlPipeline)null!).CsvExtractor<PersonRecord>(reader));
        Assert.Throws<ArgumentNullException>(() => ((EtlPipeline)null!).CsvExtractor(extractor));

        // Null argument — every overload.
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor<PersonRecord>((string)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor<PersonRecord>((StreamReader)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor((CsvExtractor<PersonRecord>)null!));
    }


    [Fact]
    public void CsvLoader_terminators_reject_null_arguments()
    {
        var source = WriteTempFile("guard.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var pipeline = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);

        using var writer = new StreamWriter(new MemoryStream(), Utf8NoBom);
        var loader = new CsvLoader<PersonRecord>(writer, new CsvLoaderOptions<PersonRecord>
        { LeaveOpen = true});

        // Null pipeline receiver — every overload.
        Assert.Throws<ArgumentNullException>(() => ((IEtlPipeline<PersonRecord>)null!).CsvLoader("x.csv"));
        Assert.Throws<ArgumentNullException>(() => ((IEtlPipeline<PersonRecord>)null!).CsvLoader(writer));
        Assert.Throws<ArgumentNullException>(() => ((IEtlPipeline<PersonRecord>)null!).CsvLoader(loader));

        // Null argument — every overload.
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader((string)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader((StreamWriter)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader((CsvLoader<PersonRecord>)null!));
    }


    [Fact]
    public async Task Extractor_scalar_setters_apply_and_round_trip()
    {
        // Data with no comments, blank lines, quotes, or surrounding whitespace, so
        // Quote/Escape/Comment/AllowComments/IgnoreBlankLines/TrimOptions/Encoding are all
        // exercised without altering the round-tripped bytes.
        var source = WriteTempFile("scalars.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n");
        var target = Path.Combine(_tempDir, "scalars-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .Quote('"')
            .Escape('"')
            .Comment('#')
            .AllowComments(true)
            .IgnoreBlankLines(true)
            .Encoding(Utf8NoBom)
            .TrimOptions(CsvTrimOptions.Trim)
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Extractor_InitialRecordIndex_skips_leading_lines()
    {
        // InitialRecordIndex is 1-based over raw lines; 2 makes line 2 the header, skipping the banner.
        var source = WriteTempFile("banner.csv", "BANNER LINE\r\nFirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "banner-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .InitialRecordIndex(2)
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Extractor_ColumnMaps_bind_columns_by_index()
    {
        // Headerless, positionally-ordered LastName,FirstName,Age remapped onto the record.
        var source = WriteTempFile("maps.csv", "Smith,Alice,30\r\n");
        var target = Path.Combine(_tempDir, "maps-out.csv");

        var maps = new List<CsvColumnMap>
        {
            new(nameof(PersonRecord.LastName)) { Index = 0 },
            new(nameof(PersonRecord.FirstName)) { Index = 1 },
            new(nameof(PersonRecord.Age)) { Index = 2 },
        };

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .HasHeaderRecord(false)
            .ColumnMaps(maps)
            .CsvLoader(target)
            .HasHeaderRecord(false)
            .RunAsync();

        Assert.Equal("Alice,Smith,30\r\n", File.ReadAllText(target));
    }


    [Fact]
    public async Task Extractor_BadDataFound_and_ReadingExceptionOccurred_handlers_are_accepted()
    {
        var source = WriteTempFile("handlers.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "handlers-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .BadDataFound(_ => { })
            .ReadingExceptionOccurred(_ => { })
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Extractor_AsAsyncEnumerable_yields_configured_records()
    {
        var source = WriteTempFile("enum.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n");

        var names = new List<string>();

        await foreach (var person in EtlPipeline.Create().CsvExtractor<PersonRecord>(source).AsAsyncEnumerable())
        {
            names.Add(person.FirstName);
        }

        Assert.Equal(new[] { "Alice", "Bob" }, names);
    }


    [Fact]
    public async Task Extractor_To_loader_terminates_the_pipeline()
    {
        var source = WriteTempFile("to.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "to-out.csv");

        var loader = new CsvLoader<PersonRecord>(new StreamWriter(target, append: false, Utf8NoBom)) { LeaveOpen = false };

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .To(loader)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Extractor_Through_cancellation_aware_stage_runs()
    {
        var source = WriteTempFile("through-ct.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n");
        var target = Path.Combine(_tempDir, "through-ct-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .Through(FilterAdultsWithCancellation)
            .CsvLoader(target)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public void Extractor_setters_reject_null_arguments()
    {
        var source = WriteTempFile("xguard.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var builder = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);

        Assert.Throws<ArgumentNullException>(() => builder.Delimiter(null!));
        Assert.Throws<ArgumentNullException>(() => builder.Encoding(null!));
        Assert.Throws<ArgumentNullException>(() => builder.ColumnMaps(null!));
        Assert.Throws<ArgumentNullException>(() => builder.BadDataFound(null!));
        Assert.Throws<ArgumentNullException>(() => builder.ReadingExceptionOccurred(null!));
    }


    [Fact]
    public async Task Loader_scalar_setters_apply_and_round_trip()
    {
        var source = WriteTempFile("lscalars.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "lscalars-out.csv");

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .CsvLoader(target)
            .Quote('"')
            .Escape('"')
            .NewLine("\r\n")
            .TrimOptions(CsvTrimOptions.None)
            .ShouldQuote(_ => false)
            .RunAsync();

        Assert.Equal
        (
            "FirstName,LastName,Age\r\nAlice,Smith,30\r\n",
            File.ReadAllText(target)
        );
    }


    [Fact]
    public async Task Loader_ColumnMaps_control_output_columns()
    {
        var source = WriteTempFile("lmaps.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var target = Path.Combine(_tempDir, "lmaps-out.csv");

        var maps = new List<CsvColumnMap>
        {
            new(nameof(PersonRecord.LastName)) { Index = 0 },
            new(nameof(PersonRecord.FirstName)) { Index = 1 },
            new(nameof(PersonRecord.Age)) { Index = 2 },
        };

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .CsvLoader(target)
            .HasHeaderRecord(false)
            .ColumnMaps(maps)
            .RunAsync();

        Assert.Equal("Smith,Alice,30\r\n", File.ReadAllText(target));
    }


    [Fact]
    public void Loader_setters_reject_null_arguments()
    {
        var source = WriteTempFile("lguard.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var loader = EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .CsvLoader(Path.Combine(_tempDir, "lguard-out.csv"));

        Assert.Throws<ArgumentNullException>(() => loader.Delimiter(null!));
        Assert.Throws<ArgumentNullException>(() => loader.NewLine(null!));
        Assert.Throws<ArgumentNullException>(() => loader.Encoding(null!));
        Assert.Throws<ArgumentNullException>(() => loader.ShouldQuote(null!));
        Assert.Throws<ArgumentNullException>(() => loader.ColumnMaps(null!));
    }


    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);


    private static async IAsyncEnumerable<PersonRecord> FilterAdults(IAsyncEnumerable<PersonRecord> source)
    {
        await foreach (var person in source.ConfigureAwait(false))
        {
            if (person.Age >= 30)
            {
                yield return person;
            }
        }
    }


    private static async IAsyncEnumerable<PersonRecord> FilterAdultsWithCancellation
    (
        IAsyncEnumerable<PersonRecord> source,
        [EnumeratorCancellation] CancellationToken token
    )
    {
        await foreach (var person in source.WithCancellation(token).ConfigureAwait(false))
        {
            if (person.Age >= 30)
            {
                yield return person;
            }
        }
    }


    private static async IAsyncEnumerable<PersonRecord> UppercaseLastNames(IAsyncEnumerable<PersonRecord> source)
    {
        await foreach (var person in source.ConfigureAwait(false))
        {
            yield return person with { LastName = person.LastName.ToUpperInvariant() };
        }
    }


    // Single-iteration loop is intentional: throw on the first element to
    // exercise the pipeline's faulted-run cleanup path.
#pragma warning disable S1751
    private static async IAsyncEnumerable<PersonRecord> ThrowOnFirst(IAsyncEnumerable<PersonRecord> source)
    {
        await foreach (var _ in source.ConfigureAwait(false))
        {
            throw new InvalidOperationException("boom");
        }

        yield break;
    }
#pragma warning restore S1751


    private string WriteTempFile(string name, string content)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content, Utf8NoBom);
        return path;
    }


    private static StreamReader ReaderOver(string content) => new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);
}
