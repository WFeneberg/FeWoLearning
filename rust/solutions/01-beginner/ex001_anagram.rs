//! Exercise 001 — Anagram (reference solution).

pub fn is_anagram(a: &str, b: &str) -> bool {
    normalize(a) == normalize(b)
}

fn normalize(s: &str) -> Vec<char> {
    let mut chars: Vec<char> = s
        .chars()
        .filter(|c| !c.is_whitespace())
        .flat_map(|c| c.to_lowercase())
        .collect();
    chars.sort_unstable();
    chars
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
