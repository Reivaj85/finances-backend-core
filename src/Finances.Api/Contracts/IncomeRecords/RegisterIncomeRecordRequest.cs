using Finances.Domain.IncomeSources;

namespace Finances.Api.Contracts.IncomeRecords;

public sealed record RegisterIncomeRecordRequest(
    IncomeSourceId IncomeSourceId,
    decimal Amount,
    string Currency,
    DateOnly ReceivedOn);
