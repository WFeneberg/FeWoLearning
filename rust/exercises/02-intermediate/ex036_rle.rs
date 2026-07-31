//! Exercise 036 — Run-length encoding (intermediate).
//! Goal:   encode("aaabbc") == "3a2b1c"; decode is the inverse.
//! Drills: iterators, peekable, String building, round-tripping.

pub fn encode(input: &str) -> String {
    todo!("implement encode for {input:?}")
}

pub fn decode(input: &str) -> String {
    todo!("implement decode for {input:?}")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn encodes() {
        assert_eq!(encode("aaabbc"), "3a2b1c");
        assert_eq!(encode(""), "");
        assert_eq!(encode("x"), "1x");
    }

    #[test]
    fn decodes() {
        assert_eq!(decode("3a2b1c"), "aaabbc");
        assert_eq!(decode(""), "");
    }

    #[test]
    fn round_trips() {
        let s = "wwwwaaadexxxxxx";
        assert_eq!(decode(&encode(s)), s);
    }
}
