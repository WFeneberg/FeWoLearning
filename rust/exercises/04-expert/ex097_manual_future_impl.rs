//! Exercise 097 — Implementing `Future` by hand: `Poll`, state machines
//! (expert).
//! Goal:   `Countdown`, a `Future` written by hand (no `async fn` anywhere)
//!         whose `poll` method is an explicit state machine: it starts in
//!         `State::Counting(n)`, returns `Poll::Pending` and decrements `n`
//!         each time it's polled while `n > 0`, then transitions to
//!         `State::Done` and returns `Poll::Ready(value)` exactly once.
//!         Polling it again afterwards is a caller bug (it panics) — this
//!         mirrors exactly the shape an `async fn` with `.await` points
//!         desugars into behind the scenes; here you write the state
//!         machine by hand instead of letting the compiler generate it.
//! Drills: `impl Future for T`, `Poll::{Pending, Ready}`, `Pin<&mut Self>`,
//!         driving a future to completion using nothing but manual `poll`
//!         calls and a no-op `Waker` (`Waker::noop`) — no executor needed.

use std::future::Future;
use std::pin::Pin;
use std::task::{Context, Poll};

enum State {
    Counting(u32),
    Done,
}

/// A future that must be polled `initial_count + 1` times: the first
/// `initial_count` polls return `Pending`, and the final poll returns
/// `Ready(value)`.
pub struct Countdown {
    state: State,
    value: i32,
}

impl Countdown {
    pub fn new(initial_count: u32, value: i32) -> Self {
        Self {
            state: State::Counting(initial_count),
            value,
        }
    }
}

impl Future for Countdown {
    type Output = i32;

    fn poll(mut self: Pin<&mut Self>, _cx: &mut Context<'_>) -> Poll<i32> {
        todo!(
            "match self.state: State::Done -> this is a caller bug, panic! with a clear \
             message; State::Counting(0) -> set self.state = State::Done and return \
             Poll::Ready(self.value); State::Counting(n) (n > 0) -> set self.state = \
             State::Counting(n - 1) and return Poll::Pending"
        )
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::task::Waker;

    /// Polls `future` exactly once using a no-op `Waker` — enough to drive a
    /// future manually without needing a real executor.
    fn poll_once<F: Future + Unpin>(future: &mut F) -> Poll<F::Output> {
        let waker = Waker::noop();
        let mut cx = Context::from_waker(waker);
        Pin::new(future).poll(&mut cx)
    }

    #[test]
    fn a_zero_count_countdown_resolves_on_the_first_poll() {
        let mut countdown = Countdown::new(0, 42);
        assert_eq!(poll_once(&mut countdown), Poll::Ready(42));
    }

    #[test]
    fn countdown_returns_pending_exactly_n_times_then_resolves() {
        let mut countdown = Countdown::new(3, 99);
        assert_eq!(poll_once(&mut countdown), Poll::Pending);
        assert_eq!(poll_once(&mut countdown), Poll::Pending);
        assert_eq!(poll_once(&mut countdown), Poll::Pending);
        assert_eq!(poll_once(&mut countdown), Poll::Ready(99));
    }

    #[test]
    fn a_larger_count_still_resolves_with_the_right_value() {
        let mut countdown = Countdown::new(10, -7);
        for _ in 0..10 {
            assert_eq!(poll_once(&mut countdown), Poll::Pending);
        }
        assert_eq!(poll_once(&mut countdown), Poll::Ready(-7));
    }

    #[test]
    #[should_panic(expected = "polled after completion")]
    fn polling_after_completion_panics() {
        let mut countdown = Countdown::new(0, 1);
        let _ = poll_once(&mut countdown);
        let _ = poll_once(&mut countdown); // state is Done now — must panic
    }
}
