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

### Eighteen things that bite

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
- **A `GC.Collect()`-then-assert test can fail in two opposite ways, and the remedies
  differ.** Row 020's first attempt asserted the *safe* direction — a handler stored
  in a field survives a forced collection, because a rooted object's fields can never
  be collected out from under it — but measured against a deliberately broken
  implementation (an inline lambda with nothing stored anywhere), the forced
  collection simply did not reclaim the orphaned delegate: a **false green**, not a
  flake. Rows 071–075 (leaks, `WeakEventManager`) instead need the *unsafe* direction —
  a `WeakReference` genuinely dead after dropping the last root — where the failure
  mode flips to a **flaky red** on correct code if collection just hasn't happened yet.
  Row 020 replaced its GC probe with a reflection check that the handler lives in a
  delegate-typed field at all: `Dispose()` alone does not prove storage, because
  `CommandManager` compares delegates structurally, so a freshly created method-group
  delegate still unsubscribes correctly even with nothing stored anywhere.
- **A field a stub pre-declares for the learner to assign later can warn even though
  it compiles — and nullability is not the reason.** `CS0414` ("assigned but never
  used") fires when a field's only assignment is a compile-time constant it never
  reads elsewhere (a literal `null`, `null!`, or `7`); a non-constant initializer (a
  parameter, `string.Empty`, a lambda) suppresses it regardless of nullability, and
  omitting the initializer entirely gives `CS0169` instead. Row 020 hit this
  pre-declaring `private EventHandler? _handler;` with a placeholder `= null;`. The
  fix, matching row 006's convention: leave the field out of the stub and describe it
  in the TODO instead — it does not exist to warn about until the learner adds it.
- **`ResourceDictionary` lookup order, confirmed by direct measurement, not assumed:** a
  dictionary's own entries win over anything reachable only through `MergedDictionaries`,
  and among several merged dictionaries colliding on the same key, the one added LAST to
  `MergedDictionaries` wins — swapping two dictionaries' add order swaps the winner too.
  `ResourceDictionary.Keys` enumerates only a dictionary's own entries, never anything from
  a merged one — row 026 uses that distinction to prove an entry was written directly into
  the target rather than into a fresh merged dictionary the lookup happens to still find.
- **A `DataTrigger`/`MultiDataTrigger` condition re-evaluates synchronously**, unlike a
  plain `Binding`'s target update. Measured directly: setting the bound source property and
  reading the trigger's `Setter` value straight back — with no `Pump()` at all — already
  shows the new value. The `DispatcherPriority.DataBind` deferral described above is
  specific to a target-property `Binding` pushing a *value* onto a property; a trigger's
  condition check is not that binding mechanism and fires inline, inside the source's
  `PropertyChanged` handler. Row 027 still calls `Pump()` after each mutation, defensively —
  a later row that depends on this instead being deferred would need its own check.
- **`ColumnDefinition.Width`'s and `RowDefinition.Height`'s own unassigned defaults are
  already `GridLength(1, GridUnitType.Star)`** — measured directly, not `Auto` and not
  zero, and true of both types. A test that only checks a Star column's or row's
  `GridUnitType` cannot tell "explicitly assigned Star" apart from "never touched, still
  the default" for that reason; row 029 assigns a Star factor other than 1 (2) so the
  numeric `Value`, not just the `GridUnitType`, is what proves the assignment actually
  happened. Row 031 (`SharedSizeGroup`) builds `RowDefinitions` next and needs the same
  care whenever a row's or column's intended sizing happens to be plain Star(1).
- **`MeasureOverride`'s `constraint` and `ArrangeOverride`'s `finalSize` are both already
  reduced by `Margin`** before either override ever sees them, and `DesiredSize` adds
  `Margin` back onto whatever `MeasureOverride` returned — but capped at the *original*,
  pre-margin size passed to `Measure(...)`; a `MeasureOverride` that returns more than that
  gets its `DesiredSize` clamped down to what `Measure` was actually given, not merely to
  `constraint`. `ArrangeOverride`'s return value is NOT clamped the same way — whatever it
  returns becomes `RenderSize` verbatim, even if that is larger than `finalSize`. Row 028
  stays inside the non-clamped range on purpose to keep the base contract legible; row 062
  (`CustomPanel`) and row 080 (layout invalidation cost) are where the clamping edge and the
  Measure/Arrange asymmetry actually start to matter.
- **`IsInitialized` flips through `AddLogicalChild` - acquiring a logical child initializes
  BOTH the acquirer and the child - not through "any property set", and default Style/
  Template resolution is gated on exactly that flag.** This is the single most consequential
  harness fact found while building rows 031-034 - it was expected to be a virtualization
  problem (the way `uno/`'s `ItemsControl`/`ListView` never realise their items at all, see
  that track's README) and turned out to be something else entirely, one level earlier.
  Measured directly, all of these flip `IsInitialized` on BOTH ends, parent and child:
  `Panel.Children.Add(child)` (any `Panel`), `Border.Child = child`, `TextBlock.Inlines.Add`,
  `Grid.RowDefinitions.Add(...)`, and `ContentControl.Content = value` for ANY value, not just
  a string or a `FrameworkElement` - a boxed `int` (`Content = 42`) flips it too. Measured
  NOT to flip it: `ItemsControl.Items.Add(...)` and `ItemsControl.ItemsSource = ...` alone -
  an `ItemsControl`'s items are not logical children of the `ItemsControl` itself (they only
  become logical children of their generated containers, which do not exist without the flip
  already having happened, so this is not circular help). A childless element with no
  `Content` ever set (a bare `new Border()` with only `Width` assigned, say) stays
  `IsInitialized == false` forever - neither `Layout(...)` nor `Pump()` ever flips it either.

  With `IsInitialized` false, a `Control`'s *unset* default Style/Template never resolves:
  `Button.Template` stays null and `DesiredSize` stays `(0,0)` after `Layout(...)`;
  `ItemsControl.ItemContainerGenerator.Status` never leaves `NotStarted`, so
  `ContainerFromItem` returns `null` for every item forever - no exception anywhere, the tree
  just silently behaves as still mid-construction. `HarnessSmokeTests`' own `Button` test has
  been passing since this track's very first exercise for an incidental reason, not a
  deliberate one: it sets `Content = "Measure me"`, which is exactly the logical-child
  acquisition above - a bare `new Button()` with nothing set stays uninitialized and its
  `Template` stays null even after `Layout(...)`. That smoke test now asserts `IsInitialized`
  directly, and a fifth smoke test proves the *un*-initialized case fails the way described
  here (see `HarnessSmokeTests` below).

  Rows 001-030 never depended on the flip either way: a `Panel` holding a child (`Grid` in
  row 029, say) IS initialized by the rule above, it simply has no `ControlTemplate` to
  resolve in the first place (`Panel`, not `Control`); row 027 sets `Style` explicitly,
  bypassing default-style resolution entirely regardless of `IsInitialized`. Rows 031-034 are
  the first rows in this track to depend on an *unset* default Style/Template actually
  resolving - and of those, only 032-034 actually need the fix. Row 031's `Grid`s each
  already acquire a `RowDefinition` and a child `Border`, which - per the rule above -
  already flips `IsInitialized` with no extra call needed (confirmed directly: deleting
  `CompleteInitialization` from row 031's tests changes nothing, all three still pass).
  `ItemsControl` in rows 032-034 has no such incidental path - `ItemsSource`/`Items.Add` do
  not flip it, and it has no `Content` property to assign - so it genuinely needs the
  explicit fix, factored into `WpfTestContext.CompleteInitialization(element)`: one
  `BeginInit()`/`EndInit()` call on the root of an already-built tree is enough, because
  `EndInit()` reaches every descendant already attached under it at the time it is called
  (measured directly), regardless of whether `BeginInit()` precedes construction or is called
  together with `EndInit()` only at the very end. `Show(...)` also works, but for the same
  reason as everything above, not because of `PresentationSource` itself: `Window.Content =
  element` is itself a `Content` assignment - a logical-child acquisition - so it flips
  `IsInitialized` the same way `Border.Child = ...` does; it also opens a window, so
  `CompleteInitialization` is what rows 032-034 use instead.

  Verified forecast for the next tier: a root `ListBox` with two items reaches
  `ContainersGenerated` with BOTH containers realized after `CompleteInitialization` +
  `Layout(...)` - `VirtualizingStackPanel` (the default panel for `ListBox`/`ListView`,
  unlike `ItemsControl`'s plain, non-virtualizing `StackPanel`) does not reproduce `uno/`'s
  one-item realization limit here. This is a live trap for the `02-intermediate` tier's five
  `CollectionViewSourceBasics`/`SortAndGroup`/`FilterPredicate`/`DeferRefresh`/
  `EditableObjectTransactions` rows (053-057) and any later row driving a `ListBox`/
  `ComboBox`/other `ItemsControl`-derived control's default appearance - anything built by
  plain code with no logical child and no `Content` needs `CompleteInitialization(...)`
  before its default Style/Template can be trusted.
- **A definition inside a shared size group is measured as `Auto`, regardless of its own
  `GridUnitType` - `SharedSizeGroup` on a `Star`-sized definition is not "broken", it is
  simply never measured as `Star` at all.** Measured directly (building row 031): two
  `Star`-sized rows tied together by the same `SharedSizeGroup` name, given real content of
  height 20 and 80, equalize to `(80, 80)` - identical to the `Auto` case - because the
  shared-size negotiation measures every participating definition as `Auto` and discards its
  `Star` factor entirely: two rows explicitly given DIFFERENT factors (`1*` and `3*`), with
  content heights 10 and 40, still equalize to `(40, 40)`, not anything those factors would
  predict. A `Star` row with no children at all (no natural size to fall back to) measures
  `(0, 0)` under the same grouping - an earlier measurement of exactly this childless case
  was mistaken for evidence that `Star` breaks under `SharedSizeGroup`; it is only `Auto`'s
  own behavior with nothing to measure. Row 031 stays built entirely on explicit `Auto`
  definitions regardless - not because `Star` is broken, but because a `Star` factor
  assigned inside a shared group is silently discarded, which would make the row's numbers
  deceptive (readable as proving something about `Star` that the mechanism never touches)
  rather than merely wrong. Confirmed separately: the sharing itself resolves within a single
  `Layout(...)` call once the tree is initialized - no `Pump()` needed, and deleting the
  extra `Pump()` plus second `Layout(...)` from row 031's tests changes nothing.
- **A collection change reaches a generated container's CLR object synchronously, but its
  templated child content does not.** Measured directly while building row 033: right after
  `ObservableCollection<T>.Add(...)`, before any `Layout(...)` or `Pump()`,
  `ItemContainerGenerator.ContainerFromItem(newItem)` already returns a non-null
  `ContentPresenter` - the container itself needs no extra pump. But that `ContentPresenter`
  has no templated child yet at that point (`VisualTreeHelper.GetChildrenCount` is 0), so its
  bound text is not observable. Only after a second `Layout(...)` call (a plain `Pump()`
  alone also works - either one drains whatever the template instantiation is queued on)
  does the templated child exist and show the correct bound value. Row 033's tests call
  `Layout(...)` again after every collection mutation for exactly this reason.

- **A blocking `host.StartAsync().GetAwaiter().GetResult()` on this STA dispatcher does
  NOT deadlock, for a host with no real hosted background work.** Measured directly
  building row 038 (`ValidateOnStart()` failing synchronously, no other hosted services):
  both a raw blocking `.GetResult()` call and an `await`ed `StartAsync()` inside an `async
  Task` `[WpfFact]` complete in one to two seconds and propagate `OptionsValidationException`
  unwrapped, on the very STA thread doing the blocking - `[WpfFact]`'s `async Task` test
  methods do resume on the same dispatcher, confirming the harness's own claim above. This
  is NOT a general "async is safe here" finding: `DispatcherSynchronizationContext.Post`
  still queues through `Dispatcher.BeginInvoke`, and a hosted service with a real
  suspension point (actual I/O, `Task.Delay`, a background loop) resuming on that captured
  context while the dispatcher thread sits blocked in `.GetResult()` with no `PushFrame`
  pumping it would deadlock the classic way. Rows 046-050 (async commands, progress,
  dispatcher priorities, cancellation) are exactly where that risk becomes real and must
  be re-measured there, not assumed safe from this one narrow case.
- **`ValidateOnStart()` does not change WHETHER an options validation rule runs, only
  WHEN.** Any `IValidateOptions<T>`/`.Validate(...)` rule already fires lazily on the
  first `IOptions<T>.Value` access, with or without `ValidateOnStart()` - measured
  directly: a host missing `.ValidateOnStart()` still throws `OptionsValidationException`
  from `IOptions<T>.Value`, just never from `StartAsync()` itself. Row 038's tests start
  the host and assert `StartAsync()` itself throws, never touching `.Value` directly -
  that is what actually proves `ValidateOnStart()` ran, since a test that only resolves
  `.Value` after starting cannot tell "validated at start" apart from "validated lazily,
  same as always".
- **A primary-constructor parameter never referenced anywhere in the class body warns
  `CS9113` ("parameter is unread"), even though the class still compiles.** This sits
  alongside the CS0414/CS0169 field-initializer trap above but hits a different stub
  shape: a class whose subject method still `throw`s and would only start reading an
  injected constructor parameter once the learner implements the body. Row 039 hit this
  designing `Ex039_MeterReadingViewModel`; the fix is the same convention CS0414 already
  established - an explicit constructor assigning a private field, not a primary
  constructor, whenever the parameter goes unread until the learner's own code uses it.

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

`HarnessSmokeTests` exists for the same reason `uno/`'s does: it asserts STA, that an
initialized `Button` resolves its default template and measures non-zero (and now also
asserts `IsInitialized` directly, naming the `Content` side effect that makes it true - see
the `IsInitialized`/`AddLogicalChild` finding above), that a bare, never-initialized
`Button` stays template-less and `(0,0)` even after `Layout(...)` (the fifth smoke test,
proving the *failure* mode this track now depends on `CompleteInitialization` to avoid),
that a binding pushes after a pump, and that `Show(...)` raises `Loaded`. **If those fail,
the harness is broken and every other failure in the run is noise.**

## What the harness cannot do

A green test here is **not proof of desktop behaviour**. The harness answers one
narrow question — does the WPF mechanism work — and deliberately does not attempt:

- **No `Application`.** Default control templates resolve through `SystemResources`
  without one, which is why the harness never constructs one — and an `Application`
  can only be constructed once per process, so a single stray instance would poison
  every test that ran after it. Anything whose subject is an `Application` member
  (`Application.Current`, `DispatcherUnhandledException`, resource lookup through
  `Application.Resources`, …) cannot be an exercise here; see row 067 for the
  concrete case this ruled out. A direct consequence for implicit (`TargetType`-keyed)
  styles: real WPF's implicit-style chain is the element tree's `Resources` then
  `Application.Current.Resources` - and that is all of it, so the missing app-level
  stop is simply absent here. Measured: the theme dictionaries are *not* part of this
  chain - they feed a separate, lower rung reached through `DefaultStyleKey`
  (`BaseValueSource.DefaultStyle`), which is why a plain `Button` here still resolves a
  real `Template` with no `Style` in sight. An implicit style itself has nowhere but an
  element's own `Resources` to live here; row 023 (`ImplicitStyleByType`) is built
  around this, and every later style/resource row (026 onward) meets the same absence.
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
  default and the ability to override it. This is also why `HorizontalAlignment`/
  `VerticalAlignment` take effect on a bare top-level element with no parent panel
  involved at all: the alignment logic lives in `FrameworkElement`'s own arrange
  machinery, not in whichever panel calls `Arrange` on a child, so `Layout(element,
  availableSize)` alone is enough to see `RenderSize` shrink to the element's natural
  size and its `VisualOffset` move to a corner of `availableSize` — row 030 needs no
  wrapping panel to exercise this.
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

Five failure modes, each of which has already shipped in some track of this repo:

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
- **Every instruction in a stub's TODO must have an assertion behind it, or a learner
  who ignores it goes green.** Row 015 shipped a TODO telling the learner to set
  `StringFormat = "{0:0}"` with no test checking it — a learner who skipped that part
  of the instruction still passed every test. Before shipping an exercise, map each
  instruction in its TODO to the specific test that would fail if it were ignored; an
  instruction with no such test gets an assertion added, or gets dropped.

## Deliberate gaps

- **WinForms interop.** `WindowsFormsHost` would pull WinForms into both content
  libraries for one row, so ex088 does `HwndSource`/`HwndHost` plus P/Invoke instead.
- **Wall-clock performance.** No exercise asserts elapsed time — that is noise on a
  loaded machine. The performance rows (076–080) assert *that* the mechanism fired
  instead — container identity across a scroll, `IsFrozen`, the number of measure
  passes an invalidation caused.

Windows-only, because WPF is.
