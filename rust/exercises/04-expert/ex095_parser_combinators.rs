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
        todo!(
            "if input.starts_with(expected), return Ok((&input[expected.len()..], ())); \
             otherwise Err(0) — literal never consumes anything on failure"
        )
    })
}

/// Runs `first`, then `second` on whatever `first` left behind, returning
/// both values as a tuple.
pub fn and_then<'a, T: 'a, U: 'a>(
    first: BoxParser<'a, T>,
    second: BoxParser<'a, U>,
) -> BoxParser<'a, (T, U)> {
    Box::new(move |input: &'a str| -> ParseResult<'a, (T, U)> {
        todo!(
            "parse `first` on `input`, propagating its error position unchanged on failure; \
             on success, parse `second` on the remainder; on ITS failure, translate the \
             position by adding how many bytes `first` consumed (input.len() minus the \
             length of the remainder after `first`); on success, return the final \
             remainder and (first_value, second_value)"
        )
    })
}

/// Tries `first`; if it fails, tries `second` on the SAME original input
/// (not on whatever `first` may have partially consumed before failing).
pub fn or<'a, T: 'a>(first: BoxParser<'a, T>, second: BoxParser<'a, T>) -> BoxParser<'a, T> {
    Box::new(move |input: &'a str| -> ParseResult<'a, T> {
        todo!(
            "try first.parse(input); if it's Ok, return it as-is; if it's Err, \
             return second.parse(input) instead (also as-is — no position translation \
             needed, both were given the same `input`)"
        )
    })
}

/// Applies `inner` zero or more times, collecting every successful value,
/// stopping (without propagating the error) at the first failure.
pub fn many<'a, T: 'a>(inner: BoxParser<'a, T>) -> BoxParser<'a, Vec<T>> {
    Box::new(move |input: &'a str| -> ParseResult<'a, Vec<T>> {
        todo!(
            "loop: call inner.parse on the current remainder; on Ok, push the value \
             and advance the remainder; on Err, stop the loop (this Err is NOT \
             propagated — `many` always succeeds, possibly with an empty Vec) and \
             return Ok((current_remainder, collected_values))"
        )
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
