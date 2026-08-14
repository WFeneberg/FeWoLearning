//! Exercise 027 — Mutable borrows (reference solution).

pub fn double_all(values: &mut Vec<i32>) {
    for v in values.iter_mut() {
        *v *= 2;
    }
}

pub fn increment_and_report(counter: &mut i32) -> i32 {
    *counter += 1;
    *counter
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
