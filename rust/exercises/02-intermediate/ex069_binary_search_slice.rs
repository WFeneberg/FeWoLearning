//! Exercise 069 — `binary_search_by` and insertion points (intermediate).
//! Goal:   locate a value in a sorted slice in O(log n), and separately
//!         compute where it would go if it isn't already there.
//! Drills: `slice::binary_search`, the `Ok(index)` / `Err(insertion_point)`
//!         shape of its result.

/// Finds `target` in `sorted`, or `Err` reports the index it would be
/// inserted at to keep the slice sorted.
pub fn find_index(sorted: &[i32], target: i32) -> Result<usize, usize> {
    todo!("find_index({sorted:?}, {target})")
}

/// The index at which `target` would need to be inserted to keep `sorted`
/// in order — whether or not it's already present.
pub fn insertion_point(sorted: &[i32], target: i32) -> usize {
    todo!("insertion_point({sorted:?}, {target})")
}

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
