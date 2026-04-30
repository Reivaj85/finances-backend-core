namespace Finances.Infrastructure.Persistence.Records;

public sealed class RecurringExpenseRecord
{
    public Guid Id { get; set; }

    public Guid HouseholdId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal ExpectedAmount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
