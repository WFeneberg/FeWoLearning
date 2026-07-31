//! Exercise 001 — Anagram (beginner).
//! Goal:   true if `a` and `b` are anagrams, case-insensitive, ignoring spaces.
//! Drills: iterators, char handling, sorting.

pub fn is_anagram(a: &str, b: &str) -> bool {
    todo!("implement is_anagram for {a:?} and {b:?}")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn detects_anagrams() {
        assert!(is_anagram("listen", "silent"));
        assert!(is_anagram("Dormitory", "dirty room"));
    }

    #[test]
    fn rejects_non_anagrams() {
        assert!(!is_anagram("hello", "world"));
        assert!(!is_anagram("abc", "abcd"));
    }
}
