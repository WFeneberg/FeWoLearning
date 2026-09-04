# Avalonia Track — Design

**Date:** 2026-09-04
**Status:** approved
**Track folder:** `avalonia/`

---

## 1. Purpose

A tenth self-contained learning track teaching **Avalonia 12 desktop UI with
ReactiveUI MVVM**, following the repo's universal exercise pattern: 100 graded
exercises, each a stub that fails red before implementation and passes green
once it matches its reference solution.

"Beginner" means **Avalonia** beginner, not C# beginner. `ex001` is a
`UserControl` with a bound `TextBlock`, not a `FizzBuzz`. Plain C# language
drills belong to the `dotnet/` track; Blazor's component model belongs to
`blazor/`.

The MVVM base is **ReactiveUI throughout**, chosen deliberately by the track
owner over CommunityToolkit.Mvvm. The known cost is that the Rx learning curve
mixes with the Avalonia learning curve. Section 6 mitigates this by ordering the
content so the beginner tier uses ReactiveUI only declaratively
(`ReactiveObject`, `RaiseAndSetIfChanged`, `ReactiveCommand.Create`) and defers
observable *composition* (`WhenAnyValue` multi-arity, `Throttle`, `ToProperty`,
schedulers) to the intermediate tier onward.

## 2. Toolchain (verified 2026-09-04)

- .NET SDK **10.0.400** (`dotnet --version`).
- nuget.org reachable; the Avalonia 12.1.1 set is already in the local package
  cache except `Avalonia.Headless.XUnit`, which restores cleanly.

Pinned versions, all coherent at **12.1.1**:

| Package | Version |
|---|---|
| `Avalonia` | 12.1.1 |
| `Avalonia.Themes.Fluent` | 12.1.1 |
| `Avalonia.Fonts.Inter` | 12.1.1 |
| `Avalonia.Desktop` (gallery only) | 12.1.1 |
| `Avalonia.Headless.XUnit` (tests only) | 12.1.1 |
| `ReactiveUI.Avalonia` | 12.1.1 |
| `xunit.v3` | 3.2.2 |
| `xunit.runner.visualstudio` | 3.1.4 |
| `Microsoft.NET.Test.Sdk` | 17.14.1 |

`ReactiveUI.Avalonia` transitively brings `ReactiveUI` 24.1.0,
`ReactiveUI.Core` 24.1.0 and `ReactiveUI.Primitives` 7.1.0.

Avalonia 12.1.2 exists but `ReactiveUI.Avalonia`'s matching 12.x release stops
at 12.1.1, so the whole set is pinned to 12.1.1 to keep one coherent minor.
A `ReactiveUI.Avalonia` **14.7.1** also exists on nuget.org; it is deliberately
not used, because 12.1.1 is the version that matches Avalonia 12.1.x and is the
one this design's probe actually verified.

### 2.1 Seven constraints discovered by the probe

A throwaway probe in the scratchpad — a ReactiveUI view model plus a real
`.axaml` view with compiled bindings, exercised by a headless test — ran **2/2
green**, but only after seven corrections. Each of these contradicts what
current ReactiveUI and Avalonia tutorials say, so each must be written down or
it will be rediscovered the hard way per exercise.

| What tutorials say | What actually holds here |
|---|---|
| package `Avalonia.ReactiveUI` | **`ReactiveUI.Avalonia`** — renamed; the old package ends at Avalonia 11.3.9 |
| `[AvaloniaTest]` | **`[AvaloniaFact]`** / `[AvaloniaTheory]` |
| xunit 2.x | **xunit.v3** — `Avalonia.Headless.XUnit` 12.1.1 depends on `xunit.v3.extensibility.core` 3.2.2 |
| `System.Reactive.Unit` | **`ReactiveUI.Primitives.RxVoid`** — ReactiveUI 24 has no `Unit` type at all |
| `using System.Reactive.Linq` | **`using ReactiveUI.Primitives`** — operators live in `ReactiveUI.Primitives.LinqExtensions` |
| `IScheduler` | **`ISequencer`** in `ReactiveUI.Primitives.Concurrency` |
| ReactiveUI self-initializes | **`RxAppBuilder.CreateReactiveUIBuilder().WithAvalonia().Build()` is mandatory** |

Consequences for the track:

- **Referencing `xunit` 2.x alongside `Avalonia.Headless.XUnit` is a hard
  error**, not a warning: `FactAttribute` then exists in both `xunit.core`
  2.9.3 and `xunit.v3.core` 3.2.2 and every test file fails with CS0433. The
  tests project references `xunit.v3` and nothing from the 2.x line. This is
  the one place the Avalonia track cannot copy `blazor/`, which is on xunit
  2.9.3 + bUnit.
- **ReactiveUI initialization is a track-wide prerequisite.** Without it the
  first `WhenAnyValue` in any exercise throws
  `TypeInitializationException` → `InvalidOperationException: ReactiveUI has not
  been initialized`. Every exercise would then be red for the wrong reason,
  which silently destroys the red/green invariant. The tests project therefore
  carries a `[ModuleInitializer]` that runs the builder once. The gallery calls
  the same builder from its `AppBuilder` chain.
- `RxAppBuilder.ResetForTesting()` exists and is the supported way to get a
  clean ReactiveUI registry, should an advanced exercise need one.

One finding improves the content plan: `ReactiveUI.Core` 24.1.0 ships its own
change sets (`ReactiveChangeSet<T>`, `ReactiveChange<T>`,
`ChangeSetExtensions`, `CollectionChangedExtensions`). The advanced tier can
therefore drill reactive collection pipelines — filtering, sorting, count
projection — with **no** extra DynamicData dependency.

## 3. Project structure

```
avalonia/
  Directory.Build.props                                 # UseSolutions output redirect
  FeWoLearning.Avalonia.slnx                            # .slnx, as in dotnet/ and blazor/
  catalog.md                                            # 100-row ledger = work queue
  README.md
  exercises/  FeWoLearning.Avalonia.Exercises.csproj     # stubs
    01-beginner/ 02-intermediate/ 03-advanced/ 04-expert/
    _support/                                            # shared sample types
  solutions/  FeWoLearning.Avalonia.Solutions.csproj     # same namespaces, own assembly
    01-beginner/ …
  tests/      FeWoLearning.Avalonia.Tests.csproj         # xunit.v3 + Avalonia.Headless.XUnit
    01-beginner/ …
    _harness/                                            # TestAppBuilder, ReactiveUI init
  gallery/    FeWoLearning.Avalonia.Gallery.csproj       # Avalonia.Desktop app
```

Per exercise, three files at mirrored paths, plus a fourth where the result is
visual:

- `exercises/<tier>/ExNNN_<Slug>.axaml` + `.axaml.cs` — the stub (view exercises)
  or `exercises/<tier>/ExNNN_<Slug>.cs` — the stub (view-model-only exercises)
- `tests/<tier>/ExNNN_<Slug>Tests.cs`
- `solutions/<tier>/ExNNN_<Slug>.axaml` + `.axaml.cs` (or `.cs`)
- `gallery/Pages/<Tier>/ExNNN.axaml` — only where the result is visual;
  view-model-only exercises get no page

Namespaces are pinned per tier — `FeWoLearning.Avalonia.Exercises.Beginner`,
`.Intermediate`, `.Advanced`, `.Expert` — and do **not** follow the `NN-tier`
folder names, because a C# identifier cannot start with a digit. Unlike Blazor
there is no `_Imports.razor` equivalent for XAML, so every `.axaml` states its
`x:Class` fully qualified and its code-behind declares the matching namespace.

`gallery/` rather than Blazor's `host/`: this is literally a control gallery,
not a web host.

`Directory.Build.props` redirects the solutions build to `artifacts-solutions/`
via `UseArtifactsOutput`/`ArtifactsPath` when `UseSolutions=true`. This is
required, not cosmetic — the same CS0579 duplicate-attribute failure documented
in `blazor/Directory.Build.props` applies verbatim.

## 4. The red/green mechanism

`tests/` and `gallery/` each reference **exactly one** of the two content
projects, selected by an MSBuild condition:

```xml
<ItemGroup Condition="'$(UseSolutions)' != 'true'">
  <ProjectReference Include="..\exercises\FeWoLearning.Avalonia.Exercises.csproj" />
</ItemGroup>
<ItemGroup Condition="'$(UseSolutions)' == 'true'">
  <ProjectReference Include="..\solutions\FeWoLearning.Avalonia.Solutions.csproj" />
</ItemGroup>
```

Never both — that is what keeps the identical namespaces and type names from
colliding, and it is what makes the green check a single command instead of a
scratchpad overlay.

### 4.1 Commands (run from inside `avalonia/`)

| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |
| Look at it | `dotnet run --project gallery` |
| Look at the answers | `dotnet run --project gallery -p:UseSolutions=true` |

## 5. Stub failure mode

The repo invariant is that a stub **compiles** and fails at **runtime**, so the
learner sees a red test rather than a build error. Applied to two exercise
kinds:

- **View-model exercises** — the method or property getter under test throws
  `NotImplementedException("TODO: ExNNN – …")`. Where the constructor is itself
  the subject (wiring up an OAPH, say), the constructor throws.
- **View exercises** — the stub `.axaml` stays a **valid, compilable document**
  holding only a placeholder (`<TextBlock Text="TODO" />`), and the code-behind
  throws after `InitializeComponent()`. This mirrors Blazor's trick of throwing
  in `OnParametersSet`.

An `.axaml` that breaks the XAML compiler is a bug, exactly as a non-compiling
stub is in every other track: the learner would get a build error instead of a
red test.

## 6. Content plan

### Tier themes

- **01-beginner (001–035)** — Avalonia fundamentals with ReactiveUI used
  declaratively: layout panels, core controls, `DataContext`,
  `{CompiledBinding}` with `x:DataType`, binding modes, `IValueConverter`,
  hand-written `INotifyPropertyChanged` before `ReactiveObject` so the
  mechanism is understood rather than assumed, `ReactiveCommand.Create` with
  `CanExecute`, `DataTemplate`/`ItemsControl`, style selectors and pseudo-
  classes, resources, `UserControl` composition, `StyledProperty`.
- **02-intermediate (036–070)** — where Rx composition deliberately begins:
  `WhenAnyValue` at higher arity, `ObservableAsPropertyHelper`/`ToProperty`,
  `ReactiveCommand.CreateFromTask` with `IsExecuting` and `ThrownExceptions`,
  `Throttle` for search-as-you-type, `ISequencer` and the main-thread
  scheduler, `Interaction` for dialogs, `IViewFor`/`ReactiveUserControl`,
  `RoutingState` navigation, `WhenActivated` and `IActivatableViewModel`,
  `TemplatedControl` with `ControlTheme` and template parts, attached
  properties, `Transitions` and animations, `DataGrid`, `Dispatcher.UIThread`
  and cancellation.
- **03-advanced (071–090)** — custom `Control`s with `Render(DrawingContext)`,
  `MeasureOverride`/`ArrangeOverride`, geometry and brushes, virtualization and
  `ItemsRepeater` layouts, pointer input and gestures, `KeyBinding` and focus
  management, drag & drop, clipboard, ReactiveUI change-set collection
  pipelines (filter/sort/count), theming over FluentTheme with merged resource
  dictionaries, localization and `FlowDirection`.
- **04-expert (091–100)** — whole-application MVVM architecture (routing plus
  DI plus activation), a custom `IViewLocator`, AOT- and trimming-friendly
  binding, a layout panel written from scratch, multi-window and
  `IClassicDesktopStyleApplicationLifetime`, plugin-loaded views, binding
  performance, and rendered-frame capture tests.

### Beginner slugs

    001 HelloView                013 BindingStringFormat     025 ItemsControlTemplate
    002 LayoutStackPanel         014 BindingFallback         026 ObservableCollectionUpdates
    003 LayoutGrid               015 ValueConverter          027 EmptyStateFallback
    004 LayoutGridSpan           016 ReactiveCommandBasics   028 StyleSelectors
    005 LayoutDockPanel          017 CommandCanExecute       029 StyleClasses
    006 AlignmentAndMargin       018 CommandParameter        030 PseudoClasses
    007 LayoutWrapPanel          019 ButtonClickEvent        031 StaticAndDynamicResource
    008 ObservableViewModel      020 CheckBoxBinding         032 UserControlComposition
    009 ReactiveObjectBasics     021 RadioGroupBinding       033 StyledPropertyBasics
    010 CompiledBinding          022 SliderBinding           034 AttachedPropertyUsage
    011 BindingModes             023 ComboBoxSelection       035 ScrollViewerAndSizing
    012 TextBoxTwoWay            024 ListBoxSelection

`ex008` intentionally precedes `ex009`: the learner writes
`INotifyPropertyChanged` by hand once, then never again.

### Non-goals

Not in the catalog, because they cannot be tested honestly under a headless
platform: real window management and DPI scaling, GPU rendering paths, native
file dialogs, OS-level clipboard and drag & drop hand-off, and platform
handles. Where an exercise touches such an area, its test asserts against
Avalonia's headless platform and its own abstractions — which command ran, what
the visual tree and `Bounds` became — never against operating-system behaviour.
`avalonia/README.md` states this so nobody later mistakes a green test for
proof of desktop behaviour.

## 7. Test-quality rules

Checked for every exercise. The first is this track's equivalent of the missing
`pump()` documented for `flutter/` and the `NotImplementedError`/`RuntimeError`
trap documented for `python/`.

- **A headless test that never forces a layout pass asserts on un-arranged
  controls.** `Bounds` is `0,0,0,0` until layout has run, so the assertion
  silently fails for the wrong reason — or worse, silently passes.

  Calling `Measure`/`Arrange` on the control is **not sufficient and was
  measured to be wrong**: a `UserControl`'s XAML lives in its `Content`, hosted
  by a `ContentPresenter` from its control template, so without an applied
  template the control itself reports the arranged size while **every child
  stays `0,0,0,0`**. `ApplyTemplate()` before `Measure`/`Arrange` does not fix
  it either.

  The one verified recipe is to put the control in a `Window` and `Show()` it,
  which applies templates and drives the full pass. A headless `Window`'s
  client area equals its requested `Width`/`Height` exactly, so geometry
  assertions are deterministic. Every view exercise's test therefore goes
  through the shared `Show` helper in `tests/_harness/`.
- **Anything scheduled through the main-thread scheduler has not run yet when
  the assertion executes.** Drain the dispatcher queue with
  `Dispatcher.UIThread.RunJobs()` before asserting, or the test proves only the
  pre-scheduling state.
- **A "build this XAML" exercise whose test asserts only on the view model
  proves nothing about the XAML** — it would pass against an empty view. Assert
  through the visual tree: `FindControl<T>`, the logical/visual children, the
  applied template.
- **Never assert `Assert.Throws<NotImplementedException>`**, and never assert an
  error the *signature* alone produces. Either passes against the untouched
  stub.
- Before accepting a red run, ask: **would a naive or wrong implementation also
  pass this test?** If yes, the test is defective. For style, template, and
  binding-mode exercises specifically, check that the assertion could not be
  satisfied by a hard-coded literal in the view.
- **Rendered geometry cannot prove which sizing mode produced it.** ex003 first
  shipped with a test in which `RowDefinitions="24,*"` rendered bit-for-bit
  identically to `"Auto,*"` at the host size under test, so the exercise's own
  subject went unasserted while the test looked thorough. Whenever an exercise's
  point is a *sizing mode* rather than a *result*, name the panel and assert the
  definitions — `GridLength`'s `IsAuto` / `IsStar` / `IsAbsolute` / `Value` —
  alongside the geometry.
- **A named control the test looks up must be named in the stub's TODO.** If the
  solution introduces `Name="RootGrid"` and the stub never mentions it, a learner
  who writes correct XAML fails on a null lookup, for a reason the exercise is not
  about.
- **Assigning a control the value it already holds is a no-op, not an act.**
  Avalonia's property system suppresses the change notification when the new value
  equals the current one, so the binding never runs and the assertion that follows
  passes without exercising anything. ex015 shipped exactly this: the arrange
  rendered the box to `"32"`, the test then set `Text = "32"`, and `ConvertBack` was
  never invoked — a fake converter passed the suite. Seed the arrange from a value
  whose rendered form differs from every value the test later writes.
- **A round trip through the thing under test proves nothing on its own.**
  `ConvertBack(Convert(c)) == c` holds for any correct implementation *and* for a
  lookup table seeded with that one pair. Drive both directions from inputs the test
  did not itself just produce, and use at least two distinct values per direction so
  no single hard-coded pair satisfies the suite.
- **Prove a discriminator by writing the cheat and running it.** Every defect of this
  class found in this track was found that way and none was found by reading. Before
  accepting an exercise, implement the laziest wrong version you can think of, run
  the real unmodified test against it, and confirm it goes red.
- **The cheat must live where the learner writes.** ex018 shipped a test that a
  `Click`-handler bypass defeated 3/3, even though its author had run a cheat overlay
  first — the overlay mutated the exercise's *given* view model, which the stub marks
  "do not change", so it tested a layer no learner touches. For a view exercise the
  cheat belongs in the `.axaml` and its code-behind, and nowhere else.
- **A view exercise needs one structural assertion, not only behavioural ones.**
  Every behavioural assertion about a command can be satisfied by a code-behind event
  handler that mutates the view model directly. `Assert.Same(vm.SomeCommand,
  button.Command)` — or `Assert.Null(button.Command)` where the exercise's point is
  that there is no command — is what pins the wiring. ex016 and ex019 carried it and
  were immune; ex018 lacked it and was not.
- **Beware a control's own built-in behaviour standing in for the mechanism.** ex021
  drills a converter's `ConvertBack`, but a `RadioButton`'s `GroupName` gives
  mutual exclusivity for free, entirely inside the control and independent of any
  binding. A `Mode=OneWay` binding plus `Click` handlers writing the view model
  directly therefore reproduced every observable effect while `ConvertBack` threw
  unconditionally. When a control does part of the exercise's job by itself, the UI
  test cannot reach the part that matters — **test that unit directly** with a plain
  `[Fact]` against the converter, and assert the exact sentinel (`Assert.Same(
  BindingOperations.DoNothing, result)`), not merely a falsy value.
- **Some mechanisms are not mechanically provable, and that is worth saying out
  loud.** A hand-rolled `SelectionChanged`/`PropertyChanged` sync is behaviourally
  indistinguishable from a declarative `SelectedItem` binding — the cheat is not even
  wrong, just not the lesson. Where no public API separates the two, prefer a
  documented limitation in the stub's `Goal:` and a note in the test file over an
  assertion that would fail a legitimate solution.
- Confirm each red failure comes from the exercise's own
  `NotImplementedException` — not from a compile error, not from a missing XAML
  resource, and above all not from the uninitialized-ReactiveUI exception of
  section 2.1, which looks like a genuine failure but invalidates the exercise.

## 8. Deliberate deviation from the repo convention

`CLAUDE.md` states that `solutions/` is kept out of every build because it
reuses the stubs' names and namespaces. This track, like `blazor/`, keeps
`solutions/` **in** the solution as its own project.

The collision the convention exists to prevent cannot occur here, because no
project references both content projects at once (section 4). The benefit is
that reference solutions are **compile-checked on every build**, which
eliminates for this track the entire "Known gaps" failure class — silent
solution drift, which the 2026-08-03 audit found had already produced five
broken solutions in `vue/` and four defective tests in `go/`.

This deviation must be recorded in `CLAUDE.md` and in `avalonia/README.md` so a
future reader does not "fix" it back.

## 9. Definition of done (first delivery)

1. `avalonia/` scaffolding builds: `dotnet build` clean for all four projects,
   in both the default and the `-p:UseSolutions=true` configuration.
2. `catalog.md` has all 100 rows — 001–010 marked ✅, 011–100 ⬜ — with a
   matching `**Status:**` line.
3. `dotnet test` — 10 exercise tests, **all red**, each failure traced to its
   own `NotImplementedException` and not to any of section 7's false causes.
4. `dotnet test -p:UseSolutions=true` — the same 10 tests, **all green**.
5. The gallery builds in both modes, and every gallery page it actually
   registers is covered by a headless smoke test that constructs it. The
   per-exercise proof is the test suite, not the gallery; if a real windowed
   `dotnet run` is attempted and no display is available, that is reported
   plainly rather than claimed as verified.
6. `avalonia/README.md` documents setup, all six commands of section 4.1, the
   seven constraints of section 2.1, the deviation of section 8, and the
   non-goals of section 6.
7. `CLAUDE.md` updated: the track table, the per-track command table, the
   toolchain status, the track-specific gotchas of section 2.1, and the
   deviation of section 8. Root `README.md` gets its track row.
   `docs/exercise-format.md` gets its naming row.
8. `blazor/` and `php/` are absent from `CLAUDE.md` and root `README.md`. That
   is pre-existing documentation drift, is **out of scope** here, and is not to
   be swept into this track's commits.
9. One commit per batch of five, `avalonia: exNNN-exNNN`, staging explicit
   paths — never `git add -A`.
