using System.Text.RegularExpressions;

namespace Finances.Tests.Architecture;

public sealed class StronglyTypedIdsArchitectureTests
{
    private static readonly Regex RawEntityIdPattern = new(
        @"\bGuid\s+[_a-zA-Z0-9]*Id\b",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Theory]
    [InlineData("src/Finances.Domain")]
    [InlineData("src/Finances.Application")]
    [InlineData("src/Finances.Api/Contracts")]
    public void DomainApplicationAndApiContracts_ShouldNotUseRawGuidEntityIds_WhenStronglyTypedIdsAreRequired(
        string relativePath)
    {
        var rootPath = ResolveRootPath();
        var targetPath = Path.Combine(rootPath, relativePath);
        var violations = Directory
            .GetFiles(targetPath, "*.cs", SearchOption.AllDirectories)
            .Where(filePath => !filePath.EndsWith("Id.cs", StringComparison.Ordinal))
            .SelectMany(filePath => FindViolations(filePath, rootPath))
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Raw entity IDs found:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    }

    private static IEnumerable<string> FindViolations(string filePath, string rootPath)
    {
        var lines = File.ReadAllLines(filePath);

        for (var index = 0; index < lines.Length; index++)
        {
            if (RawEntityIdPattern.IsMatch(lines[index]))
            {
                yield return $"{Path.GetRelativePath(rootPath, filePath)}:{index + 1}: {lines[index].Trim()}";
            }
        }
    }

    private static string ResolveRootPath()
    {
        return Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }
}
