using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.MongoDB;
using Aspire.Hosting.Postgres;
using Aspire.Hosting.Redis;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex035_BeginnerCapstoneModelTests
{
    private static async Task<Dictionary<string, object>> EnvironmentOf(
        ModelHarness.Result model, string name, CancellationToken cancellationToken)
    {
        var resource = model.Resource(name);
        var environment = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run),
            resource, environment, cancellationToken);
        foreach (var callback in resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await callback.Callback(context);
        }
        return environment;
    }

    [Fact]
    public void Four_stores_four_types_and_four_STRUCTURALLY_different_connection_strings()
    {
        var model = ModelHarness.Build(Ex035_BeginnerCapstoneModel.Configure);

        // Servers and children, by type. Every one of these is satisfied by
        // AddContainer(name, image) if only names are graded, which is why the whole
        // tier insists on the type.
        Assert.IsType<SqlServerServerResource>(model.Resource("sql"));
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<MongoDBServerResource>(model.Resource("docs"));
        Assert.IsType<RedisResource>(model.Resource("cache"));

        var catalog = Assert.IsType<SqlServerDatabaseResource>(model.Resource("catalog"));
        var orders = Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
        var reviews = Assert.IsType<MongoDBDatabaseResource>(model.Resource("reviews"));

        Assert.Same(model.Resource("sql"), catalog.Parent);
        Assert.Same(model.Resource("pg"), orders.Parent);
        Assert.Same(model.Resource("docs"), reviews.Parent);

        // Redis is the flavour with no child at all - graded in both directions in the
        // same model, so a helper that answered "no children" for everything would fail
        // on the three above.
        Assert.IsNotAssignableFrom<IResourceWithParent>(model.Resource("cache"));
        Assert.DoesNotContain(model.Resources.OfType<IResourceWithParent>(), r => r.Parent.Name == "cache");

        // The four strings a consumer actually receives. Each is a different SHAPE, not
        // a different spelling: a keyed ADO string whose database clause says
        // "Initial Catalog"; a keyed ADO string whose database clause says "Database";
        // a URI whose database is a PATH SEGMENT in the middle; and a bare host:port
        // with comma-separated StackExchange.Redis options and no scheme.
        var connections = new[] { catalog, orders, reviews, (IResource)model.Resource("cache") }
            .Select(ModelHarness.ConnectionString)
            .ToArray();
        Assert.Equal(4, connections.Distinct().Count());

        Assert.Equal("{sql.connectionString};Initial Catalog=catalog", connections[0]);
        Assert.Equal("{pg.connectionString};Database=orders", connections[1]);
        Assert.Equal(
            "mongodb://admin:{docs-password.value}@{docs.bindings.tcp.host}:{docs.bindings.tcp.port}"
            + "/reviews?authSource=admin&authMechanism=SCRAM-SHA-256",
            connections[2]);
        Assert.StartsWith("{cache.bindings.tcp.host}:{cache.bindings.tcp.port},password=", connections[3]);

        // ...and the structural claims, stated separately from the exact strings so the
        // row keeps grading the shape if a future version reformats one of them: two of
        // the four defer to their parent's string, one re-renders the whole URI because
        // its database name sits in the middle of it, and one is not keyed at all.
        Assert.All(connections[..2], c => Assert.StartsWith("{", c));
        Assert.DoesNotContain("{reviews", connections[2]);
        Assert.DoesNotContain(";", connections[3]);
    }

    [Fact]
    public async Task ONE_WithReference_call_hands_over_a_different_variable_set_per_flavour()
    {
        var model = ModelHarness.Build(Ex035_BeginnerCapstoneModel.Configure);
        var token = TestContext.Current.CancellationToken;

        // This is the fact only the combination can produce. Measured on 13.5.3: the
        // same WithReference call gives a consumer ConnectionStrings__<name> plus SEVEN
        // flavour-specific siblings for SQL Server and PostgreSQL, NINE for MongoDB and
        // FIVE for Redis - so four references in one model yield four different key
        // sets, and every key is the RESOURCE name upper-cased rather than the database
        // name. A model that referenced the SERVERS instead of the databases produces
        // SQL_*/PG_*/DOCS_* and ConnectionStrings__sql, and fails on the very first
        // comparison.
        var catalogApi = await EnvironmentOf(model, "catalog-api", token);
        Assert.Equal(
            new[]
            {
                "CACHE_HOST", "CACHE_PASSWORD", "CACHE_PORT", "CACHE_URI",
                "CATALOG_DATABASENAME", "CATALOG_HOST", "CATALOG_JDBCCONNECTIONSTRING",
                "CATALOG_PASSWORD", "CATALOG_PORT", "CATALOG_URI", "CATALOG_USERNAME",
                "ConnectionStrings__cache", "ConnectionStrings__catalog"
            },
            catalogApi.Keys.Order(StringComparer.Ordinal).ToArray());

        var ordersApi = await EnvironmentOf(model, "orders-api", token);
        Assert.Equal(
            new[]
            {
                "ConnectionStrings__orders", "ConnectionStrings__reviews",
                "ORDERS_DATABASENAME", "ORDERS_HOST", "ORDERS_JDBCCONNECTIONSTRING",
                "ORDERS_PASSWORD", "ORDERS_PORT", "ORDERS_URI", "ORDERS_USERNAME",
                "REVIEWS_AUTHENTICATIONDATABASE", "REVIEWS_AUTHENTICATIONMECHANISM",
                "REVIEWS_DATABASENAME", "REVIEWS_HOST", "REVIEWS_PASSWORD",
                "REVIEWS_PORT", "REVIEWS_URI", "REVIEWS_USERNAME"
            },
            ordersApi.Keys.Order(StringComparer.Ordinal).ToArray());

        // The three flavour fingerprints inside those sets, so the fact does not rest
        // on key names alone. Mongo is the only one that needs an authentication
        // database; Redis is the only one whose URI takes its scheme from the BINDING
        // rather than from the connection string, which is why "no scheme" in fact 1
        // and a scheme here are both true at once; and the JDBC forms differ by driver.
        Assert.Equal("admin", ((IManifestExpressionProvider)ordersApi["REVIEWS_AUTHENTICATIONDATABASE"]).ValueExpression);
        Assert.StartsWith("{cache.bindings.tcp.scheme}://",
            ((IManifestExpressionProvider)catalogApi["CACHE_URI"]).ValueExpression);
        Assert.StartsWith("jdbc:sqlserver://",
            ((IManifestExpressionProvider)catalogApi["CATALOG_JDBCCONNECTIONSTRING"]).ValueExpression);
        Assert.StartsWith("jdbc:postgresql://",
            ((IManifestExpressionProvider)ordersApi["ORDERS_JDBCCONNECTIONSTRING"]).ValueExpression);
    }

    [Fact]
    public void Waiting_on_a_CHILD_waits_on_its_server_too_so_the_two_consumers_differ()
    {
        var model = ModelHarness.Build(Ex035_BeginnerCapstoneModel.Configure);

        static string[] WaitedOn(IResource resource)
            => resource.Annotations.OfType<WaitAnnotation>()
                       .Select(w => w.Resource.Name).Order(StringComparer.Ordinal).ToArray();

        // Two calls each, three annotations against four - because "catalog" and
        // "orders" and "reviews" each drag their server along and "cache" has none.
        // The count is the combination-only part: no single-store row can show that two
        // consumers written the same way end up with different-sized wait sets.
        Assert.Equal(new[] { "cache", "catalog", "sql" }, WaitedOn(model.Resource("catalog-api")));
        Assert.Equal(new[] { "docs", "orders", "pg", "reviews" }, WaitedOn(model.Resource("orders-api")));

        Assert.All(model.Resource("catalog-api").Annotations.OfType<WaitAnnotation>(),
            w => Assert.Equal(WaitType.WaitUntilHealthy, w.WaitType));
        Assert.All(model.Resource("orders-api").Annotations.OfType<WaitAnnotation>(),
            w => Assert.Equal(WaitType.WaitUntilHealthy, w.WaitType));

        // The mutant this rejects is the one that reads best: WaitFor(sqlServer) and
        // WaitFor(postgresServer) - waiting for the SERVER, which is what a reader
        // expects "wait for the database to be up" to mean. It leaves two annotations
        // on each consumer instead of three and four, and it starts the service before
        // Aspire has created the database, so the first query fails against a server
        // that is perfectly healthy.

        // Nobody waits across the boundary: the two services share no store, so a
        // model that hung every wait off both consumers passes the counts and fails here.
        Assert.DoesNotContain("orders", WaitedOn(model.Resource("catalog-api")));
        Assert.DoesNotContain("catalog", WaitedOn(model.Resource("orders-api")));

        // The stores themselves carry no wait annotations - waiting is a property of
        // the consumer, and a model that put WaitFor on the database would compile.
        foreach (var store in new[] { "sql", "catalog", "pg", "orders", "docs", "reviews", "cache" })
        {
            Assert.Empty(model.Resource(store).Annotations.OfType<WaitAnnotation>());
        }
    }
}
