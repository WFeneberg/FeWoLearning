//! Exercise 055 — `take_while`, `skip_while`, `step_by` (reference solution).

pub fn take_while_positive(nums: &[i32]) -> Vec<i32> {
    nums.iter().take_while(|&&n| n > 0).copied().collect()
}

pub fn skip_leading_zeros(nums: &[i32]) -> Vec<i32> {
    nums.iter().skip_while(|&&n| n == 0).copied().collect()
}

pub fn every_nth(nums: &[i32], n: usize) -> Vec<i32> {
    nums.iter().step_by(n).copied().collect()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
