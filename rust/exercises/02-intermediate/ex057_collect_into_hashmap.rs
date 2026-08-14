//! Exercise 057 — Collecting into `HashMap`/`HashSet`/`String` (intermediate).
//! Goal:   let `collect()`'s return-type inference build three different
//!         collections from the same kind of iterator pipeline.
//! Drills: `collect::<HashMap<_, _>>()`, `collect::<HashSet<_>>()`,
//!         `collect::<String>()`.

use std::collections::{HashMap, HashSet};

/// Maps each word to its length.
pub fn word_lengths(words: &[&str]) -> HashMap<String, usize> {
    todo!("word_lengths({words:?})")
}

/// Every distinct letter that appears in `text` (case-sensitive).
pub fn unique_letters(text: &str) -> HashSet<char> {
    todo!("unique_letters({text:?})")
}

/// Upper-cases every word and joins them back with spaces, built purely by
/// collecting into a `String`.
pub fn shout(words: &[&str]) -> String {
    todo!("shout({words:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn maps_words_to_their_length() {
        let lengths = word_lengths(&["a", "bb", "ccc"]);
        assert_eq!(lengths.get("a"), Some(&1));
        assert_eq!(lengths.get("bb"), Some(&2));
        assert_eq!(lengths.get("ccc"), Some(&3));
        assert_eq!(lengths.len(), 3);
    }

    #[test]
    fn word_lengths_of_no_words_is_empty() {
        assert!(word_lengths(&[]).is_empty());
    }

    #[test]
    fn collects_distinct_letters() {
        let mut letters: Vec<char> = unique_letters("banana").into_iter().collect();
        letters.sort_unstable();
        assert_eq!(letters, vec!['a', 'b', 'n']);
    }

    #[test]
    fn shout_upper_cases_and_rejoins() {
        assert_eq!(shout(&["hello", "world"]), "HELLO WORLD");
    }

    #[test]
    fn shout_of_no_words_is_empty() {
        assert_eq!(shout(&[]), "");
    }
}
