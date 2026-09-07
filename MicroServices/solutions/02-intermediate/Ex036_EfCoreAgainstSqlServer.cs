using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Put the first EF Core DbContext in the track on the SQL Server the AppHost
///         models, over the connection string the AppHost injects - and nothing else.
/// Drills: `AddSqlServer` + `AddDatabase` on the model side; `AddDbContext` +
///         `UseSqlServer(configuration.GetConnectionString("catalog"))` on the service
///         side. Two halves of one wire: the resource named "catalog" is why the key
///         is `ConnectionStrings:catalog`.
/// Passes: "sqldata" is a SqlServerServerResource and "catalog" a
///         SqlServerDatabaseResource parented to it, whose expression ends
///         ";Initial Catalog=catalog"; a host configured with a connection string the
///         test invented resolves a CatalogContext on the SQL Server provider carrying
///         exactly that string; and a host with NO ConnectionStrings:catalog key
///         resolves a context with NO connection string at all.
/// Note:   The third fact is the one that costs a "safe" local default. Writing
///         `UseSqlServer(cs ?? "Server=localhost;...")` looks defensive and passes the
///         first two facts; it also means the service silently talks to whatever is on
///         the developer's machine when the injection breaks, which is the failure this
///         row exists to make impossible. Measured on EF Core 10.0.11:
///         `UseSqlServer(null)` does NOT throw - the context resolves happily and
///         `Database.GetConnectionString()` returns null - so "no fallback" is an
///         assertion about that null, not about an exception.
///         The live SQL Server proof is ex038 and ex040; this row is the wiring, and
///         every one of its facts runs with no container.
/// </summary>
public static class Ex036_EfCoreAgainstSqlServer
{
    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:catalog</c>.</summary>
    public const string DatabaseResourceName = "catalog";

    public sealed class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public sealed class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // AddSqlServer, not AddContainer: only the integration produces a
        // SqlServerServerResource, and only its child produces the
        // "{sqldata.connectionString};Initial Catalog=catalog" expression. A generic
        // container plus AddConnectionString renders a plausible string and is neither
        // type - the mutant the first fact rejects.
        builder.AddSqlServer("sqldata")
               .AddDatabase(DatabaseResourceName);
    }

    /// <summary>
    /// Registers <see cref="CatalogContext"/> on SQL Server, reading the connection
    /// string the AppHost injected as <c>ConnectionStrings:catalog</c>.
    /// </summary>
    public static void AddCatalogDbContext(IHostApplicationBuilder builder)
    {
        // GetConnectionString(name) is GetSection("ConnectionStrings")[name] - the key
        // Aspire's WithReference writes as ConnectionStrings__catalog. Passing it
        // straight through is the whole point: no "??" default, no literal, nothing the
        // AppHost cannot change.
        var connectionString = builder.Configuration.GetConnectionString(DatabaseResourceName);

        builder.Services.AddDbContext<CatalogContext>(options => options.UseSqlServer(connectionString));
    }
}
