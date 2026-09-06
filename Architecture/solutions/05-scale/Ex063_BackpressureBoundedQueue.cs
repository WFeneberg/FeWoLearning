namespace FeWoLearning.Architecture.Exercises.Scale.Ex063;

/// <summary>What a full buffer does to the writer.</summary>
public enum FullPolicy
{
    /// <summary>Make the producer wait. Backpressure reaches all the way up.</summary>
    Wait,

    /// <summary>Keep the newest, discard the oldest. For state, where only "now" matters.</summary>
    DropOldest,

    /// <summary>Keep what is already queued, discard the arrival. For events, where order matters.</summary>
    DropNewest,
}

// Exercise 063 — BackpressureBoundedQueue (reference solution).
public sealed class BoundedBuffer<T>(int capacity, FullPolicy policy)
{
    private readonly Queue<T> _items = new();
    private readonly Queue<TaskCompletionSource> _waitingWriters = new();
    private readonly Lock _gate = new();
    private int _dropped;

    public int Count
    {
        get { lock (_gate) return _items.Count; }
    }

    public int Dropped => Volatile.Read(ref _dropped);

    public bool TryWrite(T item)
    {
        lock (_gate)
        {
            if (_items.Count < capacity)
            {
                _items.Enqueue(item);
                return true;
            }

            switch (policy)
            {
                case FullPolicy.DropOldest:
                    // State: only "now" matters, so the stale value goes and the arrival
                    // is kept. Counted, because silent loss is the failure mode that
                    // outlives everybody who understood the system.
                    _items.Dequeue();
                    _items.Enqueue(item);
                    Interlocked.Increment(ref _dropped);
                    return true;

                case FullPolicy.DropNewest:
                    // Events: the first ones say what started, and losing the middle of a
                    // sequence is worse than losing its tail.
                    Interlocked.Increment(ref _dropped);
                    return false;

                default:
                    // Wait is not a drop policy - the writer is supposed to be awaiting
                    // WriteAsync, and silently accepting here would blow the bound.
                    return false;
            }
        }
    }

    public Task WriteAsync(T item)
    {
        lock (_gate)
        {
            if (_items.Count < capacity)
            {
                _items.Enqueue(item);
                return Task.CompletedTask;
            }

            // The producer is made to wait, which is what pushes the pressure back up the
            // chain. It is the right answer when the producer CAN slow down, and the
            // wrong one when it is a network socket that cannot.
            var waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waitingWriters.Enqueue(waiter);

            return WaitThenEnqueue(waiter, item);
        }
    }

    private async Task WaitThenEnqueue(TaskCompletionSource waiter, T item)
    {
        await waiter.Task.ConfigureAwait(false);

        lock (_gate)
            _items.Enqueue(item);
    }

    public bool TryRead(out T item)
    {
        TaskCompletionSource? released = null;

        lock (_gate)
        {
            if (_items.Count == 0)
            {
                item = default!;
                return false;
            }

            item = _items.Dequeue();

            if (_waitingWriters.Count > 0)
                released = _waitingWriters.Dequeue();
        }

        // Released OUTSIDE the lock: the waiter's continuation enqueues, and completing it
        // while still holding the gate would deadlock the moment it runs synchronously.
        released?.TrySetResult();
        return true;
    }
}
