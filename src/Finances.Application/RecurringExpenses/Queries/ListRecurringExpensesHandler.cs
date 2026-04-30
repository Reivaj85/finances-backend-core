using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Application.RecurringExpenses.Contracts;
using Finances.Application.RecurringExpenses.Mappings;

namespace Finances.Application.RecurringExpenses.Queries;

public static class ListRecurringExpensesHandler
{
    public static async Task<IReadOnlyList<RecurringExpenseResponse>> Handle(
        ListRecurringExpensesQuery query,
        IRecurringExpenseRepository repository,
        CancellationToken cancellationToken)
    {
        var recurringExpenses = await repository.ListByHouseholdAsync(query.HouseholdId, cancellationToken);

        return recurringExpenses
            .Select(recurringExpense => recurringExpense.ToResponse())
            .ToArray();
    }
}
