using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Write one AppHost file that produces two different graphs: a throwaway
///         Postgres container when a developer runs it, and a reference to a managed
///         database that already exists when it is published.
/// Drills: `builder.ExecutionContext.IsRunMode` / `IsPublishMode`. The execution
///         context is decided before any resource is added, so the branch is an
///         ordinary `if` around ordinary builder calls - and the consumer on the
///         other side does not change at all, because both branches produce a
///         resource named "orders" with a connection string.
/// Passes: In RUN mode the model holds "pg" (a PostgresServerResource) and "orders"
///         as a PostgresDatabaseResource whose expression is
///         "{pg.connectionString};Database=orders". In PUBLISH mode there is no "pg"
///         at all and "orders" is a ConnectionStringResource reading
///         "Host=orders.postgres.database.contoso.com;Port=5432;Database=orders;Username=app",
///         which publishes as value.v0. "api" is in both.
/// Note:   The row fails an implementation that branches on nothing, so the two
///         graphs are compared directly and have to differ in a NAMED way - the
///         presence of "pg" and the runtime type of "orders" - rather than merely
///         somewhere. Note also which harness is which: ModelHarness builds in run
///         mode, ManifestHarness publishes. A publish-mode MODEL, which is what the
///         second fact needs, comes from ModelHarness.BuildForPublish - the same
///         "--operation publish" args ManifestHarness passes, but stopping at
///         Build() so nothing is written to disk.
/// </summary>
public static class Ex020_RunVersusPublishMode
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex020 - in run mode add a Postgres server \"pg\" with a database "
            + "\"orders\"; in publish mode add a connection string resource "
            + "\"orders\" reading "
            + "\"Host=orders.postgres.database.contoso.com;Port=5432;Database=orders;Username=app\" "
            + "and no server at all. In both modes add a container \"api\" on image "
            + "\"nginx\" that references \"orders\".");
}
