//! Exercise 006 — String vs &str (reference solution).

pub fn loudest_word(sentence: &str) -> String {
    // `word` here is a `&str` borrowing from `sentence`.
    let word: &str = sentence
        .split_whitespace()
        .fold("", |best, w| if w.len() > best.len() { w } else { best });

    // Convert the borrowed `&str` into an owned `String`...
    let owned: String = word.to_owned();
    // ...then borrow it back out as `&str` to call a `&str` method.
    owned.as_str().to_uppercase()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
