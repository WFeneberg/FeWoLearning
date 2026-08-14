//! Exercise 014 — Match on literals and ranges (reference solution).

pub fn classify_magnitude(n: i32) -> &'static str {
    match n {
        i32::MIN..=-1 => "negative",
        0 => "zero",
        1..=9 => "single digit",
        10..=99 => "double digit",
        _ => "large",
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
