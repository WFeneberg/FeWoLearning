//! Exercise 014 — Match on literals and ranges (beginner).
//! Goal:   classify an integer into a size bucket using `match` with literal
//!         and range patterns.
//! Drills: `match` on integers, inclusive range patterns (`1..=9`),
//!         exhaustiveness via a catch-all `_`.

pub fn classify_magnitude(n: i32) -> &'static str {
    todo!("classify_magnitude({n})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn classifies_negative_and_zero() {
        assert_eq!(classify_magnitude(-5), "negative");
        assert_eq!(classify_magnitude(0), "zero");
    }

    #[test]
    fn classifies_single_and_double_digits() {
        assert_eq!(classify_magnitude(7), "single digit");
        assert_eq!(classify_magnitude(42), "double digit");
    }

    #[test]
    fn classifies_large_numbers() {
        assert_eq!(classify_magnitude(1000), "large");
    }
}
