using FluentValidation;

namespace Finances.Application.RecurringExpenses.Queries;

public sealed class ListRecurringExpensesQueryValidator : AbstractValidator<ListRecurringExpensesQuery>
{
    public ListRecurringExpensesQueryValidator()
    {
        RuleFor(query => query.HouseholdId.Value)
            .NotEmpty()
            .WithErrorCode("RecurringExpense.HouseholdRequired")
            .WithMessage("El hogar es obligatorio.");
    }
}
