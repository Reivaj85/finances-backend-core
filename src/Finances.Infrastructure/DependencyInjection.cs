using Finances.Application.RecurringExpenses.Abstractions;
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

        return services;
    }
}
