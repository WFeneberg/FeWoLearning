//! Exercise 099 — Zero-copy binary decoding: byte slices, borrowed views
//! (expert).
//! Goal:   decode a simple TLV (type-length-value) binary format WITHOUT
//!         copying the payload bytes anywhere: every decoded `Record`'s
//!         `value` is a `&[u8]` slice borrowed directly from the input
//!         buffer, never an owned `Vec<u8>`. The API only ever hands out
//!         borrowed views — there is no owned alternative to reach for, so
//!         allocating a copy of the payload isn't even possible by
//!         accident.
//! Drills: `&'a [u8]` sub-slices that alias the caller's buffer, manual
//!         little-endian integer decoding, `str::from_utf8` over a borrowed
//!         slice, bounds-checked parsing that returns `Err` instead of
//!         panicking/indexing out of range.

use std::str::Utf8Error;

/// One decoded type-length-value record: `record_type` is a single tag
/// byte, `value` is a slice borrowed directly from the buffer
/// `decode_records` was given — no copy.
#[derive(Debug, PartialEq)]
pub struct Record<'a> {
    pub record_type: u8,
    pub value: &'a [u8],
}

impl<'a> Record<'a> {
    /// Interprets `value` as UTF-8 without copying it.
    pub fn as_str(&self) -> Result<&'a str, Utf8Error> {
        std::str::from_utf8(self.value)
    }
}

/// A decoding error, tagged with the byte offset into the input at which it
/// occurred.
#[derive(Debug, PartialEq)]
pub struct DecodeError {
    pub offset: usize,
    pub message: &'static str,
}

/// Decodes `buf` into a sequence of records. Each record is laid out as
/// `[type: u8][length: u16 little-endian][value: length bytes]`, with
/// records packed back-to-back until `buf` is exhausted.
pub fn decode_records(buf: &[u8]) -> Result<Vec<Record<'_>>, DecodeError> {
    let mut records = Vec::new();
    let mut offset = 0;
    while offset < buf.len() {
        if offset + 3 > buf.len() {
            return Err(DecodeError {
                offset,
                message: "truncated record header (need 1 type byte + 2 length bytes)",
            });
        }
        let record_type = buf[offset];
        let len = u16::from_le_bytes([buf[offset + 1], buf[offset + 2]]) as usize;
        let value_start = offset + 3;
        let value_end = value_start + len;
        if value_end > buf.len() {
            return Err(DecodeError {
                offset,
                message: "declared record length exceeds remaining buffer",
            });
        }
        records.push(Record {
            record_type,
            value: &buf[value_start..value_end],
        });
        offset = value_end;
    }
    Ok(records)
}

#[cfg(test)]
mod tests {
    use super::*;

    fn record(record_type: u8, value: &[u8]) -> Vec<u8> {
        let mut bytes = vec![record_type];
        bytes.extend_from_slice(&(value.len() as u16).to_le_bytes());
        bytes.extend_from_slice(value);
        bytes
    }

    #[test]
    fn decodes_a_single_record() {
        let buf = record(1, b"hello");
        let records = decode_records(&buf).unwrap();
        assert_eq!(records.len(), 1);
        assert_eq!(records[0].record_type, 1);
        assert_eq!(records[0].value, b"hello");
    }

    #[test]
    fn decoded_value_borrows_directly_from_the_input_buffer() {
        let buf = record(1, b"hello");
        let records = decode_records(&buf).unwrap();
        // Prove zero-copy: the returned slice's data pointer must fall
        // within `buf`'s own address range, not point at a fresh allocation.
        let buf_start = buf.as_ptr() as usize;
        let buf_range = buf_start..(buf_start + buf.len());
        let value_ptr = records[0].value.as_ptr() as usize;
        assert!(buf_range.contains(&value_ptr));
    }

    #[test]
    fn decodes_multiple_back_to_back_records() {
        let mut buf = record(1, b"foo");
        buf.extend(record(2, b"bar"));
        buf.extend(record(3, b""));
        let records = decode_records(&buf).unwrap();
        assert_eq!(records.len(), 3);
        assert_eq!(records[0].value, b"foo");
        assert_eq!(records[1].value, b"bar");
        assert_eq!(records[2].value, b"");
    }

    #[test]
    fn as_str_decodes_utf8_without_allocating() {
        let buf = record(9, "héllo".as_bytes());
        let records = decode_records(&buf).unwrap();
        assert_eq!(records[0].as_str().unwrap(), "héllo");
    }

    #[test]
    fn as_str_reports_invalid_utf8() {
        let buf = record(9, &[0xff, 0xfe]);
        let records = decode_records(&buf).unwrap();
        assert!(records[0].as_str().is_err());
    }

    #[test]
    fn truncated_header_is_an_error() {
        // A single type byte with no length bytes at all.
        let buf = vec![1u8];
        assert!(decode_records(&buf).is_err());
    }

    #[test]
    fn declared_length_longer_than_remaining_buffer_is_an_error() {
        let mut buf = vec![1u8];
        buf.extend_from_slice(&10u16.to_le_bytes()); // claims 10 bytes follow
        buf.extend_from_slice(b"short"); // only 5 actually do
        assert!(decode_records(&buf).is_err());
    }

    #[test]
    fn empty_buffer_decodes_to_zero_records() {
        let records = decode_records(&[]).unwrap();
        assert!(records.is_empty());
    }
}
