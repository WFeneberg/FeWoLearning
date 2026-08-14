//! Exercise 027 — Mutable borrows (beginner).
//! Goal:   mutate through a `&mut` reference, both in a loop over a whole
//!         `Vec` and via a single reborrowed `&mut i32`.
//! Drills: `&mut T`, exclusivity (only one mutable borrow at a time),
//!         reborrowing.

pub fn double_all(values: &mut Vec<i32>) {
    todo!("double_all({values:?})")
}

pub fn increment_and_report(counter: &mut i32) -> i32 {
    todo!("increment_and_report({counter})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn doubles_every_element_in_place() {
        let mut values = vec![1, 2, 3];
        double_all(&mut values);
        assert_eq!(values, vec![2, 4, 6]);
    }

    #[test]
    fn increment_mutates_and_returns_the_new_value() {
        let mut x = 4;
        let reported = increment_and_report(&mut x);
        assert_eq!(reported, 5);
        assert_eq!(x, 5);
    }
}
