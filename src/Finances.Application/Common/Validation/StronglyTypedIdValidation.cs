using Finances.Domain.Categories;
using Finances.Domain.Households;
using Finances.Domain.IncomeSources;

namespace Finances.Application.Common.Validation;

internal static class StronglyTypedIdValidation
{
    public static bool HasValue(HouseholdId id)
    {
        return HasValue(() => id.Value);
    }

    public static bool HasValue(CategoryId id)
    {
        return HasValue(() => id.Value);
    }

    public static bool HasValue(IncomeSourceId id)
    {
        return HasValue(() => id.Value);
    }

    private static bool HasValue(Func<Guid> getValue)
    {
        try
        {
            return getValue() != Guid.Empty;
        }
        catch (Exception exception) when (exception.GetType().FullName == "Vogen.ValueObjectValidationException")
        {
            return false;
        }
    }
}
