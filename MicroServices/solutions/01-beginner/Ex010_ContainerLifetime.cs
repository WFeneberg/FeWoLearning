using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Decide, per container, whether it is torn down when the AppHost stops.
/// Drills: `WithLifetime` and the ContainerLifetimeAnnotation it writes. A Session
///         container is created and destroyed with the AppHost run that started it,
///         so restarting the AppHost hands you a fresh, empty one. A Persistent
///         container is created once and left running when the AppHost exits, so
///         the next run finds it and reuses it - the same container, the same
///         volume contents, no re-seeding. That is why a developer pins a database
///         Persistent and leaves a stateless API on the default.
/// Passes: "db" carries a ContainerLifetimeAnnotation of Persistent, "api" one of
///         Session, and "worker" carries none - the default is the ABSENCE of the
///         annotation, not an annotation holding Session.
/// Note:   Measured on Aspire 13.5.3 - lifetime does not appear in the published
///         manifest at all (persistent, session and untouched containers publish
///         identically), because it is a local run-mode concept. This exercise is
///         therefore graded entirely against the model.
/// </summary>
public static class Ex010_ContainerLifetime
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // Persistent: survives the AppHost exiting, so the next run reattaches to
        // this container and its data instead of starting an empty one.
        builder.AddContainer("db", "postgres")
               .WithLifetime(ContainerLifetime.Persistent);

        // Session, stated explicitly: created and destroyed with this AppHost run.
        builder.AddContainer("api", "nginx")
               .WithLifetime(ContainerLifetime.Session);

        // Left alone. Session is the default behaviour, and asking for it writes an
        // annotation that not asking for it does not - which is the difference the
        // third test fact grades.
        builder.AddContainer("worker", "busybox");
    }
}
