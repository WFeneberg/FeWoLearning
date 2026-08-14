//! Exercise 080 — Why `Send` and `Sync` bounds exist (reference solution).

use std::sync::Arc;
use std::thread;

pub fn run_on_each_thread<T, F, R>(shared: Arc<T>, thread_count: usize, f: F) -> Vec<R>
where
    T: Send + Sync + 'static,
    F: Fn(&T) -> R + Send + Sync + 'static,
    R: Send + 'static,
{
    let f = Arc::new(f);
    let handles: Vec<_> = (0..thread_count)
        .map(|_| {
            let shared = Arc::clone(&shared);
            let f = Arc::clone(&f);
            thread::spawn(move || f(&shared))
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
