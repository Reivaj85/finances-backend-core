using Finances.Domain.Households;

namespace Finances.Application.IncomeSources.Commands;

public sealed record CreateIncomeSourceCommand(
    HouseholdId HouseholdId,
    string Name,
    decimal ExpectedAmount,
    string Currency,
    string Frequency,
    string Stability);
