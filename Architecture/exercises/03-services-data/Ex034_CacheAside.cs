using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex034;

// Exercise 034 — CacheAside (services-data).
// Goal:   Read through a cache that only asks the source when it has to, and that lets
//         go of a value when it gets too old.
// Drills: hit and miss, loader invocation count, TTL, explicit invalidation.
// Passes: two reads of one key   - the loader runs ONCE and both reads return its value.
//         a second key           - the loader runs again; entries are per key.
//         within the TTL         - the loader does not run again.
//         past the TTL           - it does, and the fresh value is returned.
//         Invalidate(key)        - the next read reloads.
//         Invalidate(unknown)    - harmless.
//
// The loader's invocation COUNT is the only thing that separates a cache from no cache
// at all. Asserting that GetOrLoad "returns the right value" is satisfied perfectly by
// an implementation that calls the loader every single time - which is to say, by
// deleting the cache. Anything that grades a cache has to count.
//
// Time comes from IClock, so the TTL facts advance a ManualClock instead of sleeping.
public sealed class CacheAside<TKey, TValue>(IClock clock, TimeSpan timeToLive)
    where TKey : notnull
{
    /// <summary>
    /// Return the cached value for <paramref name="key"/>, calling
    /// <paramref name="loader"/> only when there is no live entry for it.
    /// </summary>
    public TValue GetOrLoad(TKey key, Func<TKey, TValue> loader) =>
        throw new NotImplementedException(
            "TODO: Ex034 - return a live cached entry, otherwise load, store it with the current time, and return it");

    /// <summary>Drop the entry for <paramref name="key"/>, if there is one.</summary>
    public void Invalidate(TKey key) =>
        throw new NotImplementedException("TODO: Ex034 - forget this key");
}
