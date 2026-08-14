//! Exercise 069 — `binary_search_by` and insertion points (reference solution).

pub fn find_index(sorted: &[i32], target: i32) -> Result<usize, usize> {
    sorted.binary_search(&target)
}

pub fn insertion_point(sorted: &[i32], target: i32) -> usize {
    sorted.binary_search(&target).unwrap_or_else(|i| i)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn finds_a_present_value() {
        assert_eq!(find_index(&[1, 3, 5, 7, 9], 5), Ok(2));
    }

    #[test]
    fn reports_the_insertion_point_for_a_missing_value() {
        assert_eq!(find_index(&[1, 3, 5, 7, 9], 4), Err(2));
    }

    #[test]
    fn missing_value_before_the_start_inserts_at_zero() {
        assert_eq!(find_index(&[1, 3, 5], 0), Err(0));
    }

    #[test]
    fn missing_value_after_the_end_inserts_at_the_end() {
        assert_eq!(find_index(&[1, 3, 5], 10), Err(3));
    }

    #[test]
    fn insertion_point_of_a_present_value_is_still_reported() {
        assert_eq!(insertion_point(&[1, 3, 5, 7], 5), 2);
    }

    #[test]
    fn insertion_point_of_an_absent_value_between_two_others() {
        assert_eq!(insertion_point(&[1, 3, 5, 7], 4), 2);
    }

    #[test]
    fn insertion_point_of_an_empty_slice_is_zero() {
        assert_eq!(insertion_point(&[], 42), 0);
    }
}
