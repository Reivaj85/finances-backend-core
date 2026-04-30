namespace Finances.Tests.Architecture;

/// <summary>
/// Evita que aparezcan archivos *Extensions.cs dispersos en Finances.Api fuera del composition root de configuración.
/// </summary>
public sealed class ApiExtensionsArchitectureTests
{
    [Fact]
    public void Api_ShouldDeclareExtensionClasses_OnlyInsideConfigurationsDirectory()
    {
        var apiRootPath = ResolveApiRootPath();
        var configurationsRoot = Path.GetFullPath(Path.Combine(apiRootPath, "Configurations"));

        var extensionFiles = Directory.GetFiles(apiRootPath, "*Extensions.cs", SearchOption.AllDirectories);

        var violations = extensionFiles
            .Where(path => !IsUnderDirectory(path, configurationsRoot))
            .ToList();

        Assert.True(
            violations.Count == 0,
            $"Archivos *Extensions.cs fuera de Finances.Api/Configurations: {string.Join(", ", violations)}");
    }

    private static bool IsUnderDirectory(string filePath, string directoryRoot)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (string.IsNullOrEmpty(directory))
        {
            return false;
        }

        var root = Path.GetFullPath(directoryRoot).TrimEnd(Path.DirectorySeparatorChar);
        directory = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);

        return string.Equals(directory, root, StringComparison.OrdinalIgnoreCase)
            || directory.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveApiRootPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src", "Finances.Api");
    }
}
