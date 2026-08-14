//! Exercise 038 — Structs holding references (intermediate).
//! Goal:   a struct that borrows instead of owning its data, with methods
//!         relying on lifetime elision.
//! Drills: `struct Excerpt<'a>`, tying the struct's lifetime to a field,
//!         lifetime elision in method signatures.

pub struct Excerpt<'a> {
    pub text: &'a str,
}

impl<'a> Excerpt<'a> {
    /// Builds an excerpt borrowing `text`.
    pub fn new(text: &'a str) -> Self {
        todo!("Excerpt::new({text:?})")
    }

    /// The first whitespace-separated word, borrowed from the same source
    /// as `self.text` (elided lifetime: `fn first_word(&self) -> &str`).
    pub fn first_word(&self) -> &str {
        todo!("first_word for {:?}", self.text)
    }

    /// Whether the excerpt's text ends with `suffix`.
    pub fn ends_with(&self, suffix: &str) -> bool {
        todo!("ends_with({:?}, {suffix:?})", self.text)
    }
}

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
