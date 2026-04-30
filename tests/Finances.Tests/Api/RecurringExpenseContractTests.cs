using System.Net;
using System.Net.Http.Json;
using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Application.RecurringExpenses.Contracts;
using Finances.Domain.Households;
using Finances.Domain.RecurringExpenses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finances.Tests.Api;

public sealed class RecurringExpenseContractTests
{
    private static readonly Guid HouseholdId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task Health_ShouldReturnHealthy_WhenApiStarts()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostRecurringExpenses_ShouldReturnCreated_WhenRequestIsValid()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/recurring-expenses",
            new
            {
                householdId = HouseholdId,
                categoryId = CategoryId,
                name = "Colegio Demo",
                expectedAmount = 850_000m,
                currency = "COP",
                frequency = "monthly"
            });

        var body = await response.Content.ReadFromJsonAsync<RecurringExpenseResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Colegio Demo", body.Name);
        Assert.True(body.ContributesToMonthlyCashPressure);
    }

    [Fact]
    public async Task PostRecurringExpenses_ShouldReturnBadRequest_WhenValidationFails()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/recurring-expenses",
            new
            {
                householdId = HouseholdId,
                categoryId = CategoryId,
                name = "Colegio Demo",
                expectedAmount = -1m,
                currency = "COP",
                frequency = "monthly"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecurringExpenses_ShouldReturnCreatedExpenses_WhenHouseholdHasExpenses()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync(
            "/recurring-expenses",
            new
            {
                householdId = HouseholdId,
                categoryId = CategoryId,
                name = "Internet Demo",
                expectedAmount = 120_000m,
                currency = "COP",
                frequency = "monthly"
            });

        var response = await client.GetAsync($"/recurring-expenses?householdId={HouseholdId}");
        var body = await response.Content.ReadFromJsonAsync<RecurringExpenseResponse[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var recurringExpense = Assert.Single(body ?? []);
        Assert.Equal("Internet Demo", recurringExpense.Name);
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IRecurringExpenseRepository>();
                    services.AddSingleton<IRecurringExpenseRepository, InMemoryRecurringExpenseRepository>();
                });
            });
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
