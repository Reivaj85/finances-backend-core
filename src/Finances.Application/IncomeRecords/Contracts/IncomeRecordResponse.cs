using Finances.Domain.IncomeRecords;
using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeRecords.Contracts;

public sealed record IncomeRecordResponse(
    IncomeRecordId Id,
    IncomeSourceId IncomeSourceId,
    decimal Amount,
    string Currency,
    DateOnly ReceivedOn);
