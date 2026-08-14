//! Exercise 012 — HashMap word count (reference solution).

use std::collections::HashMap;

pub fn word_count(text: &str) -> HashMap<String, u32> {
    let mut counts = HashMap::new();
    for word in text.split_whitespace() {
        *counts.entry(word.to_string()).or_insert(0) += 1;
    }
    counts
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn counts_repeated_words() {
        let counts = word_count("the cat sat on the mat");
        let mut expected = HashMap::new();
        expected.insert("the".to_string(), 2);
        expected.insert("cat".to_string(), 1);
        expected.insert("sat".to_string(), 1);
        expected.insert("on".to_string(), 1);
        expected.insert("mat".to_string(), 1);
        assert_eq!(counts, expected);
    }

    #[test]
    fn empty_text_gives_empty_map() {
        assert_eq!(word_count("").len(), 0);
    }
}
