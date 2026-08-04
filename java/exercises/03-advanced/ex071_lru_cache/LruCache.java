package fewolearning.exercises.advanced.ex071_lru_cache;

import java.util.LinkedHashMap;
import java.util.Map;

/*
Exercise 071 - LRU cache (advanced).

Goal:   Evict the least recently used entry once the cache exceeds its capacity.
Drills: maps, eviction policy, recency tracking.
*/
public final class LruCache<K, V> {
    private final int capacity;
    private final Map<K, V> store;

    public LruCache(int capacity) {
        this.capacity = capacity;
        this.store = new LinkedHashMap<>(16, 0.75f, true);
    }

    public void put(K key, V value) {
        throw new UnsupportedOperationException("TODO");
    }

    public V get(K key) {
        throw new UnsupportedOperationException("TODO");
    }

    public int size() {
        throw new UnsupportedOperationException("TODO");
    }
}
