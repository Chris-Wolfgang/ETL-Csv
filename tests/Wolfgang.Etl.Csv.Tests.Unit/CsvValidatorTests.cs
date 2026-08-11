using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Xunit;

namespace Wolfgang.Etl.Csv.Tests.Unit;

public class CsvValidatorTests
{
    [Fact]
    public void NotNullOrEmpty_passes_a_non_empty_value_and_fails_null_or_empty()
    {
        var validator = CsvValidator.NotNullOrEmpty<Order>(o => o.OrderNumber, nameof(Order.OrderNumber));

        Assert.True(validator(new Order { OrderNumber = "A1" }).IsValid);
        Assert.False(validator(new Order { OrderNumber = "" }).IsValid);
        Assert.False(validator(new Order { OrderNumber = null! }).IsValid);
    }



    [Fact]
    public void NotNullOrEmpty_failure_message_uses_the_member_name()
    {
        var validator = CsvValidator.NotNullOrEmpty<Order>(o => o.OrderNumber, "OrderNumber");

        var result = validator(new Order { OrderNumber = "" });

        Assert.Contains("OrderNumber", Assert.Single(result.Failures));
    }



    [Fact]
    public void GreaterThan_passes_above_and_fails_at_or_below_the_threshold()
    {
        var validator = CsvValidator.GreaterThan<Order>(o => o.Quantity, 0);

        Assert.True(validator(new Order { Quantity = 1 }).IsValid);
        Assert.False(validator(new Order { Quantity = 0 }).IsValid);
        Assert.False(validator(new Order { Quantity = -1 }).IsValid);
    }



    [Fact]
    public void InRange_passes_inside_and_fails_outside_the_inclusive_bounds()
    {
        var validator = CsvValidator.InRange<Order>(o => o.Quantity, 1, 10);

        Assert.True(validator(new Order { Quantity = 1 }).IsValid);
        Assert.True(validator(new Order { Quantity = 10 }).IsValid);
        Assert.False(validator(new Order { Quantity = 0 }).IsValid);
        Assert.False(validator(new Order { Quantity = 11 }).IsValid);
    }



    [Fact]
    public void MaxLength_passes_within_the_limit_and_fails_beyond_it_null_passes()
    {
        var validator = CsvValidator.MaxLength<Order>(o => o.Notes, 3);

        Assert.True(validator(new Order { Notes = "abc" }).IsValid);
        Assert.True(validator(new Order { Notes = null! }).IsValid);
        Assert.False(validator(new Order { Notes = "abcd" }).IsValid);
    }



    [Fact]
    public void Matches_passes_matching_input_and_fails_non_matching()
    {
        var validator = CsvValidator.Matches<Order>(o => o.OrderNumber, new Regex("^A[0-9]+$"));

        Assert.True(validator(new Order { OrderNumber = "A12" }).IsValid);
        Assert.False(validator(new Order { OrderNumber = "B12" }).IsValid);
    }



    [Fact]
    public void Custom_passes_when_the_predicate_holds_and_reports_the_supplied_message()
    {
        var validator = CsvValidator.Custom<Order>(o => o.Quantity > 0, "Quantity must be positive.");

        Assert.True(validator(new Order { Quantity = 1 }).IsValid);
        var result = validator(new Order { Quantity = 0 });
        Assert.Equal("Quantity must be positive.", Assert.Single(result.Failures));
    }



    [Theory]
    [MemberData(nameof(NullArgumentCases))]
    public void Factory_methods_reject_null_arguments(Action act)
    {
        Assert.Throws<ArgumentNullException>(act);
    }



    public static TheoryData<Action> NullArgumentCases() => new()
    {
        () => CsvValidator.NotNullOrEmpty<Order>(null!),
        () => CsvValidator.GreaterThan<Order>(null!, 0),
        () => CsvValidator.GreaterThan<Order>(o => o.Quantity, null!),
        () => CsvValidator.InRange<Order>(null!, 0, 1),
        () => CsvValidator.InRange<Order>(o => o.Quantity, null!, 1),
        () => CsvValidator.InRange<Order>(o => o.Quantity, 0, null!),
        () => CsvValidator.MaxLength<Order>(null!, 1),
        () => CsvValidator.Matches<Order>(null!, new Regex(".")),
        () => CsvValidator.Matches<Order>(o => o.OrderNumber, null!),
        () => CsvValidator.Custom<Order>(null!, "x"),
        () => CsvValidator.Custom<Order>(_ => true, null!),
    };



    [Fact]
    public void GreaterThan_treats_incompatible_comparand_types_as_a_failure_not_a_crash()
    {
        var validator = CsvValidator.GreaterThan<Order>(o => o.Quantity, 0L);   // int selector vs long threshold

        var result = validator(new Order { Quantity = 5 });

        Assert.False(result.IsValid);
    }



    [Fact]
    public void InRange_treats_incompatible_comparand_types_as_a_failure_not_a_crash()
    {
        var validator = CsvValidator.InRange<Order>(o => o.Quantity, 0L, 10L);   // int selector vs long bounds

        var result = validator(new Order { Quantity = 5 });

        Assert.False(result.IsValid);
    }



    [Fact]
    public void MaxLength_rejects_a_negative_limit()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CsvValidator.MaxLength<Order>(o => o.Notes, -1));
    }



    [Fact]
    public void CsvValidationResult_Pass_is_valid_with_no_failures()
    {
        Assert.True(CsvValidationResult.Pass.IsValid);
        Assert.Empty(CsvValidationResult.Pass.Failures);
    }



    [Fact]
    public void CsvValidationResult_Fail_carries_the_reasons_and_is_invalid()
    {
        var result = CsvValidationResult.Fail("a", "b");

        Assert.False(result.IsValid);
        Assert.Equal(new[] { "a", "b" }, result.Failures);
    }



    [Fact]
    public void CsvValidationException_exposes_line_and_failures_in_its_message()
    {
        var ex = new CsvValidationException(7, new[] { "bad" });

        Assert.Equal(7, ex.LineNumber);
        Assert.Equal("bad", Assert.Single(ex.Failures));
        Assert.Contains("line 7", ex.Message);
        Assert.Contains("bad", ex.Message);
    }



    [ExcludeFromCodeCoverage]
    public record Order
    {
        public string OrderNumber { get; set; } = string.Empty;



        public int Quantity { get; set; }



        public string Notes { get; set; } = string.Empty;
    }
}
