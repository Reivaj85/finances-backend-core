using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;

namespace Finances.Application.RecurringExpenses.Abstractions;

public interface IRecurringExpenseRepository
{
    Task AddAsync(RecurringExpense recurringExpense, CancellationToken cancellationToken);

    Task<IReadOnlyList<RecurringExpense>> ListByHouseholdAsync(HouseholdId householdId, CancellationToken cancellationToken);
}
