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

// Exercise 092 — ConnectionPooling (reference solution).
public sealed class ConnectionPool(IClock clock, int size)
{
    private readonly Stack<PooledConnection> _idle =
        new(Enumerable.Range(1, size).Select(i => new PooledConnection($"conn-{i}")));

    private readonly HashSet<PooledConnection> _leased = [];

    public int Available => _idle.Count;

    public int InUse => _leased.Count;

    public PooledConnection Lease()
    {
        if (_idle.Count == 0)
            // Fails NOW rather than waiting. A caller blocked on an exhausted pool holds
            // its own request thread while it waits, so an exhausted pool becomes an
            // exhausted thread pool, and the outage spreads to endpoints that never
            // touched the database.
            throw new PoolExhaustedException(size, TimeSpan.Zero);

        var connection = _idle.Pop();
        connection.LeasedAt = clock.UtcNow;
        connection.Uses++;
        _leased.Add(connection);
        return connection;
    }

    public void Return(PooledConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        // The guard, and it is load-bearing. Without it a double return pushes the same
        // connection twice, two callers lease the same one, and their queries interleave
        // on a single session - which does not look like a pooling bug from anywhere
        // downstream.
        if (!_leased.Remove(connection))
            return;

        connection.LeasedAt = null;
        _idle.Push(connection);
    }

    public IReadOnlyList<PooledConnection> FindLeaks(TimeSpan threshold) =>
        // The pool does not grow, so every leak is permanent: one code path that forgets
        // to return is one connection fewer for the lifetime of the process, and the
        // symptom arrives hours later as a timeout somewhere unrelated. "Exhausted" says
        // it is broken; this says who broke it.
        [.. _leased.Where(c => c.LeasedAt is { } at && clock.UtcNow - at >= threshold)
                   .OrderBy(c => c.LeasedAt)];
}
