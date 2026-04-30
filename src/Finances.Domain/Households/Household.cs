using Finances.Domain.Common;

namespace Finances.Domain.Households;

public sealed class Household : Entity<HouseholdId>
{
    private Household(HouseholdId id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; }

    public static Result<Household> Create(HouseholdId id, string name)
    {
        return Result<Household>.Success(new Household(id, name.Trim()));
    }
}
