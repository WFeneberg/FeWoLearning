//! Exercise 026 — Immutable borrows (reference solution).

pub fn total_length(strings: &[String]) -> usize {
    strings.iter().map(|s| s.len()).sum()
}

pub fn longest<'a>(strings: &'a [String]) -> Option<&'a str> {
    strings
        .iter()
        .max_by_key(|s| s.len())
        .map(|s| s.as_str())
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sums_the_length_of_every_string() {
        let strings = vec!["ab".to_string(), "cde".to_string()];
        // Two immutable borrows of `strings` alive in the same expression.
        assert_eq!(total_length(&strings) + longest(&strings).unwrap().len(), 8);
    }

    #[test]
    fn finds_the_longest_string() {
        let strings = vec!["a".to_string(), "abc".to_string(), "ab".to_string()];
        assert_eq!(longest(&strings), Some("abc"));
    }

    #[test]
    fn empty_slice_has_no_longest_and_zero_length() {
        let strings: Vec<String> = vec![];
        assert_eq!(total_length(&strings), 0);
        assert_eq!(longest(&strings), None);
    }
}
