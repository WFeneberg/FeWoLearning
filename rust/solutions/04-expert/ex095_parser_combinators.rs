//! Exercise 095 — Parser combinators: composition and error positions
//! (expert).
//! Goal:   four small combinators over `&str` — `literal`, `and_then`
//!         (sequence), `or` (alternative), and `many` (zero-or-more) — built
//!         on a `Parser` trait so a bare closure `Fn(&str) -> ParseResult<T>`
//!         and the combinators below are interchangeable. Every parser
//!         returns `Result<(remaining_input, value), position>`, where
//!         `position` is the byte offset — RELATIVE TO THE INPUT THAT
//!         SPECIFIC PARSER WAS GIVEN — at which it gave up. Composing
//!         parsers means translating a sub-parser's local failure position
//!         into the outer parser's coordinate frame (see `and_then`).
//! Drills: a trait implemented via a blanket impl over `Fn`, `Box<dyn Trait>`
//!         for object-safe composition, threading error positions through
//!         composed parsers.

pub type ParseResult<'a, T> = Result<(&'a str, T), usize>;

/// Anything callable as `Fn(&'a str) -> ParseResult<'a, T>` is a parser.
pub trait Parser<'a, T> {
    fn parse(&self, input: &'a str) -> ParseResult<'a, T>;
}

impl<'a, T, F> Parser<'a, T> for F
where
    F: Fn(&'a str) -> ParseResult<'a, T>,
{
    fn parse(&self, input: &'a str) -> ParseResult<'a, T> {
        self(input)
    }
}

/// A boxed, object-safe parser — what every combinator below returns, so
/// they can be composed without each caller needing to name a unique
/// closure type.
pub type BoxParser<'a, T> = Box<dyn Parser<'a, T> + 'a>;

/// Matches `expected` at the very start of the input.
pub fn literal<'a>(expected: &'static str) -> BoxParser<'a, ()> {
    Box::new(move |input: &'a str| -> ParseResult<'a, ()> {
        if input.starts_with(expected) {
            Ok((&input[expected.len()..], ()))
        } else {
            Err(0)
        }
    })
}

/// Runs `first`, then `second` on whatever `first` left behind, returning
/// both values as a tuple.
pub fn and_then<'a, T: 'a, U: 'a>(
    first: BoxParser<'a, T>,
    second: BoxParser<'a, U>,
) -> BoxParser<'a, (T, U)> {
    Box::new(move |input: &'a str| -> ParseResult<'a, (T, U)> {
        let (rest1, value1) = first.parse(input)?;
        match second.parse(rest1) {
            Ok((rest2, value2)) => Ok((rest2, (value1, value2))),
            Err(pos) => Err((input.len() - rest1.len()) + pos),
        }
    })
}

/// Tries `first`; if it fails, tries `second` on the SAME original input
/// (not on whatever `first` may have partially consumed before failing).
pub fn or<'a, T: 'a>(first: BoxParser<'a, T>, second: BoxParser<'a, T>) -> BoxParser<'a, T> {
    Box::new(move |input: &'a str| -> ParseResult<'a, T> {
        match first.parse(input) {
            Ok(result) => Ok(result),
            Err(_) => second.parse(input),
        }
    })
}

/// Applies `inner` zero or more times, collecting every successful value,
/// stopping (without propagating the error) at the first failure.
pub fn many<'a, T: 'a>(inner: BoxParser<'a, T>) -> BoxParser<'a, Vec<T>> {
    Box::new(move |input: &'a str| -> ParseResult<'a, Vec<T>> {
        let mut remaining = input;
        let mut values = Vec::new();
        while let Ok((rest, value)) = inner.parse(remaining) {
            values.push(value);
            remaining = rest;
        }
        Ok((remaining, values))
    })
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn literal_matches_prefix_and_returns_remainder() {
        let parser = literal("hello");
        let (rest, ()) = parser.parse("hello world").unwrap();
        assert_eq!(rest, " world");
    }

    #[test]
    fn literal_fails_at_position_zero_on_mismatch() {
        let parser = literal("hello");
        assert_eq!(parser.parse("goodbye"), Err(0));
    }

    #[test]
    fn and_then_sequences_two_parsers() {
        let parser = and_then(literal("foo"), literal("bar"));
        let (rest, ((), ())) = parser.parse("foobarbaz").unwrap();
        assert_eq!(rest, "baz");
    }

    #[test]
    fn and_then_reports_the_second_parsers_failure_position() {
        let parser = and_then(literal("foo"), literal("bar"));
        // "foo" consumes 3 bytes, then "bar" fails right after that.
        assert_eq!(parser.parse("fooXYZ"), Err(3));
    }

    #[test]
    fn and_then_propagates_the_first_parsers_failure_position() {
        let parser = and_then(literal("foo"), literal("bar"));
        assert_eq!(parser.parse("XYZ"), Err(0));
    }

    #[test]
    fn or_prefers_the_first_parser_when_it_matches() {
        let parser = or(literal("cat"), literal("dog"));
        assert_eq!(parser.parse("cat!").unwrap().0, "!");
    }

    #[test]
    fn or_falls_back_to_the_second_parser() {
        let parser = or(literal("cat"), literal("dog"));
        assert_eq!(parser.parse("dog!").unwrap().0, "!");
    }

    #[test]
    fn or_fails_with_the_second_parsers_position_when_both_fail() {
        let parser = or(literal("cat"), literal("dog"));
        assert_eq!(parser.parse("bird"), Err(0));
    }

    #[test]
    fn many_collects_zero_matches_without_failing() {
        let parser = many(literal("ab"));
        let (rest, matches) = parser.parse("xyz").unwrap();
        assert_eq!(matches.len(), 0);
        assert_eq!(rest, "xyz");
    }

    #[test]
    fn many_collects_repeated_matches_until_failure() {
        let parser = many(literal("ab"));
        let (rest, matches) = parser.parse("ababab!").unwrap();
        assert_eq!(matches.len(), 3);
        assert_eq!(rest, "!");
    }

    #[test]
    fn combinators_compose_literal_and_then_many() {
        let parser = and_then(literal("go:"), many(literal("+")));
        let (rest, (_, pluses)) = parser.parse("go:+++end").unwrap();
        assert_eq!(pluses.len(), 3);
        assert_eq!(rest, "end");
    }
}
