//! Exercise 083 — A custom smart pointer with `Deref`/`DerefMut` (advanced).
//! Goal:   a wrapper type that lets callers use the wrapped value almost as
//!         if it weren't wrapped at all (auto-deref for method calls and
//!         `*`), while still tracking how many times it was dereferenced.
//! Drills: `impl Deref`, `impl DerefMut`, auto-deref during method
//!         resolution, `Cell` for a counter behind `&self`.

use std::cell::Cell;
use std::ops::{Deref, DerefMut};

pub struct Boxy<T> {
    value: T,
    pub access_count: Cell<u32>,
}

impl<T> Boxy<T> {
    pub fn new(value: T) -> Self {
        Boxy {
            value,
            access_count: Cell::new(0),
        }
    }
}

impl<T> Deref for Boxy<T> {
    type Target = T;

    fn deref(&self) -> &T {
        todo!("bump self.access_count, then return &self.value")
    }
}

impl<T> DerefMut for Boxy<T> {
    fn deref_mut(&mut self) -> &mut T {
        todo!("return &mut self.value")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn deref_gives_access_to_the_inners_own_methods() {
        let boxy = Boxy::new(String::from("hello"));
        assert_eq!(boxy.len(), 5); // auto-derefs to call String::len
        assert_eq!(*boxy, "hello".to_string());
    }

    #[test]
    fn deref_mut_allows_mutation_through_the_wrapper() {
        let mut boxy = Boxy::new(vec![1, 2, 3]);
        boxy.push(4); // auto-derefs (mutably) to call Vec::push
        assert_eq!(*boxy, vec![1, 2, 3, 4]);
    }

    #[test]
    fn each_deref_call_bumps_the_access_count() {
        let boxy = Boxy::new(10);
        assert_eq!(boxy.access_count.get(), 0);
        let _ = *boxy;
        assert_eq!(boxy.access_count.get(), 1);
        let _ = *boxy;
        assert_eq!(boxy.access_count.get(), 2);
    }
}
