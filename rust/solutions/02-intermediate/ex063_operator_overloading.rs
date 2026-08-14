//! Exercise 063 — Operator overloading via `std::ops` (reference solution).

#[derive(Debug, Clone, Copy, PartialEq)]
pub struct Vec2 {
    pub x: f64,
    pub y: f64,
}

impl std::ops::Add for Vec2 {
    type Output = Vec2;

    fn add(self, rhs: Vec2) -> Vec2 {
        Vec2 { x: self.x + rhs.x, y: self.y + rhs.y }
    }
}

impl std::ops::Mul<f64> for Vec2 {
    type Output = Vec2;

    fn mul(self, scalar: f64) -> Vec2 {
        Vec2 { x: self.x * scalar, y: self.y * scalar }
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn adds_component_wise() {
        let a = Vec2 { x: 1.0, y: 2.0 };
        let b = Vec2 { x: 3.0, y: -1.0 };
        assert_eq!(a + b, Vec2 { x: 4.0, y: 1.0 });
    }

    #[test]
    fn adding_the_zero_vector_is_a_no_op() {
        let a = Vec2 { x: 5.0, y: -5.0 };
        let zero = Vec2 { x: 0.0, y: 0.0 };
        assert_eq!(a + zero, a);
    }

    #[test]
    fn scales_both_components() {
        let a = Vec2 { x: 2.0, y: -3.0 };
        assert_eq!(a * 2.0, Vec2 { x: 4.0, y: -6.0 });
    }

    #[test]
    fn scaling_by_zero_collapses_to_the_origin() {
        let a = Vec2 { x: 7.0, y: 9.0 };
        assert_eq!(a * 0.0, Vec2 { x: 0.0, y: 0.0 });
    }

    #[test]
    fn operators_compose() {
        let a = Vec2 { x: 1.0, y: 1.0 };
        let b = Vec2 { x: 2.0, y: 3.0 };
        assert_eq!((a + b) * 2.0, Vec2 { x: 6.0, y: 8.0 });
    }
}
