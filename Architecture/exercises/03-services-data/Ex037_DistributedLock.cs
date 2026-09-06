using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex037;

/// <summary>
/// A lease, not a lock. It expires on its own, because the holder may have died and
/// nobody is coming to release it.
/// </summary>
public sealed record Lease(string Resource, string Owner, long FencingToken, DateTimeOffset ExpiresAt);

// Exercise 037 — DistributedLock (services-data).
// Goal:   Build a lease-based mutual exclusion that survives the holder dying, and then
//         see why that alone is not enough.
// Drills: lease acquisition, expiry, fencing tokens.
// Passes: acquire     - a free resource is granted; a second owner is refused.
//         expiry      - once the lease has run out (ManualClock), a second owner IS
//                       granted it.
//         release     - the owner may hand it back early; somebody else may not.
//         tokens      - every grant carries a strictly larger fencing token than the last.
//         THE ONE      - holder A's lease expires, B takes it and writes; A then writes
//                       with its OLD token and is REJECTED, and the value is still B's.
//
// The last one is the whole exercise. A lease with expiry and no fencing token passes
// everything above it and still corrupts data, because the thing a lease cannot do is
// stop the previous holder: A was paused - a GC pause, a stalled disk, a VM migration -
// and wakes up believing it still holds a lock that expired minutes ago. Nothing has
// told it otherwise, and nothing can. The resource has to be the one to notice, by
// refusing any write whose token is older than the last one it accepted.
public sealed class LeaseManager(IClock clock)
{
    /// <summary>
    /// Grant <paramref name="resource"/> to <paramref name="owner"/> for
    /// <paramref name="duration"/>, or return null if somebody else holds a live lease.
    /// </summary>
    public Lease? TryAcquire(string resource, string owner, TimeSpan duration) =>
        throw new NotImplementedException(
            "TODO: Ex037 - grant the lease when free or expired, with a fencing token larger than every previous one");

    /// <summary>Hand a lease back early. Only the current owner may.</summary>
    public bool Release(string resource, string owner) =>
        throw new NotImplementedException("TODO: Ex037 - release only if this owner currently holds it");
}

/// <summary>
/// The protected resource. It is what makes fencing work: it remembers the highest token
/// it has ever accepted and refuses anything older.
/// </summary>
public sealed class FencedResource
{
    public string? Value { get; private set; }

    public long HighestAcceptedToken { get; private set; }

    public bool TryWrite(long fencingToken, string value) =>
        throw new NotImplementedException(
            "TODO: Ex037 - accept the write only if this token is at least the highest one seen so far");
}
