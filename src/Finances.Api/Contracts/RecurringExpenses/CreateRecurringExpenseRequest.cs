using Finances.Domain.Categories;
using Finances.Domain.Households;

namespace Finances.Api.Contracts.RecurringExpenses;

public sealed record CreateRecurringExpenseRequest(
    HouseholdId HouseholdId,
    CategoryId CategoryId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency);
