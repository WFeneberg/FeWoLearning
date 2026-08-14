//! Exercise 020 — The ? operator (reference solution).

use std::num::ParseIntError;

pub fn sum_parsed(parts: &[&str]) -> Result<i32, ParseIntError> {
    let mut total = 0;
    for part in parts {
        total += part.parse::<i32>()?;
    }
    Ok(total)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sums_valid_numbers() {
        assert_eq!(sum_parsed(&["1", "2", "3"]), Ok(6));
    }

    #[test]
    fn propagates_the_first_parse_error() {
        assert!(sum_parsed(&["1", "x", "3"]).is_err());
    }

    #[test]
    fn empty_list_sums_to_zero() {
        assert_eq!(sum_parsed(&[]), Ok(0));
    }
}
