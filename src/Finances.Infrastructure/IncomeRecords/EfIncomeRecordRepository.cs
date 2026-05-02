using Finances.Application.IncomeRecords.Abstractions;
using Finances.Domain.Common;
using Finances.Domain.IncomeRecords;
using Finances.Domain.IncomeSources;
using Finances.Domain.ValueObjects;
using Finances.Infrastructure.Persistence;
using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Finances.Infrastructure.IncomeRecords;

public sealed class EfIncomeRecordRepository(FinancesDbContext dbContext) : IIncomeRecordRepository
{
    public async Task AddAsync(IncomeRecord incomeRecord, CancellationToken cancellationToken)
    {
        dbContext.IncomeRecords.Add(ToRecord(incomeRecord));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IncomeRecord>> ListBySourceAsync(
        IncomeSourceId incomeSourceId,
        CancellationToken cancellationToken)
    {
        var records = await dbContext.IncomeRecords
            .AsNoTracking()
            .Where(incomeRecord => incomeRecord.IncomeSourceId == incomeSourceId.Value)
            .OrderByDescending(incomeRecord => incomeRecord.ReceivedOn)
            .ToArrayAsync(cancellationToken);

        return records.Select(ToDomain).ToArray();
    }

    private static IncomeRecordRecord ToRecord(IncomeRecord incomeRecord)
    {
        return new IncomeRecordRecord
        {
            Id = incomeRecord.Id.Value,
            IncomeSourceId = incomeRecord.IncomeSourceId.Value,
            Amount = incomeRecord.Amount.Amount,
            Currency = incomeRecord.Amount.Currency.Code,
            ReceivedOn = incomeRecord.ReceivedOn
        };
    }

    private static IncomeRecord ToDomain(IncomeRecordRecord record)
    {
        var currency = EnsureSuccess(Currency.Create(record.Currency));
        var money = EnsureSuccess(Money.Create(record.Amount, currency));

        return EnsureSuccess(IncomeRecord.Create(
            IncomeRecordId.From(record.Id),
            IncomeSourceId.From(record.IncomeSourceId),
            money,
            record.ReceivedOn));
    }

    private static T EnsureSuccess<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"El registro persistido no pudo materializarse: {result.Error!.Code} - {result.Error.Description}");
        }

        return result.Value;
    }
}
