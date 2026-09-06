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

// Exercise 053 — ResiliencePipeline (reference solution).
public static class Ex053_ResiliencePipeline
{
    public static bool RetryOutsideTimeout(TimedWork work, int maxAttempts, TimeSpan perAttemptTimeout)
    {
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var succeeded = work.Run();

            // The budget applies to THIS attempt only, and is checked after it: an
            // attempt that overran has timed out, which is a failure like any other and
            // is therefore retried. Total elapsed time is unbounded by design -
            // maxAttempts times the per-attempt budget.
            if (succeeded && work.Cost <= perAttemptTimeout)
                return true;
        }

        return false;
    }

    public static bool TimeoutOutsideRetry(IClock clock, TimedWork work, int maxAttempts, TimeSpan totalTimeout)
    {
        var deadline = clock.UtcNow + totalTimeout;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            // Checked BEFORE starting. Starting an attempt that cannot finish in the time
            // left spends the caller's remaining patience on work whose result will be
            // discarded - which is the whole reason an outer timeout exists.
            if (clock.UtcNow + work.Cost > deadline)
                return false;

            if (work.Run())
                return true;
        }

        return false;
    }
}
