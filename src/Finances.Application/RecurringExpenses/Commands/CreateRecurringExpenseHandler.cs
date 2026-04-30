using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Application.RecurringExpenses.Contracts;
using Finances.Application.RecurringExpenses.Mappings;
using Finances.Domain.Common;
using Finances.Domain.RecurringExpenses;
using Finances.Domain.ValueObjects;

namespace Finances.Application.RecurringExpenses.Commands;

public static class CreateRecurringExpenseHandler
{
    public static async Task<Result<RecurringExpenseResponse>> Handle(
        CreateRecurringExpenseCommand command,
        IRecurringExpenseRepository repository,
        CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(command.Currency);
        if (currencyResult.IsFailure)
        {
            return Result<RecurringExpenseResponse>.Failure(currencyResult.Error!);
        }

        var moneyResult = Money.Create(command.ExpectedAmount, currencyResult.Value);
        if (moneyResult.IsFailure)
        {
            return Result<RecurringExpenseResponse>.Failure(moneyResult.Error!);
        }

        var frequencyResult = ParseFrequency(command.Frequency);
        if (frequencyResult.IsFailure)
        {
            return Result<RecurringExpenseResponse>.Failure(frequencyResult.Error!);
        }

        var recurringExpenseResult = RecurringExpense.Create(
            RecurringExpenseId.From(Guid.NewGuid()),
            command.HouseholdId,
            command.CategoryId,
            command.Name,
            moneyResult.Value,
            frequencyResult.Value);

        if (recurringExpenseResult.IsFailure)
        {
            return Result<RecurringExpenseResponse>.Failure(recurringExpenseResult.Error!);
        }

        await repository.AddAsync(recurringExpenseResult.Value, cancellationToken);

        return Result<RecurringExpenseResponse>.Success(recurringExpenseResult.Value.ToResponse());
    }

    private static Result<RecurringExpenseFrequency> ParseFrequency(string frequency)
    {
        if (string.IsNullOrWhiteSpace(frequency))
        {
            return Result<RecurringExpenseFrequency>.Failure(new Error(
                "RecurringExpense.FrequencyRequired",
                "La frecuencia del gasto recurrente es obligatoria."));
        }

        if (Enum.TryParse<RecurringExpenseFrequency>(frequency, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed))
        {
            return Result<RecurringExpenseFrequency>.Success(parsed);
        }

        return Result<RecurringExpenseFrequency>.Failure(new Error(
            "RecurringExpense.InvalidFrequency",
            "La frecuencia del gasto recurrente no es válida."));
    }
}
