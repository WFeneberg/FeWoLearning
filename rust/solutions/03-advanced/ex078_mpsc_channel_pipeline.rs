//! Exercise 078 — Producer/consumer pipelines with `mpsc` channels (reference solution).

use std::sync::mpsc;
use std::thread;

pub fn double_pipeline(count: i64) -> Vec<i64> {
    let (tx, rx) = mpsc::channel();
    let producer = thread::spawn(move || {
        for i in 0..count {
            tx.send(i).expect("receiver dropped");
        }
    });
    let doubled: Vec<i64> = rx.iter().map(|v| v * 2).collect();
    producer.join().expect("producer thread panicked");
    doubled
}

pub fn merge_from_producers(chunks: Vec<Vec<i64>>) -> Vec<i64> {
    let (tx, rx) = mpsc::channel();
    let mut handles = Vec::with_capacity(chunks.len());
    for chunk in chunks {
        let tx = tx.clone();
        handles.push(thread::spawn(move || {
            for v in chunk {
                tx.send(v).expect("receiver dropped");
            }
        }));
    }
    drop(tx); // drop the original sender so the receiver iterator ends once
              // every cloned sender (held by a worker thread) has also dropped
    let mut received: Vec<i64> = rx.iter().collect();
    for handle in handles {
        handle.join().expect("producer thread panicked");
    }
    received.sort_unstable();
    received
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn double_pipeline_doubles_every_value_in_order() {
        assert_eq!(double_pipeline(5), vec![0, 2, 4, 6, 8]);
    }

    #[test]
    fn double_pipeline_of_zero_is_empty() {
        assert_eq!(double_pipeline(0), Vec::<i64>::new());
    }

    #[test]
    fn merge_from_producers_collects_every_value() {
        let merged = merge_from_producers(vec![vec![3, 1], vec![2, 4], vec![]]);
        assert_eq!(merged, vec![1, 2, 3, 4]);
    }

    #[test]
    fn merge_from_producers_with_no_chunks_is_empty() {
        let merged = merge_from_producers(vec![]);
        assert!(merged.is_empty());
    }

    #[test]
    fn merge_from_producers_handles_many_producers_deterministically() {
        let chunks: Vec<Vec<i64>> = (0..10).map(|i| vec![i]).collect();
        let merged = merge_from_producers(chunks);
        assert_eq!(merged, (0..10).collect::<Vec<i64>>());
    }
}
