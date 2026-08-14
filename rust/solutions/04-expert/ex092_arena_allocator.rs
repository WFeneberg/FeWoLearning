//! Exercise 092 — Arena allocation: index handles instead of references
//! (expert).
//! Goal:   an `Arena<T>` that owns a growable pool of `T` values and hands
//!         out opaque `Handle<T>` VALUES (a slot index plus a generation
//!         counter) instead of `&T`/`&mut T` references. Handles are `Copy`,
//!         carry no lifetime, and can be stored anywhere (a `Vec<Handle<T>>`,
//!         another arena-allocated struct, ...) — the classic escape hatch
//!         from borrow-checker-fighting graph/tree structures. Removing a
//!         slot bumps its generation, so a handle taken out BEFORE the
//!         removal is detected as stale (`get` returns `None`) instead of
//!         silently reading whatever unrelated value got inserted into the
//!         reused slot afterwards.
//! Drills: index-based "references", generation counters for stale-handle
//!         detection, a free-list for slot reuse.

use std::marker::PhantomData;

/// An opaque handle to a value stored in an [`Arena<T>`]. `Copy`, carries no
/// borrow, and becomes stale (fails `get`/`get_mut`/`remove`) once its slot
/// has been removed and reused.
pub struct Handle<T> {
    index: usize,
    generation: u32,
    // `fn() -> T` (rather than `T`) keeps `Handle<T>` `Copy`/`Send`/`Sync`
    // regardless of whether `T` is — the handle doesn't actually own or
    // expose a `T`, it's just an index plus a generation tag.
    _marker: PhantomData<fn() -> T>,
}

impl<T> Clone for Handle<T> {
    fn clone(&self) -> Self {
        *self
    }
}

impl<T> Copy for Handle<T> {}

impl<T> std::fmt::Debug for Handle<T> {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        f.debug_struct("Handle")
            .field("index", &self.index)
            .field("generation", &self.generation)
            .finish()
    }
}

impl<T> PartialEq for Handle<T> {
    fn eq(&self, other: &Self) -> bool {
        self.index == other.index && self.generation == other.generation
    }
}

#[cfg(test)]
impl<T> Handle<T> {
    fn for_test(index: usize, generation: u32) -> Self {
        Self {
            index,
            generation,
            _marker: PhantomData,
        }
    }
}

enum Slot<T> {
    Occupied { value: T, generation: u32 },
    Vacant { generation: u32 },
}

/// A slab/arena of `T`s, addressed by [`Handle<T>`] rather than by reference.
pub struct Arena<T> {
    slots: Vec<Slot<T>>,
    free_list: Vec<usize>,
}

impl<T> Arena<T> {
    pub fn new() -> Self {
        Self {
            slots: Vec::new(),
            free_list: Vec::new(),
        }
    }

    /// Inserts `value`, reusing a freed slot (with its generation bumped) if
    /// one is available, otherwise growing the arena. Returns a `Handle`
    /// that stays valid until that slot is next removed.
    pub fn insert(&mut self, value: T) -> Handle<T> {
        if let Some(index) = self.free_list.pop() {
            let generation = match &self.slots[index] {
                Slot::Vacant { generation } => *generation,
                Slot::Occupied { .. } => unreachable!("free_list index must be Vacant"),
            };
            self.slots[index] = Slot::Occupied { value, generation };
            Handle {
                index,
                generation,
                _marker: PhantomData,
            }
        } else {
            let index = self.slots.len();
            let generation = 0;
            self.slots.push(Slot::Occupied { value, generation });
            Handle {
                index,
                generation,
                _marker: PhantomData,
            }
        }
    }

    /// Returns `Some(&T)` if `handle` still refers to a live value (matching
    /// index AND generation), `None` if it's out of range or stale.
    pub fn get(&self, handle: Handle<T>) -> Option<&T> {
        match self.slots.get(handle.index) {
            Some(Slot::Occupied { value, generation }) if *generation == handle.generation => {
                Some(value)
            }
            _ => None,
        }
    }

    /// Mutable counterpart to [`Arena::get`].
    pub fn get_mut(&mut self, handle: Handle<T>) -> Option<&mut T> {
        match self.slots.get_mut(handle.index) {
            Some(Slot::Occupied { value, generation }) if *generation == handle.generation => {
                Some(value)
            }
            _ => None,
        }
    }

    /// Removes and returns the value `handle` refers to, if it's still live.
    /// Bumps the slot's generation so any other outstanding handle to the
    /// same slot becomes stale, and returns the slot's index to the free
    /// list for reuse.
    pub fn remove(&mut self, handle: Handle<T>) -> Option<T> {
        match self.slots.get(handle.index) {
            Some(Slot::Occupied { generation, .. }) if *generation == handle.generation => {
                let next_generation = generation + 1;
                let old = std::mem::replace(
                    &mut self.slots[handle.index],
                    Slot::Vacant {
                        generation: next_generation,
                    },
                );
                self.free_list.push(handle.index);
                match old {
                    Slot::Occupied { value, .. } => Some(value),
                    Slot::Vacant { .. } => unreachable!(),
                }
            }
            _ => None,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn insert_and_get_roundtrip() {
        let mut arena = Arena::new();
        let handle = arena.insert("hello");
        assert_eq!(arena.get(handle), Some(&"hello"));
    }

    #[test]
    fn distinct_inserts_get_distinct_handles() {
        let mut arena = Arena::new();
        let h1 = arena.insert(1);
        let h2 = arena.insert(2);
        assert_ne!(h1, h2);
        assert_eq!(arena.get(h1), Some(&1));
        assert_eq!(arena.get(h2), Some(&2));
    }

    #[test]
    fn get_mut_allows_in_place_mutation() {
        let mut arena = Arena::new();
        let handle = arena.insert(10);
        *arena.get_mut(handle).unwrap() += 5;
        assert_eq!(arena.get(handle), Some(&15));
    }

    #[test]
    fn remove_returns_the_value_and_empties_the_slot() {
        let mut arena = Arena::new();
        let handle = arena.insert("bye");
        assert_eq!(arena.remove(handle), Some("bye"));
        assert_eq!(arena.get(handle), None);
    }

    #[test]
    fn removed_handle_stays_stale_even_after_slot_is_reused() {
        let mut arena = Arena::new();
        let old_handle = arena.insert("first");
        arena.remove(old_handle).unwrap();
        // Reuses the freed slot (same index, bumped generation).
        let new_handle = arena.insert("second");
        assert_ne!(old_handle, new_handle);
        assert_eq!(
            arena.get(old_handle),
            None,
            "stale handle must not see the new value"
        );
        assert_eq!(arena.get(new_handle), Some(&"second"));
    }

    #[test]
    fn get_on_an_out_of_range_handle_is_none() {
        let arena: Arena<i32> = Arena::new();
        let bogus = Handle::<i32>::for_test(999, 0);
        assert_eq!(arena.get(bogus), None);
    }
}
