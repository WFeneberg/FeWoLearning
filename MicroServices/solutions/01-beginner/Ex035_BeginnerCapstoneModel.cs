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
    {
        // Three flavours that HAVE a database child, and the child - not the server -
        // is what a consumer references: it is the child whose connection string
        // carries the database, and the child whose health means "ready to query".
        var catalog = builder.AddSqlServer("sql").AddDatabase("catalog");
        var orders = builder.AddPostgres("pg").AddDatabase("orders");
        var reviews = builder.AddMongoDB("docs").AddDatabase("reviews");

        // The fourth flavour has no child at all. A Redis logical database is an
        // integer selected on the connection, not a resource in the graph (ex029).
        var cache = builder.AddRedis("cache");

        // Two edges each, and both halves matter. WithReference decides WHAT the
        // container is told; WaitFor decides WHEN it is started. A model with only the
        // first starts every service immediately and lets them fail their first query.
        builder.AddContainer("catalog-api", "nginx")
               .WithReference(catalog).WaitFor(catalog)
               .WithReference(cache).WaitFor(cache);

        builder.AddContainer("orders-api", "nginx")
               .WithReference(orders).WaitFor(orders)
               .WithReference(reviews).WaitFor(reviews);
    }
}
