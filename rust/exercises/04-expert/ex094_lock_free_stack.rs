//! Exercise 094 — A lock-free stack: CAS loops, `AtomicPtr`, and the ABA
//! problem (expert).
//! Goal:   a Treiber stack — `push`/`pop` swing a single `AtomicPtr<Node<T>>`
//!         head through a compare-and-swap retry loop, no mutex anywhere.
//!         Simplification, spelled out up front: `pop` deliberately LEAKS the
//!         popped node's allocation (never `Box::from_raw`s + drops it)
//!         instead of freeing it. That sidesteps the classic ABA/use-after-
//!         free hazard a real lock-free stack needs epoch-based reclamation
//!         or hazard pointers to solve properly (out of scope here) — because
//!         a node's memory is never freed, no CAS loop can ever read a freed
//!         `.next` pointer, and no freed address can be reallocated out from
//!         under a racing thread. The value itself is still moved out exactly
//!         once (by whichever thread's CAS wins the pop), so nothing is
//!         double-read or double-dropped.
//! Drills: `AtomicPtr<T>`, CAS retry loops, `Ordering::{Acquire,Release,AcqRel}`,
//!         why `Send`/`Sync` for a hand-rolled concurrent type must be written
//!         by hand (bounded on `T: Send`) rather than left to auto-derivation.

use std::marker::PhantomData;
use std::ptr;
use std::sync::atomic::{AtomicPtr, Ordering};

struct Node<T> {
    value: T,
    next: *mut Node<T>,
}

/// A lock-free (Treiber) stack. See the module doc above for the deliberate
/// "leak on pop" simplification and why it's sound.
pub struct LockFreeStack<T> {
    head: AtomicPtr<Node<T>>,
    // A bare `AtomicPtr<Node<T>>` is unconditionally `Send`/`Sync` in `std`
    // regardless of `T`, which would make this struct auto-derive `Send`/
    // `Sync` even for a `!Send` `T` — unsound, since `push`/`pop` move a `T`
    // across threads. `PhantomData<*mut T>` is itself neither `Send` nor
    // `Sync`, which opts this struct OUT of auto-derivation so the manual,
    // correctly-bounded impls below are the only source of `Send`/`Sync`.
    _marker: PhantomData<*mut T>,
}

// SAFETY: a given `T` value lives in exactly one node, and a node is only
// ever read/owned by the single thread whose `compare_exchange` won the race
// that pushed or popped it, so moving the whole stack — or a `T` through it —
// across threads is sound whenever `T: Send` (the same bound `std::sync::
// Mutex<T>` uses for both its `Send` and `Sync` impls; `T` need not be `Sync`
// since no two threads ever observe the same live `T` at once).
unsafe impl<T: Send> Send for LockFreeStack<T> {}
unsafe impl<T: Send> Sync for LockFreeStack<T> {}

impl<T> LockFreeStack<T> {
    pub fn new() -> Self {
        Self {
            head: AtomicPtr::new(ptr::null_mut()),
            _marker: PhantomData,
        }
    }

    /// Pushes `value` onto the top of the stack.
    pub fn push(&self, value: T) {
        todo!(
            "Box::into_raw a new Node{{ value, next: null }}; loop: load head (Acquire), \
             write it into new_node.next, then compare_exchange(old, new_node, AcqRel, Acquire); \
             retry the loop on failure, return on success"
        )
    }

    /// Pops and returns the top value, or `None` if the stack is empty.
    pub fn pop(&self) -> Option<T> {
        todo!(
            "loop: load head (Acquire); if null return None; read its `next`; \
             compare_exchange(old, next, AcqRel, Acquire); on success, ptr::read \
             the value out of the (now-detached, deliberately leaked) node and \
             return Some(value); retry the loop on failure"
        )
    }
}

impl<T> Default for LockFreeStack<T> {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::sync::Arc;
    use std::thread;

    #[test]
    fn push_then_pop_is_lifo_single_threaded() {
        let stack = LockFreeStack::new();
        stack.push(1);
        stack.push(2);
        stack.push(3);
        assert_eq!(stack.pop(), Some(3));
        assert_eq!(stack.pop(), Some(2));
        assert_eq!(stack.pop(), Some(1));
        assert_eq!(stack.pop(), None);
    }

    #[test]
    fn pop_on_empty_stack_returns_none() {
        let stack: LockFreeStack<i32> = LockFreeStack::new();
        assert_eq!(stack.pop(), None);
    }

    #[test]
    fn concurrent_pushes_from_many_threads_are_all_recorded() {
        let stack = Arc::new(LockFreeStack::new());
        let mut handles = Vec::new();
        for t in 0..8i32 {
            let stack = Arc::clone(&stack);
            handles.push(thread::spawn(move || {
                for i in 0..100i32 {
                    stack.push(t * 100 + i);
                }
            }));
        }
        for h in handles {
            h.join().unwrap();
        }

        let mut popped = Vec::new();
        while let Some(v) = stack.pop() {
            popped.push(v);
        }
        popped.sort();
        let expected: Vec<i32> = (0..800).collect();
        assert_eq!(popped, expected);
    }
}
