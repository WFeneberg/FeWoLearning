//! Exercise 062 — `Default` and `..Default::default()` (reference solution).

#[derive(Debug, Clone, PartialEq)]
pub struct ServerConfig {
    pub host: String,
    pub port: u16,
    pub timeout_secs: u32,
}

impl Default for ServerConfig {
    fn default() -> Self {
        ServerConfig {
            host: "localhost".to_string(),
            port: 8080,
            timeout_secs: 30,
        }
    }
}

pub fn with_custom_port(port: u16) -> ServerConfig {
    ServerConfig { port, ..Default::default() }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn default_has_sensible_values() {
        let cfg = ServerConfig::default();
        assert_eq!(cfg.host, "localhost");
        assert_eq!(cfg.port, 8080);
        assert_eq!(cfg.timeout_secs, 30);
    }

    #[test]
    fn custom_port_overrides_only_that_field() {
        let cfg = with_custom_port(9000);
        assert_eq!(cfg.port, 9000);
        assert_eq!(cfg.host, "localhost");
        assert_eq!(cfg.timeout_secs, 30);
    }

    #[test]
    fn custom_port_config_still_equals_default_elsewhere() {
        let default_cfg = ServerConfig::default();
        let custom_cfg = with_custom_port(default_cfg.port);
        assert_eq!(custom_cfg, default_cfg);
    }
}
