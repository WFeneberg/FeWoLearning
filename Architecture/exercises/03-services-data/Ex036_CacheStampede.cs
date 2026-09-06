namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex036;

// Exercise 036 — CacheStampede (services-data).
// Goal:   When many callers ask for the same cold key at once, load it once and give
//         everyone the same result.
// Drills: single-flight, concurrent loaders, per-key locking.
// Passes: sequential      - a second read of a warm key does not load again.
//         THE ONE          - N callers arriving together on a COLD key cause exactly ONE
//                            loader invocation, and all N receive its value.
//         per key          - two different cold keys load CONCURRENTLY. A single global
//                            lock passes the fact above and serialises the entire cache.
//         a failed load    - does not poison the key: the next caller may try again, and
//                            every waiter on the failed attempt sees the failure.
//
// This is the row where an ordinary cache-aside falls over. Under cache-aside, the
// moment a hot key expires every in-flight request misses simultaneously and every one
// of them calls the database - which is how an expiry becomes an outage. The fix is to
// cache the TASK rather than the value, so the second caller finds work already in
// progress and awaits it.
//
// The facts drive this with a rendezvous gate rather than a stopwatch: the loader blocks
// until the test releases it, so a wrong implementation fails by not finishing rather
// than by being slow on a loaded machine.
//
// A trap measured while building this exercise, worth knowing before you start:
// ConcurrentDictionary.GetOrAdd does NOT promise to run its factory only once. Under
// contention several threads may each build a candidate and only one is stored - so a
// factory that STARTS THE LOAD has just started two or three of them, which is the
// stampede arriving through the very mechanism meant to prevent it. Measured here: the
// eight-caller fact below failed on two runs out of three that way. Keep the factory
// cheap and make the load happen on the stored instance only.
public sealed class SingleFlightCache<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Return the cached value for <paramref name="key"/>. If a load for that key is
    /// already in progress, join it rather than starting a second one.
    /// </summary>
    public Task<TValue> GetOrLoadAsync(TKey key, Func<TKey, Task<TValue>> loader) =>
        throw new NotImplementedException(
            "TODO: Ex036 - cache the in-flight Task per key so concurrent callers share one load, and drop it again if it fails");
}
