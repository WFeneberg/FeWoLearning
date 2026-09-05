# Avalonia Track

A 100-exercise catalog of test-driven Avalonia 12 desktop UI exercises, built
on **ReactiveUI** MVVM throughout. "Beginner" means Avalonia beginner, not C#
beginner: ex001 is a `UserControl` with a bound `TextBlock`, not a
`FizzBuzz`. Plain C# language
drills belong to the `dotnet/` track; Blazor's component model belongs to
`blazor/`.

Every exercise is a stub that **fails red** before implementation and **passes
green** once it matches its reference solution — the same invariant as every
other track in this repo. The exercises written so far were each confirmed
red as a stub and green against its reference solution by a real
`dotnet test` / `dotnet test -p:UseSolutions=true` run; the rest of the
catalog is planned, not yet written — see `catalog.md`'s status line for the
live count of how many exist. The scaffolding, headless test harness, and
gallery already cover all four tiers, so adding the remaining exercises needs
no further plumbing.

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

- **`GallerySmokeTests` enforces an exact exception-message contract.** Every
  gallery-registered stub's thrown message must contain `TODO: Ex<NNN>` with
  that exact capitalisation and a three-digit, zero-padded number (`TODO: Ex011`,
  not `TODO - Ex011` or `TODO: Ex11`) — the smoke test string-matches this
  literally, so a stub that drifts from it fails a test far away from the typo.
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
- **A `MultiBinding` converter is called while its bindings are still
  settling, and the first call carries nothing.** Measured on ex066's own view,
  a three-source `MultiBinding` invoked its `IMultiValueConverter` four times as
  the view loaded: `[UnsetValue, UnsetValue, UnsetValue]`, then the values
  filling in left to right. So any `Convert` written for this track must tolerate
  `AvaloniaProperty.UnsetValue` and wrong types instead of indexing and casting
  blindly — otherwise it throws before the view has finished loading. Do not
  assert a converter's call *count* either; it is an implementation detail of
  how the bindings settle.
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
  This applies inside `tests/` too, whose namespace is
  `FeWoLearning.Avalonia.Tests.…`: `Avalonia.Media.TransformGroup` fails the
  same way there.
- Every `DataTemplate` needs an explicit `x:DataType`, because compiled
  bindings are this project's default.

## The suite must stay serial, and a parallel run lies about it

`tests/_harness/AssemblyInfo.cs` carries
`[assembly: CollectionBehavior(DisableTestParallelization = true)]`. **Do not
remove it.** Every `[AvaloniaFact]` runs on the one headless dispatcher that
`AvaloniaTestApplication` sets up, against a single `Application` for the whole
assembly, and xunit.v3 runs collections in parallel by default — so two test
classes starting at once contend for that dispatcher and the run deadlocks.

The failure mode is what makes this worth a section rather than a comment: the
run does not error, it simply **stops**, and whatever finished before the
deadlock is still reported with a normal-looking summary line. A truncated run
therefore reads as a completed one — only the test count and the missing exit
code give it away. Measured on this machine before the attribute existed: the
beginner tier passed cleanly as two halves of 52 and 59 tests, hung after 4 when
asked for all 111 at once, and a plain `dotnet test` hung after 27 of 225. So
when you check a batch, **read the test count, not just the word `Failed`**.

`uno/` and `caliburn/` carry the same one-line attribute; `wpf/` needs the other
spelling (`[assembly: Parallelization(Mode = ParallelMode.None)]`) because
`DisableTestParallelization` is `Obsolete(error: true)` from xunit.v3 4.0.0 on.

## Animation and transitions: what is assertable, and what is not

Animation **progress** cannot be observed in this harness, and no amount of
cleverness changes that. `AvaloniaHeadlessPlatform.ForceRenderTimerTick(n)`
looks like the animation clock but forces *frames*, not time: a 200 ms
`DoubleTransition` on `Opacity` moved from 1.000 to only 0.990 across 505 forced
ticks, which is exactly the ~2 ms of real time those ticks took. Nor can a
controllable clock be injected — measured by reflection, `IClock`, `ClockBase`,
`Clock` and `IGlobalClock` are all **internal** in Avalonia 12.1.1, and
`Animatable.Clock`'s accessors are internal too. There is no public seam.

So never write a test that waits for an animation to advance. What *is*
deterministic, all measured, is enough to grade ex063–ex065 honestly:

- **A transition defers the value, which is a clean binary discriminator.**
  Setting `Opacity = 0.0` on a plain `Border` reads back `0.000` at once; on a
  `Border` carrying a `DoubleTransition` it reads back `1.000`, because the
  transition owns the property. Give the transition a long duration (5 s) so a
  few milliseconds of real time cannot blur the reading.
- **A style animation is attached and observable immediately.** A `Border`
  matched by a `Style` carrying `Style.Animations` reads
  `GetDiagnostic(Visual.OpacityProperty).Priority == BindingPriority.Animation`
  right after `Show`, and holds a value inside the range its keyframes declare.
  That is a real attachment proof, it needs no clock, and — unlike scanning
  `view.Styles` — it does not care where the markup lives. Require
  `IterationCount="Infinite"`: a finite animation runs out and hands the
  property back, so ownership would otherwise depend on when the test looked.
- **Transitions and animations are structurally inspectable.**
  `transitions.OfType<DoubleTransition>()` yields the instance with
  `Property.Name` and `Duration` readable; an `Animation` exposes `Duration`,
  `IterationCount.IsInfinite`, `PlaybackDirection` and `Children`. One catch:
  `KeyFrame.Setters` is typed as `IAnimationSetter`, whose `Property` and
  `Value` are **not publicly accessible** (CS0122) — cast the items to
  `Avalonia.Styling.Setter`, which is measured to be their concrete type.
- **`Transitions` is itself a styled property**, so `GetDiagnostic(...).Priority`
  separates a `<Setter Property="Transitions">` in a `Style` from a
  `<Border.Transitions>` element nested in the markup. The two defer values
  identically, so without the priority check the row's own subject goes
  ungraded.

Two things specific to transforms:

- **You cannot animate `RenderTransform` itself.** A keyframe
  `<Setter Property="RenderTransform" Value="scale(2.5)" />` throws at
  style-attach time with `InvalidOperationException: No animator registered for
  the property RenderTransform`. Animate a transform *sub*-property instead,
  spelled with its owning type: `Property="RotateTransform.Angle"`. Avalonia
  then installs a `TransformGroup` on the control holding one transform of every
  kind (measured: `ScaleTransform, SkewTransform, RotateTransform,
  TranslateTransform, Rotate3DTransform`) and animates the matching one inside
  it. That group is itself the attachment evidence, since an un-animated control
  has a null `RenderTransform` — note that `RenderTransform`'s own priority
  stays `LocalValue` here, so the `BindingPriority.Animation` trick above does
  **not** transfer to transforms.
- **`RenderTransformOrigin="0.5,0.5"` is not the centre.** It parses as
  `RelativeUnit.Absolute` — half a device pixel from the corner, near enough the
  default to change nothing. Only the percentage spelling, `"50%,50%"`, is
  relative to the control's own size, and it equals `RelativePoint.Center`.
  Assert the `RelativePoint`, not the numbers.

Transform *state*, by contrast, needs no clock at all: `Transform.Value` gives
the `Matrix` directly, and a `ScaleTransform(2, 3)` measures `M11 = 2`,
`M22 = 3`.

## Rendering: what a headless test can and cannot see

Nothing that a `Render` override *draws* is observable here. Three separate
measurements, each ruling out one obvious approach:

- **`DrawingContext` has a private constructor**, so a recording double cannot be
  derived from it.
- **The render data a real context records is entirely internal** —
  `RenderDataDrawingContext`, `CompositionRenderData` and `Visual`'s own
  `CompositionVisual` are all non-public.
- **The headless backend discards draw commands.** This is the nastiest of the
  three, because it looks like it worked: `RenderTargetBitmap.Render` followed by
  `CopyPixels` throws nothing and returns plausible-looking bytes. Rendering a
  solid red 8×8 `Border` produced **22 distinct pixel values** — uninitialized
  noise. Never assert on pixels obtained this way.

`Window.GetLastRenderedFrame()` names the cure in its own exception message:
*"make sure that headless application was initialized with `.UseSkia()` and
disabled `UseHeadlessDrawing` in the `AvaloniaHeadlessPlatformOptions`."*
This track does **not** do that today, and `CaptureRenderedFrame()` returns
`null` under the current options. Turning it on would make real pixel assertions
possible and would close the gap ex071 documents — it is a harness-wide change
(`tests/_harness/TestAppHarness.cs`), so it needs its own pass and a full
re-verification of every existing test, not a drive-by edit inside a batch.

What *is* reliable, all measured:

- **`Render` is called**, and its exceptions propagate — but at
  `Dispatcher.UIThread.RunJobs()`, **not** at `Show()`. A test that shows a
  control and never drains the dispatcher silently misses a throwing `Render`.
  `MeasureOverride`/`ArrangeOverride` are the other way round: they throw
  synchronously inside `Show()`.
- **`InvalidateVisual()` plus `RunJobs()` produces another `Render` call** (1 → 2
  in a spy control). `ForceRenderTimerTick` adds none.
- **Layout is exact.** `MeasureOverride` sees the real constraint (including
  `double.PositiveInfinity`), `DesiredSize` clamps as computed, `ArrangeOverride`
  sees `finalSize`, and children land precisely where they were arranged.
- **`Geometry.Bounds` and `Geometry.GetRenderBounds(pen)` are exact.** A pen
  inflates the bounds by exactly half its thickness on every side — verified at
  thicknesses 1, 4 and 10.
- **`PathGeometry` is inspectable, `StreamGeometry` is not.** A `StreamGeometry`
  is write-only by design: segments go into a sink and cannot be read back, so a
  test can learn nothing about the shape beyond its `Bounds`, which a plain
  rectangle satisfies just as well. Grade a shape as a `PathGeometry` and walk
  `Figures[i].Segments[j]` — `LineSegment.Point` is public. Use `StreamGeometry`
  where you only draw.

Two geometry APIs that **must not** carry an assertion in this harness:

- **`FillContains` is wrong for anything but the simplest convex outline.**
  Sampled on a grid, a *solid* arrow reported its own centre row as hollow and
  its left edge as outside. It also ignores the fill rule entirely: a
  self-intersecting five-point star reported the centre as filled under
  `EvenOdd` and `NonZero` alike, when distinguishing exactly that is the whole
  purpose of the rule. So a fill rule can be graded as the property it is
  (`PathGeometry.FillRule` round-trips), never as a hole in a shape.
- **`StrokeContains` is simply broken here** — it returned `false` for a point
  plainly inside a 10 px stroke down the middle of a horizontal line.

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
