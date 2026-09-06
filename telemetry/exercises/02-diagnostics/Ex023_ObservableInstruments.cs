using System.Diagnostics.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 023 — ObservableInstruments (diagnostics).
// Goal:   Tell a value you PUSH apart from a value someone PULLS, and a number that
//         only rises apart from one that goes both ways.
// Drills: ObservableGauge, the pull model, UpDownCounter vs Counter.
// Passes: registering the gauge calls nothing - the callback runs only when a listener
//                     polls, once per poll;
//         each poll reports whatever the source says AT THAT MOMENT, so a changed
//                     backing value shows up on the next poll;
//         granting a lease adds 1 to both "leases.granted" and "leases.active";
//         releasing one subtracts 1 from "leases.active" and touches
//                     "leases.granted" not at all.
//
// The first clause is what "observable" means and it is routinely misread as "reports
// automatically". Nothing is automatic: the callback is invoked by whoever is
// collecting, on their schedule, and if nobody collects it never runs at all. That is
// the design - a queue depth has a value at every instant, and sampling it when asked
// is far cheaper than emitting an event every time it changes.
//
// It is also the trap: that callback runs on the collector's thread, on its cadence,
// and anything slow or throwing in there degrades or breaks the whole collection cycle
// for every instrument. Read a field. Do not query a database.
//
// The last two clauses are the distinction that breaks dashboards silently. A Counter
// promises to be MONOTONIC, and backends rely on it: they compute rates from the
// difference between consecutive readings, so a value that goes down is not read as
// "minus one", it is read as a process restart and the whole delta is discarded.
// Something that goes up and down - active leases, queue length, open connections -
// is an UpDownCounter. "Granted ever" and "active now" are two different numbers, and
// wanting both is normal.
public static class Ex023_ObservableInstruments
{
    /// <summary>The name this exercise's meter is registered under.</summary>
    public const string MeterName = "fewolearning.telemetry.ex023";

    /// <summary>Pulled on demand: how deep the queue is right now.</summary>
    public const string QueueDepthGauge = "queue.depth";

    /// <summary>Monotonic: how many leases have ever been granted.</summary>
    public const string LeasesGrantedCounter = "leases.granted";

    /// <summary>Bidirectional: how many leases are held right now.</summary>
    public const string LeasesActiveUpDown = "leases.active";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Point <see cref="QueueDepthGauge"/> at <paramref name="readDepth"/>, creating
    /// the <see cref="ObservableGauge{T}"/> of <see cref="int"/> if it does not exist
    /// yet.
    ///
    /// Registering must not call <paramref name="readDepth"/>. Only a collector may.
    ///
    /// Create the gauge AT MOST ONCE for the life of the process, and let later calls
    /// merely swap the source. An instrument cannot be removed from a Meter once it is
    /// published, so registering a second gauge under the same name leaves both alive
    /// and every collection then reports the value twice - which reads as a doubled
    /// queue rather than as a bug.
    /// </summary>
    public static void RegisterQueueDepth(Func<int> readDepth) =>
        throw new NotImplementedException(
            "TODO: Ex023 - register an observable gauge that reads the depth when polled, and not before");

    /// <summary>
    /// A lease was granted: add 1 to <see cref="LeasesGrantedCounter"/> and 1 to
    /// <see cref="LeasesActiveUpDown"/>.
    /// </summary>
    public static void GrantLease() =>
        throw new NotImplementedException("TODO: Ex023 - record a granted lease on both instruments");

    /// <summary>
    /// A lease was released: subtract 1 from <see cref="LeasesActiveUpDown"/>, and
    /// leave <see cref="LeasesGrantedCounter"/> alone - it counts what has happened,
    /// which cannot un-happen.
    /// </summary>
    public static void ReleaseLease() =>
        throw new NotImplementedException("TODO: Ex023 - record a released lease on the up-down counter only");
}
