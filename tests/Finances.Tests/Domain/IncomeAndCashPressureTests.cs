using Finances.Domain.Analytics;
using Finances.Domain.Households;
using Finances.Domain.IncomeSources;
using Finances.Domain.ValueObjects;

namespace Finances.Tests.Domain;

public sealed class IncomeAndCashPressureTests
{
    private static readonly HouseholdId HouseholdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    [Fact]
    public void IncomeSource_ShouldContributeExpectedIncome_WhenSourceIsActiveWithPositiveAmount()
    {
        var incomeSource = CreateIncomeSource(1_800_000m, IncomeSourceStatus.Active);

        Assert.True(incomeSource.ContributesExpectedMonthlyIncome());
    }

    [Fact]
    public void IncomeSource_ShouldNotContributeExpectedIncome_WhenSourceIsInactive()
    {
        var incomeSource = CreateIncomeSource(1_800_000m, IncomeSourceStatus.Inactive);

        Assert.False(incomeSource.ContributesExpectedMonthlyIncome());
    }

    [Fact]
    public void CashPressure_ShouldCalculateFreeCashFlow_WhenIncomeExists()
    {
        var summary = MonthlyCashPressure.Calculate(2_000_000m, 1_250_000m);

        Assert.Equal(2_000_000m, summary.ExpectedIncomeTotal);
        Assert.Equal(1_250_000m, summary.ActiveRecurringExpenseTotal);
        Assert.Equal(1_250_000m, summary.MonthlyCashPressure);
        Assert.Equal(750_000m, summary.EstimatedFreeCashFlow);
        Assert.False(summary.IsIncomplete);
    }

    [Fact]
    public void CashPressure_ShouldBeIncomplete_WhenIncomeIsMissing()
    {
        var summary = MonthlyCashPressure.Calculate(0m, 1_250_000m);

        Assert.Equal(-1_250_000m, summary.EstimatedFreeCashFlow);
        Assert.True(summary.IsIncomplete);
    }

    private static IncomeSource CreateIncomeSource(decimal amount, IncomeSourceStatus status)
    {
        var currency = Currency.Create("COP").Value;
        var expectedAmount = Money.Create(amount, currency).Value;

        return IncomeSource.Create(
            IncomeSourceId.From(Guid.Parse("11111111-1111-1111-1111-111111111111")),
            HouseholdId,
            "Empresa Demo S.A.",
            expectedAmount,
            IncomeFrequency.Monthly,
            IncomeStability.Stable,
            status).Value;
    }
}
