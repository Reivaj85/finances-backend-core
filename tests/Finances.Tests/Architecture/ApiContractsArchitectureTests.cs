using System.Text.RegularExpressions;

namespace Finances.Tests.Architecture;

public sealed class ApiContractsArchitectureTests
{
    private static readonly Regex EmbeddedContractRegex = new(
        @"\b(record|class|struct)\s+\w+(Request|Response|Dto)\b|\b(record|class|struct)\s+ErrorResponse\b",
        RegexOptions.Compiled);
    private static readonly Regex PublicTypeDeclarationRegex = new(
        @"^\s*public\s+(?:sealed\s+|abstract\s+|static\s+|partial\s+|readonly\s+)*(?:record|class|struct)\s+([A-Za-z_]\w*)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    [Fact]
    public void Endpoints_ShouldExistOnlyInsideEndpointsDirectory()
    {
        var apiRootPath = ResolveApiRootPath();
        var endpointFiles = Directory.GetFiles(apiRootPath, "*Endpoints.cs", SearchOption.AllDirectories);

        Assert.NotEmpty(endpointFiles);

        var invalidEndpointPaths = endpointFiles
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}Endpoints{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();

        Assert.True(
            invalidEndpointPaths.Count == 0,
            $"Endpoints fuera de 'Finances.Api/Endpoints': {string.Join(", ", invalidEndpointPaths)}");
    }

    [Fact]
    public void EndpointFiles_ShouldNotDeclareEmbeddedContracts()
    {
        var endpointDirectory = Path.Combine(ResolveApiRootPath(), "Endpoints");
        var endpointFiles = Directory.GetFiles(endpointDirectory, "*Endpoints.cs", SearchOption.AllDirectories);

        Assert.NotEmpty(endpointFiles);

        var violations = new List<string>();

        foreach (var endpointFile in endpointFiles)
        {
            var code = File.ReadAllText(endpointFile);

            if (EmbeddedContractRegex.IsMatch(code))
            {
                violations.Add(endpointFile);
            }
        }

        Assert.True(
            violations.Count == 0,
            $"Se detectaron contratos embebidos en endpoints: {string.Join(", ", violations)}");
    }

    [Fact]
    public void ContractFiles_ShouldDeclareSinglePublicType_WithMatchingFileName()
    {
        var contractsDirectory = Path.Combine(ResolveApiRootPath(), "Contracts");
        var contractFiles = Directory.GetFiles(contractsDirectory, "*.cs", SearchOption.AllDirectories);

        Assert.NotEmpty(contractFiles);

        var violations = new List<string>();

        foreach (var contractFile in contractFiles)
        {
            var code = File.ReadAllText(contractFile);
            var matches = PublicTypeDeclarationRegex.Matches(code);

            if (matches.Count != 1)
            {
                violations.Add($"{contractFile} (declaraciones públicas encontradas: {matches.Count})");
                continue;
            }

            var declaredTypeName = matches[0].Groups[1].Value;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(contractFile);

            if (!string.Equals(declaredTypeName, fileNameWithoutExtension, StringComparison.Ordinal))
            {
                violations.Add(
                    $"{contractFile} (tipo: {declaredTypeName}, archivo: {fileNameWithoutExtension})");
            }
        }

        Assert.True(
            violations.Count == 0,
            $"Contratos inválidos (1 tipo público por archivo y nombre consistente): {string.Join(", ", violations)}");
    }

    private static string ResolveApiRootPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src", "Finances.Api");
    }
}
