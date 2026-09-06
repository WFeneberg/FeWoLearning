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

// Exercise 071 — ReadReplicaRouting (scale).
// Goal:   Serve reads from a replica without ever showing a user data older than what
//         they themselves just wrote.
// Drills: read/write splitting, replication lag, read-your-writes, per-session stickiness.
// Passes: default    - a read from a session that has not written goes to the REPLICA.
//                      That is the point of having one.
//         stickiness - a read from a session that just wrote goes to the PRIMARY, for
//                      readYourWritesWindow after the write.
//         expiry     - once the window has passed, that session's reads go back to the
//                      replica.
//         THE ONE     - the window is PER SESSION. Another session's read still goes to
//                      the replica while one session is sticky. A global "somebody wrote
//                      recently, everybody reads primary" is correct, trivial, and
//                      deletes the entire benefit of having replicas on any system with
//                      more than one active user.
//         lag        - the lag is real: reading the replica immediately after a write
//                      returns the OLD value.
//
// Eventual consistency is acceptable everywhere except where a user is looking at the
// result of their own action. They will not accept it there, they will press the button
// again, and the duplicate they create is a real one.
//
// The window is a bet on the lag, and it is the honest form of the trade: too short and
// somebody sees their own edit vanish, too long and the primary serves the traffic the
// replicas were bought for. It has to be a number somebody chose, not an accident.
public sealed class ReadRouter(IClock clock, ReplicatedStore store, TimeSpan readYourWritesWindow)
{
    /// <summary>Write on behalf of a session, and remember that this session wrote.</summary>
    public void Write(string sessionId, string key, string value) =>
        throw new NotImplementedException(
            "TODO: Ex071 - write to the store and record when THIS session last wrote");

    /// <summary>Read on behalf of a session, from wherever that session may safely read.</summary>
    public string? Read(string sessionId, string key) =>
        throw new NotImplementedException(
            "TODO: Ex071 - read from the primary while this session is inside its window, otherwise from the replica");
}
