namespace FeWoLearning.Exercises.Intermediate;

// Exercise 047 — Parallel Task Aggregator (intermediate).
// Goal:   Given a collection of asynchronous operations that each produce an int,
//         run them all concurrently with Task.WhenAll and return the sum of their
//         results.
// Drills: Task composition, Task.WhenAll, async/await, LINQ over tasks.
public static class ParallelTaskAggregator
{
    // Runs every operation concurrently (do not await them one by one in a loop),
    // waits for all of them to complete via Task.WhenAll, and returns the sum of
    // the individual results.
    public static Task<int> SumAsync(IEnumerable<Func<Task<int>>> operations)
        => throw new NotImplementedException();
}
