# WPF Track

Test-driven WPF exercises on .NET 10. Needs the .NET 10 SDK and Windows — nothing
else: no workload, no template, no IDE plugin. A window opens only when an exercise
(or the harness's own `Hosted_Element_Raises_Loaded` smoke test) asks for one via
`Show(...)` — see "What the harness cannot do" below.

## Commands

Run these **from inside `wpf/`**.

| Action                         | Command                                                               |
|--------------------------------|-----------------------------------------------------------------------|
| Run all tests (stubs → red)    | `dotnet test`                                                         |
| Run one exercise               | `dotnet test --filter FullyQualifiedName~Ex001_`                      |
| Verify the reference solutions | `dotnet test -p:UseSolutions=true`                                    |
| Verify one solution            | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

## Layout

```
exercises/<tier>/ExNNN_<Slug>.cs      # the stub you implement (+ .xaml/.xaml.cs for markup exercises)
tests/<tier>/ExNNN_<Slug>Tests.cs     # its xunit test — fails until the stub is done
solutions/<tier>/ExNNN_<Slug>.cs      # reference implementation
tests/_harness/                       # the STA/dispatcher harness, not an exercise
```

`exercises/` and `solutions/` are two libraries that compile the *same* type names
into the *same* namespaces (`FeWoLearning.Wpf.Exercises.Beginner` and friends — tier
folders like `01-beginner` are not valid C# identifiers). The test project references
**exactly one** of them: `dotnet test` builds against the stubs,
`dotnet test -p:UseSolutions=true` against the solutions. That is the whole red/green
mechanism — there is no second copy of the tests to keep in sync.

Stubs throw `NotImplementedException`, so the library always compiles and an
unfinished exercise fails at *test* time rather than breaking the build. A stub that
fails to compile is a bug.

See [`catalog.md`](catalog.md) — the 100-row progress ledger and the work queue.

## `global.json` — why `dotnet test` even runs

[`global.json`](global.json) sets `{"test":{"runner":"Microsoft.Testing.Platform"}}`.
Without it, `dotnet test` fails outright on the .NET 10 SDK: `xunit.v3` 4.0.0 pulls in
`Microsoft.Testing.Platform.MSBuild` 2.3.3, which refuses to run under the classic
VSTest bridge ("Testing with VSTest target is no longer supported... opt-in to the new
dotnet test experience"). This is version-specific, not a general xunit.v3 requirement
— the sibling `avalonia/` track runs xunit.v3 3.2.2 with no such opt-in needed.
`xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk` are still referenced despite
this file: they serve the IDE test explorers, a different code path from the CLI.

## How the headless harness works

WPF needs two things a plain xunit run does not give it, and the harness supplies
exactly those two:

1. **An STA thread with a live `Dispatcher`.** `DispatcherObject` requires STA and
   xunit runs tests on MTA pool threads, so every test method is `[WpfFact]` or
   `[WpfTheory]` from `Xunit.StaFact`. Those also install a real
   `DispatcherSynchronizationContext`, so `await` inside a test resumes on the
   dispatcher instead of on a pool thread. That one choice removes the whole bug
   class that cost `uno/` the most time.
2. **A layout pass and a way to drain the queue.** `WpfTestContext` — the base class
   every test derives from — has `Layout(...)` and `Pump(...)`.

What the harness does *not* need is what makes WPF pleasant here compared with
`uno/`: **no window**. `Measure`/`Arrange`/`UpdateLayout`, `ApplyTemplate`, default
theme resolution and the whole binding engine work on a disconnected tree. There is
no `Application` either; WPF resolves default control templates through
`SystemResources` without one.

### Six things that bite

- **Nothing about a `FrameworkElement` is trustworthy before `Layout(...)`.**
  `DesiredSize` and `ActualWidth` are zero and template children do not exist yet.
- **Bindings update at `DispatcherPriority.DataBind`, not synchronously.** A test
  that mutates the source and asserts the target immediately reads the *old* value
  and passes or fails for the wrong reason. Call
  `Pump(DispatcherPriority.DataBind)` — or plain `Pump()`, which drains everything —
  in between. This is the single most common way a WPF test lies.
- **`CommandManager.RequerySuggested` stores handlers weakly and raises
  asynchronously.** A test that subscribes with an inline lambda and never keeps a
  reference to the delegate can have it collected before the event fires; keep the
  delegate in a local. And `InvalidateRequerySuggested()` posts at
  `DispatcherPriority.Background`, so `Pump()` before asserting.
- **`FrameworkPropertyMetadataOptions.Inherits` only actually propagates when the
  property is attached.** A plain `Register` flagged `Inherits` reads back `0.0`/
  `null`/whatever the registered default is on every descendant — even one of the
  *same* owning type as the ancestor that set it — because WPF's inheritance-context
  walk only fires for properties registered via `RegisterAttached`. Row 008
  (`MetadataInheritance`) and any later row that touches inheritance (016
  `DataContextInheritance`, 060 `AttachedBehavior`) depend on this: register the
  inheritable property as attached, the way `FontSize` and `DataContext` are, not as
  an instance property on the class that happens to consume it.
- **A `Binding`'s format culture never comes from `Thread.CurrentCulture`.** It comes
  from `Binding.ConverterCulture`, falling back to the bound element's `Language`
  property, which defaults to a hard-coded `en-US` regardless of the OS locale —
  measured on this de-CH/de-DE machine: `StringFormat="{0:C}"` with no
  `ConverterCulture` renders `$1,234.50`, not CHF, even after forcing
  `Thread.CurrentCulture` to de-CH (which a plain `string.Format` with that culture
  would render as `CHF 1'234.50`). Row 014 (`StringFormatAndFallbacks`) pins
  `target.Language` explicitly anyway, so the row states its assumption instead of
  relying on an invisible default — `ConverterCulture` itself stays row 069's subject.
- **A push/no-push test for `UpdateSourceTrigger` needs real (logical) focus, not
  just a target edit.** `TextBox.Text` defaults to `LostFocus`, not `PropertyChanged`,
  and editing the target alone never raises it — move focus off the target with
  `FocusManager.SetFocusedElement` (works on a windowless tree: no `Show(...)`, no
  input simulation) to actually trigger it. `BindingExpression.IsDirty` can't
  discriminate either — it reads `True` after an edit under unset, `LostFocus` and
  `Explicit` alike, only `PropertyChanged` reads `False` — and `UpdateSource()`
  succeeds under every trigger, so there is no exception-shape check. Row 013 asserts
  both the focus-based behavior and the binding's declared `UpdateSourceTrigger`.

### `Show(...)` — opt-in, and the only reason a window ever appears

A few things genuinely need a real `PresentationSource`: `Loaded`, keyboard focus,
and `HwndSource`/`HwndHost` interop. `WpfTestContext.Show(element)` parks the element
in a window positioned at `(-10000, -10000)` with `ShowActivated = false`, returns
that `Window`, and `Dispose` closes it.

Use it only when the exercise is about one of those three things. This is the
capability `uno/`'s windowless harness could not offer at all — its `Loaded`, focus
and input rows had to be re-scoped or dropped, and several catalog rows there say so.

### Tests are serial

`tests/_harness/AssemblyInfo.cs` sets
`[assembly: Parallelization(Mode = ParallelMode.None)]`. `SystemResources`,
the theme dictionaries and `Application.Current` are process-global.

`HarnessSmokeTests` exists for the same reason `uno/`'s does: it asserts STA, that a
`Button` resolves its default template and measures non-zero, that a binding pushes
after a pump, and that `Show(...)` raises `Loaded`. **If those fail, the harness is
broken and every other failure in the run is noise.**

## What the harness cannot do

A green test here is **not proof of desktop behaviour**. The harness answers one
narrow question — does the WPF mechanism work — and deliberately does not attempt:

- **No `Application`.** Default control templates resolve through `SystemResources`
  without one, which is why the harness never constructs one — and an `Application`
  can only be constructed once per process, so a single stray instance would poison
  every test that ran after it. Anything whose subject is an `Application` member
  (`Application.Current`, `DispatcherUnhandledException`, resource lookup through
  `Application.Resources`, …) cannot be an exercise here; see row 067 for the
  concrete case this ruled out.
- **No time control and no wall-clock assertions.** There is no virtual clock and no
  exercise asserts elapsed time — that is noise on a loaded machine. This is also why
  the performance rows (076–080) assert *that* the mechanism fired (container identity
  across a scroll, `IsFrozen`, the number of measure passes an invalidation caused)
  rather than how fast it fired.
- **`Show(...)` really does open a window.** It is off-screen and unactivated, but it
  is a real `Window.Show()` — the same constraint `caliburn/README.md` states for
  itself. Running the full suite needs a real, interactive desktop session; it will
  not run headless, as a service, or in a session-0/RDP-disconnected context.
- **No keyboard or pointer input simulation.** `Show(...)` gets an element a real
  `PresentationSource`, which is what `Loaded`, keyboard focus and HWND interop need
  — it does not drive an actual mouse or keyboard. No exercise raises synthetic input.
- **`Layout(...)` arranges into the full available rect and defaults to 800×600.**
  `Layout(element, available)` measures and arranges against `available ?? new
  Size(800, 600)` — rows 028–031 (measure/arrange contract, star/auto sizing, margins,
  shared size groups) and row 080 (layout invalidation cost) depend on both the
  default and the ability to override it.
- **`CommandManager` coalescing.** A second `InvalidateRequerySuggested()` call while
  one is still pending is swallowed — it posts at `DispatcherPriority.Background`, so
  a test that invalidates twice without a `Pump()` between them observes only one
  event. Row 020 (`RequerySuggested`) is exactly about this. Also: ex005's
  `RelayCommand` leaves a handler registered on the process-global `CommandManager`
  for the rest of the run, so no later exercise may assert an *exact* count of global
  `RequerySuggested`/`CanExecuteChanged` events — other tests' commands are still
  subscribed.
- **The shared-fixture convention.** ex001 and ex002 each needed the same kind of
  dependency-property reflection helper, and wrote it two different ways — a private
  static property in `Ex001_ClrToDependencyPropertyTests`, a private static method in
  `Ex002_CoerceAndValidateTests` — and row 006 (`RegisterReadOnly`,
  `DependencyPropertyKey`) needed a third shape: a private `DependencyPropertyKey`
  field. `exercises/_support/` is for *content* fixtures both libraries compile, not
  test code — a test-only reflection helper belongs in `tests/_harness/` instead,
  alongside `WpfTestContext`, which is already the shared test surface outside both
  content libraries. Row 006 added `DependencyPropertyReflection` there for exactly
  this, and later rows needing the same kind of reflection should extend it rather
  than write a fourth private copy. (ex001/ex002's two existing idioms were not
  unified — this only records the convention for what comes next.)
- **The "ready to use" convention, and its anti-bypass rule.** An exercise may ship a
  finished collaborator marked "ready to use" (its doc comment may even read slightly
  differently between the stub and the solution — that is deliberate, not drift).
  But *if that collaborator could be edited to do the exercise's job itself, at least
  one test must exercise the subject directly* — not only through the collaborator.
  ex003 originally violated this: `Ex003_MeterViewModel` is "ready to use", and all
  six of its tests reached `SetProperty` only through it, so a learner could inline
  the comparison/assignment/event-raise straight into `Reading`'s and `Label`'s
  setters, leave `SetProperty` itself throwing, and still pass every test. Its tests
  now also call `SetProperty` through a small test-local subclass, directly.

## Writing an exercise without writing a test that lies

Four failure modes, each of which has already shipped in some track of this repo:

- A test asserting only what the **signature** produces — wrong arity, wrong call
  style — passes before the stub's body ever runs. Assert on introspected metadata,
  or leave the signature to the learner.
- A test asserting only **rendered geometry** cannot prove which mechanism produced
  it: `RowDefinitions="24,*"` satisfies a rectangles-only test just as well as the
  `Auto,*` the exercise was about. Sizing and layout exercises must also assert the
  definitions themselves.
- A test asserting only **rendered text** can be satisfied by a hard-coded literal.
  Every binding exercise must mutate the source afterwards, `Pump`, and assert the
  target followed.
- A test that observes a dependency property only through its CLR wrapper cannot
  prove the logic lives in the property system — a hand-rolled clamp in the setter
  satisfies it, while a binding, a style setter or an animation would bypass it
  entirely. Any exercise about metadata, coercion or validation must also write
  through `SetValue` and read through `GetValue`.

## Deliberate gaps

- **WinForms interop.** `WindowsFormsHost` would pull WinForms into both content
  libraries for one row, so ex088 does `HwndSource`/`HwndHost` plus P/Invoke instead.
- **Wall-clock performance.** No exercise asserts elapsed time — that is noise on a
  loaded machine. The performance rows (076–080) assert *that* the mechanism fired
  instead — container identity across a scroll, `IsFrozen`, the number of measure
  passes an invalidation caused.

Windows-only, because WPF is.
