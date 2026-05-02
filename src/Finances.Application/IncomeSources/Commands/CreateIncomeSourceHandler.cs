using Finances.Application.IncomeSources.Abstractions;
using Finances.Application.IncomeSources.Contracts;
using Finances.Application.IncomeSources.Mappings;
using Finances.Domain.Common;
using Finances.Domain.IncomeSources;
using Finances.Domain.ValueObjects;

namespace Finances.Application.IncomeSources.Commands;

public static class CreateIncomeSourceHandler
{
    public static async Task<Result<IncomeSourceResponse>> Handle(
        CreateIncomeSourceCommand command,
        IIncomeSourceRepository repository,
        CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(command.Currency);
        if (currencyResult.IsFailure)
        {
            return Result<IncomeSourceResponse>.Failure(currencyResult.Error!);
        }

        var moneyResult = Money.Create(command.ExpectedAmount, currencyResult.Value);
        if (moneyResult.IsFailure)
        {
            return Result<IncomeSourceResponse>.Failure(moneyResult.Error!);
        }

        var frequencyResult = ParseFrequency(command.Frequency);
        if (frequencyResult.IsFailure)
        {
            return Result<IncomeSourceResponse>.Failure(frequencyResult.Error!);
        }

        var stabilityResult = ParseStability(command.Stability);
        if (stabilityResult.IsFailure)
        {
            return Result<IncomeSourceResponse>.Failure(stabilityResult.Error!);
        }

        var incomeSourceResult = IncomeSource.Create(
            IncomeSourceId.From(Guid.NewGuid()),
            command.HouseholdId,
            command.Name,
            moneyResult.Value,
            frequencyResult.Value,
            stabilityResult.Value);

        if (incomeSourceResult.IsFailure)
        {
            return Result<IncomeSourceResponse>.Failure(incomeSourceResult.Error!);
        }

        await repository.AddAsync(incomeSourceResult.Value, cancellationToken);

        return Result<IncomeSourceResponse>.Success(incomeSourceResult.Value.ToResponse());
    }

    private static Result<IncomeFrequency> ParseFrequency(string frequency)
    {
        if (Enum.TryParse<IncomeFrequency>(frequency, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed))
        {
            return Result<IncomeFrequency>.Success(parsed);
        }

        return Result<IncomeFrequency>.Failure(new Error(
            "IncomeSource.InvalidFrequency",
            "La frecuencia del ingreso no es válida."));
    }

    private static Result<IncomeStability> ParseStability(string stability)
    {
        if (Enum.TryParse<IncomeStability>(stability, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed))
        {
            return Result<IncomeStability>.Success(parsed);
        }

        return Result<IncomeStability>.Failure(new Error(
            "IncomeSource.InvalidStability",
            "La estabilidad del ingreso no es válida."));
    }
}
