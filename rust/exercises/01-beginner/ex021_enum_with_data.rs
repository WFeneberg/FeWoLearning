//! Exercise 021 — Enum with data (beginner).
//! Goal:   compute the area of a `Shape`, whose variants carry their own
//!         data (tuple variants and a struct-like variant).
//! Drills: data-carrying enum variants, matching and destructuring them.

pub enum Shape {
    Circle(f64),
    Rectangle(f64, f64),
    Triangle { base: f64, height: f64 },
}

pub fn area(shape: &Shape) -> f64 {
    todo!("area of shape")
}

#[cfg(test)]
mod tests {
    use super::*;

    fn approx_eq(a: f64, b: f64) -> bool {
        (a - b).abs() < 1e-9
    }

    #[test]
    fn circle_area() {
        assert!(approx_eq(area(&Shape::Circle(2.0)), std::f64::consts::PI * 4.0));
    }

    #[test]
    fn rectangle_area() {
        assert!(approx_eq(area(&Shape::Rectangle(3.0, 4.0)), 12.0));
    }

    #[test]
    fn triangle_area() {
        assert!(approx_eq(
            area(&Shape::Triangle {
                base: 6.0,
                height: 2.0
            }),
            6.0
        ));
    }
}
