using System.Collections.Concurrent;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex036;

// Exercise 036 — CacheStampede (reference solution).
public sealed class SingleFlightCache<TKey, TValue>
    where TKey : notnull
{
    // Lazy<Task<TValue>>, not Task<TValue>. ConcurrentDictionary.GetOrAdd does NOT
    // promise to run its factory only once: under contention several threads may each
    // build a candidate, and only one of them is stored. If the factory itself started
    // the load, the losers would have started a second and third one - which is the
    // stampede, arriving through the very mechanism meant to prevent it.
    //
    // Measured on this machine: with a plain Task the eight-caller fact failed on two
    // runs out of three. Wrapping in a Lazy makes the factory cheap - it allocates and
    // nothing else - and the load happens exactly once, on the .Value of whichever
    // instance actually won.
    private readonly System.Collections.Concurrent.ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _inFlight = new();

    public Task<TValue> GetOrLoadAsync(TKey key, Func<TKey, Task<TValue>> loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        // Per key. A single lock around the whole cache also yields one load per key -
        // and serialises every unrelated key behind it, turning the cache into the
        // bottleneck it was installed to remove.
        var lazy = _inFlight.GetOrAdd(key, k => new Lazy<Task<TValue>>(
            () => LoadAndForgetOnFailure(k, loader),
            LazyThreadSafetyMode.ExecutionAndPublication));

        return lazy.Value;
    }

    private async Task<TValue> LoadAndForgetOnFailure(TKey key, Func<TKey, Task<TValue>> loader)
    {
        try
        {
            return await loader(key).ConfigureAwait(false);
        }
        catch
        {
            // A failed load must not be cached, or one transient error poisons the key
            // for the lifetime of the process. Every waiter still sees this exception -
            // they are all awaiting this same task.
            _inFlight.TryRemove(key, out _);
            throw;
        }
    }
}
