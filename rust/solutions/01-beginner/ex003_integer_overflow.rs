//! Exercise 003 — Integer overflow (reference solution).

pub fn add_three_ways(a: u8, b: u8) -> (u8, Option<u8>, u8) {
    (a.wrapping_add(b), a.checked_add(b), a.saturating_add(b))
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
