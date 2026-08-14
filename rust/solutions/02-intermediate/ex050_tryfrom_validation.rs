//! Exercise 050 — Validated construction with `TryFrom` (reference solution).

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct EvenNumber(pub i32);

impl TryFrom<i32> for EvenNumber {
    type Error = String;

    fn try_from(value: i32) -> Result<Self, Self::Error> {
        if value % 2 == 0 {
            Ok(EvenNumber(value))
        } else {
            Err(format!("{value} is not even"))
        }
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn accepts_an_even_number() {
        assert_eq!(EvenNumber::try_from(4), Ok(EvenNumber(4)));
    }

    #[test]
    fn accepts_zero() {
        assert_eq!(EvenNumber::try_from(0), Ok(EvenNumber(0)));
    }

    #[test]
    fn rejects_an_odd_number() {
        assert!(EvenNumber::try_from(5).is_err());
    }

    #[test]
    fn rejects_a_negative_odd_number() {
        assert!(EvenNumber::try_from(-3).is_err());
    }

    #[test]
    fn works_through_try_into_as_well() {
        let result: Result<EvenNumber, String> = 8.try_into();
        assert_eq!(result, Ok(EvenNumber(8)));
    }
}
