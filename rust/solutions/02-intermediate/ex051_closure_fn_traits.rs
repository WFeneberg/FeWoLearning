//! Exercise 051 — `Fn`, `FnMut`, `FnOnce` (reference solution).

pub fn apply_fn<F: Fn(i32) -> i32>(f: F, x: i32) -> i32 {
    f(x)
}

pub fn collect_from_fn_mut<F: FnMut() -> i32>(mut f: F, times: usize) -> Vec<i32> {
    (0..times).map(|_| f()).collect()
}

pub fn apply_fn_once<F: FnOnce() -> String>(f: F) -> String {
    f()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
