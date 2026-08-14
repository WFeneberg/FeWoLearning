//! Exercise 060 — A generic `Stack<T>` (intermediate).
//! Goal:   a LIFO container that works for any element type, with no
//!         trait bound needed since it never inspects its elements.
//! Drills: `struct Stack<T>`, `impl<T> Stack<T>`, generic methods.

pub struct Stack<T> {
    items: Vec<T>,
}

impl<T> Stack<T> {
    pub fn new() -> Self {
        Stack { items: Vec::new() }
    }

    pub fn push(&mut self, item: T) {
        todo!("Stack::push")
    }

    pub fn pop(&mut self) -> Option<T> {
        todo!("Stack::pop")
    }

    pub fn peek(&self) -> Option<&T> {
        todo!("Stack::peek")
    }

    pub fn is_empty(&self) -> bool {
        todo!("Stack::is_empty")
    }

    pub fn len(&self) -> usize {
        todo!("Stack::len")
    }
}

impl<T> Default for Stack<T> {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn new_stack_is_empty() {
        let stack: Stack<i32> = Stack::new();
        assert!(stack.is_empty());
        assert_eq!(stack.len(), 0);
    }

    #[test]
    fn push_then_pop_is_lifo() {
        let mut stack = Stack::new();
        stack.push(1);
        stack.push(2);
        stack.push(3);
        assert_eq!(stack.pop(), Some(3));
        assert_eq!(stack.pop(), Some(2));
        assert_eq!(stack.pop(), Some(1));
        assert_eq!(stack.pop(), None);
    }

    #[test]
    fn peek_does_not_remove() {
        let mut stack = Stack::new();
        stack.push("a".to_string());
        assert_eq!(stack.peek(), Some(&"a".to_string()));
        assert_eq!(stack.len(), 1);
    }

    #[test]
    fn works_with_any_element_type() {
        let mut stack = Stack::new();
        stack.push(vec![1, 2]);
        stack.push(vec![3]);
        assert_eq!(stack.pop(), Some(vec![3]));
        assert_eq!(stack.len(), 1);
    }
}
