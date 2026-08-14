//! Exercise 023 — Struct impl methods (reference solution).

pub struct Counter {
    count: u32,
}

impl Counter {
    pub fn new() -> Self {
        Counter { count: 0 }
    }

    pub fn increment(&mut self) {
        self.count += 1;
    }

    pub fn value(&self) -> u32 {
        self.count
    }

    pub fn into_value(self) -> u32 {
        self.count
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
