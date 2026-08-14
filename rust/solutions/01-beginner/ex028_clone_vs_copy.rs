//! Exercise 028 — Clone vs Copy (reference solution).

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct Point {
    pub x: i32,
    pub y: i32,
}

pub fn sum_after_copy(p: Point) -> (Point, i32) {
    // `p` is `Copy`, so using it here does not move it — it can still be
    // returned afterwards.
    let sum = p.x + p.y;
    (p, sum)
}

pub fn shout_after_clone(s: String) -> (String, String) {
    // `String` is not `Copy`; without `.clone()` here, `s` would be moved
    // into `to_uppercase()` and unavailable for the tuple below.
    let upper = s.clone().to_uppercase();
    (s, upper)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn point_is_still_usable_after_being_passed() {
        let p = Point { x: 3, y: 4 };
        assert_eq!(sum_after_copy(p), (Point { x: 3, y: 4 }, 7));
    }

    #[test]
    fn string_is_cloned_before_being_consumed() {
        assert_eq!(
            shout_after_clone("hi".to_string()),
            ("hi".to_string(), "HI".to_string())
        );
    }
}
