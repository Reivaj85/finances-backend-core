using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Finance;

public sealed class IncomeSourceRecordConfiguration : IEntityTypeConfiguration<IncomeSourceRecord>
{
    public void Configure(EntityTypeBuilder<IncomeSourceRecord> builder)
    {
        builder.ToTable("income_sources", "finance");

        builder.HasKey(incomeSource => incomeSource.Id);

        builder.Property(incomeSource => incomeSource.Id)
            .HasColumnName("id");

        builder.Property(incomeSource => incomeSource.HouseholdId)
            .HasColumnName("household_id");

        builder.Property(incomeSource => incomeSource.Name)
            .HasColumnName("name")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(incomeSource => incomeSource.ExpectedAmount)
            .HasColumnName("expected_amount")
            .HasPrecision(18, 2);

        builder.Property(incomeSource => incomeSource.Currency)
            .HasColumnName("currency")
            .HasColumnType("char(3)")
            .IsRequired();

        builder.Property(incomeSource => incomeSource.Frequency)
            .HasColumnName("frequency")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(incomeSource => incomeSource.Stability)
            .HasColumnName("stability")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(incomeSource => incomeSource.Status)
            .HasColumnName("status")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(incomeSource => incomeSource.HouseholdId)
            .HasDatabaseName("ix_income_sources_household_id");

        builder.HasIndex(incomeSource => incomeSource.Status)
            .HasDatabaseName("ix_income_sources_status");
    }
}
