//! Exercise 004 — Float comparison (beginner).
//! Goal:   compare two `f64` values for "close enough" equality instead of
//!         using `==`, which fails for values like `0.1 + 0.2` vs `0.3`.
//! Drills: float precision, `f64::EPSILON`-scaled tolerance comparisons.

pub fn approx_eq(a: f64, b: f64) -> bool {
    todo!("approx_eq({a}, {b})")
}

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
