using Finances.Application.Analytics.Contracts;
using Finances.Domain.Households;

namespace Finances.Application.Analytics.Abstractions;

public interface IMonthlyCashPressureReader
{
    Task<IReadOnlyList<MonthlyCashPressureResponse>> ListByHouseholdAsync(
        HouseholdId householdId,
        CancellationToken cancellationToken);
}
