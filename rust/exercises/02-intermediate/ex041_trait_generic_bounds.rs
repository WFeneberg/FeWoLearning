//! Exercise 041 — Generic functions bounded by a trait (intermediate).
//! Goal:   write functions generic over any type implementing `Summable`,
//!         using both bound syntaxes.
//! Drills: `<T: Trait>` inline bounds, `where` clauses, `PartialOrd`
//!         comparisons inside generic code.

pub trait Summable {
    fn value(&self) -> f64;
}

pub struct Item {
    pub price: f64,
    pub qty: u32,
}

impl Summable for Item {
    fn value(&self) -> f64 {
        self.price * self.qty as f64
    }
}

/// Sums `value()` across every item. Uses an inline `<T: Summable>` bound.
pub fn total<T: Summable>(items: &[T]) -> f64 {
    todo!("total over {} items", items.len())
}

/// Finds the item with the largest `value()`. Uses a `where` clause instead
/// of an inline bound — same meaning, different spelling.
pub fn largest<T>(items: &[T]) -> Option<&T>
where
    T: Summable,
{
    todo!("largest over {} items", items.len())
}

#[cfg(test)]
mod tests {
    use super::*;

    const EPSILON: f64 = 1e-6;

    fn sample() -> Vec<Item> {
        vec![
            Item { price: 2.5, qty: 4 },  // 10.0
            Item { price: 9.0, qty: 1 },  // 9.0
            Item { price: 1.0, qty: 20 }, // 20.0
        ]
    }

    #[test]
    fn total_sums_every_items_value() {
        assert!((total(&sample()) - 39.0).abs() < EPSILON);
    }

    #[test]
    fn total_of_no_items_is_zero() {
        let empty: Vec<Item> = vec![];
        assert!((total(&empty) - 0.0).abs() < EPSILON);
    }

    #[test]
    fn largest_finds_the_highest_value_item() {
        let items = sample();
        let winner = largest(&items).unwrap();
        assert!((winner.value() - 20.0).abs() < EPSILON);
    }

    #[test]
    fn largest_of_no_items_is_none() {
        let empty: Vec<Item> = vec![];
        assert!(largest(&empty).is_none());
    }
}
