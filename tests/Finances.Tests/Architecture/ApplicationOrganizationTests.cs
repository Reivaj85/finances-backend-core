namespace Finances.Tests.Architecture;

public sealed class ApplicationOrganizationTests
{
    [Theory]
    [InlineData("CreateRecurringExpenseCommand.cs", "RecurringExpenses", "Commands")]
    [InlineData("CreateRecurringExpenseHandler.cs", "RecurringExpenses", "Commands")]
    [InlineData("CreateRecurringExpenseCommandValidator.cs", "RecurringExpenses", "Commands")]
    [InlineData("ListRecurringExpensesQuery.cs", "RecurringExpenses", "Queries")]
    [InlineData("ListRecurringExpensesHandler.cs", "RecurringExpenses", "Queries")]
    [InlineData("ListRecurringExpensesQueryValidator.cs", "RecurringExpenses", "Queries")]
    [InlineData("RecurringExpenseResponse.cs", "RecurringExpenses", "Contracts")]
    [InlineData("RecurringExpenseMapping.cs", "RecurringExpenses", "Mappings")]
    [InlineData("IRecurringExpenseRepository.cs", "RecurringExpenses", "Abstractions")]
    public void ApplicationFeatureFiles_ShouldLiveInExpectedFolder_WhenUsingVerticalSliceOrganization(
        string fileName,
        string featureFolder,
        string expectedFolder)
    {
        var applicationPath = ResolveApplicationPath();
        var expectedPath = Path.Combine(applicationPath, featureFolder, expectedFolder, fileName);

        Assert.True(File.Exists(expectedPath), $"Expected file at {expectedPath}");
    }

    [Fact]
    public void RecurringExpensesRoot_ShouldNotContainApplicationArtifacts_WhenFeatureIsOrganizedByConcern()
    {
        var featurePath = Path.Combine(ResolveApplicationPath(), "RecurringExpenses");
        var rootFiles = Directory.GetFiles(featurePath, "*.cs", SearchOption.TopDirectoryOnly);

        Assert.Empty(rootFiles);
    }

    private static string ResolveApplicationPath()
    {
        var rootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        return Path.Combine(rootPath, "src", "Finances.Application");
    }
}
