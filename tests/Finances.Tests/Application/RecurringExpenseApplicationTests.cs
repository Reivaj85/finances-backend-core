using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Application.RecurringExpenses.Commands;
using Finances.Application.RecurringExpenses.Queries;
using Finances.Domain.Categories;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;

namespace Finances.Tests.Application;

public sealed class RecurringExpenseApplicationTests
{
    private static readonly HouseholdId HouseholdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly CategoryId CategoryId = CategoryId.From(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

    [Fact]
    public async Task CreateRecurringExpense_ShouldPersistActiveExpense_WhenCommandIsValid()
    {
        var repository = new InMemoryRecurringExpenseRepository();
        var command = new CreateRecurringExpenseCommand(
            HouseholdId,
            CategoryId,
            "Colegio Demo",
            850_000m,
            "COP",
            "monthly");

        var result = await CreateRecurringExpenseHandler.Handle(command, repository, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Colegio Demo", result.Value.Name);
        Assert.Equal("active", result.Value.Status);
        Assert.True(result.Value.ContributesToMonthlyCashPressure);
        Assert.Single(await repository.ListByHouseholdAsync(HouseholdId, CancellationToken.None));
    }

    [Fact]
    public async Task CreateRecurringExpense_ShouldRejectCommand_WhenExpectedAmountIsNegative()
    {
        var repository = new InMemoryRecurringExpenseRepository();
        var command = new CreateRecurringExpenseCommand(
            HouseholdId,
            CategoryId,
            "Colegio Demo",
            -10m,
            "COP",
            "monthly");

        var result = await CreateRecurringExpenseHandler.Handle(command, repository, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Money.NegativeAmount", result.Error?.Code);
        Assert.Empty(await repository.ListByHouseholdAsync(HouseholdId, CancellationToken.None));
    }

    [Fact]
    public async Task ListRecurringExpenses_ShouldReturnOnlyExpensesForHousehold_WhenRepositoryHasMultipleHouseholds()
    {
        var repository = new InMemoryRecurringExpenseRepository();
        var otherHouseholdId = HouseholdId.From(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"));

        await CreateRecurringExpenseHandler.Handle(
            new CreateRecurringExpenseCommand(HouseholdId, CategoryId, "Internet Demo", 120_000m, "COP", "monthly"),
            repository,
            CancellationToken.None);
        await CreateRecurringExpenseHandler.Handle(
            new CreateRecurringExpenseCommand(otherHouseholdId, CategoryId, "Mercado Demo", 500_000m, "COP", "monthly"),
            repository,
            CancellationToken.None);

        var response = await ListRecurringExpensesHandler.Handle(
            new ListRecurringExpensesQuery(HouseholdId),
            repository,
            CancellationToken.None);

        var recurringExpense = Assert.Single(response);
        Assert.Equal("Internet Demo", recurringExpense.Name);
    }

    private sealed class InMemoryRecurringExpenseRepository : IRecurringExpenseRepository
    {
        private readonly List<RecurringExpense> recurringExpenses = [];

        public Task AddAsync(RecurringExpense recurringExpense, CancellationToken cancellationToken)
        {
            recurringExpenses.Add(recurringExpense);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<RecurringExpense>> ListByHouseholdAsync(HouseholdId householdId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<RecurringExpense>>(
                recurringExpenses.Where(recurringExpense => recurringExpense.HouseholdId == householdId).ToArray());
        }
    }
}
