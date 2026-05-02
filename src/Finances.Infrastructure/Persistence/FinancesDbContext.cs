using Finances.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Finances.Infrastructure.Persistence;

public sealed class FinancesDbContext(DbContextOptions<FinancesDbContext> options) : DbContext(options)
{
    public DbSet<HouseholdRecord> Households => Set<HouseholdRecord>();

    public DbSet<CategoryRecord> Categories => Set<CategoryRecord>();

    public DbSet<RecurringExpenseRecord> RecurringExpenses => Set<RecurringExpenseRecord>();

    public DbSet<IncomeSourceRecord> IncomeSources => Set<IncomeSourceRecord>();

    public DbSet<IncomeRecordRecord> IncomeRecords => Set<IncomeRecordRecord>();

    public DbSet<MonthlyCashPressureView> MonthlyCashPressure => Set<MonthlyCashPressureView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinancesDbContext).Assembly);
    }
}
