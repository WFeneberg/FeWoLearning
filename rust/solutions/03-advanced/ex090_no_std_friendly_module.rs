//! Exercise 090 — Writing `core`-only code inside an otherwise-`std` crate (reference solution).

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
        let bytes = s.as_bytes();
        if self.len + bytes.len() > N {
            return Err(fmt::Error);
        }
        self.buf[self.len..self.len + bytes.len()].copy_from_slice(bytes);
        self.len += bytes.len();
        Ok(())
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
