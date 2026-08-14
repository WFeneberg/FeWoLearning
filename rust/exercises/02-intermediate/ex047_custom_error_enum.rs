//! Exercise 047 — A custom error enum (intermediate).
//! Goal:   model the ways parsing can fail as an enum, and make it a real
//!         error type via `Display` + `std::error::Error`.
//! Drills: error enums, `impl std::fmt::Display`, `impl std::error::Error`.

#[derive(Debug, PartialEq)]
pub enum ParseAgeError {
    Empty,
    NotANumber(String),
    OutOfRange(i32),
}

impl std::fmt::Display for ParseAgeError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            ParseAgeError::Empty => write!(f, "age string is empty"),
            ParseAgeError::NotANumber(s) => write!(f, "{s:?} is not a number"),
            ParseAgeError::OutOfRange(n) => write!(f, "{n} is out of range for a human age"),
        }
    }
}

impl std::error::Error for ParseAgeError {}

/// Parses an age from `input`, rejecting empty strings, non-numeric text,
/// and values outside `0..=150`.
pub fn parse_age(input: &str) -> Result<u8, ParseAgeError> {
    todo!("parse_age({input:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_a_valid_age() {
        assert_eq!(parse_age("25"), Ok(25));
    }

    #[test]
    fn rejects_an_empty_string() {
        assert_eq!(parse_age(""), Err(ParseAgeError::Empty));
    }

    #[test]
    fn rejects_non_numeric_text() {
        assert_eq!(parse_age("abc"), Err(ParseAgeError::NotANumber("abc".to_string())));
    }

    #[test]
    fn rejects_an_out_of_range_age() {
        assert_eq!(parse_age("200"), Err(ParseAgeError::OutOfRange(200)));
    }

    #[test]
    fn error_messages_are_human_readable() {
        assert_eq!(parse_age("").unwrap_err().to_string(), "age string is empty");
        assert_eq!(
            parse_age("200").unwrap_err().to_string(),
            "200 is out of range for a human age"
        );
    }

    #[test]
    fn it_is_usable_as_a_trait_object_error() {
        fn accepts_any_error(_: &dyn std::error::Error) {}
        let err = parse_age("abc").unwrap_err();
        accepts_any_error(&err);
    }
}
