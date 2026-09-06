using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Runtime.Ex097;

public sealed class BudgetExhaustedException(string step)
    : Exception($"No time left to start '{step}'.")
{
    public string Step { get; } = step;
}

// Exercise 097 — TimeoutBudget (runtime).
// Goal:   Give a whole request one deadline, and have every call below it inherit what is
//         LEFT rather than starting its own clock.
// Drills: deadline propagation, remaining time, refusing work that cannot finish.
// Passes: remaining - Remaining shrinks as the clock moves, and never goes below zero.
//         THE ONE    - a nested step is given the time REMAINING, not a fresh copy of the
//                      original timeout. Three sequential steps with "5 seconds each"
//                      under a 5-second request budget can take fifteen, and the caller
//                      gave up after five.
//         refusing  - a step whose own minimum exceeds what is left is refused BEFORE it
//                      starts, naming itself.
//         nesting   - a child budget can be shorter than the parent's remaining time, but
//                      never longer. A sub-call is not entitled to more patience than the
//                      caller has.
//         expiry    - once the budget is gone, IsExpired is true and nothing else may start.
//
// A deadline that does not propagate is the reason a request that was supposed to take a
// second takes forty. Every layer times out correctly - the HTTP client at five seconds,
// the database at thirty, the retry three times - and each of them is measuring from when
// IT started. The numbers are all defensible and the total is nobody's number.
//
// Propagating the REMAINING time is what makes the total bounded, and it is why a deadline
// is better than a timeout: a timeout is a duration each layer re-applies, while a deadline
// is an instant everybody shares. The refusal is the other half - starting work that cannot
// finish spends the caller's last two seconds computing a result that will be thrown away.
public sealed class RequestBudget
{
    private RequestBudget(IClock clock, DateTimeOffset deadline) =>
        throw new NotImplementedException("TODO: Ex097 - hold the clock and the shared deadline");

    public static RequestBudget StartingNow(IClock clock, TimeSpan total) =>
        throw new NotImplementedException("TODO: Ex097 - a budget expiring `total` from now");

    public TimeSpan Remaining =>
        throw new NotImplementedException("TODO: Ex097 - how long is left, never negative");

    public bool IsExpired =>
        throw new NotImplementedException("TODO: Ex097 - is there any time left at all");

    /// <summary>
    /// A budget for a nested call: <paramref name="wanted"/>, or what is left, whichever is
    /// shorter.
    /// </summary>
    public RequestBudget Nest(TimeSpan wanted) =>
        throw new NotImplementedException(
            "TODO: Ex097 - a child budget capped by this one's remaining time");

    /// <summary>Refuse a step that needs more time than is left.</summary>
    public void EnsureRoomFor(string step, TimeSpan needed) =>
        throw new NotImplementedException(
            "TODO: Ex097 - throw BudgetExhaustedException naming the step when it cannot finish in the time remaining");
}
