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
        => throw new NotImplementedException(
            "TODO: ex015 - add a Postgres server \"pg\" with a database \"orders\"; "
            + "add a one-shot container \"migrator\" on image \"migrate/migrate\" "
            + "that waits for \"orders\" to be healthy; and add a long-running "
            + "container \"api\" on image \"nginx\" that starts only once the "
            + "migrator has run to completion.");
}
