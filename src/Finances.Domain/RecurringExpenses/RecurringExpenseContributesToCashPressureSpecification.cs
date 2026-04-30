namespace Finances.Domain.RecurringExpenses;

public sealed class RecurringExpenseContributesToCashPressureSpecification
{
    public bool IsSatisfiedBy(RecurringExpense recurringExpense)
    {
        return recurringExpense.Status == RecurringExpenseStatus.Active
            && recurringExpense.ExpectedAmount.IsPositive;
    }
}
