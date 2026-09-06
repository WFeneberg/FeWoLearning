namespace FeWoLearning.Architecture.Exercises.Scale.Ex061;

public sealed class BulkheadRejectedException(string partition)
    : Exception($"Partition '{partition}' is at capacity.")
{
    public string Partition { get; } = partition;
}

// Exercise 061 — BulkheadIsolation (scale).
// Goal:   Give each dependency its own slice of the process's capacity, so that one of
//         them going slow cannot take the rest down with it.
// Drills: partitioned concurrency, resource isolation, releasing on failure.
// Passes: capacity    - up to a partition's capacity runs at once.
//         rejection   - the next caller is rejected with BulkheadRejectedException, and
//                       the work is NOT invoked. A caller that is going to fail should
//                       fail now and cheaply.
//         THE ONE      - saturating partition A leaves partition B fully available. One
//                       shared semaphore satisfies both facts above and is not a bulkhead.
//         release     - a slot comes back when the work finishes, AND when it throws.
//
// The isolation fact is the whole pattern. Without it, a payment provider that starts
// taking thirty seconds instead of thirty milliseconds does not degrade payments - it
// consumes every thread, connection and task slot in the process, and the site stops
// serving its home page. The failure looks like "everything is down", which is why it
// takes so long to find, and the cause is one dependency nobody was watching.
//
// The cost is written on the tin: capacity is now reserved rather than shared, so the
// process can be rejecting work in one partition while another sits idle. That is the
// trade, and it is the right one - a bulkhead is a guarantee about the OTHER
// partitions.
public sealed class Bulkhead(IReadOnlyDictionary<string, int> capacities)
{
    /// <summary>How many callers are currently inside <paramref name="partition"/>.</summary>
    public int InFlight(string partition) =>
        throw new NotImplementedException("TODO: Ex061 - how many callers hold a slot in this partition");

    /// <summary>
    /// Run <paramref name="work"/> inside <paramref name="partition"/>'s slice, or reject
    /// immediately if it is full. An unknown partition has no limit.
    /// </summary>
    public Task<T> ExecuteAsync<T>(string partition, Func<Task<T>> work) =>
        throw new NotImplementedException(
            "TODO: Ex061 - take a slot in THIS partition or throw BulkheadRejectedException without calling work, and release the slot even on failure");
}
