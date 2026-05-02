using Finances.Application.Common.Validation;
using Finances.Domain.IncomeSources;
using FluentValidation;

namespace Finances.Application.IncomeSources.Commands;

public sealed class CreateIncomeSourceCommandValidator : AbstractValidator<CreateIncomeSourceCommand>
{
    public CreateIncomeSourceCommandValidator()
    {
        RuleFor(command => command.HouseholdId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("IncomeSource.HouseholdRequired")
            .WithMessage("El hogar es obligatorio.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithErrorCode("IncomeSource.NameRequired")
            .WithMessage("El nombre de la fuente de ingreso es obligatorio.")
            .MaximumLength(160)
            .WithErrorCode("IncomeSource.NameTooLong")
            .WithMessage("El nombre de la fuente de ingreso no puede superar 160 caracteres.");

        RuleFor(command => command.ExpectedAmount)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("Money.NegativeAmount")
            .WithMessage("El monto financiero no puede ser negativo.");

        RuleFor(command => command.Currency)
            .NotEmpty()
            .WithErrorCode("Currency.Required")
            .WithMessage("La moneda es obligatoria.")
            .Matches("^[A-Za-z]{3}$")
            .WithErrorCode("Currency.InvalidIsoCode")
            .WithMessage("La moneda debe ser un código ISO de tres letras.");

        RuleFor(command => command.Frequency)
            .NotEmpty()
            .WithErrorCode("IncomeSource.FrequencyRequired")
            .WithMessage("La frecuencia del ingreso es obligatoria.")
            .Must(IsValidFrequency)
            .WithErrorCode("IncomeSource.InvalidFrequency")
            .WithMessage("La frecuencia del ingreso no es válida.");

        RuleFor(command => command.Stability)
            .NotEmpty()
            .WithErrorCode("IncomeSource.StabilityRequired")
            .WithMessage("La estabilidad del ingreso es obligatoria.")
            .Must(IsValidStability)
            .WithErrorCode("IncomeSource.InvalidStability")
            .WithMessage("La estabilidad del ingreso no es válida.");
    }

    private static bool IsValidFrequency(string frequency)
    {
        return Enum.TryParse<IncomeFrequency>(frequency, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed);
    }

    private static bool IsValidStability(string stability)
    {
        return Enum.TryParse<IncomeStability>(stability, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed);
    }
}
