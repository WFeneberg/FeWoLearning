//! Exercise 059 — `mod`, `pub`, `pub(crate)`, paths (intermediate).
//! Goal:   navigate a small module tree with mixed visibility, calling
//!         inward from the crate root through exactly the paths that are
//!         actually reachable.
//! Drills: `mod` nesting, `pub`, `pub(crate)`, `pub(super)`, module paths.

mod shapes {
    pub struct Circle {
        pub radius: f64,
    }

    // Visible anywhere in this crate, but not to other crates that might
    // depend on this one as a library.
    pub(crate) fn area(circle: &Circle) -> f64 {
        std::f64::consts::PI * circle.radius * circle.radius
    }

    mod internal {
        // Visible only to `shapes`, its direct parent module.
        pub(super) fn double(x: f64) -> f64 {
            x * 2.0
        }
    }

    pub fn diameter(circle: &Circle) -> f64 {
        internal::double(circle.radius)
    }
}

/// Reaches through `shapes`'s `pub(crate)` function via a full crate path.
pub fn circle_area(radius: f64) -> f64 {
    todo!("circle_area({radius})")
}

/// Reaches `shapes::diameter`, which internally uses `shapes::internal`
/// (itself unreachable from here directly — only through `diameter`).
pub fn circle_diameter(radius: f64) -> f64 {
    todo!("circle_diameter({radius})")
}

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
