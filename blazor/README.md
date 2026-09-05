# Blazor — Exercise Track

## 1. What this track is

100 graded exercises on Microsoft's Blazor component model. **"Beginner" means
Blazor beginner, not C# beginner**: ex001 is a component with a `[Parameter]`
and a computed member, not a `FizzBuzz` static method. This track never drills
plain C# language features — those belong to the `dotnet/` track. As of this
writing, ex001–ex080 are written and verified — the whole of **01-beginner**
(ex001–ex035, component fundamentals: `[Parameter]`, rendering directives,
`@bind`, `EventCallback`, `RenderFragment`, lifecycle, `CascadingValue`) and
the whole of **02-intermediate** (ex036–ex070: `EditForm`/validation, DI and
state containers, JS interop, navigation, persistent component state, `@ref`,
the async lifecycle, error boundaries and generic components), plus the first
ten rows of **03-advanced** (ex071–ex080: `ShouldRender`, `@key` diffing,
`Virtualize`, custom `InputBase<T>` inputs, custom and cross-field validators,
`DynamicComponent`). ex081–ex100 are catalog rows only (⬜) — `catalog.md` is
the source of truth.

## 2. Prerequisites

- **.NET 10 SDK**, 10.0.400 verified (`dotnet --list-sdks`).
- **nuget.org reachable** on first restore — the test project pulls `bunit`
  2.9.0 plus the xunit stack the `dotnet/` track already uses
  (`xunit` 2.9.3, `xunit.runner.visualstudio` 3.1.4,
  `Microsoft.NET.Test.Sdk` 17.14.1).
- No browser, Node, or JS toolchain is needed — bUnit renders headlessly.

## 3. Commands

Run every command **from inside `blazor/`**, not the repo root.

| Command | Effect |
|---|---|
| `dotnet test` | run the stubs — all red |
| `dotnet test -p:UseSolutions=true` | run the same 295 facts against the reference solutions — all green |
| `dotnet test --filter FullyQualifiedName~Ex001_` | run one exercise |
| `dotnet run --project host` | exercise host: stub demo pages at `http://localhost:5199`, surfaces each unfinished exercise's `NotImplementedException` |
| `dotnet run --project host -p:UseSolutions=true` | reference host: the same demo pages backed by the solutions |

There is no separate install step — `dotnet test` restores on first run.

## 4. How an exercise works

Every stub compiles while unfinished and throws `NotImplementedException` at
runtime, carrying a `TODO: ExNNN - ...` message that identifies which exercise
a red failure came from. Two shapes exist, depending on where the TODO
naturally falls:

- **Shape A — the TODO lives in `@code`.** Markup renders a computed member;
  the member itself throws:

  ```razor
  <p id="greeting">@Greeting</p>
  @code {
      private string Greeting => throw new NotImplementedException("TODO: Ex001 - build the greeting");
  }
  ```

- **Shape B — the TODO would otherwise have to sit in markup, which is
  illegal.** `throw` is not a valid expression inside Razor markup
  (`CS8115`: "a throw expression is not allowed in this context"), so the
  stub throws from a lifecycle method or event handler instead. Where the
  unfinished behavior *is* the markup — e.g. ex032's
  `@if (AllowHtml) { @((MarkupString)Html) } else { @Html }` — that markup
  *is* the whole TODO and the stub needs nothing else. Six other shape-B
  stubs (`Ex020`, `Ex023`, `Ex024`, `Ex025`, `Ex027`, `Ex031`) additionally
  declare a field for the learner to use (`_ticks`, `_note`, `_quantity`,
  `_selected` twice, `_total`), because their finished markup needs something
  to bind to that the stub cannot supply itself. Until the learner writes
  that markup, the field is genuinely unused, which is why the stub build
  carries warnings (see §9). Where the field is avoidable a later stub avoids
  it — by rendering the echo markup that reads it and leaving only the
  *modified* markup as the TODO (ex058), by leaving the field itself to the
  learner (ex060's `PersistingComponentStateSubscription`), or by exposing the
  state as an auto-property, which never warns (ex061's `Panel.IsOpen`).
  Where it is not avoidable it is left alone: ex061's two `@ref` targets are
  written as private fields because that is what real Blazor code looks like,
  and an `@ref` the learner has not added yet is by definition a field nothing
  assigns.

A reference solution replaces the throwing member/method with the real
implementation and deletes anything that was there only to throw.

## 5. Layout

```
exercises/01-beginner/Ex001_HelloComponent.razor   -> FeWoLearning.Blazor.Exercises.Beginner
tests/01-beginner/Ex001_HelloComponentTests.cs     -> FeWoLearning.Blazor.Tests.Beginner
solutions/01-beginner/Ex001_HelloComponent.razor   -> FeWoLearning.Blazor.Exercises.Beginner
host/Components/Demos/Beginner/Ex001.razor         -> @page "/beginner/001"
```

- **Tier namespaces are pinned by a folder-level `_Imports.razor`**
  (`@namespace FeWoLearning.Blazor.Exercises.Beginner` and friends), because
  `01-beginner` is not a valid C# identifier and cannot itself form part of a
  namespace.
- **Not every file of a multi-file exercise carries a TODO.** Where one half is
  the subject and the other is only the setting, the setting ships finished and
  is byte-identical in `exercises/` and `solutions/`: ex067's `.razor` merely
  hosts the boundary (the exercise is the `.cs` subclass beside it), and ex068's
  parent *is* the demonstration — its call sites are what the reader is meant to
  study, so handing them over as a TODO would delete the lesson.
- **Multi-file exercises** follow `ExNNN_<Slug>_<Part>.razor` — e.g. ex035's
  parent component is `Ex035_TabsComposition.razor`, and the child it
  cascades to is `Ex035_TabsComposition_Tab.razor`. This keeps every file that
  belongs to one exercise sorted and grouped together on disk.
- **`_support/` is a fixture folder, not an exercise.** Both `exercises/` and
  `solutions/` carry an identical `_support/` directory (`Person.cs`,
  `AlertSeverity.cs`, `Ticker.cs`, `AddButton.razor`, `RosterEntry.razor`,
  `Level2.razor`, `Level3.razor`, `ExplodingChild.razor`, `ErrorLog.cs`,
  `DynamicBadge.razor`, `DynamicNote.razor`, and the DI fixtures, plus its own
  `_Imports.razor`) holding types
  and small components that several exercises' tests depend on. It never
  contains a TODO, is never itself a graded exercise, and never gets a
  `catalog.md` row.

## 6. Why `solutions/` is a real project here

`CLAUDE.md`'s universal convention keeps `solutions/` out of every track's
build, because solutions deliberately reuse the stubs' names and namespaces
and would otherwise collide with them. **This track is a documented,
deliberate exception**: `solutions/FeWoLearning.Blazor.Solutions.csproj` is a
real project, referenced by `tests/` and `host/` whenever `UseSolutions=true`
is passed.

The reason is that the collision the convention guards against cannot occur
here: `tests/` and `host/` each reference *either* `exercises/` *or*
`solutions/`, selected by the `UseSolutions` MSBuild property — never both in
the same build — so the identical namespaces and type names in the two RCLs
are never loaded into one compilation.

The benefit is real: because `solutions/` compiles and runs on every green
check, this track cannot suffer the silent-solution-drift failure class that
the 2026-08-03 audit found elsewhere in the repo (five broken solutions in
`vue/`, four defective tests in `go/`) — a reference solution that no longer
matches its test would fail to compile or fail its test, not sit unnoticed.

**Do not "fix" this back** by excluding `solutions/` from the `.slnx` or the
build. That would remove the compile-checking that is this track's specific
defense against drift, for no benefit — the collision risk is structurally
absent, not merely avoided by convention.

## 7. Non-goals

Nothing in this track exercises real browser behavior. Specifically excluded,
because bUnit cannot test them honestly:

- Real WebAssembly loading.
- Real SignalR circuit reconnects.
- Real `focus()` / `scrollIntoView()` behavior.

Where an exercise needs a JS call, its test asserts against **bUnit's
`JSInterop` mock** — which invocation happened, with which arguments — never
against actual browser behavior. **A green test in this track is not evidence
of browser behavior**; it is evidence the component called the right method
with the right arguments against a stand-in.

`<Virtualize>` is a partial exception worth spelling out, because ex073/ex074
depend on knowing where the line is. It renders in bUnit, but there is no
viewport for it to measure, so it falls back to a fixed window: **`ItemSize`
does not change how many rows are realised** (20f and 80f both produced 100),
and a *pending* items provider renders neither rows nor placeholders. What is
observable, and what those two exercises are therefore graded on: the
`ItemsProviderRequest` the component issues, the rows it realises versus the
declared total, `OverscanCount` widening that window (100 → 120 for an
overscan of 10), the `Placeholder` fragment filling the slots a provider
*under-delivered* on, and `ItemSize` scaling the trailing spacer `<div>` that
reserves room for the rows that are not in the DOM. Anything phrased in terms
of scrolling belongs in a browser, not here.

`preventDefault` is a non-goal of this tier specifically:
`@onclick:preventDefault` has no observable effect on bUnit's DOM (there is no
real browser default action to suppress), so a bUnit test for it would pass
whether or not the directive were present — the tier used
`@onclick:stopPropagation` instead (ex022), which bUnit *can* observe through
bubbling. `preventDefault` moves to the intermediate tier, where an
`EditForm` submit gives it an observable effect (the form not re-posting).

## 8. bUnit 2 API notes

bUnit 2.x renamed several members relative to older bUnit/blazor-testing
material found online. This track uses the current names throughout:

- Test classes derive from **`Bunit.BunitContext`**, not `TestContext` —
  `TestContext` is ambiguous with `Xunit.TestContext` (`CS0104`) once both
  namespaces are in scope.
- Re-rendering with new parameters is **`cut.Render(p => ...)`**, not
  `SetParametersAndRender`.
- **`Find`/`FindAll` return element wrappers, not component instances.** To
  prove component *identity* (e.g. "the same child instance survived a
  reorder"), go through `cut.FindComponents<T>()[i].Instance`, not through an
  element handle.
- **`<ErrorBoundary>` works in bUnit, `Recover()` included** — bUnit registers
  an `IErrorBoundaryLogger` of its own, so nothing has to be wired up. Two
  consequences worth knowing. A test can register its own
  `IErrorBoundaryLogger` over bUnit's, which is the only way to observe whether
  a custom boundary called `base.OnErrorAsync` (ex067 grades exactly that).
  And a boundary with no `ErrorContent` rethrows, so `Render` itself fails —
  meaning "the error content appeared" and "the exception was caught" are one
  fact, not two.
- **A custom `ErrorBoundary` subclass has to be a `.cs` file, not a `.razor`
  one.** Every `.razor` file emits a `BuildRenderTree` override; on a subclass
  of `ErrorBoundary` that silently replaces the base implementation and the
  boundary renders nothing at all. ex067's subclass is therefore plain C# with
  `[Inject]` property injection, which works on any component, markup or not.
- **There is no parameterless `Render()` on a rendered component.** To push
  parameters again — which is how ex062 proves its load runs once and ex064
  counts sets — re-supply them: `cut.Render(p => p.Add(c => c.X, value))`.
  `cut.Render(_ => { })` pushes an empty `ParameterView`, which does *not*
  reset already-assigned parameters; it is a set carrying nothing.
- **`BunitNavigationManager.History` is stack-ordered and typed
  `IReadOnlyCollection<NavigationHistory>`.** The *first* element is the most
  recent navigation, not the oldest, and the initial URI is not in it at all.
  It has no indexer, so it is `History.First()`, not `History[0]`. Each entry
  carries a `NavigationState` — `Succeeded` / `Prevented` / `Faulted` — which
  is how ex057 proves a location-changing handler actually cancelled a
  navigation rather than merely running.
- **`RegisterLocationChangingHandler` works against the fake navigation
  manager** — `PreventNavigation()` really does leave `Uri` where it was and
  mark the history entry `Prevented`, and disposing the registration really
  does re-arm navigation. That handler runs *outside* the render loop, though,
  so it must not call `StateHasChanged`; ex057 counts blocks into a property
  the test reads off `cut.Instance` instead of rendering them.
- **`PersistentComponentState` is not in bUnit's default services, but bUnit
  ships the test double for it** — `AddBunitPersistentComponentState()`
  registers it and hands back a `BunitPersistentComponentState` with three
  members that are the whole of ex059/ex060's test surface: `Persist<T>(key,
  value)` seeds what an earlier render pass left behind, `TriggerOnPersisting()`
  runs the callbacks the component registered, and `TryTake<T>(key, out value)`
  reads back what they wrote. Like every service registration it has to happen
  before the first render. This one is easy to miss and expensive to miss:
  building the equivalent by hand out of `ComponentStatePersistenceManager`,
  a fake `IPersistentComponentStateStore` and `PersistStateAsync(store,
  Renderer)` works, but is ~40 lines of fixture, and touching
  `BunitContext.Renderer` counts as the first service resolution — which locks
  the service collection and makes the ordering rules subtle for no gain.

## 9. The stub build is not warning-free — by design

`dotnet build` on `exercises/` emits **exactly twelve warnings**, all
`CS0169`/`CS0414`/`CS0649` (unused/never-assigned field), for fields that
shape-B stubs (§4) declare for the learner to wire up:

- `Ex020_DisposableComponent._ticks`
- `Ex023_InputTextBinding._note`
- `Ex024_NumericInputParsing._quantity`
- `Ex025_SelectBinding._selected`
- `Ex027_RadioGroup._selected`
- `Ex031_ChildToParentCallback._total`
- `Ex040_EditContextFieldState._context`
- `Ex041_CustomFieldValidation._context`
- `Ex045_CascadingServiceInjection._fromProperty`
- `Ex052_JsInteropModule._module`
- `Ex061_RefCaptureBasics._box`
- `Ex061_RefCaptureBasics._panel`

These are **expected and must not be suppressed**. Each field is genuinely
unused until the learner's implementation reads or assigns it; suppressing
the warning (via `#pragma`, `NoWarn`, or renaming) would silence exactly the
category of warning a real unused-field bug would also produce, defeating the
purpose of leaving the build unsuppressed. The **solutions** project builds
with **0 warnings** — building `solutions/FeWoLearning.Blazor.Solutions.csproj`
directly confirms this; a whole-`.slnx` build in solutions mode still shows
the same twelve exercises warnings because `exercises/` remains part of the
`.slnx` regardless of `UseSolutions`.

## 10. Sharpest edge in this tier: ex035 can hang the test host, not fail it

ex035 (`TabsComposition`) cascades a parent reference to its tabs so a tab can
ask its parent to re-render when its own `Title` changes. **A naive,
plausible-looking wrong implementation of that callback — an unconditional
`Parent?.Refresh()` that calls `StateHasChanged()` every time it's invoked,
with no guard — does not fail red. It hangs the test host.** A reviewer
reproduced this directly: all six ex035 facts ran to the 180-second timeout
and were killed (`EXIT 124`), because each `StateHasChanged()` triggers
another render pass, which triggers another `Refresh()` call, forever.

If an ex035 test run stalls rather than failing quickly, this is almost
certainly why — do not raise the timeout; fix the recursion.

The reference solution bounds the cascade with a `_refreshQueued` flag:
`Refresh()` is a no-op if a refresh is already queued, and the flag is
cleared in `OnAfterRender`, so at most one extra render pass can ever be
in flight regardless of how many tabs call `Refresh()` in the same pass or
how often `Title` keeps changing across passes.

## 11. Test-quality rules

Checked for every exercise in this tier, the Blazor analogue of the Python
and Kotlin traps already documented in the repo root `CLAUDE.md`:

- A bUnit test that does not wait for a re-render after a state change
  asserts only on the **first** frame. For `@onclick` and async lifecycle,
  use `cut.WaitForAssertion(...)` / `InvokeAsync` on **markup** assertions,
  where a stale render frame is possible — never a bare assertion on markup
  immediately after the trigger. Three exemptions: an assertion on a
  **captured local** (a value an `EventCallback` or delegate handed straight
  to a test variable) does not need the wrapper, since there is no render
  frame to go stale; a **negative assertion** ("this markup did not change")
  must stay bare, since `WaitForAssertion` would pass on its first attempt
  regardless and, when the value genuinely does change, would delay the
  failure by the full timeout instead of catching it; and an assertion right
  after a synchronous `cut.Render(...)` parameter push (as opposed to an
  event dispatch) needs no wrapper either, since the render completes before
  `Render` returns. In every exemption case, wrapping only delays reporting a
  genuine failure by the full timeout.
- A test that compares `cut.Markup` against a whole string breaks on any
  whitespace change and proves nothing about behavior. Assert through
  `Find`/`FindAll` plus `TextContent` or a specific attribute instead.
- Before accepting a red run, ask: **would a naive or wrong implementation
  also pass this test?** If yes, the test is defective.
- Confirm each red failure comes from the exercise's own
  `NotImplementedException`, not from a compile or resolution error. A stub
  that fails to build is a bug, not a passing red check.

### What this track actually learned the hard way

Beyond the rules above, review rounds on this specific tier turned up four
concrete failure modes worth carrying into future batches:

- **Never assert on whole-markup strings.** Same reasoning as above, stated
  as a hard rule rather than a guideline — it was tempting more than once,
  and always regretted.
- **Prefer exact equality over `Contains` where the string is fully
  determined.** A `Contains` assertion on one exercise's counter text once
  accepted a trailing separator character that a wrong implementation also
  produced, and stayed green. When the expected string is fully known, assert
  `Assert.Equal`, not `Assert.Contains`.
- **A fixture must not report success independently of the mechanism it is
  supposed to observe.** A hand-tracked subscriber counter in one exercise's
  support fixture once incremented on subscribe and decremented on any
  disposal path, which let a leaking component (one that subscribed twice but
  disposed once) still show a "correct" final count. Fixtures must observe
  the actual mechanism (the real event's invocation, the real list's
  contents), not a parallel bookkeeping variable that can drift from it.
- **One fact in this track goes red on an assertion instead of on a
  `NotImplementedException`, deliberately.** ex069's subject is a *type
  constraint*, and no behaviour can prove one: `Items.Min()`/`Max()` need no
  constraint and satisfy every behavioural fact. That row is graded by reading
  the type parameter's metadata
  (`typeof(C<>).GetGenericArguments()[0].GetGenericParameterConstraints()`),
  which is red on the stub because the constraint is missing, not because
  anything threw. Verified by mutation: replacing the solution with a
  LINQ-based, unconstrained one takes down that fact and no other. Where a
  requirement is invisible to behaviour, assert the metadata — but say so at
  the fact, because it breaks the rule above.
- **To make a negative assertion about async work mean something, drain the
  dispatcher first.** "The load that finished after disposal did not write
  `Result`" (ex063) and "the superseded search's answer did not overwrite the
  newer one" (ex065) both pass vacuously if the continuation simply has not run
  yet. `await Renderer.Dispatcher.InvokeAsync(() => { })` fixes that without a
  sleep: the continuation was queued on that dispatcher first, so once a no-op
  queued after it has completed, it has already had its turn. Pair it with
  `TaskCreationOptions.RunContinuationsAsynchronously` on the test's
  `TaskCompletionSource`, or `SetResult` runs the continuation inline on the
  test thread and the ordering argument no longer holds.
- **Real time appears exactly once in this track, in ex065.** A `Task.Delay`
  debounce cannot be tested without it. The window is a parameter so the test
  can shrink it (200ms) and every wait has a deadline far above it (5s), which
  is a ~25x margin rather than a few percent; the suite was re-run three times
  to confirm. Any future timing exercise should follow the same shape rather
  than sleeping for a fixed guess. `WaitForAssertion` is not a substitute here:
  it re-checks on renders, so a condition about a fixture's own bookkeeping
  (how many times the fake search was called) needs a plain polling loop.
- **Always ask what a naive, wrong implementation would do before trusting a
  green test** — restated here because it is this tier's single most
  effective check, and the one most easily skipped under time pressure.
- **For a "stops doing X once disposed" fact, mutate the solution and watch
  the fact go red.** Asking the question is cheap but easy to answer wrongly;
  ex056–ex060 were each checked by actually breaking the solution (an empty
  `Dispose()`, a callback capturing its value at registration time, a
  hand-rolled query-string split, a plain `@bind` with the modifiers stripped)
  and confirming that exactly the intended facts failed and no others. Two of
  those mutations are the bugs a learner is most likely to write, and both
  produce a component that looks entirely correct while it is on screen.
  ex061–ex065 were checked the same way — dropping both `@ref` captures,
  clearing the loading flag before the `await` instead of after, disposing the
  `CancellationTokenSource` without cancelling it, comparing the incoming
  parameter *after* `base.SetParametersAsync`, removing the debounce delay, and
  removing the post-`await` `ThrowIfCancellationRequested` — each taking down
  exactly the facts it should. The `base.SetParametersAsync` ordering mutation
  is the one worth remembering: it fails all four of ex064's facts, because
  once `base` has assigned the property there is no longer a previous value to
  compare against, and every push looks unchanged. ex066–ex070 likewise:
  dropping `ErrorContent`, returning early instead of calling
  `base.OnErrorAsync`, ignoring the `RenderFragment<T>`, swapping the
  constrained comparison for LINQ, and rendering the `<ul>` unconditionally.
  ex071–ex075 as well: forgetting to re-open the `ShouldRender` gate after a
  click (both click facts), dropping the `@key` (four of five), materialising
  the whole list instead of paging it, not forwarding `ItemSize`/
  `OverscanCount` to `<Virtualize>`, and accepting invalid input in
  `TryParseValueFromString` instead of reporting it. And ex076–ex080: parsing
  against `CurrentCulture`, clearing the whole `ValidationMessageStore` on a
  field change instead of the one field, re-checking only the field that
  changed when the rule spans two, handing back a freshly constructed instance
  rather than `DynamicComponent.Instance`, and accepting any public property as
  a parameter. ex081–ex085 too: dropping the `Authorizing` fragment, building
  the `ClaimsIdentity` without an authentication type, leaving
  `NotifyAuthenticationStateChanged` out, keeping `StateHasChanged` inside the
  custom `IHandleEvent`, running the once-only after-render work every time,
  and hardcoding the render-mode name.
- **A stub whose TODO is only markup needs a throwing lifecycle method too.**
  ex079's first draft left the `<DynamicComponent>` markup as the TODO and threw
  only from a property one fact touched; the other three failed on a missing
  element instead — red, but not traceably red. Adding a throwing
  `OnParametersSet` (the ex023/ex025 shape) makes every fact of the exercise
  fail with its own message. Check the red run's *reasons*, not just its count.
- **Mutate the solution before writing what a fact proves, not after.** ex075's
  test carried a comment claiming that assigning a half-parsed array before
  returning `false` from `TryParseValueFromString` would fail it. The mutation
  said otherwise: `InputBase` ignores the `out` value entirely on the false
  path, so that implementation passes every fact. The comment was wrong, not
  the test — the graded contract is the `false` plus the message, and the fact
  does catch an implementation that returns `true` for bad input. A confident
  sentence in a test comment is a claim like any other; run it.
- **A first render never consults `ShouldRender`.** ex071 originally had a
  "the first render happens" fact, which passed against the untouched stub for
  that reason — ComponentBase renders once before it ever asks. Its assertions
  now ride along inside the fact that pushes a second time. Any fact about a
  gate must arrange for the gate to be consulted.
- **A `ClaimsIdentity` with no authentication type is not authenticated.** ex082
  asserts the name *and* `IsAuthenticated` for exactly this reason: a provider
  that returns `new ClaimsIdentity([nameClaim])` reports the right user and
  leaves every `AuthorizeView` on the page showing its anonymous branch. A test
  that only checked the name would call that implementation correct.
- **Implementing `IHandleAfterRender` silences `OnAfterRender` completely.** The
  interface *is* the dispatch — `ComponentBase` implements it and calls
  `OnAfterRender` from inside it — so re-implementing it on a derived component
  replaces that wholesale, and the familiar override never runs again. ex084
  asserts the base hook stays at zero, measured directly.
- **`decimal` keeps its scale through arithmetic.** ex076 stores a fraction and
  shows a percentage, and `0.15m * 100m` is `15.00`, not `15` — a bare
  `ToString` puts the trailing zeros in the input box. Found by a green run
  failing, and now part of what the exercise teaches rather than something the
  test was loosened to accept.
- **`T?` on an unconstrained type parameter is an annotation, not
  `Nullable<T>`.** ex068 renders a badge for `T="Guid"` with no value, and the
  fallback is `Guid.Empty`, not nothing — a value-type `T` has no null to fall
  back on. This was found by a green run failing, and is now asserted rather
  than designed around, because it is the surprise a reader of a generic
  component most needs to have had once. ex069 carries the same asymmetry in
  its empty-list case, which is why only a reference type is asserted there.
