//! Exercise 082 — Static dispatch (generics) vs. dynamic dispatch (`dyn`) (advanced).
//! Goal:   the same computation written two ways: once generic over `T:
//!         Shape` (monomorphized per concrete type, direct calls, but only
//!         ever homogeneous collections), once over `Box<dyn Shape>`
//!         (one shared function body, vtable calls, but genuinely
//!         heterogeneous collections).
//! Drills: trait objects (`dyn Trait`, object safety), generic trait bounds,
//!         when each dispatch style is the only option.

pub trait Shape {
    fn area(&self) -> f64;
}

pub struct Circle {
    pub radius: f64,
}

impl Shape for Circle {
    fn area(&self) -> f64 {
        std::f64::consts::PI * self.radius * self.radius
    }
}

pub struct Square {
    pub side: f64,
}

impl Shape for Square {
    fn area(&self) -> f64 {
        self.side * self.side
    }
}

/// Static dispatch: the compiler generates one specialized copy of this
/// function per concrete `T` actually used (monomorphization). Every
/// `.area()` call is a direct, inlinable call — but `shapes` must all be the
/// *same* concrete type.
pub fn total_area_generic<T: Shape>(shapes: &[T]) -> f64 {
    todo!("shapes.iter().map(Shape::area).sum()")
}

/// Dynamic dispatch: one function body shared by every shape type, at the
/// cost of a vtable lookup per `.area()` call. In exchange, `shapes` can mix
/// different concrete `Shape` types in the same `Vec` — impossible with the
/// generic version above.
pub fn total_area_dyn(shapes: &[Box<dyn Shape>]) -> f64 {
    todo!("shapes.iter().map(|s| s.area()).sum()")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn generic_version_sums_areas_of_one_concrete_type() {
        let circles = vec![Circle { radius: 1.0 }, Circle { radius: 2.0 }];
        let total = total_area_generic(&circles);
        let expected = std::f64::consts::PI * 1.0 + std::f64::consts::PI * 4.0;
        assert!((total - expected).abs() < 1e-9);
    }

    #[test]
    fn dyn_version_sums_areas_of_mixed_concrete_types() {
        let shapes: Vec<Box<dyn Shape>> =
            vec![Box::new(Circle { radius: 1.0 }), Box::new(Square { side: 3.0 })];
        let total = total_area_dyn(&shapes);
        let expected = std::f64::consts::PI + 9.0;
        assert!((total - expected).abs() < 1e-9);
    }

    #[test]
    fn both_dispatch_styles_agree_on_the_same_shape() {
        let squares = vec![Square { side: 2.0 }, Square { side: 4.0 }];
        let generic_total = total_area_generic(&squares);

        let boxed: Vec<Box<dyn Shape>> =
            squares.into_iter().map(|s| Box::new(s) as Box<dyn Shape>).collect();
        let dyn_total = total_area_dyn(&boxed);

        assert!((generic_total - dyn_total).abs() < 1e-9);
    }

    #[test]
    fn empty_slices_sum_to_zero() {
        let circles: Vec<Circle> = vec![];
        assert_eq!(total_area_generic(&circles), 0.0);

        let boxed: Vec<Box<dyn Shape>> = vec![];
        assert_eq!(total_area_dyn(&boxed), 0.0);
    }
}
