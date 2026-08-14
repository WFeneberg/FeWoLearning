//! Exercise 024 — Struct update syntax (beginner).
//! Goal:   build a default `Config` and derive variations from it without
//!         repeating every field.
//! Drills: field init shorthand, the `..base` struct update syntax.

#[derive(Debug, PartialEq, Clone)]
pub struct Config {
    pub width: u32,
    pub height: u32,
    pub title: String,
}

pub fn default_config() -> Config {
    todo!("default_config")
}

pub fn with_title(base: &Config, title: String) -> Config {
    todo!("with_title({base:?}, {title:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn default_config_has_expected_values() {
        assert_eq!(
            default_config(),
            Config {
                width: 800,
                height: 600,
                title: "untitled".to_string(),
            }
        );
    }

    #[test]
    fn with_title_keeps_other_fields() {
        let base = default_config();
        let updated = with_title(&base, "Game".to_string());
        assert_eq!(
            updated,
            Config {
                width: 800,
                height: 600,
                title: "Game".to_string(),
            }
        );
        // The original is untouched.
        assert_eq!(base.title, "untitled");
    }
}
