using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Apply migrations when the service starts, correctly - which means the second
///         start applies NOTHING, and says so.
/// Drills: a hosted service that resolves a scope, asks `Database` what is pending,
///         applies it with `MigrateAsync`, and reports what it applied; plus the
///         `WaitFor` in the AppHost that stops it racing the database container.
/// Passes: at L1 - the migrator waits for the database with a WaitAnnotation whose
///         WaitType is WaitUntilHealthy. Offline - AddCatalogMigrations registers the
///         context, the report, and a hosted service. At L3 - the first start applies
///         both migrations, in order, and leaves both ids in __EFMigrationsHistory; the
///         SECOND start, against the very same database, applies ZERO and reports an
///         empty list.
/// Note:   The zero is the whole exercise. `EnsureCreatedAsync()` creates the schema and
///         is what almost everyone reaches for; it writes NOTHING to
///         __EFMigrationsHistory, has no notion of "pending", and can never apply the
///         second migration to a database the first one already built. It passes any
///         test that only asks whether the table exists - so this row asks what the
///         start APPLIED, and asks it twice.
///         The equally plausible near-miss is reporting `GetAppliedMigrationsAsync()`
///         instead of what was pending: that is right on the first start and wrong on
///         the second, which is exactly the pair of facts below.
///
///         The two Migration classes are GIVEN, not part of the TODO. They are
///         hand-written rather than scaffolded by `dotnet ef`, because this track has no
///         design-time tooling; a Migration needs only [DbContext], [Migration] and Up -
///         measured, including that EF raises no pending-model-changes error without a
///         ModelSnapshot class.
///
///         Only the third fact is 🐳. The AppHost model in Configure names a project
///         resource so that WaitFor has something to gate; the container fact starts a
///         store of its own instead, because building and running that project would
///         add a minute to every container run to prove something the first fact
///         already grades at L1.
/// </summary>
public static class Ex038_MigrationsOnStartup
{
    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:catalog</c>.</summary>
    public const string DatabaseResourceName = "catalog";

    /// <summary>The two migration ids, oldest first. EF orders by id, and so does the test.</summary>
    public static IReadOnlyList<string> MigrationIds { get; } =
        ["20260101000000_CreateProducts", "20260102000000_AddProductSku"];

    public sealed class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Sku { get; set; }
    }

    public sealed class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }

    /// <summary>
    /// Given: what one start applied. Register it as a singleton in
    /// <see cref="AddCatalogMigrations"/> and have your hosted service fill it in; each
    /// host gets its own, so two starts produce two reports.
    /// </summary>
    public sealed class MigrationReport
    {
        private readonly List<string> _applied = [];

        /// <summary>The migrations THIS start applied - empty when there was nothing to do.</summary>
        public IReadOnlyList<string> Applied => _applied;

        public void Record(IEnumerable<string> migrationIds) => _applied.AddRange(migrationIds);
    }

    /// <summary>
    /// Registers <see cref="CatalogContext"/> on SQL Server over the injected
    /// <c>ConnectionStrings:catalog</c>, a singleton <see cref="MigrationReport"/>, and a
    /// hosted service that migrates the database when the host starts.
    /// </summary>
    public static void AddCatalogMigrations(IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(DatabaseResourceName);

        builder.Services.AddDbContext<CatalogContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddSingleton<MigrationReport>();
        builder.Services.AddHostedService<MigrationHostedService>();
    }

    /// <summary>
    /// Migrates once, when the host starts. A DbContext is scoped and a hosted service is
    /// a singleton, so it opens its own scope rather than taking a CatalogContext in the
    /// constructor - which would capture one for the lifetime of the process.
    /// </summary>
    private sealed class MigrationHostedService(
        IServiceScopeFactory scopeFactory, MigrationReport report) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            // Ask FIRST, apply SECOND. After MigrateAsync there is no longer any
            // difference between "pending a moment ago" and "applied at some point in
            // history", and reporting GetAppliedMigrationsAsync() instead is the mutant
            // that is right on the first start and wrong on the second.
            var pending = await context.Database.GetPendingMigrationsAsync(cancellationToken);

            // MigrateAsync, not EnsureCreatedAsync: it consults __EFMigrationsHistory,
            // applies only what is missing, and is therefore a no-op on the second start.
            await context.Database.MigrateAsync(cancellationToken);

            report.Record(pending);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var catalog = builder.AddSqlServer("sqldata")
                             .AddDatabase(DatabaseResourceName);

        // WithReference injects ConnectionStrings__catalog; WaitFor is the gate. Without
        // it the migrator starts the moment the container process exists, which is well
        // before SQL Server is accepting logins - the classic "works on my machine,
        // fails in CI" of startup migrations. launchProfileName: null keeps the project
        // free of the endpoints its launchSettings.json would otherwise contribute;
        // this resource exists to migrate, not to serve.
        builder.AddProject("migrator", ServiceProject(builder, "Catalog"), launchProfileName: null)
               .WithReference(catalog)
               .WaitFor(catalog);
    }

    /// <summary>
    /// Given, not a TODO. The absolute path of one of the track's shared services'
    /// project files - see ex011 and README section 5.
    /// </summary>
    internal static string ServiceProject(IDistributedApplicationBuilder builder, string name)
    {
        var dir = new DirectoryInfo(builder.AppHostDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "FeWoLearning.MicroServices.slnx")))
        {
            dir = dir.Parent;
        }

        var root = dir?.FullName
                   ?? throw new InvalidOperationException(
                       $"'{builder.AppHostDirectory}' is not inside MicroServices/.");

        return Path.Combine(root, "services", name, $"{name}.csproj");
    }

    /// <summary>Given. The first migration: the table.</summary>
    [DbContext(typeof(CatalogContext))]
    [Migration("20260101000000_CreateProducts")]
    public sealed class CreateProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Products", x => x.Id));

        protected override void Down(MigrationBuilder migrationBuilder)
            => migrationBuilder.DropTable("Products");
    }

    /// <summary>
    /// Given. The second migration - the one an `EnsureCreated` implementation can never
    /// apply, because it has no idea the first one already ran.
    /// </summary>
    [DbContext(typeof(CatalogContext))]
    [Migration("20260102000000_AddProductSku")]
    public sealed class AddProductSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.AddColumn<string>(name: "Sku", table: "Products", nullable: true);

        protected override void Down(MigrationBuilder migrationBuilder)
            => migrationBuilder.DropColumn(name: "Sku", table: "Products");
    }
}
