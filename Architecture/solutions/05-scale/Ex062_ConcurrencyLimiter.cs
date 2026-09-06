namespace FeWoLearning.Architecture.Exercises.Scale.Ex062;

public sealed class LoadSheddingException() : Exception("The service is at capacity; try again later.");

// Exercise 062 — ConcurrencyLimiter (reference solution).
public sealed class AdmissionController(int concurrency, int queueDepth)
{
    private readonly SemaphoreSlim _slots = new(concurrency, concurrency);
    private int _running;
    private int _queued;

    public int Running => Volatile.Read(ref _running);

    public int Queued => Volatile.Read(ref _queued);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> work)
    {
        // The admission decision is made BEFORE waiting, and it is made against the queue
        // depth. This is the line that turns "slow" into "refused": with an unbounded
        // queue nothing is ever refused, the queue grows until everything in it has
        // already timed out client-side, and the service spends all its capacity
        // computing answers nobody is waiting for. That state is stable and looks like a
        // total outage.
        if (!TryEnterQueue())
            throw new LoadSheddingException();

        try
        {
            await _slots.WaitAsync().ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Decrement(ref _queued);
        }

        Interlocked.Increment(ref _running);

        try
        {
            return await work().ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Decrement(ref _running);
            _slots.Release();
        }
    }

    /// <summary>
    /// Compare-and-swap rather than "read, decide, increment". Under load - which is the
    /// only time this method matters - the read-then-write version admits more callers
    /// than the depth allows, and the limit quietly stops being a limit exactly when it
    /// is needed.
    /// </summary>
    private bool TryEnterQueue()
    {
        while (true)
        {
            var queued = Volatile.Read(ref _queued);
            var running = Volatile.Read(ref _running);

            if (running + queued >= concurrency + queueDepth)
                return false;

            if (Interlocked.CompareExchange(ref _queued, queued + 1, queued) == queued)
                return true;
        }
    }
}
