//! Exercise 040 — Implementing a trait for your own types (reference solution).

pub trait Area {
    fn area(&self) -> f64;
}

pub struct Circle {
    pub radius: f64,
}

pub struct Rectangle {
    pub width: f64,
    pub height: f64,
}

impl Area for Circle {
    fn area(&self) -> f64 {
        std::f64::consts::PI * self.radius * self.radius
    }
}

impl Area for Rectangle {
    fn area(&self) -> f64 {
        self.width * self.height
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    const EPSILON: f64 = 1e-6;

    #[test]
    fn circle_area_matches_pi_r_squared() {
        let c = Circle { radius: 2.0 };
        assert!((c.area() - std::f64::consts::PI * 4.0).abs() < EPSILON);
    }

    #[test]
    fn zero_radius_circle_has_no_area() {
        let c = Circle { radius: 0.0 };
        assert!((c.area() - 0.0).abs() < EPSILON);
    }

    #[test]
    fn rectangle_area_is_width_times_height() {
        let r = Rectangle { width: 3.0, height: 4.0 };
        assert!((r.area() - 12.0).abs() < EPSILON);
    }

    #[test]
    fn square_is_a_rectangle_with_equal_sides() {
        let r = Rectangle { width: 5.0, height: 5.0 };
        assert!((r.area() - 25.0).abs() < EPSILON);
    }
}
