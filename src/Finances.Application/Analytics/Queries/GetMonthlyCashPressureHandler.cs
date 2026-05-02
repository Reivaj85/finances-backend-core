using Finances.Application.Analytics.Abstractions;
using Finances.Application.Analytics.Contracts;

namespace Finances.Application.Analytics.Queries;

public static class GetMonthlyCashPressureHandler
{
    public static Task<IReadOnlyList<MonthlyCashPressureResponse>> Handle(
        GetMonthlyCashPressureQuery query,
        IMonthlyCashPressureReader reader,
        CancellationToken cancellationToken)
    {
        return reader.ListByHouseholdAsync(query.HouseholdId, cancellationToken);
    }
}
