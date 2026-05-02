using Finances.Domain.Households;
using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeSources.Abstractions;

public interface IIncomeSourceRepository
{
    Task AddAsync(IncomeSource incomeSource, CancellationToken cancellationToken);

    Task<IReadOnlyList<IncomeSource>> ListByHouseholdAsync(HouseholdId householdId, CancellationToken cancellationToken);
}
