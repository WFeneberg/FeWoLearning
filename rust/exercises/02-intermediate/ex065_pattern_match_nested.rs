//! Exercise 065 — Nested destructuring and slice patterns (intermediate).
//! Goal:   match deeper than a single level: a tuple with guards, and a
//!         slice whose shape (empty / one / many) drives the branch.
//! Drills: nested tuple patterns, match guards, `[first, .., last]` slice
//!         patterns.

/// Classifies a point on the plane by which axis (if any) it sits on.
pub fn classify_point(point: (i32, i32)) -> &'static str {
    todo!("classify_point({point:?})")
}

/// Describes a slice by its shape.
pub fn describe_slice(nums: &[i32]) -> String {
    todo!("describe_slice({nums:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn origin_is_its_own_category() {
        assert_eq!(classify_point((0, 0)), "origin");
    }

    #[test]
    fn nonzero_x_with_zero_y_is_on_the_x_axis() {
        assert_eq!(classify_point((5, 0)), "x-axis");
    }

    #[test]
    fn nonzero_y_with_zero_x_is_on_the_y_axis() {
        assert_eq!(classify_point((0, -3)), "y-axis");
    }

    #[test]
    fn anything_else_is_elsewhere() {
        assert_eq!(classify_point((2, 3)), "elsewhere");
    }

    #[test]
    fn describes_an_empty_slice() {
        assert_eq!(describe_slice(&[]), "empty");
    }

    #[test]
    fn describes_a_single_element_slice() {
        assert_eq!(describe_slice(&[7]), "single: 7");
    }

    #[test]
    fn describes_first_and_last_of_a_longer_slice() {
        assert_eq!(describe_slice(&[1, 2, 3, 4]), "from 1 to 4");
    }

    #[test]
    fn describes_a_two_element_slice_by_first_and_last_too() {
        assert_eq!(describe_slice(&[9, 10]), "from 9 to 10");
    }
}
