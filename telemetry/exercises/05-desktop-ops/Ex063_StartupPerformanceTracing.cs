using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 063 — StartupPerformanceTracing (desktop-ops).
// Goal:   Answer "why does it take eleven seconds to open" with a number per phase rather
//         than a shrug.
// Drills: one span per startup phase, nested under one root, failure that stays a failure.
// Passes: startup produces one root span with one child per phase, named for it, in order;
//         the phases are SIBLINGS under the root, not a chain;
//         the root outlives all of them - it stops last;
//         a phase that throws is recorded with Error status and startup still fails;
//         and no phase after the failing one runs.
//
// Startup is the one part of a desktop application every user experiences and nobody
// profiles, because it happens before anything is running that could measure it. It is
// also the part that decays most quietly: nobody adds eleven seconds, forty people add a
// quarter of a second each over three years.
//
// One span per phase is what turns that into an argument you can win. "Startup is slow" is
// a complaint; "service registration is 4.2 seconds of the 6.1, and it was 0.9 last
// release" is a bug report with a suspect.
//
// The fourth and fifth clauses are row 018's lesson at the least forgiving moment. A
// startup that swallows a failed phase produces an application that opens into a broken
// state - and the telemetry, having recorded a green root span, agrees that everything is
// fine.
public static class Ex063_StartupPerformanceTracing
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex063";

    /// <summary>The root span covering the whole of startup.</summary>
    public const string StartupSpanName = "app.startup";

    /// <summary>The attribute naming which phase a child span covered.</summary>
    public const string PhaseTag = "startup.phase";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex063 - build a provider recording this source");

    /// <summary>
    /// Run <paramref name="phases"/> in order inside one <see cref="StartupSpanName"/>
    /// span.
    ///
    /// Each phase gets its own child span named after the phase and tagged
    /// <see cref="PhaseTag"/> with it. A phase that throws records the failure on ITS span
    /// and on the root, and the exception continues on its way - so nothing after it runs
    /// and the caller finds out.
    /// </summary>
    public static Task RunStartupAsync(IReadOnlyList<(string Name, Func<Task> Work)> phases) =>
        throw new NotImplementedException(
            "TODO: Ex063 - time each phase inside one startup span, and let a failure stay one");
}
