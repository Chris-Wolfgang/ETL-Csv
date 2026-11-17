using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the lifetime of the files the path-based <c>CsvExtractor</c> / <c>CsvLoader</c> pipeline
/// builders open: the source only while it is enumerated, the destination only replaced on success.
/// </summary>
public sealed class EtlPipelineCsvPathBuilderTests : IDisposable
{
    private const string Content = "FirstName,LastName,Age\r\nAlice,Smith,30\r\nBob,Jones,25\r\n";

    private readonly string _tempDir;



    public EtlPipelineCsvPathBuilderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "etl-csv-path-builder-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }



    public void Dispose()
    {
        // No try/catch: a leaked file handle must surface as a test failure, not be swallowed.
        Directory.Delete(_tempDir, recursive: true);
    }



    [Fact]
    public void A_composed_path_pipeline_that_is_never_run_does_not_hold_the_source_open()
    {
        var source = WriteFile("src.csv", Content);

        _ = EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(source)
            .AsAsyncEnumerable();

        File.Delete(source);
        Assert.False(File.Exists(source));
    }



    [Fact]
    public async Task A_sink_that_fails_to_open_does_not_leave_the_source_open()
    {
        var source = WriteFile("src.csv", Content);
        var missingDirectory = Path.Combine(_tempDir, "missing-dir", "out.csv");

#pragma warning disable VSTHRD003
        await Assert.ThrowsAsync<DirectoryNotFoundException>
        (
            () => EtlPipeline
                .Create()
                .CsvExtractor<PersonRecord>(source)
                .CsvLoader(missingDirectory)
                .RunAsync()
        );
#pragma warning restore VSTHRD003

        File.Delete(source);
        Assert.False(File.Exists(source));
    }



    [Fact]
    public async Task A_run_with_an_already_cancelled_token_does_not_leave_the_source_open()
    {
        var source = WriteFile("src.csv", Content);
        var target = Path.Combine(_tempDir, "out.csv");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => EtlPipeline
                .Create()
                .CsvExtractor<PersonRecord>(source)
                .CsvLoader(target)
                .RunAsync(null, cts.Token)
        );

        File.Delete(source);
        Assert.False(File.Exists(source));
        Assert.False(File.Exists(target));
        Assert.Empty(Directory.GetFiles(_tempDir, "*.tmp"));
    }



    [Fact]
    public async Task A_path_source_pipeline_can_be_enumerated_more_than_once()
    {
        var source = WriteFile("src.csv", Content);
        var pipeline = EtlPipeline.Create().CsvExtractor<PersonRecord>(source);

        var first = await CountAsync(pipeline.AsAsyncEnumerable());
        var second = await CountAsync(pipeline.AsAsyncEnumerable());

        Assert.Equal(2, first);
        Assert.Equal(2, second);
    }



    [Fact]
    public async Task A_run_whose_source_cannot_be_opened_leaves_an_existing_destination_untouched()
    {
        var target = WriteFile("out.csv", "PRECIOUS,DATA\r\n1,2\r\n");

        await Assert.ThrowsAsync<FileNotFoundException>
        (
            () => EtlPipeline
                .Create()
                .CsvExtractor<PersonRecord>(Path.Combine(_tempDir, "missing.csv"))
                .CsvLoader(target)
                .RunAsync()
        );

        Assert.Equal("PRECIOUS,DATA\r\n1,2\r\n", File.ReadAllText(target));
        Assert.Empty(Directory.GetFiles(_tempDir, "*.tmp"));
    }



    [Fact]
    public async Task A_second_RunAsync_throws_and_keeps_the_first_runs_output()
    {
        var source = WriteFile("src.csv", Content);
        var target = Path.Combine(_tempDir, "out.csv");
        var sink = EtlPipeline.Create().CsvExtractor<PersonRecord>(source).CsvLoader(target);

        await sink.RunAsync();
        var firstOutput = File.ReadAllText(target);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sink.RunAsync());

        Assert.Equal("The loader has already been run; RunAsync can only be called once per builder.", ex.Message);
        Assert.Equal(firstOutput, File.ReadAllText(target));
        Assert.Contains("Alice", firstOutput);
    }



    [Fact]
    public async Task A_successful_run_replaces_an_existing_destination_and_leaves_no_temporary_file()
    {
        var source = WriteFile("src.csv", Content);
        var target = WriteFile("out.csv", "OLD\r\n");

        await EtlPipeline.Create().CsvExtractor<PersonRecord>(source).CsvLoader(target).RunAsync();

        Assert.Equal(Content, File.ReadAllText(target));
        Assert.Empty(Directory.GetFiles(_tempDir, "*.tmp"));
    }



    private string WriteFile(string name, string content)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content);
        return path;
    }



    private static async Task<int> CountAsync(IAsyncEnumerable<PersonRecord> items)
    {
        var count = 0;
        await foreach (var _ in items)
        {
            count++;
        }

        return count;
    }
}
