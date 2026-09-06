# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

FeWoLearning is a **polyglot skills-training monorepo**, not an application. It
holds independent, self-contained learning tracks — one per folder below —
each with its own toolchain, test runner, and a graded set of **exercises**
(stubs the learner implements) paired with reference **solutions**. There is
no shared build and no cross-track code. Treat each language folder as its
own project.

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
| `uno/`    | — (restore on first `dotnet test`)      | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |
| `caliburn/`| — (restore on first `dotnet test`)      | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |
| `wpf/`    | — (restore on first `dotnet test`)      | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |
| `MicroServices/`| — (restore on first `dotnet test`)| `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |
| `security/`| — (restore on first `dotnet test`)      | `dotnet test --solution FeWoLearning.Security.slnx` | `dotnet test --project tests/FeWoLearning.Security.Tests.csproj --filter-class "*Ex001*"` |
| `Architecture/`| — (restore on first `dotnet test`) | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |
| `telemetry/`| — (restore on first `dotnet test`)     | `dotnet test`            | `dotnet test --filter FullyQualifiedName~Ex001_` |

Run every command **from inside the track folder**, not the repo root.

For `blazor/`, `uno/`, `caliburn/` and `wpf/`, `dotnet test -p:UseSolutions=true` runs the identical
suite against the reference solutions instead of the stubs. `MicroServices/` supports the same
`-p:UseSolutions=true` flag, plus `-p:Containers=true` to additionally run the container-backed
rows (skipped by default). `dotnet run --project playground -- --exercise exNNN` runs a single
exercise in the Aspire dashboard. `Architecture/` supports `-p:UseSolutions=true` for its green run and
`-p:Containers=true` for its eight container-backed rows. `security/` supports the same
`-p:UseSolutions=true` flag for
its green run: `dotnet test --solution FeWoLearning.Security.slnx -p:UseSolutions=true`.

A footnote in the same spirit as the `wpf/`/`MicroServices/` entries below: `security/global.json`
carries the same `Microsoft.Testing.Platform` opt-in, and a bare, argument-less `dotnet test` here
was reported once as exiting 5 with zero tests discovered; repeated retests could not reproduce
it, so it is not treated as an established defect here — but if you ever do see zero tests, the
fix is the explicit `--solution`/`--project` form above. This applies to `wpf/` too, which carries
the same `global.json` opt-in.

## Toolchain status (verified 2026-09-04)

- ✅ Verified end-to-end: **.NET 10**, **Python 3.14**, **Node 26 / npm 11**
  (both `vue/` and `angular/` have `node_modules`), **Go 1.26.5**, **Rust 1.97.1**,
  **Avalonia 12.1.1 with ReactiveUI 24.1.0 on .NET 10**. The Avalonia set is
  pinned and coherent at 12.1.1 and must not be bumped piecemeal: `ReactiveUI.Avalonia`'s
  12.x line stops at 12.1.1, while Avalonia itself has already released 12.1.2.
  **Blazor's** beginner tier (35/100) is verified end-to-end as of 2026-09-04 on
  **.NET 10.0.400 with bUnit 2.9.0**: 115 stub facts red, 0 passed on the
  untouched tree; the same 115 facts pass under `-p:UseSolutions=true` — unlike
  `java/`, `kotlin/`, `flutter/` and `php/`. **Caliburn.Micro** 5.0.258 with
  **`Xunit.StaFact` 3.0.13 on xunit.v3 3.2.2, .NET 10.0.400** is likewise
  verified as of 2026-09-05: the beginner tier (001-035) is complete and the
  intermediate tier is through ex060, where
  `dotnet test` shows 349 failed, 8 passed on the untouched tree (349
  exercise facts across ex001-ex060, plus 8 harness smoke tests which pass
  in both modes) —
  and `dotnet test -p:UseSolutions=true` shows 357 passed, 0 failed. **`wpf/`**'s beginner tier
  (35/100, `01-beginner` ex001-ex035) is verified end-to-end as of 2026-09-05, on
  **.NET 10.0.400** with **xunit.v3 4.0.0** and **Xunit.StaFact 4.0.23**
  (`Microsoft.WindowsDesktop.App` 10.0.11): `dotnet test` shows 5 passed (the
  harness smoke tests) and 205 exercise facts red on the untouched tree; all
  210 facts pass under `-p:UseSolutions=true`, twice in a row, with zero
  warnings on both `--no-incremental` builds. Windows-only, because WPF is.
  `Xunit.StaFact` 4.x depends on
  `xunit.v3.extensibility.core` 4.0.0, so this track sits on xunit.v3 **4.0.0**
  while `avalonia/` and `caliburn/` sit on 3.2.2 — pinned independently, and
  adding xunit 2.x to any of them puts `FactAttribute` in two assemblies
  (`CS0433`). `wpf/global.json` pins
  `{"test":{"runner":"Microsoft.Testing.Platform"}}`, which is mandatory:
  xunit.v3 4.0.0 pulls in `Microsoft.Testing.Platform.MSBuild` 2.3.3, which
  refuses to run under the classic VSTest bridge on the .NET 10 SDK without
  that opt-in, and `dotnet test` fails outright with "Testing with VSTest
  target is no longer supported". `avalonia/` runs xunit.v3 3.2.2 and needs
  no such file — this is version-specific, and the next track to bump
  xunit.v3 will hit it too. **As of 2026-09-05, that same combination now
  makes `dotnet test` here exit 5 with zero tests discovered on this
  machine** — see the `MicroServices/` entry below for the measured detail;
  out of scope to fix here.
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
- **`MicroServices/`** is verified as of 2026-09-06 on **Aspire 13.5.3 with
  .NET 10.0.400**, **Docker 29.7.2**, **devcontainer CLI 0.89.0**, and
  **xunit.v3 3.2.2** (`xunit.runner.visualstudio` 3.1.5,
  `Microsoft.NET.Test.Sdk` 17.14.1) pinned on the classic VSTest path:
  `dotnet test` gives 95 exercise facts red, 7 harness facts passed, 1 skipped
  (103 total); `dotnet test -p:UseSolutions=true` gives 102 passed, 1 skipped,
  0 failed; `dotnet test -p:Containers=true` gives 95 red, 8 passed, 0 skipped. `Aspire.Hosting.Elasticsearch` is deliberately pinned at 13.3.0 —
  its own latest stable — while every other Aspire package on the track is
  13.5.3. This also surfaced a problem elsewhere in the repo: `wpf/` sits on
  xunit.v3 4.0.0 plus a `Microsoft.Testing.Platform` `global.json`, and on
  this machine that combination now makes `dotnet test` exit 5 with zero
  tests discovered, even though its test executable still runs its suite
  correctly when invoked directly — so the `wpf/` entries above claiming
  `dotnet test` as verified are no longer accurate here. Recorded as a known
  issue; fixing `wpf/` is out of scope for this track.

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

  **The suite must stay serial, and a parallel run lies about it.**
  `avalonia/tests/_harness/AssemblyInfo.cs` carries
  `[assembly: CollectionBehavior(DisableTestParallelization = true)]` — the same
  one-liner `uno/` and `caliburn/` have, and the same need `wpf/` meets with
  `[assembly: Parallelization(Mode = ParallelMode.None)]`. It was **missing here
  until 2026-09-05**, and the consequence is worth knowing because it is silent:
  every `[AvaloniaFact]` runs on the one headless dispatcher against a single
  `Application`, so two collections starting at once deadlock, and the run does
  not error — it simply stops, still printing a normal-looking summary for
  whatever had finished. Measured before the fix: the beginner tier passed as two
  halves of 52 and 59 tests but hung after 4 when asked for all 111, and a plain
  `dotnet test` hung after 27 of 225. Any earlier claim about this track rests on
  filtered per-batch runs; the full suite first ran end to end on 2026-09-05
  (225 tests: 218 red / 7 green — the 7 being the harness and gallery smoke
  tests — and 225 / 0 under `-p:UseSolutions=true`). **Read the test count, not
  just the word `Failed`.**

  **Animation progress is not assertable, by construction.**
  `ForceRenderTimerTick(n)` forces frames, not time, and there is no seam to
  inject a clock: `IClock`, `ClockBase`, `Clock` and `IGlobalClock` are all
  internal in Avalonia 12.1.1, as are `Animatable.Clock`'s accessors. What is
  deterministic instead: a transition **defers** the value (a plain `Border`
  reads `0.000` right after `Opacity = 0.0`, one carrying a `DoubleTransition`
  reads `1.000`); a style animation takes ownership immediately, so
  `GetDiagnostic(Visual.OpacityProperty).Priority == BindingPriority.Animation`
  is a location-free attachment proof when `IterationCount` is `Infinite`; and
  `Transform.Value` gives the matrix directly. Three traps behind that:
  `KeyFrame.Setters` is typed `IAnimationSetter`, whose `Property`/`Value` are
  not publicly accessible (CS0122) — cast to `Avalonia.Styling.Setter`;
  `RenderTransform` itself **cannot** be animated ("No animator registered"), so
  animate a sub-property such as `RotateTransform.Angle`, after which Avalonia
  installs a `TransformGroup` and `RenderTransform`'s priority stays
  `LocalValue`; and `RenderTransformOrigin="0.5,0.5"` parses as **absolute** half
  a pixel, not the centre — `"50%,50%"` is `RelativePoint.Center`. The full
  register is in `avalonia/README.md`.

  Its `01-beginner` (ex001-ex035) and `02-intermediate` (ex036-ex070) tiers are
  both complete as of 2026-09-05, verified by a full-suite run in both modes:
  246 test facts, 239 red / 7 green on the untouched tree (the 7 green are the
  harness and gallery smoke tests, which pass in both modes) and 246 / 0 under
  `-p:UseSolutions=true`. One more measured trap from that tier, because it will
  bite any converter row: a `MultiBinding` calls its `IMultiValueConverter` once
  per binding *as each one settles*, and the first call carries nothing at all —
  measured `[UnsetValue, UnsetValue, UnsetValue]`, then the values filling in
  left to right. A `Convert` that indexes and casts blindly therefore throws
  before the view has finished loading, and a converter's call count must never
  be asserted.

  **Nothing a `Render` override draws is observable, and one of the ways to try
  it lies.** `DrawingContext` has a private constructor (no recording double);
  the recorded render data is entirely internal (`RenderDataDrawingContext`,
  `CompositionRenderData`, `Visual.CompositionVisual`); and the headless backend
  discards draw commands while `RenderTargetBitmap.Render` + `CopyPixels`
  throws nothing and hands back plausible bytes — a solid red 8×8 `Border`
  measured **22 distinct pixel values**, i.e. noise. Never assert on those
  pixels. `Window.GetLastRenderedFrame()` states the cure in its own exception:
  build the app with `.UseSkia()` and `UseHeadlessDrawing` off. This track does
  not, so ex071 grades the coordinate maths and documents the gap; switching the
  harness over is a worthwhile separate pass, not a drive-by edit. Reliable
  instead: `Render` *is* called and its exceptions surface at `RunJobs()` — not
  at `Show()`, unlike `MeasureOverride`/`ArrangeOverride`, which throw
  synchronously inside it; layout arithmetic is exact; and
  `Geometry.GetRenderBounds(pen)` inflates bounds by exactly half the thickness
  (verified at 1, 4 and 10). Two geometry APIs that must never carry an
  assertion here: **`FillContains`**, which reported a *solid* arrow's centre
  row as hollow and ignores the fill rule outright (a self-intersecting star
  read as filled under `EvenOdd` and `NonZero` alike), and **`StrokeContains`**,
  which returned false for a point plainly inside a 10 px stroke. Grade a shape
  as a `PathGeometry` and walk `Figures[i].Segments[j]`; a `StreamGeometry` is
  write-only by design and cannot be inspected at all — which is why
  `catalog.md` row 074 now reads `PathGeometry` where it once said
  `StreamGeometry`.

  **Input, by contrast, works properly — with one trap that costs an afternoon: a
  control with no `Background` is invisible to the pointer.** Measured, the same
  control with the same overrides received *nothing at all*, at any position,
  until it had one; `Brushes.Transparent` suffices and behaves like an opaque
  brush, because hit testing asks what was painted, not what was arranged. The
  corollary shapes tests: a negative input assertion ("nothing happened") is
  equally satisfied by nothing having *arrived*, so anchor every negative to a
  positive one in the same test. Also measured: `KeyPress` now requires a
  `PhysicalKey` (the old three-argument overload is gone; `KeyPressQwerty` is the
  short route), `KeyBindings` on an ancestor fire while a descendant holds focus,
  `IsTabStop` gates traversal but not `Focus()`, and traversal wraps. Three things
  input cannot show: pointer **capture** makes no observable difference (moves
  outside arrive either way), `ScrollGesture` never fires from mouse input and
  `ScrollGestureRecognizer` is not public, and `Avalonia.Input.Gestures` is not
  public either, so the `Gestures.AddTappedHandler` route in most samples does not
  compile — hence `catalog.md` row 078 now names tapped/wheel rather than scroll
  gestures. **Repaints need `ViewHarness.PumpRender()`** —
  `ForceRenderTimerTick(1)` then `RunJobs()` — and one window per test: a bare
  `RunJobs()` flushes a frame only for the first window and only once, which
  silently reports zero repaints on any later measurement. With the pump the
  behaviour is exact: idle pumps repaint nothing, each `InvalidateVisual` costs
  one repaint, and five before a single pump coalesce into one.

  **Three more APIs that moved, and every tutorial predates the move.** Drag
  payloads and the clipboard both use **`DataTransfer`**: `DataObject` and
  `DataFormats` are `[Obsolete]` and `IDataObject` is gone, so the old names put
  warnings in a build this track keeps at zero. Watch the sync/async split — a
  drop hands you an `IDataTransfer` with `TryGetRaw`, the clipboard an
  `IAsyncDataTransfer` with `TryGetRawAsync` (or the tidier
  `TryGetTextAsync()` extension). Both work headlessly: a synthesised
  `window.DragDrop(...)` raises enter/over/drop with a readable payload, and
  **nothing at all arrives without `DragDrop.SetAllowDrop`**; the clipboard
  round-trips, and after `ClearAsync` `TryGetDataAsync` returns **null** rather
  than an empty transfer (it is process-global, so a serial suite must clear
  before trusting it). And **change sets here are ReactiveUI's own, not
  DynamicData** — `ToReactiveChangeSet`/`IReactiveChangeSet<T>`/`ReactiveChange<T>`
  live in `ReactiveUI.Core`, this track references no DynamicData, and there is
  no `Filter`/`Sort`/`Transform` operator, so applying changes is the work.
  Measured: subscribing emits one set describing what is already there (all as
  `Add`); `Remove` puts the removed item in **`Current`**, not `Previous`;
  `Clear()` expands into one `Remove` per item rather than a reset;
  `IReactiveChangeSet<T>.Count` counts *changes*, not items; and
  `WhenCountChanged()` passes an add or remove but not a replace or move.
  **Virtualization is real**: 500 rows realized 2/4/9 containers at 60/120/300-unit
  viewports, and `ScrollIntoView(300)` recycled index 0 to null — while an
  `ItemsPanel` of a plain `StackPanel` realized all 500.

  **`03-advanced` (ex071–ex090) is complete as of 2026-09-06**, verified by a
  full-suite run in both modes: 408 facts, 401 red / 7 green on the untouched
  tree and 408 / 0 under `-p:UseSolutions=true`. Four more findings from its last
  batch. **`TryGetResource` does not walk up the tree** — a control inside a
  panel returned false for every one of that panel's keys, and the panel false
  for its window's; each host answers only for its own dictionary plus what is
  merged into it, and tree-wide inheritance is a property of the *binding*
  (`DynamicResource`), not the lookup. Same for theme dictionaries, so a consumer
  binds rather than looks up. **Merged precedence**: a host's own entries beat
  every merged dictionary, and among the merged ones the *last added* wins.
  **To extend a FluentTheme control theme** you need `BasedOn`, and the theme is
  findable only on the application — a styled `Button`'s `Theme` is null and
  neither it nor its window answers for `typeof(Button)`; an implicit theme in a
  host's `Resources` scopes to that subtree but must be in place *before* the
  host is shown. **Right-to-left mirrors text and nothing else observable**:
  panel `Bounds` were identical in both directions, `RenderTransform` null,
  `TransformToVisual` the identity and `HasMirrorTransform` false, because
  non-text mirroring happens render-side — but `TextBlock.TextLayout` mirrors
  exactly (`TextLines[0].Start` moved from 0 to `Width - TextLayout.Width`), so
  that is what ex090 grades. Also worth knowing for any culture-dependent code
  here: **the ambient culture on this machine is `de-CH`/`de-DE`, not English**,
  so an explicit `CultureInfo` is mandatory in code and tests alike.

  Note that expert row **098 is `RenderedFrameCapture` — `CaptureRenderedFrame`
  pixel assertions**, so the Skia harness switch that ex071 documents as a gap
  is not optional for the last tier: it is that row's whole subject, and it wants
  doing as its own pass with a full re-verification before ex091–ex100 start.

  **ex091–ex095 are done (95 / 100)**, verified at 446 facts — 439 red / 7 green
  untouched, 446 / 0 under `-p:UseSolutions=true`. What the expert tier turned up
  about ReactiveUI 24's own plumbing: **`ViewLocator.Current` is read-only** (no
  runtime global swap; `ViewModelViewHost.ViewLocator` *is* settable per host, and
  that is how ex092 gets a locator into a host); **`IViewLocator` has four
  members**, and the two generic overloads may return null but must exist;
  **the resolver is Splat's**, and since `Locator.Current` is process-global the
  exercises use an isolated `ModernDependencyResolver` (public, parameterless
  ctor) instead of mutating it. `DependencyResolverRegistrar` lifetimes measured:
  `Register` transient with the factory run every time, `RegisterLazySingleton`
  once on first resolve, `RegisterConstant` at registration, and an unregistered
  type resolves to **null** rather than throwing. `DefaultViewLocator` plus
  `ViewMappingBuilder.Map<TViewModel,TView>(Func<TView>)` is the framework's
  explicit, factory-based registration — unmapped resolves to null, each resolve
  re-runs the factory, and `IViewFor.ViewModel` is assigned for you.
  **`Disposable.Create`/`DisposeWith` are in no obvious ReactiveUI 24 namespace** —
  `ReactiveUI.Disposables` is an assembly, not a namespace; use the
  `WhenActivated((Action<IDisposable> register) => …)` form ex048 established.
  On bindings: a correct path makes compiled and reflection bindings
  indistinguishable, a misspelt **reflection** path renders nothing *silently*
  (a `FallbackValue` is its only net), and the same misspelling compiled is a
  build error — which is why ex094 grades that trade-off and not wall-clock cost,
  since this track makes no timing claims. Finally,
  **`Activator.CreateInstance` throws `MissingMethodException`** for a view whose
  constructor takes arguments, so a name-based locator crashes rather than
  degrading — that, plus its willingness to resolve types nobody registered, is
  what ex095 grades as the trimming failure in miniature.

  **The track is complete at 100 / 100 as of 2026-09-06**: 485 facts, 478 red / 7
  green on the untouched tree and 485 / 0 under `-p:UseSolutions=true`,
  `solutions/` at 0 warnings. **The harness now renders for real**: ex098 is
  `CaptureRenderedFrame`, which refuses outright without `.UseSkia()` and
  `UseHeadlessDrawing = false`, so `tests/` gained an `Avalonia.Skia` 12.1.1
  reference and `TestAppBuilder` sets both. Pixels come back as `Rgba8888` — byte
  0 is **red**, not blue — and a solid fill captures exactly. Two follow-on
  consequences, both fixed rather than left to rot: the old claim that pixels are
  uninitialized noise was true only of the null backend and is gone from
  `avalonia/README.md` and ex071's header; and `Geometry.GetRenderBounds(pen)` is
  **backend-dependent** — it used to be a plain bounding-box inflate by half the
  thickness, and Skia computes the true stroke outline, which moved ex074's
  chevron from 104 wide to 101.37, so that test now asserts a relationship rather
  than a rectangle. Also measured in this last batch:
  **`Application.ApplicationLifetime` is null** under headless (no head installs
  one), so row 096 is re-scoped to owned windows and `Closing` cancellation and
  the catalog says so; **ReactiveUI's own `RegisterViewsForViewModels` throws** on
  the first view without a parameterless constructor, losing the whole scan, which
  is why ex097 has the learner write a skipping one; a second `AppBuilder` can be
  **composed but not started** and is fully inspectable, which is how ex099 is
  graded without pulling `Avalonia.Headless`/`Themes.Fluent` into a content
  library; and **`ReactiveCommand` needs `Sequencer.CurrentThread`** to be
  observable inline — draining the dispatcher afterwards does not help, and
  waiting on `IsExecuting` returns its current `false` before the command starts.
- **Caliburn** — The solution is `FeWoLearning.Caliburn.slnx`; three projects
  (`exercises/`, `solutions/`, `tests/`). `solutions/` is deliberately **in**
  the build, the same waiver `avalonia/`, `blazor/` and `uno/` take, so
  reference solutions are compile-checked — do not "fix" it back to match the
  rest of the repo. Tier namespaces are pinned
  (`FeWoLearning.Caliburn.Exercises.Beginner` and friends) because
  `01-beginner` is not a valid C# identifier. **`Xunit.StaFact` is pinned to
  3.0.13, and 4.x must not be used**: 4.x needs xunit.v3 4.0.0, which dropped
  the VSTest bridge, and `dotnet test` then dies on the .NET 10 SDK with
  "Testing with VSTest target is no longer supported by
  Microsoft.Testing.Platform". The `TestingPlatformDotnetTestSupport` property
  alone did not fix it. 3.0.13 sits on xunit.v3 3.2.2, the same generation
  `avalonia/` runs. (`wpf/` *does* run `Xunit.StaFact` 4.x successfully, by
  opting into the `Microsoft.Testing.Platform` runner in `wpf/global.json`;
  `caliburn/` stays on 3.0.13 instead so it keeps the VSTest path.) **A
  Caliburn action only fires when the view is hosted in
  a real `Window`.** `Microsoft.Xaml.Behaviors` triggers will not resolve
  their source without a `PresentationSource`; `Measure`/`Arrange`,
  `ApplyTemplate()` and hand-raised `Loaded` all fail to supply one. Hence
  `CaliburnViewContext.Show(...)`, which opens a real window off-screen at
  zero opacity — **and hence the track needs an interactive desktop session
  and will not run in a service or session-0 context.**
  **Guard evaluation and action invocation have different thresholds.**
  Measured: with a `CanXxx` guard that is false from the start,
  `ViewModelBinder.Bind` alone leaves the button **enabled** and un-gated, and
  so does a `Measure`/`Arrange` pass. The guard is evaluated on the
  **`Loaded` event** — the harness's `Load(view)`, which raises `Loaded` by
  hand, is enough. **Invocation** is the stricter one: it needs a real
  `PresentationSource`, so only `Show(view)` makes a click reach the method.
  A test that only calls `Layout` proves neither; one that calls `Load`
  proves the guard but not the action. `ActionMessage` gates only
  `IsEnabled`, not execution: raising `ButtonBase.ClickEvent` programmatically
  on a guard-**disabled** button still invokes the method. WPF blocks real
  input, `RaiseEvent` does not. `IoC` must be
  initialized even with no UI: `Coroutine.BeginExecute` calls `IoC.BuildUp`.
  `XamlPlatformProvider` captures `Dispatcher.CurrentDispatcher` in its
  constructor, so it leaks across tests; `CaliburnCoreContext` resets
  `PlatformProvider.Current` per test, and tests run serially.
  `FrameworkElement.LoadedEvent` is a *direct* routed event — raising it on a
  view never reaches that view's children. An element must not be named
  after a `FrameworkElement` member: `x:Name="Name"` hides
  `FrameworkElement.Name` (`CS0108`). Exercises use `UserName`-style names.
  The `exercises/` build emits expected `CS0067`/`CS0649`/`CS0169`/`CS0414`
  warnings from stubs whose members throw; `solutions/` builds with **0
  warnings** and a warning there is a finding; `tests/` suppresses
  `xUnit1051` only, via `NoWarn` in its `.csproj`, and any other warning there
  is a finding too — the full register is in `caliburn/README.md`. Same
  stance as `blazor/`.
  Caliburn.Micro 5 marks `Screen.OnInitializeAsync` and `Screen.OnActivateAsync`
  `[Obsolete]`, with the messages "Override OnInitializedAsync" and "Override
  OnActivatedAsync". Overriding the obsolete pair puts `CS0672` in the build,
  which breaks the track's zero-warnings rule for `solutions/`. Both members
  of each pair genuinely exist and both run — measured order on a first
  activation is `OnInitializeAsync` → `OnInitializedAsync` → `OnActivateAsync`
  → `OnActivatedAsync` — so they are the same lifecycle point and the
  non-obsolete name is the one to override. `OnDeactivateAsync` is **not**
  obsolete and has no `OnDeactivatedAsync` counterpart.
  `tests/_harness/CaliburnCoreContext.cs` resets six process-global Caliburn
  statics before every test — `PlatformProvider.Current`,
  `AssemblySource.Instance`, the `IoC` delegates,
  `ViewLocator.NameTransformer`, and (see below) `AssemblySource.FindTypeByNames`
  and `AssemblySourceCache.ExtractTypes` — and that list is incomplete **by design**:
  it does not yet reset `ViewLocator.LocateTypeForModelType`,
  `ViewModelLocator`'s own separate `NameTransformer` (measured: a genuinely
  different object from `ViewLocator.NameTransformer`, not an alias) and
  locator delegates, `ViewModelBinder.*`, `MessageBinder.*`,
  `ActionMessage.*`, `LogManager.GetLog` or `BindingScope.GetNamedElements`.
  There is no public `ConventionManager.ElementConventions` — the real
  surface is `ConventionManager.AddElementConvention`, which writes into a
  **private** static dictionary with no public removal, so it cannot be
  reset the way the others above can; ex020 (`CustomElementConvention`)
  handles this by registering its convention only for a type the exercise
  itself owns, so the unresettable leak can never affect another test.
  Whoever writes the first exercise that mutates one of the resettable
  statics above must extend the harness in the same style, or their
  mutation leaks into every later test in the serial run. `caliburn/README.md`
  carries the full register. A subtle trap found while building that reset,
  worth recording because it is not Caliburn-specific: the pristine snapshot
  must be taken in an **explicit static constructor**, not a static field
  initializer — a field initializer leaves the type `beforefieldinit`, which
  lets the runtime defer initialization until the first read of that field,
  and that read happens *after* the same instance constructor's `Clear()`
  call, so the snapshot comes back empty and permanently zeroes the
  collection for the whole run. Measured on this machine.
  **`BootstrapperBase.Initialize()` permanently rewires type lookup.** It
  calls `AssemblySourceCache.Install()`, which swaps
  `AssemblySource.FindTypeByNames` for a cached lookup that only finds types
  assignable to `INotifyPropertyChanged` (the WPF bootstrapper widens it to
  `UIElement` as well), guarded by a private `isInstalled` flag so it never
  happens twice and never reverts. Measured consequence: once any test calls
  `Initialize()`, exercises resolving plain POCO view models — ex016's
  `ViewModelLocator` — fail for the rest of the run.
  `tests/_harness/CaliburnCoreContext.cs` therefore snapshots and restores
  both `AssemblySource.FindTypeByNames` and `AssemblySourceCache.ExtractTypes`;
  the harness now resets **six** process-globals per test.
  **`SimpleContainer.BuildUp` injects interface-typed properties only.**
  Measured: an interface-typed property is injected whether its setter is
  public or private; a **concrete**-typed property is never injected, even
  when that exact concrete type is registered; fields are never injected.
  The same namespace-shadowing trap the file already records for `avalonia/`
  applies here: a file whose own namespace starts `FeWoLearning.Caliburn.…`
  cannot reference a Caliburn type fully qualified —
  `Caliburn.Micro.Action.SetTarget(...)` fails `CS0234`, because the leading
  `Caliburn` segment binds to the enclosing `FeWoLearning.Caliburn`. `using
  Caliburn.Micro;` directives are exempt; the workarounds are a `using
  CaliburnAction = Caliburn.Micro.Action;` alias or `global::Caliburn.Micro.…`.
  This bites stub TODO strings especially, since a learner types those
  verbatim. **A measured fact that reframes several exercises:**
  `ViewModelBinder.Bind(viewModel, view, null)` calls `Action.SetTarget` on the
  root under the hood. Measured on a freshly parsed view with no
  `DataContext` assignment at all, `Bind` alone leaves `DataContext` set to
  the view model, `Action.HasTargetSet` true, `GetTarget` set and
  `GetTargetWithoutContext` null. That is why the `$view` special value
  resolves to the bound view rather than collapsing onto `$source` — the
  collapse only happens where nothing up the tree ever had a target set.
  **`EventAggregator` holds its subscribers weakly.** Measured: drop the only
  strong reference, force a full GC, and `HandlerExistsFor` returns false with
  nothing delivered. Forgetting to unsubscribe therefore does not leak — but a
  subscriber nobody else owns *silently stops working*, which is harder to
  diagnose than a leak. Consequence for tests: any test that subscribes an
  object must keep it reachable at the assertion, with `GC.KeepAlive` or by
  reading a member of it. Measured on a local whose last read precedes the
  assertion: 0/1000 collected under the default runtime config, but
  **615/1000 under Server GC and 1000/1000 with `TieredCompilation=0`** — so a
  test can be green here and spuriously green, or spuriously red, elsewhere.
  **Check whether a Caliburn member is on the interface or an extension
  method before writing it down.** `IConductor` declares `ActivateItemAsync`,
  `DeactivateItemAsync` and `ActivationProcessed`; `CloseItemAsync` is a
  `ScreenExtensions` extension. `IEventAggregator` declares `Subscribe`,
  `PublishAsync`, `Unsubscribe`, `HandlerExistsFor`; the
  `…OnUIThread`/`…OnBackgroundThread` variants are extensions. Both are in the
  `Caliburn.Micro` namespace, so `using Caliburn.Micro;` makes the extensions
  look like interface members at the call site — and an exercise that claims
  a member "does not exist" is falsified by one IntelliSense keystroke.
  **A coroutine step that never raises `Completed` hangs the test instead of
  failing it.** `Coroutine.ExecuteAsync` returns a task that completes only
  when the chain does, so a wrong implementation stalls the suite rather than
  going red — the same sharp edge ex010 documents for a blocking guard.
  `tests/_harness/CaliburnCoreContext.cs` therefore provides `BoundedAsync`,
  `BoundedAsync<T>` and `BoundedExceptionAsync`, which race the coroutine
  against a 5-second delay and assert it won *before* observing its result.
  Every coroutine await in the track uses them. Note the ordering matters:
  wrapping a bounded await in `Record.ExceptionAsync` swallows the timeout
  assertion and turns a hang back into a silent pass.
  **Coroutine surface, measured:** `IResult` is
  `Execute(CoroutineExecutionContext)` plus a `Completed` event; `IResult<T>`
  adds **`Result`**, not `Value`; `ResultCompletionEventArgs` carries only
  `Error` and `WasCancelled`. A step setting `WasCancelled` makes
  `ExecuteAsync` throw `TaskCanceledException`; a step setting `Error` makes
  it throw that same exception with its message intact; either way no later
  step runs. `TaskExtensions.AsResult()` adapts `Task`/`Task<T>`, and a
  **faulted task surfaces as `AggregateException`** — unlike a hand-written
  `IResult` setting `Error`, which surfaces the original directly.
  **A modal dialog cannot be escaped by a timeout — only by closing the
  window.** `WindowManager.ShowDialogAsync` awaits `CreateWindowAsync` (which
  completes synchronously) and then calls `Window.ShowDialog()`, which pushes
  its own managed `DispatcherFrame`. The STA test thread is blocked *inside*
  that call, so a `Task.WhenAny(dialogTask, Task.Delay(...))` losing the race
  cannot unwind it — the timeout continuation runs only because the frame
  keeps pumping, and the sole way out is to close the window.
  `tests/_harness/CaliburnViewContext.cs` therefore schedules the close from
  inside the frame *before* showing, and on timeout re-derives the hosting
  window from the root model (`((IViewAware)vm).GetView()` yields it while
  the dialog is open) and force-closes it. Anything less hangs the suite
  instead of failing it — and the hang is reachable from a *correct* learner
  implementation that merely `await`s something before calling
  `ShowDialogAsync`.
  **`WindowManager` mutates the settings dictionary you hand it.** Measured: a
  shared static settings dictionary works for the first dialog and is
  consumed thereafter, so every later dialog silently renders as a real,
  visible, centred window instead of the invisible one you configured. Hand
  out a fresh dictionary per call. This was worth roughly 75 seconds of wall
  clock across one suite run before it was found.
  **Dialog semantics, measured:** `TryCloseAsync(true)` → `true`,
  `TryCloseAsync(false)` → `false`, and **`TryCloseAsync(null)` → `false`, not
  `null`** — a `bool?` return offers three shapes but only two
  distinguishable outcomes, so an application needs its own state to tell
  "dismissed" from "declined". A `UserControl` view is wrapped in a plain
  `Window`; a `Window`-derived view is used as-is; after the dialog closes
  `GetView()` returns null. The settings dictionary applies arbitrary window
  properties, but **not size or position**: `EnsureWindow` sets
  `SizeToContent` and a centred `WindowStartupLocation` *before* the
  dictionary is applied, the dictionary's `Width`/`Left` do land on the
  window, and WPF then discards them at `Show()` time to honour those two.
  **`CanCloseAsync` is not always a pure query.** Measured: with the default
  `DefaultCloseStrategy<T>`, one refusing child makes the strategy's
  `Children` come back empty, so asking closes nothing. But a strategy that
  returns a willing subset alongside `CloseCanOccur == false` —
  `DefaultCloseStrategy<T>(closeConductedItemsWhenConductorCannotClose:
  true)`, or any custom `ICloseStrategy<T>` that does the same — makes that
  very same `CanCloseAsync()` call deactivate those children with
  `close: true` and remove them from `Items`. The flag never changes
  `CloseCanOccur`; it changes what happens to the children that *were*
  willing.
  **Validation: the two interfaces are not symmetric.** Measured on a
  convention-created binding: a view model implementing `IDataErrorInfo` gets
  `ValidatesOnDataErrors == true` (a plain one gets `false`) — Caliburn's
  convention flips it for you. A view model implementing
  `INotifyDataErrorInfo` changes nothing at all: both it and a plain view
  model get `ValidatesOnDataErrors == false` and
  `ValidatesOnNotifyDataErrors == true`, because the latter is WPF's own
  default and the newer interface needs no help. A test asserting on
  `ValidatesOnNotifyDataErrors` therefore discriminates nothing.
  **`ConventionManager.DefaultItemTemplate` is one process-wide static
  `DataTemplate`, and WPF pins a `DependencyObject`'s `Dispatcher` to the
  first thread that realizes it.** Since every `[WpfFact]` runs on its own
  STA thread, any test that calls `LoadContent()` or reads
  `Triggers`/`VisualTree` on that shared template passes in isolation and
  then throws `InvalidOperationException` in a later test in the same run.
  Reference comparisons (`Assert.Same(ConventionManager.DefaultItemTemplate,
  control.ItemTemplate)`) are safe because they read no dependency property.
  `caliburn/README.md` carries the full register entry.
  **`ViewModelBinder` never consults an element's convention for a name it
  cannot match.** An element whose `x:Name` matches no view-model property
  gets **no binding on any dependency property** — on every element type,
  not just ones whose own convention is unusual. The `FrameworkElement`
  fallback that binds `Visibility` is reached only when the name *does*
  match and the element type has no more specific convention. Measured and
  tested in ex017; ex058 initially got this backwards and was corrected.
  **`ConventionManager.ApplyItemTemplate` keys on the items' runtime type,
  not the property's static type.** Measured: `DefaultItemTemplate` is
  assigned for any reference type except `string` — including a plain
  `object` with no Caliburn relationship — and a property declared as
  non-generic `IEnumerable` gets it just the same, refuting the intuition
  that the collection property must be generic. It does short-circuit,
  leaving the control alone, if `DisplayMemberPath` is already non-empty or
  `ItemTemplate`/`ItemTemplateSelector` is already set.
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
- **MicroServices** — The folder is capitalised, `MicroServices/`, unlike every
  other track's lowercase name. Deliberate, do not "fix" it. `solutions/` is
  deliberately **in** the build, the same waiver `blazor/`, `uno/`, `wpf/`,
  `caliburn/` and `avalonia/` take, so reference solutions are compile-checked
  and cannot drift silently.

  **xunit.v3 4.0.0 plus a `Microsoft.Testing.Platform` `global.json` makes
  `dotnet test` exit 5 with zero tests discovered on this machine.** This
  track therefore pins xunit.v3 3.2.2 on the classic VSTest path and ships no
  `global.json` — see the toolchain-status entry above for how that same
  combination now breaks `wpf/`'s `dotnet test`, unfixed and out of scope
  here. A related trap: `xunit.runner.visualstudio` has no 3.1.6 or 3.1.7;
  3.1.5 is the last 3.x release and the next is 4.0.0. Naming a nonexistent
  3.x patch version does not fail the build — NuGet silently resolves forward
  to 4.0.0 with only an `NU1603` warning, landing back on the broken
  generation without an error to catch it.

  `FactAttribute.Skip` is not virtual in xunit.v3 3.2.2, so the idiomatic
  custom `[ContainerFact]` overriding `Skip` fails with `CS0506`. The
  container gate uses `Assert.SkipUnless` in the test body instead, gated by
  `-p:Containers=true` — an MSBuild property that is otherwise invisible at
  runtime, so it reaches the test process through a
  `RuntimeHostConfigurationOption` read back via `AppContext.GetData`.

  `aspire publish` writes its artifacts and then never exits in a
  non-interactive shell — it must stay out of any test loop. In-process
  publish returns in about 3.7 seconds and emits `aspire-manifest.json` plus
  the Bicep module files; Docker Compose YAML is the one artifact NOT
  obtainable in-process, since every publisher argument combination yields
  the manifest instead. Declaring two compute environments without assigning
  resources to either fails the `validate-compute-environments` pipeline
  step.

  The recurring bug class: rendered connection data does not prove the
  mechanism (spec §8.2). A test asserting only that "a Postgres-ish container
  exists" is satisfied by a bare `AddContainer`, so every persistence exercise
  asserts both the resource type and the connection-string expression. A real
  example from this track's own first batch: asserting a `WaitAnnotation`
  merely names the right resource is satisfied equally by `WaitForCompletion`,
  which models the opposite promise — the wait *type* must be asserted too.

  **Tests run serially now.** `tests/` carries `[assembly:
  CollectionBehavior(DisableTestParallelization = true)]`, because several
  exercises keep process-global mutable state (a static `ActivitySource`,
  health-check flags, an eventing hook log) that is only safe if test
  classes never run concurrently. Note the version specificity: that's the
  spelling xunit.v3 **3.2.2** accepts — `wpf/`'s `[assembly:
  Parallelization(Mode = ParallelMode.None)]` is a 4.x form, and
  `ParallelizationAttribute` does not exist in 3.2.2 at all. The cost is
  real: the green suite went from about 26 seconds to about 1 minute 8
  seconds.

  Both content libraries now carry a `FrameworkReference` to
  `Microsoft.AspNetCore.App`, plus OpenTelemetry 1.18.0,
  `Microsoft.Extensions.ServiceDiscovery` and
  `Microsoft.Extensions.Http.Resilience` 10.9.0. Service-side exercises live
  in the same `exercises/`+`solutions/` pair as the AppHost-modelling ones
  rather than in a third project pair, deliberately, so the `UseSolutions`
  switch stays at one pair. The consequence worth recording: the repo's
  "ASP.NET Core work belongs to `blazor/`" boundary is from here on a
  judgement call rather than a compile error.

  The model and the manifest disagree about MongoDB's connection string —
  the model renders the raw password placeholder while the manifest renders
  a URI-encoded one. Any later Mongo row must decide which it is grading.

  Two catalog rows were corrected after being written, because the catalog
  is this track's spec and a wrong row propagates: row 018 claimed a fixed
  host port and replicas are contradictory, which is measurably false
  (Aspire polices neither; a proxied endpoint puts one listener in front of
  N instances); row 027 was a near-duplicate of ex001 and ex014 and was
  re-scoped onto `AddDatabase(name, databaseName)`. The general lesson: when
  a row turns out to misstate behaviour, fix the row itself, not just the
  exercise header.

  The devcontainer does **not** use the `docker-outside-of-docker` or `node`
  devcontainer features — they fail on this network with `NO_PUBKEY
  62D54FD4003F6525` during apt signature verification (corporate TLS
  interception without the root CA inside the build). It bind-mounts the
  host Docker socket and installs version-pinned static `docker` and `node`
  binaries over plain HTTPS instead. Measured: builds in ~85 s, `docker ps`
  works inside as the non-root `vscode` user, and `dotnet test` inside
  matched the host at the time (4 passed / 1 skipped, before any exercise
  landed). It is **not verified end-to-end**, and `MicroServices/README.md`
  §8 is the authority on why: the bar was Aspire starting a sibling database
  container from inside the devcontainer, and that has never been run. The
  named risk is that the socket is bind-mounted with **no host networking**,
  so containers Aspire starts are siblings created by the host daemon and
  publish their ports on the *host's* `localhost`, not the devcontainer's —
  the classic docker-outside-of-docker breakage, which `docker ps`
  succeeding proves nothing about. Whoever lands the first 🐳 row (034) runs
  it inside the container and records the result.
- **Security** — 60 rows across four **attack-surface blocks**
  (`01-web-aspnet`, `02-web-blazor`, `03-desktop-core`, `04-desktop-wpf`), not
  100 rows in four difficulty tiers like every other track: "beginner" is not
  a meaningful axis for security, since a path-traversal guard is not
  conceptually harder than a CSP header — they are different attack surfaces,
  and difficulty rises *within* each block instead. The solution is
  `FeWoLearning.Security.slnx`, three projects (`exercises/`, `solutions/`,
  `tests/`) sharing the `UseSolutions` mechanism with `wpf/`, `blazor/`,
  `uno/`, `caliburn/` and `avalonia/`: `exercises/` and `solutions/` compile
  the same type names into the same namespaces and `tests/` references
  exactly one of them, so `solutions/` is compile-checked on every green run.
  Namespaces are pinned per **block**, not per folder, because `01-web-aspnet`
  is not a valid C# identifier: `FeWoLearning.Security.Exercises.WebAspNet` /
  `.WebBlazor` / `.DesktopCore` / `.DesktopWpf`.

  **The recurring bug class is the track's whole point: an attack fact with
  no paired use fact grades nothing**, because a reject-everything
  implementation passes it for free — `Ex004_PathTraversalGuard`'s validator
  returning a constant `false` passes every traversal payload ever written,
  and only a paired use fact ("the legitimate file is still served") catches
  that degenerate. Every batch in this track was checked by actually building
  reject-everything variants of its stubs and confirming only the paired use
  facts, and not the attack facts, failed against them.

  **The bug class one level past that, and the thing the final whole-work
  review actually found, is *wrong-but-implemented*:** an earnest
  implementation that does real work but picks the wrong mechanism. It is not
  degenerate, so the reject-everything probe never catches it, and it passes
  because the facts assert an outcome more than one mechanism produces. Four
  exercises were under-grading exactly this way after every per-batch review
  had passed them — `Ex007` (any of the three encoders escapes all four
  payloads), `Ex023` (an extension-only allowlist rejects the disguised
  `report.pdf` before sniffing a byte), `Ex025` (a denylist where the track
  teaches allowlists) and `Ex041` (plain `string.Equals` satisfies every
  behavioural fact a constant-time compare does). Three were fixed by
  asserting a property only the right mechanism has — a round-trip through
  the decoder that sink's real consumer would use, a use fact that forces the
  attacker's own case onto the happy path, an assertion on the parsed DOM
  rather than on known-bad substrings. `Ex041` cannot be fixed without a
  timing assertion or IL reflection and is documented as behaviourally graded
  instead. **So: after the reject-everything variant, build the plausible
  wrong one too.** `security/README.md`'s "How a security test lies" is the
  long form.

  Toolchain traps, all measured: **bUnit 2.9 still ships an obsolete
  `Bunit.TestContext`**, which collides with xunit.v3's `Xunit.TestContext`
  (`CS0104`) the moment a test file has `using Bunit;` and also touches
  `TestContext.Current.CancellationToken` — fixed with
  `using TestContext = Xunit.TestContext;`; `blazor/` never hits this because
  it runs xunit 2.x, which has no `TestContext` at all. `Microsoft.Data.Sqlite`
  10.0.0 drags in `SQLitePCLRaw.lib.e_sqlite3` 2.1.11, which carries
  **GHSA-2m69-gcr7-jv3q** and emits `NU1903` on every build — pinned instead
  to **`SQLitePCLRaw.lib.e_sqlite3` 2.1.13**, since
  `SQLitePCLRaw.bundle_e_sqlite3` can't fix it (bundle and lib versions are
  decoupled). **Do not reference `Microsoft.Extensions.Hosting` or
  `System.Security.Cryptography.ProtectedData`** — both are already in the
  shared framework for `net10.0-windows` here, and referencing either emits
  `NU1510` (unlike `wpf/`, which *does* reference `Microsoft.Extensions.Hosting`
  explicitly, because it targets the same TFM but does not carry
  `Microsoft.AspNetCore.App` — do not copy that line into this track). The
  test project uses **`Microsoft.NET.Sdk`, not the Razor SDK** — it has no
  `.razor` files of its own, since the components under test live in the
  content library.

  Windows-only, and block `04-desktop-wpf` additionally needs an
  **interactive desktop session**, because WPF is. `Ex055_ClipboardHygiene`
  briefly disturbs the real system clipboard while the suite runs.
- **Telemetry** — 70 rows across five **subject-area blocks** (`01-logging`,
  `02-diagnostics`, `03-otel-sdk`, `04-web-services`, `05-desktop-ops`), not
  100 rows in four difficulty tiers, for the same reason `security/` and
  `Architecture/` depart: "beginner telemetry" is not a meaningful axis. The
  solution is `FeWoLearning.Telemetry.slnx`, three projects sharing the
  `UseSolutions` mechanism with `wpf/`, `blazor/`, `uno/`, `caliburn/`,
  `avalonia/`, `security/` and `Architecture/`. Namespaces are pinned per
  **block**: `FeWoLearning.Telemetry.Exercises.Logging` / `.Diagnostics` /
  `.Otel` / `.WebServices` / `.DesktopOps`.

  **Block 03 is `.Otel` and must never be renamed to `.OpenTelemetry`.** Inside
  a namespace ending in `OpenTelemetry`, a fully qualified
  `OpenTelemetry.Trace.Sampler` binds its leading segment to the enclosing
  namespace and fails `CS0234` — the shadowing trap this file already records
  for `avalonia/` and `caliburn/`, here avoided rather than documented.

  `net10.0-windows` with `UseWPF`, so block `05-desktop-ops` can drill real
  `Dispatcher`, `DispatcherUnhandledException` and `PresentationTraceSources`
  mechanisms. Windows-only as a result. Measured 2026-09-06: `[WpfFact]` from
  `Xunit.StaFact` 3.0.13 gives an STA thread and a live
  `Dispatcher.CurrentDispatcher` **without** an interactive desktop session,
  unlike `caliburn/`, which needs one because it opens a real `Window` — no row
  here is supposed to open one.

  **Pinned to `Xunit.StaFact` 3.0.13 + xunit.v3 3.2.2 +
  `xunit.runner.visualstudio` 3.1.5 + `Microsoft.NET.Test.Sdk` 17.14.1, with no
  `global.json`** — `caliburn/`'s measured VSTest pairing, deliberately *not*
  `security/`'s xunit.v3 4.0.0 + `Microsoft.Testing.Platform` `global.json`,
  which is the combination that exits 5 with zero tests discovered here.
  `OpenTelemetry` and its family sit at **1.18.0**, except
  `OpenTelemetry.Exporter.Prometheus.AspNetCore`, which has **no stable release
  at all** and is pinned at `1.18.0-beta.1`.

  **The recurring bug class is that a telemetry test which asserts the rendered
  result instead of the structure grades nothing.** `$"user {id} failed"`
  renders byte-identically to `"user {UserId} failed", id` and carries no named
  field; "a span exists" is satisfied by automatic instrumentation the learner
  never wrote and by a span with the wrong parent; an exporter in the DI
  container proves nothing without the in-memory exporter's contents; and a
  single `Collect` cannot tell Delta from Cumulative. `telemetry/README.md`'s
  "How a telemetry test lies" is the long form, with all six.

  **Diagnostics state is process-global**, so `tests/_harness/AssemblyInfo.cs`
  carries `[assembly: CollectionBehavior(DisableTestParallelization = true)]`
  *and* `TelemetryContext` resets `Activity.Current`,
  `Activity.DefaultIdFormat`, `Activity.ForceDefaultIdFormat`,
  `Sdk.SetDefaultTextMapPropagator` and `Baggage.Current` per test — its
  snapshot taken in an **explicit static constructor**, never a field
  initializer, for the `beforefieldinit` reason recorded under `caliburn/`.
  Every exercise additionally owns a uniquely named
  `ActivitySource`/`Meter` (`fewolearning.telemetry.exNNN`).

  Four small traps measured while building this out: `FakeLogger`'s accessor is
  `logger.Collector.LatestRecord`, not `logger.Latest`; `FakeLogger` captures
  scopes with no `IncludeScopes` opt-in and does **not** flatten them, so
  `FakeLogRecord.Scopes` holds the raw scope object; **a literal double
  hyphen is illegal in an XML comment**, so a `.csproj` comment naming a CLI
  flag fails `MSB4025` — the rule this file records for `.axaml` applies to
  project files too; and **`UseWPF=true` shortens the implicit-using list** to
  the WindowsDesktop SDK's set, dropping `System.IO` and `System.Net.Http`, so
  `IOException`, `Stream`, `Path` and `HttpClient` need an explicit `using` in
  every file here and the resulting `CS0246` is otherwise baffling. That last
  one applies to `wpf/`, `caliburn/` and `security/` too.

  **`Activity.IsAllDataRequested` is a hint to the caller, not a guard the
  API applies** — measured 2026-09-06, and it corrects a claim repeated all
  over the internet. Under `ActivitySamplingResult.PropagationData` the flag
  is `false` and `SetTag` **still writes**; the tag is on the activity
  afterwards. The cost of ignoring the flag is therefore not a missing tag
  but building detail the listener said it did not want, for an activity an
  SDK downstream discards anyway. Grading consequence: a fact asserting only
  the tag cannot distinguish `PropagationData` from `AllData` — it must
  assert the flag. Also: `using Serilog;` next to
  `using Microsoft.Extensions.Logging;` makes the bare `ILogger` ambiguous
  (`CS0104`), since both namespaces declare one.

  One more, general enough to be worth stating outside this track: **a
  reflection fact asserting that a type *declares* an interface grades
  nothing when the stub already declares it.** Ex008's provider has to declare
  `ISupportExternalScope` for its `SetScopeProvider` member to make sense, so
  such a fact passed against the untouched stub — caught only because the red
  run's `1 passed` was read. An interface list is part of the signature, and
  this file's rule about facts the signature already satisfies covers it.

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
   - `blazor`: no overlay — `dotnet test -p:UseSolutions=true` runs the same
     suite against `solutions/`.
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
| `avalonia/`| 100 / 100 (verified) | — |
| `blazor/` | 100 / 100 (verified) | —         |
| `uno/`    | 100 / 100 (verified) | —         |
| `caliburn/`| 60 / 100 (verified) | 40 |
| `wpf/`    | 35 / 100 (verified) | 65 |
| `MicroServices/`| 30 / 100 (verified) | 70 |
| `security/`| 60 / 60 (verified) | —         |
| `Architecture/`| 100 / 100 (verified) | —         |
| `telemetry/`| 65 / 70 (verified) | 5 |

Every 100-exercise ledger is fully seeded except `avalonia/`, `caliburn/`,
`wpf/` and `MicroServices/`, all four still being built out — see the table above
for exact counts. Nothing else is
"remaining" in the sense of unwritten content; `java/`, `kotlin/`, and
`flutter/` still need their first real compile/test run (see below) before
they can be trusted the way the verified tracks are.

`dotnet/`, `go/`, `vue/`, `python/`, `angular/`, `rust/`, `uno/` and `blazor/`
are content-complete **and verified** (every stub confirmed red, every solution confirmed green,
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
`blazor/` is content-complete: all 100 exercises across all four tiers carry
393 individual test facts; `dotnet test` shows 393 failed/0 passed on the
untouched tree and `dotnet test -p:UseSolutions=true` shows 393 passed/0 failed
(verified 2026-09-05). Two facts go red on an assertion rather than on their
exercise's `NotImplementedException`, both deliberately and both documented at
the fact: ex069's generic type constraint and ex100's `[StreamRendering]`
attribute are properties of the code's *metadata*, which no behaviour can
prove. Every failure but one traces
to its own exercise's `NotImplementedException`; the exception is ex069, whose
subject is a generic *type constraint* that no behaviour can prove (LINQ's
`Min`/`Max` need none), so it is graded by reading the type parameter's
metadata and goes red on that assertion instead. The `exercises/` build itself
carries 12 expected `CS0169`/`CS0414`/`CS0649` warnings for fields that
shape-B stubs declare for the learner to wire up — these are intentionally
left unsuppressed; `solutions/` builds with 0 warnings. See
`blazor/README.md` for the full list and for a sharp edge in ex035: a naive,
unbounded parent-refresh callback hangs the test host rather than failing it.
Two things the intermediate tier added to that README, both easy to get wrong:
`BunitNavigationManager.History` is stack-ordered (`History.First()` is the
*newest* navigation) and has no indexer, and `PersistentComponentState` is not
in bUnit's default services — `AddBunitPersistentComponentState()` registers it
and returns the double ex059/ex060 drive it with (`Persist` /
`TriggerOnPersisting` / `TryTake`). Hand-building that out of
`ComponentStatePersistenceManager` and a fake store also works and is what this
batch tried first; it is ~40 lines of fixture for nothing, and it drags in the
rule that touching `BunitContext.Renderer` counts as the first service
resolution. A third, from ex063/ex065: a negative assertion about async work
("the cancelled load did not write its result") is vacuous until the renderer's
queue has drained, and `await Renderer.Dispatcher.InvokeAsync(() => { })` is
the drain — no sleep needed, because the continuation was queued there first.
A fourth, from ex073/ex074: `<Virtualize>` does render under bUnit, but with no
viewport to measure it falls back to a fixed window, so `ItemSize` does **not**
change how many rows are realised and a pending items provider renders nothing
at all. What is observable is the `ItemsProviderRequest`, `OverscanCount`
widening the window, `Placeholder` filling slots an *under-delivering* provider
left empty, and `ItemSize` scaling the trailing spacer div — see
`blazor/README.md` §7. The same section records two limits that shaped a
catalog row rather than merely a test: .NET 10's `[PersistentState]` attribute
cannot be round-tripped under bUnit (the double's restore snapshot is taken at
registration), and static SSR's "event handlers are ignored" is unobservable
because bUnit's renderer is always interactive — so ex086/ex087 are graded on
what a component controls instead. Two more from the expert tier:
`RenderHandle.Render` runs its fragment synchronously inside a dispatcher turn,
so `ComponentBase`-style render coalescing has nothing to coalesce under bUnit
and ex093 is scoped without it; and the `ASP0006` analyzer rejects any
sequence-number argument that is not an integer literal — even a named
constant, which is worth knowing before writing a `BuildRenderTree` by hand.

`security/` is content-complete and verified end-to-end: 60/60 exercises
across four attack-surface blocks (not tiers — see its own entry in
"Track-specific gotchas" above), 333 test facts total (131 `01-web-aspnet` +
58 `02-web-blazor` + 104 `03-desktop-core` + 37 `04-desktop-wpf`, plus 3
harness canaries). The stub run reports Total: 333, Failed: 329, Passed: 3,
Skipped: 1 (the 3 passing are the harness canaries; the 1 skipped is
Ex060's symbolic-link fact, which needs elevation or Windows Developer Mode);
the solutions run reports Total: 333, Failed: 0, Passed: 332, Skipped: 1.
Both builds emit 0 warnings. Verified via
`dotnet test --solution FeWoLearning.Security.slnx` (red) and the same
command with `-p:UseSolutions=true` (green).

## The `uno/` track

Uno Platform / WinUI, verified end-to-end on 100 / 100 exercises. Same two-library
mechanism as `blazor/`: `exercises/` and `solutions/` compile the same type names
into the same namespaces (`FeWoLearning.Uno.Exercises.<Tier>`) and the xunit project
references exactly one of them, so `dotnet test` is the red run and
`dotnet test -p:UseSolutions=true` the green one.

The unusual part is the test harness. Uno's Skia backend has no headless head, so
`uno/tests/_harness/UnoHeadlessRuntime.cs` installs what a platform head would - the
two `NativeDispatcher` hooks (by reflection, since they are internal), the ICU data
an Uno *head* assembly carries, and an `Application` - from a `[ModuleInitializer]`.
That buys the real thing: compiled XAML, `Measure`/`Arrange` with Skia text metrics,
the binding engine, Fluent default styles and `AutomationPeer`-driven invocation,
with no window. `global.json` pins `Uno.Sdk` and the package versions in the test
project are pinned to match; `HarnessSmokeTests` fails first when a bump breaks the
reflection.

**Read `uno/README.md` before adding an exercise.** It lists what a windowless tree
cannot do - `ItemsControl`/`ListView` never realise items, virtualising layouts
realise one, no input or focus or `Loaded`/`SizeChanged`, `TransformToVisual` returns
the origin, `await CancellationTokenSource.CancelAsync()` overflows the stack - plus
the WinUI members Uno leaves unimplemented. Several catalog rows were re-scoped
around those limits, each with the reason recorded in the commit that did it.

MVUX (`Feed<T>`, `State<T>`, `ListState<T>`, `Command`) sits in rows 064, 065, 092
and 093. Two findings shaped them: `Command.Async` needs the dispatcher that the
`Uno.Extensions.Reactive.WinUI` package supplies, and `await state.Value(ct)` returns
the *previous* value inside a live `SourceContext`. Feeds are therefore taught as
message streams, values are read outside a context, and that lag is documented rather
than asserted. The Reactive package also brings a source generator that errors on any
record in the assembly with an `Id`-shaped member that is not `partial`.

## The `wpf/` track

WPF on .NET 10, verified end-to-end on its complete beginner tier (`01-beginner`
ex001–ex035, 35/100). The solution is `FeWoLearning.Wpf.slnx`. It shares the
`UseSolutions` mechanism with `blazor/`, `uno/`, `avalonia/` and `caliburn/`:
`exercises/` and `solutions/` compile the same type names into the same
namespaces (`FeWoLearning.Wpf.Exercises.<Tier>`) and `tests/` references
exactly one of them via the `UseSolutions` MSBuild property, so `dotnet test`
is the red run and `dotnet test -p:UseSolutions=true` the green one. Its
project count matches `uno/` and `caliburn/` at three, not `blazor/`'s four
(the extra one is `host/`) or `avalonia/`'s four (`gallery/`): `wpf/` has no
fourth runnable project. As with those siblings, `wpf/Directory.Build.props`
redirecting the solutions build's output via `UseArtifactsOutput`/
`ArtifactsPath` is **required, not cosmetic** — without it `exercises/` and
`solutions/` share an `obj/` tree and the build fails `CS0579` on duplicate
generated assembly-info attributes. Stubs throw `NotImplementedException`.

The harness is smaller than `uno/`'s: WPF resolves default control templates
through `SystemResources` with **no `Application` instance needed**, where
`uno/`'s harness has to construct one. It supplies `[WpfFact]`/`[WpfTheory]`
from `Xunit.StaFact` 4.0.23 (an STA thread plus a real
`DispatcherSynchronizationContext`, so `await` resumes on the dispatcher),
`WpfTestContext`'s `Layout(...)`/`Pump(...)` to drain the queue, and an opt-in
`Show(...)` that parks an element in an off-screen window and returns the
`Window` itself, for the few rows needing a real `PresentationSource` —
`Loaded`, keyboard focus, `HwndSource`/`HwndHost` — a capability `uno/`'s
windowless harness could not offer at all. Tests are serialised with
`[assembly: Parallelization(Mode = ParallelMode.None)]` —
`CollectionBehavior(DisableTestParallelization = true)` is
`Obsolete(error: true)` in xunit.v3 4.0.0 and does not compile. Because
`Xunit.StaFact` 4.x depends on `xunit.v3.extensibility.core` 4.0.0, the track
also needs `wpf/global.json` — see the Toolchain-status entry above for why
that file's `Microsoft.Testing.Platform` opt-in is mandatory here. **As of
2026-09-05, that same opt-in makes `dotnet test` exit 5 with zero tests
discovered on this machine** (the test executable itself still runs
correctly) — see the `MicroServices/` toolchain entry above; unfixed, out of
scope for that track.

**Read `wpf/README.md` before adding an exercise.** The recurring bug class is
timing, not capability: bindings update at `DispatcherPriority.DataBind`, and
`CommandManager.InvalidateRequerySuggested()` posts at `Background`, so a test
that mutates and asserts immediately reads the stale value — `Pump()` in
between. `CommandManager.RequerySuggested` also stores handlers weakly, so a
test subscribing with an inline lambda must keep the delegate alive in a
local. `wpf/README.md` documents four ways a WPF test can lie; the sharpest is
specific to this track — a test that observes a dependency property only
through its CLR wrapper cannot prove the logic lives in the property system,
since a hand-rolled clamp in the setter satisfies it just as happily as a
binding, style or animation that writes straight to the store and bypasses
it — so any exercise about metadata, coercion or validation must also write
through `SetValue` and read through `GetValue`. Two deliberate gaps, both
recorded in `wpf/catalog.md`: WinForms interop (row 088 uses
`HwndSource`/`HwndHost` plus P/Invoke rather than pulling `UseWindowsForms`
into both content libraries), and wall-clock performance assertions (rows
076–080 assert that the mechanism fired, never how fast).

## The `Architecture/` track

Application and system architecture on .NET 10 — **100 rows in eight subject-area
blocks**, not four difficulty tiers, for the same reason `security/`
departs: "beginner architecture" is not a meaningful axis, and difficulty rises
*within* each block instead. `01-web` 001–016, `02-desktop` 017–028,
`03-services-data` 029–052, `04-cross-cutting` 053–060, `05-scale` 061–073,
`06-evolution` 074–080, `07-domain` 081–090, `08-runtime` 091–100. The first four
build one process that is correct; `05`
asks what happens when there are fifty of it sharing a finite resource, `06`
what happens when two versions of everything are live while the schema changes
underneath, `07` how the domain itself is modelled — value objects, aggregate
boundaries, state machines, long-running work — and `08` what the process does
when something breaks under it: pools, degradation, timeout budgets, poison
messages, actors, supervision. The folder is capitalised
like `MicroServices/`; deliberate, do not "fix" it.

It is **not** a second `MicroServices/`: that track teaches Aspire and the
deployment topology, this one teaches the patterns inside the process — what an
outbox guarantees, why a cache-aside loader must be counted rather than observed,
what a saga compensates. It is **not** a UI track either: block `02-desktop` is
deliberately UI-framework-free (MVVM composition, navigation, messaging, plugin
loading, offline sync, undo/redo are all testable without a rendering stack), so
unlike `wpf/`, `caliburn/` and `security/`'s block 04, **the whole track is
headless and needs no Windows desktop session**. Everything targets plain
`net10.0`.

Three projects on the `UseSolutions` mechanism shared with `blazor/`, `uno/`,
`wpf/`, `caliburn/`, `avalonia/` and `security/`. `dotnet test` is red,
`-p:UseSolutions=true` green, `-p:Containers=true` additionally runs the thirteen
container-backed rows (032, 036, 037, 038, 039, 046, 047, 050,
065, 066, 074, 092, 094), which are otherwise skipped — every one of those exercises is still fully graded without
Docker by its in-process facts.

**Verified end-to-end on 2026-09-06, complete at 100/100**: 679 test facts.
`dotnet test` gives 662
failed / 4 passed (the harness smoke facts) / 13 skipped (the container rows);
`-p:UseSolutions=true` gives 0 failed / 666 passed / 13 skipped; and
`-p:UseSolutions=true -p:Containers=true` gives 679 passed, 0 skipped, in 20 s
against Docker 29.7.2 with `postgres:17-alpine`, `redis:7-alpine`,
`rabbitmq:4-alpine` and `eclipse-mosquitto:2`. Both builds are clean - every
build warning is `CS9113`/`CS0169`/`CS0649` from `exercises/` stubs; `solutions/`
and `tests/` emit none.

**A fact that HANGS is worse than one that fails**, and blocks 05/06 produced two
of them before the probe caught it: an admission controller or bulkhead that
QUEUES instead of refusing does not throw, it blocks behind a holder the test only
releases further down - which never runs, because the assertion above it has not
returned. The suite stalls and reports nothing. Every "this must be rejected" fact
wraps its call in `WaitAsync`.

**Broker and driver traps, all measured here.** RabbitMQ 4 refuses a transient
non-exclusive queue - `INTERNAL_ERROR - Feature 'transient_nonexcl_queues' is
deprecated` - and reports it as an AMQP connection close rather than a
validation error, so `QueueDeclareAsync` needs `durable: true`; its move to a
dead-letter exchange is asynchronous, so a single `BasicGet` after the reject
races it. MQTTnet 5.2's own broker does **not** publish a client's will in
response to `DisconnectAsync` with
`MqttClientDisconnectOptionsReason.DisconnectWithWillMessage`, with or without
`WithProtocolVersion(V500)` - disposing the client does. A single MQTTnet client
delivers to its `ApplicationMessageReceivedAsync` handler **sequentially**, so a
test cannot force two in-flight requests by blocking in a responder.
`ConcurrentDictionary.GetOrAdd` does not promise to run its factory once, which
makes a single-flight cache built directly on it start two loads under
contention (measured: failing 2 runs in 3) - `Lazy<Task<T>>` fixes it.

**Toolchain, all measured.** xunit.v3 **3.2.2** + `xunit.runner.visualstudio`
**3.1.5** + `Microsoft.NET.Test.Sdk` 17.14.1 on the classic VSTest path, and
**no `global.json`** — copied from `MicroServices/` because xunit.v3 4.0.0 plus a
`Microsoft.Testing.Platform` `global.json` is what makes `dotnet test` exit 5 with
zero tests discovered here. `MQTTnet` 5 **splits its broker into a separate
`MQTTnet.Server` package**, which lives in `tests/` only. Unlike `security/`, this
track does **not** pin `SQLitePCLRaw.lib.e_sqlite3`: `Microsoft.Data.Sqlite`
10.0.11 no longer drags in the 2.1.11 carrying GHSA-2m69-gcr7-jv3q, and both
builds measure 0 warnings without it. `tests/` suppresses `xUnit1051` only;
`exercises/` additionally emits `CS9113` for primary-constructor parameters a stub
does not read yet, left unsuppressed like the other stub warnings.

**MQTT is graded in the DEFAULT run, not behind Docker.** `tests/_harness/MqttBrokerFixture`
starts a real MQTTnet 5 broker in-process on a loopback port, so rows 049–051 get
real protocol frames, real QoS 1 redelivery, real retained messages and a real
last will with no container at all. `SqliteScratch` is a temp **file** database
and deliberately not `:memory:` — the outbox, unit-of-work and locking rows prove
a transaction boundary by opening a *second* connection, and every `:memory:`
connection gets its own private database, which would make those facts pass
vacuously.

**The recurring bug class — read `Architecture/README.md`'s "How an architecture
test lies" before writing a fact.** Architecture tests lie in three ways: the
outcome is reachable *without* the pattern (an outbox test asserting only "the
message arrived" passes a direct publish; a cache test asserting only "the value
came back" passes having no cache); the pattern is asserted but never *exercised*
(an idempotency test that delivers each message once grades nothing); and a test
asserts *structure the runtime does not enforce*, which is why rows 001, 005, 011,
026, 041, 058, 060, 081, 083, 084, 091 and 099 read type or assembly metadata by
reflection. Each of those anchors the metadata assertion to a real call on the
exercise first, because a fact asserting only metadata passes on the stub. Every batch is therefore checked with
**two** probes, not one: the degenerate implementation, and then the *plausible
wrong* one — the pattern implemented earnestly with the wrong mechanism. The
second probe is the one that finds things. Measured in the first batch: Ex004's
status fact asserted 200, which is `DefaultHttpContext`'s own default, so an empty
pipeline passed it — the terminal middleware now sets 202, and **any fact about a
response status must assert a value the default is not**.

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

`security/` is the same kind of exception: its `solutions/` is a real project
referenced by `tests/` under `UseSolutions=true`, the same mechanism
`blazor/`, `avalonia/`, `uno/`, `caliburn/` and `wpf/` use, so it too is
compile-checked and test-run on every green check and cannot drift silently
the way described above.
