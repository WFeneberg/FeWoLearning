using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Put the whole tier in one graph: four stores of four different flavours, two
///         services, and every edge between them declared twice - once for the
///         connection string, once for the ordering.
/// Drills: SQL Server + PostgreSQL + MongoDB + Redis side by side, each with the
///         database child it has (Redis has none), and two consumers that carry both
///         `WithReference` and `WaitFor` for each store they use.
/// Passes: the four consumer-facing connection expressions are four structurally
///         different strings; one `WithReference` call produces a DIFFERENT set of
///         environment variables per flavour; and the wait graph counts three
///         annotations on "catalog-api" against four on "orders-api".
/// Note:   What only the combination proves - none of this is visible one store at a
///         time, which is why the capstone is not just ex026-ex029 re-run:
///         * `WithReference` is one call with four behaviours. Measured on 13.5.3, a
///           consumer gets ConnectionStrings__&lt;name&gt; plus SEVEN siblings for SQL
///           Server and PostgreSQL, NINE for MongoDB (it adds
///           _AUTHENTICATIONDATABASE and _AUTHENTICATIONMECHANISM) and FIVE for Redis
///           (whose _URI takes its scheme from the BINDING, since the connection
///           string has none). Every key is the RESOURCE name upper-cased.
///         * `WaitFor` on a database CHILD leaves two annotations, one for the child
///           and one for its server; on Redis, which has no child, it leaves one. So
///           "catalog-api" waits on {sql, catalog, cache} and "orders-api" on
///           {pg, orders, docs, reviews} - 3 against 4, from two calls each. Waiting
///           on the servers instead would give 2 and 2, which is the mutant this
///           counts out.
///         * Redis is the odd one in three separate ways at once here: no child
///           resource, comma-separated options rather than semicolon-separated
///           clauses, and no scheme in the string.
/// </summary>
public static class Ex035_BeginnerCapstoneModel
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex035 - build one model holding: SQL Server \"sql\" with database "
            + "\"catalog\"; PostgreSQL \"pg\" with database \"orders\"; MongoDB "
            + "\"docs\" with database \"reviews\"; Redis \"cache\". Then two nginx "
            + "containers: \"catalog-api\", which references AND waits for \"catalog\" "
            + "and \"cache\", and \"orders-api\", which references AND waits for "
            + "\"orders\" and \"reviews\".");
}
