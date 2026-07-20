using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

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
            .CsvLoader<PersonRecord>(target)
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
            .CsvLoader<PersonRecord>(targetWriter)
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
        var loader = new CsvLoader<PersonRecord>(targetWriter) { LeaveOpen = true };

        await EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(extractor)
            .CsvLoader<PersonRecord>(loader)
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
            .CsvLoader<PersonRecord>(target)
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
            .CsvLoader<PersonRecord>(target)
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
            .CsvLoader<PersonRecord>(target)
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
            .CsvLoader<PersonRecord>(target)
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
            .CsvLoader<PersonRecord>(target)
            .RunAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => run);

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

        Assert.Throws<ArgumentNullException>(() => ((EtlPipeline)null!).CsvExtractor<PersonRecord>("x.csv"));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor<PersonRecord>((string)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor<PersonRecord>((StreamReader)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvExtractor<PersonRecord>((CsvExtractor<PersonRecord>)null!));
    }


    [Fact]
    public void CsvLoader_terminators_reject_null_arguments()
    {
        var source = WriteTempFile("guard.csv", "FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var pipeline = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);

        Assert.Throws<ArgumentNullException>(() => ((IEtlPipeline<PersonRecord>)null!).CsvLoader<PersonRecord>("x.csv"));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader<PersonRecord>((string)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader<PersonRecord>((StreamWriter)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.CsvLoader<PersonRecord>((CsvLoader<PersonRecord>)null!));
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


    private static async IAsyncEnumerable<PersonRecord> UppercaseLastNames(IAsyncEnumerable<PersonRecord> source)
    {
        await foreach (var person in source.ConfigureAwait(false))
        {
            yield return person with { LastName = person.LastName.ToUpperInvariant() };
        }
    }


    private static async IAsyncEnumerable<PersonRecord> ThrowOnFirst(IAsyncEnumerable<PersonRecord> source)
    {
        await foreach (var _ in source.ConfigureAwait(false))
        {
            throw new InvalidOperationException("boom");
        }

        yield break;
    }


    private string WriteTempFile(string name, string content)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content, Utf8NoBom);
        return path;
    }


    private static StreamReader ReaderOver(string content) => new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);
}
