using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Learn where seed data belongs, and what "safe to run again" means. EF gives
///         you two places to put it and they are not interchangeable: `HasData` in
///         `OnModelCreating` becomes part of the SCHEMA - the migration carries the
///         INSERTs - while a startup seed is ordinary code that runs on every boot and
///         must therefore be an upsert.
/// Drills: `HasData` for fixed reference rows; an idempotent upsert for the rest.
/// Passes: the generated script carries one INSERT per seeded category, with the exact
///         ids and names, and NO insert for the settings table; and running the startup
///         seed a second time inserts nothing, restores a canonical value someone had
///         edited, and leaves a row the seed does not own completely alone.
/// Note:   The row is graded on RE-RUN safety, not on the row count after one start.
///         Counting rows after a single seed passes against `AddRange(...)` +
///         `SaveChangesAsync()`, which is the wrong answer: the second boot throws a
///         primary-key violation. It also passes against `RemoveRange(everything)` then
///         re-insert, which is worse than wrong - it silently deletes rows the seed did
///         not put there. Both are rejected here, and so is the "insert only if the
///         table is empty" version, which never repairs a value that drifted.
///
///         `HasData` rows arrive with the schema, so a database created from this model
///         already contains the categories before any seeding code runs. That is
///         measured by the third fact and is the clearest single statement of the
///         difference between the two mechanisms.
///
///         Every fact here is offline. The re-run half runs against a real relational
///         store - SQLite in memory, supplied by the test - because a fake dictionary
///         would not have a primary key and could not reject the mutant that matters.
///         `SeedSettingsAsync` therefore takes a DbContext and never names a provider.
/// </summary>
public static class Ex039_SeedDataInTheModel
{
    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:catalog</c>.</summary>
    public const string DatabaseResourceName = "catalog";

    public sealed class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public sealed class Setting
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }

    /// <summary>
    /// The reference rows that belong in the MODEL. Given, so that the migration and any
    /// test read one list; your job is to get them into <c>OnModelCreating</c>.
    /// </summary>
    public static IReadOnlyList<Category> SeededCategories { get; } =
    [
        new() { Id = 1, Name = "Tools" },
        new() { Id = 2, Name = "Books" },
        new() { Id = 3, Name = "Toys" }
    ];

    /// <summary>
    /// The rows that belong to STARTUP: operational defaults that an operator is
    /// expected to edit, and that a redeploy must restore rather than duplicate.
    /// </summary>
    public static IReadOnlyDictionary<string, string> StartupSettings { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["catalog:pageSize"] = "25",
            ["catalog:currency"] = "EUR",
            ["catalog:featuredCategoryId"] = "1"
        };

    public sealed class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
    {
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Setting> Settings => Set<Setting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Scaffolding: Setting is keyed by its own Key property, not by convention.
            modelBuilder.Entity<Setting>().HasKey(setting => setting.Key);

            SeedCategories(modelBuilder);
        }
    }

    /// <summary>
    /// Puts <see cref="SeededCategories"/> into the MODEL, so that they become part of
    /// the generated schema rather than of some startup routine.
    /// </summary>
    private static void SeedCategories(ModelBuilder modelBuilder)
        => throw new NotImplementedException(
            "TODO: ex039 - seed SeededCategories through the model, so the generated "
            + "script carries the INSERTs. Nothing here may touch Settings.");

    /// <summary>
    /// Applies <see cref="StartupSettings"/> to <paramref name="context"/> and returns
    /// the keys it INSERTED this time - empty on every run after the first.
    ///
    /// It must be safe to run on every boot: never duplicate, never throw, never touch a
    /// setting it does not own, and restore any of its own that were changed.
    /// </summary>
    public static Task<IReadOnlyList<string>> SeedSettingsAsync(
        CatalogContext context, CancellationToken cancellationToken)
        => throw new NotImplementedException(
            "TODO: ex039 - upsert StartupSettings and report the keys you inserted.");

    /// <summary>
    /// Given: the CREATE script for this model on SQL Server, generated offline - the
    /// same mechanism ex037 introduced. Nothing connects.
    /// </summary>
    public static string CreateScript()
    {
        var options = new DbContextOptionsBuilder<CatalogContext>()
            .UseSqlServer("Server=offline;Database=catalog")
            .Options;

        using var context = new CatalogContext(options);
        return context.Database.GenerateCreateScript();
    }

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex039 - add a SQL Server \"sqldata\" carrying a database whose resource "
            + "name is DatabaseResourceName.");
}
