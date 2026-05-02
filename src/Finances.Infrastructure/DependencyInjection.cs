using Finances.Application.Analytics.Abstractions;
using Finances.Application.IncomeRecords.Abstractions;
using Finances.Application.IncomeSources.Abstractions;
using Finances.Application.RecurringExpenses.Abstractions;
using Finances.Infrastructure.Analytics;
using Finances.Infrastructure.IncomeRecords;
using Finances.Infrastructure.IncomeSources;
using Finances.Infrastructure.Persistence;
using Finances.Infrastructure.RecurringExpenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finances.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinancesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FinancesDb")
            ?? Environment.GetEnvironmentVariable("FINANCES_DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        services.AddDbContext<FinancesDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IRecurringExpenseRepository, EfRecurringExpenseRepository>();
        services.AddScoped<IIncomeSourceRepository, EfIncomeSourceRepository>();
        services.AddScoped<IIncomeRecordRepository, EfIncomeRecordRepository>();
        services.AddScoped<IMonthlyCashPressureReader, EfMonthlyCashPressureReader>();

        return services;
    }
}
