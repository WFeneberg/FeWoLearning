//! Exercise 048 — Wrapping a lower-level error (intermediate).
//! Goal:   wrap a `ParseIntError` in your own error type and expose it
//!         through `source()` so callers can still see the original cause.
//! Drills: `std::error::Error::source`, error wrapping, `Option<&(dyn
//!         Error + 'static)>`.

#[derive(Debug, PartialEq)]
pub struct ParseConfigError {
    pub source: std::num::ParseIntError,
}

impl std::fmt::Display for ParseConfigError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "failed to parse config value")
    }
}

impl std::error::Error for ParseConfigError {
    fn source(&self) -> Option<&(dyn std::error::Error + 'static)> {
        todo!("source() for {self:?}")
    }
}

/// Parses a port number, wrapping any parse failure in `ParseConfigError`.
pub fn parse_port(input: &str) -> Result<u16, ParseConfigError> {
    todo!("parse_port({input:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_a_valid_port() {
        assert_eq!(parse_port("8080"), Ok(8080));
    }

    #[test]
    fn wraps_the_parse_failure() {
        let err = parse_port("not-a-port").unwrap_err();
        assert_eq!(err.to_string(), "failed to parse config value");
    }

    #[test]
    fn exposes_the_original_error_as_source() {
        use std::error::Error;
        let err = parse_port("not-a-port").unwrap_err();
        let source = err.source().expect("should have a source");
        assert!(source.to_string().contains("invalid digit"));
    }

    #[test]
    fn out_of_range_port_also_fails_to_parse() {
        // u16 overflow at parse time is itself a ParseIntError.
        let err = parse_port("99999999").unwrap_err();
        use std::error::Error;
        assert!(err.source().is_some());
    }
}
