//! Exercise 089 — Fixed-size APIs with const generics (advanced).
//! Goal:   `ArrayStack<T, N>` is backed by a plain `[Option<T>; N]` — no
//!         heap allocation — where `N` is baked into the type itself, so
//!         `ArrayStack<i32, 2>` and `ArrayStack<i32, 8>` are genuinely
//!         different types with their capacity known at compile time.
//! Drills: `struct Foo<T, const N: usize>`, `[T; N]` fields,
//!         `std::array::from_fn`, capacity checks against `N`.

pub struct ArrayStack<T, const N: usize> {
    items: [Option<T>; N],
    len: usize,
}

impl<T, const N: usize> ArrayStack<T, N> {
    pub fn new() -> Self {
        ArrayStack {
            items: std::array::from_fn(|_| None),
            len: 0,
        }
    }

    pub fn capacity(&self) -> usize {
        N
    }

    pub fn len(&self) -> usize {
        self.len
    }

    pub fn is_empty(&self) -> bool {
        self.len == 0
    }

    pub fn is_full(&self) -> bool {
        self.len == N
    }

    /// Pushes `value` if there's room, else hands it back in `Err`.
    pub fn push(&mut self, value: T) -> Result<(), T> {
        todo!("if self.is_full() {{ return Err(value); }} store into items[len], len += 1, Ok(())")
    }

    /// Pops the most recently pushed value, if any.
    pub fn pop(&mut self) -> Option<T> {
        todo!("if empty return None; else len -= 1, take items[len]")
    }
}

impl<T, const N: usize> Default for ArrayStack<T, N> {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_fresh_stack_reports_its_const_generic_capacity() {
        let mut stack: ArrayStack<i32, 4> = ArrayStack::new();
        assert_eq!(stack.capacity(), 4);
        assert!(stack.is_empty());
        stack.push(1).unwrap();
        assert!(!stack.is_empty());
        assert_eq!(stack.len(), 1);
    }

    #[test]
    fn push_then_pop_is_lifo_up_to_capacity() {
        let mut stack: ArrayStack<i32, 3> = ArrayStack::new();
        assert!(stack.push(1).is_ok());
        assert!(stack.push(2).is_ok());
        assert!(stack.push(3).is_ok());
        assert!(stack.is_full());
        assert_eq!(stack.pop(), Some(3));
        assert_eq!(stack.pop(), Some(2));
        assert_eq!(stack.pop(), Some(1));
        assert_eq!(stack.pop(), None);
    }

    #[test]
    fn pushing_past_capacity_returns_the_rejected_value() {
        let mut stack: ArrayStack<&str, 2> = ArrayStack::new();
        stack.push("a").unwrap();
        stack.push("b").unwrap();
        assert_eq!(stack.push("c"), Err("c"));
    }

    #[test]
    fn different_n_values_are_independent_types_with_correct_capacities() {
        let mut small: ArrayStack<i32, 1> = ArrayStack::new();
        assert_eq!(small.capacity(), 1);
        assert!(small.push(1).is_ok());
        assert_eq!(small.push(2), Err(2));

        let mut big: ArrayStack<i32, 8> = ArrayStack::new();
        assert_eq!(big.capacity(), 8);
        for i in 0..8 {
            assert!(big.push(i).is_ok());
        }
        assert!(big.is_full());
    }
}
