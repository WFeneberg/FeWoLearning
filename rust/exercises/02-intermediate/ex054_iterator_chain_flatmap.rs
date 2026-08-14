//! Exercise 054 — `chain`, `flat_map`, `flatten` (intermediate).
//! Goal:   join, expand, and un-nest iterators without manual loops.
//! Drills: `Iterator::chain`, `Iterator::flat_map`, `Iterator::flatten`.

/// Chains `a` after `b`... after `a`, then doubles every value.
pub fn combine_and_double(a: &[i32], b: &[i32]) -> Vec<i32> {
    todo!("combine_and_double({a:?}, {b:?})")
}

/// Splits every sentence into its words, flattening the result into one
/// list of owned `String`s.
pub fn flatten_words(sentences: &[&str]) -> Vec<String> {
    todo!("flatten_words({sentences:?})")
}

/// Un-nests a `Vec<Vec<i32>>` into a single `Vec<i32>`.
pub fn flatten_nested(nested: Vec<Vec<i32>>) -> Vec<i32> {
    todo!("flatten_nested({nested:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn chains_then_doubles_both_slices() {
        assert_eq!(combine_and_double(&[1, 2], &[3, 4]), vec![2, 4, 6, 8]);
    }

    #[test]
    fn chaining_an_empty_slice_is_a_no_op() {
        assert_eq!(combine_and_double(&[1, 2], &[]), vec![2, 4]);
        assert_eq!(combine_and_double(&[], &[3]), vec![6]);
    }

    #[test]
    fn flat_maps_sentences_into_words() {
        let sentences = ["hello world", "foo bar baz"];
        assert_eq!(
            flatten_words(&sentences),
            vec!["hello", "world", "foo", "bar", "baz"]
        );
    }

    #[test]
    fn flattens_a_nested_vec() {
        let nested = vec![vec![1, 2], vec![], vec![3, 4, 5]];
        assert_eq!(flatten_nested(nested), vec![1, 2, 3, 4, 5]);
    }

    #[test]
    fn flattens_an_empty_nested_vec() {
        let nested: Vec<Vec<i32>> = vec![];
        assert!(flatten_nested(nested).is_empty());
    }
}
