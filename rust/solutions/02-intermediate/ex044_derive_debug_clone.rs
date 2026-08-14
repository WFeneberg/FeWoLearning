//! Exercise 044 — What `#[derive(...)]` actually generates (reference solution).

#[derive(Debug, Clone, PartialEq)]
pub struct Config {
    pub name: String,
    pub retries: u32,
}

pub fn with_more_retries(cfg: &Config, extra: u32) -> Config {
    let mut copy = cfg.clone();
    copy.retries += extra;
    copy
}

pub fn describe(cfg: &Config) -> String {
    format!("{cfg:?}")
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
