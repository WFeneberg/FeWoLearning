# Caliburn.Micro Track — Design

**Date:** 2026-09-04
**Status:** approved
**Track folder:** `caliburn/`

---

## 1. Purpose

A fourteenth self-contained learning track teaching **Caliburn.Micro 5 MVVM on
WPF**, following the repo's universal exercise pattern: 100 graded exercises,
each a stub that fails red before implementation and passes green once it
matches its reference solution.

**Caliburn.Micro is the subject; WPF is the carrier.** This was chosen
deliberately by the track owner over two alternatives (a pure `wpf/` track, or
two separate `wpf/` + `caliburn/` tracks). The consequence is that WPF topics
appear only where a Caliburn mechanism needs them — a `Button` exists so an
action convention has something to bind to; there are no exercises on
`ControlTemplate` authoring, animations, custom-drawn controls, or
virtualization, because none of those teach Caliburn.

"Beginner" means **Caliburn** beginner, not C# or WPF beginner. `ex001`
implements `INotifyPropertyChanged` by hand, not a `FizzBuzz`. Plain C#
language drills belong to `dotnet/`; Avalonia's ReactiveUI MVVM belongs to
`avalonia/`; Blazor's component model to `blazor/`.

## 2. Toolchain (verified 2026-09-04)

- .NET SDK **10.0.400** (`dotnet --version`).
- nuget.org reachable. `Caliburn.Micro` and `Caliburn.Micro.Core` are already
  in the local package cache at 3.2.0, 4.0.173, 4.0.212 and 5.0.258;
  `Xunit.StaFact` is not cached and restores cleanly.

Pinned versions:

| Package | Version | Note |
|---|---|---|
| `Caliburn.Micro` | 5.0.258 | newest stable; 6.0.x exists only as beta |
| `Xunit.StaFact` | 3.0.13 | supplies `[WpfFact]` / `[WpfTheory]` |
| `xunit.v3` | 3.2.2 | same version `avalonia/` pins |
| `xunit.runner.visualstudio` | 3.1.4 | |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | |

`Caliburn.Micro` 5.0.258 ships a `net9.0-windows7.0` asset, which
`net10.0-windows` consumes without complaint. It brings `Caliburn.Micro.Core`
and `Microsoft.Xaml.Behaviors.Wpf` transitively — the latter matters, see
section 2.1.

### 2.1 The StaFact version trap

`Xunit.StaFact` **4.0.23** is the newest release and must not be used.

It depends on `xunit.v3.extensibility.core` **4.0.0**, and xunit.v3 4.x has
dropped the VSTest bridge that `Microsoft.NET.Test.Sdk` +
`xunit.runner.visualstudio` rely on. On the .NET 10 SDK the build then dies
with:

```
Testing with VSTest target is no longer supported by Microsoft.Testing.Platform
on .NET 10 SDK and later.
```

Opting into the Microsoft.Testing.Platform runner was attempted — both the
`TestingPlatformDotnetTestSupport` MSBuild property and a `dotnet.config`
carrying `[dotnet.test.runner] name = "Microsoft.Testing.Platform"` — and
neither made `dotnet test` succeed from this SDK. Rather than fight it, the
track pins **`Xunit.StaFact` 3.0.13**, whose dependency is
`xunit.v3.extensibility.core` **3.0.0** and which therefore sits exactly on the
xunit.v3 **3.2.2** that `avalonia/` already runs. One xunit generation across
both XAML tracks, on the path this repo has already proven.

### 2.2 Six constraints discovered by the probe

A throwaway probe in the scratchpad — 15 tests spanning the Caliburn core, a
compiled `.xaml` view, the ViewLocator, convention binding and actions — ended
**15/15 green**, but only after six corrections. Each cost a red run to find,
so each is written down here rather than rediscovered per exercise.

| Assumption | What actually holds |
|---|---|
| `IoC` only matters once there is a container to resolve from | **`IoC` must be initialized even with no UI at all.** `Coroutine.BeginExecute` calls `IoC.BuildUp`, so an otherwise pure-core coroutine test throws `InvalidOperationException: IoC is not initialized`. |
| `PlatformProvider` is harmless test-to-test state | **`XamlPlatformProvider` captures `Dispatcher.CurrentDispatcher` in its constructor.** Built on one `[WpfFact]`'s STA thread and left in place, it makes the *next* test's `NotifyOfPropertyChange` marshal onto a dispatcher that no longer pumps — surfacing as `TaskCanceledException` deep inside `PropertyChangedBase`. Every test must reset it. |
| `Screen.Activated` is a normal event | It is an **async** event handler returning `Task`; `(_, _) => flag = true` does not compile. |
| Raising `Loaded` on the root loads the tree | **`FrameworkElement.LoadedEvent` is a *direct* routed event.** Raised on a view it never reaches that view's children; it has to be raised per element. |
| `Measure`/`Arrange` is enough to exercise a view | It is enough for layout and for **guard** evaluation, but **not for firing actions**. Caliburn's actions ride on `Microsoft.Xaml.Behaviors` triggers, which refuse to resolve their source until the element has a real `PresentationSource`. Neither `Measure`/`Arrange`, nor `ApplyTemplate()`, nor hand-raised `Loaded` supplies one — **only a real `Window` does**. |
| An element may be named after any view-model property | **Not `Name`.** `x:Name="Name"` generates a field that hides `FrameworkElement.Name` and the build warns `CS0108`. Since Caliburn's whole point is naming elements after properties, and `Name` is an entirely natural property name, this will be hit. Exercises use `UserName`-style names and the README says why. |

What *does* work with no window at all, and is therefore cheap to test:
default control templates resolve (a `Button` measures greater than zero),
compiled XAML in a class library loads, `XamlReader.Parse` works, the
ViewLocator convention resolves, and `ViewModelBinder` binds by name in both
directions.

### 2.3 The desktop-session requirement

Because actions need a real `PresentationSource` (section 2.2), the test
harness opens a genuine WPF `Window` — parked off-screen at `Left`/`Top`
`-32000`, `Opacity = 0`, `ShowActivated = false`, `ShowInTaskbar = false`, and
closed when the test disposes. Nothing appears on screen and nothing steals
focus, but it is a real window.

**Consequence:** this track requires an interactive desktop session. It will
not run in a service or session-0 context. This is a genuine limitation, it is
not fixable within WPF, and it must be stated in `caliburn/README.md` and
`CLAUDE.md` rather than discovered by whoever first points CI at it.

## 3. Layout

```
caliburn/
  FeWoLearning.Caliburn.slnx
  Directory.Build.props
  .gitignore                          # artifacts-solutions/
  README.md
  catalog.md                          # the 100-row ledger and work queue
  exercises/<tier>/ExNNN_<Slug>.cs    # + .xaml / .xaml.cs where a view is the subject
  solutions/<tier>/ExNNN_<Slug>.cs
  tests/<tier>/ExNNN_<Slug>Tests.cs
  tests/_harness/                     # not exercises, never a catalog row
```

Three projects: `exercises/`, `solutions/`, `tests/`. There is deliberately no
gallery or host project — `avalonia/` and `blazor/` have one because a rendered
page is worth looking at; a Caliburn conductor is not, and a fourth project
would be scaffolding nobody runs.

Tier namespaces are pinned, because `01-beginner` is not a valid C# identifier:

    FeWoLearning.Caliburn.Exercises.Beginner / .Intermediate / .Advanced / .Expert

Both content projects use `RootNamespace` `FeWoLearning.Caliburn.Exercises`;
only `AssemblyName` differs (`…Exercises` / `…Solutions`).

## 4. The red/green mechanism

Identical to `uno/`, `avalonia/` and `blazor/`. `tests/` references **exactly
one** content project:

```xml
<ItemGroup Condition="'$(UseSolutions)' != 'true'">
  <ProjectReference Include="..\exercises\FeWoLearning.Caliburn.Exercises.csproj" />
</ItemGroup>
<ItemGroup Condition="'$(UseSolutions)' == 'true'">
  <ProjectReference Include="..\solutions\FeWoLearning.Caliburn.Solutions.csproj" />
</ItemGroup>
```

Never both — that is what keeps the identical namespaces and type names from
colliding, and it makes the green check one command instead of a scratchpad
overlay.

`Directory.Build.props` redirects the solutions build to its own output tree
via `UseArtifactsOutput` / `ArtifactsPath=artifacts-solutions`. This is
required, not cosmetic: sharing an `obj/` tree between two projects that emit
the same assembly-info attributes fails the build with `CS0579`. `blazor/`
documents that setting these conditionally in the `.csproj` body is read too
late; putting them in `Directory.Build.props` is the fix.

### 4.1 Commands (run from inside `caliburn/`)

| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

## 5. The test harness

`tests/_harness/` holds two base classes. The split is not tidiness — section
2.2 shows the two halves have incompatible `PlatformProvider` requirements.

**`CaliburnCoreContext`** — for exercises with no view. Per test it:

- resets `PlatformProvider.Current` to the inline `DefaultPlatformProvider`,
  undoing whatever a previous `[WpfFact]` installed;
- clears and re-seeds `AssemblySource.Instance`;
- initializes `IoC.GetInstance` / `GetAllInstances` / `BuildUp` from a fresh
  `SimpleContainer`, falling back to `Activator.CreateInstance`.

**`CaliburnViewContext : CaliburnCoreContext, IDisposable`** — for exercises
with a view. Runs only under `[WpfFact]`/`[WpfTheory]`, because it installs
`XamlPlatformProvider`, which must capture the test's own STA dispatcher. Adds:

- `Show(view)` — the off-screen `Window` of section 2.3, tracked and closed on
  dispose. **This is the only way to exercise an action.**
- `Layout(element)` — `Measure`/`Arrange`/`UpdateLayout`, enough for geometry
  and guards.
- `Load(element)` — raises `Loaded` per element, for the narrow cases that need
  the callback but not a window.
- `Pump(priority)` — drains the dispatcher queue before asserting.

The whole assembly runs serially — `[assembly: CollectionBehavior(DisableTestParallelization = true)]`
— because Caliburn's configuration (`IoC`, `PlatformProvider`,
`AssemblySource`, `ViewLocator`) is process-global.

`_harness/` also carries its **own smoke test**, covering `Show`, convention
binding, guard gating and action invocation. It is not an exercise, gets no
`catalog.md` row, and exists so the harness is proven green in the real tree
from the first commit rather than first exercised at ex012.

## 6. Stub failure mode

The repo invariant: a stub **compiles** and fails at **runtime**, so the
learner sees a red test, not a build error.

- **View-model exercises** — the member under test throws
  `NotImplementedException("TODO: ExNNN – …")`. Where the constructor is the
  subject (bootstrapper wiring, container registration), the constructor throws.
- **View exercises** — the stub `.xaml` stays a **valid, compilable document**
  holding only a placeholder, and the code-behind throws after
  `InitializeComponent()`.

A `.xaml` that breaks the XAML compiler is a bug, exactly as a non-compiling
stub is in every other track.

## 7. Content plan

### Tier themes

- **01-beginner (001–035)** — the Caliburn primitives: `PropertyChangedBase`
  and `BindableCollection`; the full `Screen` lifecycle (`OnInitializeAsync`,
  `OnActivateAsync`/`OnDeactivateAsync`, `CanCloseAsync`, `TryCloseAsync`);
  `IViewAware`; the `ViewLocator` convention and `NameTransformer`;
  `ViewModelBinder` name-based property conventions and `ElementConvention`;
  actions with `CanXxx` guards, explicit `Message.Attach`, parameters, special
  values and targets; `SimpleContainer`, the `IoC` facade and
  `BootstrapperBase`; all three `Conductor` shapes.
- **02-intermediate (036–070)** — `IChild`/parent chains; the
  `EventAggregator` (`IHandle<T>`, marshalling, unsubscribe-on-deactivate);
  coroutines (`IResult`, `IResult<T>`, sequences, `CoroutineExecutionContext`,
  cancellation, task adapters); `WindowManager` dialogs and dialog results;
  `ICloseStrategy` and the guard cascade through a conductor; validation via
  `IDataErrorInfo` and `INotifyDataErrorInfo`; item conventions,
  `DataTemplate`/ViewLocator interplay and the `ActiveItem`↔`SelectedItem`
  convention; `Execute`/`PlatformProvider`; `LogManager`; design-time; swapping
  `SimpleContainer` for `Microsoft.Extensions.DependencyInjection`;
  `ActionMessage` customization, custom special values and action filters.
- **03-advanced (071–090)** — a custom `ViewLocator` strategy and custom
  `ViewModelBinder` conventions; `BindingScope` inside `DataTemplate`s; writing
  a `Conductor` from scratch and extending `ConductorBaseWithActiveItem`;
  navigation over a conductor; routing messages to a parent; EventAggregator
  leak avoidance; background work and dispatcher marshalling; testing screens
  and close strategies without a view; a custom `IResult` library;
  `BindableCollection` under load with `IsNotifying` suspension.
- **04-expert (091–100)** — a modular shell over `AssemblySource` with
  dynamically loaded plug-in assemblies; a bootstrapper doing convention-based
  discovery; Caliburn on the generic host; a complete custom convention engine;
  interception around `ActionMessage`; undo/redo over `PropertyChangedBase`; an
  async composite validation pipeline; a multi-screen capstone; and a closing
  comparison of Caliburn's conventions against modern source-generator MVVM.

### Beginner slugs

    001 NotifyByHand              013 ViewLocatorConvention     025 MessageAttachExplicit
    002 PropertyChangedBaseBasics 014 ViewLocatorContext        026 ActionParameters
    003 NotifyOfPropertyChange    015 NameTransformerRule       027 ActionSpecialValues
    004 DependentProperties       016 ViewModelLocator          028 ActionTarget
    005 BindableCollectionBasics  017 ViewModelBinderNames      029 SimpleContainerBasics
    006 BindableCollectionRange   018 BindingConventionTwoWay   030 SimpleContainerInstances
    007 ScreenDisplayName         019 ElementConventionLookup   031 IoCFacade
    008 ScreenInitialize          020 CustomElementConvention   032 BootstrapperConfigure
    009 ScreenActivate            021 ConventionValueConverter  033 ConductorSingleActive
    010 ScreenGuardClose          022 ActionConventionButton    034 ConductorOneActive
    011 ScreenTryClose            023 ActionGuardProperty       035 ConductorAllActive
    012 ViewAwareCallbacks        024 ActionGuardRefresh

`ex001` intentionally precedes `ex002`: the learner writes
`INotifyPropertyChanged` by hand once, then never again — the same ordering
trick `avalonia/` uses at ex008/ex009.

`ex001`–`ex011` are pure core and derive from `CaliburnCoreContext` — the
`Screen` lifecycle needs no view. The view harness first appears at `ex012`
(`ViewAwareCallbacks`), which is why section 5 gives the harness its own smoke
test rather than letting it sit unproven for eleven exercises.

### Non-goals

Not in the catalog, because they cannot be tested honestly here or because they
teach WPF rather than Caliburn:

- Real window management, DPI scaling, multi-monitor placement — the harness's
  window is an off-screen implementation detail, never the subject.
- Actual user input. Tests raise routed events or drive automation peers; no
  exercise asserts on OS-level mouse or keyboard behaviour.
- `ControlTemplate` authoring, animations, custom-drawn controls, virtualization,
  theming, localization. These belong to a WPF track, which this is not.
- Blend design-time tooling beyond `Execute.InDesignMode`.

`caliburn/README.md` states this so nobody later mistakes a green test for
proof of desktop behaviour.

## 8. Test-quality rules

Checked for every exercise. The first two are this track's equivalent of the
missing `pump()` documented for `flutter/` and the
`NotImplementedError`/`RuntimeError` trap documented for `python/`.

- **An action test that never calls `Show` proves nothing.** Without a real
  window the trigger source is unresolved, so the method is never invoked — and
  a test written to assert "nothing happened yet" would pass against a correct
  solution *and* a broken one. Every action exercise goes through `Show`.
- **A guard test must move the guard.** Asserting `IsEnabled` once is satisfied
  by a control that was never bound at all. Assert the disabled state, change
  the view model, `Pump()`, then assert it flipped.
- **A convention exercise's test must not be satisfiable by a hard-coded
  literal in the XAML.** Same rule `avalonia/` and `blazor/` carry: after
  asserting the initial text, change the view model, pump, and assert the view
  followed.
- **A "bind this view" exercise whose test asserts only on the view model
  proves nothing about the view** — it would pass against an empty
  `UserControl`. Assert through the tree: `FindName`, the visual children, the
  attached `ActionMessage`.
- **A named element the test looks up must be named in the stub's TODO.** If
  the solution introduces `x:Name="UserName"` and the stub never mentions it, a
  learner who writes correct XAML fails on a null lookup for a reason the
  exercise is not about.
- **Never assert `Assert.Throws<NotImplementedException>`**, and never assert an
  error the *signature* alone produces. Either passes against the untouched stub.
- Before accepting a red run, ask: **would a naive or wrong implementation also
  pass?** For lifecycle exercises specifically, check that a screen which
  overrides nothing would not satisfy the assertion.
- Confirm each red failure comes from the exercise's own
  `NotImplementedException` — not from a compile error, and above all not from
  `IoC is not initialized` or the dead-dispatcher `TaskCanceledException` of
  section 2.2, both of which look like genuine failures but invalidate the
  exercise.

## 9. Deliberate deviation from the repo convention

`CLAUDE.md` states that `solutions/` is kept out of every build because it
reuses the stubs' names and namespaces. This track, like `avalonia/`,
`blazor/` and `uno/`, keeps `solutions/` **in** the solution as its own
project.

The collision the convention exists to prevent cannot occur here, because no
project references both content projects at once (section 4). The benefit is
that reference solutions are **compile-checked on every build**, which
eliminates for this track the entire "Known gaps" failure class — silent
solution drift, which the 2026-08-03 audit found had already produced five
broken solutions in `vue/` and four defective tests in `go/`.

This deviation must be recorded in `CLAUDE.md` and in `caliburn/README.md` so a
future reader does not "fix" it back.

## 10. Definition of done (first delivery)

1. `caliburn/` scaffolding builds: `dotnet build` clean for all three projects,
   in both the default and the `-p:UseSolutions=true` configuration.
2. `catalog.md` has all 100 rows — 001–005 marked ✅, 006–100 ⬜ — with a
   matching `**Status:**` line.
3. The `_harness` smoke test is green in **both** configurations.
4. `dotnet test` — the five exercise tests **all red**, each failure traced to
   its own `NotImplementedException` and not to any of section 8's false causes.
5. `dotnet test -p:UseSolutions=true` — the same five tests **all green**, with
   the harness smoke test still green alongside.
6. `caliburn/README.md` documents setup, the four commands of section 4.1, the
   StaFact trap of section 2.1, the six constraints of section 2.2, the
   desktop-session requirement of section 2.3, the harness of section 5, the
   deviation of section 9, and the non-goals of section 7.
7. `CLAUDE.md` updated: the track table, the per-track command table, the
   toolchain status, the track-specific gotchas, and the deviation of section 9.
   Root `README.md` gets its track row. `docs/exercise-format.md` gets its
   naming row.
8. Pre-existing documentation drift, verified 2026-09-04: `php/` exists on disk
   but has no row in any `CLAUDE.md` table and none in root `README.md`;
   `uno/` is current in `CLAUDE.md` but missing from root `README.md`. Both are
   **out of scope** here and must not be swept into this track's commits.
9. One commit per batch of five, `caliburn: exNNN-exNNN`, staging explicit
   paths — never `git add -A`. The scaffolding lands in its own preceding
   commit, `caliburn: track scaffolding`.
