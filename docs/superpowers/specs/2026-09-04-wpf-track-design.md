# WPF Track — Design

**Date:** 2026-09-04
**Status:** approved
**Track folder:** `wpf/`

---

## 1. Purpose

Another self-contained learning track teaching **WPF on .NET 10**, following
the repo's universal exercise pattern: 100 graded exercises, each a stub that
fails red before implementation and passes green once it matches its reference
solution.

"Beginner" means **WPF** beginner, not C# beginner. `ex001` turns a plain CLR
property into a `DependencyProperty`, not a `FizzBuzz`. Plain C# language drills
belong to `dotnet/`; WinUI's property system belongs to `uno/`.

The content angle is **practice and migration**, chosen deliberately by the
track owner over an API-coverage sweep: the catalog is weighted toward what
actually hurts in a grown WPF solution — DI and the generic host instead of
`App.xaml` singletons, async over the dispatcher, memory leaks through bindings
and event handlers, performance switches, custom controls, and Win32 interop.
Section 5 lays out the tier plan; section 5.1 states the two places where that
angle forces a compromise.

## 2. Toolchain

### 2.1 Verified on this machine (2026-09-04)

- .NET SDK **10.0.400** (`dotnet --version`), with 10.0.303, 9, 8, 7, 6 also
  installed.
- `Microsoft.WindowsDesktop.App` shared runtime **10.0.11** present, and
  `Microsoft.WindowsDesktop.App.Ref` targeting pack present under
  `C:\Program Files\dotnet\packs`. So `net10.0-windows` with `UseWPF=true`
  builds without installing anything.
- nuget.org reachable. `Xunit.StaFact`'s latest stable is **4.0.23**, and its
  `.nuspec` declares a dependency on **`xunit.v3.extensibility.core` 4.0.0** for
  every target framework group — including `net8.0-windows7.0`, which
  `net10.0-windows` satisfies.
- `xunit.v3` **4.0.0** and `xunit.runner.visualstudio` **4.0.0** are both
  published as stable.

The consequence of the StaFact dependency: this is an **xunit.v3 track**, like
`avalonia/`, not an xunit 2.x track like `uno/`. It pins its own versions —
`avalonia/` sits on xunit.v3 3.2.2 and must not be dragged along.

### 2.2 Pinned versions

| Package | Version | Where |
|---|---|---|
| `Xunit.StaFact` | 4.0.23 | tests only |
| `xunit.v3` | 4.0.0 | tests only |
| `xunit.runner.visualstudio` | 4.0.0 | tests only |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | tests only |
| `Microsoft.Extensions.Hosting` | 10.0.0 | `exercises/` + `solutions/` |

Almost everything the track drills ships in `Microsoft.WindowsDesktop.App`, so
the two content libraries carry exactly **one** package reference between them:
`Microsoft.Extensions.Hosting`, for the DI and generic-host rows in tier 2
(036–039). It is the same version `uno/` pins. It is added to both libraries at
scaffolding time rather than later, so the two `.csproj` files never diverge.

Five pinned packages in total. A set this small cannot drift the way the
Avalonia set can.

### 2.3 Platform

Windows-only, because WPF is. Unlike `uno/`, this is not a workaround for a
native asset — there is no `RuntimeIdentifier` and no ICU plumbing.

## 3. Repository layout and the red/green mechanism

```
wpf/
  FeWoLearning.Wpf.slnx
  Directory.Build.props
  catalog.md
  README.md
  exercises/
    FeWoLearning.Wpf.Exercises.csproj
    01-beginner/  02-intermediate/  03-advanced/  04-expert/
    _support/                        # shared fixtures; created on first need, no catalog row
  solutions/
    FeWoLearning.Wpf.Solutions.csproj
    01-beginner/  02-intermediate/  03-advanced/  04-expert/
    _support/                        # identical copy, kept in sync by hand
  tests/
    FeWoLearning.Wpf.Tests.csproj
    01-beginner/  02-intermediate/  03-advanced/  04-expert/
    _harness/                        # the STA/dispatcher harness, not an exercise
```

Three projects, the same shape `uno/`, `blazor/` and `avalonia/` use:

- `exercises/` and `solutions/` are two libraries compiling the **same type
  names** into the **same namespaces** (`RootNamespace` is
  `FeWoLearning.Wpf.Exercises` in both; only `AssemblyName` differs).
- `tests/` references **exactly one** of them, selected by the MSBuild property
  `UseSolutions`. `dotnet test` builds against the stubs (red);
  `dotnet test -p:UseSolutions=true` builds against the reference solutions
  (green). There is no second copy of the tests to keep in sync.

This is the repo-wide "`solutions/` stays out of the build" convention's
deliberate exception, for the third time and for the same reason: the name
collision that convention exists to prevent cannot occur when only one of the
two libraries is ever referenced, and the payoff is that reference solutions are
compile-checked and test-run on every green check instead of drifting silently.

### 3.1 Namespaces

Tier namespaces are pinned per tier:
`FeWoLearning.Wpf.Exercises.Beginner` / `.Intermediate` / `.Advanced` /
`.Expert` — they follow the **tier**, not the `NN-tier` folder name, because
`01-beginner` is not a valid C# identifier. Every `.xaml` needs a fully
qualified `x:Class` for the same reason.

Test namespaces mirror them under `FeWoLearning.Wpf.Tests.<Tier>`.

### 3.2 `Directory.Build.props`

```xml
<PropertyGroup Condition="'$(UseSolutions)' == 'true'">
  <UseArtifactsOutput>true</UseArtifactsOutput>
  <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
</PropertyGroup>
```

Required, not cosmetic. Both libraries emit the same generated assembly-info
attributes, so sharing an `obj/` tree fails the build with `CS0579`
duplicate-attribute errors. This must live in `Directory.Build.props` and not in
the `.csproj` body: `BaseOutputPath`/`BaseIntermediateOutputPath` set inside a
`.csproj` are read after the SDK props import, too late to redirect `obj/`.
`blazor/` and `uno/` each paid for this discovery once.

`artifacts-solutions/` is git-ignored the way the other tracks' is.

### 3.3 Stub shape

Stubs throw `NotImplementedException`, so the library always compiles and an
unfinished exercise fails at *test* time. **A stub that fails to compile is a
bug** — the learner would get a build error instead of a red test.

Each stub carries the standard header comment: `Goal:` / `Drills:` / `Passes:`,
where `Passes:` is the filtered `dotnet test` command and `Drills:` is what
populates the Concepts column of `catalog.md`.

For XAML exercises, `throw` is illegal directly in markup, so the same two
shapes `blazor/` documents apply: shape A throws from a computed member the
markup binds to; shape B throws from a lifecycle method or event handler.

### 3.4 Commands

Run from inside `wpf/`:

| Action | Command |
|---|---|
| Run all tests (stubs → red) | `dotnet test` |
| Run one exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| Verify the reference solutions | `dotnet test -p:UseSolutions=true` |
| Verify one solution | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

### 3.5 No gallery or host project

`avalonia/` has a `gallery/` and `blazor/` a `host/`. `wpf/` deliberately has
neither: every exercise is asserted headless, so a runnable head would be a
fourth project maintained for nobody. It stays addable later without touching
the three that exist.

## 4. The headless test harness

WPF is far more tractable headless than Uno. `Measure`/`Arrange`/`UpdateLayout`,
`ApplyTemplate()`, default theme resolution and the binding engine all work
**with no window at all**. The harness therefore supplies only what WPF
genuinely demands, plus one opt-in escape hatch.

### 4.1 STA and the dispatcher — `Xunit.StaFact`

WPF's `DispatcherObject` requires an STA thread, and xunit runs tests on
thread-pool (MTA) threads. `[WpfFact]` / `[WpfTheory]` from `Xunit.StaFact` give
each test an STA thread with a real `DispatcherSynchronizationContext`, so
`await` resumes on the dispatcher rather than on a pool thread.

That choice removes an entire bug class up front. It is the class that cost
`uno/` the most time (see its README on continuations resuming inside the
dispatcher), and the migration-focused catalog is async- and threading-heavy, so
it would otherwise recur across tiers 2 and 3.

### 4.2 `tests/_harness/WpfTestContext.cs`

The base class every test derives from. Three members:

- **`Layout(FrameworkElement element, Size? available = null)`** — a real
  measure/arrange pass followed by `UpdateLayout()`. Nothing about a
  `FrameworkElement` is trustworthy before it: `DesiredSize` and `ActualWidth`
  are zero and template children do not exist yet.
- **`Pump(DispatcherPriority priority = DispatcherPriority.SystemIdle)`** —
  drains the dispatcher queue down to `priority`. Bindings update at
  `DispatcherPriority.DataBind`; a test that asserts before pumping reads the
  stale value. This is the single most common way a WPF test lies.
- **`Host(FrameworkElement element)`** — **opt-in.** Parks the element in an
  off-screen `Window` (`Left = -10000`, `Top = -10000`,
  `ShowActivated = false`, shown then measured) and returns a disposable that
  closes it. Only for exercises that genuinely need a `PresentationSource`:
  `Loaded`, keyboard focus, and the `HwndSource`/`HwndHost` interop rows.

`Host` is what `uno/` could not offer — Uno's harness has no window, so its
`Loaded`, focus and input rows had to be re-scoped or dropped. Here those
exercises are real. The cost is a real (invisible, unactivated) window per test
that opts in, so it stays opt-in and must be disposed.

### 4.3 `HarnessSmokeTests`

Exists for the same reason `uno/`'s does: if it fails, every other failure in
the run is noise. Four assertions:

1. The test thread's apartment state is STA and `Dispatcher.CurrentDispatcher`
   is non-null.
2. A bare `Button` acquires its default `ControlTemplate` and measures to more
   than 0×0 — this is what proves theme resolution works without an
   `Application.Current`.
3. A `Binding` set up in code pushes its source value to the target after
   `Pump(DispatcherPriority.DataBind)`.
4. `Host(...)` fires `Loaded` on the hosted element.

### 4.4 Serial execution

`tests/_harness/AssemblyInfo.cs` carries
`[assembly: CollectionBehavior(DisableTestParallelization = true)]`.
`SystemResources`, the theme dictionaries and `Application.Current` are
process-global; `Application` in particular can only be constructed once per
process.

### 4.5 The one open question, and how it gets closed

Whether the harness needs an `Application` instance at all is **not yet known**.
WPF resolves default control templates through `SystemResources` without one,
but `pack://application:,,,` URIs and `Application.LoadComponent` may not. If it
turns out to be needed, the harness constructs exactly one on the STA thread and
**never calls `Run()`** — constructing it is what sets `Application.Current`;
running it would block.

This is settled by building the harness and its smoke tests **first**, before a
single exercise exists. Assertion 2 of §4.3 is the probe. If it fails without an
`Application`, the harness gains one and §4.2 grows a note; if it passes, the
harness stays leaner than `uno/`'s.

The same first step also confirms two things this design asserts from package
metadata rather than from a build: that StaFact's `net8.0-windows7.0` asset
applies to `net10.0-windows`, and that `xunit.v3` 4.0.0 sets the test project's
`OutputType` to `Exe` on its own (it does so via its build props; the `.csproj`
should not set it by hand).

## 5. Catalog content

100 rows, tiered the repo way — `01-beginner` 001–035, `02-intermediate`
036–070, `03-advanced` 071–090, `04-expert` 091–100 — with the selection
weighted toward migration and maintenance rather than API coverage.

### 01-beginner (001–035) — what a migration touches daily

| Block | Rows | Subjects |
|---|---|---|
| Property system | 6 | `DependencyProperty.Register`, metadata defaults, `PropertyChangedCallback`, `CoerceValueCallback`, `ValidateValueCallback`, `RegisterAttached` |
| Change notification | 4 | `INotifyPropertyChanged`, a `SetProperty` base with `[CallerMemberName]`, notifying only on real change, computed-property fan-out |
| Binding basics | 8 | `SetBinding`, binding modes, `UpdateSourceTrigger`, `StringFormat`/`FallbackValue`/`TargetNullValue`, `RelativeSource`, `DataContext` inheritance, `IValueConverter`, `MultiBinding` |
| Commands | 3 | `ICommand`, `CanExecute` + `CommandManager.RequerySuggested`, `RoutedCommand`/`CommandBinding` |
| Styles and resources | 6 | `Setter`, `BasedOn`, implicit styles by type, `StaticResource` vs `DynamicResource`, `ResourceDictionary` merging, `DataTrigger` |
| Layout | 4 | the measure/arrange contract, `Grid` star vs auto, `Margin`/`Padding`/alignment, shared size groups |
| Collections | 3 | `ItemsControl` + `DataTemplate`, `ObservableCollection`, `DataTemplateSelector` |
| Routed events | 1 | bubbling vs tunnelling vs `Handled` |

### 02-intermediate (036–070) — MVVM, host, async, views without code-behind

| Block | Rows | Subjects |
|---|---|---|
| Hand-written MVVM | 6 | hardened `ViewModelBase`, view-model-first navigation, a dialog-service abstraction, `INotifyDataErrorInfo`, `IDataErrorInfo`, `ValidationRule` |
| DI and the generic host | 4 | host bootstrap replacing `App.xaml` singletons, a view-model factory, `IOptions`/configuration, `ILogger` |
| async and the dispatcher | 7 | `async void` handler → task-based command, `IProgress<T>`, `Dispatcher.InvokeAsync` priorities, `SynchronizationContext` capture, cancellation, `BindingOperations.EnableCollectionSynchronization`, `BackgroundWorker` → `Task` |
| Collection views | 5 | `CollectionViewSource`, sort/filter/group, `DeferRefresh`, live shaping, `IEditableObject` |
| Templates and behaviors | 8 | `ControlTemplate` + `TemplateBinding`, `VisualStateManager`, an attached behavior replacing code-behind, `Freezable`/`Freeze`, a custom `Panel`, virtualization switches, a custom `MarkupExtension`, templates as resources |
| Cross-cutting | 5 | localization, global exception hooks, settings migration, converter culture, `PresentationTraceSources` for binding failures |

### 03-advanced (071–090) — leaks, performance, threading, controls, interop

| Block | Rows | Subjects |
|---|---|---|
| Leaks | 5 | binding to a non-INPC source, an event handler as GC root, `WeakEventManager`/`PropertyChangedEventManager`, the `DependencyPropertyDescriptor` trap, diagnosing a rooted view model |
| Performance | 5 | container recycling, frozen brushes, `IsAsync`/`PriorityBinding`, `RenderOptions`/`BitmapScalingMode`, the cost of layout invalidation |
| Threading | 3 | cross-thread collection updates, dispatcher priority starvation, a background pipeline feeding the UI |
| Custom controls | 4 | `DefaultStyleKey` + `Themes/Generic.xaml`, template parts, `Adorner`/`AdornerLayer`, `CommandManager` integration |
| Interop and visual layer | 3 | `HwndSource`/`HwndHost` plus a Win32 message hook, `DrawingVisual`/`RenderTargetBitmap`, `DispatcherTimer` vs an animation clock |

### 04-expert (091–100) — architecture of a grown WPF solution

Modular shell/region composition · a DI scope per view · a testable
navigation/dialog service · a binding-diagnostics layer · feature modules from
configuration · a markup-extension DSL · undo/redo over the property store ·
tracing and diagnostics · a migration seam hosting WPF content in a modern
shell · capstone.

### 5.1 Two compromises the practice focus forces

- **Performance rows assert mechanism, never milliseconds.** A test that
  measures elapsed time is noise on any machine under load. So the performance
  block asserts the mechanism instead of the clock: container identity across a
  scroll (076), `IsFrozen` (077), the fallback order a `PriorityBinding` resolves
  (078), the render options actually set (079), the number of measure passes an
  invalidation caused (080). This is the same discipline `uno/`'s README
  documents for virtualization, and it is why those rows are still worth having.
- **No `UseWindowsForms`.** `WindowsFormsHost` would pull WinForms into both
  content libraries for a single row. The interop rows use `HwndSource` /
  `HwndHost` plus P/Invoke instead — the harder and more transferable half.
  WinForms interop is recorded as a deliberate gap in `catalog.md`, the way
  `uno/` records MVUX.

### 5.2 Test-honesty rules carried over

Three failure modes from `CLAUDE.md` and the other tracks' READMEs apply
directly here and belong in `wpf/README.md`:

- A test asserting only what the **signature** produces (wrong arity, wrong call
  style) passes before the stub's body ever runs. Assert on introspected
  metadata, or leave the signature to the learner.
- A test asserting only **rendered geometry** cannot prove which mechanism
  produced it — `RowDefinitions="24,*"` satisfies a test that only checks
  rectangles just as well as the `Auto,*` the exercise was about. Any sizing or
  layout exercise must also assert the definitions.
- A test asserting only **rendered text** can be satisfied by a hard-coded
  literal in the XAML. Every binding exercise must mutate the source afterwards,
  `Pump`, and assert the target followed.
- A test observing a dependency property only through its **CLR wrapper** cannot
  prove the logic lives in the property system. A hand-rolled clamp in the setter
  satisfies it, while a binding, style setter or animation — all of which write
  straight to the store — bypasses that clamp entirely, which is the opposite of
  what the exercise teaches. So any exercise about metadata, coercion or validation
  must also write through `SetValue`, read through `GetValue`, and check that
  `ClearValue` returns to the registered default. Found by review on ex002, which
  passed all eight of its tests against an implementation whose `DependencyProperty`
  registrations would have been purely decorative.

## 6. First delivery

Scope: scaffolding, harness, catalog, README, and **ex001–ex005** verified red
and green. Then the track continues in batches of five, the way `CLAUDE.md`
prescribes.

| # | Slug | Drills |
|---|---|---|
| 001 | `ClrToDependencyProperty` | `DependencyProperty.Register`, metadata default, `GetValue`/`SetValue`, `ClearValue` |
| 002 | `CoerceAndValidate` | `PropertyChangedCallback`, `CoerceValueCallback`, `ValidateValueCallback` — the clamping a legacy setter did by hand |
| 003 | `ObservableViewModelBase` | `INotifyPropertyChanged`, `SetProperty` with `[CallerMemberName]`, no event without a real change |
| 004 | `CodeBehindToBinding` | a manual code-behind copy replaced by a real `Binding`; `Mode`, `UpdateSourceTrigger`, asserted after `Pump(DataBind)` |
| 005 | `RelayCommand` | `ICommand`, `CanExecute`, `CommandManager.RequerySuggested` instead of a `Click` handler |

### 6.1 Order of work

1. Scaffolding (`.slnx`, three `.csproj`, `Directory.Build.props`), harness, and
   `HarnessSmokeTests` — **green before any exercise exists.** This closes §4.5.
2. `catalog.md` with all 100 rows at ⬜ and a `**Status: 0 ✅ / 100 ⬜**` line;
   `README.md` with the command table and the harness's limits.
3. ex001–ex005: stub, test, reference solution.
4. **Red check**, filtered to the five. Every failure must trace to its own
   stub's `NotImplementedException`, and **no test may pass**.
5. **Green check** with `-p:UseSolutions=true` on the same filter, then
   unfiltered. No overlay is needed — the `UseSolutions` switch is the
   mechanism.
6. Flip exactly those five `catalog.md` rows ⬜ → ✅ and update the status line.
7. Repo-level docs (§7).

### 6.2 Repo-level documentation updates

- Root `CLAUDE.md`: a `wpf/` row in the per-track command table; a `wpf/` row in
  the Current-state table (`5 / 100`); a Toolchain-status entry; and a
  Track-specific-gotchas entry covering the three-project `UseSolutions` layout,
  the STA/`[WpfFact]` requirement, `Pump(DataBind)`, and `Host` being opt-in.
- `docs/exercise-format.md`: a `wpf/` row in the naming table, and a mention in
  the Known-gaps section that `wpf/` — like `avalonia/` and `blazor/` — cannot
  drift silently because its `solutions/` is compile-checked.

### 6.3 Commits

Two, staging **explicit paths only**:

1. `wpf: track scaffold + catalog`
2. `wpf: ex001–ex005`

`git add -A` is forbidden here: the working tree holds untracked `uno/`
ex096–ex100 files that must not be swept in.

## 7. Out of scope

- A runnable gallery or host project (§3.5).
- WinForms interop (§5.1).
- Any third-party MVVM framework. The track owner chose the practice focus over
  a toolkit-based one, and hand-written MVVM is what a migration produces before
  it adopts anything.
- Rows 006–100. They are catalogued and planned, not written.
