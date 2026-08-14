//! Exercise 059 — `mod`, `pub`, `pub(crate)`, paths (reference solution).

mod shapes {
    pub struct Circle {
        pub radius: f64,
    }

    pub(crate) fn area(circle: &Circle) -> f64 {
        std::f64::consts::PI * circle.radius * circle.radius
    }

    mod internal {
        pub(super) fn double(x: f64) -> f64 {
            x * 2.0
        }
    }

    pub fn diameter(circle: &Circle) -> f64 {
        internal::double(circle.radius)
    }
}

pub fn circle_area(radius: f64) -> f64 {
    shapes::area(&shapes::Circle { radius })
}

pub fn circle_diameter(radius: f64) -> f64 {
    shapes::diameter(&shapes::Circle { radius })
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    const EPSILON: f64 = 1e-6;

    #[test]
    fn computes_area_through_the_crate_visible_function() {
        assert!((circle_area(2.0) - std::f64::consts::PI * 4.0).abs() < EPSILON);
    }

    #[test]
    fn zero_radius_has_no_area() {
        assert!((circle_area(0.0) - 0.0).abs() < EPSILON);
    }

    #[test]
    fn computes_diameter_through_the_public_function() {
        assert!((circle_diameter(3.0) - 6.0).abs() < EPSILON);
    }
}
