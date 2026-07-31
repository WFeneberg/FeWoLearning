//! Exercise 001 — Anagram (reference solution).

pub fn is_anagram(a: &str, b: &str) -> bool {
    normalize(a) == normalize(b)
}

fn normalize(s: &str) -> Vec<char> {
    let mut chars: Vec<char> = s
        .chars()
        .filter(|c| !c.is_whitespace())
        .flat_map(|c| c.to_lowercase())
        .collect();
    chars.sort_unstable();
    chars
}
