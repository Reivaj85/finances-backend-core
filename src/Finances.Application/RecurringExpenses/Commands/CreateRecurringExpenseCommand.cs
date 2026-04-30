using Finances.Domain.Categories;
using Finances.Domain.Households;

namespace Finances.Application.RecurringExpenses.Commands;

public sealed record CreateRecurringExpenseCommand(
    HouseholdId HouseholdId,
    CategoryId CategoryId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency);
