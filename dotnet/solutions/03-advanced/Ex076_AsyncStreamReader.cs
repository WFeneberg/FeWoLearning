using System.Runtime.CompilerServices;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 076 — Async stream reader (reference solution).
// An async iterator: each MoveNextAsync waits `delay` (if positive) before
// producing the next item, checking the cancellation token on every step.
public static class AsyncStreamReader
{
    public static async IAsyncEnumerable<T> ReadAsync<T>(
        IEnumerable<T> source,
        TimeSpan delay,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken);

            yield return item;
        }
    }
}
