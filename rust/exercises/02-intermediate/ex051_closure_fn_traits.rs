//! Exercise 051 — `Fn`, `FnMut`, `FnOnce` (intermediate).
//! Goal:   write three small generic functions, one per closure trait, that
//!         show what each one allows: read-only calls, calls that mutate a
//!         capture, and a call that consumes an owned capture.
//! Drills: `Fn(T) -> R`, `FnMut() -> R`, `FnOnce() -> R`, captures by
//!         reference, by mutable reference, and by move.

/// Calls a read-only closure once and returns its result.
pub fn apply_fn<F: Fn(i32) -> i32>(f: F, x: i32) -> i32 {
    todo!("apply_fn(_, {x})")
}

/// Calls a mutating closure `times` times, collecting each result. Useful
/// with a closure that captures a counter by mutable reference.
pub fn collect_from_fn_mut<F: FnMut() -> i32>(mut f: F, times: usize) -> Vec<i32> {
    todo!("collect_from_fn_mut(_, {times})")
}

/// Calls a closure that consumes its capture, exactly once.
pub fn apply_fn_once<F: FnOnce() -> String>(f: F) -> String {
    todo!("apply_fn_once(_)")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn fn_reads_its_captures_without_consuming_them() {
        let factor = 3;
        let double_and_scale = |x: i32| x * factor;
        assert_eq!(apply_fn(double_and_scale, 5), 15);
        // `factor` is still usable — `Fn` only borrowed it.
        assert_eq!(factor, 3);
    }

    #[test]
    fn fn_mut_can_mutate_its_capture_across_calls() {
        let mut counter = 0;
        let results = collect_from_fn_mut(
            || {
                counter += 1;
                counter
            },
            4,
        );
        assert_eq!(results, vec![1, 2, 3, 4]);
    }

    #[test]
    fn fn_mut_of_zero_calls_is_empty() {
        let mut counter = 10;
        let results = collect_from_fn_mut(
            || {
                counter += 1;
                counter
            },
            0,
        );
        assert!(results.is_empty());
    }

    #[test]
    fn fn_once_can_consume_an_owned_capture() {
        let owned = String::from("hello");
        let shout = move || owned.to_uppercase();
        assert_eq!(apply_fn_once(shout), "HELLO");
    }
}
