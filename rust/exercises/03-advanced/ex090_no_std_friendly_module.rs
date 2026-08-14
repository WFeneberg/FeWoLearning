//! Exercise 090 — Writing `core`-only code inside an otherwise-`std` crate (advanced).
//! Goal:   `FixedString<N>` is a fixed-capacity text buffer built entirely
//!         from `core` items — no `String`, no `Vec`, no heap allocation at
//!         all. That's deliberate: this is exactly the kind of type a real
//!         `#![no_std]` crate (embedded firmware, a kernel, a `wasm` module
//!         with no allocator) needs, and everything above the test module
//!         would keep compiling unmodified if this file were moved into one.
//!         The test module below still uses `std` freely — tests always run
//!         under the ordinary std test harness, regardless of what the code
//!         under test does or doesn't depend on.
//! Drills: `core::fmt::Write` (the same trait `std::fmt::Write` re-exports),
//!         fixed-size buffers instead of heap allocation, why `no_std` code
//!         reaches for `core::` paths instead of `std::` ones.

use core::fmt::{self, Write};

pub struct FixedString<const N: usize> {
    buf: [u8; N],
    len: usize,
}

impl<const N: usize> FixedString<N> {
    pub fn new() -> Self {
        FixedString { buf: [0u8; N], len: 0 }
    }

    pub fn as_str(&self) -> &str {
        core::str::from_utf8(&self.buf[..self.len]).expect("buffer holds valid UTF-8 by construction")
    }

    pub fn capacity(&self) -> usize {
        N
    }
}

impl<const N: usize> Default for FixedString<N> {
    fn default() -> Self {
        Self::new()
    }
}

impl<const N: usize> Write for FixedString<N> {
    fn write_str(&mut self, s: &str) -> fmt::Result {
        todo!(
            "if self.len + s.len() > N, return Err(fmt::Error); otherwise copy s's bytes \
             into self.buf starting at self.len, then bump self.len"
        )
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use core::fmt::Write as _;

    #[test]
    fn writing_within_capacity_succeeds() {
        let mut buf: FixedString<16> = FixedString::new();
        write!(buf, "hello {}", 42).unwrap();
        assert_eq!(buf.as_str(), "hello 42");
    }

    #[test]
    fn multiple_writes_append() {
        let mut buf: FixedString<10> = FixedString::new();
        write!(buf, "ab").unwrap();
        write!(buf, "cd").unwrap();
        assert_eq!(buf.as_str(), "abcd");
    }

    #[test]
    fn writing_past_capacity_fails_without_partial_corruption() {
        let mut buf: FixedString<4> = FixedString::new();
        write!(buf, "ab").unwrap();
        assert!(write!(buf, "cdef").is_err());
        // The rejected write must leave the buffer exactly as it was.
        assert_eq!(buf.as_str(), "ab");
    }

    #[test]
    fn a_fresh_buffer_reports_its_const_generic_capacity() {
        let mut buf: FixedString<32> = FixedString::new();
        assert_eq!(buf.capacity(), 32);
        assert_eq!(buf.as_str(), "");
        write!(buf, "x").unwrap();
        assert_eq!(buf.as_str(), "x");
    }
}
