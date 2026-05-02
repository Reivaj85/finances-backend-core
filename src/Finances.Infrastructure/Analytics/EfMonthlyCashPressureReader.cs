using Finances.Application.Analytics.Abstractions;
using Finances.Application.Analytics.Contracts;
using Finances.Domain.Households;
using Finances.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finances.Infrastructure.Analytics;

public sealed class EfMonthlyCashPressureReader(FinancesDbContext dbContext) : IMonthlyCashPressureReader
{
    public async Task<IReadOnlyList<MonthlyCashPressureResponse>> ListByHouseholdAsync(
        HouseholdId householdId,
        CancellationToken cancellationToken)
    {
        return await dbContext.MonthlyCashPressure
            .AsNoTracking()
            .Where(row => row.HouseholdId == householdId.Value)
            .OrderBy(row => row.Period)
            .Select(row => new MonthlyCashPressureResponse(
                HouseholdId.From(row.HouseholdId),
                row.Period,
                row.ExpectedIncomeTotal,
                row.ActiveRecurringExpenseTotal,
                row.MonthlyCashPressure,
                row.EstimatedFreeCashFlow,
                row.IsIncomplete))
            .ToArrayAsync(cancellationToken);
    }
}
