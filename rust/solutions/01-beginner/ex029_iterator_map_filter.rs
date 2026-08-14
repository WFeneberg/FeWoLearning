//! Exercise 029 — Iterator map/filter/collect (reference solution).

pub fn even_squares(values: &[i32]) -> Vec<i32> {
    values
        .iter()
        .filter(|&&v| v % 2 == 0)
        .map(|&v| v * v)
        .collect()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
