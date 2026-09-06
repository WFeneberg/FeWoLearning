using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Runtime.Ex097;

public sealed class BudgetExhaustedException(string step)
    : Exception($"No time left to start '{step}'.")
{
    public string Step { get; } = step;
}

// Exercise 097 — TimeoutBudget (reference solution).
public sealed class RequestBudget
{
    private readonly IClock _clock;
    private readonly DateTimeOffset _deadline;

    private RequestBudget(IClock clock, DateTimeOffset deadline) =>
        // A DEADLINE - an instant everybody shares - rather than a timeout, which is a
        // duration each layer re-applies from whenever it happened to start.
        (_clock, _deadline) = (clock, deadline);

    public static RequestBudget StartingNow(IClock clock, TimeSpan total) =>
        new(clock, clock.UtcNow + total);

    public TimeSpan Remaining
    {
        get
        {
            var left = _deadline - _clock.UtcNow;
            // Never negative: an overrun is "no time", not "minus four seconds", and a
            // negative TimeSpan handed to a socket timeout is an argument exception in the
            // one code path nobody tested.
            return left > TimeSpan.Zero ? left : TimeSpan.Zero;
        }
    }

    public bool IsExpired => Remaining == TimeSpan.Zero;

    public RequestBudget Nest(TimeSpan wanted) =>
        // Capped by what is left. A sub-call is not entitled to more patience than its
        // caller has - and three sequential steps each starting a fresh five seconds under a
        // five-second request budget take fifteen, long after the caller gave up.
        new(_clock, _clock.UtcNow + (wanted < Remaining ? wanted : Remaining));

    public void EnsureRoomFor(string step, TimeSpan needed)
    {
        // Refused BEFORE it starts. Beginning work that cannot finish spends the caller's
        // last two seconds computing a result that will be thrown away.
        if (needed > Remaining)
            throw new BudgetExhaustedException(step);
    }
}
