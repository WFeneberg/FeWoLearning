namespace FeWoLearning.Exercises.Advanced;

// Exercise 074 — SemaphoreSlim throttling (advanced).
// Goal:   Limit the number of concurrently running async operations to a
//         fixed maximum using SemaphoreSlim, regardless of how many callers
//         request work at once.
// Drills: SemaphoreSlim, async/await, try/finally release discipline,
//         bounded concurrency.
public sealed class SemaphoreThrottle : IDisposable
{
    public SemaphoreThrottle(int maxConcurrency) => throw new NotImplementedException();

    public int MaxConcurrency => throw new NotImplementedException();

    public Task<T> RunAsync<T>(Func<Task<T>> operation) => throw new NotImplementedException();

    public Task RunAsync(Func<Task> operation) => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
