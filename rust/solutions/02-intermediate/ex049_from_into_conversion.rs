//! Exercise 049 — `From`/`Into` error conversion (reference solution).

#[derive(Debug, PartialEq)]
pub struct AppError(pub String);

impl From<std::num::ParseIntError> for AppError {
    fn from(e: std::num::ParseIntError) -> Self {
        AppError(e.to_string())
    }
}

impl From<std::num::ParseFloatError> for AppError {
    fn from(e: std::num::ParseFloatError) -> Self {
        AppError(e.to_string())
    }
}

pub fn parse_and_sum(a: &str, b: &str) -> Result<i32, AppError> {
    let x: i32 = a.parse()?;
    let y: i32 = b.parse()?;
    Ok(x + y)
}

pub fn parse_ratio(text: &str) -> Result<f64, AppError> {
    let value: f64 = text.parse()?;
    Ok(value)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
