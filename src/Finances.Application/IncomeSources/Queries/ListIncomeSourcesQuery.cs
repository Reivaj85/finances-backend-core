using Finances.Domain.Households;

namespace Finances.Application.IncomeSources.Queries;

public sealed record ListIncomeSourcesQuery(HouseholdId HouseholdId);
