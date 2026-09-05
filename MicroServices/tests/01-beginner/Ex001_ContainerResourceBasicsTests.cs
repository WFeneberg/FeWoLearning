using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex001_ContainerResourceBasicsTests
{
    [Fact]
    public void Models_a_postgres_server_and_a_database_on_it()
    {
        var model = ModelHarness.Build(Ex001_ContainerResourceBasics.Configure);

        // The TYPE matters, not merely that something called "pg" exists:
        // AddContainer("pg", "postgres") plus AddContainer("orders", "postgres")
        // would satisfy a name-only assertion just as well.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
    }

    [Fact]
    public void Database_connection_string_composes_onto_the_server()
    {
        var model = ModelHarness.Build(Ex001_ContainerResourceBasics.Configure);

        // The composition is the point: "orders" is not an independent connection
        // string, it is the server's with a Database= clause appended. A learner who
        // added two unrelated Postgres SERVERS would get "{orders.connectionString}"
        // shaped output here and fail.
        Assert.Equal(
            "{pg.connectionString};Database=orders",
            ModelHarness.ConnectionString(model.Resource("orders")));

        Assert.Contains(
            "Username=postgres",
            ModelHarness.ConnectionString(model.Resource("pg")));
    }
}
