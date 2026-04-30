using Finances.Application.RecurringExpenses.Commands;
using Finances.Domain.RecurringExpenses;
using Finances.Infrastructure.Persistence;
using NetArchTest.Rules;

namespace Finances.Tests.Architecture;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Domain_ShouldNotDependOnApplicationInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(RecurringExpense).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Finances.Application",
                "Finances.Infrastructure",
                "Finances.Api",
                "Npgsql",
                "DbUp")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(CreateRecurringExpenseCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Finances.Infrastructure",
                "Finances.Api",
                "Microsoft.EntityFrameworkCore",
                "Npgsql",
                "DbUp")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_ShouldDependOnApplicationAndDomainOnlyThroughInwardLayers()
    {
        var result = Types.InAssembly(typeof(FinancesDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Finances.Api")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
