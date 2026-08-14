//! Exercise 086 — `macro_rules!`: repetition and hygiene (reference solution).

macro_rules! min_max {
    ($first:expr $(, $rest:expr)*) => {
        // `$crate::` roots the path at this crate regardless of where the
        // macro is invoked from (e.g. the `tests` submodule below) — a
        // bare `min_max_of` would instead resolve relative to the CALL
        // site's module, which has no such function.
        $crate::ex086_macro_rules_basics::min_max_of(&[$first $(, $rest)*])
    };
}

fn min_max_of<T: PartialOrd + Copy>(items: &[T]) -> (T, T) {
    let mut min = items[0];
    let mut max = items[0];
    for &value in &items[1..] {
        if value < min {
            min = value;
        }
        if value > max {
            max = value;
        }
    }
    (min, max)
}

macro_rules! swap_via_macro {
    ($a:ident, $b:ident) => {{
        let temp = $a;
        $a = $b;
        $b = temp;
    }};
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    #[test]
    fn min_max_of_a_single_value() {
        assert_eq!(min_max!(5), (5, 5));
    }

    #[test]
    fn min_max_of_several_values() {
        assert_eq!(min_max!(3, 1, 4, 1, 5, 9, 2, 6), (1, 9));
    }

    #[test]
    fn min_max_works_with_negative_numbers() {
        assert_eq!(min_max!(-5, 3, -10, 7), (-10, 7));
    }

    #[test]
    fn swap_via_macro_swaps_both_bindings() {
        let mut x = 1;
        let mut y = 2;
        swap_via_macro!(x, y);
        assert_eq!(x, 2);
        assert_eq!(y, 1);
    }

    #[test]
    fn swap_via_macro_does_not_leak_its_internal_temp_binding() {
        // The macro's own `temp` binding is hygienically distinct from this
        // `temp`, even though they share a name — this couldn't work with a
        // textual-substitution macro system (e.g. C's preprocessor).
        let mut a = "first".to_string();
        let mut b = "second".to_string();
        let temp = "caller's own temp, untouched by the macro";
        swap_via_macro!(a, b);
        assert_eq!(a, "second");
        assert_eq!(b, "first");
        assert_eq!(temp, "caller's own temp, untouched by the macro");
    }
}
