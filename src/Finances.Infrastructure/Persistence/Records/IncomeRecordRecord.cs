namespace Finances.Infrastructure.Persistence.Records;

public sealed class IncomeRecordRecord
{
    public Guid Id { get; set; }

    public Guid IncomeSourceId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public DateOnly ReceivedOn { get; set; }
}
