//! Exercise 071 — A generic LRU cache (advanced).
//! Goal:   a fixed-capacity cache that evicts the *least recently used* entry
//!         when it would otherwise grow past capacity, tracking recency with
//!         plain `HashMap` + `Vec` bookkeeping (no external crate).
//! Drills: generics with trait bounds, `HashMap`, manual recency-order upkeep.

use std::collections::HashMap;
use std::hash::Hash;

pub struct LruCache<K, V> {
    capacity: usize,
    map: HashMap<K, V>,
    // Recency order, least-recently-used first, most-recently-used last.
    order: Vec<K>,
}

impl<K: Eq + Hash + Clone, V> LruCache<K, V> {
    pub fn new(capacity: usize) -> Self {
        assert!(capacity > 0, "capacity must be positive");
        LruCache {
            capacity,
            map: HashMap::new(),
            order: Vec::new(),
        }
    }

    /// Looks up `key`, marking it as the most recently used entry if found.
    pub fn get(&mut self, key: &K) -> Option<&V> {
        todo!("bump `key` to the back of `order` if present, then return the value")
    }

    /// Inserts or updates `key`. If the cache is over capacity afterwards,
    /// evicts the least recently used entry (the front of `order`).
    pub fn put(&mut self, key: K, value: V) {
        todo!("insert/update, bump recency, and evict the LRU entry if over capacity")
    }

    pub fn len(&self) -> usize {
        self.map.len()
    }

    pub fn is_empty(&self) -> bool {
        self.map.is_empty()
    }

    pub fn contains_key(&self, key: &K) -> bool {
        self.map.contains_key(key)
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_fresh_cache_is_empty() {
        let mut cache: LruCache<i32, &str> = LruCache::new(2);
        assert!(cache.is_empty());
        assert_eq!(cache.len(), 0);
        assert_eq!(cache.get(&1), None);
    }

    #[test]
    fn put_then_get_round_trips() {
        let mut cache = LruCache::new(2);
        cache.put("a", 1);
        cache.put("b", 2);
        assert_eq!(cache.get(&"a"), Some(&1));
        assert_eq!(cache.get(&"b"), Some(&2));
        assert_eq!(cache.len(), 2);
    }

    #[test]
    fn inserting_past_capacity_evicts_the_least_recently_used() {
        let mut cache = LruCache::new(2);
        cache.put(1, "one");
        cache.put(2, "two");
        cache.put(3, "three"); // 1 is LRU, should be evicted
        assert_eq!(cache.len(), 2);
        assert!(!cache.contains_key(&1));
        assert!(cache.contains_key(&2));
        assert!(cache.contains_key(&3));
    }

    #[test]
    fn getting_an_entry_protects_it_from_eviction() {
        let mut cache = LruCache::new(2);
        cache.put(1, "one");
        cache.put(2, "two");
        // Touch 1 so 2 becomes the LRU entry instead.
        assert_eq!(cache.get(&1), Some(&"one"));
        cache.put(3, "three");
        assert!(cache.contains_key(&1));
        assert!(!cache.contains_key(&2));
        assert!(cache.contains_key(&3));
    }

    #[test]
    fn updating_an_existing_key_does_not_grow_the_cache() {
        let mut cache = LruCache::new(2);
        cache.put(1, "one");
        cache.put(1, "uno");
        assert_eq!(cache.len(), 1);
        assert_eq!(cache.get(&1), Some(&"uno"));
    }
}
