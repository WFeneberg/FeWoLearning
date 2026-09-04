# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

FeWoLearning is a **polyglot skills-training monorepo**, not an application. It
holds ten independent, self-contained learning tracks — `dotnet/`, `python/`,
`vue/`, `angular/`, `go/`, `rust/`, `java/`, `kotlin/`, `flutter/`, `blazor/` — each with its own toolchain, test runner,
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
| `flutter/`| planned                                 | planned                  | planned |
| `avalonia/`| — (restore on first `dotnet test`)     | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_HelloView` |
| `blazor/` | — (restore on first `dotnet test`)      | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |

Run every command **from inside the track folder**, not the repo root.

For `blazor/`, `dotnet test -p:UseSolutions=true` runs the identical suite
against the reference solutions instead of the stubs.

## Toolchain status (verified 2026-08-03)

- ✅ Verified end-to-end: **.NET 10**, **Python 3.14**, **Node 26 / npm 11**
  (both `vue/` and `angular/` have `node_modules`), **Go 1.26.5**, **Rust 1.97.1**,
  **Avalonia 12.1.1 with ReactiveUI 24.1.0 on .NET 10**. The Avalonia set is
  pinned and coherent at 12.1.1 and must not be bumped piecemeal: `ReactiveUI.Avalonia`'s
  12.x line stops at 12.1.1, while Avalonia itself has already released 12.1.2.
  **Blazor's** beginner tier (35/100) is verified end-to-end as of 2026-09-04 on
  **.NET 10.0.400 with bUnit 2.9.0**: 115 stub facts red, 0 passed on the
  untouched tree; the same 115 facts pass under `-p:UseSolutions=true` — unlike
  `java/`, `kotlin/`, `flutter/` and `php/`.
- `java/` and `kotlin/` are currently **catalog-only** additions: their ledgers
  and README files exist, but the build scaffolding and seeded exercises do not.
- `flutter/` is content-complete like `java/` and `kotlin/`: `pubspec.yaml`
  scaffolding, README, a 100-row `catalog.md`, and all 100 exercises (stub +
  test + solution) exist. No Flutter or Dart SDK is installed on this
  machine, so none of it has ever been analyzed, compiled, or run.
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
- **Java** — Gradle (`java/build.gradle`), no wrapper committed (none could be
  generated without a JDK/Gradle on this machine — install both, or run
  `gradle wrapper` once you have Gradle, before first use). One package folder
  per exercise (`exercises/<tier>/exNNN_slug/`) containing the stub plus a
  sibling JUnit 5 test — both live in a single Gradle `test` source set rooted
  at `exercises/` (there is no `src/main`/`src/test` split), so `solutions/`
  is naturally excluded from the build just by never being referenced by a
  source set. Package names are `fewolearning.exercises.<tier>.exNNN_slug`.
  Stubs `throw new UnsupportedOperationException("TODO")`. ex084 additionally
  needs `java/resources/META-INF/services/...` (a `ServiceLoader` provider-
  configuration file, at the classpath root — hence its own top-level
  `resources/` source dir rather than living under `exercises/`). **This
  entire track is unverified** — see "Current state" above.
- **Kotlin** — Gradle Kotlin DSL (`kotlin/build.gradle.kts`), same single-`test`-
  source-set layout as Java (`exercises/<tier>/exNNN_slug/` holding both the
  stub and its sibling JUnit 5 test), but favors top-level functions, data
  classes, and idiomatic null-safety over Java-style wrapper classes. Stubs
  `TODO()` at runtime. Coroutine-heavy tiers (intermediate onward: channels,
  `Flow`, `SharedFlow`/`StateFlow`, supervisors, actors) depend on
  `kotlinx-coroutines-core` and test via `kotlinx-coroutines-test`'s
  `runTest { ... }` with virtual time — never `Thread.sleep`/wall-clock delays.
  A recurring bug class here: a test/solution that "passes" regardless of
  whether the actual mechanism (`supervisorScope`, `combine`, `debounce`,
  `flatMapLatest`) is even used — always ask whether a naive/wrong
  implementation would still pass before trusting a coroutine test. **This
  entire track is unverified** — see "Current state" above.
- **Flutter/Dart** — `flutter/pubspec.yaml`, no `test/` root the way a typical
  Dart package uses; each exercise's stub and its sibling test live together
  under `exercises/<tier>/exNNN_slug/`, same convention as every other track.
  `01-beginner` and most of `02-intermediate` are pure Dart and test via
  `package:test` (plain `dart test`); from ex053 onward exercises build real
  widgets and test via `package:flutter_test` (`WidgetTester`, `pumpWidget`),
  which needs the full Flutter SDK, not just Dart. Stubs `throw
  UnimplementedError('TODO')`. A recurring bug class to watch for once this
  track is verified: a widget test that "passes" without ever calling
  `tester.pump()`/`pumpAndSettle()` after a state change, so it's really just
  asserting on the widget's *initial* build — always check the test actually
  exercises `setState`/stream/animation timing, not just the first frame.
  **This entire track is unverified** — see "Current state" above. Three
  exercises are flagged as extra-risky in `flutter/README.md`: ex069 needs a
  golden-file baseline generated on a real machine, and ex094/ex095 use
  plausible-but-unexercised platform-channel-mocking and cross-isolate
  closure APIs.
- **Avalonia** — The solution is `FeWoLearning.Avalonia.slnx`, with **four**
  projects: `exercises/`, `solutions/`, `tests/`, `gallery/`. Unlike the repo
  convention above, `solutions/` is deliberately **in** the build here, the
  same deviation `blazor/` makes: `tests/` and `gallery/` each reference
  exactly one content project via the MSBuild property `UseSolutions`, never
  both, so the name collision the repo-wide convention exists to prevent
  cannot occur. The payoff is that reference solutions are compile-checked on
  every build. This is deliberate and permanent — do not "fix" it back to
  match the rest of the repo. Namespaces are pinned per tier
  (`FeWoLearning.Avalonia.Exercises.Beginner` and friends), and every `.axaml`
  needs a fully qualified `x:Class` to match, because `01-beginner` is not a
  valid C# identifier. Stubs throw `NotImplementedException` after
  `InitializeComponent()`.

  Seven things current ReactiveUI/Avalonia tutorials get wrong for this
  package set, each verified by running real code on this machine: the
  package is `ReactiveUI.Avalonia`, not `Avalonia.ReactiveUI` (the old one
  stops at Avalonia 11.3.9); the test attribute is `[AvaloniaFact]` /
  `[AvaloniaTheory]`, not `[AvaloniaTest]`; **xunit.v3 is mandatory** — adding
  `xunit` 2.x puts `FactAttribute` in two assemblies and every test file fails
  CS0433; `ReactiveUI.Primitives.RxVoid` replaces `System.Reactive.Unit` —
  ReactiveUI 24 has no `Unit` type at all; Rx operators come from
  `using ReactiveUI.Primitives;`, not `System.Reactive.Linq` — there is no
  `System.Reactive` dependency; the scheduler abstraction is `ISequencer`, not
  `IScheduler`; and `RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build()`
  is mandatory, or the first `WhenAnyValue` anywhere throws and every exercise
  goes red for the wrong reason.

  Two more build traps measured during implementation: a file whose own
  namespace starts `FeWoLearning.Avalonia.…` cannot reference an Avalonia type
  fully qualified (`Avalonia.Media.TextWrapping` fails CS0234 — the leading
  segment binds to the enclosing namespace), though `using` directives are
  exempt; and an `.axaml` `<!-- -->` comment cannot contain two consecutive
  hyphens, so a literal `--filter` is a XAML compiler error — the real command
  lives in the `.axaml.cs`.

  A recurring bug class here: a headless test that never shows its control in
  a `Window` asserts on children that are all still `0,0,0,0` —
  `Measure`/`Arrange` alone does not apply the control template, and neither
  does `ApplyTemplate()`. Use `ViewHarness.Show`. The second: rendered
  geometry cannot prove *which* mechanism produced it — a test asserting only
  rectangles passed `RowDefinitions="24,*"` as happily as the `"Auto,*"` the
  exercise was about, so any exercise whose subject is a sizing or layout mode
  must also assert the definitions themselves. The third: a test that asserts
  only rendered text can be satisfied by a hard-coded literal in the XAML, so
  every binding exercise must change the view model afterwards, call
  `Dispatcher.UIThread.RunJobs()`, and assert the text followed.
- **Blazor** — The solution is `FeWoLearning.Blazor.slnx`, with **four**
  projects: `exercises/`, `solutions/`, `tests/`, `host/`. Like `avalonia/`,
  `solutions/` is deliberately **in** the build here (the repo-wide convention
  above is waived for the same reason: `tests/` and `host/` each reference
  exactly one of the two RCLs via the `UseSolutions` MSBuild property, never
  both, so the name collision the convention exists to prevent cannot occur).
  Things that actually cost time building this track: `-p:UseSolutions=true`
  swaps which RCL `tests/`/`host/` reference, and `Directory.Build.props` must
  redirect the solutions build's output via `UseArtifactsOutput`/
  `ArtifactsPath` — setting `BaseOutputPath`/`BaseIntermediateOutputPath`
  conditionally inside the `.csproj` body is read too late (before the SDK
  props import), so the stale default `obj/` gets globbed alongside the new
  one and the build fails with `CS0579`. Tier namespaces are pinned by a
  folder-level `_Imports.razor` (`@namespace FeWoLearning.Blazor.Exercises.Beginner`
  and friends), because `01-beginner` is not a valid C# identifier. A Razor
  component's type name **is its file name** — `Ex001_HelloComponent.razor`
  declares `Ex001_HelloComponent`. Stubs use one of two shapes: shape A throws
  from a computed member that markup references (`@Greeting`, where `Greeting`
  throws); shape B throws from a lifecycle method or event handler instead,
  because `throw` is illegal directly in Razor markup (`CS8115`). bUnit 2.x
  renamed `TestContext` to `BunitContext` (the old name collides with
  `Xunit.TestContext`, `CS0104`) and `SetParametersAndRender` to `cut.Render`.
  Each RCL needs a `FrameworkReference` to `Microsoft.AspNetCore.App`, or the
  Razor source generator cannot resolve `Microsoft.AspNetCore.Components` and
  every `.razor` file fails `CS0234`. `_support/` (identical in both RCLs)
  holds shared fixtures several exercises' tests depend on — it is never a
  TODO and never gets a `catalog.md` row.

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
   - `python`/`go`/`rust`/`dotnet`/`java`/`kotlin`/`flutter`: copy the track into the scratchpad, overlay each
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
| `java/`   | 100 / 100 (seeded, **unverified** — see below) | —  |
| `kotlin/` | 100 / 100 (seeded, **unverified** — see below) | —  |
| `flutter/`| 100 / 100 (seeded, **unverified** — see below) | —  |
| `avalonia/`| 10 / 100 (verified) | 90 |
| `blazor/` | 35 / 100 (verified) | 65 |

Every 100-exercise ledger is fully seeded except `avalonia/` and `blazor/`,
both still being built out — see the table above for exact counts. Nothing else is
"remaining" in the sense of unwritten content; `java/`, `kotlin/`, and
`flutter/` still need their first real compile/test run (see below) before
they can be trusted the way the other six tracks are.

`dotnet/`, `go/`, `vue/`, `python/`, `angular/` and `rust/` are content-complete
**and verified** (every stub confirmed red, every solution confirmed green,
by actually running that track's test command). `java/` and `kotlin/` are also
content-complete — Gradle scaffolds, all 100 stubs' sibling JUnit tests, and
all 100 reference solutions exist for each — **but nothing in either has ever
been compiled or run**: this machine has no JDK, Gradle, or Kotlin installed,
and both tracks were written by careful manual authorship and self/fork-review
instead of a red/green test cycle. Several real bugs were still found this
way without a compiler (e.g. Kotlin ex063's solution originally swallowed
every task exception with `runCatching`, which made its test pass under any
coroutine scope — not just `supervisorScope`, the thing it was supposed to
prove; ex072's test combined two fully-synchronous flows with no suspension
points, risking silent conflation of intermediate `combine` emissions).
Treat `java/` and `kotlin/` as substantially higher-risk than every other
track until someone with a real JDK 21 + Gradle (+ Kotlin, for `kotlin/`)
install runs `gradle test` against each for the first time — expect to find
and fix real compile errors then. `flutter/` is content-complete the same
way — all 100 exercises (stub + test + solution), authored by 6 parallel
agents each covering a batch of the catalog, then spot-checked by hand — but
carries the same unverified risk: no Flutter/Dart SDK on this machine to run
`dart analyze`/`dart test`/`flutter test`. See `flutter/README.md` for the
handful of exercises (ex069, ex094, ex095) flagged as extra-risky by the
agents that wrote them. `go/` was verified by
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
`blazor/`'s 35 written (beginner-tier) exercises carry 115 individual test
facts; `dotnet test` shows 115 failed/0 passed on the untouched tree, each
failure traced to its own exercise's `NotImplementedException`, and
`dotnet test -p:UseSolutions=true` shows 115 passed/0 failed. The `exercises/`
build itself carries 6 expected `CS0169`/`CS0414`/`CS0649` warnings for
fields that shape-B stubs declare for the learner to wire up — these are
intentionally left unsuppressed; `solutions/` builds with 0 warnings. See
`blazor/README.md` for the full list and for a sharp edge in ex035: a naive,
unbounded parent-refresh callback hangs the test host rather than failing it.

## Known gaps

`solutions/` is deliberately outside every build, so reference implementations are
largely **unverified** and can drift silently — see the "Known gaps" section of
[`docs/exercise-format.md`](docs/exercise-format.md) for the per-track table and
the manual overlay recipe. An audit on 2026-08-03 found five broken solutions in
`vue/` and four defective tests in `go/` that had gone unnoticed for exactly this
reason.

`blazor/` — like `avalonia/` — is a deliberate exception: its `solutions/` is
a real project referenced by `tests/`/`host/` under `UseSolutions=true`, so it
is compile-checked and test-run on every green check and cannot drift
silently the way described above.
