using System.Globalization;
using Aspire.Hosting;
using Npgsql;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Start a real PostgreSQL and run a real query through the connection string
///         Aspire resolved - the first exercise in the track where anything is
///         actually running. Everything before this graded a graph or an artifact.
/// Drills: `AddDatabase(name, databaseName)` again, but now with the consequence
///         visible: the `{pg.connectionString};Database=orders_v1` expression that L1
///         can only look at becomes
///         `Host=localhost;Port=&lt;whatever DCP picked&gt;;Username=postgres;Password=&lt;generated&gt;;Database=orders_v1`,
///         and a client either connects with it or does not.
/// Passes: at L1 - "pg" is a PostgresServerResource and "orders" a
///         PostgresDatabaseResource parented to it, with the Postgres-flavoured
///         expression. At L3 (`dotnet test -p:Containers=true`) - the container starts,
///         the resource reports healthy, and ReadScalarAsync returns a value that the
///         TEST wrote into the database moments earlier, which nothing but a real
///         connection can produce.
/// Note:   The L3 fact is skipped unless `-p:Containers=true` (or FEWO_MS_CONTAINERS=1)
///         is set; with the switch ON and Docker unreachable it FAILS rather than
///         skipping, on purpose - see MicroServices/README.md sections 3 and 4.
///         ReadScalarAsync is deliberately given the SQL as a parameter rather than
///         owning a query of its own: the test picks the statement, so an
///         implementation that returns a plausible constant instead of connecting has
///         nothing to guess.
/// </summary>
public static class Ex034_FirstRealQuery
{
    /// <summary>
    /// The RESOURCE name - the key a consumer reads (<c>ConnectionStrings__orders</c>).
    /// </summary>
    public const string ResourceName = "orders";

    /// <summary>
    /// The DATABASE name inside Postgres. Deliberately different from
    /// <see cref="ResourceName"/>; ex027 is the row about why those are two things.
    /// </summary>
    public const string DatabaseName = "orders_v1";

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Two names, on purpose: "orders" is what a consumer looks up, "orders_v1" is
        // what CREATE DATABASE gets called with and what current_database() answers.
        builder.AddPostgres("pg")
               .AddDatabase(ResourceName, DatabaseName);
    }

    /// <summary>
    /// Opens <paramref name="connectionString"/>, executes <paramref name="sql"/> and
    /// returns its first column of its first row as text (null when there is no row, or
    /// when the value is NULL).
    /// </summary>
    public static async Task<string?> ReadScalarAsync(
        string connectionString, string sql, CancellationToken cancellationToken)
    {
        // The connection string is an argument, not a constant. Under the AppHost it
        // arrives as ConnectionStrings__orders; here the test hands over exactly what
        // the running application resolved, port and generated password included.
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        var value = await command.ExecuteScalarAsync(cancellationToken);

        // ExecuteScalar answers null for "no rows" and DBNull for "the value was NULL";
        // both are "nothing" to a caller.
        return value is null or DBNull
            ? null
            : Convert.ToString(value, CultureInfo.InvariantCulture);
    }
}
