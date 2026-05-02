using Finances.Domain.Households;

namespace Finances.Api.Contracts.IncomeSources;

public sealed record CreateIncomeSourceRequest(
    HouseholdId HouseholdId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency,
    string Stability);
