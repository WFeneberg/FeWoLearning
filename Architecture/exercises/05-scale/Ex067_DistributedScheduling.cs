using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex067;

/// <summary>
/// Records which OCCURRENCE of a job has been claimed. Shared between instances - in
/// production a row with a unique constraint, or a Redis SET NX.
/// </summary>
public sealed class RunLog
{
    private readonly HashSet<(string Job, DateTimeOffset Occurrence)> _claimed = [];

    /// <summary>Atomically claim this occurrence. Returns false if somebody already has it.</summary>
    public bool TryClaim(string job, DateTimeOffset occurrence) => _claimed.Add((job, occurrence));

    public int ClaimCount => _claimed.Count;
}

// Exercise 067 — DistributedScheduling (scale).
// Goal:   Run a scheduled job once per interval across a fleet, when every instance is
//         running the same timer.
// Drills: occurrence identity, claiming, missed intervals, at-most-once semantics.
// Passes: occurrence - OccurrenceFor floors the current time to the interval, so every
//                      instance names the same occurrence without agreeing on anything.
//         once       - within one interval, the first instance runs the job and the rest
//                      do not, and the job is invoked exactly once.
//         next       - once the clock crosses into the next interval, it runs again -
//                      once.
//         THE ONE     - after a gap of several intervals, it runs ONCE for the current
//                      occurrence, not once per interval that was missed.
//         failure    - a job that throws has still claimed its occurrence, so no other
//                      instance runs it too.
//
// Flooring the clock is what makes this work without coordination. "Has it been an hour
// since the last run" needs shared state that every instance updates and races on; "which
// hour is it" is a pure function of the clock, and two instances a few milliseconds apart
// compute the same answer. The claim then only has to be atomic, which any store can do.
//
// The catch-up clause is the one that costs a night's sleep. An instance that comes back
// after a two-hour outage and finds twelve unclaimed occurrences will, given the chance,
// run twelve reports at once - against a system that has just come back and is already
// struggling. The occurrence being asked for is THIS one.
//
// The failure clause is a deliberate choice, not an oversight: claiming BEFORE the job
// runs makes this at-most-once. A job that fails does not get picked up elsewhere, and
// somebody has to notice. Claiming after would make it at-least-once and let two
// instances run it when the first is merely slow. Neither is free; the choice has to be
// made, and this exercise makes it explicitly.
public sealed class DistributedScheduler(IClock clock, RunLog log, TimeSpan interval)
{
    /// <summary>The occurrence the current time belongs to: the clock, floored to the interval.</summary>
    public DateTimeOffset OccurrenceFor(DateTimeOffset moment) =>
        throw new NotImplementedException(
            "TODO: Ex067 - floor the moment to a whole number of intervals since the Unix epoch");

    /// <summary>Run <paramref name="job"/> if this instance wins the claim. Returns whether it ran.</summary>
    public bool TryRun(string jobName, Action job) =>
        throw new NotImplementedException(
            "TODO: Ex067 - claim the CURRENT occurrence and run the job only if the claim was won");
}
