//! Exercise 057 — Collecting into `HashMap`/`HashSet`/`String` (reference solution).

use std::collections::{HashMap, HashSet};

pub fn word_lengths(words: &[&str]) -> HashMap<String, usize> {
    words.iter().map(|w| (w.to_string(), w.len())).collect()
}

pub fn unique_letters(text: &str) -> HashSet<char> {
    text.chars().collect()
}

pub fn shout(words: &[&str]) -> String {
    words
        .iter()
        .map(|w| w.to_uppercase())
        .collect::<Vec<_>>()
        .join(" ")
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
