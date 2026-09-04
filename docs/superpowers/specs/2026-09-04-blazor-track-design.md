# Blazor Track — Design

**Date:** 2026-09-04
**Status:** approved, ready for implementation planning
**Scope of first delivery:** scaffolding + 100-row `catalog.md` + the complete
Beginner tier (ex001–ex035), each exercise verified red and green.

## 1. Purpose

Add a new track, `blazor/`, to the FeWoLearning monorepo: 100 graded exercises
on Microsoft's Blazor component model, following the repo's universal exercise
pattern (stub + failing test + reference solution + catalog ledger).

The owner is a senior .NET architect. "Beginner" in this track therefore means
**Blazor beginner, not C# beginner**: ex001 is a component with a `[Parameter]`,
not a `FizzBuzz` static method. No exercise drills plain C# language features —
those belong to the existing `dotnet/` track.

## 2. Toolchain (verified 2026-09-04)

Confirmed by a throwaway probe in the scratchpad, not assumed:

- .NET SDK **10.0.400** installed (`dotnet --list-sdks`); `net10.0` target.
- **bUnit 2.9.0** from nuget.org, combined with **xunit 2.9.3** +
  **xunit.runner.visualstudio 3.1.4** + **Microsoft.NET.Test.Sdk 17.14.1** —
  the same xunit stack the `dotnet/` track already uses. This combination was
  run end to end and reported one passing test.
- nuget.org is reachable from this machine.

Consequence: unlike `java/`, `kotlin/`, `flutter/` and `php/`, this track is
**verifiable from day one** and must never be committed in an unverified state.

### 2.1 Two hard constraints discovered by the probe

1. **`throw` is illegal in Razor markup** (`CS8115`: a throw expression is not
   allowed in this context). A stub therefore cannot put its TODO inline in
   markup. It throws from the `@code` block instead — markup renders `@Greeting`
   while `Greeting` itself is the TODO:

       <p id="greeting">@Greeting</p>
       @code {
           [Parameter] public string Name { get; set; } = "world";

           // TODO: return $"Hello, {Name}!"
           private string Greeting => throw new NotImplementedException("TODO: Ex001");
       }

   This preserves the repo invariant exactly: the project compiles while the
   exercise is unfinished, and fails at runtime.

2. **bUnit 2.x renamed `TestContext` to `BunitContext`.** Test classes derive
   from `Bunit.BunitContext`; using `TestContext` yields `CS0104` because it is
   ambiguous with `Xunit.TestContext`. Additionally a Razor Class Library needs
   a `FrameworkReference` to `Microsoft.AspNetCore.App`, or the Razor source
   generator cannot resolve `Microsoft.AspNetCore.Components` and every
   `.razor` file fails with `CS0234`.

## 3. Project structure

    blazor/
      FeWoLearning.Blazor.slnx
      Directory.Build.props                                # the UseSolutions switch
      exercises/  FeWoLearning.Blazor.Exercises.csproj     # Razor Class Library, stubs
      solutions/  FeWoLearning.Blazor.Solutions.csproj     # RCL, same namespaces/type names
      tests/      FeWoLearning.Blazor.Tests.csproj         # bUnit + xunit
      host/       FeWoLearning.Blazor.Host.csproj          # Blazor Web App, InteractiveServer
      catalog.md
      README.md

Per-exercise file layout (tier namespaces carry no digit, as in `dotnet/`,
because C# identifiers cannot start with one):

    exercises/01-beginner/Ex001_HelloComponent.razor   -> FeWoLearning.Blazor.Exercises.Beginner
    tests/01-beginner/Ex001_HelloComponentTests.cs     -> FeWoLearning.Blazor.Tests.Beginner
    solutions/01-beginner/Ex001_HelloComponent.razor   -> FeWoLearning.Blazor.Exercises.Beginner
    host/Components/Demos/Beginner/Ex001.razor         -> @page "/beginner/001"

Namespaces per tier: `FeWoLearning.Blazor.Exercises.Beginner`, `.Intermediate`,
`.Advanced`, `.Expert`. The `solutions/` RCL deliberately reuses the exercises'
namespaces and type names; because it is a separate assembly and no single
project references both RCLs, there is no collision.

## 4. The red/green mechanism

`tests/` and `host/` each reference **either** `exercises/` **or** `solutions/`,
never both, selected by the `UseSolutions` property. In each of those two
`.csproj` files:

    <ItemGroup Condition="'$(UseSolutions)' != 'true'">
      <ProjectReference Include="..\exercises\FeWoLearning.Blazor.Exercises.csproj" />
    </ItemGroup>
    <ItemGroup Condition="'$(UseSolutions)' == 'true'">
      <ProjectReference Include="..\solutions\FeWoLearning.Blazor.Solutions.csproj" />
    </ItemGroup>

`Directory.Build.props` redirects the solutions build to a separate output tree,
so stub and solution binaries cannot contaminate each other:

    <Project>
      <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
        <UseArtifactsOutput>true</UseArtifactsOutput>
        <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
      </PropertyGroup>
    </Project>

This redirection is **required, not cosmetic**. Setting
`BaseOutputPath`/`BaseIntermediateOutputPath` conditionally inside the `.csproj`
body was measured to fail: those properties are read before the SDK props are
imported, and the stale default `obj/` is then globbed alongside the new one,
producing `CS0579` duplicate-attribute errors. `UseArtifactsOutput` in
`Directory.Build.props` was verified to work across a red-green-red cycle with
no stale-binary contamination.

`artifacts-solutions/` must be added to `.gitignore`.

### 4.1 Commands (run from inside `blazor/`)

| Command | Effect |
|---|---|
| `dotnet test` | stubs — **all red** |
| `dotnet test -p:UseSolutions=true` | reference solutions — **all green** |
| `dotnet test --filter FullyQualifiedName~Ex001_` | one exercise |
| `dotnet run --project host` | exercise host: stub demo pages, hot reload, error page until implemented |
| `dotnet run --project host -p:UseSolutions=true` | reference host: the same demo pages backed by the solutions |

The demo pages under `host/Components/Demos/` are written once and serve both
modes, because they resolve component types by name against whichever RCL is
referenced.

## 5. Deliberate deviation from the repo convention

`CLAUDE.md` states that `solutions/` is kept out of every build because it
reuses the stubs' names and namespaces. This track keeps `solutions/` **in** the
solution as its own project.

The reason the convention exists — the name collision — cannot occur here, since
no project references both RCLs at once. The benefit is that reference solutions
are **compile-checked on every build**, which eliminates for this track the
entire "Known gaps" failure class: silent solution drift, which the 2026-08-03
audit found had produced five broken solutions in `vue/` and four defective
tests in `go/`.

This deviation must be recorded in `CLAUDE.md` and in `blazor/README.md` so a
future reader does not "fix" it back.

## 6. Content plan

### Tier themes

- **01-beginner (001–035)** — component fundamentals: `[Parameter]`, rendering
  directives, `@bind`, `EventCallback`, `RenderFragment`, lifecycle,
  `CascadingValue`.
- **02-intermediate (036–070)** — `EditForm` and validation, DI and scoped state
  containers, `IJSRuntime` (mocked), `NavigationManager`,
  `PersistentComponentState`, async lifecycle and cancellation, `ErrorBoundary`,
  generic components.
- **03-advanced (071–090)** — render performance (`ShouldRender`, `@key`
  diffing), `Virtualize` with `ItemsProvider`, custom `InputBase<T>`
  derivatives, custom validators, `DynamicComponent`,
  `CascadingAuthenticationState`, `IHandleEvent`/`IHandleAfterRender`,
  `RenderMode` semantics.
- **04-expert (091–100)** — hand-written `RenderTreeBuilder`, custom
  `ComponentBase` base classes, custom routing/`Router` behaviour,
  `RenderFragment` composition in code, diff-algorithm understanding,
  streaming-SSR semantics.

### Beginner slugs (this delivery)

    001 HelloComponent          013 TemplatedFragment       025 SelectBinding
    002 ParameterDefaults       014 AttributeSplatting      026 CheckboxGroup
    003 ConditionalRendering    015 DynamicCssClass         027 RadioGroup
    004 ListRendering           016 InlineStyleBinding      028 CascadingValueBasics
    005 KeyedListDiffing        017 OnInitialized           029 NamedCascadingValue
    006 ClickEventCallback      018 OnParametersSet         030 ComponentComposition
    007 CounterState            019 OnAfterRenderFirst      031 ChildToParentCallback
    008 TwoWayBinding           020 DisposableComponent     032 MarkupStringRendering
    009 BindFormat              021 EventArgsHandling       033 EmptyStateFallback
    010 BindEventOnInput        022 PreventDefault          034 NestedParameterFlow
    011 ChildContent            023 InputTextBinding        035 TabsComposition
    012 NamedFragments          024 NumericInputParsing

### Non-goals

These are **not** in the catalog, because they cannot be tested honestly
headless: real WebAssembly loading, real SignalR circuit reconnects, real
`focus()`/`scrollIntoView` behaviour. Where an exercise needs a JS call, its
test asserts against bUnit's `JSInterop` mock — which invocation happened with
which arguments — never against browser behaviour. `blazor/README.md` states
this explicitly so nobody later mistakes a green test for proof of browser
behaviour.

## 7. Test-quality rules

Checked for every exercise, as the Blazor analogue of the Python and Kotlin
traps already documented in `CLAUDE.md`:

- A bUnit test that does not wait for a re-render after a state change asserts
  only on the **first** frame. For `@onclick` and async lifecycle, use
  `cut.WaitForAssertion(...)` / `InvokeAsync`, never a bare assertion.
- A test that compares `cut.Markup` against a whole string breaks on any
  whitespace change and proves nothing about behaviour. Assert through
  `Find`/`FindAll` plus `TextContent` or a specific attribute.
- Before accepting a red run, ask: **would a naive or wrong implementation also
  pass this test?** If yes, the test is defective.
- Confirm each red failure comes from the `NotImplementedException`, not from a
  compile or resolution error. A stub that fails to build is a bug.

## 8. Definition of done (first delivery)

1. `blazor/` scaffolding builds: `dotnet build` clean for all four projects.
2. `catalog.md` has all 100 rows; 001–035 marked done, 036–100 planned, with a
   matching `**Status:**` line.
3. `dotnet test` — 35 exercise tests, **all red**, every failure traced to its
   `NotImplementedException`.
4. `dotnet test -p:UseSolutions=true` — the same 35 tests, **all green**.
5. `dotnet run --project host` serves the 35 demo pages in both modes.
6. `README.md` documents setup, both commands, the deviation of section 5, and
   the non-goals of section 6.
7. `CLAUDE.md` updated: the track table, the per-track command table, the
   toolchain status, the track-specific gotchas of section 2.1, and the
   deviation of section 5. (`php/` is also missing from `CLAUDE.md`; that is a
   separate omission and out of scope here.)
8. One commit per batch of five, `blazor: exNNN-exNNN`, staging explicit paths.
