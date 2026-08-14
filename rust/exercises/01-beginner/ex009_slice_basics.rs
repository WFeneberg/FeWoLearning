//! Exercise 009 — Slice basics (beginner).
//! Goal:   slice out the "middle" of a list (everything but the first and
//!         last element) and safely fetch an element by index.
//! Drills: slices, range indexing (`[1..len-1]`), bounds-checked `.get`.

pub fn middle_slice(v: &[i32]) -> &[i32] {
    todo!("middle_slice({v:?})")
}

pub fn safe_third(v: &[i32]) -> Option<i32> {
    todo!("safe_third({v:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn strips_first_and_last() {
        assert_eq!(middle_slice(&[1, 2, 3, 4, 5]), &[2, 3, 4]);
    }

    #[test]
    fn short_slices_have_no_middle() {
        assert_eq!(middle_slice(&[1, 2]), &[] as &[i32]);
        assert_eq!(middle_slice(&[1]), &[] as &[i32]);
        assert_eq!(middle_slice(&[]), &[] as &[i32]);
    }

    #[test]
    fn safe_third_returns_none_out_of_bounds() {
        assert_eq!(safe_third(&[1, 2]), None);
    }

    #[test]
    fn safe_third_returns_element_when_present() {
        assert_eq!(safe_third(&[1, 2, 3, 4]), Some(3));
    }
}
