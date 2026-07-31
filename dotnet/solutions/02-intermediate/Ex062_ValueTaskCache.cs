namespace FeWoLearning.Exercises.Intermediate;

// Exercise 062 — ValueTaskCache (reference solution).
public sealed class ValueTaskCache
{
    private readonly Dictionary<int, int> _cache = new();
    private readonly Func<int, int> _compute;

    public ValueTaskCache(Func<int, int> compute) => _compute = compute;

    public ValueTask<int> GetAsync(int key)
    {
        // Cache hit: wrap the already-known value directly in a ValueTask
        // so the caller can complete synchronously, without allocating a
        // Task or awaiting anything.
        if (_cache.TryGetValue(key, out var cached))
        {
            return new ValueTask<int>(cached);
        }

        // Cache miss: fall back to the asynchronous path.
        return new ValueTask<int>(ComputeAndCacheAsync(key));
    }

    private async Task<int> ComputeAndCacheAsync(int key)
    {
        // Task.Yield guarantees this method never completes synchronously,
        // which is what makes the miss path observably asynchronous.
        await Task.Yield();

        var result = _compute(key);
        _cache[key] = result;
        return result;
    }
}
