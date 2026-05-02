using Finances.Domain.Households;

namespace Finances.Application.Analytics.Contracts;

public sealed record MonthlyCashPressureResponse(
    HouseholdId HouseholdId,
    DateOnly Period,
    decimal ExpectedIncomeTotal,
    decimal ActiveRecurringExpenseTotal,
    decimal MonthlyCashPressure,
    decimal EstimatedFreeCashFlow,
    bool IsIncomplete);
