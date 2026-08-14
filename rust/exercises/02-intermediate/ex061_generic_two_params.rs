//! Exercise 061 — Multiple type parameters (intermediate).
//! Goal:   a `Pair<A, B>` that can hold two different types, and a swap
//!         that produces `Pair<B, A>` — each concrete instantiation is a
//!         distinct monomorphized type.
//! Drills: `struct Pair<A, B>`, `impl<A, B> Pair<A, B>`, bounded generic
//!         functions operating on it.

pub struct Pair<A, B> {
    pub first: A,
    pub second: B,
}

impl<A, B> Pair<A, B> {
    pub fn new(first: A, second: B) -> Self {
        Pair { first, second }
    }

    /// Consumes the pair, returning one with the two fields swapped —
    /// necessarily a `Pair<B, A>`, a different monomorphization.
    pub fn swap(self) -> Pair<B, A> {
        todo!("Pair::swap")
    }
}

/// Renders a pair via `Debug`, generic over both of its field types.
pub fn describe_pair<A: std::fmt::Debug, B: std::fmt::Debug>(pair: &Pair<A, B>) -> String {
    todo!("describe_pair")
}

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
