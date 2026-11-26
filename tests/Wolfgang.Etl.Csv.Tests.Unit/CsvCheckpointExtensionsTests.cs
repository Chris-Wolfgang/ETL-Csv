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
    public static TheoryData<string> BlankPaths => new()
    {
        string.Empty,
        "   ",
    };


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
            Assert.Empty(TempFilesFor(path));
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
            var ex = await Assert.ThrowsAnyAsync<Exception>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, 1)
            );

            Assert.True(ex is IOException or UnauthorizedAccessException, ex.GetType().FullName);
            Assert.Empty(TempFilesFor(path));
        }
        finally
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
    }


    [Theory]
    [MemberData(nameof(BlankPaths))]
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
            await WriteRawAsync(path, "42".PadLeft(CsvCheckpointExtensions.MaxCheckpointBytes));

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
            await WriteRawAsync(path, "42".PadLeft(CsvCheckpointExtensions.MaxCheckpointBytes + 1));

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
    [MemberData(nameof(BlankPaths))]
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
            Assert.Empty(TempFilesFor(path));
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_leaves_a_stale_fixed_name_tmp_file_alone()
    {
        // A "<path>.tmp" left by an earlier version or another program is not the temp file this
        // write uses, so it is neither opened nor renamed over the checkpoint.
        var path = NewTempPath();
        var stale = path + ".tmp";
        try
        {
            await WriteRawAsync(stale, "stale");

            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 8);

            Assert.Equal(8, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
            using (var reader = new StreamReader(stale))
            {
                Assert.Equal("stale", await reader.ReadToEndAsync());
            }
            Assert.Empty(TempFilesFor(path));
        }
        finally
        {
            File.Delete(stale);
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_when_another_writer_holds_a_temp_file_open_still_succeeds()
    {
        // Another writer mid-write holds its temp file open with no sharing. With a fixed "<path>.tmp"
        // the second write would open that same file and fail with a sharing violation; with a name per
        // call it writes its own file and never touches the other one.
        var path = NewTempPath();
        var other = path + ".tmp";
        try
        {
            using (new FileStream(other, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await CsvCheckpointExtensions.WriteCheckpointAsync(path, 12);
            }

            Assert.Equal(12, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
            Assert.Empty(TempFilesFor(path));
        }
        finally
        {
            File.Delete(other);
            Cleanup(path);
        }
    }


    [Fact]
    public async Task WriteCheckpointAsync_when_count_is_negative_throws_and_writes_nothing()
    {
        var path = NewTempPath();
        try
        {
            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, -5)
            );

            Assert.Equal("count", ex.ParamName);
            Assert.False(File.Exists(path));
            Assert.Empty(TempFilesFor(path));
        }
        finally
        {
            Cleanup(path);
        }
    }


    [Fact]
    public async Task ReadCheckpointAsync_when_the_file_holds_a_negative_count_throws_FormatException()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "-5");

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
    public async Task WriteCheckpointAsync_when_count_is_zero_round_trips_zero()
    {
        var path = NewTempPath();
        try
        {
            await CsvCheckpointExtensions.WriteCheckpointAsync(path, 0);

            Assert.Equal(0, await CsvCheckpointExtensions.ReadCheckpointAsync(path));
        }
        finally
        {
            Cleanup(path);
        }
    }



    [Fact]
    public async Task WriteCheckpointAsync_when_count_is_negative_says_why_in_the_message()
    {
        var path = NewTempPath();
        try
        {
            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, -1)
            );

            Assert.Contains("must not be negative", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            Cleanup(path);
        }
    }



    [Fact]
    public async Task ReadCheckpointAsync_when_the_file_holds_a_negative_count_names_the_file_and_the_count()
    {
        var path = NewTempPath();
        try
        {
            await WriteRawAsync(path, "-1");

            var ex = await Assert.ThrowsAsync<FormatException>
            (
                async () => await CsvCheckpointExtensions.ReadCheckpointAsync(path)
            );

            Assert.Contains(path, ex.Message, StringComparison.Ordinal);
            Assert.Contains("negative count (-1)", ex.Message, StringComparison.Ordinal);
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
    }


    // The temp files WriteCheckpointAsync creates for path: "<name>.<random>.tmp" in the same folder.
    private static string[] TempFilesFor(string path) =>
        Directory.GetFiles(Path.GetDirectoryName(path)!, Path.GetFileName(path) + ".*.tmp");
}
