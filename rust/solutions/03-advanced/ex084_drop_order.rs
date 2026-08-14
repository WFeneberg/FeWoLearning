//! Exercise 084 — `Drop`, deterministic teardown, and controlling drop order (reference solution).

use std::cell::RefCell;
use std::rc::Rc;

pub type DropLog = Rc<RefCell<Vec<String>>>;

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

pub fn drop_each_explicitly(items: Vec<Guard>, log: &DropLog) -> Vec<String> {
    for item in items {
        drop(item);
    }
    log.borrow().clone()
}

pub fn drop_second_then_first(pair: Pair, log: &DropLog) -> Vec<String> {
    let Pair { first, second } = pair;
    drop(second);
    drop(first);
    log.borrow().clone()
}

pub fn drop_pair_naturally(pair: Pair) {
    drop(pair);
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
