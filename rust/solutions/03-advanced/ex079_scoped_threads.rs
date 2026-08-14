//! Exercise 079 — Borrowing locals across threads with `thread::scope` (reference solution).

use std::thread;

pub fn scoped_chunk_sum(data: &[i64], thread_count: usize) -> i64 {
    if data.is_empty() || thread_count == 0 {
        return 0;
    }
    let chunk_size = (data.len() + thread_count - 1) / thread_count;
    thread::scope(|s| {
        let handles: Vec<_> = data
            .chunks(chunk_size.max(1))
            .map(|chunk| s.spawn(move || chunk.iter().sum::<i64>()))
            .collect();
        handles
            .into_iter()
            .map(|h| h.join().expect("worker thread panicked"))
            .sum()
    })
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
