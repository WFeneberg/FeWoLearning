//! Exercise 074 — A parent/child tree with `Rc<RefCell<T>>` and `Weak` (reference solution).

use std::cell::RefCell;
use std::rc::{Rc, Weak};

pub struct Node {
    pub value: i32,
    children: RefCell<Vec<Rc<Node>>>,
    parent: RefCell<Weak<Node>>,
}

impl Node {
    pub fn new(value: i32) -> Rc<Node> {
        Rc::new(Node {
            value,
            children: RefCell::new(Vec::new()),
            parent: RefCell::new(Weak::new()),
        })
    }

    pub fn child_count(&self) -> usize {
        self.children.borrow().len()
    }

    pub fn add_child(parent: &Rc<Node>, child: Rc<Node>) {
        *child.parent.borrow_mut() = Rc::downgrade(parent);
        parent.children.borrow_mut().push(child);
    }

    pub fn parent_value(&self) -> Option<i32> {
        self.parent.borrow().upgrade().map(|p| p.value)
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn child_can_reach_its_parent_value() {
        let root = Node::new(1);
        let child = Node::new(2);
        Node::add_child(&root, Rc::clone(&child));

        assert_eq!(root.child_count(), 1);
        assert_eq!(child.parent_value(), Some(1));
    }

    #[test]
    fn multiple_children_are_all_attached() {
        let root = Node::new(10);
        let a = Node::new(1);
        let b = Node::new(2);
        Node::add_child(&root, Rc::clone(&a));
        Node::add_child(&root, Rc::clone(&b));

        assert_eq!(root.child_count(), 2);
        assert_eq!(a.parent_value(), Some(10));
        assert_eq!(b.parent_value(), Some(10));
    }

    #[test]
    fn a_root_node_has_no_parent() {
        let root = Node::new(42);
        assert_eq!(root.parent_value(), None);
    }

    #[test]
    fn weak_parent_ref_does_not_keep_the_parent_alive() {
        let child = {
            let root = Node::new(99);
            let child = Node::new(5);
            Node::add_child(&root, Rc::clone(&child));
            assert_eq!(child.parent_value(), Some(99));
            child
            // `root` is dropped here: its only strong owner (this scope) ends,
            // and `child`'s back-reference is only a `Weak`, so it doesn't
            // keep `root` alive.
        };
        assert_eq!(child.parent_value(), None);
    }
}
