//! Exercise 079 — Borrowing locals across threads with `thread::scope` (advanced).
//! Goal:   sum a slice by splitting the work across several threads that
//!         *borrow* sub-slices of it directly — no cloning, no `Arc`, no
//!         `'static` bound — because `thread::scope` guarantees every spawned
//!         thread finishes before the scope itself returns.
//! Drills: `thread::scope`, `Scope::spawn`, borrowing non-`'static` locals.

use std::thread;

/// Sums `data` by splitting it into `thread_count` borrowed sub-slices, one
/// per scoped thread, and adding up their partial sums.
pub fn scoped_chunk_sum(data: &[i64], thread_count: usize) -> i64 {
    todo!("thread::scope(|s| ...): spawn one thread per chunk of `data`, sum each, join, add up")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sums_a_locally_owned_slice_without_cloning_or_arc() {
        // `data` is a plain stack local — not `'static`, not wrapped in `Arc`.
        // Only `thread::scope` makes it sound to hand borrows of it to threads.
        let data = vec![10, 20, 30, 40];
        assert_eq!(scoped_chunk_sum(&data, 2), 100);
    }

    #[test]
    fn empty_slice_sums_to_zero() {
        assert_eq!(scoped_chunk_sum(&[], 4), 0);
    }

    #[test]
    fn single_thread_sums_everything() {
        let data = vec![1, 2, 3, 4, 5];
        assert_eq!(scoped_chunk_sum(&data, 1), 15);
    }

    #[test]
    fn more_threads_than_elements_still_sums_correctly() {
        let data = vec![7, 8, 9];
        assert_eq!(scoped_chunk_sum(&data, 10), 24);
    }
}
