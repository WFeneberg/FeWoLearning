//! Exercise 067 — Chaining combinators instead of `unwrap` (intermediate).
//! Goal:   thread two independent `Option`s and a `HashMap` lookup through
//!         to a final answer without ever calling `.unwrap()`.
//! Drills: `Option::zip`, `Option::map`, `Option::filter`,
//!         `Option::ok_or_else`.

use std::collections::HashMap;

/// Combines a price and a discount percentage, if both are present, and
/// only keeps the result if it didn't go negative.
pub fn discounted_price(price: Option<f64>, discount_pct: Option<f64>) -> Option<f64> {
    todo!("discounted_price({price:?}, {discount_pct:?})")
}

/// Looks `key` up in `map`, doubles it, or reports a descriptive error.
pub fn lookup_and_double(map: &HashMap<String, i32>, key: &str) -> Result<i32, String> {
    todo!("lookup_and_double(_, {key:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn applies_the_discount_when_both_are_present() {
        assert_eq!(discounted_price(Some(100.0), Some(25.0)), Some(75.0));
    }

    #[test]
    fn missing_price_yields_none() {
        assert_eq!(discounted_price(None, Some(10.0)), None);
    }

    #[test]
    fn missing_discount_yields_none() {
        assert_eq!(discounted_price(Some(50.0), None), None);
    }

    #[test]
    fn a_discount_over_a_hundred_percent_is_filtered_out() {
        assert_eq!(discounted_price(Some(50.0), Some(150.0)), None);
    }

    #[test]
    fn lookup_and_double_finds_and_doubles_a_value() {
        let mut map = HashMap::new();
        map.insert("a".to_string(), 21);
        assert_eq!(lookup_and_double(&map, "a"), Ok(42));
    }

    #[test]
    fn lookup_and_double_reports_a_missing_key() {
        let map: HashMap<String, i32> = HashMap::new();
        assert_eq!(lookup_and_double(&map, "missing"), Err("missing key: missing".to_string()));
    }
}
