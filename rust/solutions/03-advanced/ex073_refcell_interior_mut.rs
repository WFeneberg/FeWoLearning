//! Exercise 073 — `RefCell` and runtime borrow checking (reference solution).

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

    pub fn log(&self, msg: &str) {
        self.entries.borrow_mut().push(msg.to_string());
    }

    pub fn entry_count(&self) -> usize {
        self.entries.borrow().len()
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

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
