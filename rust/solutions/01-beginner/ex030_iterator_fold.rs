//! Exercise 030 — Iterator fold (reference solution).

pub fn product_of_positives(values: &[i32]) -> i64 {
    values.iter().fold(1i64, |acc, &v| {
        if v > 0 {
            acc * v as i64
        } else {
            acc
        }
    })
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
