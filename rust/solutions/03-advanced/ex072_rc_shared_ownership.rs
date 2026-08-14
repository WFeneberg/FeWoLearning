//! Exercise 072 — Shared immutable ownership with `Rc` (reference solution).

use std::rc::Rc;

pub struct Resource {
    pub name: String,
}

pub struct Owner {
    pub label: String,
    pub resource: Rc<Resource>,
}

pub fn make_shared_resource(name: &str) -> Rc<Resource> {
    Rc::new(Resource { name: name.to_string() })
}

pub fn attach_owner(label: &str, resource: &Rc<Resource>) -> Owner {
    Owner {
        label: label.to_string(),
        resource: Rc::clone(resource),
    }
}

pub fn strong_count(resource: &Rc<Resource>) -> usize {
    Rc::strong_count(resource)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
