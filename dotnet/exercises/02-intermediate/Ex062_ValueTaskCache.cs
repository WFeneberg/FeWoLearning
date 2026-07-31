namespace FeWoLearning.Exercises.Intermediate;

// Exercise 062 — ValueTaskCache (intermediate).
// Goal:   Implement a cached lookup that returns ValueTask<int>, completing
//         synchronously on a cache hit (no allocation, no awaiting) and
//         asynchronously on a cache miss (computes the value, caches it,
//         then returns it).
// Drills: ValueTask<T> vs Task<T>, synchronous vs asynchronous completion,
//         avoiding unnecessary allocations on the hot (cached) path.
public sealed class ValueTaskCache
{
    private readonly Dictionary<int, int> _cache = new();
    private readonly Func<int, int> _compute;

    public ValueTaskCache(Func<int, int> compute) => throw new NotImplementedException();

    // Returns the cached value synchronously if present; otherwise computes
    // it asynchronously, caches it, and returns it.
    public ValueTask<int> GetAsync(int key) => throw new NotImplementedException();
}
