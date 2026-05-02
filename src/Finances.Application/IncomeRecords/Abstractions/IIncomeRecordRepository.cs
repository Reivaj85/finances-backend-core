using Finances.Domain.IncomeRecords;
using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeRecords.Abstractions;

public interface IIncomeRecordRepository
{
    Task AddAsync(IncomeRecord incomeRecord, CancellationToken cancellationToken);

    Task<IReadOnlyList<IncomeRecord>> ListBySourceAsync(IncomeSourceId incomeSourceId, CancellationToken cancellationToken);
}
