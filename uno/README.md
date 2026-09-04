# Uno Platform Track

Test-driven Uno Platform / WinUI exercises. Needs the .NET 10 SDK; the `Uno.Sdk`
version is pinned in [`global.json`](global.json) and restored from NuGet on the
first build. No Uno templates, no workloads and no IDE plugin are required — and
no window ever opens.

## Commands

Run these **from inside `uno/`**.

| Action                        | Command                                                        |
|-------------------------------|----------------------------------------------------------------|
| Run all tests (stubs → red)   | `dotnet test`                                                  |
| Run one exercise              | `dotnet test --filter FullyQualifiedName~Ex001_`               |
| Verify the reference solutions | `dotnet test -p:UseSolutions=true`                            |
| Verify one solution           | `dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_` |

## Layout

```
exercises/<tier>/ExNNN_<Slug>.cs      # the stub you implement (+ .xaml/.xaml.cs for markup exercises)
tests/<tier>/ExNNN_<Slug>Tests.cs     # its xunit test — fails until the stub is done
solutions/<tier>/ExNNN_<Slug>.cs      # reference implementation
tests/_harness/                       # the headless Uno runtime, not an exercise
```

`exercises/` and `solutions/` are two libraries that compile the *same* type names
into the *same* namespaces (`FeWoLearning.Uno.Exercises.Beginner` and friends —
tier folders like `01-beginner` are not valid C# identifiers). The test project
references **exactly one** of them: `dotnet test` builds against the stubs,
`dotnet test -p:UseSolutions=true` against the solutions. That is the whole
red/green mechanism — there is no third copy of the tests to keep in sync.

Stubs throw `NotImplementedException`, so the library always compiles and an
unfinished exercise fails at *test* time rather than breaking the build. A stub
that fails to compile is a bug.

See [`catalog.md`](catalog.md) — the 100-row progress ledger and the work queue.

## How the headless runtime works

Uno's Skia backend is normally driven by a platform *head* (Win32, X11, WPF, …)
that owns a native window and a message loop. There is no headless head, so
[`tests/_harness/UnoHeadlessRuntime.cs`](tests/_harness/UnoHeadlessRuntime.cs)
installs the three things a head would install, in a `[ModuleInitializer]`:

1. **Dispatcher hooks.** `NativeDispatcher.HasThreadAccessOverride` returns `true`
   (the test thread *is* the UI thread here) and `DispatchOverride` runs queued work
   inline and synchronously — so a test never pumps a loop or awaits a frame.
2. **ICU.** Uno segments and shapes text through ICU, whose loader reads `icudt.dat`
   out of an embedded resource that only an Uno *head* assembly carries. Hence
   `IsUnoHead=true` and `RuntimeIdentifier=win-x64` in the test project, and the
   `Uno.icu-win` / `HarfBuzzSharp` package references.
3. **An `Application`.** Templated controls resolve their default style off
   `Application.Current`; without one a `Button` has a null `Template` and measures
   to 0×0. `Application.Start` returns immediately because the inline dispatcher
   never blocks.

What you get is the real thing: XAML compiled into the exercise library,
`Measure`/`Arrange` with genuine Skia text metrics, the binding engine,
`DataTemplate`/`ControlTemplate` instantiation, Fluent default styles, and
`AutomationPeer`-driven invocation instead of synthetic pointer input.

XAML handed to `XamlReader.Load` at runtime must declare the `x` namespace
itself - `x:Name` in a template string is an `XmlException` without it.

`UnoTestContext` is the base class every test derives from; its `Layout(...)` helper
runs a real measure/arrange pass. **Nothing about a `FrameworkElement` is
trustworthy before that pass**: `DesiredSize` and `ActualWidth` are zero and
template children do not exist yet. Tests are serial
(`DisableTestParallelization`) because Uno's runtime and the `Application` are
process-global.

### What the harness cannot do

There is no window, so there is no viewport and nothing is ever *loaded*. Five
consequences, all of them found by probing rather than by reading docs:

- **`ItemsControl` and `ListView` never realise their items.** They get their
  default template, but `ItemsPanelRoot` stays null and no item container is
  built, even with an explicit `ItemsPresenter` template. Collection exercises
  use `ItemsRepeater` instead.
- **Virtualising layouts realise one item.** `StackLayout` and friends size their
  realisation window from the effective viewport, which is empty here. An
  `ItemsRepeater` with a *non-virtualising* layout realises everything, so that is
  what the exercises build on; the virtualisation exercises assert on the layout
  protocol (what the context is asked for) rather than on how many children a
  viewport happened to produce.
- **`TransformToVisual` returns the origin.** It needs render state a windowless
  tree does not have. `UnoTestContext.Offset` reads `ActualOffset` instead, which
  is accurate.
- **A few WinUI members behave differently or not at all in Uno.** They compile
  and then throw, or quietly do nothing. Found so far:
  `Microsoft.UI.Xaml.ElementFactoryGetArgs.Data` throws (the
  `Microsoft.UI.Xaml.Controls` twin of that type works);
  `ItemsRepeater.ItemTemplate` rejects any `IElementFactory` that is not a
  `DataTemplate` or Uno's internal shim; `InitializeForContextCore` is never
  called for a `NonVirtualizingLayout`, so `LayoutContext.LayoutState` cannot be
  used and ex071 keys its per-host state by context itself;
  `ApplicationDataContainer.CreateContainer` throws; `x:Load` realises its element
  immediately instead of deferring; `FlowDirection.RightToLeft` does not mirror
  a layout; and `Binding.ConverterLanguage` is ignored - the converter is handed
  the current thread culture instead. When an exercise hits one, keep the real
  API in the signature and put the logic where the tests can reach it (ex046),
  or pick a different subject for that row.
- **Nothing driven by the frame loop or by input happens.** No pointer or
  keyboard events, `Focus()` returns false, and `Loaded`, `Unloaded`,
  `SizeChanged` and `LayoutUpdated` never fire. So the track exercises event
  plumbing through what the property system and the automation peers drive:
  dependency-property callbacks, `RegisterPropertyChangedCallback`,
  `INotifyPropertyChanged`, `INotifyCollectionChanged`, and `Click` via
  `ButtonAutomationPeer.Invoke`. Two catalog rows were re-scoped for this (030
  and 081); do not add an exercise that needs a real input event.

### Two things that bite async exercises

- **`await CancellationTokenSource.CancelAsync()` overflows the stack** and takes
  the test host down with no failing test to point at. A continuation resuming
  inside a cancellation callback re-enters the dispatcher, and the recursion has
  no floor. Use the synchronous `Cancel()`; ex064 and ex067 say so at the call
  site. The inline dispatcher in the harness also guards against re-entering
  Uno's own pump, for the same class of reason.
- **xunit waits for the async work a synchronous test started.** A test that
  leaves a `TaskCompletionSource` unsettled hangs the whole run, not just itself
  (ex049, ex067 both learned this). Settle every gate before the test returns.
  A hung run also leaves a `testhost` process holding the output DLL, so the next
  build fails with MSB3027 until it is killed.

### The fragile spot

The two dispatcher hooks are `internal` in `Uno.UI.Dispatching`, so the harness
reaches them by reflection. This is the one part of the track an Uno upgrade can
break. `global.json` pins `Uno.Sdk`, and the package versions in
[`tests/FeWoLearning.Uno.Tests.csproj`](tests/FeWoLearning.Uno.Tests.csproj) are
pinned to match — bump them together.

`HarnessSmokeTests` exists for exactly this: it asserts that text gets measured and
that default styles resolve. If those two fail, the harness is broken and every
other failure in the run is noise. The harness throws a pointed exception naming
the missing hook when a field is gone.

Windows-only, because of the `win-x64` runtime identifier the ICU native library
needs under `dotnet test`.
