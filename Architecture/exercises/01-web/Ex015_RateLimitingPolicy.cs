using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Web.Ex015;

// Exercise 015 — RateLimitingPolicy (web).
// Goal:   Build a token bucket that refills gradually, holds a separate budget per
//         client, and never accumulates more than it is allowed to.
// Drills: token bucket vs fixed window, per-client partitioning, virtual clock.
// Passes: burst      - the first `capacity` calls for a client succeed and the next fails.
//         partition  - a second client is unaffected by the first exhausting its budget.
//         refill     - advancing the clock by exactly one refill interval grants exactly
//                      ONE token: the next call succeeds and the one after it fails.
//         no hoarding- advancing by a very long time still leaves at most `capacity`
//                      tokens, so an idle client cannot save up a flood.
//
// The one-token refill is what separates a token bucket from a fixed window. A fixed
// window with the same numbers restores the WHOLE budget the moment the window rolls
// over, which is why it lets a client fire 2 x capacity requests across a window
// boundary - the burst it was installed to prevent.
//
// Time comes from IClock. Nothing here sleeps, and nothing here reads DateTime.UtcNow.
public sealed class TokenBucketLimiter(IClock clock, int capacity, TimeSpan refillInterval)
{
    /// <summary>
    /// Take one token for <paramref name="clientId"/> if the bucket has one, refilling
    /// first based on how much time has passed since that client was last seen.
    /// A client seen for the first time starts with a full bucket.
    /// </summary>
    public bool TryAcquire(string clientId) =>
        throw new NotImplementedException(
            "TODO: Ex015 - refill this client's bucket by elapsed time (capped at capacity), then take one token if there is one");
}
