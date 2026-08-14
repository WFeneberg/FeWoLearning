//! Exercise 019 — Result basics (reference solution).

pub fn parse_positive(s: &str) -> Result<u32, String> {
    let n: u32 = match s.parse() {
        Ok(n) => n,
        Err(_) => return Err("not a number".to_string()),
    };

    if n == 0 {
        Err("must be positive".to_string())
    } else {
        Ok(n)
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
