using Finances.Domain.Common;

namespace Finances.Domain.RecurringExpenses;

public sealed record RecurringExpenseCancelledDomainEvent(RecurringExpenseId RecurringExpenseId) : IDomainEvent;
