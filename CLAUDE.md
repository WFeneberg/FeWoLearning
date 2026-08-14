# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

FeWoLearning is a **polyglot skills-training monorepo**, not an application. It
holds eight independent, self-contained learning tracks — `dotnet/`, `python/`,
`vue/`, `angular/`, `go/`, `rust/`, `java/`, `kotlin/` — each with its own toolchain, test runner,
and a graded set of **exercises** (stubs the learner implements) paired with
reference **solutions**. There is no shared build and no cross-track code. Treat
each language folder as its own project.

The owner is a senior .NET architect using this repo to keep .NET sharp and to
learn the other seven ecosystems, driven with JetBrains IDEs (Rider, PyCharm,
GoLand, WebStorm, RustRover, IntelliJ IDEA).

## The universal exercise pattern (applies to every track)

Every track mirrors the same structure:

```
<track>/
  exercises/<tier>/…   # stubs you edit — each ships with a FAILING test
  solutions/<tier>/…   # reference implementations (NOT part of the build)
  catalog.md           # 100-row progress ledger (✅ done / ⬜ planned)
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
| `go/`     | — (deps already downloaded)             | `go test ./...`          | `go test ./exercises/01-beginner/ex001_fizzbuzz/` |
| `rust/`   | — (`LIB` comes from `.cargo/config.toml`) | `cargo test`           | `cargo test ex001` |
| `java/`   | planned                                 | planned                  | planned |
| `kotlin/` | planned                                 | planned                  | planned |

Run every command **from inside the track folder**, not the repo root.

## Toolchain status (verified 2026-08-03)

- ✅ Verified end-to-end: **.NET 10**, **Python 3.14**, **Node 26 / npm 11**
  (both `vue/` and `angular/` have `node_modules`), **Go 1.26.5**, **Rust 1.97.1**.
- `java/` and `kotlin/` are currently **catalog-only** additions: their ledgers
  and README files exist, but the build scaffolding and seeded exercises do not.
- **Rust links via `rust/.cargo/config.toml`.** rustc auto-detects the VS 18
  Professional toolset, which ships only `lib\onecore` and no desktop `lib\x64`, so
  `link.exe` died with `LNK1104: cannot open file 'msvcrt.lib'`. That config's
  `[env]` table points `LIB` at VS 2022 Community's `lib\x64` plus the Windows 10
  SDK. The MSVC/SDK versions are pinned there — a VS upgrade breaks it. See
  [`docs/requirements.md`](docs/requirements.md).
- Go and Rust are **not on `PATH`**: prepend `C:\Program Files\Go\bin` and
  `%USERPROFILE%\.cargo\bin` before invoking them from a plain shell.
- `go test` needs `GOTMPDIR` outside `%TEMP%`, or on-access scanning deletes test
  binaries before exec (`fork/exec …: file not found`).

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
- **Java** — planned layout is one package folder per exercise containing the
  stub plus a sibling JUnit test; when unfinished, stubs should `throw` at runtime
  rather than fail compilation.
- **Kotlin** — planned layout mirrors Java, but the exercises should prefer
  top-level functions, data classes, and idiomatic null-safety. Unfinished stubs
  should `TODO()` at runtime.

## Adding or completing exercises

Work in **batches of five**, and do not re-inventory the disk — `catalog.md` is the
work queue.

1. Read the track's `catalog.md`; the next five ⬜ rows are the assignment. Their
   Slug and Concepts columns are the spec.
2. Read **one** already-finished exercise from the same tier as a style template —
   once per tier, not once per batch.
3. For each exercise write the stub (header comment with `Goal:` / `Drills:` /
   `Passes:`), its test, and the reference solution under `solutions/<tier>/`.
   Register Rust exercises in `exercises/lib.rs`.
4. **Red check**, filtered to the five. Confirm each failure is caused by the TODO,
   not by an import or compile error (a stub that fails to build is a bug), and that
   **no test passes**. Two ways a test accidentally passes against a stub:
   - Python: `NotImplementedError` **subclasses `RuntimeError`**, so
     `pytest.raises(RuntimeError)` is satisfied by any stub. Use a locally defined
     exception type instead.
   - A test that asserts an error the *signature* produces (wrong call style, wrong
     arity) passes before the body ever runs. Assert on introspected metadata
     instead, or leave the signature itself to the learner.

   And one way the failure lands in the wrong place: a `@pytest.mark.parametrize`
   decorator referencing something the stub does not define yet (an enum member, a
   class attribute) is evaluated at **collection** time, so pytest reports a
   collection error instead of a failing test. Parametrise on plain data and resolve
   it inside the test body.
5. **Green check** by overlaying the solutions:
   - `vue`/`angular`: copy the `*.test.ts` / `*.spec.ts` into the matching
     `solutions/` folder; `vitest.config.ts` already collects them (Jest needs
     `--testMatch='**/solutions/**/*.spec.ts'`).
   - `python`/`go`/`rust`/`dotnet`/`java`/`kotlin`: copy the track into the scratchpad, overlay each
     solution onto its stub, delete `solutions/`, run the tests there.
6. Run the track's type gate: `npm run typecheck:solutions` (vue), `go vet ./...`.
   `solutions/` must stay clean; a couple of errors under `exercises/` are expected
   and documented.
7. Flip exactly those five `catalog.md` rows ⬜ → ✅ and update its `**Status:**`
   line. Beware: some catalogs pad the status cell (`⬜     |`), others do not.
8. Commit as `<track>: exNNN–exNNN`. Stage explicit paths — `git add -A` has
   already swept up unrelated files once.

Keep the batch's test run filtered; run the full suite once per completed tier.

## Current state

Each track's `catalog.md` is a **100-row ledger** — one row per exercise, ✅ when
stub + test + solution exist and are verified, ⬜ when planned. That file is the
source of truth for what is done and what is next; do not re-inventory the disk.

| Track     | Written    | Remaining |
|-----------|------------|-----------|
| `dotnet/` | 100 / 100  | —         |
| `go/`     | 100 / 100  | —         |
| `vue/`    | 100 / 100  | —         |
| `python/` | 100 / 100  | —         |
| `angular/`| 100 / 100  | —         |
| `rust/`   | 100 / 100  | —         |
| `java/`   | 0 / 100    | 100       |
| `kotlin/` | 0 / 100    | 100       |

Work order for the remaining exercises: **java → kotlin**, in
batches of five, each batch red-verified then green-verified before its catalog
rows flip.

`dotnet/`, `go/`, `vue/`, `python/`, `angular/` and `rust/` are content-complete.
`java/` and `kotlin/` are cataloged but not scaffolded yet. `go/` was verified by
overlaying every reference solution onto its stub (`go vet ./...` clean, 100
stubs red, 100 solutions green); `vue/` runs 100 red exercise suites and 72
green solution suites, with `npm run typecheck:solutions` at zero errors;
`angular/` runs 100 red stub suites under `npm test` with zero compile errors,
and each exercise's solution was green-checked individually by overlaying it
onto its stub — see Known gaps below for the same kind of solutions/ drift
found in vue/ and go/. `python/` ex082–100 (the batch added to close the
track out) were each verified stub-red/solution-green individually by
overlaying into a scratch copy; the full 100-exercise suite collects and
runs red end-to-end with zero collection errors. `rust/` ex002–100 (the
batch added to close that track out) were likewise verified per-batch
(stub-red on the real tree, solution-green in a scratch overlay, re-run
2-3× for any concurrency exercise); the full crate additionally has all
100 solutions overlaid together at once as an integration check —
`cargo test` shows 0 passed/100 stubs red on the untouched tree and
395 passed/0 failed with every solution overlaid, doc-tests included.

## Known gaps

`solutions/` is deliberately outside every build, so reference implementations are
largely **unverified** and can drift silently — see the "Known gaps" section of
[`docs/exercise-format.md`](docs/exercise-format.md) for the per-track table and
the manual overlay recipe. An audit on 2026-08-03 found five broken solutions in
`vue/` and four defective tests in `go/` that had gone unnoticed for exactly this
reason.
