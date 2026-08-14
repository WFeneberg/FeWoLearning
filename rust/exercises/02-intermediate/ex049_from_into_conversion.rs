//! Exercise 049 — `From`/`Into` error conversion (intermediate).
//! Goal:   let `?` convert two unrelated parse errors into one application
//!         error type automatically, via `From`.
//! Drills: `impl From<X> for Y`, how `?` calls `From::from` on error types.

#[derive(Debug, PartialEq)]
pub struct AppError(pub String);

impl From<std::num::ParseIntError> for AppError {
    fn from(e: std::num::ParseIntError) -> Self {
        todo!("From<ParseIntError> for AppError: {e}")
    }
}

impl From<std::num::ParseFloatError> for AppError {
    fn from(e: std::num::ParseFloatError) -> Self {
        todo!("From<ParseFloatError> for AppError: {e}")
    }
}

/// Parses two integers and sums them, `?` converting any parse error into
/// `AppError` via the `From` impls above.
pub fn parse_and_sum(a: &str, b: &str) -> Result<i32, AppError> {
    todo!("parse_and_sum({a:?}, {b:?})")
}

/// Parses a float, converting via the same mechanism.
pub fn parse_ratio(text: &str) -> Result<f64, AppError> {
    todo!("parse_ratio({text:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sums_two_valid_integers() {
        assert_eq!(parse_and_sum("2", "3"), Ok(5));
    }

    #[test]
    fn reports_an_app_error_on_bad_int_input() {
        let err = parse_and_sum("x", "3").unwrap_err();
        assert!(err.0.contains("invalid digit"));
    }

    #[test]
    fn parses_a_valid_ratio() {
        assert_eq!(parse_ratio("3.5"), Ok(3.5));
    }

    #[test]
    fn reports_an_app_error_on_bad_float_input() {
        let err = parse_ratio("nope").unwrap_err();
        assert!(!err.0.is_empty());
    }

    #[test]
    fn into_works_the_same_direction_as_from() {
        let parse_err: std::num::ParseIntError = "x".parse::<i32>().unwrap_err();
        let app_err: AppError = parse_err.into();
        assert!(app_err.0.contains("invalid digit"));
    }
}
