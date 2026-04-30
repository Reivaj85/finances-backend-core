using Microsoft.AspNetCore.Builder;
using Wolverine.Http;

namespace Finances.Api.Configurations;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapWolverineEndpoints();
        return app;
    }

    public static WebApplication MapDevelopmentOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        return app;
    }
}
