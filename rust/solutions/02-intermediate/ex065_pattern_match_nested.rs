//! Exercise 065 — Nested destructuring and slice patterns (reference solution).

pub fn classify_point(point: (i32, i32)) -> &'static str {
    match point {
        (0, 0) => "origin",
        (x, 0) if x != 0 => "x-axis",
        (0, y) if y != 0 => "y-axis",
        _ => "elsewhere",
    }
}

pub fn describe_slice(nums: &[i32]) -> String {
    match nums {
        [] => "empty".to_string(),
        [x] => format!("single: {x}"),
        [first, .., last] => format!("from {first} to {last}"),
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
