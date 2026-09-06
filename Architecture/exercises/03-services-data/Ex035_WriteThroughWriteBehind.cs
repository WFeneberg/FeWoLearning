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

// Exercise 035 — WriteThroughWriteBehind (services-data).
// Goal:   Implement both write policies and make the difference between them
//         observable - because the difference is a window in which data can be lost.
// Drills: write-through vs write-behind, flush semantics, write coalescing.
// Passes: write-through - the store holds the new value IMMEDIATELY, and each write is
//                         one store write.
//         write-behind  - the store still holds the OLD value after the write; the cache
//                         already returns the new one; Flush() closes the gap.
//         coalescing    - three write-behind writes to ONE key cost exactly ONE store
//                         write at flush. That is the entire reason to accept the risk.
//         reads         - both policies read their own writes straight back.
//
// The middle fact is the one that must be asserted from the STORE and not from the
// cache. "Write-behind" that writes through immediately returns identical values from
// every read, passes any assertion made through the cache, and has quietly given up the
// only benefit the policy has - while still being described in the design document as
// deferred. The risk window and the coalescing are the same property seen from two
// sides; an implementation cannot have one without the other.
public sealed class WriteThroughCache(BackingStore store)
{
    public void Write(string key, string value) =>
        throw new NotImplementedException("TODO: Ex035 - update the cache AND the store, now");

    public string? Read(string key) =>
        throw new NotImplementedException("TODO: Ex035 - the cached value, falling back to the store");
}

public sealed class WriteBehindCache(BackingStore store)
{
    public void Write(string key, string value) =>
        throw new NotImplementedException("TODO: Ex035 - update the cache and remember the key as dirty; do NOT touch the store");

    public string? Read(string key) =>
        throw new NotImplementedException("TODO: Ex035 - the cached value, falling back to the store");

    /// <summary>Write every pending change to the store - one store write per KEY.</summary>
    public void Flush() =>
        throw new NotImplementedException("TODO: Ex035 - write each dirty key's current value once, then clear the dirty set");
}
