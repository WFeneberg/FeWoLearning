//! Exercise 008 — Char classification (reference solution).

pub fn classify(c: char) -> &'static str {
    if c.is_ascii_digit() {
        "digit"
    } else if c.is_alphabetic() {
        "alpha"
    } else if c.is_whitespace() {
        "whitespace"
    } else {
        "other"
    }
}

pub fn utf8_len(c: char) -> usize {
    c.len_utf8()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn classifies_digits_letters_whitespace_and_other() {
        assert_eq!(classify('7'), "digit");
        assert_eq!(classify('A'), "alpha");
        assert_eq!(classify(' '), "whitespace");
        assert_eq!(classify('!'), "other");
    }

    #[test]
    fn classifies_non_ascii_letters_as_alpha() {
        assert_eq!(classify('é'), "alpha");
        assert_eq!(classify('中'), "alpha");
    }

    #[test]
    fn utf8_byte_length_grows_with_code_point() {
        assert_eq!(utf8_len('a'), 1);
        assert_eq!(utf8_len('é'), 2);
        assert_eq!(utf8_len('中'), 3);
    }
}
