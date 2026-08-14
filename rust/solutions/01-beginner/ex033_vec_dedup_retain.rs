//! Exercise 033 — Vec dedup & retain (reference solution).

pub fn clean_up(values: &mut Vec<i32>) {
    values.dedup();
    values.retain(|&v| v >= 0);
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn dedups_consecutive_values_then_drops_negatives() {
        let mut values = vec![1, 1, 2, 2, -3, 3, 3, -1, 4];
        clean_up(&mut values);
        assert_eq!(values, vec![1, 2, 3, 4]);
    }

    #[test]
    fn non_consecutive_duplicates_are_kept() {
        let mut values = vec![1, 2, 1];
        clean_up(&mut values);
        assert_eq!(values, vec![1, 2, 1]);
    }

    #[test]
    fn empty_vec_stays_empty() {
        let mut values: Vec<i32> = vec![];
        clean_up(&mut values);
        assert_eq!(values, Vec::<i32>::new());
    }
}
