namespace FeWoLearning.Exercises.Advanced;

// Exercise 076 — Async stream reader (advanced).
// Goal:   Implement an async iterator that yields items from a source sequence,
//         waiting `delay` between each yield, and honors cooperative cancellation.
// Drills: IAsyncEnumerable<T>, async iterators (yield return in an async method),
//         Task.Delay, CancellationToken / [EnumeratorCancellation].
public static class AsyncStreamReader
{
    public static IAsyncEnumerable<T> ReadAsync<T>(
        IEnumerable<T> source,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
