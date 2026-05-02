using Finances.Application.IncomeRecords.Abstractions;
using Finances.Application.IncomeRecords.Contracts;
using Finances.Application.IncomeRecords.Mappings;
using Finances.Domain.Common;
using Finances.Domain.IncomeRecords;
using Finances.Domain.ValueObjects;

namespace Finances.Application.IncomeRecords.Commands;

public static class RegisterIncomeRecordHandler
{
    public static async Task<Result<IncomeRecordResponse>> Handle(
        RegisterIncomeRecordCommand command,
        IIncomeRecordRepository repository,
        CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(command.Currency);
        if (currencyResult.IsFailure)
        {
            return Result<IncomeRecordResponse>.Failure(currencyResult.Error!);
        }

        var moneyResult = Money.Create(command.Amount, currencyResult.Value);
        if (moneyResult.IsFailure)
        {
            return Result<IncomeRecordResponse>.Failure(moneyResult.Error!);
        }

        var incomeRecordResult = IncomeRecord.Create(
            IncomeRecordId.From(Guid.NewGuid()),
            command.IncomeSourceId,
            moneyResult.Value,
            command.ReceivedOn);

        if (incomeRecordResult.IsFailure)
        {
            return Result<IncomeRecordResponse>.Failure(incomeRecordResult.Error!);
        }

        await repository.AddAsync(incomeRecordResult.Value, cancellationToken);

        return Result<IncomeRecordResponse>.Success(incomeRecordResult.Value.ToResponse());
    }
}
