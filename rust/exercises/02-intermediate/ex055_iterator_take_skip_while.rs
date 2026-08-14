//! Exercise 055 — `take_while`, `skip_while`, `step_by` (intermediate).
//! Goal:   slice a sequence by a predicate or a stride instead of an index.
//! Drills: `Iterator::take_while`, `Iterator::skip_while`,
//!         `Iterator::step_by`.

/// Takes values from the front while they stay positive, stopping at the
/// first value that is zero or negative.
pub fn take_while_positive(nums: &[i32]) -> Vec<i32> {
    todo!("take_while_positive({nums:?})")
}

/// Drops leading zeros, keeping everything from the first non-zero value on.
pub fn skip_leading_zeros(nums: &[i32]) -> Vec<i32> {
    todo!("skip_leading_zeros({nums:?})")
}

/// Every `n`-th element, starting from the first.
pub fn every_nth(nums: &[i32], n: usize) -> Vec<i32> {
    todo!("every_nth({nums:?}, {n})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn stops_at_the_first_non_positive_value() {
        assert_eq!(take_while_positive(&[1, 2, 3, -1, 5]), vec![1, 2, 3]);
    }

    #[test]
    fn take_while_positive_of_all_positive_is_everything() {
        assert_eq!(take_while_positive(&[1, 2, 3]), vec![1, 2, 3]);
    }

    #[test]
    fn drops_leading_zeros_only() {
        assert_eq!(skip_leading_zeros(&[0, 0, 1, 0, 2]), vec![1, 0, 2]);
    }

    #[test]
    fn skip_leading_zeros_of_no_zeros_is_unchanged() {
        assert_eq!(skip_leading_zeros(&[5, 6, 7]), vec![5, 6, 7]);
    }

    #[test]
    fn picks_every_second_element() {
        assert_eq!(every_nth(&[1, 2, 3, 4, 5, 6], 2), vec![1, 3, 5]);
    }

    #[test]
    fn picks_every_third_element() {
        assert_eq!(every_nth(&[10, 20, 30, 40, 50, 60, 70], 3), vec![10, 40, 70]);
    }
}
