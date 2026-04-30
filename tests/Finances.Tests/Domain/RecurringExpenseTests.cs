using Finances.Domain.Categories;
using Finances.Domain.Common;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;
using Finances.Domain.ValueObjects;

namespace Finances.Tests.Domain;

public sealed class RecurringExpenseTests
{
    private static readonly HouseholdId HouseholdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly CategoryId CategoryId = CategoryId.From(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Fact]
    public void Create_ShouldCreateActiveMonthlyRecurringExpense_WhenRequiredDataIsValid()
    {
        var recurringExpense = CreateRecurringExpense().Value;

        Assert.Equal("Colegio Demo", recurringExpense.Name);
        Assert.Equal(RecurringExpenseStatus.Active, recurringExpense.Status);
        Assert.Equal(RecurringExpenseFrequency.Monthly, recurringExpense.Frequency);
        Assert.True(recurringExpense.ContributesToMonthlyCashPressure());
    }

    [Fact]
    public void ContributesToMonthlyCashPressure_ShouldBeFalse_WhenRecurringExpenseIsCancelled()
    {
        var recurringExpense = CreateRecurringExpense(status: RecurringExpenseStatus.Cancelled).Value;

        Assert.False(recurringExpense.ContributesToMonthlyCashPressure());
    }

    [Fact]
    public void Cancel_ShouldRegisterDomainEvent_WhenRecurringExpenseIsActive()
    {
        var recurringExpense = CreateRecurringExpense().Value;

        var result = recurringExpense.Cancel();

        Assert.True(result.IsSuccess);
        Assert.Equal(RecurringExpenseStatus.Cancelled, recurringExpense.Status);
        var domainEvent = Assert.IsType<RecurringExpenseCancelledDomainEvent>(
            Assert.Single(recurringExpense.DomainEvents));
        Assert.Equal(recurringExpense.Id, domainEvent.RecurringExpenseId);
    }

    [Fact]
    public void Cancel_ShouldReturnFailure_WhenRecurringExpenseIsAlreadyCancelled()
    {
        var recurringExpense = CreateRecurringExpense(status: RecurringExpenseStatus.Cancelled).Value;

        var result = recurringExpense.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal("RecurringExpense.AlreadyCancelled", result.Error?.Code);
        Assert.Empty(recurringExpense.DomainEvents);
    }

    [Fact]
    public void ContributesToMonthlyCashPressure_ShouldBeFalse_WhenExpectedAmountIsZero()
    {
        var recurringExpense = CreateRecurringExpense(expectedAmount: 0).Value;

        Assert.False(recurringExpense.ContributesToMonthlyCashPressure());
    }

    private static Result<RecurringExpense> CreateRecurringExpense(
        decimal expectedAmount = 850_000m,
        RecurringExpenseStatus status = RecurringExpenseStatus.Active)
    {
        var currency = Currency.Create("COP").Value;
        var money = Money.Create(expectedAmount, currency).Value;

        return RecurringExpense.Create(
            RecurringExpenseId.From(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")),
            HouseholdId,
            CategoryId,
            "Colegio Demo",
            money,
            RecurringExpenseFrequency.Monthly,
            status);
    }
}
