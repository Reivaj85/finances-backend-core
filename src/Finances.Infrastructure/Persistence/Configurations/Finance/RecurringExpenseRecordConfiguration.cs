using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Finance;

public sealed class RecurringExpenseRecordConfiguration : IEntityTypeConfiguration<RecurringExpenseRecord>
{
    public void Configure(EntityTypeBuilder<RecurringExpenseRecord> builder)
    {
        builder.ToTable("recurring_expenses", "finance");

        builder.HasKey(recurringExpense => recurringExpense.Id);

        builder.Property(recurringExpense => recurringExpense.Id)
            .HasColumnName("id");

        builder.Property(recurringExpense => recurringExpense.HouseholdId)
            .HasColumnName("household_id");

        builder.Property(recurringExpense => recurringExpense.CategoryId)
            .HasColumnName("category_id");

        builder.Property(recurringExpense => recurringExpense.Name)
            .HasColumnName("name")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(recurringExpense => recurringExpense.ExpectedAmount)
            .HasColumnName("expected_amount")
            .HasPrecision(18, 2);

        builder.Property(recurringExpense => recurringExpense.Currency)
            .HasColumnName("currency")
            .HasColumnType("char(3)")
            .IsRequired();

        builder.Property(recurringExpense => recurringExpense.Frequency)
            .HasColumnName("frequency")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(recurringExpense => recurringExpense.Status)
            .HasColumnName("status")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(recurringExpense => recurringExpense.HouseholdId)
            .HasDatabaseName("ix_recurring_expenses_household_id");

        builder.HasIndex(recurringExpense => recurringExpense.CategoryId)
            .HasDatabaseName("ix_recurring_expenses_category_id");

        builder.HasIndex(recurringExpense => recurringExpense.Status)
            .HasDatabaseName("ix_recurring_expenses_status");
    }
}
