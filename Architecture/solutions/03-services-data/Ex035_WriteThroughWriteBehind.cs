namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex035;

/// <summary>The slow, durable thing behind the cache. Counts its writes.</summary>
public sealed class BackingStore
{
    private readonly Dictionary<string, string> _values = [];

    public int Writes { get; private set; }

    public string? Read(string key) => _values.GetValueOrDefault(key);

    public void Write(string key, string value)
    {
        Writes++;
        _values[key] = value;
    }
}

// Exercise 035 — WriteThroughWriteBehind (reference solution).
public sealed class WriteThroughCache(BackingStore store)
{
    private readonly Dictionary<string, string> _cache = [];

    public void Write(string key, string value)
    {
        _cache[key] = value;
        // Immediately. The cost is a store write on every single call; the benefit is
        // that there is no window in which a crash loses the change.
        store.Write(key, value);
    }

    public string? Read(string key) => _cache.GetValueOrDefault(key) ?? store.Read(key);
}

public sealed class WriteBehindCache(BackingStore store)
{
    private readonly Dictionary<string, string> _cache = [];
    private readonly HashSet<string> _dirty = new(StringComparer.Ordinal);

    public void Write(string key, string value)
    {
        _cache[key] = value;

        // A SET of keys, not a list of writes. That is what makes three writes to one
        // key cost one store write at flush - and coalescing is the entire reason to
        // accept the loss window in the first place.
        _dirty.Add(key);
    }

    public string? Read(string key) => _cache.GetValueOrDefault(key) ?? store.Read(key);

    public void Flush()
    {
        foreach (var key in _dirty)
            store.Write(key, _cache[key]);

        _dirty.Clear();
    }
}
