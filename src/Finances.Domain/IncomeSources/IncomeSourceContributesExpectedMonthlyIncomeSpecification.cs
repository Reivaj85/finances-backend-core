namespace Finances.Domain.IncomeSources;

public sealed class IncomeSourceContributesExpectedMonthlyIncomeSpecification
{
    public bool IsSatisfiedBy(IncomeSource incomeSource)
    {
        return incomeSource.Status == IncomeSourceStatus.Active
            && incomeSource.ExpectedAmount.IsPositive;
    }
}
