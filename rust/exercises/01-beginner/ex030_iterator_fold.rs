//! Exercise 030 — Iterator fold (beginner).
//! Goal:   multiply together only the strictly positive numbers in a slice,
//!         threading an accumulator through with `fold` instead of a manual
//!         loop with a `mut` variable.
//! Drills: `Iterator::fold`, accumulator threading.

pub fn product_of_positives(values: &[i32]) -> i64 {
    todo!("product_of_positives({values:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn multiplies_only_positive_values() {
        assert_eq!(product_of_positives(&[2, -3, 4, 0, 5]), 40);
    }

    #[test]
    fn empty_slice_has_product_one() {
        assert_eq!(product_of_positives(&[]), 1);
    }

    #[test]
    fn no_positive_values_has_product_one() {
        assert_eq!(product_of_positives(&[-1, -2, 0]), 1);
    }
}
