namespace FeWoLearning.Exercises.Advanced;

// Exercise 089 — Object pool (advanced).
// Goal:   Fixed-size pool of reusable buffer instances that grows on demand;
//         Get() hands out an available instance (or creates a new one when
//         exhausted), Return() gives it back for reuse and guards against
//         returning an instance that was not rented (or already returned).
// Drills: object pooling pattern, reference-identity tracking, resource reuse.
public sealed class ObjectPoolImpl
{
    public ObjectPoolImpl(int bufferSize, int initialSize) => throw new NotImplementedException();

    public int CreatedCount => throw new NotImplementedException();

    public int AvailableCount => throw new NotImplementedException();

    public PooledBuffer Get() => throw new NotImplementedException();

    public void Return(PooledBuffer buffer) => throw new NotImplementedException();
}

// A pooled unit of reusable storage. Instances are only ever constructed by
// ObjectPoolImpl; callers rent and return them through the pool.
public sealed class PooledBuffer
{
    public byte[] Data { get; }

    internal PooledBuffer(int size) => Data = new byte[size];
}
