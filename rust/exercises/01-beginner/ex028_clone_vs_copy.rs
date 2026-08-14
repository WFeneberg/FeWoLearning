//! Exercise 028 — Clone vs Copy (beginner).
//! Goal:   notice that a `Copy` type (`Point`) can still be used after being
//!         passed by value, while a non-`Copy` type (`String`) needs an
//!         explicit `.clone()` to be used again after being consumed.
//! Drills: `#[derive(Clone, Copy)]`, implicit copies vs. explicit clones.

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct Point {
    pub x: i32,
    pub y: i32,
}

pub fn sum_after_copy(p: Point) -> (Point, i32) {
    todo!("sum_after_copy({p:?})")
}

pub fn shout_after_clone(s: String) -> (String, String) {
    todo!("shout_after_clone({s:?})")
}

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
