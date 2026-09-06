using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Order a three-step startup correctly: the database comes up, a one-shot
///         migration job runs to completion against it, and only then does the
///         long-running API start.
/// Drills: `WaitForCompletion` versus `WaitFor`, and WaitAnnotation.WaitType. The
///         two calls are not interchangeable and choosing the wrong one is not a
///         style question: WaitFor on the one-shot job releases the API as soon as
///         the job is up, i.e. BEFORE the migration has finished, while
///         WaitForCompletion on the database server means "start after Postgres
///         exits", i.e. never.
/// Passes: "migrator" waits for "orders" with WaitType.WaitUntilHealthy and carries
///         no completion wait at all; "api" has exactly ONE wait, naming "migrator",
///         with WaitType.WaitForCompletion and ExitCode 0 - so the API's only gate
///         is the migrator, whose own gate is the database.
/// Note:   Measured on 13.5.3, and the reason this row is graded on WaitType rather
///         than on the presence of a WaitAnnotation: BOTH calls produce a
///         WaitAnnotation, so a test that only counts them cannot tell the correct
///         model from the exactly-inverted one. Measured too - WaitFor(orders) emits
///         a WaitAnnotation for the parent SERVER "pg" as well as for "orders", so
///         a wait assertion has to filter by resource name.
/// </summary>
public static class Ex015_WaitForCompletion
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var orders = builder.AddPostgres("pg")
                            .AddDatabase("orders");

        // A long-running server never exits, so the only sensible promise to wait for
        // is its HEALTH. WaitForCompletion here would mean "start after Postgres
        // stops" and the migrator would never run at all.
        var migrator = builder.AddContainer("migrator", "migrate/migrate")
                              .WaitFor(orders);

        // A one-shot job is the opposite case: it is meant to finish, and "finished
        // successfully" is exactly the promise the API depends on. WaitFor here would
        // release the API as soon as the migrator STARTED, i.e. mid-migration.
        // The API needs no wait on the database of its own - the migrator already
        // carries that gate, and the chain orders all three.
        builder.AddContainer("api", "nginx")
               .WaitForCompletion(migrator);
    }
}
