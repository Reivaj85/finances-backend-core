using Finances.Domain.Common;
using Finances.Domain.Households;
using Finances.Domain.ValueObjects;

namespace Finances.Domain.IncomeSources;

public sealed class IncomeSource : AggregateRoot<IncomeSourceId>
{
    private IncomeSource(
        IncomeSourceId id,
        HouseholdId householdId,
        string name,
        Money expectedAmount,
        IncomeFrequency frequency,
        IncomeStability stability,
        IncomeSourceStatus status)
        : base(id)
    {
        HouseholdId = householdId;
        Name = name;
        ExpectedAmount = expectedAmount;
        Frequency = frequency;
        Stability = stability;
        Status = status;
    }

    public HouseholdId HouseholdId { get; }

    public string Name { get; }

    public Money ExpectedAmount { get; }

    public IncomeFrequency Frequency { get; }

    public IncomeStability Stability { get; }

    public IncomeSourceStatus Status { get; private set; }

    public static Result<IncomeSource> Create(
        IncomeSourceId id,
        HouseholdId householdId,
        string name,
        Money expectedAmount,
        IncomeFrequency frequency,
        IncomeStability stability,
        IncomeSourceStatus status = IncomeSourceStatus.Active)
    {
        return Result<IncomeSource>.Success(new IncomeSource(
            id,
            householdId,
            name.Trim(),
            expectedAmount,
            frequency,
            stability,
            status));
    }

    public bool ContributesExpectedMonthlyIncome()
    {
        var specification = new IncomeSourceContributesExpectedMonthlyIncomeSpecification();

        return specification.IsSatisfiedBy(this);
    }
}
