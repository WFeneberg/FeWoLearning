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
    public async Task The_migrator_WAITS_for_the_database_rather_than_merely_referencing_it()
    {
        var model = ModelHarness.Build(Configure);
        var token = TestContext.Current.CancellationToken;

        var server = Assert.IsType<SqlServerServerResource>(model.Resource("sqldata"));
        var database = Assert.IsType<SqlServerDatabaseResource>(model.Resource(DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        var migrator = Assert.IsType<ProjectResource>(model.Resource("migrator"));

        // Both annotations must be there, and they are DIFFERENT annotations - ex002's
        // subject. Counting EnvironmentCallbackAnnotations would grade nothing here:
        // measured and recorded in README section 5, AddProject arrives carrying FOUR of
        // them before anything is referenced, so "not empty" is true of a migrator that
        // was never told where its database is. Run the callbacks instead and read the
        // variable out - ex007's technique, and the only thing WithReference actually
        // promises.
        var environment = new Dictionary<string, object>();
        var callbackContext = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run),
            migrator, environment, token);
        foreach (var annotation in migrator.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await annotation.Callback(callbackContext);
        }

        // The key is the RESOURCE name, which is why ex036 named the database resource
        // "catalog" in the first place - and the value is still an expression, because
        // nothing has been allocated at model time.
        Assert.True(
            environment.ContainsKey($"ConnectionStrings__{DatabaseResourceName}"),
            "the migrator was never told where the database is; variables present: "
            + string.Join(", ", environment.Keys.Order()));
        // At model time the value is not a string: WithReference stores a deferred
        // reference, and only its manifest expression says which resource it points at.
        // A hand-written WithEnvironment("ConnectionStrings__catalog", "Server=...")
        // would land a plain System.String here and fail this cast.
        var reference = Assert.IsAssignableFrom<IManifestExpressionProvider>(
            environment[$"ConnectionStrings__{DatabaseResourceName}"]);
        Assert.Equal($"{{{DatabaseResourceName}.connectionString}}", reference.ValueExpression);

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

        // A fresh, empty database on the assembly's SHARED SQL Server. The learner's
        // Configure is graded at L1 by the first fact - it names a project resource so
        // that WaitFor has something to gate, and building plus running that project
        // would add a minute to every container run for a claim already graded. What
        // this fact needs is a real server and a database nobody else has touched, and
        // that is exactly what DatabaseAsync is: milliseconds, on a server the rest of
        // the assembly shares. See MicroServices/README.md section 4.
        var database = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "ex038", token);
        var connectionString = database.ConnectionString;

        // Empty to begin with: no tables, and in particular no __EFMigrationsHistory.
        // If that were not true, the "applied both" assertion below would be reading
        // some earlier test's leftovers instead of this exercise's work.
        Assert.False(await TableExistsAsync(connectionString, "__EFMigrationsHistory", token));

        // ---- first start -----------------------------------------------------------
        var firstRun = await RunOneStartAsync(connectionString, token);

        // Both, in EF's order. Rejects EnsureCreatedAsync(), which creates the schema and
        // reports nothing, and rejects a migrator that applies only the first migration.
        Assert.Equal(MigrationIds, firstRun);

        // ...and the history table exists and names both. This is the assertion
        // EnsureCreated can never satisfy: it writes no history at all, so the very same
        // schema comes back with no such table.
        Assert.Equal(MigrationIds, await HistoryAsync(connectionString, token));

        // The second migration really ran against the first one's table.
        Assert.True(await ColumnExistsAsync(connectionString, "Products", "Sku", token));

        // ---- second start, same database -------------------------------------------
        var secondRun = await RunOneStartAsync(connectionString, token);

        // The point of the row. Rejects the near-miss that reports
        // GetAppliedMigrationsAsync() - correct on the first start, "2" here.
        Assert.Empty(secondRun);

        // ...and nothing was re-applied behind the report's back: the history is still
        // exactly two rows, so no migration ran twice.
        Assert.Equal(MigrationIds, await HistoryAsync(connectionString, token));

        // A row written between two starts must survive the next one, which rejects a
        // "migrator" that drops and recreates the database to make itself idempotent.
        var sku = $"ex038-{Guid.NewGuid():N}";
        await ExecuteAsync(connectionString,
            $"INSERT INTO [Products] ([Name], [Sku]) VALUES (N'probe', N'{sku}')", token);
        var thirdRun = await RunOneStartAsync(connectionString, token);
        Assert.Empty(thirdRun);
        Assert.Equal(1L, await ScalarAsync(connectionString,
            $"SELECT COUNT_BIG(*) FROM [Products] WHERE [Sku] = N'{sku}'", token));
    }

    private static async Task<bool> TableExistsAsync(
        string connectionString, string table, CancellationToken cancellationToken)
        => await ScalarAsync(connectionString,
            $"SELECT COUNT_BIG(*) FROM sys.tables WHERE name = N'{table}'", cancellationToken) is 1L;

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
