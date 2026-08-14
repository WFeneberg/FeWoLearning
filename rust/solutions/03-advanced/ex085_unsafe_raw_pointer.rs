//! Exercise 085 — Raw pointers and the invariants `unsafe` must uphold (reference solution).

pub fn swap_via_raw_pointers<T>(slice: &mut [T], i: usize, j: usize) {
    assert!(i < slice.len() && j < slice.len(), "index out of bounds");
    if i == j {
        return;
    }
    let ptr = slice.as_mut_ptr();
    // SAFETY: `i` and `j` are both checked `< slice.len()` above, so
    // `ptr.add(i)` and `ptr.add(j)` point within the single allocation
    // backing `slice` and are properly aligned for `T` (derived from a
    // valid `&mut [T]`). Since `i != j`, the two addresses are distinct, so
    // `ptr::swap` never reads/writes the same memory as both its source and
    // destination at once.
    unsafe {
        std::ptr::swap(ptr.add(i), ptr.add(j));
    }
}

pub fn split_at_mut_manual<T>(slice: &mut [T], mid: usize) -> (&mut [T], &mut [T]) {
    let len = slice.len();
    assert!(mid <= len, "mid out of bounds");
    let ptr = slice.as_mut_ptr();
    // SAFETY: `mid <= len` (checked above), so both `[0, mid)` and
    // `[mid, len)` are index ranges entirely within the original
    // allocation. The two ranges are disjoint, so the resulting `&mut [T]`
    // slices never alias each other even though both are derived from the
    // same base pointer — upholding the exclusive-borrow invariant that
    // `&mut` normally relies on the compiler to enforce.
    unsafe {
        (
            std::slice::from_raw_parts_mut(ptr, mid),
            std::slice::from_raw_parts_mut(ptr.add(mid), len - mid),
        )
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn swap_via_raw_pointers_swaps_two_positions() {
        let mut v = vec![1, 2, 3, 4];
        swap_via_raw_pointers(&mut v, 0, 3);
        assert_eq!(v, vec![4, 2, 3, 1]);
    }

    #[test]
    fn swap_via_raw_pointers_is_a_no_op_for_the_same_index() {
        let mut v = vec![1, 2, 3];
        swap_via_raw_pointers(&mut v, 1, 1);
        assert_eq!(v, vec![1, 2, 3]);
    }

    #[test]
    fn swap_via_raw_pointers_works_for_non_copy_types() {
        let mut v = vec!["a".to_string(), "b".to_string(), "c".to_string()];
        swap_via_raw_pointers(&mut v, 0, 2);
        assert_eq!(v, vec!["c".to_string(), "b".to_string(), "a".to_string()]);
    }

    #[test]
    #[should_panic(expected = "out of bounds")]
    fn swap_via_raw_pointers_panics_on_out_of_bounds_index() {
        let mut v = vec![1, 2, 3];
        swap_via_raw_pointers(&mut v, 0, 10);
    }

    #[test]
    fn split_at_mut_manual_produces_two_independently_mutable_halves() {
        let mut v = vec![1, 2, 3, 4, 5];
        {
            let (left, right) = split_at_mut_manual(&mut v, 2);
            assert_eq!(left, &mut [1, 2]);
            assert_eq!(right, &mut [3, 4, 5]);
            left[0] = 100;
            right[0] = 200;
        }
        assert_eq!(v, vec![100, 2, 200, 4, 5]);
    }

    #[test]
    fn split_at_mut_manual_handles_edge_boundaries() {
        let mut v = vec![1, 2, 3];
        {
            let (left, right) = split_at_mut_manual(&mut v, 0);
            assert!(left.is_empty());
            assert_eq!(right, &mut [1, 2, 3]);
        }
        {
            let (left, right) = split_at_mut_manual(&mut v, 3);
            assert_eq!(left, &mut [1, 2, 3]);
            assert!(right.is_empty());
        }
    }

    #[test]
    #[should_panic(expected = "out of bounds")]
    fn split_at_mut_manual_panics_when_mid_exceeds_length() {
        let mut v = vec![1, 2, 3];
        let _ = split_at_mut_manual(&mut v, 10);
    }
}
