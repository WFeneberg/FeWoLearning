namespace FeWoLearning.Exercises.Advanced;

// Exercise 071 — Generic LRU cache (advanced).
// Goal:   Fixed-capacity Least-Recently-Used cache with O(1) Get/Put.
// Drills: generics, LinkedList + Dictionary, eviction policy.
public sealed class LruCache<TKey, TValue> where TKey : notnull
{
    public LruCache(int capacity) => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool TryGet(TKey key, out TValue value) => throw new NotImplementedException();

    public void Put(TKey key, TValue value) => throw new NotImplementedException();
}
