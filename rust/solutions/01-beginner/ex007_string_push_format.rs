//! Exercise 007 — String push & format (reference solution).

pub fn build_report(items: &[&str]) -> String {
    let mut s = String::with_capacity(items.len() * 8);
    for item in items {
        s.push_str(&format!("- {item}\n"));
    }
    s
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn builds_a_multi_line_report() {
        assert_eq!(build_report(&["milk", "eggs"]), "- milk\n- eggs\n");
    }

    #[test]
    fn single_item() {
        assert_eq!(build_report(&["x"]), "- x\n");
    }

    #[test]
    fn empty_list_is_empty_string() {
        assert_eq!(build_report(&[]), "");
    }
}
