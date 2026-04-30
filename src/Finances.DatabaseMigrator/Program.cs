using System.Reflection;
using DbUp;
using DbUp.Engine;

var connectionString = Environment.GetEnvironmentVariable("FINANCES_DB_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("FINANCES_DB_CONNECTION_STRING es obligatoria para ejecutar migraciones.");
    return 1;
}

var result = DatabaseMigrationRunner.Run(connectionString);

if (!result.Successful)
{
    Console.Error.WriteLine(result.Error);
    return 1;
}

Console.WriteLine("Migraciones DBUp ejecutadas correctamente.");
return 0;

public static class DatabaseMigrationRunner
{
    public static DatabaseUpgradeResult Run(string connectionString)
    {
        return DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .JournalToPostgresqlTable("public", "schema_versions")
            .LogToConsole()
            .Build()
            .PerformUpgrade();
    }
}
