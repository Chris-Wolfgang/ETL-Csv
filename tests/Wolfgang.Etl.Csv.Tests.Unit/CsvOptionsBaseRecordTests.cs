using System;
using System.IO;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Csv.Tests.Unit.TestModels;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

/// <summary>
/// The options records inherit the per-stage-kind base records from Wolfgang.Etl.Abstractions 0.24 (ADR-0009),
/// so the four settings every base stage shares travel through the same record as the CSV settings and are
/// applied by the base constructor.
/// </summary>
public class CsvOptionsBaseRecordTests
{
    private static Func<ItemErrorContext, ItemErrorAction> AnyPolicy => _ => default;



    [Fact]
    public void CsvExtractorOptions_inherits_ExtractorOptions()
    {
        Assert.IsAssignableFrom<ExtractorOptions>(new CsvExtractorOptions<PersonRecord>());
    }



    [Fact]
    public void CsvLoaderOptions_inherits_LoaderOptions()
    {
        Assert.IsAssignableFrom<LoaderOptions>(new CsvLoaderOptions<PersonRecord>());
    }



    [Fact]
    public void CsvExtractor_when_inherited_settings_are_set_through_options_applies_them()
    {
        var policy = AnyPolicy;
        var options = new CsvExtractorOptions<PersonRecord>
        {
            ReportingInterval = 5,
            SkipItemCount = 2,
            MaximumItemCount = 3,
            ErrorPolicy = policy,
            Delimiter = ";",
        };
        using var reader = new StreamReader(new MemoryStream());

        var sut = new CsvExtractor<PersonRecord>(reader, options);

        Assert.Equal(5, sut.ReportingInterval);
        Assert.Equal(2, sut.SkipItemCount);
        Assert.Equal(3, sut.MaximumItemCount);
        Assert.Same(policy, sut.ErrorPolicy);
        Assert.Equal(";", sut.Delimiter);
    }



    [Fact]
    public void CsvLoader_when_inherited_settings_are_set_through_options_applies_them()
    {
        var policy = AnyPolicy;
        var options = new CsvLoaderOptions<PersonRecord>
        {
            ReportingInterval = 5,
            SkipItemCount = 2,
            MaximumItemCount = 3,
            ErrorPolicy = policy,
            Delimiter = ";",
        };
        using var writer = new StreamWriter(new MemoryStream());

        var sut = new CsvLoader<PersonRecord>(writer, options);

        Assert.Equal(5, sut.ReportingInterval);
        Assert.Equal(2, sut.SkipItemCount);
        Assert.Equal(3, sut.MaximumItemCount);
        Assert.Same(policy, sut.ErrorPolicy);
        Assert.Equal(";", sut.Delimiter);
    }



    [Fact]
    public void CsvExtractor_when_options_are_omitted_keeps_the_base_defaults()
    {
        using var reader = new StreamReader(new MemoryStream());
        var defaults = new ExtractorOptions();

        var sut = new CsvExtractor<PersonRecord>(reader);

        Assert.Equal(defaults.ReportingInterval, sut.ReportingInterval);
        Assert.Equal(defaults.SkipItemCount, sut.SkipItemCount);
        Assert.Equal(defaults.MaximumItemCount, sut.MaximumItemCount);
    }



#pragma warning disable CS0618 // The deprecated aliases must keep forwarding until they are removed (#283).
    [Fact]
    public void SkipRecordCount_forwards_to_SkipItemCount_on_both_records()
    {
        var extractorOptions = new CsvExtractorOptions<PersonRecord> { SkipRecordCount = 4 };
        var loaderOptions = new CsvLoaderOptions<PersonRecord> { SkipRecordCount = 6 };

        Assert.Equal(4, extractorOptions.SkipItemCount);
        Assert.Equal(4, extractorOptions.SkipRecordCount);
        Assert.Equal(6, loaderOptions.SkipItemCount);
        Assert.Equal(6, loaderOptions.SkipRecordCount);
    }



    [Fact]
    public void MaxRecordCount_forwards_to_MaximumItemCount_on_both_records()
    {
        var extractorOptions = new CsvExtractorOptions<PersonRecord> { MaxRecordCount = 4 };
        var loaderOptions = new CsvLoaderOptions<PersonRecord> { MaximumItemCount = 6 };

        Assert.Equal(4, extractorOptions.MaximumItemCount);
        Assert.Equal(4, extractorOptions.MaxRecordCount);
        Assert.Equal(6, loaderOptions.MaxRecordCount);
        Assert.Equal(6, loaderOptions.MaximumItemCount);
    }
#pragma warning restore CS0618



    [Fact]
    public void With_expression_preserves_the_inherited_settings()
    {
        var original = new CsvExtractorOptions<PersonRecord> { SkipItemCount = 2, MaximumItemCount = 3 };

        var copy = original with { Delimiter = ";" };

        Assert.Equal(2, copy.SkipItemCount);
        Assert.Equal(3, copy.MaximumItemCount);
        Assert.Equal(";", copy.Delimiter);
    }
}
