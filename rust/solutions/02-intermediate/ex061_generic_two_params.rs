//! Exercise 061 — Multiple type parameters (reference solution).

pub struct Pair<A, B> {
    pub first: A,
    pub second: B,
}

impl<A, B> Pair<A, B> {
    pub fn new(first: A, second: B) -> Self {
        Pair { first, second }
    }

    pub fn swap(self) -> Pair<B, A> {
        Pair { first: self.second, second: self.first }
    }
}

pub fn describe_pair<A: std::fmt::Debug, B: std::fmt::Debug>(pair: &Pair<A, B>) -> String {
    format!("({:?}, {:?})", pair.first, pair.second)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn swap_flips_both_fields_and_their_types() {
        let pair = Pair::new(1, "one");
        let swapped = pair.swap();
        assert_eq!(swapped.first, "one");
        assert_eq!(swapped.second, 1);
    }

    #[test]
    fn describe_renders_both_fields() {
        let pair = Pair::new(3, "three");
        let rendered = describe_pair(&pair);
        assert!(rendered.contains('3'));
        assert!(rendered.contains("three"));
    }

    #[test]
    fn works_with_same_type_for_both_parameters() {
        let pair = Pair::new(1.5, 2.5);
        let swapped = pair.swap();
        assert_eq!(swapped.first, 2.5);
        assert_eq!(swapped.second, 1.5);
    }
}
