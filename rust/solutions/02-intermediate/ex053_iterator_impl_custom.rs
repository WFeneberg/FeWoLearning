//! Exercise 053 — Implementing `Iterator` by hand (reference solution).

pub struct Counter {
    count: u32,
    max: u32,
}

impl Counter {
    pub fn new(max: u32) -> Self {
        Counter { count: 0, max }
    }
}

impl Iterator for Counter {
    type Item = u32;

    fn next(&mut self) -> Option<u32> {
        if self.count < self.max {
            self.count += 1;
            Some(self.count)
        } else {
            None
        }
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn collects_the_full_sequence() {
        let values: Vec<u32> = Counter::new(5).collect();
        assert_eq!(values, vec![1, 2, 3, 4, 5]);
    }

    #[test]
    fn counting_zero_yields_nothing() {
        let values: Vec<u32> = Counter::new(0).collect();
        assert!(values.is_empty());
    }

    #[test]
    fn sum_works_because_iterator_is_implemented() {
        let total: u32 = Counter::new(5).sum();
        assert_eq!(total, 15);
    }

    #[test]
    fn other_adapters_come_for_free() {
        let paired: Vec<(u32, u32)> = Counter::new(3).zip(Counter::new(5)).collect();
        assert_eq!(paired, vec![(1, 1), (2, 2), (3, 3)]);
    }

    #[test]
    fn iterator_is_exhausted_after_max() {
        let mut counter = Counter::new(2);
        assert_eq!(counter.next(), Some(1));
        assert_eq!(counter.next(), Some(2));
        assert_eq!(counter.next(), None);
        assert_eq!(counter.next(), None);
    }
}
