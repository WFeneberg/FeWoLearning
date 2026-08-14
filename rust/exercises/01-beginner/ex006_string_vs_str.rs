//! Exercise 006 — String vs &str (beginner).
//! Goal:   find the longest whitespace-delimited word in a borrowed `&str`
//!         and return it as an owned, uppercased `String`. On a tie, the
//!         first word of that length wins.
//! Drills: `String` vs `&str`, `to_owned`/`to_string`, `as_str`.

pub fn loudest_word(sentence: &str) -> String {
    todo!("loudest_word({sentence:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn picks_the_longest_word() {
        assert_eq!(loudest_word("the quick brown fox"), "QUICK");
    }

    #[test]
    fn breaks_ties_with_the_first_word() {
        assert_eq!(loudest_word("hi there world"), "THERE");
    }

    #[test]
    fn handles_a_single_word() {
        assert_eq!(loudest_word("hello"), "HELLO");
    }
}
