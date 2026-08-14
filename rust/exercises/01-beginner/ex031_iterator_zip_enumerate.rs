//! Exercise 031 — Iterator zip/enumerate/rev (beginner).
//! Goal:   pair up two slices element-wise, and separately label a list of
//!         items with their index, listed back to front.
//! Drills: `zip` (stopping at the shorter iterator), `enumerate`, `rev`.

pub fn zip_sum(a: &[i32], b: &[i32]) -> Vec<i32> {
    todo!("zip_sum({a:?}, {b:?})")
}

pub fn label_in_reverse(items: &[&str]) -> Vec<String> {
    todo!("label_in_reverse({items:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn zips_and_sums_pairwise() {
        assert_eq!(zip_sum(&[1, 2, 3], &[10, 20]), vec![11, 22]);
    }

    #[test]
    fn zip_sum_of_empty_is_empty() {
        assert_eq!(zip_sum(&[], &[1, 2]), Vec::<i32>::new());
    }

    #[test]
    fn labels_items_from_last_index_to_first() {
        assert_eq!(
            label_in_reverse(&["a", "b", "c"]),
            vec!["2: c".to_string(), "1: b".to_string(), "0: a".to_string()]
        );
    }
}
