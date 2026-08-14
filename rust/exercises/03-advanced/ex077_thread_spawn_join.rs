//! Exercise 077 — `std::thread::spawn`, `join`, and moving captures (advanced).
//! Goal:   spawn worker threads that take *ownership* of the data they need
//!         (via `move` closures) and collect their results deterministically
//!         through `JoinHandle::join`.
//! Drills: `thread::spawn`, `move` closures, `JoinHandle<T>::join`.

use std::thread;

/// Moves `name` into a spawned thread that builds a greeting, joins it, and
/// returns the greeting. `name` must be owned (not borrowed) because the
/// spawned thread may outlive the caller's stack frame in general.
pub fn greet_in_thread(name: String) -> String {
    todo!("thread::spawn(move || format!(\"Hello, {{name}}!\")), then join and return it")
}

/// Splits `data` into `thread_count` contiguous chunks, sums each chunk on
/// its own thread (each thread owns its chunk via a `move` closure), and
/// returns the partial sums in the same order as the original chunks.
pub fn parallel_chunk_sums(data: Vec<i64>, thread_count: usize) -> Vec<i64> {
    todo!("chunk `data`, spawn one thread per chunk to sum it, join all in order, collect sums")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn greet_in_thread_returns_the_joined_result() {
        assert_eq!(greet_in_thread("Ada".to_string()), "Hello, Ada!");
        assert_eq!(greet_in_thread("Grace".to_string()), "Hello, Grace!");
    }

    #[test]
    fn parallel_chunk_sums_splits_and_sums_in_order() {
        let sums = parallel_chunk_sums(vec![1, 2, 3, 4, 5, 6], 3);
        assert_eq!(sums, vec![3, 7, 11]); // [1,2] [3,4] [5,6]
    }

    #[test]
    fn more_threads_than_elements_still_works() {
        let sums = parallel_chunk_sums(vec![10, 20, 30], 5);
        assert_eq!(sums.iter().sum::<i64>(), 60);
    }

    #[test]
    fn empty_data_produces_no_sums() {
        let sums = parallel_chunk_sums(vec![], 4);
        assert!(sums.is_empty());
    }
}
