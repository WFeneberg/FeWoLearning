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

// Exercise 067 — DistributedScheduling (reference solution).
public sealed class DistributedScheduler(IClock clock, RunLog log, TimeSpan interval)
{
    public DateTimeOffset OccurrenceFor(DateTimeOffset moment)
    {
        // A pure function of the clock. "Has it been an hour since the last run" needs
        // shared state every instance updates and races on; "which hour is it" needs
        // nothing, and two instances a few milliseconds apart compute the same answer.
        var ticks = moment.UtcTicks / interval.Ticks;
        return new DateTimeOffset(ticks * interval.Ticks, TimeSpan.Zero);
    }

    public bool TryRun(string jobName, Action job)
    {
        // THIS occurrence, never a backlog of missed ones. An instance returning after a
        // two-hour outage would otherwise run twelve reports at once, against a system
        // that has only just come back.
        var occurrence = OccurrenceFor(clock.UtcNow);

        // Claimed BEFORE the job runs, which makes this at-most-once: a job that fails is
        // not picked up elsewhere, and somebody has to notice. Claiming afterwards would
        // be at-least-once and would let a second instance run it while the first is
        // merely slow. Neither is free - the point is that it is a decision.
        if (!log.TryClaim(jobName, occurrence))
            return false;

        job();
        return true;
    }
}
