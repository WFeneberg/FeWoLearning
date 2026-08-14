//! Exercise 093 — A mini async executor: `Waker`, task queue, polling to
//! completion (expert).
//! Goal:   a minimal, single-threaded executor good enough to drive hand-
//!         written `Future`s to completion: `spawn` boxes+pins a future into
//!         a `Task`, `run` pops tasks off a queue and polls them, and a task
//!         that returns `Poll::Pending` re-schedules ITSELF by calling
//!         `cx.waker().wake_by_ref()`, which our `Wake` impl turns into
//!         "push me back onto the queue." `run` returns once the queue is
//!         drained and every spawned future has resolved.
//! Drills: `std::task::{Wake, Waker, Context, Poll}`, a `VecDeque` task
//!         queue, `Pin<Box<dyn Future>>`, building a `Waker` from an
//!         `Arc<T: Wake>` via `Waker::from`.

use std::collections::VecDeque;
use std::future::Future;
use std::pin::Pin;
use std::sync::{Arc, Mutex};
use std::task::{Context, Poll, Wake, Waker};

type BoxFuture = Pin<Box<dyn Future<Output = ()> + Send>>;
type Queue = Arc<Mutex<VecDeque<Arc<Task>>>>;

/// One spawned future plus a handle back to the queue it re-schedules itself
/// onto when woken. `future` is `None` once the task has resolved.
struct Task {
    future: Mutex<Option<BoxFuture>>,
    queue: Queue,
}

impl Wake for Task {
    /// Called (via the `Waker` handed to `poll`) when this task should be
    /// polled again — e.g. because it returned `Poll::Pending` and then
    /// called `cx.waker().wake_by_ref()` on itself.
    fn wake(self: Arc<Self>) {
        // Clone the queue handle out first: `self.queue.lock()` would
        // otherwise keep `self` borrowed for the whole statement (the
        // `MutexGuard` temporary lives until the end of the statement),
        // which conflicts with moving `self` itself into `push_back`.
        let queue = self.queue.clone();
        queue.lock().unwrap().push_back(self);
    }
}

/// A minimal single-threaded executor: a FIFO queue of ready tasks, polled
/// one at a time until the queue drains.
pub struct Executor {
    queue: Queue,
}

impl Executor {
    pub fn new() -> Self {
        Self {
            queue: Arc::new(Mutex::new(VecDeque::new())),
        }
    }

    /// Boxes, pins, and enqueues `future` to be driven by a later `run()`.
    pub fn spawn(&self, future: impl Future<Output = ()> + Send + 'static) {
        let task = Arc::new(Task {
            future: Mutex::new(Some(Box::pin(future))),
            queue: self.queue.clone(),
        });
        self.queue.lock().unwrap().push_back(task);
    }

    /// Pops tasks off the queue and polls each one to either `Pending`
    /// (leaving it parked until it wakes itself back onto the queue) or
    /// `Ready` (dropping it). Returns once the queue is empty.
    pub fn run(&self) {
        loop {
            let task = {
                let mut queue = self.queue.lock().unwrap();
                match queue.pop_front() {
                    Some(task) => task,
                    None => break,
                }
            };

            let mut slot = task.future.lock().unwrap();
            let Some(mut future) = slot.take() else {
                // Already finished (or a stale duplicate wake) — nothing to do.
                continue;
            };

            let waker = Waker::from(Arc::clone(&task));
            let mut cx = Context::from_waker(&waker);

            match future.as_mut().poll(&mut cx) {
                Poll::Pending => {
                    *slot = Some(future);
                }
                Poll::Ready(()) => {
                    // Leave `*slot` as `None` — the task is done.
                }
            }
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    /// A hand-written future that stays `Pending` for `remaining` polls
    /// (immediately re-waking itself each time), then resolves and appends
    /// `id` to `log` — proving both the polling loop and the wake-driven
    /// re-scheduling work.
    struct CountdownFuture {
        id: u32,
        remaining: usize,
        log: Arc<Mutex<Vec<u32>>>,
    }

    impl Future for CountdownFuture {
        type Output = ();

        fn poll(mut self: Pin<&mut Self>, cx: &mut Context<'_>) -> Poll<()> {
            if self.remaining == 0 {
                self.log.lock().unwrap().push(self.id);
                Poll::Ready(())
            } else {
                self.remaining -= 1;
                cx.waker().wake_by_ref();
                Poll::Pending
            }
        }
    }

    #[test]
    fn executor_polls_a_single_task_to_completion() {
        let log = Arc::new(Mutex::new(Vec::new()));
        let exec = Executor::new();
        exec.spawn(CountdownFuture {
            id: 1,
            remaining: 3,
            log: log.clone(),
        });
        exec.run();
        assert_eq!(*log.lock().unwrap(), vec![1]);
    }

    #[test]
    fn executor_drains_multiple_tasks_to_completion() {
        let log = Arc::new(Mutex::new(Vec::new()));
        let exec = Executor::new();
        exec.spawn(CountdownFuture {
            id: 1,
            remaining: 2,
            log: log.clone(),
        });
        exec.spawn(CountdownFuture {
            id: 2,
            remaining: 0,
            log: log.clone(),
        });
        exec.spawn(CountdownFuture {
            id: 3,
            remaining: 5,
            log: log.clone(),
        });
        exec.run();
        let mut ids = log.lock().unwrap().clone();
        ids.sort();
        assert_eq!(ids, vec![1, 2, 3]);
    }

    #[test]
    fn executor_run_returns_promptly_with_no_spawned_tasks() {
        let exec = Executor::new();
        exec.run();
    }

    #[test]
    fn a_task_that_resolves_on_the_first_poll_is_polled_exactly_once() {
        let log = Arc::new(Mutex::new(Vec::new()));
        let exec = Executor::new();
        exec.spawn(CountdownFuture {
            id: 42,
            remaining: 0,
            log: log.clone(),
        });
        exec.run();
        assert_eq!(*log.lock().unwrap(), vec![42]);
    }
}
