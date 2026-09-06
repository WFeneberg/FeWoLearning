using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give a service two probes that answer two different questions, and get the
///         difference right - because getting it wrong is the single most common way to
///         turn a slow start into a restart loop.
/// Drills: `AddHealthChecks().AddCheck(name, check, tags:)` and tag-filtered endpoints.
///         `/alive` answers "is this process still a process" and must only run the
///         checks tagged "live". `/health` answers "can this instance serve traffic yet"
///         and runs EVERY check, liveness ones included. Liveness failing means kill me;
///         readiness failing means take me out of rotation and wait.
/// Passes: Three scenarios, and the row needs all three because no two of them together
///         pin the answer down:
///         during startup      /alive 200, /health 503
///         once warm           /alive 200, /health 200
///         warm but stalled    /alive 503, /health 503
/// Note:   Scenario one is the bug the row exists to drill - a readiness check that also
///         answers the liveness probe. Map `/alive` with no predicate and it goes 503
///         while the database connection is still warming up, and the orchestrator kills
///         a container that was doing nothing wrong.
///
///         Scenario three is what stops the filter being faked. Measured, all three of
///         these mutants pass scenarios one and two and fail only this one: an `/alive`
///         filtered by NAME (Predicate = r =&gt; r.Name == "self") rather than by tag,
///         which silently drops "event-loop"; an `/alive` that is really a
///         MapGet("/alive", () =&gt; Results.Ok()); and a `/health` narrowed to the "ready"
///         tag, which stops reporting a genuinely dead process. Two endpoints existing
///         is not the exercise - which checks each one runs is.
/// </summary>
public static class Ex023_LivenessVersusReadiness
{
    // ---------------------------------------------------------------------------
    // GIVEN - the two facts about the process that the checks below have to read. The
    // test flips them to move between the three scenarios; a real service would set
    // them from a startup task and a watchdog.
    // ---------------------------------------------------------------------------

    /// <summary>False while the service is still warming up its dependencies.</summary>
    public static bool WarmupComplete { get; set; }

    /// <summary>True when the process itself is wedged and only a restart will help.</summary>
    public static bool EventLoopStalled { get; set; }

    // ---------------------------------------------------------------------------
    // TODO
    // ---------------------------------------------------------------------------

    /// <summary>
    /// TODO: ex023 - register exactly three health checks:
    ///   "self"       tagged "live",  always healthy;
    ///   "event-loop" tagged "live",  unhealthy when EventLoopStalled is true;
    ///   "catalog-db" tagged "ready", unhealthy until WarmupComplete is true.
    /// The tags are the mechanism, not decoration - MapProbes below filters on them.
    /// </summary>
    public static void ConfigureProbes(IHostApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex023 - AddHealthChecks() with \"self\" and \"event-loop\" tagged "
            + "\"live\" and \"catalog-db\" tagged \"ready\"; \"event-loop\" follows "
            + "EventLoopStalled and \"catalog-db\" follows WarmupComplete.");

    /// <summary>
    /// TODO: ex023 - map the two probes. "/alive" runs ONLY the checks tagged "live";
    /// "/health" runs them all. Filter by tag, not by name.
    /// </summary>
    public static void MapProbes(IEndpointRouteBuilder endpoints)
        => throw new NotImplementedException(
            "TODO: ex023 - MapHealthChecks(\"/health\") for every check, and "
            + "MapHealthChecks(\"/alive\", ...) restricted to the \"live\" tag.");
}
