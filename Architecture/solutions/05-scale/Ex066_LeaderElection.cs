using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex066;

/// <summary>
/// The one shared thing every candidate can see. An interface, so the same election runs
/// against an in-memory store and against Redis.
/// </summary>
public interface ILeaseStore
{
    /// <summary>
    /// Atomically take the lease if it is free or expired, or extend it if this node
    /// already holds it. Returns whether the node holds it afterwards.
    /// </summary>
    bool TryAcquireOrRenew(string resource, string nodeId, DateTimeOffset now, TimeSpan duration);

    void Release(string resource, string nodeId);

    /// <summary>Who holds it right now, or null if nobody does.</summary>
    string? HolderOf(string resource, DateTimeOffset now);
}

// Exercise 066 — LeaderElection (scale).
// Goal:   Pick one instance out of several to do a thing that must only happen once, and
//         have it notice when it stops being that instance.
// Drills: lease-based leadership, renewal, losing leadership without being told.
// Passes: election   - the first node to heartbeat becomes leader; a second node does not.
//         renewal    - the leader heartbeating inside the lease keeps it.
//         takeover   - once the lease expires without renewal, another node takes it.
//         THE ONE     - IsLeader goes FALSE on its own when the lease expires, WITHOUT
//                      the node heartbeating and being told. A leader that only learns it
//                      has been replaced the next time it asks keeps acting as leader in
//                      the meantime - and "the meantime" is exactly when the network
//                      partition that caused this is happening.
//         resignation- Resign frees the lease immediately for somebody else.
//
// Leadership is not a fact about the world, it is a claim with an expiry date, and the
// only honest reading of it is against the clock. Everything else - a flag set when the
// heartbeat succeeded, a bool the store handed back - describes the past.
//
// The safe pattern that follows: check IsLeader immediately before each unit of work,
// keep the lease long relative to the work, and pair it with a fencing token (exercise
// 037) for anything the resource itself can reject. A leader that is wrong for two
// seconds is only harmless if two seconds of its writes are harmless.
public sealed class InMemoryLeaseStore : ILeaseStore
{
    private readonly Dictionary<string, (string Holder, DateTimeOffset ExpiresAt)> _leases = [];

    public bool TryAcquireOrRenew(string resource, string nodeId, DateTimeOffset now, TimeSpan duration)
    {
        // Free, expired, or already mine - all three are grants. Refusing to renew my own
        // live lease would make every heartbeat a re-election, and leadership would flap
        // between nodes for no reason at all.
        if (_leases.TryGetValue(resource, out var lease) && lease.ExpiresAt > now && lease.Holder != nodeId)
            return false;

        _leases[resource] = (nodeId, now + duration);
        return true;
    }

    public void Release(string resource, string nodeId)
    {
        if (_leases.TryGetValue(resource, out var lease) && lease.Holder == nodeId)
            _leases.Remove(resource);
    }

    public string? HolderOf(string resource, DateTimeOffset now) =>
        _leases.TryGetValue(resource, out var lease) && lease.ExpiresAt > now ? lease.Holder : null;
}

public sealed class LeaderElection(IClock clock, ILeaseStore store, string resource, string nodeId, TimeSpan leaseDuration)
{
    // Read from the store against the CLOCK, every time. A bool set when the last
    // heartbeat succeeded describes the past, and a leader that only learns it has been
    // replaced the next time it asks keeps acting as leader in the meantime - which is
    // exactly when the partition that caused this is happening.
    public bool IsLeader => store.HolderOf(resource, clock.UtcNow) == nodeId;

    public bool Heartbeat() =>
        store.TryAcquireOrRenew(resource, nodeId, clock.UtcNow, leaseDuration);

    public void Resign() => store.Release(resource, nodeId);
}
