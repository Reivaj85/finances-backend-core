namespace Finances.Tests.Architecture;

public sealed class ProgramArchitectureTests
{
    [Fact]
    public void Program_ShouldNotDeclareMinimalApiMappings_WhenUsingWolverineEndpointsConvention()
    {
        var programPath = ResolveProgramPath();
        var programCode = File.ReadAllText(programPath);

        var forbiddenMappings = new[]
        {
            "app.MapGet(",
            "app.MapPost(",
            "app.MapPut(",
            "app.MapDelete(",
            "app.MapPatch(",
            "app.MapGroup("
        };

        foreach (var forbiddenMapping in forbiddenMappings)
        {
            Assert.DoesNotContain(forbiddenMapping, programCode, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Program_ShouldUseCompositionRootExtensions_ForApiBootstrap()
    {
        var programPath = ResolveProgramPath();
        var programCode = File.ReadAllText(programPath);

        var expectedCalls = new[]
        {
            "AddApiDocumentation(",
            "AddApiDependencies(",
            "AddWolverineMessaging(",
            "UseGlobalExceptionHandling(",
            "MapDevelopmentOpenApi(",
            "MapApiEndpoints("
        };

        foreach (var expectedCall in expectedCalls)
        {
            Assert.Contains(expectedCall, programCode, StringComparison.Ordinal);
        }

        Assert.DoesNotContain("builder.Services.AddOpenApi(", programCode, StringComparison.Ordinal);
        Assert.DoesNotContain("builder.Services.AddWolverineHttp(", programCode, StringComparison.Ordinal);
        Assert.DoesNotContain("builder.Host.UseWolverine(", programCode, StringComparison.Ordinal);
        Assert.DoesNotContain("app.MapWolverineEndpoints(", programCode, StringComparison.Ordinal);
        Assert.DoesNotContain("app.Use(async", programCode, StringComparison.Ordinal);
    }

    [Fact]
    public void Api_ShouldDeclareConfigurationExtensions_InsideConfigurationsDirectory()
    {
        var apiRootPath = ResolveApiRootPath();
        var configurationsPath = Path.Combine(apiRootPath, "Configurations");

        Assert.True(Directory.Exists(configurationsPath), "No existe el directorio Finances.Api/Configurations.");

        var expectedFiles = new[]
        {
            "ServiceCollectionExtensions.cs",
            "WolverineConfigurationExtensions.cs",
            "MiddlewareExtensions.cs",
            "EndpointRouteBuilderExtensions.cs"
        };

        foreach (var expectedFile in expectedFiles)
        {
            var fullPath = Path.Combine(configurationsPath, expectedFile);
            Assert.True(File.Exists(fullPath), $"No existe el archivo de configuración esperado: {fullPath}");
        }
    }

    private static string ResolveProgramPath()
    {
        return Path.Combine(ResolveApiRootPath(), "Program.cs");
    }

    private static string ResolveApiRootPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src", "Finances.Api");
    }
}
