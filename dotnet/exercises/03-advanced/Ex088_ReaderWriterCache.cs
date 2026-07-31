namespace FeWoLearning.Exercises.Advanced;

// Exercise 088 — Reader/writer cache (advanced).
// Goal:   A keyed cache that allows many concurrent readers but only one
//         exclusive writer at a time, using ReaderWriterLockSlim.
// Drills: ReaderWriterLockSlim, read/write lock scopes, thread safety.
public sealed class ReaderWriterCache<TKey, TValue> where TKey : notnull
{
    public ReaderWriterCache() => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public TValue AddOrUpdate(
        TKey key,
        Func<TKey, TValue> addValueFactory,
        Func<TKey, TValue, TValue> updateValueFactory) => throw new NotImplementedException();

    public bool TryGet(TKey key, out TValue value) => throw new NotImplementedException();

    public bool Remove(TKey key) => throw new NotImplementedException();
}
