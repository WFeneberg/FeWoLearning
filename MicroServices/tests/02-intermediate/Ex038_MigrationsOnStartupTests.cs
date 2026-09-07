using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Intermediate;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static FeWoLearning.MicroServices.Exercises.Intermediate.Ex038_MigrationsOnStartup;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex038_MigrationsOnStartupTests
{
    [Fact]
    public void The_migrator_WAITS_for_the_database_rather_than_merely_referencing_it()
    {
        var model = ModelHarness.Build(Configure);

        var server = Assert.IsType<SqlServerServerResource>(model.Resource("sqldata"));
        var database = Assert.IsType<SqlServerDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        var migrator = Assert.IsType<ProjectResource>(model.Resource("migrator"));

        // WithReference alone is the mutant: the migrator gets ConnectionStrings__catalog
        // and starts immediately, well before SQL Server accepts logins. Both annotations
        // must be there, and they are different annotations - ex002's subject.
        Assert.NotEmpty(migrator.Annotations.OfType<EnvironmentCallbackAnnotation>());

        // Filter by name. Measured (README section 6): WaitFor on a database CHILD leaves
        // a WaitAnnotation for the parent server too, so an unfiltered assertion is
        // satisfied by the weaker WaitFor(sqldata).
        var wait = Assert.Single(
            migrator.Annotations.OfType<WaitAnnotation>(),
            w => w.Resource.Name == DatabaseResourceName);

        // ...and WaitUntilHealthy, not WaitForCompletion: the database is a long-running
        // server, and waiting for it to EXIT would hang forever. ex015's subject.
        Assert.Equal(WaitType.WaitUntilHealthy, wait.WaitType);
    }

    [Fact]
    public void The_registration_is_a_hosted_service_and_a_report_not_a_migration_at_startup()
    {
        // No connection string on purpose: registering must not connect to anything, and
        // an implementation that migrates inside AddCatalogMigrations - rather than from
        // a hosted service - blows up right here instead of waiting for a database.
        var builder = Host.CreateApplicationBuilder();
        AddCatalogMigrations(builder);

        using var host = builder.Build();

        Assert.NotNull(host.Services.GetRequiredService<MigrationReport>());
        Assert.Empty(host.Services.GetRequiredService<MigrationReport>().Applied);

        // Exactly one hosted service, and it came from the exercise rather than from the
        // host builder's own defaults.
        var hosted = host.Services.GetServices<IHostedService>()
            .Where(s => s.GetType().Assembly == typeof(MigrationReport).Assembly)
            .ToList();
        Assert.Single(hosted);

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);

        // The two migrations are discoverable from the context - the assertion that says
        // "there is something to apply" without applying it.
        Assert.Equal(MigrationIds, context.Database.GetMigrations());
    }

    [Fact]
    public async Task A_real_SqlServer_is_migrated_ONCE_and_the_second_start_applies_NOTHING()
    {
        ContainerGate.Require();

        var token = TestContext.Current.CancellationToken;

        // The container fact starts a store of its own rather than the learner's
        // Configure: Configure names a project resource so that WaitFor has something to
        // gate, and building plus running it would cost a minute per container run to
        // prove what the first fact already grades at L1.
        await ContainerHarness.RunAsync(
            builder => builder.AddSqlServer("sqldata").AddDatabase(DatabaseResourceName),
            async session =>
            {
                await session.WaitForHealthyAsync(DatabaseResourceName);
                var connectionString = await session.ConnectionStringAsync(DatabaseResourceName);

                // The expression really is gone - ex034's proof, repeated here because
                // this is the first SQL Server row that runs. DCP publishes on an
                // ephemeral host port, never 1433, and SQL Server spells host and port
                // "Server=host,port".
                Assert.DoesNotContain("{sqldata.", connectionString);
                Assert.DoesNotContain(".connectionString}", connectionString);
                var dataSource = new SqlConnectionStringBuilder(connectionString).DataSource;
                Assert.Contains(",", dataSource);
                Assert.NotEqual(1433, int.Parse(dataSource.Split(',')[1]));

                // ---- first start -------------------------------------------------
                var firstRun = await RunOneStartAsync(connectionString, token);

                // Both, in EF's order. Rejects EnsureCreatedAsync(), which creates the
                // schema and reports nothing, and rejects a migrator that applies only
                // the first migration.
                Assert.Equal(MigrationIds, firstRun);

                // ...and the history table exists and names both. This is the assertion
                // EnsureCreated can never satisfy: it writes no history at all, so the
                // very same schema comes back with no such table.
                Assert.Equal(MigrationIds, await HistoryAsync(connectionString, token));

                // The second migration really ran against the first one's table.
                Assert.True(await ColumnExistsAsync(connectionString, "Products", "Sku", token));

                // ---- second start, same database ---------------------------------
                var secondRun = await RunOneStartAsync(connectionString, token);

                // The point of the row. Rejects the near-miss that reports
                // GetAppliedMigrationsAsync() - correct on the first start, "2" here.
                Assert.Empty(secondRun);

                // ...and nothing was re-applied behind the report's back: the history is
                // still exactly two rows, so no migration ran twice.
                Assert.Equal(MigrationIds, await HistoryAsync(connectionString, token));

                // A row written between two starts must survive the next one, which
                // rejects a "migrator" that drops and recreates the database to make
                // itself idempotent.
                var sku = $"ex038-{Guid.NewGuid():N}";
                await ExecuteAsync(connectionString,
                    $"INSERT INTO [Products] ([Name], [Sku]) VALUES (N'probe', N'{sku}')", token);
                var thirdRun = await RunOneStartAsync(connectionString, token);
                Assert.Empty(thirdRun);
                Assert.Equal(1L, await ScalarAsync(connectionString,
                    $"SELECT COUNT_BIG(*) FROM [Products] WHERE [Sku] = N'{sku}'", token));
            },
            token);
    }

    /// <summary>
    /// One complete start of the learner's registration, from a fresh host, and what it
    /// reported applying. Two calls are two independent processes as far as the exercise
    /// can tell - nothing is shared but the database.
    /// </summary>
    private static async Task<IReadOnlyList<string>> RunOneStartAsync(
        string connectionString, CancellationToken cancellationToken)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration[$"ConnectionStrings:{DatabaseResourceName}"] = connectionString;
        AddCatalogMigrations(builder);

        using var host = builder.Build();
        await host.StartAsync(cancellationToken);
        try
        {
            return [.. host.Services.GetRequiredService<MigrationReport>().Applied];
        }
        finally
        {
            await host.StopAsync(cancellationToken);
        }
    }

    private static async Task<IReadOnlyList<string>> HistoryAsync(
        string connectionString, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            "SELECT [MigrationId] FROM [__EFMigrationsHistory] ORDER BY [MigrationId]", connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var ids = new List<string>();
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetString(0));
        }

        return ids;
    }

    private static async Task<bool> ColumnExistsAsync(
        string connectionString, string table, string column, CancellationToken cancellationToken)
        => await ScalarAsync(connectionString,
            $"SELECT COUNT_BIG(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[{table}]') "
            + $"AND name = N'{column}'", cancellationToken) is 1L;

    private static async Task<object?> ScalarAsync(
        string connectionString, string sql, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        return await command.ExecuteScalarAsync(cancellationToken);
    }

    private static async Task ExecuteAsync(
        string connectionString, string sql, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
