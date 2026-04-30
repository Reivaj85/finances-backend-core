using Finances.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Http;

namespace Finances.Api.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static IServiceCollection AddApiDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddWolverineHttp();
        services.AddFinancesInfrastructure(configuration);
        return services;
    }
}
