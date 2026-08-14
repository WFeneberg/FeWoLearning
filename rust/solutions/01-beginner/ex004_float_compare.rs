//! Exercise 004 — Float comparison (reference solution).

pub fn approx_eq(a: f64, b: f64) -> bool {
    (a - b).abs() < f64::EPSILON * 8.0
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn classic_rounding_error_is_approx_equal() {
        assert!(approx_eq(0.1 + 0.2, 0.3));
    }

    #[test]
    fn exactly_equal_values_are_approx_equal() {
        assert!(approx_eq(2.5, 2.5));
    }

    #[test]
    fn clearly_different_values_are_not_approx_equal() {
        assert!(!approx_eq(1.0, 1.0001));
    }
}
