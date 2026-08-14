//! Exercise 008 — Char classification (beginner).
//! Goal:   classify a `char` as one of a few categories, and separately
//!         report how many bytes it takes when encoded as UTF-8 (a `char`
//!         is a Unicode scalar value, not a byte).
//! Drills: `char` methods (`is_ascii_digit`, `is_alphabetic`, `is_whitespace`),
//!         `char::len_utf8`, Unicode vs. bytes.

pub fn classify(c: char) -> &'static str {
    todo!("classify({c:?})")
}

pub fn utf8_len(c: char) -> usize {
    todo!("utf8_len({c:?})")
}

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
