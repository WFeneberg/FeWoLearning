//! Exercise 012 — HashMap word count (beginner).
//! Goal:   count how many times each whitespace-delimited word appears in a
//!         piece of text.
//! Drills: `HashMap`, the `entry` API, counting with `or_insert`.

use std::collections::HashMap;

pub fn word_count(text: &str) -> HashMap<String, u32> {
    todo!("word_count({text:?})")
}

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
