using Finances.Application.RecurringExpenses.Commands;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

namespace Finances.Api.Configurations;

public static class WolverineConfigurationExtensions
{
    public static IHostBuilder AddWolverineMessaging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine(options =>
        {
            options.Discovery.IncludeAssembly(typeof(CreateRecurringExpenseHandler).Assembly);
            options.UseFluentValidation();
        });

        return hostBuilder;
    }
}
