using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Wire a worker to a Postgres database so that it BOTH receives the
///         connection configuration AND starts only after the database is ready.
/// Drills: `WithReference` (injects config) versus `WaitFor` (orders startup) -
///         two different jobs that are easy to confuse for one.
/// Passes: The worker carries a WaitAnnotation for the database AND an
///         EnvironmentCallbackAnnotation from the reference, and the manifest
///         shows a ConnectionStrings__orders entry in its environment.
/// </summary>
public static class Ex002_ReferenceVersusWaitFor
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var orders = builder.AddPostgres("pg").AddDatabase("orders");

        // Two separate jobs: WithReference puts ConnectionStrings__orders into the
        // worker's environment; WaitFor holds the worker back until the database
        // reports healthy. Neither implies the other.
        builder.AddContainer("worker", "busybox")
               .WithReference(orders)
               .WaitFor(orders);
    }
}
