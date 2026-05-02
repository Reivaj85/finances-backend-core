using Finances.Application.Analytics.Abstractions;
using Finances.Application.Analytics.Contracts;
using Finances.Application.Analytics.Queries;
using Finances.Application.IncomeRecords.Abstractions;
using Finances.Application.IncomeRecords.Commands;
using Finances.Application.IncomeSources.Abstractions;
using Finances.Application.IncomeSources.Commands;
using Finances.Application.IncomeSources.Queries;
using Finances.Domain.Households;
using Finances.Domain.IncomeRecords;
using Finances.Domain.IncomeSources;

namespace Finances.Tests.Application;

public sealed class IncomeApplicationTests
{
    private static readonly HouseholdId HouseholdId = HouseholdId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    [Fact]
    public async Task CreateIncomeSource_ShouldPersistActiveSource_WhenCommandIsValid()
    {
        var repository = new InMemoryIncomeSourceRepository();
        var command = new CreateIncomeSourceCommand(
            HouseholdId,
            "Empresa Demo S.A.",
            2_000_000m,
            "COP",
            "monthly",
            "stable");

        var result = await CreateIncomeSourceHandler.Handle(command, repository, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Empresa Demo S.A.", result.Value.Name);
        Assert.Equal("active", result.Value.Status);
        Assert.True(result.Value.ContributesExpectedMonthlyIncome);
        Assert.Single(await repository.ListByHouseholdAsync(HouseholdId, CancellationToken.None));
    }

    [Fact]
    public async Task CreateIncomeSource_ShouldRejectCommand_WhenExpectedAmountIsNegative()
    {
        var repository = new InMemoryIncomeSourceRepository();
        var command = new CreateIncomeSourceCommand(
            HouseholdId,
            "Empresa Demo S.A.",
            -1m,
            "COP",
            "monthly",
            "stable");

        var result = await CreateIncomeSourceHandler.Handle(command, repository, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Money.NegativeAmount", result.Error?.Code);
        Assert.Empty(await repository.ListByHouseholdAsync(HouseholdId, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterIncomeRecord_ShouldPersistReceivedIncome_WhenCommandIsValid()
    {
        var repository = new InMemoryIncomeRecordRepository();
        var incomeSourceId = IncomeSourceId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var command = new RegisterIncomeRecordCommand(
            incomeSourceId,
            1_950_000m,
            "COP",
            new DateOnly(2026, 4, 15));

        var result = await RegisterIncomeRecordHandler.Handle(command, repository, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(incomeSourceId, result.Value.IncomeSourceId);
        Assert.Equal(1_950_000m, result.Value.Amount);
        Assert.Single(await repository.ListBySourceAsync(incomeSourceId, CancellationToken.None));
    }

    [Fact]
    public async Task MonthlyCashPressure_ShouldReturnAggregates_WhenReaderHasData()
    {
        var reader = new InMemoryMonthlyCashPressureReader([
            new MonthlyCashPressureResponse(
                HouseholdId,
                new DateOnly(2026, 4, 1),
                2_000_000m,
                1_250_000m,
                1_250_000m,
                750_000m,
                false)
        ]);

        var response = await GetMonthlyCashPressureHandler.Handle(
            new GetMonthlyCashPressureQuery(HouseholdId),
            reader,
            CancellationToken.None);

        var summary = Assert.Single(response);
        Assert.Equal(750_000m, summary.EstimatedFreeCashFlow);
        Assert.False(summary.IsIncomplete);
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
        private readonly List<IncomeRecord> incomeRecords = [];

        public Task AddAsync(IncomeRecord incomeRecord, CancellationToken cancellationToken)
        {
            incomeRecords.Add(incomeRecord);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<IncomeRecord>> ListBySourceAsync(IncomeSourceId incomeSourceId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<IncomeRecord>>(
                incomeRecords.Where(incomeRecord => incomeRecord.IncomeSourceId == incomeSourceId).ToArray());
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
