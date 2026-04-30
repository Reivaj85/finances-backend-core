using Finances.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiDocumentation()
    .AddApiDependencies(builder.Configuration);

builder.Host.AddWolverineMessaging();

var app = builder.Build();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapDevelopmentOpenApi();
}

app.MapApiEndpoints();

app.Run();

public partial class Program;
