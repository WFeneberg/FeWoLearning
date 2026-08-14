//! Exercise 017 — Option::map/unwrap_or/and_then (beginner).
//! Goal:   transform `Option` values without ever matching on them manually.
//! Drills: `Option::map`, `Option::unwrap_or`, `Option::and_then`.

pub fn double_or_default(maybe: Option<i32>, default: i32) -> i32 {
    todo!("double_or_default({maybe:?}, {default})")
}

pub fn half_if_even(n: i32) -> Option<i32> {
    todo!("half_if_even({n})")
}

pub fn quarter_if_divisible_by_4(n: i32) -> Option<i32> {
    todo!("quarter_if_divisible_by_4({n})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn doubles_a_present_value() {
        assert_eq!(double_or_default(Some(5), 0), 10);
    }

    #[test]
    fn falls_back_to_default_when_absent() {
        assert_eq!(double_or_default(None, 7), 7);
    }

    #[test]
    fn halves_even_numbers_only() {
        assert_eq!(half_if_even(8), Some(4));
        assert_eq!(half_if_even(7), None);
    }

    #[test]
    fn chains_two_halvings_with_and_then() {
        assert_eq!(quarter_if_divisible_by_4(8), Some(2));
        assert_eq!(quarter_if_divisible_by_4(6), None);
        assert_eq!(quarter_if_divisible_by_4(7), None);
    }
}
