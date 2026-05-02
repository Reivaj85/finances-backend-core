using Finances.Domain.Households;
using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeSources.Contracts;

public sealed record IncomeSourceResponse(
    IncomeSourceId Id,
    HouseholdId HouseholdId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency,
    string Stability,
    string Status,
    bool ContributesExpectedMonthlyIncome);
