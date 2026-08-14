//! Exercise 013 — HashMap iteration & sorting (beginner).
//! Goal:   turn a `HashMap` into a deterministically ordered `Vec`, sorted by
//!         count descending and then alphabetically ascending on ties.
//! Drills: iterating `HashMap` entries, `Vec::sort_by`/`sort_by_key`.

use std::collections::HashMap;

pub fn sorted_by_count_desc(counts: &HashMap<String, u32>) -> Vec<(String, u32)> {
    todo!("sorted_by_count_desc({counts:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn sorts_by_count_descending() {
        let mut counts = HashMap::new();
        counts.insert("the".to_string(), 2);
        counts.insert("cat".to_string(), 1);
        counts.insert("mat".to_string(), 1);

        let sorted = sorted_by_count_desc(&counts);
        assert_eq!(
            sorted,
            vec![
                ("the".to_string(), 2),
                ("cat".to_string(), 1),
                ("mat".to_string(), 1),
            ]
        );
    }

    #[test]
    fn empty_map_gives_empty_vec() {
        let counts = HashMap::new();
        assert_eq!(sorted_by_count_desc(&counts), Vec::<(String, u32)>::new());
    }
}
