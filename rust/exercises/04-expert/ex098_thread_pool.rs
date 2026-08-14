//! Exercise 098 — A thread pool: worker threads, job channel, graceful
//! shutdown (expert).
//! Goal:   `ThreadPool::new(size)` spawns `size` worker threads sharing one
//!         `mpsc` job channel (`Receiver` behind `Arc<Mutex<_>>` so every
//!         worker can pull from it); `execute` sends a boxed closure down
//!         the channel for whichever worker is free next to run. Shutdown
//!         is graceful: `Drop` drops the `Sender` first (closing the
//!         channel, so every worker's blocking `recv()` returns `Err` and
//!         its loop exits) and only THEN joins every worker's `JoinHandle`
//!         — so `Drop` doesn't return until every already-queued job has
//!         actually finished running, not just been handed off.
//! Drills: `mpsc::channel`, sharing one `Receiver` across threads via
//!         `Arc<Mutex<_>>`, `Box<dyn FnOnce() + Send>` as a "job", detecting
//!         channel closure via `Err` from `recv()`, joining worker threads
//!         from within a custom `Drop`.

use std::sync::mpsc::{self, Sender};
use std::sync::{Arc, Mutex};
use std::thread::{self, JoinHandle};

type Job = Box<dyn FnOnce() + Send + 'static>;

/// A fixed-size pool of worker threads that run jobs handed to `execute`.
pub struct ThreadPool {
    // `None` only ever briefly, inside `Drop`, to signal shutdown by
    // closing the channel.
    sender: Option<Sender<Job>>,
    workers: Vec<JoinHandle<()>>,
}

impl ThreadPool {
    /// Spawns `size` worker threads, all pulling jobs from one shared
    /// channel.
    pub fn new(size: usize) -> Self {
        todo!(
            "create an mpsc::channel::<Job>(); wrap the Receiver in Arc<Mutex<_>> so all \
             `size` workers can share it; spawn `size` threads, each looping: lock the \
             receiver, call recv(), then DROP THE LOCK before running the job (so other \
             workers aren't blocked while this one works) — on Ok(job) run job(), on Err(_) \
             (channel closed) break the loop; collect every JoinHandle into `workers` and \
             return Self {{ sender: Some(sender), workers }}"
        )
    }

    /// Submits `job` to be run by whichever worker is free next.
    pub fn execute<F: FnOnce() + Send + 'static>(&self, job: F) {
        todo!("box `job` as a Job and send it down self.sender (it's always Some while the pool is alive)")
    }
}

impl Drop for ThreadPool {
    /// Graceful shutdown: dropping the sender closes the channel (every
    /// worker's blocking `recv()` then returns `Err` and its loop exits),
    /// then we join every worker so `Drop` doesn't return until all queued
    /// jobs have actually finished running.
    fn drop(&mut self) {
        self.sender.take();
        for worker in self.workers.drain(..) {
            let _ = worker.join();
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn executes_all_submitted_jobs() {
        let pool = ThreadPool::new(4);
        let log = Arc::new(Mutex::new(Vec::new()));
        for i in 0..10 {
            let log = Arc::clone(&log);
            pool.execute(move || {
                log.lock().unwrap().push(i);
            });
        }
        drop(pool); // graceful shutdown: blocks until every job has run
        let mut results = log.lock().unwrap().clone();
        results.sort();
        assert_eq!(results, (0..10).collect::<Vec<_>>());
    }

    #[test]
    fn drop_waits_for_in_flight_jobs_to_complete() {
        let pool = ThreadPool::new(2);
        let (done_tx, done_rx) = mpsc::channel();
        for i in 0..5 {
            let done_tx = done_tx.clone();
            pool.execute(move || {
                done_tx.send(i).unwrap();
            });
        }
        drop(done_tx);
        drop(pool); // must not return until all 5 jobs have sent
        let mut received: Vec<i32> = done_rx.iter().collect();
        received.sort();
        assert_eq!(received, vec![0, 1, 2, 3, 4]);
    }

    #[test]
    fn pool_can_run_many_rounds_of_work() {
        let pool = ThreadPool::new(3);
        let count = Arc::new(Mutex::new(0));
        for _ in 0..50 {
            let count = Arc::clone(&count);
            pool.execute(move || {
                *count.lock().unwrap() += 1;
            });
        }
        drop(pool);
        assert_eq!(*count.lock().unwrap(), 50);
    }
}
