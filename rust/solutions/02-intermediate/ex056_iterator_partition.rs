//! Exercise 056 — `partition`, `all`, `any`, `position` (reference solution).

pub fn partition_even_odd(nums: &[i32]) -> (Vec<i32>, Vec<i32>) {
    nums.iter().partition(|&&n| n % 2 == 0)
}

pub fn all_positive(nums: &[i32]) -> bool {
    nums.iter().all(|&n| n > 0)
}

pub fn any_negative(nums: &[i32]) -> bool {
    nums.iter().any(|&n| n < 0)
}

pub fn position_of_first_negative(nums: &[i32]) -> Option<usize> {
    nums.iter().position(|&n| n < 0)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn splits_into_evens_and_odds() {
        assert_eq!(
            partition_even_odd(&[1, 2, 3, 4, 5, 6]),
            (vec![2, 4, 6], vec![1, 3, 5])
        );
    }

    #[test]
    fn partition_of_no_numbers_is_two_empty_vecs() {
        assert_eq!(partition_even_odd(&[]), (vec![], vec![]));
    }

    #[test]
    fn all_positive_is_true_only_when_every_value_is() {
        assert!(all_positive(&[1, 2, 3]));
        assert!(!all_positive(&[1, -2, 3]));
        assert!(all_positive(&[]));
    }

    #[test]
    fn any_negative_finds_a_single_negative_value() {
        assert!(any_negative(&[1, 2, -3]));
        assert!(!any_negative(&[1, 2, 3]));
        assert!(!any_negative(&[]));
    }

    #[test]
    fn position_of_first_negative_reports_the_index() {
        assert_eq!(position_of_first_negative(&[1, 2, -3, -4]), Some(2));
        assert_eq!(position_of_first_negative(&[1, 2, 3]), None);
    }
}
