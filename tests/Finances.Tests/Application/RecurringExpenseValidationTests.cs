using Finances.Application.RecurringExpenses.Commands;
using Finances.Application.RecurringExpenses.Queries;
using Finances.Domain.Categories;
using Finances.Domain.Households;

namespace Finances.Tests.Application;

public sealed class RecurringExpenseValidationTests
{
    private static readonly HouseholdId HouseholdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly CategoryId CategoryId = CategoryId.From(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Fact]
    public void CreateRecurringExpenseCommandValidator_ShouldPass_WhenCommandIsValid()
    {
        var validator = new CreateRecurringExpenseCommandValidator();
        var command = new CreateRecurringExpenseCommand(
            HouseholdId,
            CategoryId,
            "Colegio Demo",
            850_000m,
            "COP",
            "monthly");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateRecurringExpenseCommandValidator_ShouldFail_WhenRequiredValuesAreInvalid()
    {
        var validator = new CreateRecurringExpenseCommandValidator();
        var command = new CreateRecurringExpenseCommand(
            HouseholdId.From(Guid.Empty),
            CategoryId.From(Guid.Empty),
            "",
            -1m,
            "CO1",
            "weeklyish");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorCode == "RecurringExpense.HouseholdRequired");
        Assert.Contains(result.Errors, error => error.ErrorCode == "RecurringExpense.CategoryRequired");
        Assert.Contains(result.Errors, error => error.ErrorCode == "RecurringExpense.NameRequired");
        Assert.Contains(result.Errors, error => error.ErrorCode == "Money.NegativeAmount");
        Assert.Contains(result.Errors, error => error.ErrorCode == "Currency.InvalidIsoCode");
        Assert.Contains(result.Errors, error => error.ErrorCode == "RecurringExpense.InvalidFrequency");
    }

    [Fact]
    public void ListRecurringExpensesQueryValidator_ShouldFail_WhenHouseholdIsMissing()
    {
        var validator = new ListRecurringExpensesQueryValidator();
        var query = new ListRecurringExpensesQuery(HouseholdId.From(Guid.Empty));

        var result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorCode == "RecurringExpense.HouseholdRequired");
    }
}
