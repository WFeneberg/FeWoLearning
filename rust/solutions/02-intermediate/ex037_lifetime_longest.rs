//! Exercise 037 — Explicit lifetimes on return values (reference solution).

pub fn longest<'a>(a: &'a str, b: &'a str) -> &'a str {
    if b.len() > a.len() {
        b
    } else {
        a
    }
}

pub fn longest_word<'a>(text: &'a str) -> Option<&'a str> {
    text.split_whitespace().max_by_key(|w| w.len())
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn picks_the_longer_string() {
        assert_eq!(longest("hello", "hi"), "hello");
        assert_eq!(longest("hi", "hello"), "hello");
    }

    #[test]
    fn ties_favor_the_first_argument() {
        assert_eq!(longest("cat", "dog"), "cat");
    }

    #[test]
    fn finds_the_longest_word_in_a_sentence() {
        assert_eq!(longest_word("the quickest brown fox"), Some("quickest"));
    }

    #[test]
    fn empty_text_has_no_longest_word() {
        assert_eq!(longest_word(""), None);
        assert_eq!(longest_word("   "), None);
    }

    #[test]
    fn returned_slice_borrows_from_the_original() {
        let sentence = String::from("wolf eagle hippopotamus ant");
        let word = longest_word(&sentence).unwrap();
        assert_eq!(word, "hippopotamus");
        // `word` is a genuine borrow of `sentence`, not an owned copy: its
        // pointer must fall inside `sentence`'s own buffer.
        let start = sentence.as_ptr() as usize;
        let end = start + sentence.len();
        let ptr = word.as_ptr() as usize;
        assert!(ptr >= start && ptr < end);
    }
}
