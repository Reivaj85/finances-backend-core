using Finances.Application.Common.Validation;
using FluentValidation;

namespace Finances.Application.IncomeSources.Queries;

public sealed class ListIncomeSourcesQueryValidator : AbstractValidator<ListIncomeSourcesQuery>
{
    public ListIncomeSourcesQueryValidator()
    {
        RuleFor(query => query.HouseholdId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("IncomeSource.HouseholdRequired")
            .WithMessage("El hogar es obligatorio.");
    }
}
