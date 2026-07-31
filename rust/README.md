# Rust Track

Test-driven Rust exercises. Requires the stable Rust toolchain — see
[`../docs/requirements.md`](../docs/requirements.md) to install it via `rustup`.

## Commands

| Action                  | Command                          |
|-------------------------|----------------------------------|
| Run all tests           | `cargo test`                     |
| Run one exercise        | `cargo test ex001`               |
| Lint (if installed)     | `cargo clippy`                   |
| Format                  | `cargo fmt`                      |

## Layout

Exercises are Rust files under `exercises/<tier>/exNNN_slug.rs`, each with inline
`#[cfg(test)] mod tests`. They are registered as modules in
[`exercises/lib.rs`](exercises/lib.rs). Stubs use the `todo!()` macro, so the
crate always compiles and an unfinished exercise fails at test time (panic)
rather than breaking the whole build.

Reference implementations mirror the tree under `solutions/<tier>/`.

When you add a new exercise, add a matching `#[path = ...] pub mod ...;` line to
`exercises/lib.rs`.

See [`catalog.md`](catalog.md) for the full 100-exercise roadmap.
