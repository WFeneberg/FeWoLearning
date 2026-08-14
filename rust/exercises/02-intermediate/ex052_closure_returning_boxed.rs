//! Exercise 052 — Returning boxed closures (intermediate).
//! Goal:   return closures whose concrete type genuinely varies at runtime,
//!         which requires boxing them behind a trait object.
//! Drills: `Box<dyn Fn(...) -> ...>`, choosing a closure at runtime, `move`
//!         captures.

/// Returns a closure that adds `x` to its argument.
pub fn make_adder_boxed(x: i32) -> Box<dyn Fn(i32) -> i32> {
    todo!("make_adder_boxed({x})")
}

/// Picks one of several two-argument operations by name. Unlike
/// `make_adder_boxed`, the *shape* of the returned closure body differs per
/// branch, so `impl Fn` (a single anonymous type) wouldn't work here — only
/// a trait object can name "some `Fn(i32, i32) -> i32`, which one varies".
pub fn pick_operation(op: &str) -> Box<dyn Fn(i32, i32) -> i32> {
    todo!("pick_operation({op:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn adder_adds_its_captured_offset() {
        let add_five = make_adder_boxed(5);
        assert_eq!(add_five(10), 15);
        assert_eq!(add_five(-5), 0);
    }

    #[test]
    fn picks_addition() {
        let add = pick_operation("add");
        assert_eq!(add(2, 3), 5);
    }

    #[test]
    fn picks_subtraction() {
        let sub = pick_operation("sub");
        assert_eq!(sub(5, 3), 2);
    }

    #[test]
    fn picks_multiplication() {
        let mul = pick_operation("mul");
        assert_eq!(mul(4, 3), 12);
    }

    #[test]
    #[should_panic(expected = "unknown operation")]
    fn panics_on_an_unknown_operation() {
        pick_operation("xor");
    }
}
