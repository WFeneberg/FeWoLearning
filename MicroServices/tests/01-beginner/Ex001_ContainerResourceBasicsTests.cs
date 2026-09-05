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
    public void Database_hangs_off_that_server_and_composes_its_connection_string()
    {
        var model = ModelHarness.Build(Ex001_ContainerResourceBasics.Configure);

        var pg = Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        var orders = Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));

        // The server/database SPLIT is the exercise, so the parent link is asserted
        // against the very server object named "pg" - not merely "some server".
        // Reference equality is what rejects a learner who added two Postgres servers
        // and hung the database off the wrong one.
        Assert.Same(pg, orders.Parent);

        // And the composition that link produces: "orders" is not an independent
        // connection string, it is the server's with a Database= clause appended.
        // Both halves of the expression are learner-determined - the resource names
        // and, via AddDatabase's databaseName, the value after Database=.
        Assert.Equal(
            "{pg.connectionString};Database=orders",
            ModelHarness.ConnectionString(orders));

        // Deliberately NOT asserted: anything inside the SERVER's own connection
        // string (Host, Port, Username=postgres, the generated password). AddPostgres
        // determines all of it, the learner writes none of it, and fact 1 already
        // proves the flavour by type. Grading framework-generated text is the habit
        // catalog.md's preamble warns against - do not copy it into later exercises.
    }
}
