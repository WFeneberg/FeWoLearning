//! Exercise 075 — Sharing state across threads with `Arc<Mutex<T>>` (reference solution).

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

    pub fn increment(&self) {
        let mut guard = self.value.lock().expect("mutex poisoned");
        *guard += 1;
    }

    pub fn get(&self) -> u64 {
        *self.value.lock().expect("mutex poisoned")
    }
}

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

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
