using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Put the same DbContext on a THIRD engine and find out what stops
///         transferring. ex036 and ex037 differ in type names; MySQL differs in
///         whether your table still exists after you deploy it to a Linux box.
/// Drills: `AddMySql` + `AddDatabase` and the MySqlServerResource /
///         MySqlDatabaseResource pair; then, off the generated DDL: lower-casing
///         table names because MySQL identifiers are FILES, a per-column
///         `utf8mb4` / `utf8mb4_bin` charset-and-collation pair, and
///         `HasPrefixLength` on an index over a TEXT column.
/// Passes: the model is a real MySqlDatabaseResource with MySQL's own two connection
///         expressions; a table name the test invents mixed-case reaches the MySQL
///         script lower-cased and the SQL Server script untouched, each provider
///         escaping its OWN delimiter; the Sku column carries CHARACTER SET utf8mb4
///         COLLATE utf8mb4_bin and no other column does; the description index carries
///         a prefix length of 32 on MySQL and none on SQL Server; and the two scripts
///         spell every type differently and share none of each other's spellings.
/// Note:   Four things measured on MySql.EntityFrameworkCore 10.0.9 while writing this.
///
///         (1) The identifier delimiter is a BACKTICK, and each provider escapes only
///         its own: a table named `a`b"c]d` comes out of MySQL as `` `a``b"c]d` `` and
///         out of SQL Server as `[a`b"c]]d]`. That is what makes fact 2 impossible to
///         satisfy with a hand-transcribed script.
///
///         (2) MySQL case sensitivity is not a collation question and not a MySQL
///         *version* question - it is `lower_case_table_names`, which defaults to 0 on
///         Linux (case-SENSITIVE, because each table is a file) and 1 on Windows and
///         macOS. A model that works on a developer's laptop and loses its tables in
///         production is the bug this row exists for. The defence is to force one case
///         yourself. COLUMN names are case-insensitive everywhere, which is why this
///         row grades the table name.
///
///         (3) `UseCollation`/`HasCharSet` on the MODEL reach `CREATE DATABASE`, which
///         `GenerateCreateScript()` does not emit - measured: they leave no trace in the
///         script at all. The per-column pair, `ForMySQLHasCharset` and
///         `ForMySQLHasCollation`, is what shows up in `CREATE TABLE`, and per-column is
///         the honest place for it anyway: one case-sensitive column beside a
///         case-insensitive default is a decision, a database-wide collation is a
///         guess.
///
///         (4) `HasPrefixLength` is real and necessary - InnoDB cannot index a TEXT
///         column without one - but this provider STORES it and does not emit it:
///         the generated `CREATE INDEX` says `(`description`)`, not
///         `(`description`(32))`. So fact 4 reads the model rather than the script.
///         That is a gap in MySql.EntityFrameworkCore 10.0.9, recorded here so the next
///         reader does not spend the afternoon looking for a call they got wrong.
/// </summary>
public static class Ex041_MySqlIdentifiersAndCollation
{
    /// <summary>The MySQL SERVER resource's name.</summary>
    public const string ServerResourceName = "mysqldb";

    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:catalog</c>.</summary>
    public const string DatabaseResourceName = "catalog";

    /// <summary>The character set every text column in this model is stored in.</summary>
    public const string CharSet = "utf8mb4";

    /// <summary>
    /// The collation <see cref="Product.Sku"/> uses. `_bin` compares byte by byte, so
    /// "ABC" and "abc" are two different SKUs - the opposite of MySQL's default
    /// `utf8mb4_0900_ai_ci`, which is accent- and case-INsensitive.
    /// </summary>
    public const string CaseSensitiveCollation = "utf8mb4_bin";

    /// <summary>How many leading characters of <see cref="Product.Description"/> the index covers.</summary>
    public const int DescriptionIndexPrefixLength = 32;

    /// <summary>The database name of that index.</summary>
    public const string DescriptionIndexName = "ix_products_description";

    /// <summary>Which provider <see cref="CreateScript"/> should generate for.</summary>
    public enum ScriptTarget
    {
        MySql,
        SqlServer
    }

    public sealed class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        /// <summary>Case-SENSITIVE: see <see cref="CaseSensitiveCollation"/>.</summary>
        public string Sku { get; set; } = "";

        /// <summary>Mapped to MySQL's <c>text</c>, which cannot be indexed without a prefix.</summary>
        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }

    public sealed class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
    {
        /// <summary>Scaffolding. Set by <see cref="CreateScript"/> only.</summary>
        public ScriptTarget Target { get; init; }

        /// <summary>Scaffolding. The table name the caller asked for, before any folding.</summary>
        public string TableName { get; init; } = "Products";

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => MapCatalog(modelBuilder, Target, TableName);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ReplaceService<IModelCacheKeyFactory, TargetAwareModelCacheKeyFactory>();
    }

    /// <summary>
    /// Scaffolding, exactly as in ex037: EF caches the built model per (context type,
    /// provider), so two targets or two table names would otherwise share one cached
    /// model and the second call would silently return the first one's script.
    /// </summary>
    public sealed class TargetAwareModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
            => (context.GetType(), (context as CatalogContext)?.Target,
                (context as CatalogContext)?.TableName, designTime);
    }

    /// <summary>
    /// Builds the model for <paramref name="target"/>, mapping the entity to
    /// <paramref name="tableName"/> - folded if this engine needs it folded.
    /// </summary>
    public static void MapCatalog(ModelBuilder modelBuilder, ScriptTarget target, string tableName)
        => throw new NotImplementedException(
            "TODO: ex041 - map Product to tableName, folded to lower case for MySQL and "
            + "left alone for SQL Server; give Sku the utf8mb4 charset and the "
            + "utf8mb4_bin collation on MySQL only; map Description to MySQL's text; and "
            + "add the description index, with a prefix length on MySQL.");

    /// <summary>
    /// The options <see cref="CreateScript"/> builds a context on. Nothing here is ever
    /// opened: `GenerateCreateScript()` reads the model and the provider's SQL
    /// generator, so a host named "offline" is the cheapest possible proof that this row
    /// touches no database.
    /// </summary>
    public static DbContextOptions<CatalogContext> CreateOptions(ScriptTarget target)
        => throw new NotImplementedException(
            "TODO: ex041 - UseMySQL or UseSqlServer, per target. The connection string is "
            + "never opened.");

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex041 - add a MySQL server named ServerResourceName carrying a database "
            + "whose resource name is DatabaseResourceName.");

    /// <summary>
    /// Scaffolding. The CREATE script <paramref name="target"/> would run to build this
    /// model with the entity mapped to <paramref name="tableName"/>.
    /// </summary>
    public static string CreateScript(ScriptTarget target, string tableName)
    {
        using var context = new CatalogContext(CreateOptions(target)) { Target = target, TableName = tableName };
        return context.Database.GenerateCreateScript();
    }

    /// <summary>
    /// Scaffolding. The built EF model, for the facts a script cannot carry - see note
    /// (4) in the header: this provider stores an index prefix length and does not emit
    /// it, so the only place to read it is here.
    /// </summary>
    public static IModel BuildModel(ScriptTarget target, string tableName)
    {
        using var context = new CatalogContext(CreateOptions(target)) { Target = target, TableName = tableName };
        return context.Model;
    }
}
