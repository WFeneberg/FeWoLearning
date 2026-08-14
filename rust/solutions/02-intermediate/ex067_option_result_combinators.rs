//! Exercise 067 — Chaining combinators instead of `unwrap` (reference solution).

use std::collections::HashMap;

pub fn discounted_price(price: Option<f64>, discount_pct: Option<f64>) -> Option<f64> {
    price
        .zip(discount_pct)
        .map(|(p, d)| p * (1.0 - d / 100.0))
        .filter(|&p| p >= 0.0)
}

pub fn lookup_and_double(map: &HashMap<String, i32>, key: &str) -> Result<i32, String> {
    map.get(key)
        .copied()
        .map(|v| v * 2)
        .ok_or_else(|| format!("missing key: {key}"))
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
