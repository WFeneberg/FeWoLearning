//! Exercise 031 — Iterator zip/enumerate/rev (reference solution).

pub fn zip_sum(a: &[i32], b: &[i32]) -> Vec<i32> {
    a.iter().zip(b.iter()).map(|(x, y)| x + y).collect()
}

pub fn label_in_reverse(items: &[&str]) -> Vec<String> {
    items
        .iter()
        .enumerate()
        .rev()
        .map(|(i, item)| format!("{i}: {item}"))
        .collect()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
