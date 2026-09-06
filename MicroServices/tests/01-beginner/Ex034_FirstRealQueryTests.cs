using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;
using FeWoLearning.MicroServices.Exercises.Beginner;
using Npgsql;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex034_FirstRealQueryTests
{
    [Fact]
    public void The_model_is_a_real_Postgres_database_child_and_not_a_lookalike_container()
    {
        var model = ModelHarness.Build(Ex034_FirstRealQuery.Configure);

        // L1, so it runs in every mode and the row is graded even with containers off.
        // The type plus the expression, per the track's persistence rule: a generic
        // AddContainer("pg", "postgres") plus AddConnectionString renders a plausible
        // string and is neither of these types.
        var server = Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.Equal(
            "Host={pg.bindings.tcp.host};Port={pg.bindings.tcp.port};Username=postgres;Password={pg-password.value}",
            ModelHarness.ConnectionString(server));

        var database = Assert.IsType<PostgresDatabaseResource>(
            model.Resource(Ex034_FirstRealQuery.ResourceName));
        Assert.Same(server, Assert.IsAssignableFrom<IResourceWithParent>(database).Parent);

        // The two names are different things, and this is the assertion that says so:
        // the RESOURCE is called "orders" (so a consumer reads ConnectionStrings__orders),
        // and the string it hands over selects the DATABASE "orders_v1". ex027 is the
        // row about that distinction; here it is also what the L3 fact reads back out
        // of a running server through current_database().
        Assert.Equal(
            $"{{pg.connectionString}};Database={Ex034_FirstRealQuery.DatabaseName}",
            ModelHarness.ConnectionString(database));
    }

    [Fact]
    public async Task A_real_Postgres_starts_and_the_INJECTED_connection_string_reaches_the_query()
    {
        // First line, always. FactAttribute.Skip is not virtual on xunit.v3 3.2.2, so
        // there is no [ContainerFact]; this is the whole gate. With the switch OFF the
        // test skips and no daemon is touched; with the switch ON and Docker
        // unreachable it FAILS, which is the point - a broken Docker setup must not be
        // able to report green by quietly skipping.
        ContainerGate.Require();

        var token = TestContext.Current.CancellationToken;
        var expected = Guid.NewGuid().ToString("N");

        await ContainerHarness.RunAsync(Ex034_FirstRealQuery.Configure, async session =>
        {
            // The harness half of "WaitFor it". A consumer inside the model waits
            // through a WaitAnnotation; a test is not in the model, so it waits here.
            // For a Postgres AddDatabase child, healthy means the entrypoint finished
            // AND Aspire has created the database - so this is also what makes the
            // query below deterministic rather than a race.
            await session.WaitForHealthyAsync(Ex034_FirstRealQuery.ResourceName);

            var connectionString = await session.ConnectionStringAsync(Ex034_FirstRealQuery.ResourceName);

            // This is the thing no model-level test can assert: the expression is GONE.
            // At L1 the same resource reads "{pg.connectionString};Database=orders_v1";
            // here every placeholder has been replaced by a real host, the port DCP
            // allocated for this run, and the password Aspire generated.
            //
            // Note what is NOT asserted: that the string contains no "{" at all. Aspire
            // generates the Postgres password from a character set that includes braces,
            // so a run in ten produces a perfectly resolved connection string with a "{"
            // in the password - measured, and it failed exactly that way the first time
            // this fact ran. Name the placeholders instead.
            Assert.DoesNotContain("{pg.", connectionString);
            Assert.DoesNotContain(".connectionString}", connectionString);
            Assert.Contains($"Database={Ex034_FirstRealQuery.DatabaseName}", connectionString);

            // ...and the port really was allocated rather than defaulted: DCP publishes
            // Postgres on an ephemeral host port, never on 5432, which is the most
            // legible single proof that a hard-coded connection string could not have
            // reached this database.
            var resolved = new NpgsqlConnectionStringBuilder(connectionString);
            Assert.NotEqual(0, resolved.Port);
            Assert.NotEqual(5432, resolved.Port);
            Assert.False(string.IsNullOrEmpty(resolved.Password));

            // Seed from the TEST's own connection, with a value invented microseconds
            // ago. This is what makes the fact depend on the connection string reaching
            // the client rather than on the container merely being up: an implementation
            // that returns a plausible constant, or that parses the answer out of the
            // connection string it was handed, has nothing to work from - the value is
            // not in the string, not in the exercise, and not on disk.
            await using (var seed = new NpgsqlConnection(connectionString))
            {
                await seed.OpenAsync(token);
                await using var create = new NpgsqlCommand(
                    "CREATE TABLE IF NOT EXISTS ex034_probe (value text NOT NULL); "
                    + "TRUNCATE ex034_probe; "
                    + $"INSERT INTO ex034_probe (value) VALUES ('{expected}');", seed);
                await create.ExecuteNonQueryAsync(token);
            }

            var actual = await Ex034_FirstRealQuery.ReadScalarAsync(
                connectionString, "SELECT value FROM ex034_probe", token);
            Assert.Equal(expected, actual);

            // ...and the server really is the one the model described, read from inside
            // rather than off the connection string: current_database() is answered by
            // Postgres, so it cannot be satisfied by a client that parsed "Database=".
            var databaseName = await Ex034_FirstRealQuery.ReadScalarAsync(
                connectionString, "SELECT current_database()", token);
            Assert.Equal(Ex034_FirstRealQuery.DatabaseName, databaseName);

            // The connection string is a PARAMETER, not decoration. Point the same call
            // at a port nothing listens on and it must fail rather than answer from
            // somewhere else. Asserted as "threw, and not NotImplementedException",
            // because "it threw" alone is satisfied by an untouched stub.
            var broken = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Port = 1,
                Timeout = 2,
                CommandTimeout = 2
            }.ConnectionString;

            var failure = await Record.ExceptionAsync(() =>
                Ex034_FirstRealQuery.ReadScalarAsync(broken, "SELECT 1", token));
            Assert.NotNull(failure);
            Assert.IsNotType<NotImplementedException>(failure);
        }, token);
    }
}
