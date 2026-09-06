using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex037;

/// <summary>
/// A lease, not a lock. It expires on its own, because the holder may have died and
/// nobody is coming to release it.
/// </summary>
public sealed record Lease(string Resource, string Owner, long FencingToken, DateTimeOffset ExpiresAt);

// Exercise 037 — DistributedLock (reference solution).
public sealed class LeaseManager(IClock clock)
{
    private readonly Dictionary<string, Lease> _leases = [];
    private long _nextToken;

    public Lease? TryAcquire(string resource, string owner, TimeSpan duration)
    {
        var now = clock.UtcNow;

        // "Held AND still live". Checking only for presence turns a crashed holder into
        // a permanently unavailable resource, which is the failure a lease exists to
        // avoid.
        if (_leases.TryGetValue(resource, out var existing) && existing.ExpiresAt > now)
            return null;

        // Monotonic across the whole manager, and never reset when a lease expires. A
        // token that restarts at 1 lets a stale writer look current again.
        var lease = new Lease(resource, owner, Interlocked.Increment(ref _nextToken), now + duration);
        _leases[resource] = lease;
        return lease;
    }

    public bool Release(string resource, string owner)
    {
        if (!_leases.TryGetValue(resource, out var existing) || existing.Owner != owner)
            return false;

        _leases.Remove(resource);
        return true;
    }
}

public sealed class FencedResource
{
    public string? Value { get; private set; }

    public long HighestAcceptedToken { get; private set; }

    public bool TryWrite(long fencingToken, string value)
    {
        // The resource, not the lock, is what stops the stale writer. A paused holder -
        // GC, stalled disk, VM migration - wakes up believing it still owns a lease that
        // expired minutes ago, and nothing can tell it otherwise. This can.
        if (fencingToken < HighestAcceptedToken)
            return false;

        HighestAcceptedToken = fencingToken;
        Value = value;
        return true;
    }
}
