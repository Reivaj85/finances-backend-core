using Finances.Application.Common.Validation;
using FluentValidation;

namespace Finances.Application.IncomeRecords.Commands;

public sealed class RegisterIncomeRecordCommandValidator : AbstractValidator<RegisterIncomeRecordCommand>
{
    public RegisterIncomeRecordCommandValidator()
    {
        RuleFor(command => command.IncomeSourceId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("IncomeRecord.IncomeSourceRequired")
            .WithMessage("La fuente de ingreso es obligatoria.");

        RuleFor(command => command.Amount)
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

        RuleFor(command => command.ReceivedOn)
            .NotEmpty()
            .WithErrorCode("IncomeRecord.ReceivedOnRequired")
            .WithMessage("La fecha de recepción del ingreso es obligatoria.");
    }
}
