using Finances.Application.RecurringExpenses.Contracts;
using Finances.Domain.RecurringExpenses;

namespace Finances.Application.RecurringExpenses.Mappings;

internal static class RecurringExpenseMapping
{
    public static RecurringExpenseResponse ToResponse(this RecurringExpense recurringExpense)
    {
        return new RecurringExpenseResponse(
            recurringExpense.Id,
            recurringExpense.HouseholdId,
            recurringExpense.CategoryId,
            recurringExpense.Name,
            recurringExpense.ExpectedAmount.Amount,
            recurringExpense.ExpectedAmount.Currency.Code,
            recurringExpense.Frequency.ToString().ToLowerInvariant(),
            recurringExpense.Status.ToString().ToLowerInvariant(),
            recurringExpense.ContributesToMonthlyCashPressure());
    }
}
