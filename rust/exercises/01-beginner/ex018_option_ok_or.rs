//! Exercise 018 — Option to Result conversion (beginner).
//! Goal:   find the index of a value in a slice, turning the `Option<usize>`
//!         that `position` gives you into a `Result` with a useful error.
//! Drills: `Option::ok_or`/`ok_or_else`.

pub fn find_index(haystack: &[i32], needle: i32) -> Result<usize, String> {
    todo!("find_index({haystack:?}, {needle})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn finds_a_present_value() {
        assert_eq!(find_index(&[1, 2, 3], 2), Ok(1));
    }

    #[test]
    fn reports_an_error_for_a_missing_value() {
        assert_eq!(find_index(&[1, 2, 3], 9), Err("9 not found".to_string()));
    }
}
