//! Exercise 007 — String push & format (beginner).
//! Goal:   build a bullet-point report from a list of items, one `"- item\n"`
//!         line per item, reserving capacity up front.
//! Drills: `String::with_capacity`, `push_str`, `format!`.

pub fn build_report(items: &[&str]) -> String {
    todo!("build_report({items:?})")
}

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
