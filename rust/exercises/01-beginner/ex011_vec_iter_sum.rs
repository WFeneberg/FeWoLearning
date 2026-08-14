//! Exercise 011 — Vec iterator sum/max/min (beginner).
//! Goal:   compute the sum, max, and min of a slice of integers in one pass
//!         using iterator adapters instead of manual index loops.
//! Drills: `iter`, `sum`, `max`, `min`.

pub fn stats(values: &[i32]) -> (i32, Option<i32>, Option<i32>) {
    todo!("stats({values:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn computes_sum_max_min() {
        assert_eq!(stats(&[3, 1, 4, 1, 5, 9]), (23, Some(9), Some(1)));
    }

    #[test]
    fn single_element() {
        assert_eq!(stats(&[7]), (7, Some(7), Some(7)));
    }

    #[test]
    fn empty_slice_has_no_max_or_min() {
        assert_eq!(stats(&[]), (0, None, None));
    }
}
