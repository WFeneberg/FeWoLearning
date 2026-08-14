//! Exercise 029 — Iterator map/filter/collect (beginner).
//! Goal:   keep only the even numbers in a slice and square them, using
//!         iterator adapters instead of a manual loop with a `Vec::push`.
//! Drills: `iter`, `filter`, `map`, `collect::<Vec<_>>()`.

pub fn even_squares(values: &[i32]) -> Vec<i32> {
    todo!("even_squares({values:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn squares_only_the_even_values() {
        assert_eq!(even_squares(&[1, 2, 3, 4, 5, 6]), vec![4, 16, 36]);
    }

    #[test]
    fn no_even_values_gives_empty_vec() {
        assert_eq!(even_squares(&[1, 3, 5]), Vec::<i32>::new());
    }
}
