//! Exercise 052 — Returning boxed closures (reference solution).

pub fn make_adder_boxed(x: i32) -> Box<dyn Fn(i32) -> i32> {
    Box::new(move |y| x + y)
}

pub fn pick_operation(op: &str) -> Box<dyn Fn(i32, i32) -> i32> {
    match op {
        "add" => Box::new(|a, b| a + b),
        "sub" => Box::new(|a, b| a - b),
        "mul" => Box::new(|a, b| a * b),
        other => panic!("unknown operation: {other}"),
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
