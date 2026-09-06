using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

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
///
///         "From the event's own Services" is a graded claim, not a style note. A handler
///         that closes over `builder.ExecutionContext` instead answers correctly in both
///         a run-mode and a publish-mode application, so those two cases alone cannot
///         separate the two. The test therefore also hands a RUN-mode application a
///         BeforeStartEvent whose service provider reports publish mode: the event is the
///         authority, and the closure disagrees with it.
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
    // TODO
    // ---------------------------------------------------------------------------

    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex025 - add a container \"db\" on image \"postgres\" and a container "
            + "\"api\" on image \"nginx\" that WaitFor(db). Then subscribe three hooks, "
            + "each recording ONE entry through Record(...): app-scoped on "
            + "BeforeStartEvent, recording \"before-start:publish\" or "
            + "\"before-start:run\" according to the DistributedApplicationExecutionContext "
            + "resolved from the event's own Services; resource-scoped on \"db\" for "
            + "BeforeResourceStartedEvent, recording \"starting:\" plus the event's "
            + "resource name; resource-scoped on \"db\" for ResourceReadyEvent, recording "
            + "\"ready:\" plus the event's resource name. Record nothing at configure "
            + "time.");
}
