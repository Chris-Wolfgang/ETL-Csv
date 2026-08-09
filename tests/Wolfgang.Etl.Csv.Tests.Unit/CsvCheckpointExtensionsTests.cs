using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvCheckpointExtensionsTests
{
    [Fact]
    public async Task ReadCheckpointAsync_when_the_file_is_missing_returns_zero()
    {
        var path = NewTempPath();

        Assert.Equal(0, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
    }


    [Fact]
    public async Task WriteCheckpointAsync_then_ReadCheckpointAsync_round_trips_the_value()
    {
        var path = NewTempPath();
        try
        {
            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 42);

            Assert.Equal(42, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_overwrites_an_existing_value_and_leaves_no_temp_file()
    {
        var path = NewTempPath();
        try
        {
            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 5);
            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 10);

            Assert.Equal(10, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
            Assert.False(File.Exists(path + ".tmp"), "the temp file should be renamed away, not left behind");
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_content_is_not_an_integer_throws_FormatException()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "not-a-number");

            await Assert.ThrowsAsync<FormatException>
            (
                async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
            );
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ResumeFromCheckpointAsync_sets_SkipRecordCount_and_returns_the_count()
    {
        var path = NewTempPath();
        try
        {
            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 7);

            using var reader = new StreamReader(new MemoryStream(Array.Empty<byte>()));
            var extractor = new CsvExtractor<PersonRecord>(reader);

            var resumed = await extractor.ResumeFromCheckpointAsync(path);

            Assert.Equal(7, resumed);
            Assert.Equal(7, extractor.SkipRecordCount);
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_path_is_null_throws_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>
        (
            async () => await CsvCheckpointExtensions.ReadCheckpointAsync(null!)
        );
    }


    [Fact]
    public async Task ResumeFromCheckpointAsync_when_extractor_is_null_throws_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>
        (
            async () => await CsvCheckpointExtensions.ResumeFromCheckpointAsync<PersonRecord>(null!, NewTempPath())
        );
    }


    private static string NewTempPath() =>
        Path.Combine(Path.GetTempPath(), "csvchk-" + Guid.NewGuid().ToString("N") + ".txt");


    private static async Task WriteRawAsync(string path, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
    }


    private static void Cleanup(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (File.Exists(path + ".tmp"))
        {
            File.Delete(path + ".tmp");
        }
    }
}
