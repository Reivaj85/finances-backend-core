namespace Finances.Tests.Architecture;

public sealed class ResultPatternArchitectureTests
{
    [Fact]
    public void SourceCode_ShouldNotUseDomainException_ForBusinessRuleFlow()
    {
        var sourceRootPath = ResolveSourceRootPath();
        var sourceFiles = Directory.GetFiles(sourceRootPath, "*.cs", SearchOption.AllDirectories);

        var forbiddenPatterns = new[]
        {
            "DomainException",
            "throw new DomainException",
            "catch (DomainException"
        };

        var violations = sourceFiles
            .Where(file => forbiddenPatterns.Any(pattern =>
                File.ReadAllText(file).Contains(pattern, StringComparison.Ordinal)))
            .ToList();

        Assert.True(
            violations.Count == 0,
            $"No se debe usar DomainException para flujo de negocio. Archivos: {string.Join(", ", violations)}");
    }

    private static string ResolveSourceRootPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src");
    }
}
