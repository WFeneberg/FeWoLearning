//! Exercise 033 — Vec dedup & retain (beginner).
//! Goal:   collapse consecutive duplicate values, then drop every negative
//!         number that remains — both in place.
//! Drills: `Vec::dedup`, `Vec::retain`.

pub fn clean_up(values: &mut Vec<i32>) {
    todo!("clean_up({values:?})")
}

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
