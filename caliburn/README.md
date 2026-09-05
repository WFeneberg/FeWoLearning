# Caliburn.Micro Track

Test-driven **Caliburn.Micro 5** MVVM exercises on **WPF**, targeting .NET 10.
No Caliburn templates, no Visual Studio extension, no scaffolding tool — the
NuGet packages are pinned in the three `.csproj` files and restore on the first
`dotnet build`.

**Caliburn.Micro is the subject; WPF is only the carrier.** A `Button` shows up
in an exercise because an action convention needs something to bind to, not
because the track teaches WPF controls. There are no exercises on
`ControlTemplate` authoring, animations, custom-drawn controls, virtualization,
theming or localization — none of those teach Caliburn. See "What the harness
cannot do" below for the full non-goals list.

## Requirements

- **.NET 10 SDK**, on **Windows** — WPF does not run cross-platform.
- **An interactive desktop session.** The test harness opens a real,
  off-screen, invisible `Window` for every view exercise, because that is the
  only way a Caliburn action actually fires (see "How the harness works"
  below). This is a genuine limitation, not a bug to route around: the track
  **cannot** run headless, as a service, or in a session-0 / RDP-disconnected
  context. Run it at a real, logged-in desktop.

## Commands

Run these **from inside `caliburn/`**.

| Purpose | Command |
|---|---|
| Red — stubs | `dotnet test` |
| Green — reference solutions | `dotnet test -p:UseSolutions=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |
| One exercise, green | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

## Layout

```
caliburn/
  FeWoLearning.Caliburn.slnx
  Directory.Build.props
  README.md
  catalog.md                          # the 100-row ledger and work queue
  exercises/<tier>/ExNNN_<Slug>.cs     # + .xaml / .xaml.cs where a view is the subject
  exercises/_support/TrackMarker.cs    # not an exercise, never a catalog row
  solutions/<tier>/ExNNN_<Slug>.cs
  solutions/_support/TrackMarker.cs    # identical marker, not an exercise
  tests/<tier>/ExNNN_<Slug>Tests.cs
  tests/_harness/                      # not exercises, never a catalog row
```

Three projects: `exercises/`, `solutions/`, `tests/`. There is deliberately no
gallery or host project the way `avalonia/`/`blazor/` have one — a rendered
Caliburn conductor is not worth looking at the way a rendered page is, and a
fourth project here would be scaffolding nobody runs.

`tests/` references **exactly one** content project, controlled by the
`UseSolutions` MSBuild property:

```xml
<ItemGroup Condition="'$(UseSolutions)' != 'true'">
  <ProjectReference Include="..\exercises\FeWoLearning.Caliburn.Exercises.csproj" />
</ItemGroup>
<ItemGroup Condition="'$(UseSolutions)' == 'true'">
  <ProjectReference Include="..\solutions\FeWoLearning.Caliburn.Solutions.csproj" />
</ItemGroup>
```

Never both — that is what lets `exercises/` and `solutions/` reuse the exact
same type names and namespaces without a collision, and it makes the green
check one command instead of a scratchpad overlay. Tier namespaces are pinned
because `01-beginner` is not a valid C# identifier:

```
FeWoLearning.Caliburn.Exercises.Beginner / .Intermediate / .Advanced / .Expert
```

`tests/_harness/` and `_support/` are **not exercises** and neither ever gets
a row in `catalog.md`, but they hold different things. `tests/_harness/`
holds the two base test classes described below plus their own smoke tests.
`_support/` is not under `tests/` at all — it exists identically in
`exercises/` and `solutions/`, and each copy holds exactly one file,
`TrackMarker.cs`: a marker type whose only job is to name that content
assembly so the test harness can register it with `AssemblySource` for
Caliburn's `ViewLocator`, without depending on any individual exercise.

## How the harness works

**The rule that matters most: an action only fires when the view is hosted
with `Show`.** Caliburn's actions ride on `Microsoft.Xaml.Behaviors` triggers,
which refuse to resolve their source until the element has a real
`PresentationSource`. Neither `Measure`/`Arrange`, nor `ApplyTemplate()`, nor a
hand-raised `Loaded` event supplies one — only a real `Window` does. So every
exercise from ex012 on hosts its view before asserting anything about an
action.

`tests/_harness/` holds two base classes. The split is not tidiness — the two
halves need incompatible `PlatformProvider` setups, and mixing them produces
failures that look like the exercise but are actually harness state bleeding
between tests.

**`CaliburnCoreContext`** — for exercises with no view (ex001–ex011). Per test
it:

- resets `PlatformProvider.Current` back to the inline default provider,
  undoing whatever a previous view test installed;
- clears and re-seeds `AssemblySource.Instance`;
- initializes the `IoC` delegates (`GetInstance`, `GetAllInstances`,
  `BuildUp`) from a fresh `SimpleContainer`;
- resets `ViewLocator.NameTransformer` back to Caliburn's 4 built-in rules,
  undoing whatever a previous test's `AddRule` (ex015 onward) left behind.
  The snapshot of those 4 rules is captured once, in an **explicit** static
  constructor — not a plain field initializer. A field initializer alone
  marks the type `beforefieldinit`, which lets the JIT defer running it
  lazily to the first read of that field, and that first read is the
  `foreach` a few lines into the instance constructor, which runs *after*
  that same constructor's own `NameTransformer.Clear()` a line above it.
  Measured on this machine: with only a field initializer, the snapshot
  came back `Count=0` every time, because `Clear()` had already emptied the
  live collection before the lazily-deferred snapshot was ever taken. An
  explicit static constructor removes `beforefieldinit`, which forces the
  CLR to run it before any instance of the class can be constructed at all.

**`CaliburnViewContext : CaliburnCoreContext, IDisposable`** — for exercises
with a view (ex012 onward). Runs only under `[WpfFact]`/`[WpfTheory]`, because
it installs `XamlPlatformProvider`, which must capture the *test's own* STA
dispatcher. It adds four helpers:

- **`Show(view)`** — opens the off-screen `Window` (parked at `Left`/`Top`
  `-32000`, `Opacity = 0`, `ShowActivated = false`, `ShowInTaskbar = false`),
  tracked and closed automatically on dispose. This is the only way to
  exercise an action.
- **`Layout(element)`** — runs `Measure`/`Arrange`/`UpdateLayout`. Enough for
  geometry and for guard evaluation, not for firing an action.
- **`Load(element)`** — raises `Loaded` per element (it is a *direct* routed
  event, so raising it on a root does not reach children), for the narrow
  cases that need the callback but not a real window.
- **`Pump(priority)`** — drains the dispatcher queue before an assertion, so
  marshalled work has actually run.

The whole test assembly runs serially
(`[assembly: CollectionBehavior(DisableTestParallelization = true)]`) because
Caliburn's configuration — `IoC`, `PlatformProvider`, `AssemblySource`, the
`ViewLocator` — is process-global, not per-test.

**Forward risk: the harness does not reset every process-global static.**
`CaliburnCoreContext` resets exactly four globals per test now —
`PlatformProvider.Current`, `AssemblySource.Instance`, the `IoC` delegates, and (since
ex011–ex015) `ViewLocator.NameTransformer`, the first of these globals an exercise
actually mutated. It still does **not** reset `ViewLocator.LocateTypeForModelType` (a
second writable static delegate field on the very type ex013–ex015 already touch, sitting
right next to the one that now is reset), `ViewModelBinder.BindProperties`/`BindActions`,
`MessageBinder.SpecialValues`/`CustomConverters`, `ActionMessage.InvokeAction`/
`ApplyAvailabilityEffect`, `LogManager.GetLog`, or `BindingScope.GetNamedElements` — all
equally static and equally process-global. Nor does it reset `Caliburn.Micro.ViewModelLocator`
— a **separate type** from `ViewLocator`, with its **own** `NameTransformer` static field
(measured: `ReferenceEquals(ViewModelLocator.NameTransformer, ViewLocator.NameTransformer)`
is `false` — a genuinely different object, both starting at 4 rules) plus
`LocateTypeForViewType`/`LocateForViewType`/`LocateForView`, used for view-first
resolution. ex016 (`ViewModelLocator`) only *reads* these — it never calls `AddRule` on
`ViewModelLocator.NameTransformer` — so it needed no harness extension; the first exercise
that *mutates* it still must extend `CaliburnCoreContext` the same way ex015 did for
`ViewLocator`'s, snapshotting it in an **explicit** static constructor and re-applying it
per test. Do not assume ex015's reset covers it — `ViewModelLocator.NameTransformer` is a
distinct object.

There is no public `ConventionManager.ElementConventions`. The real surface is
`ConventionManager.GetElementConvention(Type)` (read) and
`ConventionManager.AddElementConvention(...)` (write), backed by a **private** static
dictionary with **no public removal method at all** — unlike every static named above, this one cannot be
snapshotted and restored by `CaliburnCoreContext`, because there is nothing to call to undo
an `AddElementConvention`. ex020 (`CustomElementConvention`) is the first exercise to write
to it, and deliberately does **not** attempt a harness reset: it registers a convention for
`Ex020_RatingControl`, a type declared inside that exercise itself and never referenced by
any other exercise's test, so the permanent, unresettable dictionary entry it leaves behind
can never be observed by anything else. This is the pattern any future exercise touching
`ConventionManager`'s convention dictionary should follow — register only for a type you own,
rather than trying to reset a static that has no public way to be reset.

Beyond that, nothing shipped so far touches ex063, ex068–ex073, ex087, ex095 or ex096's
statics, but those will. Because the assembly runs serially with no restore between tests,
the first of those to mutate one of the *resettable* statics above will leak into every
later test in the run unless whoever writes it first extends `CaliburnCoreContext` (or
`CaliburnViewContext`) the same way ex015's `NameTransformer` reset was added: snapshot the
pristine value once, in an **explicit** static constructor, and re-apply it at the start of
every test's instance constructor. Until then, a later exercise failing for no visible
reason of its own is this, not a bug
in that exercise.

`tests/_harness/HarnessSmokeTests.cs` proves the harness itself: `HarnessCoreSmokeTests`
(3 `[Fact]`) exercises the core context with no view at all — including that a
`NameTransformer.AddRule` in one test cannot survive into another — and `HarnessSmokeTests`
(4 `[WpfFact]`) exercises `Show`, convention binding, guard gating and action
invocation end to end. Neither is a catalog exercise — they exist so the
harness is proven green in the real tree from the first commit, rather than
first getting exercised eleven rows later at ex012.

## What the harness cannot do

A green test here is **not proof of desktop behaviour**. The harness answers
one narrow question — does the Caliburn wiring work — and deliberately does
not attempt:

- **Real window management.** The `Show` window is parked off-screen and
  never shown to a user; DPI scaling, multi-monitor placement, and window
  chrome are out of scope.
- **OS-level input.** Tests raise routed events or invoke automation peers
  directly; no exercise drives an actual mouse or keyboard.
- **`ControlTemplate` authoring, animations, custom-drawn controls,
  virtualization, theming, localization.** These teach WPF, not Caliburn —
  they belong to a WPF track, which this is not.
- **Blend design-time tooling**, beyond what `Execute.InDesignMode` reports.

## Traps

| Trap | What actually holds |
|---|---|
| "`IoC` only matters once there is a container to resolve from" | `IoC` must be initialized even with **no UI at all** — `Coroutine.BeginExecute` calls `IoC.BuildUp`, so a pure-core coroutine test throws `InvalidOperationException: IoC is not initialized` if it isn't. |
| "`PlatformProvider` is harmless state left over between tests" | `XamlPlatformProvider` captures `Dispatcher.CurrentDispatcher` in its constructor. Left in place from one `[WpfFact]`, it makes the *next* test's `NotifyOfPropertyChange` marshal onto a dispatcher that no longer pumps — surfacing as a `TaskCanceledException` deep inside `PropertyChangedBase`. Every test resets it. |
| "`Screen.Activated` is a normal event" | It is an **async** event handler returning `Task`; `(_, _) => flag = true` does not compile. |
| "Raising `Loaded` on the root loads the whole tree" | `FrameworkElement.LoadedEvent` is a **direct** routed event — raised on a view it never reaches that view's children, so it must be raised per element. |
| "`Measure`/`Arrange` is enough to exercise a view" | It is enough for layout and for guard evaluation, but **not for firing actions** — those need a real `PresentationSource`, which only a real `Window` supplies (see above). |
| "An element may be named after any view-model property" | Not `Name`. `x:Name="Name"` generates a field that hides `FrameworkElement.Name` and warns `CS0108`. Since Caliburn's whole point is naming elements after properties, and `Name` is an entirely natural property name, exercises use `UserName`-style names instead. |
| ex010's guard test fails red, but not every wrong answer does | `Unsaved_Changes_Genuinely_Awaits_The_Confirmation_Before_Deciding` hands `CanCloseAsync` a `TaskCompletionSource<bool>` that is deliberately never completed until after the test asserts the outer task is still pending. A `CanCloseAsync` written as `ConfirmDiscardAsync().Result` or `.GetAwaiter().GetResult()` blocks synchronously on that same never-completing task - the test **hangs rather than fails**. If an ex010 run stalls instead of going red quickly, this is why; the fix is to `await` the delegate, not block on it. |
| Overriding `Screen.OnInitializeAsync`/`OnActivateAsync` is the right hook | Both are `[Obsolete]` in Caliburn.Micro 5 ("Override OnInitializedAsync" / "Override OnActivatedAsync"); overriding them puts `CS0672` in the build and breaks the zero-warning rule for `solutions/`. Override `OnInitializedAsync`/`OnActivatedAsync` instead — `OnDeactivateAsync` is not obsolete and has no `OnDeactivatedAsync` counterpart. |
| "`Xunit.StaFact` 4.x is just a newer version — bump it" | Do **not** — pin stays at 3.0.13 deliberately, not because 4.x cannot work. `Xunit.StaFact` 4.0.23 depends on `xunit.v3.extensibility.core` 4.0.0, which dropped the VSTest bridge that `Microsoft.NET.Test.Sdk` + `xunit.runner.visualstudio` rely on; on .NET 10 SDK the build dies with `Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET 10 SDK and later.` Neither the `TestingPlatformDotnetTestSupport` MSBuild property nor a `dotnet.config` naming that runner fixed it here. The mechanism that **does** work is a track-root `global.json` containing `{"test":{"runner":"Microsoft.Testing.Platform"}}` — the sibling `wpf/` track runs `Xunit.StaFact` 4.0.23 on xunit.v3 4.0.0 exactly that way. `caliburn/` stays on 3.0.13 to keep the VSTest path and xunit.v3 3.2.2 generation `avalonia/` runs; anyone bumping it must add that `global.json` too. |

Pinned package versions, for reference: `Caliburn.Micro` 5.0.258,
`Xunit.StaFact` 3.0.13, `xunit.v3` 3.2.2, `xunit.runner.visualstudio` 3.1.4,
`Microsoft.NET.Test.Sdk` 17.14.1.

## The stub build is not warning-free — by design

Stubs that throw from a member never raise their event or assign their
backing field, so `exercises/` emits `CS0067`/`CS0649`. These are expected
and deliberately left unsuppressed: silencing them would silence the same
warning class a real unused-field bug produces. `solutions/` builds with
**0 warnings** — a warning there is a finding.

## Why `solutions/` is in the build

The repo-wide convention (see the root `CLAUDE.md`) keeps `solutions/` out of
every track's build, because it deliberately reuses the stubs' exact names and
namespaces. `caliburn/`, like `avalonia/`, `blazor/` and `uno/`, **deviates
from that convention on purpose**: `solutions/` is a real project, wired into
`FeWoLearning.Caliburn.slnx` and compiled on every build.

This is safe here because the name collision the repo-wide convention exists
to prevent cannot actually occur: `tests/` references **exactly one** of
`exercises/`/`solutions/` at a time (see "Layout" above), never both. The
payoff is that reference solutions are **compile-checked on every build**,
which removes the entire "solutions silently drifted from its test" failure
class that the rest of the repo's `solutions/` folders are exposed to — the
2026-08-03 audit found five broken solutions in `vue/` and four defective
tests in `go/` for exactly that reason.

**This is deliberate and permanent. Do not "fix" it back** to match the
repo-wide convention.
