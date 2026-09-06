using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex053;

/// <summary>
/// A call that takes time. Every attempt consumes <paramref name="cost"/> of virtual
/// time, and it starts working on attempt <paramref name="succeedsOnAttempt"/>. Nothing
/// here sleeps.
/// </summary>
public sealed class TimedWork(Action<TimeSpan> advance, TimeSpan cost, int succeedsOnAttempt)
{
    public int Attempts { get; private set; }

    public TimeSpan Cost => cost;

    public bool Run()
    {
        Attempts++;
        advance(cost);
        return Attempts >= succeedsOnAttempt;
    }
}

// Exercise 053 — ResiliencePipeline (cross-cutting).
// Goal:   Compose retry and timeout both ways round, and see that the ORDER is not a
//         detail - it changes what the pipeline promises.
// Drills: strategy composition, ordering, per-attempt vs overall budgets.
// Passes: retry OUTSIDE timeout - each attempt gets its own budget. An attempt that
//                     overruns is a failure like any other and is retried, up to
//                     maxAttempts. Total time is unbounded: maxAttempts x the budget.
//         timeout OUTSIDE retry - ONE budget for everything. When it is gone, no further
//                     attempt starts, however much retry budget is left.
//         the point   - on the SAME work, the two orderings produce different attempt
//                     counts and different answers.
//         success first time - both orderings make exactly one attempt.
//
// This is the composition people get wrong by not realising there was a choice. "Retry
// three times with a five-second timeout" describes both of these, and they are not the
// same system: one can take fifteen seconds and usually succeeds, the other never
// exceeds five and often does not. Which one you want depends on whether a caller is
// waiting - and nobody can pick if the pipeline does not say which it is.
public static class Ex053_ResiliencePipeline
{
    /// <summary>
    /// Retry on the outside. Each attempt is given <paramref name="perAttemptTimeout"/>;
    /// an attempt that costs more than that has timed out and counts as a failure.
    /// Returns whether the work eventually succeeded.
    /// </summary>
    public static bool RetryOutsideTimeout(TimedWork work, int maxAttempts, TimeSpan perAttemptTimeout) =>
        throw new NotImplementedException(
            "TODO: Ex053 - attempt up to maxAttempts times, treating an attempt that costs more than its own budget as a failure");

    /// <summary>
    /// Timeout on the outside. <paramref name="totalTimeout"/> covers everything; do not
    /// start an attempt that cannot finish inside what is left.
    /// </summary>
    public static bool TimeoutOutsideRetry(IClock clock, TimedWork work, int maxAttempts, TimeSpan totalTimeout) =>
        throw new NotImplementedException(
            "TODO: Ex053 - attempt while both the retry budget and the remaining time allow it, using the clock to know how much is left");
}
