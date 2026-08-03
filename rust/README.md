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

See [`catalog.md`](catalog.md) — the 100-row progress ledger. Currently **2 / 100**; the ⬜ rows are the work queue.

## Linking on this machine

[`.cargo/config.toml`](.cargo/config.toml) sets `LIB` to VS 2022's desktop MSVC
libraries plus the Windows 10 SDK. Without it, rustc picks a Visual Studio install
that only ships `lib\onecore`, and `link.exe` fails with
`LNK1104: cannot open file 'msvcrt.lib'`. The MSVC and SDK versions are pinned in
that file — update them if Visual Studio is upgraded. See
[`../docs/requirements.md`](../docs/requirements.md).
