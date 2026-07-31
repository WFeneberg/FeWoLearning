namespace FeWoLearning.Exercises.Advanced;

// Exercise 089 — Object pool (reference solution).
// A stack of available instances plus a set of currently rented instances
// (tracked by reference identity) so Return() can reject buffers that were
// never rented, or that have already been returned.
public sealed class ObjectPoolImpl
{
    private readonly int _bufferSize;
    private readonly Stack<PooledBuffer> _available = new();
    private readonly HashSet<PooledBuffer> _rented = new(ReferenceEqualityComparer.Instance);
    private int _created;

    public ObjectPoolImpl(int bufferSize, int initialSize)
    {
        if (bufferSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "Must be positive.");
        if (initialSize < 0)
            throw new ArgumentOutOfRangeException(nameof(initialSize), initialSize, "Must be non-negative.");

        _bufferSize = bufferSize;
        for (var i = 0; i < initialSize; i++)
        {
            _available.Push(new PooledBuffer(bufferSize));
            _created++;
        }
    }

    public int CreatedCount => _created;

    public int AvailableCount => _available.Count;

    public PooledBuffer Get()
    {
        PooledBuffer buffer;
        if (_available.Count > 0)
        {
            buffer = _available.Pop();
        }
        else
        {
            buffer = new PooledBuffer(_bufferSize);
            _created++;
        }

        _rented.Add(buffer);
        return buffer;
    }

    public void Return(PooledBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (!_rented.Remove(buffer))
            throw new InvalidOperationException("Buffer was not rented from this pool (or was already returned).");

        _available.Push(buffer);
    }
}

// A pooled unit of reusable storage. Instances are only ever constructed by
// ObjectPoolImpl; callers rent and return them through the pool.
public sealed class PooledBuffer
{
    public byte[] Data { get; }

    internal PooledBuffer(int size) => Data = new byte[size];
}
