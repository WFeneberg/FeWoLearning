using System.Collections.Concurrent;

namespace FeWoLearning.Architecture.Exercises.Scale.Ex061;

public sealed class BulkheadRejectedException(string partition)
    : Exception($"Partition '{partition}' is at capacity.")
{
    public string Partition { get; } = partition;
}

// Exercise 061 — BulkheadIsolation (reference solution).
public sealed class Bulkhead(IReadOnlyDictionary<string, int> capacities)
{
    // One semaphore PER PARTITION. A single shared one limits total concurrency, which
    // is a useful thing and is not a bulkhead: a bulkhead is a guarantee about the OTHER
    // partitions, and a shared limit gives none.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _slots = new();

    public int InFlight(string partition) =>
        capacities.TryGetValue(partition, out var capacity) && _slots.TryGetValue(partition, out var gate)
            ? capacity - gate.CurrentCount
            : 0;

    public async Task<T> ExecuteAsync<T>(string partition, Func<Task<T>> work)
    {
        if (!capacities.TryGetValue(partition, out var capacity))
            return await work().ConfigureAwait(false); // unlimited

        var gate = _slots.GetOrAdd(partition, _ => new SemaphoreSlim(capacity, capacity));

        // Wait(0): take the slot or fail RIGHT NOW. Queueing here would turn the bulkhead
        // into a buffer, and a caller that is going to fail should fail cheaply - the
        // queue is where the latency the pattern exists to contain reappears.
        if (!gate.Wait(0))
            throw new BulkheadRejectedException(partition);

        try
        {
            return await work().ConfigureAwait(false);
        }
        finally
        {
            // finally, not after the await. A slot leaked on every failure means the
            // partition closes permanently the first time the dependency misbehaves -
            // which is precisely when it is needed.
            gate.Release();
        }
    }
}
