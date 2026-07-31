namespace FeWoLearning.Exercises.Advanced;

// Exercise 088 — Reader/writer cache (reference solution).
// ReaderWriterLockSlim lets any number of readers run concurrently, but a
// writer acquires the lock exclusively, blocking both new readers and other
// writers, so the map is never observed (or mutated) mid-update.
public sealed class ReaderWriterCache<TKey, TValue> where TKey : notnull
{
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
    private readonly Dictionary<TKey, TValue> _map = new();

    public int Count
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _map.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    public TValue AddOrUpdate(
        TKey key,
        Func<TKey, TValue> addValueFactory,
        Func<TKey, TValue, TValue> updateValueFactory)
    {
        ArgumentNullException.ThrowIfNull(addValueFactory);
        ArgumentNullException.ThrowIfNull(updateValueFactory);

        _lock.EnterWriteLock();
        try
        {
            TValue result = _map.TryGetValue(key, out var existing)
                ? updateValueFactory(key, existing)
                : addValueFactory(key);
            _map[key] = result;
            return result;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        _lock.EnterReadLock();
        try
        {
            return _map.TryGetValue(key, out value);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool Remove(TKey key)
    {
        _lock.EnterWriteLock();
        try
        {
            return _map.Remove(key);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}
