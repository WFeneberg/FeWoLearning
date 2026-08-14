//! Exercise 002 — let/mut/shadowing (beginner).
//! Goal:   describe a number by first shadowing it with its absolute value,
//!         then computing a doubled value in a `mut` binding, then shadowing
//!         that doubled value with its string form.
//! Drills: `let`, `mut`, shadowing (rebinding a name, even with a new type).

pub fn describe_number(n: i32) -> String {
    todo!("describe_number({n})")
}

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
