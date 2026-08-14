//! Exercise 080 — Why `Send` and `Sync` bounds exist (advanced).
//! Goal:   implement a generic helper whose signature *requires* `Send` and
//!         `Sync` because it genuinely shares one value across threads, and
//!         see, in the doc comment below, which everyday types would fail
//!         those bounds and why.
//! Drills: `Send` (safe to move to another thread), `Sync` (safe to share
//!         `&T` across threads), `Arc<T>` requiring `T: Send + Sync` to
//!         itself be `Send + Sync`.
//!
//! Some types are deliberately **not** `Send`/`Sync`:
//! - `Rc<T>` is neither: its reference count isn't atomic, so two threads
//!   cloning/dropping the same `Rc` concurrently could corrupt the count.
//!   (`Arc<T>` fixes this with an atomic count.)
//! - `RefCell<T>` is `Send` (if `T: Send`) but not `Sync`: its borrow flag
//!   isn't atomic either, so two threads calling `borrow_mut()` on a shared
//!   `&RefCell<T>` at the same time could both believe they hold the only
//!   borrow. (`Mutex<T>`/`RwLock<T>` fix this with real synchronization.)
//! - A raw pointer (`*const T` / `*mut T`) is neither, by default: the
//!   compiler has no way to know whether the pointee's aliasing is safe to
//!   share, so it conservatively assumes neither.
//! These aren't opt-outs a learner can silence — they fall out of each
//! type's fields automatically, which is exactly why `run_on_each_thread`
//! below needs `T: Send + Sync` in its signature: an `Arc<T>` is only
//! `Send`/`Sync` itself when `T` is both.

use std::sync::Arc;
use std::thread;

/// Runs `f(&shared)` once per thread across `thread_count` threads, all
/// looking at the *same* `T` value through cloned `Arc` handles, and
/// collects the results in thread-spawn order.
///
/// `T: Send + Sync` because the same `T` is read from multiple threads at
/// once (`Sync`) and the `Arc<T>` clones themselves move into new threads
/// (which itself requires `Arc<T>: Send`, which requires `T: Send + Sync`).
/// `F: Send + Sync` because the same closure is invoked from every thread.
pub fn run_on_each_thread<T, F, R>(shared: Arc<T>, thread_count: usize, f: F) -> Vec<R>
where
    T: Send + Sync + 'static,
    F: Fn(&T) -> R + Send + Sync + 'static,
    R: Send + 'static,
{
    todo!("spawn thread_count threads (each cloning `shared` and an Arc'd `f`), join all, collect in order")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn runs_the_closure_on_every_thread_and_collects_results() {
        let shared = Arc::new(vec![1, 2, 3, 4]);
        let results = run_on_each_thread(shared, 3, |data: &Vec<i32>| data.iter().sum::<i32>());
        assert_eq!(results, vec![10, 10, 10]);
    }

    #[test]
    fn zero_threads_produces_no_results() {
        let shared = Arc::new(42);
        let results = run_on_each_thread(shared, 0, |v: &i32| *v);
        assert!(results.is_empty());
    }

    #[test]
    fn each_thread_sees_the_same_shared_value() {
        let shared = Arc::new(String::from("hi"));
        let results = run_on_each_thread(Arc::clone(&shared), 5, |s: &String| s.len());
        assert_eq!(results, vec![2, 2, 2, 2, 2]);
    }
}
