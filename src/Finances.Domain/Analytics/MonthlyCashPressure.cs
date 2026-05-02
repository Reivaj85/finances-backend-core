namespace Finances.Domain.Analytics;

public sealed record MonthlyCashPressureResult(
    decimal ExpectedIncomeTotal,
    decimal ActiveRecurringExpenseTotal,
    decimal MonthlyCashPressure,
    decimal EstimatedFreeCashFlow,
    bool IsIncomplete);

public static class MonthlyCashPressure
{
    public static MonthlyCashPressureResult Calculate(
        decimal expectedIncomeTotal,
        decimal activeRecurringExpenseTotal)
    {
        return new MonthlyCashPressureResult(
            expectedIncomeTotal,
            activeRecurringExpenseTotal,
            activeRecurringExpenseTotal,
            expectedIncomeTotal - activeRecurringExpenseTotal,
            expectedIncomeTotal <= 0);
    }
}
