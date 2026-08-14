//! Exercise 005 — Tuple struct point (beginner).
//! Goal:   implement Euclidean distance and midpoint for a 2-D point
//!         represented as a tuple struct.
//! Drills: tuple structs, field access via `.0`/`.1`, destructuring.

pub struct Point(pub f64, pub f64);

pub fn distance(a: &Point, b: &Point) -> f64 {
    todo!("distance")
}

pub fn midpoint(a: &Point, b: &Point) -> Point {
    todo!("midpoint")
}

#[cfg(test)]
mod tests {
    use super::*;

    fn approx_eq(a: f64, b: f64) -> bool {
        (a - b).abs() < 1e-9
    }

    #[test]
    fn distance_of_3_4_5_triangle() {
        let a = Point(0.0, 0.0);
        let b = Point(3.0, 4.0);
        assert!(approx_eq(distance(&a, &b), 5.0));
    }

    #[test]
    fn distance_is_zero_for_same_point() {
        let a = Point(1.5, -2.5);
        assert!(approx_eq(distance(&a, &a), 0.0));
    }

    #[test]
    fn midpoint_destructures_correctly() {
        let a = Point(0.0, 0.0);
        let b = Point(4.0, 2.0);
        let Point(x, y) = midpoint(&a, &b);
        assert!(approx_eq(x, 2.0));
        assert!(approx_eq(y, 1.0));
    }
}
