using Finances.Application.IncomeSources.Contracts;
using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeSources.Mappings;

internal static class IncomeSourceMapping
{
    public static IncomeSourceResponse ToResponse(this IncomeSource incomeSource)
    {
        return new IncomeSourceResponse(
            incomeSource.Id,
            incomeSource.HouseholdId,
            incomeSource.Name,
            incomeSource.ExpectedAmount.Amount,
            incomeSource.ExpectedAmount.Currency.Code,
            incomeSource.Frequency.ToString().ToLowerInvariant(),
            incomeSource.Stability.ToString().ToLowerInvariant(),
            incomeSource.Status.ToString().ToLowerInvariant(),
            incomeSource.ContributesExpectedMonthlyIncome());
    }
}
