//! Exercise 036 — Run-length encoding (reference solution).

pub fn encode(input: &str) -> String {
    let mut out = String::new();
    let mut chars = input.chars().peekable();
    while let Some(c) = chars.next() {
        let mut count = 1;
        while chars.peek() == Some(&c) {
            chars.next();
            count += 1;
        }
        out.push_str(&count.to_string());
        out.push(c);
    }
    out
}

pub fn decode(input: &str) -> String {
    let mut out = String::new();
    let mut num = String::new();
    for c in input.chars() {
        if c.is_ascii_digit() {
            num.push(c);
        } else {
            let count: usize = num.parse().unwrap_or(1);
            for _ in 0..count {
                out.push(c);
            }
            num.clear();
        }
    }
    out
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
