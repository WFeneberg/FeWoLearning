//! Exercise 073 — `RefCell` and runtime borrow checking (advanced).
//! Goal:   a type that mutates through a `&self` API using `RefCell` for
//!         interior mutability, and seeing that Rust still enforces the
//!         "one writer XOR many readers" rule — just at runtime, via a panic,
//!         instead of at compile time.
//! Drills: `RefCell::borrow`/`borrow_mut`, `BorrowMutError`, interior mutability.

use std::cell::RefCell;

pub struct Logger {
    entries: RefCell<Vec<String>>,
}

impl Logger {
    pub fn new() -> Self {
        Logger {
            entries: RefCell::new(Vec::new()),
        }
    }

    /// Appends `msg` to the log. Panics if a borrow of `entries` is already
    /// live elsewhere (e.g. a `borrow()`/`borrow_mut()` still in scope).
    pub fn log(&self, msg: &str) {
        todo!("push msg.to_string() onto self.entries via borrow_mut()")
    }

    pub fn entry_count(&self) -> usize {
        todo!("read self.entries.borrow().len()")
    }

    pub fn snapshot(&self) -> Vec<String> {
        self.entries.borrow().clone()
    }
}

impl Default for Logger {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn logging_appends_entries() {
        let logger = Logger::new();
        logger.log("start");
        logger.log("middle");
        logger.log("end");
        assert_eq!(logger.entry_count(), 3);
        assert_eq!(logger.snapshot(), vec!["start", "middle", "end"]);
    }

    #[test]
    fn a_fresh_logger_is_empty() {
        let logger = Logger::new();
        assert_eq!(logger.entry_count(), 0);
        logger.log("first");
        assert_eq!(logger.entry_count(), 1);
    }

    #[test]
    #[should_panic(expected = "already borrowed")]
    fn logging_while_holding_a_read_borrow_panics() {
        let logger = Logger::new();
        logger.log("first");
        let _guard = logger.entries.borrow(); // live immutable borrow
        logger.log("second"); // borrow_mut() while _guard is alive -> panics
    }
}
