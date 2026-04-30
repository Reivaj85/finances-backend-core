using Finances.Api.Contracts.Common;
using Finances.Api.Contracts.Health;
using Finances.Api.Contracts.RecurringExpenses;
using Finances.Application.RecurringExpenses.Commands;
using Finances.Application.RecurringExpenses.Contracts;
using Finances.Application.RecurringExpenses.Queries;
using Finances.Domain.Common;
using Finances.Domain.Households;
using Wolverine;
using Wolverine.Http;

namespace Finances.Api.Endpoints;

public static class RecurringExpensesEndpoints
{
    [WolverineGet("/health")]
    public static IResult GetHealth()
    {
        return Results.Ok(new HealthResponse("healthy"));
    }

    [WolverinePost("/recurring-expenses")]
    public static async Task<IResult> CreateRecurringExpense(
        CreateRecurringExpenseRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<Result<RecurringExpenseResponse>>(
            new CreateRecurringExpenseCommand(
                request.HouseholdId,
                request.CategoryId,
                request.Name,
                request.ExpectedAmount,
                request.Currency,
                request.Frequency),
            cancellationToken);

        if (result.IsFailure)
        {
            return Results.UnprocessableEntity(new ErrorResponse(
                result.Error!.Code,
                result.Error.Description));
        }

        return Results.Created($"/recurring-expenses/{result.Value.Id}", result.Value);
    }

    [WolverineGet("/recurring-expenses")]
    public static async Task<IResult> ListRecurringExpenses(
        HouseholdId householdId,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        var response = await bus.InvokeAsync<IReadOnlyList<RecurringExpenseResponse>>(
            new ListRecurringExpensesQuery(householdId),
            cancellationToken);

        return Results.Ok(response);
    }
}
