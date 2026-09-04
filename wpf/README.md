# WPF Track

Test-driven WPF exercises on .NET 10. Needs the .NET 10 SDK and Windows — nothing
else: no workload, no template, no IDE plugin, and no window ever opens unless an
exercise asks for one.

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

### Three things that bite

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

### `Host(...)` — opt-in, and the only reason a window ever appears

A few things genuinely need a real `PresentationSource`: `Loaded`, keyboard focus,
and `HwndSource`/`HwndHost` interop. `WpfTestContext.Host(element)` parks the element
in a window positioned at `(-10000, -10000)` with `ShowActivated = false`, and
`Dispose` closes it.

Use it only when the exercise is about one of those three things. This is the
capability `uno/`'s windowless harness could not offer at all — its `Loaded`, focus
and input rows had to be re-scoped or dropped, and several catalog rows there say so.

### Tests are serial

`tests/_harness/AssemblyInfo.cs` sets
`[assembly: Parallelization(Mode = ParallelMode.None)]`. `SystemResources`,
the theme dictionaries and `Application.Current` are process-global.

`HarnessSmokeTests` exists for the same reason `uno/`'s does: it asserts STA, that a
`Button` resolves its default template and measures non-zero, that a binding pushes
after a pump, and that `Host(...)` raises `Loaded`. **If those fail, the harness is
broken and every other failure in the run is noise.**

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
