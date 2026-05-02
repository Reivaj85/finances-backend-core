using Finances.Application.Common.Validation;
using FluentValidation;

namespace Finances.Application.Analytics.Queries;

public sealed class GetMonthlyCashPressureQueryValidator : AbstractValidator<GetMonthlyCashPressureQuery>
{
    public GetMonthlyCashPressureQueryValidator()
    {
        RuleFor(query => query.HouseholdId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("CashPressure.HouseholdRequired")
            .WithMessage("El hogar es obligatorio.");
    }
}
