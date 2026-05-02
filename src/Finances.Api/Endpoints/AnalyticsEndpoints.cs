using Finances.Application.Analytics.Contracts;
using Finances.Application.Analytics.Queries;
using Finances.Domain.Households;
using Wolverine;
using Wolverine.Http;

namespace Finances.Api.Endpoints;

public static class AnalyticsEndpoints
{
    [WolverineGet("/analytics/monthly-cash-pressure")]
    public static async Task<IResult> GetMonthlyCashPressure(
        HouseholdId householdId,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var response = await bus.InvokeAsync<IReadOnlyList<MonthlyCashPressureResponse>>(
            new GetMonthlyCashPressureQuery(householdId),
            cancellationToken);

        return Results.Ok(response);
    }
}
