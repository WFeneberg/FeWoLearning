//! Exercise 016 — if let / while let (beginner).
//! Goal:   describe a slice's first element with `if let`, and drain a stack
//!         of its trailing positive numbers with `while let`, stopping (and
//!         discarding) at the first non-positive value found.
//! Drills: `if let` with `else`, `while let Some(...) = stack.pop()`.

pub fn describe_first(values: &[i32]) -> String {
    todo!("describe_first({values:?})")
}

pub fn drain_positive(stack: &mut Vec<i32>) -> Vec<i32> {
    todo!("drain_positive({stack:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn describes_first_element() {
        assert_eq!(describe_first(&[5, 6]), "first is 5");
    }

    #[test]
    fn describes_empty_slice() {
        assert_eq!(describe_first(&[]), "empty");
    }

    #[test]
    fn drains_all_positive_values() {
        let mut stack = vec![1, 2, 3];
        assert_eq!(drain_positive(&mut stack), vec![3, 2, 1]);
        assert!(stack.is_empty());
    }

    #[test]
    fn stops_at_first_non_positive_value() {
        let mut stack = vec![1, -2, 3];
        assert_eq!(drain_positive(&mut stack), vec![3]);
        assert_eq!(stack, vec![1]);
    }
}
