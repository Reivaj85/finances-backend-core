using Finances.Domain.Categories;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;

namespace Finances.Application.RecurringExpenses.Contracts;

public sealed record RecurringExpenseResponse(
    RecurringExpenseId Id,
    HouseholdId HouseholdId,
    CategoryId CategoryId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency,
    string Status,
    bool ContributesToMonthlyCashPressure);
