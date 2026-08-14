//! Exercise 021 — Enum with data (reference solution).

pub enum Shape {
    Circle(f64),
    Rectangle(f64, f64),
    Triangle { base: f64, height: f64 },
}

pub fn area(shape: &Shape) -> f64 {
    match shape {
        Shape::Circle(radius) => std::f64::consts::PI * radius * radius,
        Shape::Rectangle(width, height) => width * height,
        Shape::Triangle { base, height } => 0.5 * base * height,
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
