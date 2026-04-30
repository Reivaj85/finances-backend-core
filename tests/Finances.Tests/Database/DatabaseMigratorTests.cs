using Testcontainers.PostgreSql;

namespace Finances.Tests.Database;

public sealed class DatabaseMigratorTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("finances_demo")
        .WithUsername("demo_user")
        .WithPassword("demo_password")
        .Build();

    public Task InitializeAsync() => postgres.StartAsync();

    public Task DisposeAsync() => postgres.DisposeAsync().AsTask();

    [Fact(Skip = "Requiere Docker corriendo para levantar PostgreSQL con Testcontainers.")]
    public void Run_ShouldApplyDbUpScripts_WhenPostgreSqlIsAvailable()
    {
        var result = DatabaseMigrationRunner.Run(postgres.GetConnectionString());

        Assert.True(result.Successful, result.Error?.Message);
    }
}
