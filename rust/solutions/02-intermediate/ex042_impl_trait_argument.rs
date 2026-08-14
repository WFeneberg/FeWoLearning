//! Exercise 042 — `impl Trait` in argument and return position (reference solution).

pub fn describe_all(items: impl IntoIterator<Item = String>) -> String {
    items.into_iter().collect::<Vec<_>>().join(", ")
}

pub fn make_multiplier(factor: i32) -> impl Fn(i32) -> i32 {
    move |x: i32| -> i32 { x * factor }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn joins_a_vec_of_strings() {
        let items = vec!["a".to_string(), "b".to_string(), "c".to_string()];
        assert_eq!(describe_all(items), "a, b, c");
    }

    #[test]
    fn joins_an_empty_iterable() {
        let items: Vec<String> = vec![];
        assert_eq!(describe_all(items), "");
    }

    #[test]
    fn accepts_any_intoiterator_not_just_vec() {
        use std::collections::VecDeque;
        let mut items = VecDeque::new();
        items.push_back("x".to_string());
        items.push_back("y".to_string());
        assert_eq!(describe_all(items), "x, y");
    }

    #[test]
    fn multiplier_scales_its_argument() {
        let times_three = make_multiplier(3);
        assert_eq!(times_three(4), 12);
        assert_eq!(times_three(0), 0);
    }

    #[test]
    fn multiplier_by_zero_collapses_everything() {
        let times_zero = make_multiplier(0);
        assert_eq!(times_zero(100), 0);
    }
}
