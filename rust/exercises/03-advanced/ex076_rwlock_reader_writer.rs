//! Exercise 076 — Many readers or one writer with `RwLock` (advanced).
//! Goal:   a shared collection that many threads can read concurrently, but
//!         only one thread may mutate at a time.
//! Drills: `RwLock::read`/`write`, `Arc<RwLock<T>>`, deterministic
//!         concurrency assertions via `join()` return values.

use std::sync::{Arc, RwLock};
use std::thread;

pub struct SharedConfig {
    data: RwLock<Vec<String>>,
}

impl SharedConfig {
    pub fn new() -> Arc<SharedConfig> {
        Arc::new(SharedConfig {
            data: RwLock::new(Vec::new()),
        })
    }

    /// Takes the write lock and appends `entry`.
    pub fn add(&self, entry: &str) {
        todo!("write-lock self.data and push entry.to_string()")
    }

    /// Takes the read lock and returns a clone of the current entries.
    pub fn read_all(&self) -> Vec<String> {
        todo!("read-lock self.data and clone its contents")
    }

    pub fn count(&self) -> usize {
        todo!("read-lock self.data and return its length")
    }
}

/// Spawns `thread_count` threads, each adding `entries_per_thread` entries,
/// and waits for all of them to finish before returning.
pub fn add_concurrently(config: &Arc<SharedConfig>, entries_per_thread: usize, thread_count: usize) {
    let mut handles = Vec::with_capacity(thread_count);
    for t in 0..thread_count {
        let config = Arc::clone(config);
        handles.push(thread::spawn(move || {
            for i in 0..entries_per_thread {
                config.add(&format!("t{t}-{i}"));
            }
        }));
    }
    for handle in handles {
        handle.join().expect("writer thread panicked");
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_fresh_config_is_empty() {
        let config = SharedConfig::new();
        assert_eq!(config.count(), 0);
        assert!(config.read_all().is_empty());
    }

    #[test]
    fn add_then_read_all_round_trips() {
        let config = SharedConfig::new();
        config.add("first");
        config.add("second");
        assert_eq!(config.count(), 2);
        assert_eq!(config.read_all(), vec!["first".to_string(), "second".to_string()]);
    }

    #[test]
    fn concurrent_writes_then_concurrent_reads_are_consistent() {
        let config = SharedConfig::new();
        add_concurrently(&config, 50, 4); // 200 entries total, all writers joined
        assert_eq!(config.count(), 200);

        // Many readers concurrently: each must see the fully-written state,
        // because their `join()` only returns after their thread's read
        // (which happens after all writers above already joined).
        let mut handles = Vec::with_capacity(4);
        for _ in 0..4 {
            let config = Arc::clone(&config);
            handles.push(thread::spawn(move || config.read_all().len()));
        }
        for handle in handles {
            assert_eq!(handle.join().expect("reader thread panicked"), 200);
        }
    }
}
