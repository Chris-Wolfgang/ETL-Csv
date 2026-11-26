using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// End-to-end reads and writes through the library's own record binding (the CsvHelper 34 migration):
/// constructor-bound records, value types, defaults, write-side trimming and the discriminator column
/// lookup.
/// </summary>
public class CsvRecordBindingRoundTripTests
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);



    [Fact]
    public async Task ExtractAsync_binds_a_positional_record_through_its_constructor()
    {
        var rows = await ReadAllAsync(new CsvExtractor<PositionalRecord>(ReaderOver("Name,Count\r\nAlice,3\r\n")));

        Assert.Equal(new PositionalRecord("Alice", 3), Assert.Single(rows));
    }



    [Fact]
    public async Task ExtractAsync_when_a_constructor_column_is_absent_uses_the_parameter_default_or_the_type_default()
    {
        var sut = new CsvExtractor<OptionalParameterRecord>
        (
            ReaderOver("Name\r\nAlice\r\n"),
            new CsvExtractorOptions<OptionalParameterRecord>
            {
                ColumnMaps = new[]
                {
                    new CsvColumnMap(nameof(OptionalParameterRecord.Name)),
                    new CsvColumnMap(nameof(OptionalParameterRecord.Count)) { Optional = true },
                    new CsvColumnMap(nameof(OptionalParameterRecord.Note)) { Optional = true },
                },
            }
        );

        var row = Assert.Single(await ReadAllAsync(sut));

        Assert.Equal("Alice", row.Name);
        Assert.Equal(7, row.Count);
        Assert.Equal(0, row.Weight);
        Assert.Null(row.Note);
    }



    [Fact]
    public async Task ExtractAsync_reads_a_value_type_record()
    {
        var rows = await ReadAllAsync(new CsvExtractor<PointRecord>(ReaderOver("X,Y\r\n1,2\r\n")));

        Assert.Equal(new PointRecord { X = 1, Y = 2 }, Assert.Single(rows));
    }



    [Fact]
    public async Task ExtractAsync_when_the_type_has_no_public_constructor_throws_InvalidOperationException()
    {
        var sut = new CsvExtractor<NoPublicConstructorRecord>(ReaderOver("Value\r\nx\r\n"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await ReadAllAsync(sut));

        Assert.Contains("no public constructor", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public async Task ExtractAsync_when_a_field_is_empty_uses_the_column_Default()
    {
        var sut = new CsvExtractor<DefaultedRecord>(ReaderOver("Name,Count\r\nAlice,\r\n"));

        Assert.Equal(5, Assert.Single(await ReadAllAsync(sut)).Count);
    }



    [Fact]
    public async Task ExtractAsync_when_the_discriminator_column_is_not_in_the_header_reports_the_value_as_unmapped()
    {
        var sut = new CsvExtractor<KindRow>
        (
            ReaderOver("Other,Value\r\nA,1\r\n"),
            new CsvExtractorOptions<KindRow>
            {
                Discriminator = new CsvDiscriminatorBuilder<KindRow>("Kind")
                    .Map<KindRow>("A")
                    .Build(),
            }
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await ReadAllAsync(sut));

        Assert.Equal("No record type is mapped for discriminator value ''.", ex.Message);
    }



    [Fact]
    public async Task LoadAsync_when_TrimOptions_is_Trim_trims_the_written_fields()
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Utf8NoBom, 1024, leaveOpen: true))
        {
            var sut = new CsvLoader<DefaultedRecord>
            (
                writer,
                new CsvLoaderOptions<DefaultedRecord> { TrimOptions = CsvTrimOptions.Trim }
            );

            await sut.LoadAsync(new[] { new DefaultedRecord { Name = "  Alice  ", Count = 1 } }.ToAsyncEnumerable());
        }

        Assert.Equal("Name,Count\r\nAlice,1\r\n", Utf8NoBom.GetString(stream.ToArray()));
    }



    private static StreamReader ReaderOver(string content) => new(new MemoryStream(Utf8NoBom.GetBytes(content)), Utf8NoBom);



    private static async Task<List<T>> ReadAllAsync<T>(CsvExtractor<T> sut)
        where T : notnull
    {
        var list = new List<T>();
        await foreach (var item in sut.ExtractAsync())
        {
            list.Add(item);
        }

        return list;
    }



    public sealed record PositionalRecord(string Name, int Count);



    public sealed record OptionalParameterRecord(string Name, double Weight, int Count = 7, string? Note = null)
    {
        // A second, smaller public constructor: the binding picks the one with the most parameters.
        [ExcludeFromCodeCoverage] // never called: the binding uses the wider constructor
        public OptionalParameterRecord(string name, string note)
            : this(name, 0, 0, note)
        {
        }
    }



    public record struct PointRecord
    {
        public int X { get; set; }



        public int Y { get; set; }
    }



    public sealed class NoPublicConstructorRecord
    {
        [ExcludeFromCodeCoverage] // never called: the binding finds no public constructor
        private NoPublicConstructorRecord()
        {
        }



        [ExcludeFromCodeCoverage] // test POCO: never read, the record cannot be created
        public string Value { get; set; } = string.Empty;
    }



    public sealed record DefaultedRecord
    {
        public string Name { get; set; } = string.Empty;



        [CsvColumn(Default = "5")]
        public int Count { get; set; }
    }



    public record KindRow
    {
        [ExcludeFromCodeCoverage] // test POCO: the row is never bound
        public string Value { get; set; } = string.Empty;
    }
}
