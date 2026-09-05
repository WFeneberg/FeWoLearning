# Exercise Format & Conventions

Every exercise across every language follows the same conventions so the
experience is uniform and tooling stays predictable.

## Numbering & difficulty tiers

Exercises are numbered `001`–`100` per language and grouped into four tiers:

| Tier folder        | Numbers   | Focus                                              |
|--------------------|-----------|----------------------------------------------------|
| `01-beginner`      | 001–035   | Syntax, types, control flow, collections, I/O      |
| `02-intermediate`  | 036–070   | Idioms, error handling, generics, std-lib depth    |
| `03-advanced`      | 071–090   | Concurrency, performance, memory, advanced patterns|
| `04-expert`        | 091–100   | Architecture, DSLs, metaprogramming, systems design|

The per-language ledger lives in each folder's `catalog.md`, which lists all 100
entries with a ✅ / ⬜ status. That file is the source of truth for what is done
and what is next.

**Exception:** `security/` is the only track that departs from this scheme. It
has **60 rows**, not 100, organised into four **attack-surface blocks**
(`01-web-aspnet`, `02-web-blazor`, `03-desktop-core`, `04-desktop-wpf`) rather
than difficulty tiers — the blocks are surfaces, not levels, and difficulty
rises within each one instead of across the track. See its own entry in
`CLAUDE.md`'s track-specific gotchas for why.

## Naming

Identifiers cannot start with a digit in most of these languages, so the number
is prefixed:

| Track     | Exercise unit                                   | Example                                     |
|-----------|-------------------------------------------------|---------------------------------------------|
| `dotnet/` | one file per exercise, tier-wide namespace      | `exercises/01-beginner/Ex001_FizzBuzz.cs`    |
| `python/` | module file + sibling test module               | `exercises/01-beginner/ex001_temperature.py` |
| `go/`     | one package per exercise, in its own folder     | `exercises/01-beginner/ex001_fizzbuzz/`      |
| `rust/`   | one file per exercise, registered in `lib.rs`   | `exercises/01-beginner/ex001_anagram.rs`     |
| `vue/`    | folder with stub + colocated `*.test.ts`        | `exercises/01-beginner/ex001_use_counter/`   |
| `angular/`| folder with stub + colocated `*.spec.ts`        | `exercises/01-beginner/ex001_pricing_service/` |
| `java/`   | folder with package source + sibling JUnit test | `exercises/01-beginner/ex001_primitive_math/` |
| `kotlin/` | folder with package source + sibling JUnit test | `exercises/01-beginner/ex001_val_var_basics/` |
| `avalonia/`| one folder per tier, `.axaml` + code-behind, test in a separate `tests/` project | `exercises/01-beginner/Ex001_HelloView.axaml` |
| `uno/`    | one folder per tier, `.cs` (plus `.xaml` + code-behind for markup exercises), test in a separate `tests/` project | `exercises/01-beginner/Ex001_HelloProperty.cs` |
| `caliburn/`| one file per exercise, tier-wide namespace, test in a separate `tests/` project | `exercises/01-beginner/Ex001_NotifyByHand.cs` |
| `wpf/`    | one folder per tier, `.cs` (plus `.xaml` + code-behind for markup exercises), test in a separate `tests/` project | `exercises/01-beginner/Ex001_ClrToDependencyProperty.cs` |
| `MicroServices/`| one file per exercise, tier-wide namespace, test in a separate `tests/` project | `exercises/01-beginner/Ex001_ContainerResourceBasics.cs` |
| `security/`| one file per exercise per block, block-wide namespace, `.razor` for block 02, test in a separate `tests/` project | `exercises/01-web-aspnet/Ex001_SecurityHeaders.cs` |

Go package clauses drop the `exNNN_` prefix and the underscores
(`ex001_fizzbuzz` → `package fizzbuzz`). .NET namespaces follow the *tier*
(`FeWoLearning.Exercises.Beginner`), not the `NN-tier` folder name.

## Anatomy of one exercise

Each exercise exists in two mirrored places, at the same relative path:

- `exercises/<tier>/…` — the **stub** you edit, plus its failing test.
- `solutions/<tier>/…` — the **reference** implementation.

Each stub carries a header comment stating the goal, the concepts it drills
(`Drills:`), and what the test verifies. The `Drills:` line is what populates the
Concepts column of `catalog.md`, so keep it accurate and on one topic per clause.

Stubs are written so the project still **compiles/imports** while unfinished:
they `throw` / `panic` / `todo!()` / `raise NotImplementedError` at runtime rather
than breaking the build. A stub that fails to compile is a bug — the learner
would get a build error instead of a red test.

## Test-driven workflow

1. Read the exercise header for the goal and constraints.
2. Run the test — watch it fail (red), **with the TODO message as the cause**.
3. Implement until the test passes (green).
4. Compare against the reference solution and note idiomatic differences.

Run commands per language are in each folder's `README.md` and summarised in the
root `CLAUDE.md`.

## Adding new exercises

1. Create the stub and its test under `exercises/<tier>/`; confirm it is red for
   the right reason.
2. Add the mirrored reference under `solutions/<tier>/`; confirm it turns the
   test green when overlaid.
3. Register it where the track requires it — **Rust needs a `#[path]` `pub mod`
   line in `exercises/lib.rs`**, or the file is never compiled. The other tracks
   are auto-discovered by their test runner.
4. Flip the exercise's `catalog.md` row from ⬜ to ✅.

## Known gaps

These are deliberate, documented limitations rather than oversights. For most
tracks they exist because `solutions/` is intentionally kept out of each build
(the files reuse the stubs' type and module names, so compiling both at once
would collide) — `avalonia/`, `blazor/`, `caliburn/`, `wpf/` and
`MicroServices/` are exceptions that keep `solutions/` in the build instead;
see their own tracks for why.

### `solutions/` is only partly verified

| Track     | Are the reference solutions checked?                                        |
|-----------|-----------------------------------------------------------------------------|
| `dotnet/` | **No.** No project includes `solutions/`, so they are never even compiled.  |
| `python/` | **No.** `testpaths = ["exercises"]` excludes them from collection.          |
| `go/`     | Compiled only. They *are* in the module (so `go build`/`go vet` cover them) but ship no `_test.go`. |
| `angular/`| **No.** `testMatch` covers `exercises/` only.                               |
| `vue/`    | Partly. `vitest.config.ts` also collects `solutions/**/*.test.ts`, but only some solution folders carry a test copy. |
| `rust/`   | **No.** Solutions are not registered in `lib.rs`.                            |
| `java/`   | **No** — not by a build (`solutions/` isn't referenced by any Gradle source set) **and not by anything else either**: this machine has no JDK/Gradle, so the track has never been compiled at all, stubs included. Higher risk than every other row in this table. |
| `kotlin/` | **No** — same situation as `java/`: not referenced by any Gradle source set, and not compiled by anything else either, since this machine has no JDK/Gradle/Kotlin at all. Higher risk than every other row in this table. |
| `avalonia/`| **Yes.** `solutions/` is its own project, built and referenced by `tests/` (and `gallery/`) whenever the `UseSolutions` MSBuild property is set — `dotnet test -p:UseSolutions=true` compiles and runs every test against the reference solutions instead of the stubs. Lower risk than every other row in this table. |
| `uno/`    | **Yes.** Same mechanism: `solutions/` is its own project, and `dotnet test -p:UseSolutions=true` runs the identical 823 tests against it. Every solution is confirmed green and every stub confirmed red, and the solutions build is expected to be warning-free - so a warning there is a finding. Lowest risk in this table. |
| `caliburn/`| **Yes.** Same mechanism: `solutions/` is its own project, referenced by `tests/` whenever the `UseSolutions` MSBuild property is set — `dotnet test -p:UseSolutions=true` compiles and runs every test against the reference solutions instead of the stubs. Lower risk than every other row in this table except `avalonia/`/`uno/`. |
| `wpf/`    | **Yes.** Same mechanism: `solutions/` is its own project, referenced by `tests/` whenever the `UseSolutions` MSBuild property is set — `dotnet test -p:UseSolutions=true` compiles and runs every test against the reference solutions instead of the stubs. On its complete beginner tier (210 test facts: 5 harness smoke tests + 205 exercise facts), `dotnet test` shows 5 passed / 205 failed on the untouched tree, and 210 passed / 0 failed under `-p:UseSolutions=true`, with zero warnings on both builds. |

Consequence: a reference solution can silently drift until it no longer passes
its own test, and nothing reports it. This is not hypothetical — an audit found
five such solutions in `vue/` and four defective tests in `go/`, all of which had
been committed green-looking because the throwing stub masked the failure in
`exercises/`.

**Verify manually** by overlaying, never by adding `solutions/` to a build:

```bash
# copy the track to a throwaway dir, overlay each solution onto its stub, test there
cp -r <track> /tmp/check && cd /tmp/check
find solutions -type f | while read -r s; do cp "$s" "exercises/${s#solutions/}"; done
rm -rf solutions && <the track's test command>
```

In Rust the solution files must keep a copy of the stub's `#[cfg(test)] mod tests`
block, otherwise overlaying deletes the tests along with the stub.

### `exercises/` cannot be type-clean, by design

In the TypeScript tracks a throwing stub has return type `never`, which poisons
everything downstream — `mount(Stub)` infers `never`, so `wrapper.vm` and
`wrapper.element` stop type-checking. And some exercises *are* the missing
declaration: `vue/` ex030 asks the learner to declare the `level` prop that its
own template reads, so the template cannot type-check until they do.

So `npm run typecheck` reporting a couple of errors under `exercises/` is
expected. The real gate is `npm run typecheck:solutions` (vue), which must stay at
zero. When writing a test, prefer `nextTick()` from `vue` over `wrapper.vm.$nextTick()`
and cast `wrapper.element as HTMLElement` — that keeps a new test type-clean even
against a throwing stub.

### Per-track quirks

- **`python/`** — `pyproject.toml` has no `[build-system]` table although the
  README documents `pip install -e ".[dev]"`. Modern pip falls back to
  setuptools' legacy backend, which works but is unpinned.
- **`go/`** — `go test` writes its test binaries under `%TEMP%`, where on-access
  scanning can delete them before exec (`fork/exec …: file not found`). Set
  `GOTMPDIR` elsewhere, or run a `go test -c` binary directly.
- **`rust/`** — `cargo test` links and runs fine via `rust/.cargo/config.toml`,
  which pins `LIB` at a VS 2022 Community install (see that file's comment for
  why). `cargo`/`rustc` are not on `PATH` in a plain shell — prepend
  `%USERPROFILE%\.cargo\bin`.
- **`uno/`** — the tests run against the real Skia `Uno.UI` with no window, which
  the harness in `uno/tests/_harness/` makes possible by installing what a platform
  head would (two `internal` dispatcher hooks by reflection, ICU data, an
  `Application`). That has consequences an exercise author has to know before
  writing one: `ItemsControl`/`ListView` never realise items, no input or focus or
  `Loaded`/`SizeChanged` ever happens, `TransformToVisual` returns the origin, and
  `await CancellationTokenSource.CancelAsync()` overflows the stack. `uno/README.md`
  is the full list, and several catalog rows were re-scoped around it. The stub build
  carries 16 warnings on purpose - fields and events a learner has not used yet.
- **`vue/`** — the advanced tier hand-rolls minimal Pinia- and Router-shaped
  helpers rather than depending on `pinia` / `vue-router`.
- **`java/`** — one package per exercise, with the test beside the stub in a
  single Gradle `test` source set rooted at `exercises/` (no `src/main`/`src/test`
  split). ex084 needs a project-level `resources/META-INF/services/...` file for
  `ServiceLoader` — the one exercise whose fixture can't live inside its own
  exercise folder, since `ServiceLoader` provider files must sit at the classpath
  root. Seeded at 100/100 but never compiled (no local JDK/Gradle).
- **`kotlin/`** — mirrors the Java layout (one package per exercise, test beside
  the stub in a single Gradle `test` source set), but stubs favor top-level
  functions, data classes, and idiomatic null-safety over classes-for-the-sake-
  of-classes. Coroutine-heavy tiers add `kotlinx-coroutines-core`/`-test` as
  dependencies; tests use `runTest { ... }` with virtual time, never real
  delays. Seeded at 100/100 but never compiled (no local JDK/Gradle/Kotlin).
