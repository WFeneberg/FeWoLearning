//! Exercise 025 — Ownership moves (beginner).
//! Goal:   consume an `Inventory` by value and redistribute its items into
//!         a fresh, empty one and a full one — the original is moved in and
//!         cannot be used again by the caller.
//! Drills: moves, destructuring a struct to move fields out of it.

#[derive(Debug, PartialEq)]
pub struct Inventory {
    pub items: Vec<String>,
}

pub fn transfer_all(from: Inventory) -> (Inventory, Inventory) {
    todo!("transfer_all({from:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn moves_every_item_into_the_second_inventory() {
        let source = Inventory {
            items: vec!["sword".to_string(), "shield".to_string()],
        };
        let (emptied, full) = transfer_all(source);
        assert_eq!(emptied, Inventory { items: vec![] });
        assert_eq!(
            full,
            Inventory {
                items: vec!["sword".to_string(), "shield".to_string()]
            }
        );
    }

    #[test]
    fn works_on_an_already_empty_inventory() {
        let source = Inventory { items: vec![] };
        let (emptied, full) = transfer_all(source);
        assert_eq!(emptied, Inventory { items: vec![] });
        assert_eq!(full, Inventory { items: vec![] });
    }
}
