using Finances.Domain.Common;
using Finances.Domain.IncomeSources;
using Finances.Domain.ValueObjects;

namespace Finances.Domain.IncomeRecords;

public sealed class IncomeRecord : AggregateRoot<IncomeRecordId>
{
    private IncomeRecord(
        IncomeRecordId id,
        IncomeSourceId incomeSourceId,
        Money amount,
        DateOnly receivedOn)
        : base(id)
    {
        IncomeSourceId = incomeSourceId;
        Amount = amount;
        ReceivedOn = receivedOn;
    }

    public IncomeSourceId IncomeSourceId { get; }

    public Money Amount { get; }

    public DateOnly ReceivedOn { get; }

    public static Result<IncomeRecord> Create(
        IncomeRecordId id,
        IncomeSourceId incomeSourceId,
        Money amount,
        DateOnly receivedOn)
    {
        return Result<IncomeRecord>.Success(new IncomeRecord(
            id,
            incomeSourceId,
            amount,
            receivedOn));
    }
}
