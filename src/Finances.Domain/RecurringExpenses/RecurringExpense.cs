using Finances.Domain.Common;
using Finances.Domain.Categories;
using Finances.Domain.Households;
using Finances.Domain.ValueObjects;

namespace Finances.Domain.RecurringExpenses;

public sealed class RecurringExpense : AggregateRoot<RecurringExpenseId>
{
    private RecurringExpense(
        RecurringExpenseId id,
        HouseholdId householdId,
        CategoryId categoryId,
        string name,
        Money expectedAmount,
        RecurringExpenseFrequency frequency,
        RecurringExpenseStatus status)
        : base(id)
    {
        HouseholdId = householdId;
        CategoryId = categoryId;
        Name = name;
        ExpectedAmount = expectedAmount;
        Frequency = frequency;
        Status = status;
    }

    public HouseholdId HouseholdId { get; }

    public CategoryId CategoryId { get; }

    public string Name { get; }

    public Money ExpectedAmount { get; }

    public RecurringExpenseFrequency Frequency { get; }

    public RecurringExpenseStatus Status { get; private set; }

    public static Result<RecurringExpense> Create(
        RecurringExpenseId id,
        HouseholdId householdId,
        CategoryId categoryId,
        string name,
        Money expectedAmount,
        RecurringExpenseFrequency frequency,
        RecurringExpenseStatus status = RecurringExpenseStatus.Active)
    {
        return Result<RecurringExpense>.Success(new RecurringExpense(
            id,
            householdId,
            categoryId,
            name.Trim(),
            expectedAmount,
            frequency,
            status));
    }

    public bool ContributesToMonthlyCashPressure()
    {
        var specification = new RecurringExpenseContributesToCashPressureSpecification();

        return specification.IsSatisfiedBy(this);
    }

    public Result Cancel()
    {
        if (Status == RecurringExpenseStatus.Cancelled)
        {
            return Result.Failure(new Error(
                "RecurringExpense.AlreadyCancelled",
                "El gasto recurrente ya está cancelado."));
        }

        Status = RecurringExpenseStatus.Cancelled;
        AddDomainEvent(new RecurringExpenseCancelledDomainEvent(Id));

        return Result.Success();
    }
}
