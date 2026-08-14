//! Exercise 088 — Type-level tagging with `PhantomData` (reference solution).

use std::marker::PhantomData;

pub struct Meters;
pub struct Feet;

pub struct Length<Unit> {
    value: f64,
    _unit: PhantomData<Unit>,
}

impl<Unit> Length<Unit> {
    pub fn new(value: f64) -> Self {
        Length {
            value,
            _unit: PhantomData,
        }
    }

    pub fn value(&self) -> f64 {
        self.value
    }
}

impl Length<Meters> {
    pub fn to_feet(&self) -> Length<Feet> {
        Length::new(self.value * 3.28084)
    }
}

impl std::ops::Add for Length<Meters> {
    type Output = Length<Meters>;

    fn add(self, other: Self) -> Self {
        Length::new(self.value + other.value)
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn addition_combines_two_lengths_of_the_same_unit() {
        let a = Length::<Meters>::new(2.0);
        let b = Length::<Meters>::new(3.5);
        let sum = a + b;
        assert_eq!(sum.value(), 5.5);
    }

    #[test]
    fn to_feet_converts_using_the_correct_factor() {
        let m = Length::<Meters>::new(1.0);
        let ft = m.to_feet();
        assert!((ft.value() - 3.28084).abs() < 1e-9);
    }

    #[test]
    fn phantom_data_adds_no_runtime_size_and_conversion_still_works() {
        // `PhantomData<Unit>` is zero-sized: `Length<Unit>` is exactly as big
        // as the `f64` it wraps, for every choice of `Unit`.
        assert_eq!(std::mem::size_of::<Length<Meters>>(), std::mem::size_of::<f64>());
        assert_eq!(std::mem::size_of::<Length<Feet>>(), std::mem::size_of::<f64>());

        let m = Length::<Meters>::new(10.0);
        let ft = m.to_feet();
        assert!((ft.value() - 32.8084).abs() < 1e-9);
    }
}
