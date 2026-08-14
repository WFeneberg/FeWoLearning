//! Exercise 038 — Structs holding references (reference solution).

pub struct Excerpt<'a> {
    pub text: &'a str,
}

impl<'a> Excerpt<'a> {
    pub fn new(text: &'a str) -> Self {
        Excerpt { text }
    }

    pub fn first_word(&self) -> &str {
        self.text.split_whitespace().next().unwrap_or("")
    }

    pub fn ends_with(&self, suffix: &str) -> bool {
        self.text.ends_with(suffix)
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn constructs_from_a_borrowed_str() {
        let source = String::from("the quick brown fox");
        let excerpt = Excerpt::new(&source);
        assert_eq!(excerpt.text, "the quick brown fox");
    }

    #[test]
    fn first_word_borrows_the_leading_word() {
        let source = String::from("hello world");
        let excerpt = Excerpt::new(&source);
        assert_eq!(excerpt.first_word(), "hello");
    }

    #[test]
    fn first_word_of_a_single_word_is_the_whole_text() {
        let excerpt = Excerpt::new("solo");
        assert_eq!(excerpt.first_word(), "solo");
    }

    #[test]
    fn ends_with_checks_the_suffix() {
        let excerpt = Excerpt::new("run-length encoding");
        assert!(excerpt.ends_with("encoding"));
        assert!(!excerpt.ends_with("decoding"));
    }

    #[test]
    fn excerpt_can_outlive_a_temporary_binding_scope() {
        let source = String::from("borrowed all the way down");
        let word;
        {
            let excerpt = Excerpt::new(&source);
            word = excerpt.first_word().to_owned();
        }
        assert_eq!(word, "borrowed");
    }
}
