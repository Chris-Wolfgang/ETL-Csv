using System;
using System.IO;
using System.Text;
using System.Threading;
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


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ReadCheckpointAsync_when_path_is_empty_or_whitespace_throws_ArgumentException(string path)
    {
        await Assert.ThrowsAsync<ArgumentException>
        (
            async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
        );
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_file_is_too_large_throws_FormatException()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, new string('9', 200));

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
    public async Task ResumeFromCheckpointAsync_when_extractor_is_null_throws_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>
        (
            async () => await CsvCheckpointExtensions.ResumeFromCheckpointAsync<PersonRecord>(null!, NewTempPath())
        );
    }


    [Fact]
    public async Task WriteCheckpointAsync_when_the_atomic_replace_fails_cleans_up_the_temp_file()
    {
        // A directory at the target path makes the atomic replace throw, exercising the
        // best-effort temp-file cleanup on the failure path.
        var path = NewTempPath();
        Directory.CreateDirectory(path);
        try
        {
            // The atomic replace throws IOException (Linux) or UnauthorizedAccessException (Windows) —
            // WriteCheckpointAsync catches both. The temp file being gone proves the cleanup ran.
            await Assert.ThrowsAnyAsync<Exception>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, 1)
            );

            Assert.False(File.Exists(path + ".tmp"), "the temp file should be cleaned up when the replace fails");
        }
        finally
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }

            // File.Delete is a no-op when the file does not exist.
            File.Delete(path + ".tmp");
        }
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ReadCheckpointAsync_when_path_is_empty_or_whitespace_reports_the_path_parameter(string path)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>
        (
            async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
        );

        Assert.Equal("path", ex.ParamName);
        Assert.StartsWith("Path must not be empty or whitespace.", ex.Message, StringComparison.Ordinal);
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_token_is_cancelled_throws_before_touching_the_file()
    {
        // The file does not exist, so without the up-front cancellation check the call would
        // quietly return 0.
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () => await CsvCheckpointExtensions.ReadCheckpointAsync(NewTempPath(), cts.Token)
        );
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_another_reader_holds_the_file_still_reads_it()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "12");

            using var holder = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

            Assert.Equal(12, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_file_is_exactly_the_size_cap_reads_it()
    {
        // 64 bytes is the cap; a file of exactly that size is still accepted.
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "42".PadLeft(64));

            Assert.Equal(42, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_a_valid_integer_is_padded_past_the_size_cap_throws_FormatException()
    {
        // The content would parse; only the size check rejects it.
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "42".PadLeft(65));

            var ex = await Assert.ThrowsAsync<FormatException>
            (
                async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
            );

            Assert.Contains("65 bytes", ex.Message, StringComparison.Ordinal);
            Assert.Contains("too large to be a checkpoint integer", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_content_is_not_an_integer_names_the_file_in_the_message()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "not-a-number");

            var ex = await Assert.ThrowsAsync<FormatException>
            (
                async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
            );

            Assert.Equal
            (
                $"Checkpoint file '{path}' does not contain a valid integer.",
                ex.Message
            );
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_when_path_is_null_throws_ArgumentNullException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>
        (
            async () => await CsvCheckpointExtensions.WriteCheckpointAsync(null!, 1)
        );

        Assert.Equal("path", ex.ParamName);
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task WriteCheckpointAsync_when_path_is_empty_or_whitespace_throws_ArgumentException(string path)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>
        (
            async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, 1)
        );

        Assert.Equal("path", ex.ParamName);
        Assert.StartsWith("Path must not be empty or whitespace.", ex.Message, StringComparison.Ordinal);
    }


    [Fact]
    public async Task WriteCheckpointAsync_when_the_token_is_cancelled_throws_and_creates_no_file()
    {
        var path = NewTempPath();
        try
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, 3, cts.Token)
            );

            Assert.False(File.Exists(path), "a cancelled write should not create the checkpoint");
            Assert.False(File.Exists(path + ".tmp"), "a cancelled write should not create the temp file");
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_writes_through_the_tmp_file_and_replaces_a_stale_one()
    {
        // A stale "<path>.tmp" from an earlier crash is overwritten and renamed away, which only
        // happens when the write really goes through "<path>.tmp".
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path + ".tmp", "stale");

            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 8);

            Assert.False(File.Exists(path + ".tmp"), "the stale temp file should be replaced and renamed away");
            Assert.Equal(8, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
        }
        finally
        {
            Cleanup(path);
        }
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

        // File.Delete is a no-op when the file does not exist.
        File.Delete(path + ".tmp");
    }
}
