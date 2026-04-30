using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Domain.Categories;
using Finances.Domain.Common;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;
using Finances.Domain.ValueObjects;
using Finances.Infrastructure.Persistence;
using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Finances.Infrastructure.RecurringExpenses;

public sealed class EfRecurringExpenseRepository(FinancesDbContext dbContext) : IRecurringExpenseRepository
{
    public async Task AddAsync(RecurringExpense recurringExpense, CancellationToken cancellationToken)
    {
        dbContext.RecurringExpenses.Add(ToRecord(recurringExpense));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecurringExpense>> ListByHouseholdAsync(HouseholdId householdId, CancellationToken cancellationToken)
    {
        var records = await dbContext.RecurringExpenses
            .AsNoTracking()
            .Where(recurringExpense => recurringExpense.HouseholdId == householdId.Value)
            .OrderBy(recurringExpense => recurringExpense.Name)
            .ToArrayAsync(cancellationToken);

        return records.Select(ToDomain).ToArray();
    }

    private static RecurringExpenseRecord ToRecord(RecurringExpense recurringExpense)
    {
        return new RecurringExpenseRecord
        {
            Id = recurringExpense.Id.Value,
            HouseholdId = recurringExpense.HouseholdId.Value,
            CategoryId = recurringExpense.CategoryId.Value,
            Name = recurringExpense.Name,
            ExpectedAmount = recurringExpense.ExpectedAmount.Amount,
            Currency = recurringExpense.ExpectedAmount.Currency.Code,
            Frequency = recurringExpense.Frequency.ToString().ToLowerInvariant(),
            Status = recurringExpense.Status.ToString().ToLowerInvariant()
        };
    }

    private static RecurringExpense ToDomain(RecurringExpenseRecord record)
    {
        var currency = EnsureSuccess(Currency.Create(record.Currency));
        var money = EnsureSuccess(Money.Create(record.ExpectedAmount, currency));

        return EnsureSuccess(RecurringExpense.Create(
            RecurringExpenseId.From(record.Id),
            HouseholdId.From(record.HouseholdId),
            CategoryId.From(record.CategoryId),
            record.Name,
            money,
            Enum.Parse<RecurringExpenseFrequency>(record.Frequency, ignoreCase: true),
            Enum.Parse<RecurringExpenseStatus>(record.Status, ignoreCase: true)));
    }

    private static T EnsureSuccess<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"El registro persistido no pudo materializarse: {result.Error!.Code} - {result.Error.Description}");
        }

        return result.Value;
    }
}
