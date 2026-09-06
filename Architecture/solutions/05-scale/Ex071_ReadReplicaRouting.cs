using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex071;

/// <summary>
/// A primary with one asynchronous replica. The replica serves whatever was written
/// longer ago than <paramref name="replicationLag"/> - which is what replication lag
/// actually is, rather than a thing that occasionally happens.
/// </summary>
public sealed class ReplicatedStore(IClock clock, TimeSpan replicationLag)
{
    private readonly List<(string Key, string Value, DateTimeOffset WrittenAt)> _writes = [];

    public int PrimaryReads { get; private set; }

    public int ReplicaReads { get; private set; }

    public void Write(string key, string value) => _writes.Add((key, value, clock.UtcNow));

    public string? ReadFromPrimary(string key)
    {
        PrimaryReads++;
        return _writes.LastOrDefault(w => w.Key == key).Value;
    }

    public string? ReadFromReplica(string key)
    {
        ReplicaReads++;
        var visibleUpTo = clock.UtcNow - replicationLag;
        return _writes.LastOrDefault(w => w.Key == key && w.WrittenAt <= visibleUpTo).Value;
    }
}

// Exercise 071 — ReadReplicaRouting (reference solution).
public sealed class ReadRouter(IClock clock, ReplicatedStore store, TimeSpan readYourWritesWindow)
{
    // Keyed by SESSION. One shared "last write" timestamp is correct, trivial, and
    // deletes the whole benefit of replicas on any system with more than one active
    // user: somebody writes every few seconds, and the primary serves everything.
    private readonly Dictionary<string, DateTimeOffset> _lastWriteBySession = [];

    public void Write(string sessionId, string key, string value)
    {
        store.Write(key, value);
        _lastWriteBySession[sessionId] = clock.UtcNow;
    }

    public string? Read(string sessionId, string key)
    {
        var sticky = _lastWriteBySession.TryGetValue(sessionId, out var lastWrite)
                     && clock.UtcNow - lastWrite < readYourWritesWindow;

        // Eventual consistency is acceptable everywhere except where a user is looking at
        // the result of their own action - they will not accept it there, they will press
        // the button again, and the duplicate is a real one.
        return sticky ? store.ReadFromPrimary(key) : store.ReadFromReplica(key);
    }
}
