//! Exercise 087 — A hand-rolled iterator adapter, allocation-free (advanced).
//! Goal:   `every_nth` behaves like collecting into a `Vec` and filtering by
//!         index, but does neither: it's a thin lazy wrapper around the
//!         inner iterator, computed one item at a time, on demand. That's
//!         the "zero-cost" part — it composes with `.map`/`.collect`/etc.
//!         exactly like a built-in adapter, with no intermediate buffer.
//! Drills: implementing `Iterator` for a wrapper type, an extension trait
//!         (`EveryNthExt`) so `.every_nth(n)` reads like a built-in adapter,
//!         driving the inner iterator forward without collecting it.

pub struct EveryNth<I> {
    inner: I,
    n: usize,
}

pub trait EveryNthExt: Iterator + Sized {
    /// Yields items at indices `0, n, 2n, ...` of `self`, lazily.
    fn every_nth(self, n: usize) -> EveryNth<Self> {
        assert!(n > 0, "n must be positive");
        EveryNth { inner: self, n }
    }
}

impl<I: Iterator> EveryNthExt for I {}

impl<I: Iterator> Iterator for EveryNth<I> {
    type Item = I::Item;

    fn next(&mut self) -> Option<I::Item> {
        todo!("take one item from self.inner, then advance it (n - 1) more times before returning")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn yields_every_nth_item_starting_from_the_first() {
        let result: Vec<i32> = (1..=10).every_nth(3).collect();
        assert_eq!(result, vec![1, 4, 7, 10]);
    }

    #[test]
    fn n_equal_one_yields_everything() {
        let result: Vec<i32> = (1..=5).every_nth(1).collect();
        assert_eq!(result, vec![1, 2, 3, 4, 5]);
    }

    #[test]
    fn composes_with_other_adapters_with_no_intermediate_vec() {
        let result: Vec<i32> = (1..=20).every_nth(4).map(|x| x * 2).collect();
        assert_eq!(result, vec![2, 10, 18, 26, 34]);
    }

    #[test]
    fn empty_iterator_yields_nothing() {
        let result: Vec<i32> = core::iter::empty::<i32>().every_nth(3).collect();
        assert!(result.is_empty());
    }
}
