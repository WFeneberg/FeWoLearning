//! Exercise 044 — What `#[derive(...)]` actually generates (intermediate).
//! Goal:   use a derived `Clone` to branch off a modified copy without
//!         touching the original, and a derived `Debug` to render one.
//! Drills: `#[derive(Debug, Clone, PartialEq)]`, what each derive buys you.

#[derive(Debug, Clone, PartialEq)]
pub struct Config {
    pub name: String,
    pub retries: u32,
}

/// Clones `cfg` and returns a copy with `extra` added to `retries`,
/// leaving the original untouched.
pub fn with_more_retries(cfg: &Config, extra: u32) -> Config {
    todo!("with_more_retries({cfg:?}, {extra})")
}

/// A human-readable rendering built from the derived `Debug` impl.
pub fn describe(cfg: &Config) -> String {
    todo!("describe({cfg:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn returns_a_copy_with_more_retries() {
        let original = Config { name: "prod".to_string(), retries: 3 };
        let bumped = with_more_retries(&original, 2);
        assert_eq!(bumped.retries, 5);
    }

    #[test]
    fn cloning_does_not_mutate_the_original() {
        let original = Config { name: "prod".to_string(), retries: 3 };
        let _bumped = with_more_retries(&original, 2);
        assert_eq!(original.retries, 3);
    }

    #[test]
    fn bumped_config_is_not_equal_to_the_original() {
        let original = Config { name: "prod".to_string(), retries: 3 };
        let bumped = with_more_retries(&original, 2);
        assert_ne!(original, bumped);
    }

    #[test]
    fn describe_renders_the_debug_format() {
        let cfg = Config { name: "prod".to_string(), retries: 3 };
        let rendered = describe(&cfg);
        assert!(rendered.contains("prod"));
        assert!(rendered.contains("retries"));
    }
}
