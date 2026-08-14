package fewolearning.exercises.advanced.ex071_lru_cache;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;

class LruCacheTest {

    @Test
    void retainsValuesWithinCapacity() {
        LruCache<String, Integer> cache = new LruCache<>(2);

        cache.put("a", 1);
        cache.put("b", 2);

        assertEquals(1, cache.get("a").intValue());
        assertEquals(2, cache.get("b").intValue());
        assertEquals(2, cache.size());
    }

    @Test
    void evictsTheLeastRecentlyUsedEntryWhenCapacityIsExceeded() {
        LruCache<String, Integer> cache = new LruCache<>(2);

        cache.put("a", 1);
        cache.put("b", 2);
        cache.put("c", 3);

        assertNull(cache.get("a"));
        assertEquals(2, cache.get("b").intValue());
        assertEquals(3, cache.get("c").intValue());
        assertEquals(2, cache.size());
    }

    @Test
    void accessingAnEntryProtectsItFromEviction() {
        LruCache<String, Integer> cache = new LruCache<>(2);

        cache.put("a", 1);
        cache.put("b", 2);
        cache.get("a");
        cache.put("c", 3);

        assertEquals(1, cache.get("a").intValue());
        assertNull(cache.get("b"));
        assertEquals(3, cache.get("c").intValue());
    }
}
