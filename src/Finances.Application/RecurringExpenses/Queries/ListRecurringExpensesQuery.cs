using Finances.Domain.Households;

namespace Finances.Application.RecurringExpenses.Queries;

public sealed record ListRecurringExpensesQuery(HouseholdId HouseholdId);
