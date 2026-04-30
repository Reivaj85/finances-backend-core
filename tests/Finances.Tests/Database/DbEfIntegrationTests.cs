using Finances.Domain.Categories;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;
using Finances.Domain.ValueObjects;
using Finances.Infrastructure.Persistence;
using Finances.Infrastructure.RecurringExpenses;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Finances.Tests.Database;

public sealed class DbEfIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("finances_demo")
        .WithUsername("demo_user")
        .WithPassword("demo_password")
        .Build();

    private FinancesDbContext dbContext = null!;

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();

        var migrationResult = DatabaseMigrationRunner.Run(postgres.GetConnectionString());
        Assert.True(migrationResult.Successful, migrationResult.Error?.Message);

        var options = new DbContextOptionsBuilder<FinancesDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        dbContext = new FinancesDbContext(options);
    }

    public async Task DisposeAsync()
    {
        await dbContext.DisposeAsync();
        await postgres.DisposeAsync();
    }

    [Fact]
    public async Task DbContext_ShouldMapHouseholdsTable()
    {
        var id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        dbContext.Households.Add(new()
        {
            Id = id,
            Name = "Hogar Demo"
        });
        await dbContext.SaveChangesAsync();

        var household = await dbContext.Households.SingleAsync(record => record.Id == id);

        Assert.Equal("Hogar Demo", household.Name);
    }

    [Fact]
    public async Task DbContext_ShouldMapCategoriesTable()
    {
        var id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        dbContext.Categories.Add(new()
        {
            Id = id,
            Name = "Cuenta Demo"
        });
        await dbContext.SaveChangesAsync();

        var category = await dbContext.Categories.SingleAsync(record => record.Id == id);

        Assert.Equal("Cuenta Demo", category.Name);
    }

    [Fact]
    public async Task DbContext_ShouldMapRecurringExpensesTable()
    {
        var householdId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var categoryId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var recurringExpenseId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        await dbContext.Households.AddAsync(new()
        {
            Id = householdId,
            Name = "Hogar Demo"
        });
        await dbContext.Categories.AddAsync(new()
        {
            Id = categoryId,
            Name = "Colegio Demo"
        });
        await dbContext.SaveChangesAsync();

        dbContext.RecurringExpenses.Add(new()
        {
            Id = recurringExpenseId,
            HouseholdId = householdId,
            CategoryId = categoryId,
            Name = "Colegio Demo",
            ExpectedAmount = 850_000m,
            Currency = "COP",
            Frequency = "monthly",
            Status = "active"
        });
        await dbContext.SaveChangesAsync();

        var recurringExpense = await dbContext.RecurringExpenses.SingleAsync(record => record.Id == recurringExpenseId);

        Assert.Equal("Colegio Demo", recurringExpense.Name);
        Assert.Equal(850_000m, recurringExpense.ExpectedAmount);
        Assert.Equal("active", recurringExpense.Status);
    }

    [Fact]
    public async Task CategorySeed_ShouldExist_AfterMigration()
    {
        var seededCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var seededCategory = await dbContext.Categories.SingleOrDefaultAsync(record => record.Id == seededCategoryId);

        Assert.NotNull(seededCategory);
        Assert.Equal("Educación Demo", seededCategory.Name);
    }

    [Fact]
    public async Task RecurringExpenseRepository_ShouldPersistAndReadExpense()
    {
        var repository = new EfRecurringExpenseRepository(dbContext);
        var householdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        var categoryId = CategoryId.From(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var recurringExpenseId = RecurringExpenseId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));

        await dbContext.Households.AddAsync(new()
        {
            Id = householdId.Value,
            Name = "Hogar Demo"
        });
        await dbContext.SaveChangesAsync();

        var currency = Currency.Create("COP").Value;
        var amount = Money.Create(120_000m, currency).Value;
        var recurringExpense = RecurringExpense.Create(
            recurringExpenseId,
            householdId,
            categoryId,
            "Internet Demo",
            amount,
            RecurringExpenseFrequency.Monthly,
            RecurringExpenseStatus.Active).Value;

        await repository.AddAsync(recurringExpense, CancellationToken.None);

        var result = await repository.ListByHouseholdAsync(householdId, CancellationToken.None);
        var persisted = Assert.Single(result);

        Assert.Equal("Internet Demo", persisted.Name);
        Assert.Equal(120_000m, persisted.ExpectedAmount.Amount);
        Assert.True(persisted.ContributesToMonthlyCashPressure());
    }
}
