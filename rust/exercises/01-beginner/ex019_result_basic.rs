//! Exercise 019 — Result basics (beginner).
//! Goal:   parse a string as a positive whole number, reporting distinct
//!         error messages for "not a number" vs. "not positive".
//! Drills: `Result`, `match` on the `Err` variant, `Result::is_ok`.

pub fn parse_positive(s: &str) -> Result<u32, String> {
    todo!("parse_positive({s:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_a_valid_positive_number() {
        assert_eq!(parse_positive("42"), Ok(42));
    }

    #[test]
    fn rejects_non_numeric_input() {
        let result = parse_positive("abc");
        assert!(!result.is_ok());
        assert_eq!(result, Err("not a number".to_string()));
    }

    #[test]
    fn rejects_zero_as_not_positive() {
        match parse_positive("0") {
            Err(msg) => assert_eq!(msg, "must be positive"),
            Ok(_) => panic!("expected an error"),
        }
    }
}
