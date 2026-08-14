//! Exercise 070 — `tests/` vs inline `#[cfg(test)]`, and visibility
//! (intermediate).
//! Goal:   see what a private helper buys you when tests live *inside* the
//!         crate versus what an external `tests/` integration test could
//!         ever reach.
//! Drills: unit tests (inline, same-module access to private items) vs.
//!         integration tests (a separate crate that only sees `pub` API),
//!         `pub use` re-exports.

mod library {
    pub struct Book {
        pub title: String,
        pages: u32,
    }

    impl Book {
        pub fn new(title: &str, pages: u32) -> Self {
            Book { title: title.to_string(), pages }
        }

        /// The only `pub` way in: everything below is private.
        pub fn reading_minutes(&self) -> u32 {
            minutes_for_pages(self.pages)
        }
    }

    // Private to `library`. An inline `#[cfg(test)] mod` living inside this
    // same file can call it directly (that's what a "unit test" gets you);
    // a real `tests/` integration test — a separate crate that only ever
    // sees `library`'s `pub` surface — could never name it at all.
    fn minutes_for_pages(pages: u32) -> u32 {
        todo!("minutes_for_pages({pages})")
    }
}

// Re-exported so callers outside this file use `Book` without spelling out
// `library::Book` — the same shape a crate's `lib.rs` typically re-exports
// through for the benefit of its `tests/` integration suite.
pub use library::Book;

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn reading_minutes_scales_with_pages() {
        // 3 minutes per 10 pages: 300 pages is 90 minutes of reading.
        let book = Book::new("Ownership", 300);
        assert_eq!(book.reading_minutes(), 90);
    }

    #[test]
    fn zero_pages_takes_no_time() {
        let book = Book::new("Blank", 0);
        assert_eq!(book.reading_minutes(), 0);
    }

    #[test]
    fn title_is_reachable_through_the_public_field_alongside_reading_minutes() {
        let book = Book::new("Ownership", 400);
        assert_eq!(book.title, "Ownership");
        assert_eq!(book.reading_minutes(), 120);
    }
}
