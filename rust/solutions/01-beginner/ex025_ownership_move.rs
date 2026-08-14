//! Exercise 025 — Ownership moves (reference solution).

#[derive(Debug, PartialEq)]
pub struct Inventory {
    pub items: Vec<String>,
}

pub fn transfer_all(from: Inventory) -> (Inventory, Inventory) {
    // Destructuring `from` moves its `items` field out; `from` itself is
    // consumed and cannot be used again after this line.
    let Inventory { items } = from;
    (Inventory { items: Vec::new() }, Inventory { items })
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
