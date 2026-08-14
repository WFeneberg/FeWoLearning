//! Exercise 078 — Producer/consumer pipelines with `mpsc` channels (advanced).
//! Goal:   move values between threads through a channel instead of shared
//!         memory, including the multi-producer case where several senders
//!         feed one receiver.
//! Drills: `mpsc::channel`, `Sender::send`/`clone`, `Receiver` iteration,
//!         letting all senders drop so the receiver knows to stop.

use std::sync::mpsc;
use std::thread;

/// Spawns one producer thread that sends `0..count` over a channel; this
/// thread receives and doubles each value, returning results in send order.
pub fn double_pipeline(count: i64) -> Vec<i64> {
    todo!("mpsc::channel, spawn a producer sending 0..count, receive+double here, join producer")
}

/// Spawns one thread per entry of `chunks`, each sending its values through
/// a cloned `Sender`. Collects everything the receiver sees, across all
/// producers, into a sorted `Vec` (sorted because arrival order across
/// independent producer threads is not guaranteed, but the *set* received
/// is deterministic).
pub fn merge_from_producers(chunks: Vec<Vec<i64>>) -> Vec<i64> {
    todo!("clone the Sender per chunk, spawn producers, drop the original sender, collect+sort")
}

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
