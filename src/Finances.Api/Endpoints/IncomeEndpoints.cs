using Finances.Api.Contracts.Common;
using Finances.Api.Contracts.IncomeRecords;
using Finances.Api.Contracts.IncomeSources;
using Finances.Application.IncomeRecords.Commands;
using Finances.Application.IncomeRecords.Contracts;
using Finances.Application.IncomeSources.Commands;
using Finances.Application.IncomeSources.Contracts;
using Finances.Application.IncomeSources.Queries;
using Finances.Domain.Common;
using Finances.Domain.Households;
using Wolverine;
using Wolverine.Http;

namespace Finances.Api.Endpoints;

public static class IncomeEndpoints
{
    [WolverinePost("/income-sources")]
    public static async Task<IResult> CreateIncomeSource(
        CreateIncomeSourceRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<Result<IncomeSourceResponse>>(
            new CreateIncomeSourceCommand(
                request.HouseholdId,
                request.Name,
                request.ExpectedAmount,
                request.Currency,
                request.Frequency,
                request.Stability),
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.UnprocessableEntity(new ErrorResponse(
                result.Error!.Code,
                result.Error.Description));
        }

        return Results.Created($"/income-sources/{result.Value.Id}", result.Value);
    }

    [WolverineGet("/income-sources")]
    public static async Task<IResult> ListIncomeSources(
        HouseholdId householdId,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var response = await bus.InvokeAsync<IReadOnlyList<IncomeSourceResponse>>(
            new ListIncomeSourcesQuery(householdId),
            cancellationToken);

        return Results.Ok(response);
    }

    [WolverinePost("/income-records")]
    public static async Task<IResult> RegisterIncomeRecord(
        RegisterIncomeRecordRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<Result<IncomeRecordResponse>>(
            new RegisterIncomeRecordCommand(
                request.IncomeSourceId,
                request.Amount,
                request.Currency,
                request.ReceivedOn),
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.UnprocessableEntity(new ErrorResponse(
                result.Error!.Code,
                result.Error.Description));
        }

        return Results.Created($"/income-records/{result.Value.Id}", result.Value);
    }
}
