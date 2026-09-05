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

Run every command **from inside the track folder**, not the repo root.

For `blazor/`, `uno/`, `caliburn/` and `wpf/`, `dotnet test -p:UseSolutions=true` runs the identical
suite against the reference solutions instead of the stubs. `MicroServices/` supports the same
`-p:UseSolutions=true` flag, plus `-p:Containers=true` to additionally run the container-backed
rows (skipped by default). `dotnet run --project playground -- --exercise exNNN` runs a single
exercise in the Aspire dashboard. `security/` supports the same `-p:UseSolutions=true` flag for
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
  verified as of 2026-09-05: ex001-ex035 (199 exercise test facts, completing
  the beginner tier) are red on the untouched tree — `dotnet test` shows 199
  failed, 7 passed (the 7 harness smoke tests, which pass in both modes) — and
  `dotnet test -p:UseSolutions=true` shows 206 passed, 0 failed. **`wpf/`**'s beginner tier
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
- **`MicroServices/`** is verified as of 2026-09-05 on **Aspire 13.5.3 with
  .NET 10.0.400**, **Docker 29.7.2**, **devcontainer CLI 0.89.0**, and
  **xunit.v3 3.2.2** (`xunit.runner.visualstudio` 3.1.5,
  `Microsoft.NET.Test.Sdk` 17.14.1) pinned on the classic VSTest path:
  `dotnet test` gives 12 exercise facts red, 7 harness facts passed, 1 skipped
  (20 total); `dotnet test -p:UseSolutions=true` gives 19 passed, 1 skipped,
  0 failed; `dotnet test -p:Containers=true` gives 12 red, 8 passed, 0 skipped. `Aspire.Hosting.Elasticsearch` is deliberately pinned at 13.3.0 —
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
| `avalonia/`| 70 / 100 (verified) | 30 |
| `blazor/` | 100 / 100 (verified) | —         |
| `uno/`    | 100 / 100 (verified) | —         |
| `caliburn/`| 35 / 100 (verified) | 65 |
| `wpf/`    | 35 / 100 (verified) | 65 |
| `MicroServices/`| 5 / 100 (verified) | 95 |
| `security/`| 60 / 60 (verified) | —         |

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
"Track-specific gotchas" above), 326 test facts total (127 `01-web-aspnet` +
55 `02-web-blazor` + 104 `03-desktop-core` + 37 `04-desktop-wpf`, plus 3
harness canaries). The stub run reports Total: 326, Failed: 322, Passed: 3,
Skipped: 1 (the 3 passing are the harness canaries; the 1 skipped is
Ex060's symbolic-link fact, which needs elevation or Windows Developer Mode);
the solutions run reports Total: 326, Failed: 0, Passed: 325, Skipped: 1.
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
