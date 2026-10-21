using System;
using System.Collections.Generic;
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
        var validator = CsvValidator.Matches<Order>(o => o.OrderNumber, new Regex("^A[0-9]+$", RegexOptions.None, TimeSpan.FromSeconds(1)));

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
        () => CsvValidator.Matches<Order>(null!, new Regex(".", RegexOptions.None, TimeSpan.FromSeconds(1))),
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
    public void MaxLength_when_limit_is_negative_reports_the_parameter_value_and_reason()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => CsvValidator.MaxLength<Order>(o => o.Notes, -1));

        Assert.Equal("maxLength", ex.ParamName);
        Assert.Equal(-1, ex.ActualValue);
        Assert.StartsWith("Max length must be non-negative.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void MaxLength_when_limit_is_zero_accepts_only_empty_values()
    {
        var validator = CsvValidator.MaxLength<Order>(o => o.Notes, 0);

        Assert.True(validator(new Order { Notes = string.Empty }).IsValid);
        Assert.False(validator(new Order { Notes = "a" }).IsValid);
    }



    [Fact]
    public void InRange_failure_message_names_the_member_and_both_bounds()
    {
        var validator = CsvValidator.InRange<Order>(o => o.Quantity, 1, 10, nameof(Order.Quantity));

        var result = validator(new Order { Quantity = 11 });

        Assert.Equal("Quantity must be between 1 and 10 inclusive.", Assert.Single(result.Failures));
    }



    [Fact]
    public void MaxLength_failure_message_names_the_member_and_the_limit()
    {
        var validator = CsvValidator.MaxLength<Order>(o => o.Notes, 3, nameof(Order.Notes));

        var result = validator(new Order { Notes = "abcd" });

        Assert.Equal("Notes must be at most 3 characters.", Assert.Single(result.Failures));
    }



    [Fact]
    public void Matches_failure_message_names_the_member_and_the_pattern()
    {
        var validator = CsvValidator.Matches<Order>(o => o.OrderNumber, new Regex("^A[0-9]+$", RegexOptions.None, TimeSpan.FromSeconds(1)), nameof(Order.OrderNumber));

        var result = validator(new Order { OrderNumber = "B12" });

        Assert.Equal("OrderNumber must match '^A[0-9]+$'.", Assert.Single(result.Failures));
    }



    [Fact]
    public void Matches_tests_a_null_value_as_empty()
    {
        var validator = CsvValidator.Matches<Order>(o => o.OrderNumber, new Regex("^$", RegexOptions.None, TimeSpan.FromSeconds(1)));

        Assert.True(validator(new Order { OrderNumber = null! }).IsValid);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Failure_message_uses_Value_when_no_member_name_is_given(string? memberName)
    {
        var validator = CsvValidator.MaxLength<Order>(o => o.Notes, 1, memberName);

        var result = validator(new Order { Notes = "ab" });

        Assert.Equal("Value must be at most 1 characters.", Assert.Single(result.Failures));
    }



    [Fact]
    public void InRange_when_comparand_types_are_incompatible_fails_even_though_the_bounds_would_admit_zero()
    {
        // TryCompareTo reports an incompatible comparison as "not compared" (false). If it reported
        // success with a comparison of 0 instead, a value would satisfy both low >= 0 and high <= 0.
        var validator = CsvValidator.InRange<Order>(o => o.Quantity, 0L, 0L, nameof(Order.Quantity));

        var result = validator(new Order { Quantity = 0 });

        Assert.Equal("Quantity must be between 0 and 0 inclusive.", Assert.Single(result.Failures));
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
    public void CsvValidationResult_default_ctor_creates_valid_result_with_empty_failures()
    {
        var result = new CsvValidationResult();

        Assert.True(result.IsValid);
        Assert.Empty(result.Failures);
    }



    [Fact]
    public void CsvValidationResult_failed_ctor_stores_failures_and_is_invalid()
    {
        IReadOnlyList<string> reasons = new[] { "a", "b" };

        var result = new CsvValidationResult(reasons);

        Assert.False(result.IsValid);
        Assert.Equal(reasons, result.Failures);
    }



    [Fact]
    public void CsvValidationResult_failed_ctor_throws_on_null_failures()
    {
        Assert.Throws<ArgumentNullException>(() => new CsvValidationResult(failures: null!));
    }



    [Fact]
    public void CsvValidationResult_failed_ctor_throws_on_empty_failures()
    {
        Assert.Throws<ArgumentException>(() => new CsvValidationResult(Array.Empty<string>()));
    }



    [Fact]
    public void CsvValidationResult_Fail_throws_on_null_reasons()
    {
        Assert.Throws<ArgumentNullException>(() => CsvValidationResult.Fail(reasons: null!));
    }



    [Fact]
    public void CsvValidationResult_Fail_throws_on_empty_reasons()
    {
        Assert.Throws<ArgumentException>(() => CsvValidationResult.Fail());
    }



    // Legacy positional-record constructor kept for source compat + marked [Obsolete].
    // Verifies both that it still works and that the new validation catches the illegal
    // states the two-ctor design was created to prevent.

    [Fact]
    public void CsvValidationResult_legacy_ctor_accepts_valid_success_shape()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        var result = new CsvValidationResult(IsValid: true, Failures: Array.Empty<string>());
#pragma warning restore CS0618

        Assert.True(result.IsValid);
        Assert.Empty(result.Failures);
    }



    [Fact]
    public void CsvValidationResult_legacy_ctor_accepts_valid_failure_shape()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        var result = new CsvValidationResult(IsValid: false, Failures: new[] { "reason" });
#pragma warning restore CS0618

        Assert.False(result.IsValid);
        Assert.Equal("reason", Assert.Single(result.Failures));
    }



    [Fact]
    public void CsvValidationResult_legacy_ctor_throws_on_null_failures()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        Assert.Throws<ArgumentNullException>(() => new CsvValidationResult(IsValid: false, Failures: null!));
#pragma warning restore CS0618
    }



    [Fact]
    public void CsvValidationResult_legacy_ctor_throws_on_success_with_failures()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        Assert.Throws<ArgumentException>(() => new CsvValidationResult(IsValid: true, Failures: new[] { "impossible" }));
#pragma warning restore CS0618
    }



    [Fact]
    public void CsvValidationResult_legacy_ctor_throws_on_failure_without_failures()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        Assert.Throws<ArgumentException>(() => new CsvValidationResult(IsValid: false, Failures: Array.Empty<string>()));
#pragma warning restore CS0618
    }



    [Fact]
    public void CsvValidationResult_Deconstruct_still_works()
    {
        var result = CsvValidationResult.Fail("x", "y");

        var (isValid, failures) = result;

        Assert.False(isValid);
        Assert.Equal(new[] { "x", "y" }, failures);
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



    [Fact]
    public void CsvValidationException_joins_several_failures_with_a_semicolon()
    {
        var ex = new CsvValidationException(3, new[] { "first", "second" });

        Assert.Equal("Record on line 3 failed validation: first; second", ex.Message);
    }



    [Fact]
    public void CsvValidationException_with_no_failures_says_so_in_its_message()
    {
        var ex = new CsvValidationException(4, Array.Empty<string>());

        Assert.Equal("Record on line 4 failed validation: (no reasons provided)", ex.Message);
        Assert.Empty(ex.Failures);
    }



    [Fact]
    public void CsvValidationException_with_null_failures_treats_them_as_empty()
    {
        var ex = new CsvValidationException(5, null!);

        Assert.Equal("Record on line 5 failed validation: (no reasons provided)", ex.Message);
        Assert.Empty(ex.Failures);
    }



    [Fact]
    public void CsvValidationResult_failed_ctor_empty_failures_reports_parameter_and_message()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CsvValidationResult(Array.Empty<string>()));

        Assert.Equal("failures", ex.ParamName);
        Assert.StartsWith("A failed CsvValidationResult must contain at least one failure reason.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void CsvValidationResult_Fail_empty_reasons_reports_parameter_and_message()
    {
        // ParamName "reasons" proves Fail throws itself rather than relying on the constructor's
        // own check (which reports "failures").
        var ex = Assert.Throws<ArgumentException>(() => CsvValidationResult.Fail());

        Assert.Equal("reasons", ex.ParamName);
        Assert.StartsWith("At least one reason must be provided.", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void CsvValidationResult_legacy_ctor_invalid_shapes_report_parameter_and_message()
    {
#pragma warning disable CS0618 // legacy ctor is [Obsolete] — deliberate call under test
        var success = Assert.Throws<ArgumentException>(() => new CsvValidationResult(IsValid: true, Failures: new[] { "impossible" }));
        var failure = Assert.Throws<ArgumentException>(() => new CsvValidationResult(IsValid: false, Failures: Array.Empty<string>()));
#pragma warning restore CS0618

        Assert.Equal("Failures", success.ParamName);
        Assert.StartsWith("A successful CsvValidationResult cannot carry failure reasons.", success.Message, StringComparison.Ordinal);
        Assert.Equal("Failures", failure.ParamName);
        Assert.StartsWith("A failed CsvValidationResult must contain at least one failure reason.", failure.Message, StringComparison.Ordinal);
    }



    public record Order
    {
        public string OrderNumber { get; set; } = string.Empty;



        public int Quantity { get; set; }



        public string Notes { get; set; } = string.Empty;
    }
}
