//! Exercise 075 — Sharing state across threads with `Arc<Mutex<T>>` (advanced).
//! Goal:   a counter that many threads can safely increment concurrently,
//!         using `Arc` to share ownership and `Mutex` to serialize access.
//! Drills: `Arc::clone`, `Mutex::lock`, `thread::spawn` + `join` for
//!         deterministic (non-racy) assertions.

use std::sync::{Arc, Mutex};
use std::thread;

pub struct Counter {
    value: Mutex<u64>,
}

impl Counter {
    pub fn new() -> Arc<Counter> {
        Arc::new(Counter {
            value: Mutex::new(0),
        })
    }

    /// Locks the mutex and adds one to the count.
    pub fn increment(&self) {
        todo!("lock self.value and add 1")
    }

    pub fn get(&self) -> u64 {
        todo!("lock self.value and return the current count")
    }
}

/// Spawns `thread_count` threads, each incrementing `counter` exactly
/// `times_per_thread` times, then waits for all of them to finish.
pub fn increment_concurrently(counter: &Arc<Counter>, times_per_thread: u64, thread_count: usize) {
    let mut handles = Vec::with_capacity(thread_count);
    for _ in 0..thread_count {
        let counter = Arc::clone(counter);
        handles.push(thread::spawn(move || {
            for _ in 0..times_per_thread {
                counter.increment();
            }
        }));
    }
    for handle in handles {
        handle.join().expect("worker thread panicked");
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn increment_and_get_on_one_thread() {
        let counter = Counter::new();
        counter.increment();
        counter.increment();
        counter.increment();
        assert_eq!(counter.get(), 3);
    }

    #[test]
    fn a_fresh_counter_starts_at_zero() {
        let counter = Counter::new();
        assert_eq!(counter.get(), 0);
    }

    #[test]
    fn many_threads_incrementing_concurrently_all_land() {
        // `join()` inside `increment_concurrently` makes this deterministic:
        // every increment from every thread is guaranteed to have completed
        // (and been serialized by the Mutex) before we read the final value.
        let counter = Counter::new();
        increment_concurrently(&counter, 1_000, 8);
        assert_eq!(counter.get(), 8_000);
    }

    #[test]
    fn cloned_arcs_see_the_same_shared_state() {
        let counter = Counter::new();
        let cloned = Arc::clone(&counter);
        cloned.increment();
        counter.increment();
        assert_eq!(counter.get(), 2);
        assert_eq!(cloned.get(), 2);
    }
}
