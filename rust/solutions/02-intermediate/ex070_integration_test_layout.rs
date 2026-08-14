//! Exercise 070 — `tests/` vs inline `#[cfg(test)]`, and visibility
//! (reference solution).

mod library {
    pub struct Book {
        pub title: String,
        pages: u32,
    }

    impl Book {
        pub fn new(title: &str, pages: u32) -> Self {
            Book { title: title.to_string(), pages }
        }

        pub fn reading_minutes(&self) -> u32 {
            minutes_for_pages(self.pages)
        }
    }

    fn minutes_for_pages(pages: u32) -> u32 {
        pages * 3 / 10
    }
}

pub use library::Book;

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
