# Blazor — Exercise Track

## 1. What this track is

100 graded exercises on Microsoft's Blazor component model. **"Beginner" means
Blazor beginner, not C# beginner**: ex001 is a component with a `[Parameter]`
and a computed member, not a `FizzBuzz` static method. This track never drills
plain C# language features — those belong to the `dotnet/` track. As of this
writing, only the **01-beginner** tier (ex001–ex035, component fundamentals:
`[Parameter]`, rendering directives, `@bind`, `EventCallback`,
`RenderFragment`, lifecycle, `CascadingValue`) is written and verified;
02-intermediate through 04-expert are catalog rows only (⬜).

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
| `dotnet test -p:UseSolutions=true` | run the same 115 facts against the reference solutions — all green |
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
  carries six warnings (see §9).

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
- **Multi-file exercises** follow `ExNNN_<Slug>_<Part>.razor` — e.g. ex035's
  parent component is `Ex035_TabsComposition.razor`, and the child it
  cascades to is `Ex035_TabsComposition_Tab.razor`. This keeps every file that
  belongs to one exercise sorted and grouped together on disk.
- **`_support/` is a fixture folder, not an exercise.** Both `exercises/` and
  `solutions/` carry an identical `_support/` directory (`Person.cs`,
  `AlertSeverity.cs`, `Ticker.cs`, `AddButton.razor`, `RosterEntry.razor`,
  `Level2.razor`, `Level3.razor`, plus its own `_Imports.razor`) holding types
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

## 9. The stub build is not warning-free — by design

`dotnet build` on `exercises/` emits **exactly six warnings**, all
`CS0169`/`CS0414`/`CS0649` (unused/never-assigned field), for fields that
shape-B stubs (§4) declare for the learner to wire up:

- `Ex020_DisposableComponent._ticks`
- `Ex023_InputTextBinding._note`
- `Ex024_NumericInputParsing._quantity`
- `Ex025_SelectBinding._selected`
- `Ex027_RadioGroup._selected`
- `Ex031_ChildToParentCallback._total`

These are **expected and must not be suppressed**. Each field is genuinely
unused until the learner's implementation reads or assigns it; suppressing
the warning (via `#pragma`, `NoWarn`, or renaming) would silence exactly the
category of warning a real unused-field bug would also produce, defeating the
purpose of leaving the build unsuppressed. The **solutions** project builds
with **0 warnings** — building `solutions/FeWoLearning.Blazor.Solutions.csproj`
directly confirms this; a whole-`.slnx` build in solutions mode still shows
the same six exercises warnings because `exercises/` remains part of the
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
- **Always ask what a naive, wrong implementation would do before trusting a
  green test** — restated here because it is this tier's single most
  effective check, and the one most easily skipped under time pressure.
