//! Exercise 026 — Immutable borrows (beginner).
//! Goal:   read the same slice of `String`s through two independent `&`
//!         borrows at once — any number of immutable readers may coexist as
//!         long as nothing is writing.
//! Drills: `&T`, borrowing without taking ownership, lifetimes on returned
//!         references.

pub fn total_length(strings: &[String]) -> usize {
    todo!("total_length({strings:?})")
}

pub fn longest<'a>(strings: &'a [String]) -> Option<&'a str> {
    todo!("longest({strings:?})")
}

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
