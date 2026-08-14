//! Exercise 013 — HashMap iteration & sorting (reference solution).

use std::collections::HashMap;

pub fn sorted_by_count_desc(counts: &HashMap<String, u32>) -> Vec<(String, u32)> {
    let mut entries: Vec<(String, u32)> = counts
        .iter()
        .map(|(word, count)| (word.clone(), *count))
        .collect();
    entries.sort_by(|a, b| b.1.cmp(&a.1).then_with(|| a.0.cmp(&b.0)));
    entries
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
