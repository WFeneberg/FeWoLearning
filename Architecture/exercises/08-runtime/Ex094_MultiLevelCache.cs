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

// Exercise 094 — MultiLevelCache (runtime).
// Goal:   Put a fast local cache in front of a shared one without the two of them
//         disagreeing.
// Drills: L1/L2 lookup order, promotion, write-through, coherence on invalidation.
// Passes: L1 hit    - answered from L1, and L2 IS NOT READ. If a local hit still costs a
//                     network round trip, L1 is buying nothing.
//         L2 hit    - answered from L2 and PROMOTED into L1, so the second read of the same
//                     key is local. A promotion that does not happen makes L1 useless for
//                     exactly the keys that are being used.
//         miss      - the loader runs once, and the value is written into BOTH levels.
//         THE ONE    - Invalidate clears BOTH levels. Clearing only L2 leaves every
//                      instance serving its own stale copy from L1, for as long as that
//                      entry lives - and the shared invalidation everybody trusted did
//                      nothing at all.
//         counting  - the loader runs once per miss, not once per level.
//
// The second level is what makes this different from exercise 034, and it introduces the
// only genuinely hard problem in caching: two copies of the same value in two places, one
// of which nobody else can reach. Every instance has its own L1, so "invalidate the cache"
// is a message that has to reach all of them - and an implementation that only clears the
// shared level looks correct from the instance that ran the invalidation.
//
// The promotion is the other half. Without it a hot key is fetched from L2 on every single
// request, and the local cache holds only the keys nobody wants.
public sealed class TwoLevelCache(ICacheLevel l1, ICacheLevel l2)
{
    public string GetOrLoad(string key, Func<string, string> loader) =>
        throw new NotImplementedException(
            "TODO: Ex094 - try L1 first and stop there on a hit; on an L2 hit promote into L1; on a miss load once and write both");

    public void Invalidate(string key) =>
        throw new NotImplementedException("TODO: Ex094 - remove the key from BOTH levels");
}
