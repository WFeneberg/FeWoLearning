//! Exercise 081 — Lock-free increments with `AtomicUsize` (reference solution).

use std::sync::atomic::{AtomicUsize, Ordering};
use std::sync::Arc;
use std::thread;

pub struct AtomicCounter {
    value: AtomicUsize,
}

impl AtomicCounter {
    pub fn new() -> Arc<AtomicCounter> {
        Arc::new(AtomicCounter {
            value: AtomicUsize::new(0),
        })
    }

    pub fn increment(&self) -> usize {
        self.value.fetch_add(1, Ordering::SeqCst)
    }

    pub fn get(&self) -> usize {
        self.value.load(Ordering::SeqCst)
    }
}

pub fn increment_concurrently(
    counter: &Arc<AtomicCounter>,
    times_per_thread: usize,
    thread_count: usize,
) -> Vec<usize> {
    let mut handles = Vec::with_capacity(thread_count);
    for _ in 0..thread_count {
        let counter = Arc::clone(counter);
        handles.push(thread::spawn(move || {
            (0..times_per_thread)
                .map(|_| counter.increment())
                .collect::<Vec<usize>>()
        }));
    }
    handles
        .into_iter()
        .flat_map(|h| h.join().expect("worker thread panicked"))
        .collect()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_fresh_counter_starts_at_zero() {
        let counter = AtomicCounter::new();
        assert_eq!(counter.get(), 0);
    }

    #[test]
    fn increment_returns_the_value_from_before_the_add() {
        let counter = AtomicCounter::new();
        assert_eq!(counter.increment(), 0);
        assert_eq!(counter.increment(), 1);
        assert_eq!(counter.increment(), 2);
        assert_eq!(counter.get(), 3);
    }

    #[test]
    fn concurrent_increments_return_all_distinct_previous_values() {
        // If `fetch_add` weren't truly atomic, two threads could receive the
        // same "previous value" back. Sorting every returned value and
        // comparing against the dense range 0..total catches that: any
        // duplicate or gap means the increments raced.
        let counter = AtomicCounter::new();
        let (times_per_thread, thread_count) = (500, 8);
        let total = times_per_thread * thread_count;

        let mut prevs = increment_concurrently(&counter, times_per_thread, thread_count);
        prevs.sort_unstable();

        assert_eq!(prevs, (0..total).collect::<Vec<usize>>());
        assert_eq!(counter.get(), total);
    }
}
