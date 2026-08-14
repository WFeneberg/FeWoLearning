//! Exercise 016 — if let / while let (reference solution).

pub fn describe_first(values: &[i32]) -> String {
    if let Some(&first) = values.first() {
        format!("first is {first}")
    } else {
        "empty".to_string()
    }
}

pub fn drain_positive(stack: &mut Vec<i32>) -> Vec<i32> {
    let mut result = Vec::new();
    while let Some(top) = stack.pop() {
        if top > 0 {
            result.push(top);
        } else {
            break;
        }
    }
    result
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
