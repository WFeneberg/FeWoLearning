using Aspire.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FeWoLearning.MicroServices.Exercises.Intermediate;

/// <summary>
/// Goal:   Find out how much of ex036 is about RELATIONAL databases and how much is
///         about SQL Server. Oracle keeps the SQL and changes the furniture: there is
///         no CREATE SCHEMA, there is no IDENTITY habit worth keeping, and an
///         identifier is not the string you typed.
/// Drills: `AddOracle` + `AddDatabase` and the OracleDatabaseServerResource /
///         OracleDatabaseResource pair, whose child expression appends a SERVICE NAME
///         after a slash rather than a `;Database=` clause; then `HasDefaultSchema`
///         over a folded user name, `HasSequence` + `UseSequence` for keys, and a
///         naming rule that folds to upper case and caps at
///         <see cref="MaxIdentifierLength"/> characters.
/// Passes: the model is a real OracleDatabaseResource with Oracle's own two
///         expressions; `OracleIdentifier` folds and caps names the test invents; the
///         Oracle script PROBES ALL_USERS for the folded schema and contains no CREATE
///         SCHEMA at all, where the same model on SQL Server emits one; the Oracle
///         script creates a sequence and defaults the key from its NEXTVAL, where SQL
///         Server writes IDENTITY; and every identifier the Oracle script quotes is
///         upper-case and short enough, where SQL Server keeps the CLR names.
/// Note:   Four things measured on Oracle.EntityFrameworkCore 10.23.26300.
///
///         (1) A SCHEMA IS A USER, and the provider says so out loud. Asked for a
///         default schema, it does not emit CREATE SCHEMA - it emits a PL/SQL block
///         that counts rows in ALL_USERS and RAISEs USER_NOT_EXIST if the count is
///         zero. There is nothing for EF to create: creating a schema in Oracle means
///         CREATE USER, which is a DBA operation and not part of a migration.
///
///         (2) The provider does NOT fold for you. `HasDefaultSchema("orders_app")`
///         emits `USERNAME='orders_app'` verbatim, which matches nothing on a server
///         where CREATE USER stored the name as ORDERS_APP - and every identifier EF
///         emits is double-quoted, which turns off Oracle's own folding. So folding is
///         YOUR job, and this row makes it a function.
///
///         (3) The cap is <see cref="MaxIdentifierLength"/> here by choice, not by
///         force. Oracle 12.2 and later allow 128 characters and this provider reports
///         128 from `IModel.GetMaxIdentifierLength()`; 30 is the limit on 12.1 and
///         earlier, the limit database links still impose, and the number every
///         Oracle-shaped naming convention in the wild is built around. Truncating to
///         30 yourself is also the only version of this that produces the SAME name on
///         every server you deploy to.
///
///         (4) NVARCHAR2 stops at 2000 characters. `HasMaxLength(4000)` on Oracle
///         therefore lands on NCLOB - an out-of-line lob with different locking and
///         different query behaviour - where SQL Server quietly gives you
///         nvarchar(4000). "It is only a length" is exactly the kind of assumption
///         that does not transfer.
/// </summary>
public static class Ex042_OracleSchemaSemantics
{
    /// <summary>The Oracle SERVER resource's name.</summary>
    public const string ServerResourceName = "oracle";

    /// <summary>The RESOURCE name, hence the configuration key <c>ConnectionStrings:orders</c>.</summary>
    public const string DatabaseResourceName = "orders";

    /// <summary>The sequence the primary key is drawn from.</summary>
    public const string SequenceName = "ORDER_LINE_SEQ";

    /// <summary>Where that sequence starts.</summary>
    public const int SequenceStartsAt = 1000;

    /// <summary>
    /// The identifier length this model targets - see note (3). Not what
    /// <c>IModel.GetMaxIdentifierLength()</c> reports on this provider, and
    /// deliberately so.
    /// </summary>
    public const int MaxIdentifierLength = 30;

    /// <summary>Which provider <see cref="CreateScript"/> should generate for.</summary>
    public enum ScriptTarget
    {
        Oracle,
        SqlServer
    }

    public sealed class OrderLine
    {
        public int Id { get; set; }
        public string CustomerReference { get; set; } = "";

        /// <summary>4000 characters - see note (4).</summary>
        public string Notes { get; set; } = "";
    }

    public sealed class OrdersContext(DbContextOptions<OrdersContext> options) : DbContext(options)
    {
        /// <summary>Scaffolding. Set by <see cref="CreateScript"/> only.</summary>
        public ScriptTarget Target { get; init; }

        /// <summary>Scaffolding. The schema - that is, the USER - the caller asked for.</summary>
        public string SchemaUser { get; init; } = "ORDERS_APP";

        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => MapOrders(modelBuilder, Target, SchemaUser);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ReplaceService<IModelCacheKeyFactory, TargetAwareModelCacheKeyFactory>();
    }

    /// <summary>
    /// Scaffolding, exactly as in ex037 and ex041: EF caches the built model per
    /// (context type, provider), so two targets or two schema names would otherwise
    /// share one cached model and the second call would silently return the first's.
    /// </summary>
    public sealed class TargetAwareModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
            => (context.GetType(), (context as OrdersContext)?.Target,
                (context as OrdersContext)?.SchemaUser, designTime);
    }

    /// <summary>
    /// <paramref name="name"/> as Oracle would have stored it had you typed it
    /// unquoted: folded to upper case and cut to <see cref="MaxIdentifierLength"/>
    /// characters. Must be idempotent - applying it twice changes nothing.
    /// </summary>
    public static string OracleIdentifier(string name)
        => throw new NotImplementedException(
            "TODO: ex042 - fold to upper case and cut to MaxIdentifierLength characters.");

    /// <summary>
    /// Builds the model for <paramref name="target"/> inside
    /// <paramref name="schemaUser"/> - folded, on Oracle, because the provider will not
    /// fold it for you.
    /// </summary>
    public static void MapOrders(ModelBuilder modelBuilder, ScriptTarget target, string schemaUser)
        => throw new NotImplementedException(
            "TODO: ex042 - on Oracle: HasDefaultSchema over the FOLDED user name, a "
            + "sequence named SequenceName starting at SequenceStartsAt with Id drawn "
            + "from it, and every table, column and index name run through "
            + "OracleIdentifier. On SQL Server: the schema unchanged and nothing else.");

    /// <summary>
    /// The options <see cref="CreateScript"/> builds a context on. Nothing here is ever
    /// opened - <c>GenerateCreateScript()</c> reads the model and the provider's SQL
    /// generator and connects to nothing.
    /// </summary>
    public static DbContextOptions<OrdersContext> CreateOptions(ScriptTarget target)
        => throw new NotImplementedException(
            "TODO: ex042 - UseOracle or UseSqlServer, per target.");

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex042 - add an Oracle server named ServerResourceName carrying a "
            + "database whose resource name is DatabaseResourceName.");

    /// <summary>
    /// Scaffolding. The CREATE script <paramref name="target"/> would run to build this
    /// model inside <paramref name="schemaUser"/>.
    /// </summary>
    public static string CreateScript(ScriptTarget target, string schemaUser)
    {
        using var context = new OrdersContext(CreateOptions(target)) { Target = target, SchemaUser = schemaUser };
        return context.Database.GenerateCreateScript();
    }
}
