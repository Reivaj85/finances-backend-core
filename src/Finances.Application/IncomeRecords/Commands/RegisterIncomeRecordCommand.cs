using Finances.Domain.IncomeSources;

namespace Finances.Application.IncomeRecords.Commands;

public sealed record RegisterIncomeRecordCommand(
    IncomeSourceId IncomeSourceId,
    decimal Amount,
    string Currency,
    DateOnly ReceivedOn);
