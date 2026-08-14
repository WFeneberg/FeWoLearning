//! Exercise 018 — Option to Result conversion (reference solution).

pub fn find_index(haystack: &[i32], needle: i32) -> Result<usize, String> {
    haystack
        .iter()
        .position(|&x| x == needle)
        .ok_or_else(|| format!("{needle} not found"))
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
