# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

FeWoLearning is a **polyglot skills-training monorepo**, not an application. It
holds six independent, self-contained learning tracks — `dotnet/`, `python/`,
`vue/`, `angular/`, `go/`, `rust/` — each with its own toolchain, test runner,
and a graded set of **exercises** (stubs the learner implements) paired with
reference **solutions**. There is no shared build and no cross-track code. Treat
each language folder as its own project.

The owner is a senior .NET architect using this repo to keep .NET sharp and to
learn the other five ecosystems, driven with JetBrains IDEs (Rider, PyCharm,
GoLand, WebStorm, RustRover).

## The universal exercise pattern (applies to every track)

Every track mirrors the same structure:

```
<track>/
  exercises/<tier>/…   # stubs you edit — each ships with a FAILING test
  solutions/<tier>/…   # reference implementations (NOT part of the build)
  catalog.md           # the 100-exercise roadmap for that track
  README.md            # per-track setup + commands
```

Difficulty tiers and numbering are consistent across all tracks:
`01-beginner` (001–035), `02-intermediate` (036–070), `03-advanced` (071–090),
`04-expert` (091–100).

**The invariant that defines a correct exercise:** a stub's test **fails (red)**
before implementation and **passes (green)** once the stub matches its reference
solution. Stubs are written so the project still *compiles/imports* while
unfinished — they `throw`/`panic`/`todo!()` at runtime rather than breaking the
whole build. Preserve this when adding or editing exercises.

Because `solutions/` deliberately reuse the same names/namespaces as the stubs,
they are **kept out of each build** (separate sibling folder). To verify a
solution, overlay it onto the matching stub file in a throwaway copy and run the
tests there — do not add `solutions/` to a project/module.

## Per-track commands

| Track     | Install (once)                          | Run all tests            | Run one exercise |
|-----------|-----------------------------------------|--------------------------|------------------|
| `dotnet/` | — (restore on first `dotnet test`)      | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_FizzBuzz` |
| `python/` | `pip install -e ".[dev]"`               | `pytest`                 | `pytest exercises/01-beginner/test_ex001_temperature.py` |
| `vue/`    | `npm install`                           | `npm test`               | `npm run test:one -- "increments"` |
| `angular/`| `npm install`                           | `npm test`               | `npm run test:one -- "applies a discount"` |
| `go/`     | — (needs Go installed)                  | `go test ./...`          | `go test ./exercises/01-beginner/ex001_fizzbuzz/` |
| `rust/`   | — (needs Rust installed)                | `cargo test`             | `cargo test ex001` |

Run every command **from inside the track folder**, not the repo root.

## Toolchain status (verified 2026-07-31)

- ✅ Installed & verified end-to-end: **.NET 10** (`dotnet test` builds & runs),
  **Python 3.14** (`pytest` runs), **Node 26 / npm 11** (Vue & Angular configs).
- ⚠️ **Not installed yet: Go and Rust.** Their source is valid but untested
  locally. Install via `winget` (Go) and `rustup` (Rust) — see
  [`docs/requirements.md`](docs/requirements.md).
- The Vue and Angular `package.json`s are written but **`npm install` has not been
  run**, so `node_modules` is absent until the learner installs.

## Track-specific gotchas

- **.NET** — The solution is the new **`.slnx`** format (`FeWoLearning.Dotnet.slnx`),
  not `.sln`. Namespaces are fixed per tier
  (`FeWoLearning.Exercises.Beginner/.Intermediate/.Advanced/.Expert`) and do **not**
  follow the `NN-tier` folder names, because C# identifiers cannot start with a
  digit. Stubs throw `NotImplementedException`.
- **Python** — Module files are prefixed `exNNN_` because Python modules cannot
  start with a digit. Tier folders are added to pytest's `pythonpath` (in
  `pyproject.toml`) so tests import stubs directly. Note: pytest's default import
  mode prepends each test file's own directory to `sys.path`, so overriding
  `pythonpath` will **not** redirect imports to `solutions/` — overlay files
  instead. Stubs raise `NotImplementedError`.
- **Go** — Single module `fewolearning`; each exercise is its own package under
  `exercises/<tier>/exNNN_slug/`. Stubs `panic("TODO…")` so they compile.
- **Rust** — Single crate whose `[lib] path` is `exercises/lib.rs`; each exercise
  is a `#[path=…] pub mod` registered there, with inline `#[cfg(test)] mod tests`.
  Adding an exercise requires adding a `mod` line to `exercises/lib.rs`. Stubs use
  `todo!()`.
- **Vue** — Vitest + `@vue/test-utils` in jsdom; tests are `*.test.ts` colocated
  with the composable/`.vue` stub. Stubs `throw`.
- **Angular** — Headless testing via **Jest** (`jest-preset-angular`), not
  Karma; tests are `*.spec.ts`. Components are **standalone** and use **signals**.
  Stubs `throw`.

## Adding or completing exercises

1. Create the stub under `exercises/<tier>/` and its test; confirm the test is red.
2. Add the mirrored reference under `solutions/<tier>/`; confirm it turns the test
   green when overlaid.
3. Register it where the track requires (Rust: `exercises/lib.rs`; others are
   auto-discovered by the test runner).
4. Update the track's `catalog.md` row from ⬜ to ✅.

## Current state

Each track is scaffolded and seeded with 2–3 fully-worked exercises spanning the
tiers (the ✅ rows in each `catalog.md`). The remaining entries toward 100 per
track are described by theme in the catalogs and are **not yet written** — they
are the roadmap for future work.
