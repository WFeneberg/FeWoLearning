//! Exercise 091 — Typestate builder: compile-time build validation (expert).
//! Goal:   a `ServerConfigBuilder` that tracks, in its own TYPE, whether the
//!         required `host` and `port` fields have been set yet. `.build()`
//!         only exists on `ServerConfigBuilder<Set, Set>` — calling it before
//!         both setters ran is a COMPILE error (the method simply isn't there
//!         for `Missing`), not a runtime panic. The typestate markers and
//!         method signatures are already the scaffold below; your job is the
//!         runtime bodies (move the other field through unchanged, stash the
//!         new one, unwrap at the end).
//! Drills: generic marker types as compile-time state, `PhantomData<T>`,
//!         methods that only exist for specific type-parameter instantiations.

use std::marker::PhantomData;

/// Marker: a required field has not been provided yet.
pub struct Missing;
/// Marker: a required field has been provided.
pub struct Set;

/// Builds a [`ServerConfig`]. `H` and `P` are compile-time markers for
/// whether `host`/`port` have been set — either [`Missing`] or [`Set`].
pub struct ServerConfigBuilder<H, P> {
    host: Option<String>,
    port: Option<u16>,
    _host_state: PhantomData<H>,
    _port_state: PhantomData<P>,
}

impl ServerConfigBuilder<Missing, Missing> {
    /// Starts a fresh builder with neither field set.
    pub fn new() -> Self {
        Self {
            host: None,
            port: None,
            _host_state: PhantomData,
            _port_state: PhantomData,
        }
    }
}

impl<P> ServerConfigBuilder<Missing, P> {
    /// Sets `host`, transitioning the `H` marker from `Missing` to `Set`.
    pub fn host(self, host: impl Into<String>) -> ServerConfigBuilder<Set, P> {
        ServerConfigBuilder {
            host: Some(host.into()),
            port: self.port,
            _host_state: PhantomData,
            _port_state: PhantomData,
        }
    }
}

impl<H> ServerConfigBuilder<H, Missing> {
    /// Sets `port`, transitioning the `P` marker from `Missing` to `Set`.
    pub fn port(self, port: u16) -> ServerConfigBuilder<H, Set> {
        ServerConfigBuilder {
            host: self.host,
            port: Some(port),
            _host_state: PhantomData,
            _port_state: PhantomData,
        }
    }
}

impl ServerConfigBuilder<Set, Set> {
    /// Only callable once both `host` and `port` are `Set` — this is the
    /// compile-time guarantee: there is no `build` method on any other
    /// `ServerConfigBuilder<H, P>` instantiation, so calling it too early
    /// fails to compile rather than panicking at runtime.
    pub fn build(self) -> ServerConfig {
        ServerConfig {
            host: self.host.expect("H=Set guarantees host was provided"),
            port: self.port.expect("P=Set guarantees port was provided"),
        }
    }
}

/// The finished, fully-specified configuration.
pub struct ServerConfig {
    pub host: String,
    pub port: u16,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn build_after_setting_host_then_port() {
        let config = ServerConfigBuilder::new()
            .host("example.com")
            .port(8080)
            .build();
        assert_eq!(config.host, "example.com");
        assert_eq!(config.port, 8080);
    }

    #[test]
    fn build_after_setting_port_then_host() {
        // Order doesn't matter: whichever setter runs first, the other
        // still has to run before `.build()` exists at all.
        let config = ServerConfigBuilder::new()
            .port(9090)
            .host("other.example")
            .build();
        assert_eq!(config.host, "other.example");
        assert_eq!(config.port, 9090);
    }

    #[test]
    fn host_accepts_both_str_and_string() {
        let config1 = ServerConfigBuilder::new().host("a").port(1).build();
        let config2 = ServerConfigBuilder::new()
            .host(String::from("b"))
            .port(2)
            .build();
        assert_eq!(config1.host, "a");
        assert_eq!(config2.host, "b");
    }

    // `.build()` simply does not exist on `ServerConfigBuilder<Missing, _>` or
    // `ServerConfigBuilder<_, Missing>` — uncommenting either line below is a
    // COMPILE error, not a test failure, which is the whole point of the
    // typestate pattern:
    //
    //   ServerConfigBuilder::new().host("x").build();       // no `port` set
    //   ServerConfigBuilder::new().port(1).build();         // no `host` set
}
