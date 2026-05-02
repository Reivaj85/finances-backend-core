namespace Finances.Infrastructure.Persistence.Records;

public sealed class MonthlyCashPressureView
{
    public Guid HouseholdId { get; set; }

    public DateOnly Period { get; set; }

    public decimal ExpectedIncomeTotal { get; set; }

    public decimal ActiveRecurringExpenseTotal { get; set; }

    public decimal MonthlyCashPressure { get; set; }

    public decimal EstimatedFreeCashFlow { get; set; }

    public bool IsIncomplete { get; set; }
}
