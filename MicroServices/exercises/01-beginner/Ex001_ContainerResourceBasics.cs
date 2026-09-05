using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a PostgreSQL server with one database on it.
/// Drills: `AddPostgres`, `AddDatabase`, the server/database resource split.
/// Passes: The model contains a PostgresServerResource named "pg" and a
///         PostgresDatabaseResource named "orders" whose connection-string
///         expression composes onto the server's.
/// </summary>
public static class Ex001_ContainerResourceBasics
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: add a Postgres server named 'pg' with a database named 'orders'.");
}
