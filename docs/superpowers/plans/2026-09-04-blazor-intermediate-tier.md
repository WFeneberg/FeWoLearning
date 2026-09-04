# Blazor Intermediate Tier Implementation Plan (ex036–ex070)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add the Intermediate tier (ex036–ex070) to the existing `blazor/` track — 35 exercises covering `EditForm`/validation, DI and state containers, JS interop, navigation, persistent component state, async lifecycle, error boundaries and generic components — each as stub + bUnit test + reference solution + host demo page, verified red and green.

**Architecture:** No new architecture. The `blazor/` track's four projects, the `UseSolutions` red/green switch, the two stub shapes and the tier-namespace pinning are all already built, verified and documented. This plan adds exercises into that structure and nothing else.

**Tech Stack:** .NET 10 (`net10.0`, SDK 10.0.400) · bUnit 2.9.0 · xunit 2.9.3 · Razor Class Libraries · Blazor Web App host (InteractiveServer)

**Spec:** [`docs/superpowers/specs/2026-09-04-blazor-track-design.md`](../specs/2026-09-04-blazor-track-design.md) — §6's tier themes are the content authority. Deviations measured during planning are recorded in Global Constraints below.

**Predecessor:** [`docs/superpowers/plans/2026-09-04-blazor-track.md`](2026-09-04-blazor-track.md) delivered the Beginner tier (ex001–ex035, 115 facts red and green). Its nine test-quality rules were earned through eight review rounds and are now documented in `blazor/README.md`; they bind this tier too.

---

## Global Constraints

Every task's requirements implicitly include this section.

### Read these first, once

The track's conventions are documented in the repo — do not restate them, read them:

- **`blazor/README.md`** — the command table, both stub shapes and why shape B exists, the layout and namespace pinning, the multi-file exercise convention, the `_support/` fixture rule, the bUnit 2 API notes, the non-goals, and the test-quality rules.
- **`CLAUDE.md`** → the `**Blazor**` gotchas bullet and step 5 of "Adding or completing exercises".
- **One finished exercise from the Beginner tier as a style template**, once — `exercises/01-beginner/Ex035_TabsComposition.razor` plus `tests/01-beginner/Ex035_TabsCompositionTests.cs` are the most-reviewed pair in the track.

### Versions and paths

- `net10.0`. Package versions exactly as the existing `tests/FeWoLearning.Blazor.Tests.csproj` already pins them. Do not add packages unless a task says to.
- All commands run from inside `blazor/`.
- New exercises go in `exercises/02-intermediate/` and `solutions/02-intermediate/`, whose `_Imports.razor` files already pin `@namespace FeWoLearning.Blazor.Exercises.Intermediate`. Tests go in `tests/02-intermediate/` under `namespace FeWoLearning.Blazor.Tests.Intermediate`. Demo pages go in `host/Components/Demos/Intermediate/` at `@page "/intermediate/NNN"`.
- **`host/Components/_Imports.razor` already imports the Intermediate namespace** — verify, do not assume.
- **`host/Components/Pages/Home.razor` currently links only `/beginner/001`–`035`.** Task 1 extends it with an Intermediate section.

### Concurrency — this repository has other writers

Two other Claude sessions commit to this same branch, owning `uno/` and `avalonia/`. Therefore:

- Stage **explicit paths under `blazor/` only**. Never `git add -A`. Never stage anything outside `blazor/` except where a task explicitly says so.
- Never revert, rebase, reset or stash. Unfamiliar commits in `git log` are expected.
- Review ranges are derived from the implementer's **own reported commit SHAs**, never from the branch head.

### The two gates

From inside `blazor/`:

| Command | Effect |
|---|---|
| `dotnet test` | stubs — every fact red |
| `dotnet test -p:UseSolutions=true` | reference solutions — every fact green |
| `dotnet test --filter "FullyQualifiedName~Ex036_"` | one exercise |

**The Beginner tier's 115 facts must stay 115 red / 115 green throughout.** Any task that moves those numbers has broken something; stop and report rather than adjusting a test.

The stub build emits **exactly six** benign warnings today (`Ex020._ticks`, `Ex023._note`, `Ex024._quantity`, `Ex025._selected`, `Ex027._selected`, `Ex031._total`) for fields shape-B stubs declare for the learner. Each new shape-B stub that declares such a field adds one more — that is expected and acceptable. **Never suppress them**, and report the count honestly; the stub build is not warning-free and no document may claim it is. The solutions build must stay at **0 warnings**.

### Measured facts — established by probe during planning, do not re-derive

All five of the tier's risky mechanisms were measured before this plan was written:

1. **`ErrorBoundary` works under bUnit.** A child throwing from `OnParametersSet` is caught and the `<ErrorContent>` renders. Verified.
2. **bUnit's `JSInterop` mock works.** `JSInterop.Setup<string>("app.beep", 42).SetResult("beeped")` then `JSInterop.VerifyInvoke("app.beep")`. Verified end to end including the awaited return value reaching markup.
3. **Async lifecycle is honestly testable.** With `[Parameter] Task<string> Source` fed from a `TaskCompletionSource`, the loading frame **is** observable before completion, and `cut.WaitForAssertion` picks up the resolved frame after `tcs.SetResult(...)`. This is what makes the async batch possible; without it those exercises would only ever see the final frame.
4. **`NavigationManager` is registered out of the box** in `BunitContext`, and a `NavigateTo` is observable by reading `Services.GetRequiredService<NavigationManager>().Uri` (needs `using Microsoft.Extensions.DependencyInjection;` — `BunitServiceProvider` has no `GetRequiredService` of its own, so the DI extension method must be imported, or you get `CS1061`).
5. **`PersistentComponentState` is NOT registered** and its constructor is non-public, so it must be provided by a fixture. What works:

       var manager = new ComponentStatePersistenceManager(
           NullLogger<ComponentStatePersistenceManager>.Instance);
       await manager.RestoreStateAsync(new SeededStore(seed));   // optional, for the restore path
       Services.AddSingleton(manager.State);                     // MUST precede the first Render

   `IPersistentComponentStateStore` is public, so a fixture can pre-seed state exactly as a prerender would. **Registration after the first render throws** (`New services/implementations cannot be registered ... after the first service has been retrieved`). Needs `Microsoft.Extensions.Logging.Abstractions` and `Microsoft.AspNetCore.Components.Infrastructure`.

### Deviations from spec §6's tier themes, decided while planning

1. **`PersistentComponentState` requires a new `_support/` fixture** (`SeededStore`), byte-identical in both RCLs, because the service is not registered and cannot be constructed directly. Without it only the trivial "no state" branch would be testable. Task 5 adds it.
2. **`[PersistentState]` is added as an exercise.** The probe surfaced that .NET 10 ships this declarative alternative to `TryTakeFromJson` alongside the imperative API; it belongs next to it. It takes ex060's row.
3. **ex068 and ex070 are re-specced.** The Beginner tier's ex013 (`@typeparam TItem` + `RenderFragment<TItem>` over a list) already delivers what their catalog rows describe, which the Beginner tier's final review flagged. New subjects: ex068 becomes **generic type inference** (inferred from `Items` versus an explicit `TItem=`, and the cases where inference fails), ex070 becomes **`Context` naming** (`Context="item"` and nested templated components whose implicit `@context` collide). ex069 (constraints) is unchanged and already distinct.
4. **`@onsubmit:preventDefault` is dropped from ex036's Concepts cell, and the promise is removed from `blazor/README.md` and spec §6.** Measured: the *effect* is not observable in bUnit (the handler fires once with or without the directive), but the *directive* is — bUnit's DOM carries `blazor:onsubmit:preventdefault=""` as an attribute. So a discriminating test is possible, and the Beginner tier's stated reason for moving `preventDefault` here ("any test would pass whether or not the directive is present") was **wrong**. It is still dropped, for a better-founded reason: the only available assertion couples to bUnit's internal `blazor:`-prefixed attribute encoding and proves that the learner typed the directive rather than that anything was prevented. Record the corrected reasoning where the old claim stood.

### Stub shapes — a reminder of the one thing that is easy to get wrong

`throw` is illegal in Razor markup (`CS8115`). **Shape A**: markup is complete, a member in `@code` throws. **Shape B**: markup is left as a `@* TODO: … *@` comment and the component throws from a lifecycle method, written across two lines. Every stub carries the four-line `Goal:` / `Drills:` / `Passes:` header, and **its solution carries the identical header**. Every `NotImplementedException` message starts `TODO: ExNNN - `. A solution **deletes** the throwing member; it never leaves it unreachable.

For an `async` exercise, shape B's throw goes in `OnInitializedAsync` and the method stays `async Task` (add `await Task.CompletedTask;` after the throw only if the compiler demands it — it does not, because `throw` satisfies the return).

### The nine test-quality rules — read them in `blazor/README.md` §11 and apply them

Summarised here only so a task can point at a number:

1. Never a fact that asserts only markup or defaults the stub hands the learner complete. Fold into a sibling fact **only when the premise is identical**, with a comment stating that reason.
2. Scope every selector where nesting is contractual.
3. Never wrap `cut.Find(...)` in `Assert.NotNull`.
4. `WaitForAssertion` belongs on **markup** assertions after an event dispatch — not on captured locals, not after a plain `cut.Render(...)` push, and not around a negative "stayed the same" assertion.
5. Demo pages start directly with `@page`; collections come from a `@code` field, house style `private readonly string[] _x = ["a", "b"];`.
6. Never assert against `cut.Markup` as a whole string; prefer exact equality wherever the string is fully determined.
7. A fixture must not report success independently of the mechanism it observes.
8. A mid-task deviation from the specified test mechanics gets its own mutation round-trip, not a reasoned argument.
9. When a test pins an exact expected string, the stub's TODO must state it too.

Plus: **do not fold an empty-collection fact** in a projection-style exercise, and **for every exercise write down the simplest wrong implementation a learner might produce and confirm some fact rejects it.**

### Commit discipline

One commit per task, named `blazor: exNNN-exNNN`. Explicit paths. Message ends with:

    Co-Authored-By: Claude Opus 5 (1M context) <noreply@anthropic.com>

---

## File Structure

    blazor/
      exercises/02-intermediate/ExNNN_<Slug>.razor         # the stubs (+ _<Part> files where needed)
      exercises/_support/                                   # + SeededStore.cs, ContactModel.cs, CounterStore.cs
      solutions/02-intermediate/ExNNN_<Slug>.razor          # mirrors, same namespaces
      solutions/_support/                                    # byte-identical copies of the new fixtures
      tests/02-intermediate/ExNNN_<Slug>Tests.cs
      host/Components/Demos/Intermediate/ExNNN.razor        # @page "/intermediate/NNN"
      host/Components/Pages/Home.razor                      # extended with an Intermediate section (Task 1)
      catalog.md                                             # rows 036-070 flipped per batch

Every model, validator and probe component that a **test** needs but that is **not** itself the exercise goes in `_support/`, byte-identical in both RCLs, never containing a TODO, never in `catalog.md`.

---

## Task 1: Tier scaffolding and the shared fixtures

Delivers the fixtures the whole tier depends on, plus the host's Intermediate index. No exercises.

**Files:**
- Create: `blazor/exercises/_support/ContactModel.cs` and the byte-identical `blazor/solutions/_support/ContactModel.cs`
- Create: `blazor/exercises/_support/SeededStore.cs` and its byte-identical copy
- Create: `blazor/exercises/_support/CounterStore.cs` and its byte-identical copy
- Modify: `blazor/host/Components/Pages/Home.razor`
- Verify: `blazor/exercises/02-intermediate/_Imports.razor`, `blazor/solutions/02-intermediate/_Imports.razor`, `blazor/host/Components/_Imports.razor`

**Interfaces produced:**
- `FeWoLearning.Blazor.Support.ContactModel` — `public sealed class ContactModel { public string? Name { get; set; } public int Age { get; set; } public AddressModel Address { get; set; } = new(); }` with `[Required]` on `Name`, `[Range(1, 120)]` on `Age`, and `[ValidateComplexType]` on `Address`; plus `public sealed class AddressModel { [Required] public string? City { get; set; } }`. Used by ex036–ex042.
- `FeWoLearning.Blazor.Support.SeededStore` — `public sealed class SeededStore(IDictionary<string, byte[]> seed) : IPersistentComponentStateStore`, returning `seed` from `GetPersistedStateAsync()` and recording what was handed to `PersistStateAsync` in a public `IReadOnlyDictionary<string, byte[]>? Persisted` property. Used by ex059–ex060.
- `FeWoLearning.Blazor.Support.CounterStore` — a state container: `public int Value { get; private set; }`, `public event Action? Changed`, `public void Increment()` raising `Changed`, and `public int SubscriberCount => Changed?.GetInvocationList().Length ?? 0`. Used by ex043–ex046.

- [ ] **Step 1: Verify the tier folders and host imports already exist**

Read `blazor/exercises/02-intermediate/_Imports.razor` and its `solutions/` twin — Task 1 of the Beginner plan created them with `@namespace FeWoLearning.Blazor.Exercises.Intermediate`. Read `blazor/host/Components/_Imports.razor` and confirm it imports `FeWoLearning.Blazor.Exercises.Intermediate` and `FeWoLearning.Blazor.Support`. **If any is missing, create or add it and say so in your report** — do not assume.

- [ ] **Step 2: Write `ContactModel`**

```csharp
// exercises/_support/ContactModel.cs — and byte-identical in solutions/_support/
using System.ComponentModel.DataAnnotations;

namespace FeWoLearning.Blazor.Support;

/// <summary>Test fixture model for the EditForm exercises. Not an exercise.</summary>
public sealed class ContactModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int Age { get; set; } = 1;

    [ValidateComplexType]
    public AddressModel Address { get; set; } = new();
}

/// <summary>Nested fixture model for Ex042. Not an exercise.</summary>
public sealed class AddressModel
{
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
}
```

The `ErrorMessage` strings are fixed here because ex037 and ex039 pin them; rule 9 then requires those stubs' TODOs to state them.

- [ ] **Step 3: Write `SeededStore`**

```csharp
// exercises/_support/SeededStore.cs — and byte-identical in solutions/_support/
using Microsoft.AspNetCore.Components;

namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for the PersistentComponentState exercises. Hands a
/// ComponentStatePersistenceManager a pre-seeded payload, exactly as a
/// prerender would, and records what a component persisted back.
/// Not an exercise.
/// </summary>
public sealed class SeededStore(IDictionary<string, byte[]> seed) : IPersistentComponentStateStore
{
    public IReadOnlyDictionary<string, byte[]>? Persisted { get; private set; }

    public Task<IDictionary<string, byte[]>> GetPersistedStateAsync()
        => Task.FromResult(seed);

    public Task PersistStateAsync(IReadOnlyDictionary<string, byte[]> state)
    {
        Persisted = state;
        return Task.CompletedTask;
    }
}
```

`Persisted` is a real recording of what the component handed over, not a hand-tracked flag — rule 7.

- [ ] **Step 4: Write `CounterStore`**

```csharp
// exercises/_support/CounterStore.cs — and byte-identical in solutions/_support/
namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture state container for the DI exercises. SubscriberCount is
/// derived from the live invocation list, never hand-tracked, so a component
/// that subscribes and never unsubscribes cannot pass. Not an exercise.
/// </summary>
public sealed class CounterStore
{
    public int Value { get; private set; }

    public event Action? Changed;

    public int SubscriberCount => Changed?.GetInvocationList().Length ?? 0;

    public void Increment()
    {
        Value++;
        Changed?.Invoke();
    }
}
```

- [ ] **Step 5: Extend the host index**

In `blazor/host/Components/Pages/Home.razor`, add an `<h2>Intermediate</h2>` section listing `/intermediate/036`–`/intermediate/070` with the same loop shape the Beginner section uses. The links are dead until their batch lands, which is expected and matches the precedent set for the Beginner tier.

- [ ] **Step 6: Verify nothing regressed**

```bash
cd blazor
dotnet build
dotnet build -p:UseSolutions=true
dotnet test
dotnet test -p:UseSolutions=true
```

Expected: builds succeed; the solutions build at 0 warnings; the stub build still at exactly six warnings; **115 red and 115 green, unchanged**. New fixtures add no facts.

- [ ] **Step 7: Commit**

```bash
git add blazor/exercises/_support blazor/solutions/_support blazor/host/Components/Pages/Home.razor
git commit -m "blazor: intermediate tier scaffolding and shared fixtures"
```

---

## Task 2: ex036–ex040 — EditForm and validation

**Files:** five stubs under `exercises/02-intermediate/`, five mirrors under `solutions/02-intermediate/`, five test classes under `tests/02-intermediate/`, five demo pages under `host/Components/Demos/Intermediate/`, `catalog.md` rows 036–040.

**Interfaces consumed:** `Support.ContactModel`, `Support.AddressModel` (Task 1).

- [ ] **Step 1: Write the five stubs**

**ex036 `Ex036_EditFormBasics` — shape B.** Note per Global Constraints deviation 4 that `@onsubmit:preventDefault` is **not** part of this exercise; the lesson is that `EditForm` handles submission itself.
- Parameters: `[Parameter, EditorRequired] public ContactModel Model { get; set; } = default!;`, `[Parameter] public EventCallback<ContactModel> OnAccepted { get; set; }`.
- Public counter so a test can read it: `public int SubmitCount { get; private set; }`.
- TODO markup comment: render an `<EditForm Model="Model" OnValidSubmit="AcceptAsync">` containing `<InputText id="name" @bind-Value="Model.Name" />` and `<button id="submit" type="submit">Save</button>`, plus `<span id="count">@SubmitCount</span>`. State that `EditForm` renders the `<form>` element itself and handles submission — you do not write `@onsubmit` or a preventDefault directive on it.
- Throwing `OnParametersSet`, `TODO: Ex036 - render the EditForm`.

**ex037 `Ex037_DataAnnotationsValidation` — shape B.**
- Parameters: `Model`, plus `[Parameter] public EventCallback OnValid { get; set; }` and `[Parameter] public EventCallback OnInvalid { get; set; }`.
- Public counters: `public int ValidCount { get; private set; }`, `public int InvalidCount { get; private set; }`.
- TODO markup comment: an `EditForm` with a `<DataAnnotationsValidator />` and a `<ValidationSummary />`, an `<InputText id="name" @bind-Value="Model.Name" />`, an `<InputNumber id="age" @bind-Value="Model.Age" />`, a submit button `#submit`, and `<span id="counts">@ValidCount/@InvalidCount</span>`. Wire `OnValidSubmit` **and** `OnInvalidSubmit`. State the exact expected message strings: `Name is required` and `Age must be between 1 and 120` (rule 9).
- Throwing `OnParametersSet`, `TODO: Ex037 - validate the model on submit`.

**ex038 `Ex038_CustomValidationAttribute` — shape A.** The exercise is the attribute, not the markup.
- The stub file is `Ex038_CustomValidationAttribute.razor` rendering a complete `EditForm` with `<DataAnnotationsValidator />`, `<InputText id="code" @bind-Value="_model.Code" />`, `#submit`, and `<ul id="errors">` listing `_context.GetValidationMessages()`. The `@code` block holds a nested model whose `Code` property carries `[ProjectCode]`.
- The TODO is the attribute itself: a `private sealed class ProjectCodeAttribute : ValidationAttribute` whose `IsValid(object?)` override throws. It must accept a value matching `^[A-Z]{3}-\d{3}$` and reject anything else with the message `Code must look like ABC-123`.
- `TODO: Ex038 - implement the validation rule`.

**ex039 `Ex039_ValidationMessageDisplay` — shape B.**
- Parameters: `Model`.
- TODO markup comment: an `EditForm` with a `DataAnnotationsValidator`, `<InputText id="name" @bind-Value="Model.Name" />` followed by `<ValidationMessage For="() => Model.Name" />` inside a `<div id="name-field">`, and the same pair for `Age` inside `<div id="age-field">`, plus `#submit`. State that each field's message must appear **inside its own** field div — that is what `For` buys, and it is what the test scopes on.
- Throwing `OnParametersSet`, `TODO: Ex039 - show each field's message next to its field`.

**ex040 `Ex040_EditContextFieldState` — shape B.**
- Parameters: `Model`.
- Public accessor: `public bool ModelIsModified => _context?.IsModified() ?? false;`, with `private EditContext? _context;`.
- TODO markup comment: create the `EditContext` in `OnParametersSet` from `Model`, pass it to the `EditForm` via `EditContext=` (not `Model=`), render `<InputText id="name" @bind-Value="Model.Name" />`, a `<button id="reset" @onclick="Reset" type="button">Reset</button>` that calls `MarkAsUnmodified()`, and `<span id="modified">@ModelIsModified</span>`. State that the rendered input carries the CSS class `modified` once changed — that is `EditContext`'s own field-class behaviour, not something to hand-code.
- Throwing `OnParametersSet`, `TODO: Ex040 - track and reset the field state`.

- [ ] **Step 2: Write the five tests**

**`Ex036_EditFormBasicsTests`** — four facts:
1. Rendering with a fresh `ContactModel` → `Find("form")` resolves, `#name` is an `INPUT`, `#submit` is a `BUTTON`.
2. `cut.Find("form").Submit()` → inside `WaitForAssertion`, `#count` reads `1`.
3. Two submits → `#count` reads `2`.
4. With an `OnAccepted` callback captured into a local, one submit → the local holds the **same instance** as the `Model` parameter (`Assert.Same`).

Non-vacuity: wiring `OnSubmit` instead of `OnValidSubmit` passes all four here — that distinction needs a validator and is ex037's subject; say so in a comment so a later reader does not mistake it for an omission.

**`Ex037_DataAnnotationsValidationTests`** — five facts:
1. `Name` set to `"Ada"`, `Age` `30`, submit → `#counts` reads `1/0`.
2. `Name` left null, submit → `#counts` reads `0/1`.
3. Fact 2's render → `Find(".validation-summary")` (or `#errors` if the stub uses one) contains the text `Name is required`, asserted with `Assert.Contains` over the summary's item texts, **not** over whole markup.
4. `Age` set to `999`, `Name` `"Ada"`, submit → the summary contains `Age must be between 1 and 120` and `#counts` reads `0/1`.
5. Fixing the name after a failed submit and resubmitting → `#counts` reads `1/1` (one of each), proving the validator re-runs rather than caching.

Non-vacuity: omitting `<DataAnnotationsValidator />` makes every submit valid and fails facts 2–5. Wiring only `OnValidSubmit` fails fact 2's `0/1`.

**`Ex038_CustomValidationAttributeTests`** — four facts:
1. `#code` changed to `ABC-123`, submit → `#errors` has no `<li>`.
2. `#code` changed to `abc-123`, submit → `#errors` contains exactly one `<li>` whose text is `Code must look like ABC-123`.
3. `#code` changed to `ABCD-123`, submit → one error with the same message.
4. `#code` left empty, submit → one error with the same message (an empty code is not a valid code).

Non-vacuity: an `IsValid` that returns `true` unconditionally fails 2–4; one that returns `false` unconditionally fails 1; one using `Contains("-")` instead of the pattern fails 3.

**`Ex039_ValidationMessageDisplayTests`** — three facts:
1. Both fields invalid (`Name` null, `Age` `999`), submit → `#name-field .validation-message` text is `Name is required` **and** `#age-field .validation-message` text is `Age must be between 1 and 120`.
2. `Name` null but `Age` valid, submit → `#name-field .validation-message` exists and `FindAll("#age-field .validation-message")` is **empty**.
3. Both valid, submit → both `FindAll` results are empty.

Non-vacuity: a single `<ValidationSummary />` instead of two scoped `<ValidationMessage For=...>` renders both messages in one place and fails fact 2's scoped emptiness — which is the whole point of `For`.

**`Ex040_EditContextFieldStateTests`** — four facts:
1. Initial render → `#modified` reads `False`.
2. `#name` changed to `"Ada"` → inside `WaitForAssertion`, `#modified` reads `True`.
3. Then `#reset` clicked → `#modified` reads `False` again.
4. After fact 2's change, `Find("#name").ClassList` contains `modified`.

Non-vacuity: hand-tracking a `bool` on the change event passes 1–3 and fails 4, because the CSS class comes from `EditContext`'s own field-state machinery. That is the fact that forces the real mechanism.

- [ ] **Step 3: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex036_|FullyQualifiedName~Ex037_|FullyQualifiedName~Ex038_|FullyQualifiedName~Ex039_|FullyQualifiedName~Ex040_"
```

Expected: 0 passed, all failed, each failure that exercise's own `TODO: ExNNN - `. A passing test is a bug in the test; a compile error is a bug in the stub.

- [ ] **Step 4: Write the five solutions**

Points that matter beyond filling in the stub:
- **ex036:** `private Task AcceptAsync() { SubmitCount++; return OnAccepted.InvokeAsync(Model); }`. Do not add `@onsubmit` anything to the `EditForm`.
- **ex037:** both `OnValidSubmit` and `OnInvalidSubmit` wired; `<DataAnnotationsValidator />` before `<ValidationSummary />`.
- **ex038:** `IsValid` returns `ValidationResult.Success` on a match and `new ValidationResult(ErrorMessage)` otherwise; use `Regex.IsMatch` with an anchored pattern. Handle `null` by rejecting.
- **ex039:** two `<ValidationMessage For="() => Model.Name" />` / `For="() => Model.Age"` inside their own field divs.
- **ex040:** build the `EditContext` in `OnParametersSet` (recreating it when `Model` changes), pass `EditContext=`, and let the `InputText` get its `modified` class from the context — do not add the class by hand.

- [ ] **Step 5: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex036_|FullyQualifiedName~Ex037_|FullyQualifiedName~Ex038_|FullyQualifiedName~Ex039_|FullyQualifiedName~Ex040_"
```

Expected: all passed.

- [ ] **Step 6: Non-vacuity round-trips — three, each restored, both outcomes reported**

- Remove `<DataAnnotationsValidator />` from ex037's solution → facts 2–5 must fail.
- Replace ex039's two `<ValidationMessage For=...>` with one `<ValidationSummary />` → fact 2 must fail.
- Replace ex040's `EditContext.IsModified()` with a hand-tracked `bool` set on change → fact 4 must fail while 1–3 still pass.

- [ ] **Step 7: Five host demo pages**

Each needs `@rendermode InteractiveServer` and owns its own `ContactModel` in `@code`. Then:

```bash
cd blazor
dotnet build host/FeWoLearning.Blazor.Host.csproj
dotnet build host/FeWoLearning.Blazor.Host.csproj -p:UseSolutions=true
```

- [ ] **Step 8: Update `catalog.md` and commit**

Flip rows 036–040 to ✅ and set `**Status: 40 ✅ / 60 ⬜**`. **Remove `@onsubmit:preventDefault` from row 036's Concepts cell** per deviation 4, and note in your report that `blazor/README.md` and spec §6 still carry the stale promise — Task 8 removes it.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex036-ex040"
```

---

## Task 3: ex041–ex045 — custom validation and DI

- [ ] **Step 1: Write the five stubs**

**ex041 `Ex041_CustomFieldValidation` — two files, shape B.** A validator component that plugs into an `EditContext`, the way `DataAnnotationsValidator` does.
- `Ex041_CustomFieldValidation.razor`: hosts an `EditForm` with `<Ex041_CustomFieldValidation_Validator />`, `<InputText id="name" @bind-Value="Model.Name" />`, `#submit`, `<ul id="errors">` from `GetValidationMessages()`, and public `ValidCount`/`InvalidCount`. Parameters: `Model`.
- `Ex041_CustomFieldValidation_Validator.razor`: `[CascadingParameter] public EditContext EditContext { get; set; } = default!;`. TODO in `OnInitialized`: create a `ValidationMessageStore`, subscribe to `EditContext.OnValidationRequested`, and on each request clear the store and add the message `Name must not be "admin"` for the `Name` field when it equals `admin` case-insensitively. `TODO: Ex041 - wire a validation message store into the EditContext`.

**ex042 `Ex042_NestedModelValidation` — shape B.**
- Parameters: `Model` (whose `Address` is `[ValidateComplexType]`).
- TODO markup comment: an `EditForm` with `DataAnnotationsValidator`, `<InputText id="name" @bind-Value="Model.Name" />`, `<InputText id="city" @bind-Value="Model.Address.City" />`, `#submit`, `<ul id="errors">`, and public `ValidCount`/`InvalidCount`. State the nested message: `City is required`.
- Throwing `OnParametersSet`, `TODO: Ex042 - validate the nested address too`.

**ex043 `Ex043_ScopedStateContainer` — shape A.** Two sibling components sharing one injected store.
- The stub renders `<Ex043_ScopedStateContainer_Reader />` twice inside a `<div id="both">` plus a `<button id="bump" @onclick="Bump">+</button>`; `[Inject] public CounterStore Store { get; set; } = default!;`.
- TODO: `private void Bump() => throw new NotImplementedException("TODO: Ex043 - advance the shared store");`
- `Ex043_ScopedStateContainer_Reader.razor` is **also an exercise file** (shape A): it injects the store and renders `<span class="reading">@Store.Value</span>`; its TODO is `OnInitialized`, which must subscribe to `Changed` and call `StateHasChanged`, plus `Dispose` unsubscribing. `@implements IDisposable`. `TODO: Ex043 - keep this reader in sync with the store`.

**ex044 `Ex044_SingletonVsScopedState` — shape A.**
- The stub renders `<span id="scoped">@Scoped.Value</span><span id="singleton">@Singleton.Value</span>` with two injected stores distinguished by marker types declared in `@code`: `public sealed class ScopedCounter : CounterStore-like`… **simpler**: inject `CounterStore` for scoped and a `[Inject] public IServiceProvider Services` and resolve via keyed services. **Keep it simple instead:** the stub exposes `public int Reads { get; private set; }` and a TODO `private int Combined => throw ...` that must return `Scoped.Value + Singleton.Value`, with both stores injected by keyed DI (`[Inject(Key = "scoped")]`). If keyed `[Inject]` proves unavailable in this bUnit/.NET combination, **report it and fall back** to two distinct fixture types added to `_support/` in the same commit, and say so — this is the one exercise in the batch whose mechanism has not been probed.
- `TODO: Ex044 - combine the two lifetimes' values`.

**ex045 `Ex045_CascadingServiceInjection` — shape A.**
- The stub renders `<span id="via-property">@_fromProperty</span><span id="via-cascade">@FromCascade?.Value</span>`, with `[Inject] public CounterStore Store { get; set; } = default!;` and `[CascadingParameter] public CounterStore? FromCascade { get; set; }`.
- TODO: `protected override void OnInitialized() => throw new NotImplementedException("TODO: Ex045 - read the store from both injection paths");` — set `_fromProperty` to `Store.Value.ToString()`.
- The lesson: `[Inject]` resolves from DI; `[CascadingParameter]` resolves from an ancestor's `CascadingValue`, and the two are independent. A test renders it once with a cascading value present and once without.

**ex046 is Task 4's.**

- [ ] **Step 2: Write the five tests**

**`Ex041_CustomFieldValidationTests`** — four facts: valid name submits clean (`#errors` empty, counts `1/0`); `admin` submits invalid with exactly one `<li>` reading `Name must not be "admin"`; `ADMIN` likewise (case-insensitive); fixing it and resubmitting clears the error and gives counts `1/1`. Non-vacuity: a validator that never clears its store leaves the stale message and fails the last fact.

**`Ex042_NestedModelValidationTests`** — three facts: name and city both set → `1/0`; city empty → `0/1` with `City is required` present; name empty and city set → `0/1` with `Name is required`. Non-vacuity: dropping `[ValidateComplexType]`'s effect (validating only the root) passes facts 1 and 3 and fails 2.

**`Ex043_ScopedStateContainerTests`** — four facts: `Services.AddScoped<CounterStore>()` **before** rendering; initial render → both `.reading` spans read `0`; one `#bump` click → inside `WaitForAssertion`, **both** read `1`; `store.SubscriberCount` is `2` after render; after `await DisposeComponentsAsync()`, `SubscriberCount` is `0`. Non-vacuity: a reader that does not subscribe leaves the second span stale and fails fact 3; one that never unsubscribes fails fact 4.

**`Ex044_SingletonVsScopedStateTests`** — facts as the chosen mechanism allows; at minimum that the combined value reflects both stores and that advancing only one changes the total by one. Report the mechanism you landed on.

**`Ex045_CascadingServiceInjectionTests`** — three facts: rendered with a `CascadingValue<CounterStore>` wrapping it → both spans read the same value; rendered **without** the cascading value → `#via-property` still reads the injected store's value and `#via-cascade` is empty; advancing the store then re-rendering → `#via-property` is unchanged, because `OnInitialized` captured it once. Non-vacuity: reading the store in `OnParametersSet` instead fails fact 3.

- [ ] **Step 3: Red check, filtered to ex041–ex045.** Same expectations as Task 2 Step 3.

- [ ] **Step 4: Write the solutions.** ex041's validator must clear its `ValidationMessageStore` at the start of each validation request and call `EditContext.NotifyValidationStateChanged()`. ex043's reader stores its handler in a field so subscribe and unsubscribe use the same delegate.

- [ ] **Step 5: Green check, filtered.**

- [ ] **Step 6: Non-vacuity round-trips — three, restored, both outcomes each:** ex041's validator not clearing its store; ex043's reader not unsubscribing; ex045 reading the store in `OnParametersSet`.

- [ ] **Step 7: Five demo pages; build the host in both modes.**

- [ ] **Step 8: Flip rows 041–045, `**Status: 45 ✅ / 55 ⬜**`, commit `blazor: ex041-ex045`.**

---

## Task 4: ex046–ex050 — state notification, options, factory, JS interop

- [ ] **Step 1: Write the five stubs**

**ex046 `Ex046_StateContainerNotification` — shape A.** The subscribe/unsubscribe symmetry exercise, one level up from ex020: the store is injected rather than passed, and the component must survive being re-rendered without double-subscribing. `@implements IDisposable`. TODOs in `OnInitialized` and `Dispose`. `TODO: Ex046 - subscribe once and unsubscribe exactly once`.

**ex047 `Ex047_OptionsPatternComponent` — shape A.** `[Inject] public IOptions<GreetingOptions> Options` where `GreetingOptions` is declared in the stub's `@code` (a plain class with a `string Prefix` property, default `"Hello"`), so the test can register it. Markup renders `<span id="greeting">@Greeting</span>`; TODO is `Greeting`, returning `$"{Options.Value.Prefix}, {Name}!"`. `TODO: Ex047 - build the greeting from the injected options`.

**ex048 `Ex048_FactoryInjectedComponent` — shape A.** `[Inject] public IServiceProvider Services`. TODO: `private CounterStore Resolve() => throw ...` — resolve the store lazily from the provider rather than via `[Inject]`, so a test can prove resolution happens per call. `TODO: Ex048 - resolve the store from the provider`.

**ex049 `Ex049_JsInteropInvoke` — shape A.** `[Inject] public IJSRuntime JS`. Markup: `<button id="save" @onclick="SaveAsync">Save</button><span id="state">@_state</span>`. TODO: `SaveAsync` must call `JS.InvokeVoidAsync("app.save", Payload)` and then set `_state` to `"saved"`. `[Parameter] public string Payload { get; set; } = "";`. `TODO: Ex049 - call the JS function, then record the result`.

**ex050 `Ex050_JsInteropReturnValue` — shape A.** As ex049 but `InvokeAsync<string>("app.load")`, with the returned value rendered in `#loaded`. `TODO: Ex050 - return what JS gave you`.

- [ ] **Step 2: Write the five tests**

**ex046** — four facts mirroring ex043's shape but on one component: subscriber count `1` after render; a store change updates the markup; a `cut.Render(...)` parameter push does **not** raise the count to `2`; after disposal the count is `0`. Non-vacuity: subscribing in `OnParametersSet` instead of `OnInitialized` fails fact 3.

**ex047** — three facts: with `Services.AddSingleton<IOptions<GreetingOptions>>(Microsoft.Extensions.Options.Options.Create(new GreetingOptions { Prefix = "Moin" }))` registered before rendering, `#greeting` reads `Moin, Ada!`; with the default options object, it reads `Hello, Ada!`; changing `Name` via `cut.Render` updates it. Non-vacuity: hard-coding `"Hello"` fails fact 1.

**ex048** — two facts: `#value` reflects the resolved store; after the store is advanced, a re-render reflects the new value, proving resolution is not cached from `OnInitialized`. Non-vacuity: caching the resolved instance in a field set in `OnInitialized` still passes both if the store is a singleton — so register it **scoped** and have the fact advance the same instance; state that in the test comment.

**ex049** — three facts, using the measured API: `JSInterop.SetupVoid("app.save", "hi")` before rendering; click `#save` → `WaitForAssertion` sees `#state` read `saved`; `JSInterop.VerifyInvoke("app.save")` records exactly one invocation whose single argument is `"hi"`. Non-vacuity: setting `_state` without calling JS fails the `VerifyInvoke`; calling JS without awaiting it fails the `#state` assertion under strict mode.

**ex050** — three facts: `JSInterop.Setup<string>("app.load").SetResult("payload")`; render → `#loaded` reads `payload`; `VerifyInvoke("app.load")` once. Non-vacuity: returning a constant fails fact 2 once the setup's result is changed — add a second fact that re-renders with a different `SetResult` to pin it.

- [ ] **Step 3: Red check, filtered.**
- [ ] **Step 4: Solutions.** ex049/ex050 must `await` the interop call before touching state.
- [ ] **Step 5: Green check, filtered.**
- [ ] **Step 6: Round-trips — three, restored, reported:** ex046 subscribing in `OnParametersSet`; ex049 skipping the JS call; ex050 returning a constant.
- [ ] **Step 7: Five demo pages. The JS interop pages need a real `app.save`/`app.load` in the host** — add a tiny `<script>` to the demo page itself rather than a shared file, so the exercise's host page is self-contained. Build both modes.
- [ ] **Step 8: Flip rows 046–050, `**Status: 50 ✅ / 50 ⬜**`, commit `blazor: ex046-ex050`.**

---

## Task 5: ex051–ex055 — JS interop depth, navigation, persistent state

**Interfaces consumed:** `Support.SeededStore` (Task 1) for ex059–060, not this batch. This batch introduces no fixtures.

- [ ] **Step 1: Write the five stubs**

**ex051 `Ex051_JsInteropElementReference` — shape A.** Captures an element with `@ref` and hands it to JS. Markup: `<input id="target" @ref="_target" /><button id="focus" @onclick="FocusAsync">Focus</button>`. TODO: `FocusAsync` must call `JS.InvokeVoidAsync("app.focus", _target)`. `TODO: Ex051 - pass the captured element to JS`.

**ex052 `Ex052_JsInteropModule` — shape A.** `@implements IAsyncDisposable`. TODOs: `OnAfterRenderAsync(bool firstRender)` importing a module with `JS.InvokeAsync<IJSObjectReference>("import", "./app.js")` on first render only, and `DisposeAsync` disposing it. Markup renders `<span id="ready">@(_module is null ? "no" : "yes")</span>`. Two TODOs, both `TODO: Ex052 - `.

**ex053 `Ex053_JsInteropUnmatchedInvocation` — shape A.** The exercise is about the mock's strictness, so the *test* carries the lesson and the stub is minimal: a button that calls `JS.InvokeVoidAsync("app.unplanned")`. TODO is the handler. `TODO: Ex053 - make the unplanned call`.

**ex054 `Ex054_NavigationManagerBasics` — shape A.** `[Inject] public NavigationManager Navigation`. Markup: `<span id="start">@_start</span><button id="go" @onclick="Go">go</button>`. TODOs: `OnInitialized` capturing `Navigation.Uri` into `_start`, and `Go()` navigating to `[Parameter] public string Target { get; set; } = "";`. Two TODOs.

**ex055 `Ex055_NavigationLocationChanged` — shape A.** `@implements IDisposable`. Public `public int LocationChanges { get; private set; }`. TODOs in `OnInitialized` (subscribe to `Navigation.LocationChanged`) and `Dispose` (unsubscribe). Markup `<span id="changes">@LocationChanges</span>`.

- [ ] **Step 2: Write the five tests**

**ex051** — two facts: `JSInterop.SetupVoid("app.focus", _ => true)`; click `#focus` → `VerifyInvoke("app.focus")` once, and the recorded argument `ShouldBeElementReferenceTo(cut.Find("#target"))` — bUnit ships that assertion, which is what makes this exercise honest rather than a string comparison. Non-vacuity: passing a selector string instead of the `ElementReference` fails the reference assertion.

**ex052** — three facts: `JSInterop.SetupModule("./app.js")`; after render `#ready` reads `yes`; the module setup records the import; after `await DisposeComponentsAsync()` the module's `IsDisposed` is true (bUnit's module handler exposes this). Non-vacuity: importing on every render rather than only the first fails a fact asserting exactly one import after a parameter push.

**ex053** — two facts: in the default **strict** mode, clicking the button makes the render throw a bUnit `JSRuntimeUnhandledInvocationException`, asserted with `Assert.Throws`; after `JSInterop.Mode = JSRuntimeMode.Loose`, the same click completes and `VerifyInvoke` still records it. This exercise's value is that the learner sees the mock's two modes; state in the stub that the *test* is the lesson.

**ex054** — three facts: `#start` is non-empty after render; clicking `#go` with `Target="/somewhere?x=1"` → `Services.GetRequiredService<NavigationManager>().Uri` contains `/somewhere?x=1`; `#start` still shows the original URI afterwards, proving it was captured in `OnInitialized`. Remember `using Microsoft.Extensions.DependencyInjection;`.

**ex055** — three facts: `#changes` reads `0` initially; a `NavigateTo` from the test → inside `WaitForAssertion`, `#changes` reads `1`; after `await DisposeComponentsAsync()`, navigating again does not throw and the handler is gone — assert by navigating twice more and confirming the component's last observed count stayed at `1` before disposal. Non-vacuity: never unsubscribing leaves the handler live; the cleanest fact is to capture the instance before disposal and assert its counter does not advance after.

- [ ] **Step 3: Red check, filtered.**
- [ ] **Step 4: Solutions.** ex052 imports only when `firstRender`; ex055 stores its handler in a field.
- [ ] **Step 5: Green check, filtered.**
- [ ] **Step 6: Round-trips — three, restored, reported:** ex051 passing a string; ex052 importing on every render; ex055 not unsubscribing.
- [ ] **Step 7: Five demo pages, with real `app.focus`/`app.js` scripts on the pages that need them. Build both modes.**
- [ ] **Step 8: Flip rows 051–055, `**Status: 55 ✅ / 45 ⬜**`, commit `blazor: ex051-ex055`.**

---

## Task 6: ex056–ex060 — query strings, navigation interception, bind modifiers, persistent state

**Interfaces consumed:** `Support.SeededStore` (Task 1).

- [ ] **Step 1: Write the five stubs**

**ex056 `Ex056_QueryStringParsing` — shape A.** TODO: a member returning the value of a named query parameter from `Navigation.Uri`, using `QueryHelpers.ParseQuery(new Uri(Navigation.Uri).Query)`. Markup renders `<span id="q">@QueryValue</span>`. `[Parameter] public string Key { get; set; } = "q";` `TODO: Ex056 - read the query parameter`.

**ex057 `Ex057_NavigationInterception` — shape A.** `@implements IDisposable`. TODOs: `OnInitialized` registering a location-changing handler with `Navigation.RegisterLocationChangingHandler`, cancelling the navigation when `[Parameter] public bool Block { get; set; }` is true; and `Dispose` disposing the registration. Public `public int Attempts { get; private set; }`.

**ex058 `Ex058_BindDirectiveModifiers` — shape B.** Three inputs on local fields, each with a different modifier: `@bind:event="oninput"` on `#live`, `@bind:format="yyyy-MM-dd"` on a `DateOnly` field `#date`, and `@bind:after="AfterAsync"` on `#tracked` with a public `public int AfterCount { get; private set; }`. State all three exact behaviours (rule 9). Throwing `OnParametersSet`, `TODO: Ex058 - wire the three bind modifiers`.

**ex059 `Ex059_PersistComponentStateBasics` — shape A.** `[Inject] public PersistentComponentState State`. TODOs: `OnInitialized` restoring with `State.TryTakeFromJson<string>("ex059", out var v)` and falling back to `"fresh"`, plus registering a persist callback with `State.RegisterOnPersisting`. `@implements IDisposable` disposing the subscription. Markup `<span id="value">@_value</span>`.

**ex060 `Ex060_PersistentStateAttribute` — shape A.** The declarative counterpart: a `[PersistentState] public string? Saved { get; set; }` property, with the TODO being a member that renders `Saved ?? "none"`. **This mechanism was not probed;** if `[PersistentState]` does not resolve under bUnit, report it and fall back to a round-trip exercise on the imperative API (persist then restore through `SeededStore`), saying so.

- [ ] **Step 2: Write the five tests**

**ex056** — three facts, each rendering after the test navigates to a URI with a query string: `?q=blazor` → `#q` reads `blazor`; `?q=` → empty; no query at all → empty. Non-vacuity: naive `Uri.Split('=')` fails a fact with two parameters (`?a=1&q=2`) — add it.

**ex057** — three facts: with `Block=false`, a `NavigateTo` completes and the URI changes; with `Block=true`, the URI does **not** change; after disposal with `Block=true`, navigation completes again, proving the registration was disposed.

**ex058** — five facts: `#live` `.Input("ab")` updates its echo immediately; `#live` `.Change("cd")` does not (the modifier moved the trigger); `#date`'s rendered `value` attribute is `2026-09-04` for that date; `#tracked` `.Change("x")` raises `AfterCount` to `1`; a second change raises it to `2`.

**ex059** — three facts: with **no** seeded state, `#value` reads `fresh`; with `SeededStore` seeded with `"ex059"` → `"restored"`, `#value` reads `restored`; after `manager.PersistStateAsync(store, renderer)`… **simpler and sufficient**: assert `store.Persisted` contains the `"ex059"` key after the manager persists, proving the callback was registered. Non-vacuity: not registering the persist callback fails that fact; not restoring fails fact 2.

**ex060** — facts as the chosen mechanism allows; report which you used.

- [ ] **Step 3–8:** as the previous batches — red check, solutions, green check, **three round-trips** (ex056 with a naive split; ex057 not disposing the registration; ex059 not registering the persist callback), five demo pages, both host builds, flip rows 056–060 to `**Status: 60 ✅ / 40 ⬜**`, commit `blazor: ex056-ex060`.

---

## Task 7: ex061–ex065 — refs, async lifecycle, cancellation

- [ ] **Step 1: Write the five stubs**

**ex061 `Ex061_RefCaptureBasics` — two files, shape A.** The parent captures both an element (`@ref` on an `<input id="box" />`) and a **component** reference to `Ex061_RefCaptureBasics_Child`, then calls a method on the child instance. The child exposes `public void Bump()` and renders `<span id="child">@_n</span>`. Parent TODO: a `Call()` handler invoking `_child?.Bump()`. Child TODO: `Bump` itself.

**ex062 `Ex062_AsyncOnInitialized` — shape B.** `[Parameter] public Task<string> Source { get; set; } = default!;` — the measured pattern. TODO markup comment: render `<p id="loading">loading</p>` while the data is null and `<p id="data">@_data</p>` once it arrives; the TODO throw goes in `OnInitializedAsync`. State that the loading frame must be observable, which is what the test asserts.

**ex063 `Ex063_CancellationOnDispose` — shape A.** `@implements IDisposable`. Public `public bool WasCancelled { get; private set; }`. TODOs: `OnInitialized` creating a `CancellationTokenSource` and starting an awaited operation that observes the token, and `Dispose` cancelling and disposing it.

**ex064 `Ex064_SetParametersAsyncOverride` — shape A.** Public `public int SetParametersCalls { get; private set; }`. TODO: `public override Task SetParametersAsync(ParameterView parameters)` incrementing the counter and **calling `base.SetParametersAsync(parameters)`**, which is the whole lesson — forgetting it means parameters never reach the component.

**ex065 `Ex065_DebouncedAsyncSearch` — shape A.** `[Parameter] public Func<string, CancellationToken, Task<string>> Search { get; set; } = default!;` so a test can drive it deterministically. TODO: an input handler that debounces with `Task.Delay` and cancels a superseded request. Public `public int SearchCalls { get; private set; }`.

- [ ] **Step 2: Write the five tests**

**ex061** — three facts: the child renders `0`; clicking `#call` raises `#child` to `1`; `cut.FindComponents<Ex061_RefCaptureBasics_Child>()` has exactly one instance and the parent's element ref points at `#box` (assert via a JS interop `ShouldBeElementReferenceTo` if the parent hands it to JS, otherwise assert the component-ref behaviour only and say so).

**ex062** — three facts using a `TaskCompletionSource`: before completion, `#loading` is present and `FindAll("#data")` is empty; after `tcs.SetResult("arrived")`, inside `WaitForAssertion`, `#data` reads `arrived` and `FindAll("#loading")` is empty; a faulted source surfaces as a thrown render rather than a silent empty frame. Non-vacuity: awaiting in `OnParametersSetAsync` instead passes facts 1–2 — say so in a comment rather than pretending otherwise.

**ex063** — three facts: after render `WasCancelled` is false; after `await DisposeComponentsAsync()`, the captured instance's `WasCancelled` is true; the token source is disposed (a second dispose does not throw). Non-vacuity: cancelling without disposing, or disposing without cancelling, each fails one fact — write both round-trips.

**ex064** — three facts: after render `SetParametersCalls` is `1`; after a `cut.Render` push it is `2`; the pushed parameter value actually reached the component's markup, which is what proves `base` was called. Non-vacuity: omitting the `base` call passes facts 1–2 and fails 3 — that is the exercise.

**ex065** — four facts driven by the injected `Search` delegate: typing once and letting the debounce elapse calls `Search` exactly once; typing twice quickly calls it once with the **second** value; the superseded call's token is cancelled; the result reaches `#result`. Use `cut.WaitForAssertion` with the default timeout rather than wall-clock sleeps, and keep the debounce interval small (say 50 ms) so the suite stays fast — state the interval in the stub.

- [ ] **Step 3–8:** red check; solutions (ex064's must call `base`; ex065 must cancel the superseded token); green check; **four round-trips** (ex063 cancel-without-dispose and dispose-without-cancel; ex064 omitting `base`; ex065 not cancelling the superseded request); five demo pages; both host builds; flip rows 061–065 to `**Status: 65 ✅ / 35 ⬜**`; commit `blazor: ex061-ex065`.

---

## Task 8: ex066–ex070 — error boundaries and generics

- [ ] **Step 1: Write the five stubs**

**ex066 `Ex066_ErrorBoundaryBasics` — two files, shape B.** The measured mechanism: a child throwing from `OnParametersSet` is caught and `<ErrorContent>` renders.
- `Ex066_ErrorBoundaryBasics.razor`: wraps `<Ex066_ErrorBoundaryBasics_Fragile Explode="@Explode" />` in an `<ErrorBoundary @ref="_boundary">` with an `<ErrorContent>` rendering `<p id="fallback">something went wrong</p>` and a `<button id="recover" @onclick="Recover">Recover</button>` calling `_boundary?.Recover()`. `[Parameter] public bool Explode { get; set; }`.
- `Ex066_ErrorBoundaryBasics_Fragile.razor` is also an exercise: its TODO throws when `Explode` is true and renders `<p id="ok">fine</p>` otherwise.

**ex067 `Ex067_ErrorBoundaryLoggingHandler` — two files, shape A.** A custom `ErrorBoundary` subclass overriding `OnErrorAsync` to record the exception message into a public `public string? LastError { get; private set; }`. The subclass is the exercise; the fragile child comes from `_support/` — **add `_support/Fragile.razor`** in this batch, byte-identical in both RCLs, so ex067's test does not depend on ex066's exercise file.

**ex068 `Ex068_GenericTypeInference` — shape B.** Re-specced per Global Constraints deviation 3. `@typeparam TItem`. Parameters: `[Parameter] public IReadOnlyList<TItem> Items { get; set; } = [];`, `[Parameter] public TItem? Fallback { get; set; }`. TODO markup comment: render `<span id="first">` holding the first item's `ToString()` or the fallback's when the list is empty, and `<span id="type">` holding `typeof(TItem).Name`. The lesson: `TItem` is inferred from `Items` when it is non-empty and typed, and must be given explicitly as `TItem="..."` when inference cannot see it.

**ex069 `Ex069_GenericConstraintComponent` — shape A.** `@typeparam T where T : IComparable<T>`. Markup renders `<span id="max">@Largest</span>`; TODO is `Largest`, returning the maximum of `[Parameter] public IReadOnlyList<T> Values { get; set; } = [];` or the empty string when empty. The constraint is what makes `CompareTo` available.

**ex070 `Ex070_GenericContextNaming` — shape B.** Re-specced per deviation 3. Two nested templated regions whose implicit `@context` would collide: `[Parameter] public RenderFragment<string>? Outer { get; set; }` and `[Parameter] public RenderFragment<int>? Inner { get; set; }`, rendered nested inside `<div id="nest">`. TODO markup comment: render `Outer` for each of `[Parameter] public IReadOnlyList<string> Groups`, and inside each, `Inner` for each of `[Parameter] public IReadOnlyList<int> Numbers`, naming the contexts explicitly so both are reachable. State that an implicit `@context` in both positions is a compile error and that `Context="g"` / `Context="n"` is the fix.

- [ ] **Step 2: Write the five tests**

**ex066** — four facts: `Explode=false` → `#ok` present, `FindAll("#fallback")` empty; `Explode=true` → `#fallback` present, `FindAll("#ok")` empty; after a `cut.Render` back to `Explode=false` the boundary is **still** showing the fallback (an `ErrorBoundary` latches); clicking `#recover` then shows `#ok` again. That third fact is the one that teaches what `Recover()` is for — do not omit it.

**ex067** — three facts: no error → `LastError` is null; an error → `LastError` holds the thrown message; the boundary still renders its `ErrorContent`. Non-vacuity: an override that does not call the recording path leaves `LastError` null.

**ex068** — four facts: `Items=["a","b"]` with `TItem` inferred → `#first` reads `a` and `#type` reads `String`; empty `Items` with `Fallback="z"` → `#first` reads `z`; `Render<Ex068_GenericTypeInference<int>>` with `Items=[7,8]` → `#first` reads `7` and `#type` reads `Int32`; empty `Items` and no `Fallback` for a reference type → `#first` is empty rather than throwing.

**ex069** — four facts over `T = int` and `T = string`: `[3,9,4]` → `9`; `["pear","apple"]` → `pear`; single element → that element; empty → empty string. Non-vacuity: using `Max()` on `IEnumerable<T>` without the constraint would not compile, which is the point; a solution that sorts and takes the last also passes, and that is fine.

**ex070** — three facts: two groups × three numbers renders six innermost cells with the expected text pairs; the outer context's value is available inside the inner template (assert a cell whose text combines both, e.g. `a-2`); an empty `Numbers` renders the groups with no cells. Non-vacuity: rendering only the inner template, or shadowing the outer context, fails the combined-text fact.

- [ ] **Step 3–8:** red check; solutions; green check; **three round-trips** (ex066 without the latch/recover behaviour — i.e. a solution that re-renders the child on the next parameter change instead of latching, which fact 3 must reject; ex067 not recording; ex070 shadowing the outer context); five demo pages, with ex068's and ex069's closing their generics explicitly; both host builds; flip rows 066–070 to `**Status: 70 ✅ / 30 ⬜**`; commit `blazor: ex066-ex070`.

---

## Task 9: Tier verification and documentation

- [ ] **Step 1: Full-suite red check.** `dotnet test` from inside `blazor/`. Expect the Beginner tier's 115 plus this tier's facts, **0 passed, all failed**, each failure its own `TODO: ExNNN - `. Capture the summary line verbatim.

- [ ] **Step 2: Full-suite green check.** `dotnet test -p:UseSolutions=true`. Expect all passed, 0 failed. Capture it verbatim.

- [ ] **Step 3: Warning audit.** Report the stub build's warning count and list every field it names. The count will exceed six now; that is expected. **Suppress nothing.** The solutions build must still be 0 warnings.

- [ ] **Step 4: Host HTTP spot-check, both modes.** Start the host, wait for the port rather than sleeping, fetch three Intermediate demo pages in solutions mode (expect 200 with the expected rendered text) and one in exercises mode (expect the exercise's `NotImplementedException` to surface). Stop it. The host listens on `http://localhost:5199`.

- [ ] **Step 5: Update `blazor/README.md`.** Add the Intermediate tier's specifics: the `_support/` fixtures this tier added and what each is for; the `PersistentComponentState` registration recipe from Global Constraints (it is not obvious and a future author will need it); bUnit's `JSInterop` strict-versus-loose modes; and the corrected `preventDefault` reasoning per deviation 4 — **remove the stale promise that it moves to the intermediate tier** and replace it with the measured finding. Update the tier's exercise count.

- [ ] **Step 6: Update `CLAUDE.md`.** `blazor/` rows only — the current-state count from 35/100 to 70/100 and the remaining count. **Leave `php/`, `uno/` and `avalonia/` rows exactly as you find them**, and re-read the file immediately before editing it: other sessions write to it.

- [ ] **Step 7: Reconcile the spec.** Update §6 to record deviations 1–4 from Global Constraints, and add the measured mechanism notes (the `PersistentComponentState` recipe, the async-lifecycle `TaskCompletionSource` pattern, the `NavigationManager` DI import) to §2 alongside the Beginner tier's two hard constraints, since they are the same class of finding.

- [ ] **Step 8: Final verification, then commit.** Re-run both gates and confirm the numbers match Steps 1 and 2. Stage explicit paths. Report the exact counts; do not claim completion without them.

---

## Self-Review

**Spec coverage.** §6's intermediate theme list maps to the batches as: `EditForm`/validation → Tasks 2 and 3 (ex036–042); DI and scoped state containers → Task 3 and 4 (ex043–048); `IJSRuntime` mocked → Tasks 4 and 5 (ex049–053); `NavigationManager` → Tasks 5 and 6 (ex054–057); `PersistentComponentState` → Task 6 (ex059–060); async lifecycle and cancellation → Task 7 (ex062–065); `ErrorBoundary` → Task 8 (ex066–067); generic components → Task 8 (ex068–070). ex058 (`@bind` modifiers) and ex061 (`@ref`) come from the Beginner tier's final review, which found both absent from all 100 rows.

**Placeholder scan.** Three exercises have a mechanism this plan has **not** probed and each says so explicitly, with a named fallback and a requirement to report which was used: ex044 (keyed `[Inject]`), ex060 (`[PersistentState]`), and ex052's module-handler `IsDisposed` assertion. Every other exercise's mechanism was measured. That is a deliberate, flagged gap rather than a placeholder — but expect at least one of the three to need a ruling mid-task, as happened in every batch of the Beginner tier.

**Type consistency.** `ContactModel.Name`/`.Age`/`.Address` and `AddressModel.City` — produced in Task 1, consumed by ex036–042. `SeededStore(IDictionary<string, byte[]>)` with a public `Persisted` — produced in Task 1, consumed by ex059–060. `CounterStore.Value`/`.Changed`/`.Increment()`/`.SubscriberCount` — produced in Task 1, consumed by ex043–046 and ex048. `Support.Fragile` — produced in Task 8 Step 1, consumed by ex067. Public members that tests read off `cut.Instance` (`SubmitCount`, `ValidCount`/`InvalidCount`, `ModelIsModified`, `LocationChanges`, `AfterCount`, `LastError`, `WasCancelled`, `SetParametersCalls`, `SearchCalls`, `Attempts`) are declared `public` in their stubs, so the tests compile against the stub and the red run reports failures rather than compile errors.

**Known risk carried from the Beginner tier.** Its reviews found six real defects, five of them in the plan rather than the implementation — vacuous culture facts, a fixture that reported success independently of its mechanism, a `Contains` assertion that accepted a trailing separator, an `object`-typed member that compiled but never rendered, and a registration pattern that was init-only. Every batch below therefore names its non-vacuity round-trips explicitly, and every reviewer should be asked to run the mutation rather than accept the reasoning.
