//! Exercise 034 — FizzBuzz (beginner).
//! Goal:   the classic: multiples of 3 print "Fizz", multiples of 5 print
//!         "Buzz", multiples of both print "FizzBuzz", otherwise the number.
//! Drills: control flow, `%` (modulo), building `String`s.

pub fn fizzbuzz(n: u32) -> String {
    todo!("fizzbuzz({n})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn multiples_of_three_are_fizz() {
        assert_eq!(fizzbuzz(3), "Fizz");
        assert_eq!(fizzbuzz(9), "Fizz");
    }

    #[test]
    fn multiples_of_five_are_buzz() {
        assert_eq!(fizzbuzz(5), "Buzz");
        assert_eq!(fizzbuzz(10), "Buzz");
    }

    #[test]
    fn multiples_of_both_are_fizzbuzz() {
        assert_eq!(fizzbuzz(15), "FizzBuzz");
        assert_eq!(fizzbuzz(30), "FizzBuzz");
    }

    #[test]
    fn everything_else_is_the_number_itself() {
        assert_eq!(fizzbuzz(1), "1");
        assert_eq!(fizzbuzz(7), "7");
    }
}
