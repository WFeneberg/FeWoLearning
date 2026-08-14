//! Exercise 037 — Explicit lifetimes on return values (intermediate).
//! Goal:   write functions whose return value borrows from one or more
//!         arguments, and say so with an explicit lifetime annotation.
//! Drills: `'a` on function signatures, tying a returned reference's lifetime
//!         to its inputs.

/// Returns whichever of `a` or `b` is longer (by byte length). Ties favor `a`.
pub fn longest<'a>(a: &'a str, b: &'a str) -> &'a str {
    todo!("longest({a:?}, {b:?})")
}

/// Returns the longest whitespace-separated word in `text`, borrowed from it.
/// Returns `None` for an empty (or all-whitespace) `text`.
pub fn longest_word<'a>(text: &'a str) -> Option<&'a str> {
    todo!("longest_word({text:?})")
}

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
