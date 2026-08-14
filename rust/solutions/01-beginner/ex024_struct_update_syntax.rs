//! Exercise 024 — Struct update syntax (reference solution).

#[derive(Debug, PartialEq, Clone)]
pub struct Config {
    pub width: u32,
    pub height: u32,
    pub title: String,
}

pub fn default_config() -> Config {
    let width = 800;
    let height = 600;
    // Field init shorthand: `width` and `height` match the field names.
    Config {
        width,
        height,
        title: "untitled".to_string(),
    }
}

pub fn with_title(base: &Config, title: String) -> Config {
    // `..base.clone()` fills in every field we don't override.
    Config {
        title,
        ..base.clone()
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
