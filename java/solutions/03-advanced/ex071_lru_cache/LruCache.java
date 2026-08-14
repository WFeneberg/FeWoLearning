package fewolearning.exercises.advanced.ex071_lru_cache;

import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;

/*
Exercise 071 - LRU cache (reference solution).
*/
public final class LruCache<K, V> {
    private final int capacity;
    private final Map<K, V> store;

    public LruCache(int capacity) {
        this.capacity = capacity;
        this.store = new LinkedHashMap<>(16, 0.75f, true);
    }

    public void put(K key, V value) {
        store.put(key, value);
        if (store.size() > capacity) {
            Iterator<Map.Entry<K, V>> eldestFirst = store.entrySet().iterator();
            eldestFirst.next();
            eldestFirst.remove();
        }
    }

    public V get(K key) {
        return store.get(key);
    }

    public int size() {
        return store.size();
    }
}
