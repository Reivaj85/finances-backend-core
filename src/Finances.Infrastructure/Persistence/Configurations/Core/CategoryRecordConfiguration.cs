using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finances.Infrastructure.Persistence.Configurations.Core;

public sealed class CategoryRecordConfiguration : IEntityTypeConfiguration<CategoryRecord>
{
    public void Configure(EntityTypeBuilder<CategoryRecord> builder)
    {
        builder.ToTable("categories", "core");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.Id)
            .HasColumnName("id");

        builder.Property(category => category.Name)
            .HasColumnName("name")
            .HasMaxLength(120)
            .IsRequired();
    }
}
