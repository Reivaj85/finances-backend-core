using Finances.Application.IncomeSources.Abstractions;
using Finances.Domain.Common;
using Finances.Domain.Households;
using Finances.Domain.IncomeSources;
using Finances.Domain.ValueObjects;
using Finances.Infrastructure.Persistence;
using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Finances.Infrastructure.IncomeSources;

public sealed class EfIncomeSourceRepository(FinancesDbContext dbContext) : IIncomeSourceRepository
{
    public async Task AddAsync(IncomeSource incomeSource, CancellationToken cancellationToken)
    {
        dbContext.IncomeSources.Add(ToRecord(incomeSource));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IncomeSource>> ListByHouseholdAsync(
        HouseholdId householdId,
        CancellationToken cancellationToken)
    {
        var records = await dbContext.IncomeSources
            .AsNoTracking()
            .Where(incomeSource => incomeSource.HouseholdId == householdId.Value)
            .OrderBy(incomeSource => incomeSource.Name)
            .ToArrayAsync(cancellationToken);

        return records.Select(ToDomain).ToArray();
    }

    private static IncomeSourceRecord ToRecord(IncomeSource incomeSource)
    {
        return new IncomeSourceRecord
        {
            Id = incomeSource.Id.Value,
            HouseholdId = incomeSource.HouseholdId.Value,
            Name = incomeSource.Name,
            ExpectedAmount = incomeSource.ExpectedAmount.Amount,
            Currency = incomeSource.ExpectedAmount.Currency.Code,
            Frequency = incomeSource.Frequency.ToString().ToLowerInvariant(),
            Stability = incomeSource.Stability.ToString().ToLowerInvariant(),
            Status = incomeSource.Status.ToString().ToLowerInvariant()
        };
    }

    private static IncomeSource ToDomain(IncomeSourceRecord record)
    {
        var currency = EnsureSuccess(Currency.Create(record.Currency));
        var money = EnsureSuccess(Money.Create(record.ExpectedAmount, currency));

        return EnsureSuccess(IncomeSource.Create(
            IncomeSourceId.From(record.Id),
            HouseholdId.From(record.HouseholdId),
            record.Name,
            money,
            Enum.Parse<IncomeFrequency>(record.Frequency, ignoreCase: true),
            Enum.Parse<IncomeStability>(record.Stability, ignoreCase: true),
            Enum.Parse<IncomeSourceStatus>(record.Status, ignoreCase: true)));
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
