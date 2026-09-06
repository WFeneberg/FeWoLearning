using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Hook the AppHost's own lifecycle - run code before anything starts, and run
///         code when one particular resource is genuinely ready - and see why those are
///         three different moments rather than one.
/// Drills: `builder.Eventing.Subscribe&lt;T&gt;(...)` in its two shapes. The app-scoped
///         overload takes a handler alone and fires once for the whole application
///         (`BeforeStartEvent`); the resource-scoped overload takes a resource FIRST and
///         fires only for that resource (`BeforeResourceStartedEvent`,
///         `ResourceReadyEvent`). And the moments themselves: BeforeStart is before the
///         orchestrator exists, BeforeResourceStarted is before this container is
///         launched, and ResourceReady is after it has passed its health checks. READY
///         IS NOT STARTED - a Postgres container is started seconds before it will
///         accept a connection, and that gap is exactly what `WaitFor` is waiting out.
/// Passes: After Build() nothing has fired - subscribing is not running. Publishing
///         BeforeStartEvent records the execution context the event carried, so the same
///         file records "run" under a run-mode builder and "publish" under a publish-mode
///         one. Publishing the two resource events for "db" records "starting:db" then
///         "ready:db"; publishing the same two for "api" records nothing at all.
/// Note:   `WaitFor` is a MODEL annotation, not an event: it is on "api" from the moment
///         the graph is assembled, long before any hook runs. Both mechanisms are graded
///         here because confusing them is the whole misconception - "I subscribed to
///         ResourceReadyEvent" is not a substitute for `WaitFor`, and the annotation is
///         what the orchestrator actually schedules on.
///
///         The mutant this row exists to reject is the app-scoped overload used where
///         the resource-scoped one belongs: `Subscribe&lt;ResourceReadyEvent&gt;(handler)`
///         with no resource. Measured on 13.5.3, it fires for EVERY resource - so it
///         passes every positive fact here and is caught only by publishing the same
///         events for a resource nobody subscribed to.
/// </summary>
public static class Ex025_EventingAndLifecycleHooks
{
    // ---------------------------------------------------------------------------
    // GIVEN - somewhere for a hook to leave a trace. Nothing here is a TODO; the test
    // calls Reset() before each run and reads Hooks afterwards.
    // ---------------------------------------------------------------------------

    private static readonly List<string> Entries = [];

    /// <summary>What has fired so far, in order.</summary>
    public static IReadOnlyList<string> Hooks
    {
        get { lock (Entries) { return Entries.ToArray(); } }
    }

    public static void Reset()
    {
        lock (Entries) { Entries.Clear(); }
    }

    /// <summary>Call this from a hook to record that it ran.</summary>
    public static void Record(string entry)
    {
        lock (Entries) { Entries.Add(entry); }
    }

    // ---------------------------------------------------------------------------
    // The exercise
    // ---------------------------------------------------------------------------

    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var db = builder.AddContainer("db", "postgres");

        // The model-level mechanism, and the one the orchestrator actually schedules on.
        // No event is involved: this annotation is on "api" the moment the line runs.
        builder.AddContainer("api", "nginx")
               .WaitFor(db);

        // App-scoped: no resource argument, fires once for the whole application, before
        // the orchestrator has started anything at all. The event carries the live
        // service provider, which is how a hook learns what kind of run it is in.
        builder.Eventing.Subscribe<BeforeStartEvent>((@event, cancellationToken) =>
        {
            var context = @event.Services
                                .GetRequiredService<DistributedApplicationExecutionContext>();
            Record(context.IsPublishMode ? "before-start:publish" : "before-start:run");
            return Task.CompletedTask;
        });

        // Resource-scoped: the resource comes FIRST. The app-scoped overload compiles
        // just as happily and fires for every resource in the graph.
        builder.Eventing.Subscribe<BeforeResourceStartedEvent>(db.Resource,
            (@event, cancellationToken) =>
            {
                Record($"starting:{@event.Resource.Name}");
                return Task.CompletedTask;
            });

        // ...and this is a different moment entirely. "Started" means the container was
        // launched; "ready" means it has passed its health checks and will answer. The
        // gap between the two is why WaitFor above exists.
        builder.Eventing.Subscribe<ResourceReadyEvent>(db.Resource,
            (@event, cancellationToken) =>
            {
                Record($"ready:{@event.Resource.Name}");
                return Task.CompletedTask;
            });
    }
}
