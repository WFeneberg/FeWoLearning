namespace FeWoLearning.Architecture.Exercises.Runtime.Ex094;

/// <summary>A cache level. L1 is in-process; L2 is shared, slower, and survives a restart.</summary>
public interface ICacheLevel
{
    string Name { get; }

    int Reads { get; }

    int Writes { get; }

    bool TryGet(string key, out string value);

    void Set(string key, string value);

    void Remove(string key);
}

public sealed class MemoryLevel(string name) : ICacheLevel
{
    private readonly Dictionary<string, string> _entries = [];

    public string Name => name;

    public int Reads { get; private set; }

    public int Writes { get; private set; }

    public bool TryGet(string key, out string value)
    {
        Reads++;
        return _entries.TryGetValue(key, out value!);
    }

    public void Set(string key, string value)
    {
        Writes++;
        _entries[key] = value;
    }

    public void Remove(string key) => _entries.Remove(key);
}

// Exercise 094 — MultiLevelCache (reference solution).
public sealed class TwoLevelCache(ICacheLevel l1, ICacheLevel l2)
{
    public string GetOrLoad(string key, Func<string, string> loader)
    {
        // L1 first, and RETURN. Reading L2 anyway - to compare, to refresh, to be safe -
        // means a local hit still costs a network round trip, and L1 is buying nothing at
        // all.
        if (l1.TryGet(key, out var local))
            return local;

        if (l2.TryGet(key, out var shared))
        {
            // Promotion. Without it a hot key is fetched from L2 on every single request
            // and the local cache holds only the keys nobody wants.
            l1.Set(key, shared);
            return shared;
        }

        var value = loader(key);

        // Both levels, one load. Writing only L2 means the instance that just paid for the
        // load goes back to the network for its own value.
        l2.Set(key, value);
        l1.Set(key, value);
        return value;
    }

    public void Invalidate(string key)
    {
        // BOTH. Clearing only the shared level leaves every instance serving its own stale
        // copy from L1 for as long as that entry lives - and looks completely correct from
        // the instance that ran the invalidation, which is the one somebody is watching.
        l1.Remove(key);
        l2.Remove(key);
    }
}
