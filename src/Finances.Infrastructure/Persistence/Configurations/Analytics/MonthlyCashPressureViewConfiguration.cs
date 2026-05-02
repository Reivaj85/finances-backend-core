using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Analytics;

public sealed class MonthlyCashPressureViewConfiguration : IEntityTypeConfiguration<MonthlyCashPressureView>
{
    public void Configure(EntityTypeBuilder<MonthlyCashPressureView> builder)
    {
        builder.HasNoKey();
        builder.ToView("v_monthly_cash_pressure", "analytics");

        builder.Property(view => view.HouseholdId)
            .HasColumnName("household_id");

        builder.Property(view => view.Period)
            .HasColumnName("period");

        builder.Property(view => view.ExpectedIncomeTotal)
            .HasColumnName("expected_income_total")
            .HasPrecision(18, 2);

        builder.Property(view => view.ActiveRecurringExpenseTotal)
            .HasColumnName("active_recurring_expense_total")
            .HasPrecision(18, 2);

        builder.Property(view => view.MonthlyCashPressure)
            .HasColumnName("monthly_cash_pressure")
            .HasPrecision(18, 2);

        builder.Property(view => view.EstimatedFreeCashFlow)
            .HasColumnName("estimated_free_cash_flow")
            .HasPrecision(18, 2);

        builder.Property(view => view.IsIncomplete)
            .HasColumnName("is_incomplete");
    }
}
