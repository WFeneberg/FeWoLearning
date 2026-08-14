//! Exercise 003 — Integer overflow (beginner).
//! Goal:   add two `u8` values three different ways and report all three
//!         results, showing how overflow behaves differently depending on
//!         which arithmetic method is used.
//! Drills: `wrapping_add`, `checked_add`, `saturating_add`.

pub fn add_three_ways(a: u8, b: u8) -> (u8, Option<u8>, u8) {
    todo!("add_three_ways({a}, {b})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn no_overflow_all_agree() {
        assert_eq!(add_three_ways(1, 2), (3, Some(3), 3));
    }

    #[test]
    fn overflow_wraps_saturates_and_is_none() {
        assert_eq!(add_three_ways(250, 10), (4, None, 255));
    }

    #[test]
    fn overflow_at_the_edge() {
        assert_eq!(add_three_ways(255, 1), (0, None, 255));
    }
}
