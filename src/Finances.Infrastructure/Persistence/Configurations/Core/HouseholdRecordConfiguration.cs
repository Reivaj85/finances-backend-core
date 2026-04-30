using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Core;

public sealed class HouseholdRecordConfiguration : IEntityTypeConfiguration<HouseholdRecord>
{
    public void Configure(EntityTypeBuilder<HouseholdRecord> builder)
    {
        builder.ToTable("households", "core");

        builder.HasKey(household => household.Id);

        builder.Property(household => household.Id)
            .HasColumnName("id");

        builder.Property(household => household.Name)
            .HasColumnName("name")
            .HasMaxLength(160)
            .IsRequired();
    }
}
