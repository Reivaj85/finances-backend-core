using Finances.Domain.Common;
using Finances.Domain.RecurringExpenses;
using System.Text.RegularExpressions;

namespace Finances.Tests.Architecture;

public sealed class DomainEntityArchitectureTests
{
    private static readonly Regex EntityClassRegex = new(
        @"public\s+sealed\s+class\s+(?<type>\w+)\s*(?<inheritance>:[^{]+)?\{[\s\S]*?\b\k<type>Id\s+id\b",
        RegexOptions.Compiled | RegexOptions.Multiline);

    [Fact]
    public void RecurringExpense_ShouldInheritAggregateRoot_WhenItHasIdentityAndDomainEvents()
    {
        Assert.True(typeof(AggregateRoot<RecurringExpenseId>).IsAssignableFrom(typeof(RecurringExpense)));
    }

    [Fact]
    public void DomainEntities_ShouldInheritEntityOrAggregateRoot_WhenTheyHaveIdentity()
    {
        var violations = DomainEntitySourceFiles()
            .Select(file => new
            {
                File = file,
                Match = EntityClassRegex.Match(File.ReadAllText(file))
            })
            .Where(candidate => candidate.Match.Success)
            .Where(candidate =>
            {
                var inheritance = candidate.Match.Groups["inheritance"].Value;

                return !inheritance.Contains("Entity<", StringComparison.Ordinal)
                    && !inheritance.Contains("AggregateRoot<", StringComparison.Ordinal);
            })
            .Select(candidate => candidate.File)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Toda entidad con identidad debe heredar de Entity<TId> o AggregateRoot<TId>: {string.Join(", ", violations)}");
    }

    [Fact]
    public void DomainEntities_ShouldNotDeclareManualIdProperty_WhenEntityBaseOwnsIdentity()
    {
        var violations = DomainEntitySourceFiles()
            .Where(file => File.ReadAllText(file).Contains("public ", StringComparison.Ordinal)
                && File.ReadAllText(file).Contains(" Id { get; }", StringComparison.Ordinal))
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Las entidades no deben declarar Id manualmente cuando heredan de Entity<TId>: {string.Join(", ", violations)}");
    }

    [Fact]
    public void DomainEntities_ShouldNotRepeatApplicationInputValidationGuards()
    {
        var forbiddenPatterns = new[]
        {
            "Guid.Empty",
            "string.IsNullOrWhiteSpace",
            "Enum.IsDefined"
        };

        var violations = DomainEntitySourceFiles()
            .SelectMany(file => forbiddenPatterns
                .Where(pattern => File.ReadAllText(file).Contains(pattern, StringComparison.Ordinal))
                .Select(pattern => $"{file}: {pattern}"))
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"Las entidades no deben repetir validaciones de entrada ya cubiertas por FluentValidation: {string.Join(", ", violations)}");
    }

    private static IEnumerable<string> DomainEntitySourceFiles()
    {
        return Directory.GetFiles(ResolveDomainPath(), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}Common{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}ValueObjects{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(file => !file.EndsWith("Id.cs", StringComparison.Ordinal))
            .Where(file => !file.EndsWith("Specification.cs", StringComparison.Ordinal))
            .Where(file => !file.EndsWith("DomainEvent.cs", StringComparison.Ordinal))
            .Where(file => !file.EndsWith("Status.cs", StringComparison.Ordinal))
            .Where(file => !file.EndsWith("Frequency.cs", StringComparison.Ordinal));
    }

    private static string ResolveDomainPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src", "Finances.Domain");
    }
}
