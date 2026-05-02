using Finances.Domain.Households;

namespace Finances.Application.Analytics.Queries;

public sealed record GetMonthlyCashPressureQuery(HouseholdId HouseholdId);
