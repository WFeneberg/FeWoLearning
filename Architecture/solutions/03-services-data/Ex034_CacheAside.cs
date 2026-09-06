using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex034;

// Exercise 034 — CacheAside (reference solution).
public sealed class CacheAside<TKey, TValue>(IClock clock, TimeSpan timeToLive)
    where TKey : notnull
{
    private readonly Dictionary<TKey, (TValue Value, DateTimeOffset LoadedAt)> _entries = [];

    public TValue GetOrLoad(TKey key, Func<TKey, TValue> loader)
    {
        ArgumentNullException.ThrowIfNull(loader);

        var now = clock.UtcNow;

        // Strictly less than: an entry exactly at the TTL boundary is expired. Either
        // choice is defensible, but it has to be made once and asserted, or the boundary
        // behaves differently on every machine's timer resolution.
        if (_entries.TryGetValue(key, out var entry) && now - entry.LoadedAt < timeToLive)
            return entry.Value;

        var value = loader(key);
        _entries[key] = (value, now);
        return value;
    }

    public void Invalidate(TKey key) => _entries.Remove(key);
}
