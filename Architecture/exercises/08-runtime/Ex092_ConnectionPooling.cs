using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Runtime.Ex092;

public sealed class PoolExhaustedException(int size, TimeSpan waited)
    : Exception($"All {size} connections are in use; waited {waited}.")
{
    public int Size { get; } = size;
}

/// <summary>A pooled thing. Knows when it was leased, so a leak has an age.</summary>
public sealed class PooledConnection(string id)
{
    public string Id => id;

    public DateTimeOffset? LeasedAt { get; internal set; }

    public int Uses { get; internal set; }
}

// Exercise 092 — ConnectionPooling (runtime).
// Goal:   Share a small number of expensive things among many callers, and notice when
//         one of them is never given back.
// Drills: leasing and returning, reuse, exhaustion as a fast failure, leak detection.
// Passes: reuse     - returning and leasing again hands back the SAME connection, and its
//                     use count climbs. A pool that hands out a new one every time is a
//                     factory with extra steps.
//         capacity  - only `size` connections exist, however many callers ask.
//         exhaustion- with all of them out, the next caller gets PoolExhaustedException
//                     rather than waiting for ever.
//         returning - a returned connection is available again immediately.
//         THE ONE    - FindLeaks reports every connection leased longer than the threshold,
//                      and reports nothing when everything was returned. A pool that only
//                      says "exhausted" tells you it is broken; it does not tell you who
//                      broke it, and the answer is always a code path that forgot a
//                      `using`.
//         idempotent- returning twice does not create capacity that does not exist.
//
// The whole point of a pool is that the thing is expensive to make and cheap to reuse - a
// TCP connection with a TLS handshake, a database session with a server-side context. That
// makes every leak permanent: the pool does not grow, so one code path that forgets to
// return is one connection fewer, for the lifetime of the process, and the symptom arrives
// hours later as a timeout somewhere unrelated.
//
// Returning twice is the mirror-image bug and is worse, because it does not look like one.
// A double return puts one connection in the pool twice, two callers lease the same
// connection, and their queries interleave on one session.
public sealed class ConnectionPool(IClock clock, int size)
{
    // An idle STACK plus a leased set, rather than one list with a flag. That is how a
    // real pool is built, and it is what makes a double return able to do damage: pushing
    // the same connection twice puts it in the pool twice, and the guard in Return is the
    // only thing standing between that and two callers sharing one session.
    private readonly Stack<PooledConnection> _idle =
        new(Enumerable.Range(1, size).Select(i => new PooledConnection($"conn-{i}")));

    private readonly HashSet<PooledConnection> _leased = [];

    public int Available =>
        throw new NotImplementedException("TODO: Ex092 - how many are not currently leased");

    public int InUse =>
        throw new NotImplementedException("TODO: Ex092 - how many are leased right now");

    public PooledConnection Lease() =>
        throw new NotImplementedException(
            "TODO: Ex092 - hand out a free connection, marking when it went out and counting the use, or throw PoolExhaustedException");

    public void Return(PooledConnection connection) =>
        throw new NotImplementedException(
            "TODO: Ex092 - mark it free again, and ignore a connection that is already free");

    /// <summary>Connections that have been out longer than <paramref name="threshold"/>.</summary>
    public IReadOnlyList<PooledConnection> FindLeaks(TimeSpan threshold) =>
        throw new NotImplementedException("TODO: Ex092 - the leased connections older than the threshold, oldest first");
}
