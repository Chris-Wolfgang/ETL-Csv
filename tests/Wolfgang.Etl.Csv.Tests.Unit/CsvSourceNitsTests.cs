using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// Pins the argument-validation and defensive-copy behaviour tightened in #590: eager path and
/// <c>InitialRecordIndex</c> checks on the pipeline factories, duplicate rejection on the direct-init
/// <see cref="CsvDiscriminator{TBase}"/>, the copied <see cref="CsvValidationResult.Fail"/> array,
/// the extractor's tolerance of a null <see cref="CsvValidationResult.Failures"/>, and the checkpoint
/// writer's temp-file cleanup when the write itself fails.
/// </summary>
public sealed class CsvSourceNitsTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CsvExtractor_path_factory_when_path_is_whitespace_throws_ArgumentException(string path)
    {
        var ex = Assert.Throws<ArgumentException>
        (
            () => EtlPipeline
                .Create()
                .CsvExtractor<PersonRecord>(path)
        );

        Assert.Equal("path", ex.ParamName);
    }



    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CsvLoader_path_terminator_when_path_is_whitespace_throws_ArgumentException(string path)
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\n");
        var pipeline = EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(reader);

        var ex = Assert.Throws<ArgumentException>(() => pipeline.CsvLoader(path));

        Assert.Equal("path", ex.ParamName);
    }



    [Fact]
    public void InitialRecordIndex_when_less_than_one_on_options_route_throws_naming_the_argument()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\n");
        var builder = EtlPipeline
            .Create()
            .CsvExtractor<PersonRecord>(reader);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.InitialRecordIndex(0));

        Assert.Equal("oneBasedIndex", ex.ParamName);
    }



    [Fact]
    public void InitialRecordIndex_when_less_than_one_on_existing_extractor_route_throws_at_the_call()
    {
        using var reader = ReaderOver("FirstName,LastName,Age\r\n");
        var extractor = new CsvExtractor<PersonRecord>(reader);
        var builder = EtlPipeline
            .Create()
            .CsvExtractor(extractor);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.InitialRecordIndex(0));

        Assert.Equal("oneBasedIndex", ex.ParamName);
    }



    [Fact]
    public void TryResolveType_when_direct_init_keys_collide_under_Comparer_throws_InvalidOperationException()
    {
        // Mapping is ordinal, so "A" and "a" are distinct keys there but collide under the default
        // OrdinalIgnoreCase Comparer.
        var discriminator = new CsvDiscriminator<object>
        {
            ColumnIndex = 0,
            Mapping = new Dictionary<string, Type>(StringComparer.Ordinal)
            {
                ["A"] = typeof(string),
                ["a"] = typeof(Uri),
            },
        };

        var ex = Assert.Throws<InvalidOperationException>(() => discriminator.TryResolveType("A", out _));

        Assert.Contains("'a'", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void TryResolveValue_when_direct_init_maps_one_type_twice_throws_InvalidOperationException()
    {
        var discriminator = new CsvDiscriminator<object>
        {
            ColumnIndex = 0,
            Mapping = new Dictionary<string, Type>(StringComparer.Ordinal)
            {
                ["A"] = typeof(string),
                ["B"] = typeof(string),
            },
        };

        var ex = Assert.Throws<InvalidOperationException>(() => discriminator.TryResolveValue(typeof(string), out _));

        Assert.Contains(typeof(string).ToString(), ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void Direct_init_discriminator_without_duplicates_resolves_both_directions()
    {
        var discriminator = new CsvDiscriminator<object>
        {
            ColumnIndex = 0,
            Mapping = new Dictionary<string, Type>(StringComparer.Ordinal)
            {
                ["A"] = typeof(string),
                ["B"] = typeof(Uri),
            },
        };

        Assert.True(discriminator.TryResolveType("b", out var type));
        Assert.Equal(typeof(Uri), type);
        Assert.True(discriminator.TryResolveValue(typeof(string), out var value));
        Assert.Equal("A", value);
    }



    [Fact]
    public void Fail_when_caller_mutates_the_reasons_array_afterwards_keeps_the_original_failures()
    {
        var reasons = new[] { "first", "second" };

        var result = CsvValidationResult.Fail(reasons);
        reasons[0] = "changed";

        Assert.Equal(new[] { "first", "second" }, result.Failures);
    }



    [Fact]
    public async Task ExtractAsync_when_a_validator_returns_null_Failures_still_routes_the_invalid_record()
    {
        var invalid = new List<CsvInvalidRecord<PersonRecord>>();
        using var reader = ReaderOver("FirstName,LastName,Age\r\nAlice,Smith,30\r\n");
        var extractor = new CsvExtractor<PersonRecord>
        (
            reader,
            new CsvExtractorOptions<PersonRecord>
            {
                Validators = new CsvValidator<PersonRecord>[]
                {
                    _ => CsvValidationResult.Fail("x") with { Failures = null! },
                },
                OnValidationFailure = CsvValidationFailureAction.Skip,
                InvalidRecordHandler = invalid.Add,
            }
        );

        var yielded = await extractor
            .ExtractAsync()
            .ToListAsync();

        Assert.Empty(yielded);
        var record = Assert.Single(invalid);
        Assert.Empty(record.Failures);
    }



    [Fact]
    public async Task WriteCheckpointAsync_when_the_temp_file_cannot_be_written_rethrows()
    {
        // A directory squatting on the ".tmp" name makes the FileStream open fail inside the guarded
        // write; the cleanup path runs (best effort) and the original exception propagates.
        var path = Path.Combine(Path.GetTempPath(), "csvchk-" + Guid.NewGuid().ToString("N") + ".txt");
        var squatter = Directory.CreateDirectory(path + ".tmp");

        try
        {
            // Windows reports UnauthorizedAccessException; some Unix runtimes report IOException.
            var ex = await Assert.ThrowsAnyAsync<Exception>
            (
                async () => await CsvCheckpointExtensions.WriteCheckpointAsync(path, 7)
            );

            Assert.True(ex is UnauthorizedAccessException or IOException, ex.GetType().FullName);

            Assert.False(File.Exists(path));
        }
        finally
        {
            squatter.Delete();
        }
    }



    private static StreamReader ReaderOver(string content) => new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);
}
