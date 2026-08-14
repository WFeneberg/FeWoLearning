//! Exercise 009 — Slice basics (reference solution).

pub fn middle_slice(v: &[i32]) -> &[i32] {
    if v.len() < 2 {
        &[]
    } else {
        &v[1..v.len() - 1]
    }
}

pub fn safe_third(v: &[i32]) -> Option<i32> {
    v.get(2).copied()
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
