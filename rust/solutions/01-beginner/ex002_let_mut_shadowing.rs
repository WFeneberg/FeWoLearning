//! Exercise 002 — let/mut/shadowing (reference solution).

pub fn describe_number(n: i32) -> String {
    // Shadow `n` with its absolute value — same name, same type, new value.
    let n = n.abs();

    // A `mut` binding we update in place.
    let mut doubled = n;
    doubled *= 2;

    // Shadow `doubled` with a different type (i32 -> String).
    let doubled = doubled.to_string();

    format!("original abs={n}, doubled={doubled}")
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn describes_negative_number() {
        assert_eq!(describe_number(-5), "original abs=5, doubled=10");
    }

    #[test]
    fn describes_positive_number() {
        assert_eq!(describe_number(3), "original abs=3, doubled=6");
    }

    #[test]
    fn describes_zero() {
        assert_eq!(describe_number(0), "original abs=0, doubled=0");
    }
}
