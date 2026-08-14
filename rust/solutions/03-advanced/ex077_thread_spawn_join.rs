//! Exercise 077 — `std::thread::spawn`, `join`, and moving captures (reference solution).

use std::thread;

pub fn greet_in_thread(name: String) -> String {
    let handle = thread::spawn(move || format!("Hello, {name}!"));
    handle.join().expect("thread panicked")
}

pub fn parallel_chunk_sums(data: Vec<i64>, thread_count: usize) -> Vec<i64> {
    if data.is_empty() || thread_count == 0 {
        return Vec::new();
    }
    let chunk_size = (data.len() + thread_count - 1) / thread_count;
    let handles: Vec<_> = data
        .chunks(chunk_size.max(1))
        .map(|chunk| {
            let chunk = chunk.to_vec(); // owned, so it can move into the thread
            thread::spawn(move || chunk.iter().sum::<i64>())
        })
        .collect();
    handles
        .into_iter()
        .map(|h| h.join().expect("worker thread panicked"))
        .collect()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
