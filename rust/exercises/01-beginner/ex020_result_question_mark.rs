//! Exercise 020 — The ? operator (beginner).
//! Goal:   sum a list of number strings, propagating the first parse error
//!         with `?` instead of matching on every call.
//! Drills: the `?` operator in a function returning `Result`.

use std::num::ParseIntError;

pub fn sum_parsed(parts: &[&str]) -> Result<i32, ParseIntError> {
    todo!("sum_parsed({parts:?})")
}

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
