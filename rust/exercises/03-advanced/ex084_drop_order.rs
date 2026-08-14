//! Exercise 084 — `Drop`, deterministic teardown, and controlling drop order (advanced).
//! Goal:   `Guard` records its own name into a shared log whenever it's
//!         dropped (this part is already implemented — it's necessary
//!         plumbing, not the drill). The drill is *controlling* when that
//!         happens: explicitly dropping values one at a time (instead of
//!         letting a whole collection or struct fall out of scope together),
//!         including inverting a struct's normal field drop order by
//!         destructuring it first.
//! Drills: `impl Drop`, `std::mem::drop`, destructuring to drop fields out
//!         of their declared order.

use std::cell::RefCell;
use std::rc::Rc;

pub type DropLog = Rc<RefCell<Vec<String>>>;

/// Records its own name into `log` when dropped.
pub struct Guard {
    name: String,
    log: DropLog,
}

impl Guard {
    pub fn new(name: &str, log: &DropLog) -> Self {
        Guard {
            name: name.to_string(),
            log: Rc::clone(log),
        }
    }
}

impl Drop for Guard {
    fn drop(&mut self) {
        self.log.borrow_mut().push(self.name.clone());
    }
}

pub struct Pair {
    pub first: Guard,
    pub second: Guard,
}

/// Explicitly drops every item of `items`, one at a time, in iteration
/// order — as opposed to letting the whole `Vec` (and everything in it)
/// drop together at the end of some scope. Returns the log's contents
/// afterwards.
pub fn drop_each_explicitly(items: Vec<Guard>, log: &DropLog) -> Vec<String> {
    todo!("loop over items, calling drop(item) on each, then return log.borrow().clone()")
}

/// Drops `pair`'s `second` field before its `first` field: the OPPOSITE of
/// what plain `drop(pair)` would do (a `Pair` with no custom `Drop` of its
/// own drops fields in declaration order — `first`, then `second`).
/// Only possible by destructuring the struct apart first.
pub fn drop_second_then_first(pair: Pair, log: &DropLog) -> Vec<String> {
    todo!("destructure pair into first/second, drop(second), then drop(first), then return log.borrow().clone()")
}

/// Drops `pair` as a whole, letting its fields fall in their natural
/// declaration order. Provided for contrast with `drop_second_then_first`.
pub fn drop_pair_naturally(pair: Pair) {
    drop(pair);
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn drop_each_explicitly_runs_in_iteration_order() {
        let log: DropLog = Rc::new(RefCell::new(Vec::new()));
        let items = vec![Guard::new("a", &log), Guard::new("b", &log), Guard::new("c", &log)];
        let recorded = drop_each_explicitly(items, &log);
        assert_eq!(recorded, vec!["a", "b", "c"]);
    }

    #[test]
    fn drop_each_explicitly_on_an_empty_vec_records_nothing() {
        let log: DropLog = Rc::new(RefCell::new(Vec::new()));
        let recorded = drop_each_explicitly(Vec::new(), &log);
        assert!(recorded.is_empty());
    }

    #[test]
    fn drop_second_then_first_inverts_the_natural_field_order() {
        let log: DropLog = Rc::new(RefCell::new(Vec::new()));
        let pair = Pair {
            first: Guard::new("first", &log),
            second: Guard::new("second", &log),
        };
        let recorded = drop_second_then_first(pair, &log);
        assert_eq!(recorded, vec!["second", "first"]);
    }

    #[test]
    fn inverted_order_differs_from_the_natural_order() {
        let log: DropLog = Rc::new(RefCell::new(Vec::new()));
        let natural_pair = Pair {
            first: Guard::new("first", &log),
            second: Guard::new("second", &log),
        };
        drop_pair_naturally(natural_pair);
        let natural_order = log.borrow().clone();
        assert_eq!(natural_order, vec!["first", "second"]);

        log.borrow_mut().clear();
        let inverted_pair = Pair {
            first: Guard::new("first", &log),
            second: Guard::new("second", &log),
        };
        let inverted_order = drop_second_then_first(inverted_pair, &log);
        assert_ne!(natural_order, inverted_order);
        assert_eq!(inverted_order, vec!["second", "first"]);
    }
}
