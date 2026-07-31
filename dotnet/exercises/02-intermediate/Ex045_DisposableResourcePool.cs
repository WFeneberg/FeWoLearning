namespace FeWoLearning.Exercises.Intermediate;

// Exercise 045 — DisposableResourcePool (intermediate).
// Goal:   Implement a class that hands out a fixed number of resource "slots"
//         and correctly implements IDisposable so that:
//           - Acquire()/Release() manage slot usage and throw once exhausted.
//           - Any use after disposal throws ObjectDisposedException.
//           - Dispose() is idempotent: calling it more than once is safe and
//             only performs cleanup once, even though the call itself is
//             still counted.
// Drills: IDisposable, the using pattern, defensive state checks, idempotency.
public class DisposableResourcePool : IDisposable
{
    private readonly bool[] _acquired;

    public int Capacity { get; }

    public bool IsDisposed { get; private set; }

    // Counts every call to Dispose(), even redundant ones after the first.
    public int DisposeCallCount { get; private set; }

    public DisposableResourcePool(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        }

        Capacity = capacity;
        _acquired = new bool[capacity];
    }

    public int AvailableCount => throw new NotImplementedException();

    // Acquires the lowest-numbered free slot and returns its handle.
    // Throws InvalidOperationException when no slot is free.
    public int Acquire() => throw new NotImplementedException();

    // Releases a handle previously returned by Acquire().
    // Throws ArgumentOutOfRangeException for an out-of-range handle and
    // InvalidOperationException if the handle is not currently acquired.
    public void Release(int handle) => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();
}
