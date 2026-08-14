//! Exercise 058 — `Cow`, avoiding needless allocation (intermediate).
//! Goal:   only allocate when the input actually needs changing; otherwise
//!         hand back the original borrow untouched.
//! Drills: `Cow<str>`, `Cow::Borrowed` vs `Cow::Owned`, deferred allocation.

use std::borrow::Cow;

/// Strips a single trailing `/`, if present. Paths that already have none
/// are returned as a plain borrow — no allocation happens.
pub fn without_trailing_slash(path: &str) -> Cow<'_, str> {
    todo!("without_trailing_slash({path:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn strips_a_trailing_slash() {
        assert_eq!(without_trailing_slash("/usr/local/"), "/usr/local");
    }

    #[test]
    fn leaves_a_path_without_one_unchanged() {
        assert_eq!(without_trailing_slash("/usr/local"), "/usr/local");
    }

    #[test]
    fn unchanged_input_is_borrowed_not_allocated() {
        let result = without_trailing_slash("/usr/local");
        assert!(matches!(result, Cow::Borrowed(_)));
    }

    #[test]
    fn changed_input_is_owned() {
        let result = without_trailing_slash("/usr/local/");
        assert!(matches!(result, Cow::Owned(_)));
    }

    #[test]
    fn root_slash_becomes_empty() {
        assert_eq!(without_trailing_slash("/"), "");
    }
}
