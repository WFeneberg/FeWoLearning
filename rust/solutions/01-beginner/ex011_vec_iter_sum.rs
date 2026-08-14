//! Exercise 011 — Vec iterator sum/max/min (reference solution).

pub fn stats(values: &[i32]) -> (i32, Option<i32>, Option<i32>) {
    let sum = values.iter().sum();
    let max = values.iter().max().copied();
    let min = values.iter().min().copied();
    (sum, max, min)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn computes_sum_max_min() {
        assert_eq!(stats(&[3, 1, 4, 1, 5, 9]), (23, Some(9), Some(1)));
    }

    #[test]
    fn single_element() {
        assert_eq!(stats(&[7]), (7, Some(7), Some(7)));
    }

    #[test]
    fn empty_slice_has_no_max_or_min() {
        assert_eq!(stats(&[]), (0, None, None));
    }
}
