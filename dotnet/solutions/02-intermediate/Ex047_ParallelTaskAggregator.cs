namespace FeWoLearning.Exercises.Intermediate;

// Exercise 047 — Parallel Task Aggregator (reference solution).
public static class ParallelTaskAggregator
{
    public static async Task<int> SumAsync(IEnumerable<Func<Task<int>>> operations)
    {
        // Start every operation up front so they all run concurrently instead of
        // being awaited one at a time.
        var tasks = operations.Select(operation => operation()).ToArray();

        var results = await Task.WhenAll(tasks);

        return results.Sum();
    }
}
