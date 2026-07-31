namespace FeWoLearning.Exercises.Advanced;

// Exercise 074 — SemaphoreSlim throttling (reference solution).
// A SemaphoreSlim initialized with (maxConcurrency, maxConcurrency) permits
// at most `maxConcurrency` callers past WaitAsync at any given time; the
// finally block guarantees the permit is returned even if the operation
// throws.
public sealed class SemaphoreThrottle : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public SemaphoreThrottle(int maxConcurrency)
    {
        if (maxConcurrency <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxConcurrency), maxConcurrency, "Must be positive.");

        MaxConcurrency = maxConcurrency;
        _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
    }

    public int MaxConcurrency { get; }

    public async Task<T> RunAsync<T>(Func<Task<T>> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            return await operation().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RunAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            await operation().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _semaphore.Dispose();
    }
}
