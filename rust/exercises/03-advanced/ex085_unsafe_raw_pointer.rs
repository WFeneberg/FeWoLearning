//! Exercise 085 — Raw pointers and the invariants `unsafe` must uphold (advanced).
//! Goal:   two small building blocks from `std` itself, re-implemented by
//!         hand with raw pointers: swapping two elements, and splitting a
//!         slice into two independently-mutable halves. Both are only sound
//!         because of invariants YOU must guarantee the compiler can no
//!         longer check for you — write a `// SAFETY:` comment on every
//!         `unsafe` block spelling out exactly which invariant holds and why.
//! Drills: `*mut T`, `ptr::swap`, `slice::from_raw_parts_mut`, bounds checks
//!         that must happen *before* the unsafe block, not after.

/// Swaps the elements at `i` and `j` within `slice`, using raw pointers
/// directly instead of the safe `slice.swap(i, j)`.
pub fn swap_via_raw_pointers<T>(slice: &mut [T], i: usize, j: usize) {
    todo!(
        "assert i < slice.len() && j < slice.len(); if i == j return; \
         otherwise get slice.as_mut_ptr(), then unsafe {{ std::ptr::swap(ptr.add(i), ptr.add(j)) }} \
         with a SAFETY comment justifying it"
    )
}

/// A hand-rolled equivalent of `<[T]>::split_at_mut`: splits `slice` into
/// `(&mut slice[..mid], &mut slice[mid..])` using one raw pointer, proving
/// the two halves are provably non-aliasing by construction (disjoint index
/// ranges), which is exactly why the real `split_at_mut` needs `unsafe` too.
pub fn split_at_mut_manual<T>(slice: &mut [T], mid: usize) -> (&mut [T], &mut [T]) {
    todo!(
        "assert mid <= slice.len(); get slice.as_mut_ptr(), then unsafe {{ build two \
         slice::from_raw_parts_mut non-overlapping halves }} with a SAFETY comment"
    )
}

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
