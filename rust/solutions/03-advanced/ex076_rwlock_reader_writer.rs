//! Exercise 076 — Many readers or one writer with `RwLock` (reference solution).

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

    pub fn add(&self, entry: &str) {
        self.data.write().expect("lock poisoned").push(entry.to_string());
    }

    pub fn read_all(&self) -> Vec<String> {
        self.data.read().expect("lock poisoned").clone()
    }

    pub fn count(&self) -> usize {
        self.data.read().expect("lock poisoned").len()
    }
}

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

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
