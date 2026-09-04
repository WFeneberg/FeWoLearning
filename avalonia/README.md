# Avalonia Track

100 test-driven Avalonia 12 desktop UI exercises, built on **ReactiveUI** MVVM
throughout. "Beginner" means Avalonia beginner, not C# beginner: ex001 is a
`UserControl` with a bound `TextBlock`, not a `FizzBuzz`. Plain C# language
drills belong to the `dotnet/` track; Blazor's component model belongs to
`blazor/`.

Every exercise is a stub that **fails red** before implementation and **passes
green** once it matches its reference solution — the same invariant as every
other track in this repo. All 100 exercises are seeded, but only some have
been confirmed red/green by an actual test run — see `catalog.md`'s status
line for the current, live count of how many.

The MVVM base is ReactiveUI end to end: the beginner tier (001–035) uses it
only declaratively (`ReactiveObject`, `RaiseAndSetIfChanged`,
`ReactiveCommand.Create`); observable *composition* (`WhenAnyValue` at higher
arity, `ToProperty`, `Throttle`, sequencers) starts at ex036, so the Rx
learning curve does not collide with the Avalonia learning curve.

## Setup

Nothing to install beyond the **.NET 10 SDK**. The Avalonia/ReactiveUI package
set (pinned at 12.1.1, see below) restores from NuGet on the first
`dotnet test` — no Avalonia templates, no workloads, no IDE plugin, and no
window ever opens.

## Commands

Run these **from inside `avalonia/`**.

| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |
| Look at it | `dotnet run --project gallery` |
| Look at the answers | `dotnet run --project gallery -p:UseSolutions=true` |

`tests/` and `gallery/` each reference **exactly one** of the two content
projects (`exercises/` or `solutions/`), chosen by the `UseSolutions` MSBuild
property — never both. That is what keeps the identical type names and
namespaces in `exercises/` and `solutions/` from colliding, and it is what
makes the green check a single command instead of a scratchpad overlay (see
"Why `solutions/` is in the build here" below).

Versions are pinned and coherent at **12.1.1** — `Avalonia`,
`Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `Avalonia.Desktop` (gallery
only), `Avalonia.Headless.XUnit` (tests only), `ReactiveUI.Avalonia` — plus
`xunit.v3` 3.2.2. No other versions apply to this track.

## This is not the ReactiveUI you have read about

A throwaway probe run on this machine — a ReactiveUI view model plus a real
`.axaml` view with compiled bindings, exercised by a headless test — needed
seven corrections before it went green, and every one of them contradicts
what current ReactiveUI/Avalonia tutorials say. Read this table before writing
or debugging any exercise in this track; do not trust prior ReactiveUI
knowledge over it.

| What tutorials say | What actually holds here |
|---|---|
| package `Avalonia.ReactiveUI` | **`ReactiveUI.Avalonia`** — renamed; the old package ends at Avalonia 11.3.9 |
| `[AvaloniaTest]` | **`[AvaloniaFact]`** / `[AvaloniaTheory]` |
| xunit 2.x | **xunit.v3** — `Avalonia.Headless.XUnit` 12.1.1 depends on `xunit.v3.extensibility.core` 3.2.2 |
| `System.Reactive.Unit` | **`ReactiveUI.Primitives.RxVoid`** — ReactiveUI 24 has no `Unit` type at all |
| `using System.Reactive.Linq` | **`using ReactiveUI.Primitives`** — operators live in `ReactiveUI.Primitives.LinqExtensions` |
| `IScheduler` | **`ISequencer`** in `ReactiveUI.Primitives.Concurrency` |
| ReactiveUI self-initializes | **`RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build()` is mandatory** |

Two consequences that follow directly:

- **Referencing `xunit` 2.x alongside `Avalonia.Headless.XUnit` is a hard
  error**, not a warning: `FactAttribute` then exists in both `xunit.core`
  2.9.3 and `xunit.v3.core` 3.2.2 and every test file fails with CS0433. This
  is the one place this track cannot copy `blazor/`, which is on xunit 2.9.3 +
  bUnit.
- **ReactiveUI initialization is a track-wide prerequisite.** Without it the
  first `WhenAnyValue` anywhere throws `TypeInitializationException` →
  `InvalidOperationException: ReactiveUI has not been initialized`, which
  makes every exercise red for the wrong reason and silently destroys the
  red/green invariant. `tests/_harness/` carries a `[ModuleInitializer]` that
  runs the builder once; the gallery calls the same builder from its
  `AppBuilder` chain.

## Why `solutions/` is in the build here

`CLAUDE.md` states the repo convention: `solutions/` is kept **out** of every
build because it reuses the stubs' names and namespaces. This track, like
`blazor/`, deliberately deviates and keeps `solutions/` **in** the solution as
its own project.

The collision the convention exists to prevent cannot occur here, because no
project ever references both content projects at once (`tests/` and
`gallery/` each pick exactly one, via `UseSolutions`). The benefit is that
reference solutions are **compile-checked on every build**, which eliminates
for this track the entire "Known gaps" failure class documented at the repo
root — silent solution drift, the kind an audit found had already produced
five broken solutions in `vue/` and four defective tests in `go/`.

This is a deliberate, permanent deviation, not an oversight — do not "fix" it
back to match the rest of the repo.

## Writing tests for this track

- **Always drive layout through `ViewHarness.Show(view, width, height)`** in
  `tests/_harness/`, never bare `Measure`/`Arrange`. Measured fact: a
  `UserControl`'s XAML lives in its `Content`, hosted by a `ContentPresenter`
  from its control template, so without an applied template the control
  itself reports the arranged size while every child stays `0,0,0,0` —
  `ApplyTemplate()` beforehand does not fix it. Putting the control in a
  headless `Window` and calling `Show()` is the only recipe that applies
  templates and drives the full pass; a headless window's client area equals
  its requested `Width`/`Height` exactly, so geometry assertions are
  deterministic.
- **Drain the dispatcher before asserting on scheduled work.** Anything
  posted through the main-thread scheduler has not run yet when the
  assertion executes — call `Dispatcher.UIThread.RunJobs()` first, or the
  test proves only the pre-scheduling state.
- **Assert through the visual tree, not the view model.** A "build this
  XAML" exercise whose test only checks the view model would pass against an
  empty view. Use `FindControl<T>`, the logical/visual children, or the
  applied template.
- **Before accepting a red run, ask: would a naive or wrong implementation
  also pass this test?** If yes, the test is defective. Never assert
  `Assert.Throws<NotImplementedException>`, and never assert an error the
  *signature* alone produces — either passes against the untouched stub. For
  style/template/binding-mode exercises specifically, check the assertion
  could not be satisfied by a hard-coded literal in the view. Also confirm
  each red failure traces to the exercise's own `NotImplementedException` —
  not a compile error, not a missing XAML resource, and above all not the
  uninitialized-ReactiveUI exception above, which looks like a genuine
  failure but invalidates the exercise.

Two more gotchas from Task 1, worth knowing before you hit them yourself:

- A file whose own namespace starts `FeWoLearning.Avalonia.…` cannot
  reference an Avalonia type fully qualified —
  `Avalonia.Media.TextWrapping` fails CS0234, because the leading segment
  binds to the enclosing namespace instead. `using` directives are exempt.
- Every `DataTemplate` needs an explicit `x:DataType`, because compiled
  bindings are this project's default.

## Non-goals

Not in this catalog, because they cannot be tested honestly under a headless
platform: real window management and DPI scaling, GPU rendering paths, native
file dialogs, OS-level clipboard and drag-and-drop hand-off, and platform
handles. Where an exercise touches one of these areas, its test asserts
against Avalonia's headless platform and its own abstractions — which command
ran, what the visual tree and `Bounds` became — never against operating-system
behaviour. A green test in this track proves Avalonia behaviour, never desktop
behaviour.

See [`catalog.md`](catalog.md) for the 100-row progress ledger and the work
queue.
