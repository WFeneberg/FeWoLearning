//! Exercise 072 — Shared immutable ownership with `Rc` (advanced).
//! Goal:   several independent "owners" sharing one heap-allocated `Resource`
//!         without cloning its data, and observing the strong reference count
//!         rise and fall as owners come and go.
//! Drills: `Rc::new`, `Rc::clone`, `Rc::strong_count`, shared immutable data.

use std::rc::Rc;

pub struct Resource {
    pub name: String,
}

pub struct Owner {
    pub label: String,
    pub resource: Rc<Resource>,
}

/// Wraps a fresh `Resource` in an `Rc` (strong count starts at 1).
pub fn make_shared_resource(name: &str) -> Rc<Resource> {
    todo!("Rc::new(Resource {{ name: name.to_string() }})")
}

/// Attaches a new `Owner` to an existing shared resource by cloning the `Rc`
/// handle (this bumps the strong count; it does NOT clone `Resource` itself).
pub fn attach_owner(label: &str, resource: &Rc<Resource>) -> Owner {
    todo!("Owner {{ label: label.to_string(), resource: Rc::clone(resource) }}")
}

pub fn strong_count(resource: &Rc<Resource>) -> usize {
    Rc::strong_count(resource)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_fresh_resource_has_one_owner() {
        let res = make_shared_resource("config");
        assert_eq!(res.name, "config");
        assert_eq!(strong_count(&res), 1);
    }

    #[test]
    fn attaching_owners_shares_the_same_data() {
        let res = make_shared_resource("config");
        let owner_a = attach_owner("a", &res);
        let owner_b = attach_owner("b", &res);

        assert_eq!(strong_count(&res), 3);
        // Both owners see the exact same underlying data, not a copy.
        assert_eq!(owner_a.resource.name, "config");
        assert_eq!(owner_b.resource.name, "config");
        assert!(Rc::ptr_eq(&owner_a.resource, &owner_b.resource));
    }

    #[test]
    fn dropping_an_owner_decrements_the_strong_count() {
        let res = make_shared_resource("shared");
        {
            let _owner = attach_owner("temp", &res);
            assert_eq!(strong_count(&res), 2);
        }
        // `_owner` went out of scope, dropping its Rc clone.
        assert_eq!(strong_count(&res), 1);
    }

    #[test]
    fn many_owners_can_share_one_resource() {
        let res = make_shared_resource("pool");
        let owners: Vec<Owner> = (0..5)
            .map(|i| attach_owner(&format!("owner-{i}"), &res))
            .collect();

        assert_eq!(strong_count(&res), 6); // res itself + 5 owners
        drop(owners);
        assert_eq!(strong_count(&res), 1);
    }
}
