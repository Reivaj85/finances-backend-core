using System.Net;
using System.Net.Http.Json;
using Finances.Application.Analytics.Abstractions;
using Finances.Application.Analytics.Contracts;
using Finances.Application.IncomeRecords.Abstractions;
using Finances.Application.IncomeSources.Abstractions;
using Finances.Application.IncomeSources.Contracts;
using Finances.Domain.Households;
using Finances.Domain.IncomeRecords;
using Finances.Domain.IncomeSources;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finances.Tests.Api;

public sealed class IncomeAndAnalyticsContractTests
{
    private static readonly Guid HouseholdId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task PostIncomeSources_ShouldReturnCreated_WhenRequestIsValid()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/income-sources",
            new
            {
                householdId = HouseholdId,
                name = "Empresa Demo S.A.",
                expectedAmount = 2_000_000m,
                currency = "COP",
                frequency = "monthly",
                stability = "stable"
            });

        var body = await response.Content.ReadFromJsonAsync<IncomeSourceResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Empresa Demo S.A.", body.Name);
        Assert.True(body.ContributesExpectedMonthlyIncome);
    }

    [Fact]
    public async Task PostIncomeSources_ShouldReturnBadRequest_WhenValidationFails()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/income-sources",
            new
            {
                householdId = HouseholdId,
                name = "Empresa Demo S.A.",
                expectedAmount = -1m,
                currency = "COP",
                frequency = "monthly",
                stability = "stable"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIncomeSources_ShouldReturnCreatedSources_WhenHouseholdHasIncome()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync(
            "/income-sources",
            new
            {
                householdId = HouseholdId,
                name = "Empresa Demo S.A.",
                expectedAmount = 2_000_000m,
                currency = "COP",
                frequency = "monthly",
                stability = "stable"
            });

        var response = await client.GetAsync($"/income-sources?householdId={HouseholdId}");
        var body = await response.Content.ReadFromJsonAsync<IncomeSourceResponse[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var incomeSource = Assert.Single(body ?? []);
        Assert.Equal("Empresa Demo S.A.", incomeSource.Name);
    }

    [Fact]
    public async Task PostIncomeRecords_ShouldReturnCreated_WhenRequestIsValid()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var incomeSourceId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var response = await client.PostAsJsonAsync(
            "/income-records",
            new
            {
                incomeSourceId,
                amount = 1_950_000m,
                currency = "COP",
                receivedOn = new DateOnly(2026, 4, 15)
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetMonthlyCashPressure_ShouldReturnAnalyticsDto_WhenHouseholdHasSummary()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/analytics/monthly-cash-pressure?householdId={HouseholdId}");
        var body = await response.Content.ReadFromJsonAsync<MonthlyCashPressureResponse[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = Assert.Single(body ?? []);
        Assert.Equal(750_000m, summary.EstimatedFreeCashFlow);
        Assert.False(summary.IsIncomplete);
    }

    [Fact]
    public async Task GetMonthlyCashPressure_ShouldReturnBadRequest_WhenHouseholdIdIsMissing()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/analytics/monthly-cash-pressure");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IIncomeSourceRepository>();
                    services.RemoveAll<IIncomeRecordRepository>();
                    services.RemoveAll<IMonthlyCashPressureReader>();
                    services.AddSingleton<IIncomeSourceRepository, InMemoryIncomeSourceRepository>();
                    services.AddSingleton<IIncomeRecordRepository, InMemoryIncomeRecordRepository>();
                    services.AddSingleton<IMonthlyCashPressureReader>(_ => new InMemoryMonthlyCashPressureReader([
                        new MonthlyCashPressureResponse(
                            Finances.Domain.Households.HouseholdId.From(IncomeAndAnalyticsContractTests.HouseholdId),
                            new DateOnly(2026, 4, 1),
                            2_000_000m,
                            1_250_000m,
                            1_250_000m,
                            750_000m,
                            false)
                    ]));
                });
            });
    }

    private sealed class InMemoryIncomeSourceRepository : IIncomeSourceRepository
    {
        private readonly List<IncomeSource> incomeSources = [];

        public Task AddAsync(IncomeSource incomeSource, CancellationToken cancellationToken)
        {
            incomeSources.Add(incomeSource);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<IncomeSource>> ListByHouseholdAsync(HouseholdId householdId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<IncomeSource>>(
                incomeSources.Where(incomeSource => incomeSource.HouseholdId == householdId).ToArray());
        }
    }

    private sealed class InMemoryIncomeRecordRepository : IIncomeRecordRepository
    {
        public Task AddAsync(IncomeRecord incomeRecord, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<IncomeRecord>> ListBySourceAsync(IncomeSourceId incomeSourceId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<IncomeRecord>>([]);
        }
    }

    private sealed class InMemoryMonthlyCashPressureReader(
        IReadOnlyList<MonthlyCashPressureResponse> responses) : IMonthlyCashPressureReader
    {
        public Task<IReadOnlyList<MonthlyCashPressureResponse>> ListByHouseholdAsync(
            HouseholdId householdId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<MonthlyCashPressureResponse>>(
                responses.Where(response => response.HouseholdId == householdId).ToArray());
        }
    }
}
