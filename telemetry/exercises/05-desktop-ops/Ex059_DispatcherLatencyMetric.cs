using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Windows.Threading;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 059 — DispatcherLatencyMetric (desktop-ops).
// Goal:   Measure the thing a desktop user actually experiences as "the application is
//         slow", which is not how long any of your code takes.
// Drills: the Dispatcher queue, measuring WAIT rather than WORK, priority as a dimension.
// Passes: posting work records one measurement, in milliseconds, tagged with the priority
//                     it was posted at;
//         the work itself runs;
//         work posted behind something slow records a LARGE latency;
//         and work that is itself slow records a SMALL one.
//
// The last two clauses are one measurement seen from both sides, and together they are
// the row. Queue latency is how long a piece of work WAITED before the UI thread got to
// it - not how long it then took. Those are different numbers with different causes and
// different fixes: a long wait means the UI thread is busy with something else, a long
// run means this work is expensive. Conflating them produces a metric that goes up for
// two unrelated reasons and tells you nothing about either.
//
// It is also the number that matches the complaint. A user does not perceive "the click
// handler took 8ms"; they perceive the 400ms during which nothing happened because the
// thread was elsewhere. Every frame, every input event and every binding update is queued
// on this one thread, so its queue latency IS the application's responsiveness.
//
// Priority is a dimension rather than separate instruments, for row 021's reason: it is a
// small bounded set, and you want to ask "how long is the queue" across all of it and
// "which priority is starving" within it, from one series.
public static class Ex059_DispatcherLatencyMetric
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex059";

    /// <summary>How long work waited before the UI thread reached it.</summary>
    public const string LatencyHistogram = "ui.dispatcher.queue.latency";

    /// <summary>Milliseconds, in UCUM.</summary>
    public const string LatencyUnit = "ms";

    /// <summary>The dimension carrying which priority the work was posted at.</summary>
    public const string PriorityTag = "dispatcher.priority";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Post <paramref name="work"/> to <paramref name="dispatcher"/> at
    /// <paramref name="priority"/>, and record how long it WAITED in the queue - from
    /// this call until the moment it begins to run - on a
    /// <see cref="Histogram{T}"/> of <see cref="double"/> named
    /// <see cref="LatencyHistogram"/>, unit <see cref="LatencyUnit"/>, tagged
    /// <see cref="PriorityTag"/>.
    ///
    /// The measurement covers the wait and nothing else: whatever
    /// <paramref name="work"/> then costs is a different number.
    ///
    /// The returned task completes once the work has run.
    /// </summary>
    public static Task PostMeasuredAsync(Dispatcher dispatcher, DispatcherPriority priority, Action work) =>
        throw new NotImplementedException(
            "TODO: Ex059 - record how long the work waited, not how long it took");
}
