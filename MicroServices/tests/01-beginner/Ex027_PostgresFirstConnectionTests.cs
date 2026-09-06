using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex027_PostgresFirstConnectionTests
{
    /// <summary>
    /// Runs a consumer's environment callbacks and hands back what they wrote, as
    /// unresolved EXPRESSIONS. Same technique as ex007: the values a callback lands
    /// are ReferenceExpression / ConnectionStringReference objects, not strings, and
    /// ToString() on them is the type name.
    /// </summary>
    private static async Task<Dictionary<string, string>> EnvironmentOfAsync(
        IResource resource, CancellationToken ct)
    {
        var values = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run),
            resource, values, ct);
        foreach (var annotation in resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await annotation.Callback(context);
        }
        return values.ToDictionary(
            kv => kv.Key,
            kv => kv.Value switch
            {
                IManifestExpressionProvider expression => expression.ValueExpression,
                _ => kv.Value.ToString() ?? string.Empty
            });
    }

    [Fact]
    public void The_resource_name_and_the_database_name_are_two_different_names()
    {
        var model = ModelHarness.Build(Ex027_PostgresFirstConnection.Configure);

        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        var db = Assert.IsType<PostgresDatabaseResource>(model.Resource("ordersdb"));
        Assert.Same(pg, db.Parent);

        // The whole subject in one line: the resource is called "ordersdb", the
        // database it asks Postgres for is called "orders_v2". A learner who used the
        // one-argument AddDatabase("ordersdb") renders ";Database=ordersdb" and fails
        // here; one who used AddDatabase("orders_v2") fails on the name above.
        Assert.Equal("{pg.connectionString};Database=orders_v2",
            ModelHarness.ConnectionString(db));

        // And the database name is NOT a resource. This is the assertion that stops
        // "two names" from being satisfiable by two resources.
        Assert.False(model.Has("orders_v2"));

        // The consumer exists and is an ordinary container - nothing about this row
        // asks for a special one.
        Assert.IsType<ContainerResource>(model.Resource("api"));
    }

    [Fact]
    public async Task The_consumer_is_keyed_by_the_resource_name_and_told_the_database_name()
    {
        var model = ModelHarness.Build(Ex027_PostgresFirstConnection.Configure);
        var env = await EnvironmentOfAsync(model.Resource("api"), TestContext.Current.CancellationToken);

        // The key a consumer reads with configuration["ConnectionStrings:ordersdb"]
        // is the RESOURCE name. Rename the resource and every service that reads it
        // has to change; rename only the database and none of them does.
        Assert.Equal("{ordersdb.connectionString}",
            Assert.Contains("ConnectionStrings__ordersdb", env));
        Assert.DoesNotContain("ConnectionStrings__orders_v2", env.Keys);

        // The DATABASE name is delivered separately, in its own variable, and it is
        // the other of the two names. If the learner let one default to the other,
        // these two assertions would agree with each other and say nothing - which
        // is exactly why the row insists they differ.
        Assert.Equal("orders_v2", Assert.Contains("ORDERSDB_DATABASENAME", env));

        // Framework-determined, and worth seeing where it is actually used rather
        // than only inside the server's connection string: AddPostgres fixes the
        // superuser to "postgres" and the learner writes none of it.
        Assert.Equal("postgres", Assert.Contains("ORDERSDB_USERNAME", env));

        // The coordinates, still unresolved - they come from the PARENT's endpoint,
        // which is what makes them deferred rather than baked in.
        Assert.Equal("{pg.bindings.tcp.host}", Assert.Contains("ORDERSDB_HOST", env));
        Assert.Equal("{pg.bindings.tcp.port}", Assert.Contains("ORDERSDB_PORT", env));
        Assert.Equal("{pg-password.value}", Assert.Contains("ORDERSDB_PASSWORD", env));
    }

    [Fact]
    public async Task A_real_database_resource_also_emits_the_URI_and_JDBC_forms()
    {
        var model = ModelHarness.Build(Ex027_PostgresFirstConnection.Configure);
        var env = await EnvironmentOfAsync(model.Resource("api"), TestContext.Current.CancellationToken);

        // The fact this row exists for, and the one that separates AddDatabase from
        // any hand-rolled string that renders identically. Measured on 13.5.3:
        //
        //     var pg = builder.AddPostgres("pg");
        //     var db = builder.AddConnectionString("ordersdb",
        //         ReferenceExpression.Create($"{pg.Resource};Database=orders_v2"));
        //     builder.AddContainer("api", "nginx").WithReference(db);
        //
        // renders the byte-identical "{pg.connectionString};Database=orders_v2" that
        // fact 1 asserts, and the consumer receives exactly ONE variable -
        // ConnectionStrings__ordersdb - and none of these. ex014 rejects that mutant
        // through the Parent link; this rejects it through what the consumer sees.
        Assert.Equal(
            "postgresql://postgres:{pg-password.value}@{pg.bindings.tcp.host}:{pg.bindings.tcp.port}/orders_v2",
            Assert.Contains("ORDERSDB_URI", env));

        Assert.Equal(
            "jdbc:postgresql://{pg.bindings.tcp.host}:{pg.bindings.tcp.port}/orders_v2",
            Assert.Contains("ORDERSDB_JDBCCONNECTIONSTRING", env));

        // Both sibling forms end in the DATABASE name, and neither mentions the
        // resource name - stated on its own because it is the same "two names" claim
        // seen from the other side, and because a learner who swapped the two
        // arguments produces strings that still look plausible.
        Assert.EndsWith("/orders_v2", Assert.Contains("ORDERSDB_URI", env));
        Assert.DoesNotContain("ordersdb", Assert.Contains("ORDERSDB_URI", env));

        // The full set, asserted as a set: seven siblings plus the connection string.
        // A count rather than another spelling of the assertions above - it is what
        // notices an Aspire bump that quietly drops or adds one.
        string[] expected =
        [
            "ConnectionStrings__ordersdb",
            "ORDERSDB_HOST", "ORDERSDB_PORT", "ORDERSDB_USERNAME", "ORDERSDB_PASSWORD",
            "ORDERSDB_DATABASENAME", "ORDERSDB_URI", "ORDERSDB_JDBCCONNECTIONSTRING"
        ];
        Assert.Equal(expected.Order(), env.Keys.Order());
    }
}
