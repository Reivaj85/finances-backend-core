using Finances.Application.Common.Validation;
using Finances.Domain.RecurringExpenses;
using FluentValidation;

namespace Finances.Application.RecurringExpenses.Commands;

public sealed class CreateRecurringExpenseCommandValidator : AbstractValidator<CreateRecurringExpenseCommand>
{
    public CreateRecurringExpenseCommandValidator()
    {
        RuleFor(command => command.HouseholdId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("RecurringExpense.HouseholdRequired")
            .WithMessage("El hogar es obligatorio.");

        RuleFor(command => command.CategoryId)
            .Must(StronglyTypedIdValidation.HasValue)
            .WithErrorCode("RecurringExpense.CategoryRequired")
            .WithMessage("La categoría es obligatoria.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithErrorCode("RecurringExpense.NameRequired")
            .WithMessage("El nombre del gasto recurrente es obligatorio.")
            .MaximumLength(200)
            .WithErrorCode("RecurringExpense.NameTooLong")
            .WithMessage("El nombre del gasto recurrente no puede superar 200 caracteres.");

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
            .WithErrorCode("RecurringExpense.FrequencyRequired")
            .WithMessage("La frecuencia del gasto recurrente es obligatoria.")
            .Must(IsValidFrequency)
            .WithErrorCode("RecurringExpense.InvalidFrequency")
            .WithMessage("La frecuencia del gasto recurrente no es válida.");
    }

    private static bool IsValidFrequency(string frequency)
    {
        return Enum.TryParse<RecurringExpenseFrequency>(frequency, ignoreCase: true, out var parsed)
            && Enum.IsDefined(parsed);
    }
}
