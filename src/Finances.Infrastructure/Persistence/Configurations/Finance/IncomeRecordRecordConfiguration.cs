using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Finance;

public sealed class IncomeRecordRecordConfiguration : IEntityTypeConfiguration<IncomeRecordRecord>
{
    public void Configure(EntityTypeBuilder<IncomeRecordRecord> builder)
    {
        builder.ToTable("income_records", "finance");

        builder.HasKey(incomeRecord => incomeRecord.Id);

        builder.Property(incomeRecord => incomeRecord.Id)
            .HasColumnName("id");

        builder.Property(incomeRecord => incomeRecord.IncomeSourceId)
            .HasColumnName("income_source_id");

        builder.Property(incomeRecord => incomeRecord.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2);

        builder.Property(incomeRecord => incomeRecord.Currency)
            .HasColumnName("currency")
            .HasColumnType("char(3)")
            .IsRequired();

        builder.Property(incomeRecord => incomeRecord.ReceivedOn)
            .HasColumnName("received_on");

        builder.HasIndex(incomeRecord => incomeRecord.IncomeSourceId)
            .HasDatabaseName("ix_income_records_income_source_id");

        builder.HasIndex(incomeRecord => incomeRecord.ReceivedOn)
            .HasDatabaseName("ix_income_records_received_on");
    }
}
