namespace FeWoLearning.Exercises.Intermediate;

// Exercise 045 — DisposableResourcePool (reference solution).
public class DisposableResourcePool : IDisposable
{
    private readonly bool[] _acquired;

    public int Capacity { get; }

    public bool IsDisposed { get; private set; }

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

    public int AvailableCount
    {
        get
        {
            ThrowIfDisposed();
            var used = 0;
            foreach (var slot in _acquired)
            {
                if (slot)
                {
                    used++;
                }
            }

            return Capacity - used;
        }
    }

    public int Acquire()
    {
        ThrowIfDisposed();

        for (var i = 0; i < _acquired.Length; i++)
        {
            if (!_acquired[i])
            {
                _acquired[i] = true;
                return i;
            }
        }

        throw new InvalidOperationException("No resources available in the pool.");
    }

    public void Release(int handle)
    {
        ThrowIfDisposed();

        if (handle < 0 || handle >= _acquired.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        if (!_acquired[handle])
        {
            throw new InvalidOperationException($"Handle {handle} is not currently acquired.");
        }

        _acquired[handle] = false;
    }

    public void Dispose()
    {
        DisposeCallCount++;

        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        Array.Clear(_acquired, 0, _acquired.Length);
    }

    private void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(DisposableResourcePool));
        }
    }
}
