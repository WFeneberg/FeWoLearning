using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Tests.Intermediate;

public class Ex036_EfCoreAgainstSqlServerTests
{
    private const string ProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

    [Fact]
    public void The_model_is_a_real_SqlServer_database_child_and_not_a_lookalike_container()
    {
        var model = ModelHarness.Build(Ex036_EfCoreAgainstSqlServer.Configure);

        // Rejects: AddContainer("sqldata", "mcr.microsoft.com/mssql/server") plus
        // AddConnectionString("catalog", ...). That renders a plausible string and is
        // neither of these two types - the track's first rule (README section 9).
        var server = Assert.IsType<SqlServerServerResource>(model.Resource("sqldata"));
        var database = Assert.IsType<SqlServerDatabaseResource>(
            model.Resource(Ex036_EfCoreAgainstSqlServer.DatabaseResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        // SQL Server's own shape: host and port joined by a COMMA, the login fixed to
        // sa, TrustServerCertificate for the container's self-signed certificate - and
        // the child appending "Initial Catalog=", which is the SQL Server spelling of
        // what PostgreSQL calls "Database=".
        Assert.Equal(
            "Server={sqldata.bindings.tcp.host},{sqldata.bindings.tcp.port};User ID=sa;"
            + "Password={sqldata-password.value};TrustServerCertificate=true",
            ModelHarness.ConnectionString(server));
        Assert.Equal(
            $"{{sqldata.connectionString}};Initial Catalog={Ex036_EfCoreAgainstSqlServer.DatabaseResourceName}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public void The_context_is_on_the_SqlServer_provider_and_carries_the_INJECTED_string()
    {
        // A value invented microseconds ago, in the one place SQL Server keeps
        // verbatim. Nothing on disk and nothing in the exercise can produce it, so an
        // implementation that hardcodes "Server=localhost;Database=catalog;..." has
        // nothing to guess with.
        var sentinel = $"ex036-{Guid.NewGuid():N}";
        var injected =
            $"Server=nowhere,1433;Database=catalog;User ID=sa;Password=irrelevant;"
            + $"Application Name={sentinel};TrustServerCertificate=true";

        using var host = BuildHost(injected);
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<Ex036_EfCoreAgainstSqlServer.CatalogContext>();

        // Rejects UseNpgsql / UseSqlite / UseInMemoryDatabase: they all resolve a
        // CatalogContext, and only ProviderName says which one was configured.
        Assert.Equal(ProviderName, context.Database.ProviderName);

        // ...and rejects a hardcoded string. Nothing is opened here: EF hands back the
        // string it was configured with, so this fact stays offline.
        Assert.Equal(injected, context.Database.GetConnectionString());

        // The DbSet is on the context rather than beside it - a context with no mapped
        // entity would satisfy everything above.
        Assert.NotNull(context.Model.FindEntityType(typeof(Ex036_EfCoreAgainstSqlServer.Product)));
    }

    [Fact]
    public void With_no_ConnectionStrings_key_the_context_has_NO_connection_string()
    {
        // The mutant this exists for is the "defensive" one, and it is the plausible
        // mistake rather than a contrived one:
        //     UseSqlServer(cs ?? "Server=localhost;Database=catalog;Trusted_Connection=true")
        // It passes both facts above and means the service quietly talks to whatever is
        // on the developer's machine the day the injection breaks.
        //
        // Asserted as a null connection string, not as an exception: measured on EF Core
        // 10.0.11, UseSqlServer(null) does not throw and the context resolves.
        using var host = BuildHost(connectionString: null);
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<Ex036_EfCoreAgainstSqlServer.CatalogContext>();

        Assert.Equal(ProviderName, context.Database.ProviderName);
        Assert.True(string.IsNullOrEmpty(context.Database.GetConnectionString()));
    }

    private static IHost BuildHost(string? connectionString)
    {
        var builder = Host.CreateApplicationBuilder();
        if (connectionString is not null)
        {
            builder.Configuration[
                $"ConnectionStrings:{Ex036_EfCoreAgainstSqlServer.DatabaseResourceName}"] = connectionString;
        }

        Ex036_EfCoreAgainstSqlServer.AddCatalogDbContext(builder);
        return builder.Build();
    }
}
