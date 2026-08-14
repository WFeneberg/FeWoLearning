//! Exercise 023 — Struct impl methods (beginner).
//! Goal:   implement a small `Counter` showing the three method shapes:
//!         an associated function (no `self`), a `&mut self` mutator, a
//!         `&self` reader, and a consuming `self` method.
//! Drills: `impl`, `&self` vs `&mut self` vs `self`, associated functions.

pub struct Counter {
    count: u32,
}

impl Counter {
    pub fn new() -> Self {
        todo!("Counter::new")
    }

    pub fn increment(&mut self) {
        todo!("increment")
    }

    pub fn value(&self) -> u32 {
        todo!("value")
    }

    pub fn into_value(self) -> u32 {
        todo!("into_value")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn starts_at_zero() {
        assert_eq!(Counter::new().value(), 0);
    }

    #[test]
    fn increments_change_the_value() {
        let mut c = Counter::new();
        c.increment();
        c.increment();
        c.increment();
        assert_eq!(c.value(), 3);
    }

    #[test]
    fn into_value_consumes_the_counter() {
        let mut c = Counter::new();
        c.increment();
        assert_eq!(c.into_value(), 1);
    }
}
