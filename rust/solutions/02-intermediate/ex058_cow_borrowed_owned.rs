//! Exercise 058 — `Cow`, avoiding needless allocation (reference solution).

use std::borrow::Cow;

pub fn without_trailing_slash(path: &str) -> Cow<'_, str> {
    match path.strip_suffix('/') {
        Some(stripped) => Cow::Owned(stripped.to_string()),
        None => Cow::Borrowed(path),
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
