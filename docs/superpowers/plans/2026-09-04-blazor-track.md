# Blazor Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a `blazor/` track to the FeWoLearning monorepo with working scaffolding, a 100-row catalog ledger, and the complete Beginner tier (ex001–ex035) as stub + bUnit test + reference solution + host demo page, every exercise verified red and green.

**Architecture:** Four projects under `blazor/`: two Razor Class Libraries with identical namespaces and type names (`exercises/` holding stubs, `solutions/` holding reference implementations), one bUnit test project, and one Blazor Web App host. `tests/` and `host/` each reference exactly one of the two RCLs, chosen by the `UseSolutions` MSBuild property, so a single set of test files and demo pages produces both the red run (stubs) and the green run (solutions).

**Tech Stack:** .NET 10 (`net10.0`, SDK 10.0.400) · Razor Class Library (`Microsoft.NET.Sdk.Razor`) · bUnit 2.9.0 · xunit 2.9.3 + xunit.runner.visualstudio 3.1.4 + Microsoft.NET.Test.Sdk 17.14.1 · Blazor Web App (`Microsoft.NET.Sdk.Web`, InteractiveServer)

**Spec:** [`docs/superpowers/specs/2026-09-04-blazor-track-design.md`](../specs/2026-09-04-blazor-track-design.md)

---

## Global Constraints

Every task's requirements implicitly include this section.

### Versions and paths

- Target framework: `net10.0`. Never multi-target.
- Package versions, exactly: `bunit` **2.9.0**, `xunit` **2.9.3**, `xunit.runner.visualstudio` **3.1.4**, `Microsoft.NET.Test.Sdk` **17.14.1**, `coverlet.collector` **6.0.4**.
- Every command in this plan runs from inside `blazor/`, never from the repo root.
- `dotnet` is on `PATH` on this machine (SDK 10.0.400 confirmed). Go and Rust are not, but this track needs neither.

### Namespaces

`.razor` files derive their namespace from folder path, and `01-beginner` is not a valid C# identifier. Each tier folder therefore pins its namespace with a folder-level `_Imports.razor`:

    exercises/01-beginner/_Imports.razor   ->  @namespace FeWoLearning.Blazor.Exercises.Beginner
    exercises/02-intermediate/_Imports.razor -> @namespace FeWoLearning.Blazor.Exercises.Intermediate
    exercises/03-advanced/_Imports.razor   ->  @namespace FeWoLearning.Blazor.Exercises.Advanced
    exercises/04-expert/_Imports.razor     ->  @namespace FeWoLearning.Blazor.Exercises.Expert
    exercises/_support/_Imports.razor      ->  @namespace FeWoLearning.Blazor.Support

`solutions/` uses the **same** four namespaces — that is the point. Test namespaces mirror them as `FeWoLearning.Blazor.Tests.Beginner` and friends.

A component's **type name is its file name**. `Ex001_HelloComponent.razor` declares the type `Ex001_HelloComponent`, and tests instantiate it under that exact name. This is verified, not assumed.

### The `_support/` rule

Files under `exercises/_support/` and `solutions/_support/` are **fixtures, not exercises**: probe components and model types that tests need. They are byte-identical in both RCLs, they never contain a TODO, and they are never listed in `catalog.md`. When a task adds a `_support/` file, it adds the identical file to both RCLs in the same step.

### Stub shape A — the TODO lives in `@code`

For exercises whose substance is C# logic. The markup is complete; a member throws.

    @* Exercise 001 - Hello Component (beginner).
       Goal:   Render a greeting for the Name parameter.
       Drills: [Parameter], one-way binding of a computed member.
       Passes: dotnet test --filter FullyQualifiedName~Ex001_ *@
    <p id="greeting">@Greeting</p>

    @code {
        [Parameter] public string Name { get; set; } = "world";

        // TODO: return "Hello, {Name}!"
        private string Greeting => throw new NotImplementedException("TODO: Ex001 - build the greeting");
    }

### Stub shape B — the TODO is the markup itself

For exercises whose substance is Razor markup (`@if`, `@foreach`, `@key`, `RenderFragment`, `@bind`). **`throw` is illegal in Razor markup (`CS8115`)**, so the markup cannot throw. Instead the markup is left as a comment describing what to render, and the component throws from `OnParametersSet`:

    @* Exercise 003 - Conditional Rendering (beginner).
       Goal:   Render exactly one of #loading / #error / #content / #empty.
       Drills: @if / else if / else, precedence, blank-string handling.
       Passes: dotnet test --filter FullyQualifiedName~Ex003_ *@
    @* TODO: render exactly one of:
         <p id="loading">Loading</p>
         <p id="error">@ErrorMessage</p>
         <p id="content">@Content</p>
         <p id="empty">No data</p> *@

    @code {
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public string? ErrorMessage { get; set; }
        [Parameter] public string? Content { get; set; }

        protected override void OnParametersSet()
            => throw new NotImplementedException("TODO: Ex003 - render exactly one panel");
    }

Both shapes are **verified**: they compile, and bUnit surfaces the `NotImplementedException` as a test failure carrying the TODO message. The reference solution deletes the throwing member entirely.

Every stub carries the four-line header comment (`Goal:` / `Drills:` / `Passes:`) shown above. Every `NotImplementedException` message starts with `TODO: ExNNN - `.

### bUnit 2.9 API — the names that actually exist

bUnit 2 renamed several bUnit 1 APIs. Using the old names is a compile error, so use exactly these:

| Purpose | bUnit 2.9 |
|---|---|
| Test base class | `Bunit.BunitContext` (**not** `TestContext` — that is ambiguous with `Xunit.TestContext`, `CS0104`) |
| Render | `Render<TComponent>(p => p.Add(c => c.Prop, value))` |
| Re-render with new parameters | `cut.Render(p => p.Add(c => c.Prop, value))` (**not** `SetParametersAndRender`) |
| Two-way bindable parameter | `p.Bind(c => c.Value, current, v => current = v)` |
| Query | `cut.Find(sel)`, `cut.FindAll(sel)`, `cut.FindComponent<T>()`, `cut.FindComponents<T>()` |
| Child component instance | `cut.FindComponents<T>()[i].Instance` |
| Events | `.Click()`, `.Input(v)`, `.Change(v)`, `.KeyDown(v)`, `.Submit()` |
| Wait for a re-render | `cut.WaitForAssertion(() => ...)`, `cut.WaitForState(...)`, `cut.WaitForElement(sel)` |
| Unmatched attributes | `p.AddUnmatched("data-test", "x")` |

Test class skeleton, used by every test file:

    using Bunit;
    using FeWoLearning.Blazor.Exercises.Beginner;
    using Xunit;

    namespace FeWoLearning.Blazor.Tests.Beginner;

    public class Ex001_HelloComponentTests : BunitContext
    {
        [Fact]
        public void Renders_The_Name_Parameter()
        {
            var cut = Render<Ex001_HelloComponent>(p => p.Add(c => c.Name, "Blazor"));

            Assert.Equal("Hello, Blazor!", cut.Find("#greeting").TextContent);
        }
    }

### Test-quality rules (spec §7) — apply to every test written

- **Never** assert on `cut.Markup` as a whole string. Assert through `Find`/`FindAll` plus `TextContent`, `GetAttribute(...)`, or `ClassList`.
- After any state change (`.Click()`, `.Input()`, async lifecycle), assert inside `cut.WaitForAssertion(() => ...)`. A bare assertion after a state change can pass on the *previous* frame.
- Elements returned by `Find`/`FindAll` are wrappers; **never** use `Assert.Same` on them. To prove instance identity, use child *component* instances via `FindComponents<T>()[i].Instance`.
- **Non-vacuity check, mandatory for every test:** before accepting a red run, write down what a naive or wrong implementation would do, and confirm the test rejects it. Each exercise below names its specific non-vacuity check. Where the check is cheap, verify it by temporarily breaking the reference solution and confirming the test goes red.
- Confirm every red failure is the exercise's own `NotImplementedException`, not a compile or type-resolution error. A stub that does not build is a bug in the stub.

### Commands

| Command | Effect |
|---|---|
| `dotnet build` | builds all four projects against the stubs |
| `dotnet test` | stubs — must be **all red** |
| `dotnet test -p:UseSolutions=true` | reference solutions — must be **all green** |
| `dotnet test --filter "FullyQualifiedName~Ex001_"` | one exercise |
| `dotnet test --filter "FullyQualifiedName~Ex001_\|FullyQualifiedName~Ex002_"` | a filtered batch (escape `\|` in bash) |
| `dotnet run --project host` | exercise host on `http://localhost:5199` |
| `dotnet run --project host -p:UseSolutions=true` | reference host, same URL |

### Commit discipline

- Stage **explicit paths**. `git add -A` has already swept up unrelated files once in this repo.
- One commit per task. Batch commits are named `blazor: exNNN-exNNN`.
- End every commit message with:

      Co-Authored-By: Claude Opus 5 (1M context) <noreply@anthropic.com>

### Deviations from the spec's slug list, decided while planning

Two slugs from spec §6 changed, because the original could not be tested honestly:

- **ex022 `PreventDefault` → `StopPropagation`.** `@onclick:preventDefault` has no observable effect in bUnit's AngleSharp DOM — there is no navigation to prevent, so any test would pass whether or not the directive is present, which violates the non-vacuity rule. `@onclick:stopPropagation` *is* observable: the parent handler must not fire. `preventDefault` moves to the intermediate tier where an `EditForm` submit makes it observable.
- **ex023 `InputTextBinding`** keeps its name but is scoped to `@bind` on a **local field** (the idiomatic case), to stay distinct from ex008, which is the bindable-component contract (`Value`/`ValueChanged` for a parent's `@bind-Value`).

Update `catalog.md` and spec §6 to match when Task 1 writes the catalog.

---

## File Structure

    blazor/
      FeWoLearning.Blazor.slnx                     # solution, .slnx format like dotnet/
      Directory.Build.props                        # the UseSolutions output-path switch
      .gitignore                                   # artifacts-solutions/
      catalog.md                                   # 100-row ledger, the work queue
      README.md                                    # setup, both commands, deviations, non-goals

      exercises/
        FeWoLearning.Blazor.Exercises.csproj       # RCL: FrameworkReference AspNetCore.App
        _Imports.razor                             # shared @using for all components
        _support/_Imports.razor                    # @namespace FeWoLearning.Blazor.Support
        _support/*.razor, *.cs                     # fixtures: probe components + model types
        01-beginner/_Imports.razor                 # @namespace ...Exercises.Beginner
        01-beginner/ExNNN_<Slug>.razor             # the stubs
        02-intermediate/, 03-advanced/, 04-expert/ # namespace-pinned, empty for now

      solutions/
        FeWoLearning.Blazor.Solutions.csproj       # RCL, RootNamespace FeWoLearning.Blazor.Exercises
        (same tree as exercises/, same namespaces, working implementations)

      tests/
        FeWoLearning.Blazor.Tests.csproj           # bUnit + xunit; conditional ProjectReference
        _Imports.razor                             # only if a test needs inline Razor
        01-beginner/ExNNN_<Slug>Tests.cs

      host/
        FeWoLearning.Blazor.Host.csproj            # Blazor Web App; conditional ProjectReference
        Program.cs, appsettings*.json, Properties/launchSettings.json
        Components/App.razor, Routes.razor, _Imports.razor
        Components/Layout/MainLayout.razor
        Components/Pages/Home.razor, Error.razor, NotFound.razor
        Components/Demos/Beginner/ExNNN.razor      # @page "/beginner/NNN", one per exercise
        wwwroot/app.css

Responsibilities: `exercises/` and `solutions/` hold component code only, no tests. `tests/` holds every assertion and is the sole proof of correctness. `host/` is a manual playground and is never the proof of an exercise. `catalog.md` is the work queue; `README.md` is the human entry point.

---

## Task 1: Scaffolding and catalog

Delivers a `blazor/` track that builds clean in both modes and a complete 100-row ledger. No exercises yet.

**Files:**
- Create: `blazor/FeWoLearning.Blazor.slnx`
- Create: `blazor/Directory.Build.props`
- Create: `blazor/.gitignore`
- Create: `blazor/exercises/FeWoLearning.Blazor.Exercises.csproj`
- Create: `blazor/exercises/_Imports.razor`
- Create: `blazor/exercises/_support/_Imports.razor`
- Create: `blazor/exercises/01-beginner/_Imports.razor`, `02-intermediate/_Imports.razor`, `03-advanced/_Imports.razor`, `04-expert/_Imports.razor`
- Create: `blazor/solutions/FeWoLearning.Blazor.Solutions.csproj` and the same five `_Imports.razor` files
- Create: `blazor/tests/FeWoLearning.Blazor.Tests.csproj`
- Create: `blazor/host/` (generated, then edited — see Step 4)
- Create: `blazor/catalog.md`

**Interfaces:**
- Consumes: nothing.
- Produces: the `UseSolutions` property contract (`dotnet test` = stubs, `-p:UseSolutions=true` = solutions); the five namespaces of Global Constraints; the host route prefix `/beginner/NNN`.

- [ ] **Step 1: Create the two Razor Class Libraries**

`blazor/exercises/FeWoLearning.Blazor.Exercises.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Blazor.Exercises</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

</Project>
```

`blazor/solutions/FeWoLearning.Blazor.Solutions.csproj` is identical except for the assembly name, and deliberately keeps the **same** `RootNamespace`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>FeWoLearning.Blazor.Solutions</AssemblyName>
    <RootNamespace>FeWoLearning.Blazor.Exercises</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

</Project>
```

The `FrameworkReference` is required: without it the Razor source generator cannot resolve `Microsoft.AspNetCore.Components` and every `.razor` file fails with `CS0234`.

`blazor/exercises/_Imports.razor` (and the identical file in `blazor/solutions/`):

```razor
@using System.Globalization
@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using FeWoLearning.Blazor.Support
```

The five namespace-pinning files, in **both** RCLs:

```razor
@* exercises/_support/_Imports.razor *@
@namespace FeWoLearning.Blazor.Support
```

```razor
@* exercises/01-beginner/_Imports.razor *@
@namespace FeWoLearning.Blazor.Exercises.Beginner
```

```razor
@* exercises/02-intermediate/_Imports.razor *@
@namespace FeWoLearning.Blazor.Exercises.Intermediate
```

```razor
@* exercises/03-advanced/_Imports.razor *@
@namespace FeWoLearning.Blazor.Exercises.Advanced
```

```razor
@* exercises/04-expert/_Imports.razor *@
@namespace FeWoLearning.Blazor.Exercises.Expert
```

An empty folder holding only `_Imports.razor` builds fine, so create all four tiers now.

- [ ] **Step 2: Create `Directory.Build.props` and `.gitignore`**

`blazor/Directory.Build.props`:

```xml
<Project>

  <!-- Redirect the solutions build to its own output tree. This is required, not
       cosmetic: setting BaseOutputPath/BaseIntermediateOutputPath inside a .csproj
       body is evaluated too late, the stale default obj/ is then globbed alongside
       the new one, and the build fails with CS0579 duplicate-attribute errors. -->
  <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
    <UseArtifactsOutput>true</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
  </PropertyGroup>

</Project>
```

`blazor/.gitignore`:

```gitignore
artifacts-solutions/
```

- [ ] **Step 3: Create the test project**

`blazor/tests/FeWoLearning.Blazor.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Blazor.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="bunit" Version="2.9.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one of the two RCLs, never both: that is what keeps the identical
       namespaces and type names from colliding. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Blazor.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Blazor.Solutions.csproj" />
  </ItemGroup>

</Project>
```

The SDK is `Microsoft.NET.Sdk.Razor` (not plain `Microsoft.NET.Sdk`) so a later test can declare an inline Razor fixture if it needs one.

- [ ] **Step 4: Generate and adapt the host**

Generate the .NET 10 template rather than hand-writing it:

```bash
cd blazor
dotnet new blazor -o host -n FeWoLearning.Blazor.Host --interactivity Server --empty --no-https
```

Then make exactly these edits:

1. Add the conditional project references to `host/FeWoLearning.Blazor.Host.csproj`, inside the existing `<Project>`:

```xml
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Blazor.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Blazor.Solutions.csproj" />
  </ItemGroup>
```

2. Append to `host/Components/_Imports.razor` so demo pages can name exercise components directly:

```razor
@using FeWoLearning.Blazor.Support
@using FeWoLearning.Blazor.Exercises.Beginner
@using FeWoLearning.Blazor.Exercises.Intermediate
@using FeWoLearning.Blazor.Exercises.Advanced
@using FeWoLearning.Blazor.Exercises.Expert
```

3. Replace `host/Properties/launchSettings.json` with a fixed, non-launching profile so the HTTP checks in Task 9 are deterministic:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5199",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

4. Replace `host/Components/Pages/Home.razor` with an index that links the demo pages:

```razor
@page "/"

<PageTitle>FeWoLearning - Blazor</PageTitle>

<h1>FeWoLearning - Blazor</h1>

<p>
    Demo pages render the components from whichever library is referenced:
    <code>dotnet run --project host</code> shows your exercise stubs,
    <code>dotnet run --project host -p:UseSolutions=true</code> shows the reference solutions.
    An unfinished stub surfaces its <code>NotImplementedException</code> here - that is expected.
</p>

<h2>Beginner</h2>
<ul>
    @for (var n = 1; n <= 35; n++)
    {
        var id = n.ToString("D3");
        <li><a href="@($"/beginner/{id}")">Exercise @id</a></li>
    }
</ul>
```

Leave `Program.cs`, `App.razor`, `Routes.razor`, `MainLayout.razor`, `Error.razor` and `NotFound.razor` as generated. `Routes.razor` already routes on `typeof(Program).Assembly`, and demo pages live in the host assembly, so they are discovered with no further change.

- [ ] **Step 5: Create the solution file**

`blazor/FeWoLearning.Blazor.slnx` — the `.slnx` format, matching `dotnet/`:

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Blazor.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Blazor.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Blazor.Tests.csproj" />
  </Folder>
  <Folder Name="/host/">
    <Project Path="host/FeWoLearning.Blazor.Host.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 6: Verify both build configurations**

Run:

```bash
cd blazor
dotnet build
dotnet build -p:UseSolutions=true
```

Expected: both succeed with **zero warnings and zero errors**. If `CS0234` appears on a `.razor` file, the `FrameworkReference` is missing. If `CS0579` duplicate-attribute errors appear, `Directory.Build.props` is wrong or absent.

- [ ] **Step 7: Write `catalog.md`**

Create `blazor/catalog.md` following the `dotnet/catalog.md` shape: title, tier ranges, legend, a short paragraph explaining where stubs/tests/solutions live, a `**Status:**` line, then one `## <Tier> (NNN-NNN)` section per tier with a `| # | Slug | Concepts | Status |` table.

Header text:

```markdown
# Blazor — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned.

"Beginner" means **Blazor** beginner, not C# beginner: ex001 is a component with a
`[Parameter]`, not a `FizzBuzz`. Plain C# language drills belong to the `dotnet/` track.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.razor`, their bUnit tests in
`tests/<tier>/ExNNN_<Slug>Tests.cs`, reference solutions in
`solutions/<tier>/ExNNN_<Slug>.razor`, and a manual demo page in
`host/Components/Demos/<Tier>/ExNNN.razor`. Tier namespaces are pinned by a
folder-level `_Imports.razor` (`@namespace FeWoLearning.Blazor.Exercises.Beginner`
and friends), because `01-beginner` is not a valid C# identifier.

**Status: 0 ✅ / 100 ⬜**
```

All 100 rows start as ⬜. The Beginner rows use exactly these slugs and concepts:

| #   | Slug | Concepts |
|-----|------|----------|
| 001 | HelloComponent | `[Parameter]`, one-way binding of a computed member |
| 002 | ParameterDefaults | parameter defaults, nullable/blank parameter handling |
| 003 | ConditionalRendering | `@if`/`else if`/`else`, render precedence |
| 004 | ListRendering | `@foreach`, list projection, empty list |
| 005 | KeyedListDiffing | `@key`, component instance identity across reorder |
| 006 | ClickEventCallback | `@onclick`, `EventCallback<T>`, stateless child |
| 007 | CounterState | component-owned state, implicit re-render |
| 008 | TwoWayBinding | `Value`/`ValueChanged` contract for `@bind-Value` |
| 009 | BindFormat | date formatting and parsing round-trip, invariant culture |
| 010 | BindEventOnInput | `@oninput` vs `@onchange` timing |
| 011 | ChildContent | `RenderFragment`, omitting an absent fragment |
| 012 | NamedFragments | multiple named `RenderFragment` parameters |
| 013 | TemplatedFragment | `@typeparam`, `RenderFragment<TItem>`, empty template |
| 014 | AttributeSplatting | `CaptureUnmatchedValues`, `@attributes` |
| 015 | DynamicCssClass | computed class strings, enum-driven styling |
| 016 | InlineStyleBinding | computed inline style, clamping, invariant formatting |
| 017 | OnInitialized | `OnInitialized` runs once, not per parameter change |
| 018 | OnParametersSet | `OnParametersSet` runs on every parameter change |
| 019 | OnAfterRenderFirst | `OnAfterRender(bool firstRender)`, render counting |
| 020 | DisposableComponent | `@implements IDisposable`, subscribe/unsubscribe symmetry |
| 021 | EventArgsHandling | `KeyboardEventArgs`, filtering modifier keys |
| 022 | StopPropagation | `@onclick:stopPropagation`, nested handlers |
| 023 | InputTextBinding | `@bind` to a local field |
| 024 | NumericInputParsing | `@bind` to `int`, rejecting unparsable input |
| 025 | SelectBinding | `@bind` on `<select>`, option projection |
| 026 | CheckboxGroup | multi-selection state, stable result ordering |
| 027 | RadioGroup | single-selection state, mutual exclusion |
| 028 | CascadingValueBasics | `CascadingValue`/`[CascadingParameter]` |
| 029 | NamedCascadingValue | `Name`-matched cascading values of the same type |
| 030 | ComponentComposition | child registers itself with its parent |
| 031 | ChildToParentCallback | `EventCallback` re-renders the parent automatically |
| 032 | MarkupStringRendering | `MarkupString` vs escaped text |
| 033 | EmptyStateFallback | three-state rendering, exact user-facing copy |
| 034 | NestedParameterFlow | parameters do not flow implicitly through levels |
| 035 | TabsComposition | capstone: cascaded parent, registration, active state |

For rows 036–100, write slugs and concepts from the spec's tier themes (§6). These rows are a planning artifact only; a later batch may refine a row before implementing it, which is normal for this repo.

- [ ] **Step 8: Commit**

```bash
git add blazor/FeWoLearning.Blazor.slnx blazor/Directory.Build.props blazor/.gitignore \
        blazor/catalog.md blazor/exercises blazor/solutions blazor/tests blazor/host
git commit -m "blazor: scaffold track (RCL + bUnit + host) and 100-row catalog"
```

---

## Task 2: ex001–ex005

First batch. It also proves the red/green machinery end to end for the first time.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex001_HelloComponent.razor` … `Ex005_KeyedListDiffing.razor`
- Create: `blazor/exercises/_support/Person.cs`, `blazor/exercises/_support/RosterEntry.razor` (and byte-identical copies under `blazor/solutions/_support/`)
- Create: `blazor/solutions/01-beginner/Ex001_HelloComponent.razor` … `Ex005_KeyedListDiffing.razor`
- Create: `blazor/tests/01-beginner/Ex001_HelloComponentTests.cs` … `Ex005_KeyedListDiffingTests.cs`
- Create: `blazor/host/Components/Demos/Beginner/Ex001.razor` … `Ex005.razor`
- Modify: `blazor/catalog.md` (rows 001–005 and the `**Status:**` line)

**Interfaces:**
- Consumes: the namespaces, both stub shapes, and the bUnit API table from Global Constraints.
- Produces: `FeWoLearning.Blazor.Support.Person` — `public sealed record Person(int Id, string Name)`; `FeWoLearning.Blazor.Support.RosterEntry` — a component with `[Parameter, EditorRequired] public Person Person { get; set; }` exposing `Person` publicly so tests can read `FindComponents<RosterEntry>()[i].Instance.Person.Id`. Later tasks may reuse both.

- [ ] **Step 1: Add the `_support` fixtures to both RCLs**

`exercises/_support/Person.cs`, copied byte-identically to `solutions/_support/Person.cs`:

```csharp
namespace FeWoLearning.Blazor.Support;

/// <summary>Test fixture model. Not an exercise.</summary>
public sealed record Person(int Id, string Name);
```

`exercises/_support/RosterEntry.razor`, copied byte-identically to `solutions/_support/RosterEntry.razor`:

```razor
@* Test fixture, not an exercise. Exists so a test can observe component
   instance identity across a list reorder (see Ex005). *@
<span class="entry" data-id="@Person.Id">@Person.Name</span>

@code {
    [Parameter, EditorRequired] public Person Person { get; set; } = default!;
}
```

- [ ] **Step 2: Write the five stubs**

**ex001 `Ex001_HelloComponent` — shape A.** Exactly the code in Global Constraints → "Stub shape A".

**ex002 `Ex002_ParameterDefaults` — shape A.**

```razor
@* Exercise 002 - Parameter Defaults (beginner).
   Goal:   Render a badge that omits an absent or blank title, and a level line.
   Drills: parameter defaults, nullable and blank parameter handling.
   Passes: dotnet test --filter FullyQualifiedName~Ex002_ *@
<span id="badge">@BadgeText</span>
<span id="level">Level @Level</span>

@code {
    [Parameter] public string Name { get; set; } = "";
    [Parameter] public string? Title { get; set; }
    [Parameter] public int Level { get; set; } = 1;

    // TODO: "{Name} ({Title})" when Title has non-whitespace content, otherwise just Name.
    private string BadgeText => throw new NotImplementedException("TODO: Ex002 - build the badge text");
}
```

**ex003 `Ex003_ConditionalRendering` — shape B.** Exactly the code in Global Constraints → "Stub shape B".

**ex004 `Ex004_ListRendering` — shape B.**

```razor
@* Exercise 004 - List Rendering (beginner).
   Goal:   Render one <li class="tag"> per tag inside <ul id="tags">.
   Drills: @foreach, list projection, empty list.
   Passes: dotnet test --filter FullyQualifiedName~Ex004_ *@
@* TODO: render <ul id="tags"> containing one <li class="tag">@tag</li> per tag,
     in the order given. An empty list still renders the <ul>, with no <li>. *@

@code {
    [Parameter] public IReadOnlyList<string> Tags { get; set; } = [];

    protected override void OnParametersSet()
        => throw new NotImplementedException("TODO: Ex004 - render the tag list");
}
```

**ex005 `Ex005_KeyedListDiffing` — shape B.**

```razor
@* Exercise 005 - Keyed List Diffing (beginner).
   Goal:   Render a roster whose child components stay with their person when
           the list is reordered.
   Drills: @key, component instance identity across a reorder.
   Passes: dotnet test --filter FullyQualifiedName~Ex005_ *@
@* TODO: render <ul id="roster"> containing, per person, an
     <li class="row"> that holds a <RosterEntry Person="p" />.
     Reordering People must move each RosterEntry instance with its person -
     that is what @key is for. *@

@code {
    [Parameter] public IReadOnlyList<Person> People { get; set; } = [];

    protected override void OnParametersSet()
        => throw new NotImplementedException("TODO: Ex005 - render the keyed roster");
}
```

- [ ] **Step 3: Write the five tests**

**`Ex001_HelloComponentTests`** — exactly the skeleton in Global Constraints, plus a default-value test:

```csharp
using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex001_HelloComponentTests : BunitContext
{
    [Fact]
    public void Greets_The_Default_Name()
    {
        var cut = Render<Ex001_HelloComponent>();

        Assert.Equal("Hello, world!", cut.Find("#greeting").TextContent);
    }

    [Fact]
    public void Greets_The_Given_Name()
    {
        var cut = Render<Ex001_HelloComponent>(p => p.Add(c => c.Name, "Blazor"));

        Assert.Equal("Hello, Blazor!", cut.Find("#greeting").TextContent);
    }
}
```

Non-vacuity: a hard-coded `"Hello, world!"` passes the first test and fails the second.

**`Ex002_ParameterDefaultsTests`** — five facts:
1. `Name="Ada"` only → `#badge` text is `Ada`.
2. `Name="Ada"`, `Title="Architect"` → `#badge` text is `Ada (Architect)`.
3. `Name="Ada"`, `Title="   "` → `#badge` text is `Ada` (blank counts as absent).
4. `Name="Ada"` only → `#level` text is `Level 1`.
5. `Level=7` → `#level` text is `Level 7`.

Non-vacuity: an implementation using `Title is not null` instead of `IsNullOrWhiteSpace` passes 1, 2, 4, 5 and fails 3.

**`Ex003_ConditionalRenderingTests`** — six facts:
1. `IsLoading=true` → `#loading` text `Loading`, and `FindAll("#error")`, `FindAll("#content")`, `FindAll("#empty")` are all empty.
2. `ErrorMessage="boom"` → `#error` text `boom`, no other panel.
3. `Content="hi"` → `#content` text `hi`, no other panel.
4. no parameters → `#empty` text `No data`, no other panel.
5. `IsLoading=true` **and** `ErrorMessage="boom"` **and** `Content="hi"` → only `#loading` (precedence).
6. `ErrorMessage="   "`, `Content=""` → `#empty` (blank counts as absent).

Non-vacuity: rendering all four panels unconditionally passes a naive "the element exists" test; the `FindAll(...)` emptiness assertions and fact 5 reject it.

**`Ex004_ListRenderingTests`** — three facts:
1. `Tags=["a","b","c"]` → `FindAll("li.tag")` has count 3 and texts `["a","b","c"]` **in order**.
2. `Tags=[]` → `Find("#tags")` succeeds and `FindAll("li.tag")` is empty.
3. `Tags=["x","x"]` → count 2 (duplicates are not collapsed).

Non-vacuity: an implementation that renders a fixed single `<li>` fails 1; one that skips the `<ul>` when empty fails 2; one using a `HashSet` fails 3.

**`Ex005_KeyedListDiffingTests`** — two facts:

```csharp
using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex005_KeyedListDiffingTests : BunitContext
{
    private static readonly Person Ada = new(1, "Ada");
    private static readonly Person Grace = new(2, "Grace");
    private static readonly Person Linus = new(3, "Linus");

    [Fact]
    public void Renders_One_Row_Per_Person_In_Order()
    {
        var cut = Render<Ex005_KeyedListDiffing>(
            p => p.Add(c => c.People, new[] { Ada, Grace, Linus }));

        var names = cut.FindAll("li.row span.entry").Select(e => e.TextContent).ToArray();
        Assert.Equal(new[] { "Ada", "Grace", "Linus" }, names);
    }

    [Fact]
    public void Reorder_Keeps_Each_Child_Instance_With_Its_Person()
    {
        var cut = Render<Ex005_KeyedListDiffing>(
            p => p.Add(c => c.People, new[] { Ada, Grace, Linus }));
        var before = cut.FindComponents<RosterEntry>()
            .ToDictionary(c => c.Instance.Person.Id, c => (object)c.Instance);

        cut.Render(p => p.Add(c => c.People, new[] { Linus, Ada, Grace }));
        var after = cut.FindComponents<RosterEntry>()
            .ToDictionary(c => c.Instance.Person.Id, c => (object)c.Instance);

        Assert.Equal(before.Keys.Order(), after.Keys.Order());
        foreach (var id in before.Keys)
            Assert.Same(before[id], after[id]);
    }
}
```

Non-vacuity: **verified during planning.** With `@key` present the second fact passes; delete the `@key` from the reference solution and it fails with `Assert.Same() Failure: Values are not the same instance`. Re-confirm this after writing the solution.

- [ ] **Step 4: Red check, filtered to the batch**

Run:

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex001_|FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"
```

Expected: **0 passed, all failed.** Every failure message must read `System.NotImplementedException : TODO: ExNNN - ...`. Any compile error, or any test that passes, is a bug to fix before continuing.

- [ ] **Step 5: Write the five reference solutions**

Each solution is the stub with the throwing member **deleted** and the real implementation in its place. The header comment stays.

```razor
@* solutions/01-beginner/Ex001_HelloComponent.razor *@
<p id="greeting">@Greeting</p>

@code {
    [Parameter] public string Name { get; set; } = "world";

    private string Greeting => $"Hello, {Name}!";
}
```

```razor
@* solutions/01-beginner/Ex002_ParameterDefaults.razor *@
<span id="badge">@BadgeText</span>
<span id="level">Level @Level</span>

@code {
    [Parameter] public string Name { get; set; } = "";
    [Parameter] public string? Title { get; set; }
    [Parameter] public int Level { get; set; } = 1;

    private string BadgeText => string.IsNullOrWhiteSpace(Title) ? Name : $"{Name} ({Title})";
}
```

```razor
@* solutions/01-beginner/Ex003_ConditionalRendering.razor *@
@if (IsLoading)
{
    <p id="loading">Loading</p>
}
else if (!string.IsNullOrWhiteSpace(ErrorMessage))
{
    <p id="error">@ErrorMessage</p>
}
else if (!string.IsNullOrWhiteSpace(Content))
{
    <p id="content">@Content</p>
}
else
{
    <p id="empty">No data</p>
}

@code {
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string? ErrorMessage { get; set; }
    [Parameter] public string? Content { get; set; }
}
```

```razor
@* solutions/01-beginner/Ex004_ListRendering.razor *@
<ul id="tags">
    @foreach (var tag in Tags)
    {
        <li class="tag">@tag</li>
    }
</ul>

@code {
    [Parameter] public IReadOnlyList<string> Tags { get; set; } = [];
}
```

```razor
@* solutions/01-beginner/Ex005_KeyedListDiffing.razor *@
<ul id="roster">
    @foreach (var person in People)
    {
        <li class="row" @key="person.Id">
            <RosterEntry Person="person" />
        </li>
    }
</ul>

@code {
    [Parameter] public IReadOnlyList<Person> People { get; set; } = [];
}
```

- [ ] **Step 6: Green check**

Run:

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex001_|FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"
```

Expected: **all passed, 0 failed.**

- [ ] **Step 7: Confirm ex005 is not vacuous**

Temporarily delete ` @key="person.Id"` from `solutions/01-beginner/Ex005_KeyedListDiffing.razor`, re-run the green command, confirm `Ex005` **fails**, then restore the `@key` and re-run to confirm it passes again.

- [ ] **Step 8: Add the five host demo pages**

Each is minimal. Pattern:

```razor
@* host/Components/Demos/Beginner/Ex001.razor *@
@page "/beginner/001"

<h1>Exercise 001 - HelloComponent</h1>

<Ex001_HelloComponent Name="Blazor" />
```

For ex002 pass `Name="Ada" Title="Architect" Level="7"`. For ex003 render three instances: one with `IsLoading="true"`, one with `ErrorMessage="boom"`, one with no parameters. For ex004 pass `Tags="new[] { \"blazor\", \"razor\", \"bunit\" }"`. For ex005 render the component with three people and a button that reorders them, so the reorder is visible by hand:

```razor
@page "/beginner/005"
@rendermode InteractiveServer

<h1>Exercise 005 - KeyedListDiffing</h1>

<button id="shuffle" @onclick="Rotate">Rotate</button>

<Ex005_KeyedListDiffing People="_people" />

@code {
    private List<Person> _people =
    [
        new(1, "Ada"), new(2, "Grace"), new(3, "Linus")
    ];

    private void Rotate()
    {
        var first = _people[0];
        _people = [.. _people.Skip(1), first];
    }
}
```

Verify the host still builds in both modes:

```bash
cd blazor
dotnet build --project host 2>/dev/null || dotnet build host/FeWoLearning.Blazor.Host.csproj
dotnet build host/FeWoLearning.Blazor.Host.csproj -p:UseSolutions=true
```

- [ ] **Step 9: Update `catalog.md` and commit**

Flip exactly rows 001–005 from ⬜ to ✅ and set `**Status: 5 ✅ / 95 ⬜**`. Watch the cell padding — some catalogs in this repo pad the status cell, some do not; match whatever Task 1 wrote.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex001-ex005"
```

---

## Task 3: ex006–ex010

Events, state, and the binding contract.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex006_ClickEventCallback.razor` … `Ex010_BindEventOnInput.razor`
- Create: the matching five files under `blazor/solutions/01-beginner/`
- Create: `blazor/tests/01-beginner/Ex006_ClickEventCallbackTests.cs` … `Ex010_BindEventOnInputTests.cs`
- Create: `blazor/host/Components/Demos/Beginner/Ex006.razor` … `Ex010.razor`
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints; nothing from Task 2 except the conventions.
- Produces: nothing later tasks depend on.

- [ ] **Step 1: Write the five stubs**

**ex006 `Ex006_ClickEventCallback` — shape A.** Deliberately **stateless**: it reports the next value and never stores it, which is what makes fact 3 below meaningful.

- Parameters: `[Parameter] public int Count { get; set; }`, `[Parameter] public EventCallback<int> OnLike { get; set; }`.
- Markup: `<button id="like" @onclick="HandleClickAsync">Like (@Count)</button>`.
- TODO: `private Task HandleClickAsync() => throw new NotImplementedException("TODO: Ex006 - report Count + 1 through OnLike");`

**ex007 `Ex007_CounterState` — shape A.** Component-owned state.

- Parameter: `[Parameter] public int Start { get; set; }`.
- Markup: `<button id="dec" @onclick="Decrement">-</button><span id="value">@Current</span><button id="inc" @onclick="Increment">+</button>`.
- TODO: a backing field plus `private int Current => throw ...`, `private void Increment() => throw ...`, `private void Decrement() => throw ...`, all three with `TODO: Ex007 - ` messages. `Current` must start at `Start`.

**ex008 `Ex008_TwoWayBinding` — shape B.** The bindable-component contract.

- Parameters: `[Parameter] public string Value { get; set; } = "";`, `[Parameter] public EventCallback<string> ValueChanged { get; set; }`.
- TODO markup comment: render `<input id="name" value="@Value" ... />` that invokes `ValueChanged` with the new text on `onchange`, plus `<span id="echo">@Value</span>`. Note in the comment that the component must **not** write to `Value` itself — a parameter is owned by the parent.
- `protected override void OnParametersSet() => throw new NotImplementedException("TODO: Ex008 - render a bindable input");`

**ex009 `Ex009_BindFormat` — shape B.**

- Parameters: `[Parameter] public DateOnly Value { get; set; }`, `[Parameter] public EventCallback<DateOnly> ValueChanged { get; set; }`.
- TODO markup comment: render `<input id="due" type="date" value="@..." />` whose value attribute is `Value` formatted `yyyy-MM-dd` with `CultureInfo.InvariantCulture`, and which parses the incoming string back to a `DateOnly` on `onchange` and reports it through `ValueChanged`. Unparsable input is ignored (no callback).
- Throwing `OnParametersSet`, message `TODO: Ex009 - format and parse the due date`.

**ex010 `Ex010_BindEventOnInput` — shape B.**

- Parameters: `[Parameter] public string Query { get; set; } = "";`, `[Parameter] public EventCallback<string> QueryChanged { get; set; }`.
- TODO markup comment: render `<input id="q" value="@Query" ... />` that reports through `QueryChanged` on **every keystroke** (`@oninput`), not on blur, plus `<span id="echo">@Query</span>`.
- Throwing `OnParametersSet`, message `TODO: Ex010 - report the query on every keystroke`.

- [ ] **Step 2: Write the five tests**

**`Ex006_ClickEventCallbackTests`** — three facts:
1. `Count=4` → `#like` text is `Like (4)`.
2. `Count=4`, callback capturing into a local → one `.Click()` yields exactly one invocation with `5`.
3. `Count=4` → **two** `.Click()` calls yield `[5, 5]`, because the component is stateless and does not accumulate.

Wire the callback with `p.Add(c => c.OnLike, EventCallback.Factory.Create<int>(this, v => received.Add(v)))`. Assert inside `cut.WaitForAssertion(...)`.

Non-vacuity: a component that stores the count internally passes 1 and 2 and fails 3; one invoking `OnLike` with `Count` instead of `Count + 1` fails 2.

**`Ex007_CounterStateTests`** — four facts:
1. `Start=3` → `#value` text `3`.
2. one `#inc` click → `4`.
3. one `#dec` click → `2`.
4. three `#inc` clicks → `6`.

All post-click assertions inside `cut.WaitForAssertion(...)`.

Non-vacuity: a `Current` that returns `Start` unchanged passes 1 and fails 2–4; one that ignores `Start` and begins at 0 fails 1.

**`Ex008_TwoWayBindingTests`** — three facts:
1. `Value="Ada"` → `#name` `value` attribute is `Ada` and `#echo` text is `Ada`.
2. Bound with `p.Bind(c => c.Value, current, v => current = v)`: `cut.Find("#name").Change("Grace")` → the local `current` is `Grace`, and `#echo` shows `Grace` after the re-render.
3. `p.Add(c => c.Value, "Ada")` **without** a `ValueChanged` handler: `.Change("Grace")` must not throw, and `#echo` still shows `Ada` — the component does not own the value.

Non-vacuity: a component that assigns to `Value` internally passes 1 and 2 and fails 3.

**`Ex009_BindFormatTests`** — four facts:
1. `Value=new DateOnly(2026, 9, 4)` → `#due` `value` attribute is `2026-09-04`.
2. Bound: `.Change("2026-12-24")` → the local value is `new DateOnly(2026, 12, 24)`.
3. Bound: `.Change("not-a-date")` → the local value is **unchanged** and nothing throws.
4. With `CultureInfo.CurrentCulture` temporarily set to `de-DE` (restore it in a `finally`), fact 1 still yields `2026-09-04` — invariant formatting, not `04.09.2026`.

Non-vacuity: `Value.ToString("d")` or `ToShortDateString()` passes 1 under an invariant test culture and fails 4.

**`Ex010_BindEventOnInputTests`** — three facts:
1. `Query="ab"` → `#q` `value` attribute is `ab` and `#echo` text is `ab`.
2. Bound: `cut.Find("#q").Input("abc")` → the local value is `abc`.
3. Bound: `cut.Find("#q").Change("xyz")` → the local value is **still** the pre-change value. Wiring `@onchange` instead of `@oninput` would make 2 fail and 3 pass, so the pair pins the timing.

Non-vacuity: covered by facts 2 and 3 together — no single-event implementation satisfies both.

- [ ] **Step 3: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex006_|FullyQualifiedName~Ex007_|FullyQualifiedName~Ex008_|FullyQualifiedName~Ex009_|FullyQualifiedName~Ex010_"
```

Expected: 0 passed, all failed, every failure a `TODO: ExNNN - ` `NotImplementedException`.

- [ ] **Step 4: Write the five reference solutions**

Key points the implementer must get right, beyond mechanically filling in the stub:

- **ex006:** `private Task HandleClickAsync() => OnLike.InvokeAsync(Count + 1);` — no local state.
- **ex007:** `private int _current;` initialised in `OnInitialized` from `Start` (not in `OnParametersSet`, or fact 4 breaks when a parameter changes). `Current => _current`.
- **ex008:** `<input id="name" value="@Value" @onchange="OnChangedAsync" />` with `private Task OnChangedAsync(ChangeEventArgs e) => ValueChanged.InvokeAsync(e.Value?.ToString() ?? "");`. Never assign to `Value`.
- **ex009:** format with `Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)`; parse with `DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)` and only then `ValueChanged.InvokeAsync(parsed)`.
- **ex010:** identical to ex008 but wired to `@oninput`.

- [ ] **Step 5: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex006_|FullyQualifiedName~Ex007_|FullyQualifiedName~Ex008_|FullyQualifiedName~Ex009_|FullyQualifiedName~Ex010_"
```

Expected: all passed.

- [ ] **Step 6: Add five host demo pages**

Each needs `@rendermode InteractiveServer` because all five are interactive. Each holds local state in `@code` and binds it, so the behaviour is visible by hand — e.g. ex008:

```razor
@page "/beginner/008"
@rendermode InteractiveServer

<h1>Exercise 008 - TwoWayBinding</h1>

<Ex008_TwoWayBinding @bind-Value="_name" />
<p>Parent sees: <strong>@_name</strong></p>

@code {
    private string _name = "Ada";
}
```

Then `dotnet build host/FeWoLearning.Blazor.Host.csproj` and the same with `-p:UseSolutions=true`.

- [ ] **Step 7: Update `catalog.md` and commit**

Flip rows 006–010 to ✅, set `**Status: 10 ✅ / 90 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex006-ex010"
```

---

## Task 4: ex011–ex015

Render fragments, splatting, computed classes.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex011_ChildContent.razor` … `Ex015_DynamicCssClass.razor`
- Create: `blazor/exercises/_support/AlertSeverity.cs` (and the identical copy under `blazor/solutions/_support/`)
- Create: the matching five solutions, five tests, five demo pages
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints.
- Produces: `FeWoLearning.Blazor.Support.AlertSeverity` — `public enum AlertSeverity { Info, Warning, Danger }`.

- [ ] **Step 1: Add the `AlertSeverity` fixture to both RCLs**

```csharp
// exercises/_support/AlertSeverity.cs and solutions/_support/AlertSeverity.cs
namespace FeWoLearning.Blazor.Support;

/// <summary>Test fixture enum for Ex015. Not an exercise.</summary>
public enum AlertSeverity
{
    Info,
    Warning,
    Danger,
}
```

It lives in `_support/` and not in the stub on purpose: a test that names `AlertSeverity.Danger` must compile against the stub, or the batch's red run reports a **compile error instead of a failing test** — the Blazor analogue of the `@pytest.mark.parametrize` collection-time trap documented in `CLAUDE.md`.

- [ ] **Step 2: Write the five stubs**

**ex011 `Ex011_ChildContent` — shape B.**
- Parameters: `[Parameter] public string Title { get; set; } = "";`, `[Parameter] public RenderFragment? ChildContent { get; set; }`.
- TODO markup comment: render `<section class="card">` containing `<h2 class="card-title">@Title</h2>`, and a `<div class="card-body">@ChildContent</div>` **only when `ChildContent` is not null** — an absent fragment must not leave an empty body div behind.
- Throwing `OnParametersSet`, `TODO: Ex011 - render the card`.

**ex012 `Ex012_NamedFragments` — shape B.**
- Parameters: `RenderFragment? Header`, `RenderFragment? Body`, `RenderFragment? Footer`, all `[Parameter]`.
- TODO markup comment: render `<div class="dialog">` containing `<div id="dialog-header">`, `<div id="dialog-body">`, `<div id="dialog-footer">`, each rendered only when its fragment is non-null.
- Throwing `OnParametersSet`, `TODO: Ex012 - render the three named regions`.

**ex013 `Ex013_TemplatedFragment` — shape B.** Generic; the file starts with `@typeparam TItem`.
- Parameters: `[Parameter] public IReadOnlyList<TItem> Items { get; set; } = [];`, `[Parameter] public RenderFragment<TItem>? Row { get; set; }`, `[Parameter] public RenderFragment? Empty { get; set; }`.
- TODO markup comment: render `<div id="repeater">`; for each item render `Row(item)` inside `<div class="row">`; when `Items` is empty render `Empty` instead (or nothing when `Empty` is null). When `Row` is null, render the item's `ToString()`.
- Throwing `OnParametersSet`, `TODO: Ex013 - render the templated rows`.

**ex014 `Ex014_AttributeSplatting` — shape B.**
- Parameters: `[Parameter] public string Label { get; set; } = "";` and
  `[Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? Extra { get; set; }`.
- TODO markup comment: render `<button id="btn" @attributes="Extra">@Label</button>`, so caller-supplied attributes land on the button. `id="btn"` must survive unless the caller overrides it.
- Throwing `OnParametersSet`, `TODO: Ex014 - splat the unmatched attributes`.

**ex015 `Ex015_DynamicCssClass` — shape A.**
- Parameters: `[Parameter] public AlertSeverity Severity { get; set; }`, `[Parameter] public bool Dismissed { get; set; }`.
- Markup: `<div id="alert" class="@CssClass">@ChildContent</div>` with `[Parameter] public RenderFragment? ChildContent { get; set; }`.
- TODO: `private string CssClass => throw new NotImplementedException("TODO: Ex015 - build the class list");` — `alert` plus `alert-info` / `alert-warning` / `alert-danger` for the severity, plus `alert-dismissed` when `Dismissed`, space-separated in that order.

- [ ] **Step 3: Write the five tests**

**`Ex011_ChildContentTests`** — three facts:
1. `Title="Card"`, `ChildContent` rendering `<p>inner</p>` → `.card-title` text `Card`, and `Find(".card-body p").TextContent` is `inner`.
2. `Title="Card"` with no `ChildContent` → `FindAll(".card-body")` is **empty**.
3. `.card-title` is present even with no child content.

Supply the fragment with `p.AddChildContent("<p>inner</p>")`.

Non-vacuity: always rendering `.card-body` passes 1 and 3 and fails 2.

**`Ex012_NamedFragmentsTests`** — three facts:
1. All three fragments supplied → each of `#dialog-header`, `#dialog-body`, `#dialog-footer` contains its own text.
2. Only `Body` supplied → `#dialog-body` exists, `FindAll("#dialog-header")` and `FindAll("#dialog-footer")` are empty.
3. No fragments → all three `FindAll` results are empty and `Find(".dialog")` still succeeds.

Non-vacuity: rendering all three regions unconditionally fails 2 and 3.

**`Ex013_TemplatedFragmentTests`** — four facts, over `TItem = string`:
1. `Items=["a","b"]` with `Row = item => builder => ...` rendering `<span class="cell">@item</span>` → two `.row` elements with cell texts `["a","b"]` in order.
2. `Items=[]` with an `Empty` fragment rendering `<p id="none">none</p>` → `#none` exists and `FindAll(".row")` is empty.
3. `Items=[]` with no `Empty` → `#repeater` exists and is empty of `.row`.
4. `Items=["a"]` with no `Row` → the row's text is `a` (`ToString()` fallback).

Build the `RenderFragment<string>` in the test with `p.Add(c => c.Row, item => builder => { builder.OpenElement(0, "span"); builder.AddAttribute(1, "class", "cell"); builder.AddContent(2, item); builder.CloseElement(); })`. Because the component is generic, name the closed type explicitly: `Render<Ex013_TemplatedFragment<string>>(...)`.

Non-vacuity: an implementation that renders `Empty` alongside the rows fails 2; one that ignores the `Row` template fails 1.

**`Ex014_AttributeSplattingTests`** — three facts:
1. `Label="Go"` → `#btn` text `Go`.
2. `p.AddUnmatched("data-test", "x").AddUnmatched("disabled", true)` → `Find("#btn").GetAttribute("data-test")` is `x` and the button has the `disabled` attribute.
3. No unmatched attributes → `#btn` still resolves and has no `data-test` attribute.

Non-vacuity: dropping `@attributes` entirely passes 1 and 3 and fails 2.

**`Ex015_DynamicCssClassTests`** — five facts:
1. `Severity=Info` → `#alert` class list is exactly `alert alert-info`.
2. `Severity=Warning` → `alert alert-warning`.
3. `Severity=Danger` → `alert alert-danger`.
4. `Severity=Danger`, `Dismissed=true` → `alert alert-danger alert-dismissed`.
5. `Dismissed=false` → the class list does **not** contain `alert-dismissed`.

Assert with `Find("#alert").GetAttribute("class")` for exact strings, or `ClassList` for containment; do not assert on `cut.Markup`.

Non-vacuity: a hard-coded `"alert alert-info"` passes 1 and 5 and fails 2–4.

- [ ] **Step 4: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex011_|FullyQualifiedName~Ex012_|FullyQualifiedName~Ex013_|FullyQualifiedName~Ex014_|FullyQualifiedName~Ex015_"
```

Expected: 0 passed, all failed with `TODO: ExNNN - ` messages. If `Ex015` fails with `CS0246` on `AlertSeverity`, Step 1 was skipped.

- [ ] **Step 5: Write the five reference solutions**

Points that matter:

- **ex011/ex012:** guard each region with `@if (Fragment is not null)`. Do not render an empty wrapper.
- **ex013:** the file opens with `@typeparam TItem`. Render `@Row(item)` when `Row is not null`, else `@item?.ToString()`. Choose the empty branch with `@if (Items.Count == 0)`.
- **ex014:** `<button id="btn" @attributes="Extra">@Label</button>`. Place `id` **before** `@attributes` so a caller-supplied `id` wins (Blazor applies splatted attributes last).
- **ex015:** build with a `string.Join(" ", ...)` over a small list, or a `switch` expression plus a conditional suffix. Order must be `alert`, severity, then `alert-dismissed`.

- [ ] **Step 6: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex011_|FullyQualifiedName~Ex012_|FullyQualifiedName~Ex013_|FullyQualifiedName~Ex014_|FullyQualifiedName~Ex015_"
```

Expected: all passed.

- [ ] **Step 7: Add five host demo pages, build the host in both modes**

ex013's page must close the generic explicitly: `<Ex013_TemplatedFragment TItem="string" Items="_items">` with a `<Row Context="item">` template child.

- [ ] **Step 8: Update `catalog.md` and commit**

Flip rows 011–015 to ✅, set `**Status: 15 ✅ / 85 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex011-ex015"
```

---

## Task 5: ex016–ex020

Lifecycle. These are the exercises where a careless test asserts on the first frame only.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex016_InlineStyleBinding.razor` … `Ex020_DisposableComponent.razor`
- Create: `blazor/exercises/_support/Ticker.cs` (and the identical copy under `blazor/solutions/_support/`)
- Create: the matching five solutions, five tests, five demo pages
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints.
- Produces: `FeWoLearning.Blazor.Support.Ticker` — see Step 1 for the exact shape; ex020's test reads `SubscriberCount`.

- [ ] **Step 1: Add the `Ticker` fixture to both RCLs**

```csharp
// exercises/_support/Ticker.cs and solutions/_support/Ticker.cs
namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for Ex020. Counts live subscribers so a test can prove a
/// component unsubscribed on dispose. Not an exercise.
/// </summary>
public sealed class Ticker
{
    private Action? _handlers;

    public int SubscriberCount { get; private set; }

    public void Subscribe(Action handler)
    {
        _handlers += handler;
        SubscriberCount++;
    }

    public void Unsubscribe(Action handler)
    {
        _handlers -= handler;
        SubscriberCount--;
    }

    public void Tick() => _handlers?.Invoke();
}
```

- [ ] **Step 2: Write the five stubs**

**ex016 `Ex016_InlineStyleBinding` — shape A.**
- Parameter: `[Parameter] public int Percent { get; set; }`.
- Markup: `<div id="track"><div id="bar" style="@BarStyle"></div></div>`.
- TODO: `private string BarStyle => throw new NotImplementedException("TODO: Ex016 - build the inline width style");` — `width: N%` where `N` is `Percent` clamped to `0..100`, formatted with `CultureInfo.InvariantCulture`.

**ex017 `Ex017_OnInitialized` — shape A.**
- Parameter: `[Parameter] public string User { get; set; } = "";`.
- Markup: `<span id="greeting">@_greeting</span>` with `private string _greeting = "";`.
- TODO: `protected override void OnInitialized() => throw new NotImplementedException("TODO: Ex017 - capture the greeting once");` — set `_greeting` to `Welcome, {User}` **once**. The comment must say: do not recompute it when parameters change; that is ex018's job.

**ex018 `Ex018_OnParametersSet` — shape A.**
- Parameter: `[Parameter] public string Title { get; set; } = "";`.
- Markup: `<span id="slug">@_slug</span>` with `private string _slug = "";`.
- TODO: `protected override void OnParametersSet() => throw new NotImplementedException("TODO: Ex018 - recompute the slug on every parameter change");` — lower-case, spaces to `-`, non-alphanumeric dropped, collapsed runs of `-`.

**ex019 `Ex019_OnAfterRenderFirst` — shape A.**
- Parameter: `[Parameter] public string Label { get; set; } = "";`.
- Public counters so a test can read them: `public int FirstRenderCount { get; private set; }`, `public int AfterRenderCount { get; private set; }`.
- Markup: `<span id="counts">@FirstRenderCount/@AfterRenderCount</span><span id="label">@Label</span>`.
- TODO: `protected override void OnAfterRender(bool firstRender) => throw new NotImplementedException("TODO: Ex019 - count renders, and first renders separately");`

**ex020 `Ex020_DisposableComponent` — shape A.** The file opens with `@implements IDisposable`.
- Parameter: `[Parameter, EditorRequired] public Ticker Ticker { get; set; } = default!;`.
- Markup: `<span id="ticks">@_ticks</span>` with `private int _ticks;`.
- TODOs, two of them: `protected override void OnInitialized() => throw new NotImplementedException("TODO: Ex020 - subscribe to the ticker");` and `public void Dispose() => throw new NotImplementedException("TODO: Ex020 - unsubscribe from the ticker");`. The comment must say the handler has to increment `_ticks` and call `StateHasChanged()`, and that subscribe/unsubscribe must use the **same** delegate instance.

- [ ] **Step 3: Write the five tests**

**`Ex016_InlineStyleBindingTests`** — four facts:
1. `Percent=42` → `Find("#bar").GetAttribute("style")` is `width: 42%`.
2. `Percent=-5` → `width: 0%`.
3. `Percent=150` → `width: 100%`.
4. With `CultureInfo.CurrentCulture` set to `de-DE` for the duration of the test (restored in `finally`), `Percent=42` still yields `width: 42%` — no decimal-comma artefacts.

Non-vacuity: an unclamped `$"width: {Percent}%"` passes 1 and 4 and fails 2 and 3.

**`Ex017_OnInitializedTests`** — two facts:
1. `User="Ada"` → `#greeting` text `Welcome, Ada`.
2. `User="Ada"`, then `cut.Render(p => p.Add(c => c.User, "Grace"))` → `#greeting` is **still** `Welcome, Ada`.

Non-vacuity: fact 2 is the whole point — an implementation in `OnParametersSet` passes 1 and fails 2.

**`Ex018_OnParametersSetTests`** — three facts:
1. `Title="Hello Blazor World"` → `#slug` text `hello-blazor-world`.
2. then `cut.Render(p => p.Add(c => c.Title, "Second Title"))` → `#slug` is `second-title`.
3. `Title="A -- B!"` → `#slug` is `a-b`.

Non-vacuity: fact 2 rejects an `OnInitialized` implementation; fact 3 rejects a naive `Replace(" ", "-").ToLower()`.

**`Ex019_OnAfterRenderFirstTests`** — two facts:
1. After the initial render: `cut.Instance.FirstRenderCount` is `1` and `cut.Instance.AfterRenderCount` is `1`.
2. After `cut.Render(p => p.Add(c => c.Label, "changed"))`: `FirstRenderCount` is **still** `1` and `AfterRenderCount` is `2`.

Wrap both in `cut.WaitForAssertion(...)` — `OnAfterRender` runs after the render completes, so a bare assertion can read the pre-callback value.

Non-vacuity: counting every render as a first render fails 2; ignoring `firstRender` entirely fails 1 or 2.

**`Ex020_DisposableComponentTests`** — three facts:
1. `Ticker` supplied → after render, `ticker.SubscriberCount` is `1`.
2. `ticker.Tick()` twice → `#ticks` text is `2` (inside `cut.WaitForAssertion(...)`; the increment happens on the ticker's callback, not in the render pass).
3. After `DisposeComponents()` (or disposing the `BunitContext`), `ticker.SubscriberCount` is `0`.

Non-vacuity: a component that subscribes but never unsubscribes passes 1 and 2 and fails 3 — the classic leak this exercise teaches.

- [ ] **Step 4: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex016_|FullyQualifiedName~Ex017_|FullyQualifiedName~Ex018_|FullyQualifiedName~Ex019_|FullyQualifiedName~Ex020_"
```

Expected: 0 passed, all failed with `TODO: ExNNN - ` messages.

- [ ] **Step 5: Write the five reference solutions**

Points that matter:

- **ex016:** `var clamped = Math.Clamp(Percent, 0, 100);` then `string.Create(CultureInfo.InvariantCulture, $"width: {clamped}%")` or `$"width: {clamped.ToString(CultureInfo.InvariantCulture)}%"`.
- **ex017:** assign in `OnInitialized` only. Do not also override `OnParametersSet`.
- **ex018:** assign in `OnParametersSet`. Slugify by mapping each char to lower-case, spaces and non-alphanumerics to `-`, then collapsing runs and trimming `-`.
- **ex019:** `if (firstRender) FirstRenderCount++; AfterRenderCount++;` — nothing else, and no `StateHasChanged()` (that would recurse).
- **ex020:** store the handler in a field so subscribe and unsubscribe use the same delegate:

```csharp
private Action? _onTick;

protected override void OnInitialized()
{
    _onTick = () => { _ticks++; StateHasChanged(); };
    Ticker.Subscribe(_onTick);
}

public void Dispose()
{
    if (_onTick is not null) Ticker.Unsubscribe(_onTick);
}
```

- [ ] **Step 6: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex016_|FullyQualifiedName~Ex017_|FullyQualifiedName~Ex018_|FullyQualifiedName~Ex019_|FullyQualifiedName~Ex020_"
```

Expected: all passed.

- [ ] **Step 7: Confirm ex017, ex019 and ex020 are not vacuous**

Three quick checks against the reference solutions, restoring each afterwards:
- Move ex017's assignment from `OnInitialized` to `OnParametersSet` → `Ex017` fact 2 must fail.
- Drop the `if (firstRender)` guard in ex019 → `Ex019` fact 2 must fail.
- Empty out ex020's `Dispose` body → `Ex020` fact 3 must fail.

- [ ] **Step 8: Add five host demo pages, build the host in both modes**

ex020's page owns a `Ticker` and a button calling `Tick()`, plus a checkbox that removes the component from the tree so disposal is observable by hand.

- [ ] **Step 9: Update `catalog.md` and commit**

Flip rows 016–020 to ✅, set `**Status: 20 ✅ / 80 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex016-ex020"
```

---

## Task 6: ex021–ex025

Event args, propagation, and `@bind` on local fields.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex021_EventArgsHandling.razor` … `Ex025_SelectBinding.razor`
- Create: the matching five solutions, five tests, five demo pages
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints.
- Produces: nothing later tasks depend on.

- [ ] **Step 1: Write the five stubs**

**ex021 `Ex021_EventArgsHandling` — shape A.**
- Markup: `<input id="k" @onkeydown="OnKey" /><span id="last">@_last</span>` with `private string _last = "";`.
- TODO: `private void OnKey(KeyboardEventArgs e) => throw new NotImplementedException("TODO: Ex021 - record the last non-modifier key, upper-cased");` — set `_last` to `e.Key.ToUpperInvariant()`, but leave it unchanged for the modifier keys `Shift`, `Control`, `Alt` and `Meta`.

**ex022 `Ex022_StopPropagation` — shape B.**
- Public counters so the test can read them: `public int OuterClicks { get; private set; }`, `public int InnerClicks { get; private set; }`.
- TODO markup comment: render `<div id="outer" @onclick="...">` containing `<button id="inner" @onclick="..." ...>Inner</button>`, plus `<span id="counts">@OuterClicks/@InnerClicks</span>`. Clicking `#inner` must increment `InnerClicks` **only** — the click must not bubble to `#outer`.
- Throwing `OnParametersSet`, `TODO: Ex022 - stop the inner click from bubbling`.

**ex023 `Ex023_InputTextBinding` — shape B.**
- No parameters. Local field `private string _note = "";`.
- TODO markup comment: render `<input id="note" ... />` two-way bound to `_note` with `@bind` (default `onchange` timing), plus `<span id="echo">@_note</span>` and `<span id="len">@_note.Length</span>`.
- Throwing `OnParametersSet`, `TODO: Ex023 - bind the input to the local field`.

**ex024 `Ex024_NumericInputParsing` — shape B.**
- Parameter: `[Parameter] public decimal UnitPrice { get; set; }`. Local field `private int _quantity;`.
- TODO markup comment: render `<input id="qty" type="number" ... />` bound to `_quantity` with `@bind`, plus `<span id="total">…</span>` showing `_quantity * UnitPrice` formatted with `CultureInfo.InvariantCulture`. Unparsable input must leave `_quantity` unchanged — that is `@bind`'s own behaviour, not something to hand-code.
- Throwing `OnParametersSet`, `TODO: Ex024 - bind the quantity and show the total`.

**ex025 `Ex025_SelectBinding` — shape B.**
- Parameter: `[Parameter] public IReadOnlyList<string> Options { get; set; } = [];`. Local field `private string _selected = "";`.
- TODO markup comment: render `<select id="prio" ...>` bound to `_selected` with `@bind`, holding one `<option value="@o">@o</option>` per option in order, plus `<span id="chosen">@_selected</span>`.
- Throwing `OnParametersSet`, `TODO: Ex025 - bind the select to the local field`.

- [ ] **Step 2: Write the five tests**

**`Ex021_EventArgsHandlingTests`** — four facts, all post-event assertions inside `cut.WaitForAssertion(...)`:
1. `cut.Find("#k").KeyDown("a")` → `#last` text `A`.
2. `.KeyDown("Enter")` → `ENTER`.
3. `.KeyDown("a")` then `.KeyDown("Shift")` → `#last` is still `A`.
4. `.KeyDown("Shift")` as the very first event → `#last` is empty.

Non-vacuity: an implementation without the modifier filter passes 1 and 2 and fails 3 and 4.

**`Ex022_StopPropagationTests`** — three facts:
1. `cut.Find("#inner").Click()` → `cut.Instance.InnerClicks` is `1` **and** `cut.Instance.OuterClicks` is `0`.
2. `cut.Find("#outer").Click()` → `OuterClicks` is `1`, `InnerClicks` is `0`.
3. `#counts` text reflects both counters after fact 1's click.

Non-vacuity: omitting `@onclick:stopPropagation` makes fact 1 report `OuterClicks == 1` and fail. **This is exactly why the slug changed from `PreventDefault`** — see Global Constraints → Deviations.

**`Ex023_InputTextBindingTests`** — three facts:
1. Initially `#echo` is empty and `#len` is `0`.
2. `cut.Find("#note").Change("hi")` → `#echo` is `hi` and `#len` is `2`.
3. `.Change("")` after fact 2 → `#echo` empty, `#len` `0`.

Non-vacuity: a one-way `value="@_note"` with no handler fails 2.

**`Ex024_NumericInputParsingTests`** — four facts:
1. `UnitPrice=2.5m`, `.Change("3")` on `#qty` → `#total` text `7.5`.
2. `UnitPrice=2.5m`, no interaction → `#total` text `0`.
3. `UnitPrice=2.5m`, `.Change("3")` then `.Change("abc")` → `#total` is **still** `7.5`.
4. `UnitPrice=2.5m`, `.Change("3")` with `CultureInfo.CurrentCulture` set to `de-DE` (restored in `finally`) → `#total` is `7.5`, not `7,5`.

Non-vacuity: formatting without `InvariantCulture` passes 1–3 and fails 4.

**`Ex025_SelectBindingTests`** — three facts:
1. `Options=["Low","Normal","High"]` → `FindAll("#prio option")` has three entries with texts and `value` attributes in that order.
2. `.Change("High")` on `#prio` → `#chosen` text `High`.
3. `Options=[]` → `#prio` exists with no `option` children.

Non-vacuity: hard-coded options fail 1 and 3.

- [ ] **Step 3: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex021_|FullyQualifiedName~Ex022_|FullyQualifiedName~Ex023_|FullyQualifiedName~Ex024_|FullyQualifiedName~Ex025_"
```

Expected: 0 passed, all failed with `TODO: ExNNN - ` messages.

- [ ] **Step 4: Write the five reference solutions**

Points that matter:

- **ex021:** filter with a small `static readonly string[] Modifiers = ["Shift", "Control", "Alt", "Meta"];` and an early return.
- **ex022:** `<button id="inner" @onclick="OnInner" @onclick:stopPropagation>`. Both handlers just increment.
- **ex023:** `<input id="note" @bind="_note" />` — plain `@bind` to a field is legal and idiomatic; `@bind` to a `[Parameter]` is not, which is why ex008 uses the explicit contract instead.
- **ex024:** `<input id="qty" type="number" @bind="_quantity" />` and `<span id="total">@((_quantity * UnitPrice).ToString(CultureInfo.InvariantCulture))</span>`.
- **ex025:** `<select id="prio" @bind="_selected">` with `@foreach (var o in Options) { <option value="@o">@o</option> }`.

- [ ] **Step 5: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex021_|FullyQualifiedName~Ex022_|FullyQualifiedName~Ex023_|FullyQualifiedName~Ex024_|FullyQualifiedName~Ex025_"
```

Expected: all passed.

- [ ] **Step 6: Confirm ex022 and ex024 are not vacuous**

- Remove `@onclick:stopPropagation` from ex022's solution → `Ex022` fact 1 must fail. Restore.
- Drop `CultureInfo.InvariantCulture` from ex024's solution → `Ex024` fact 4 must fail. Restore.

- [ ] **Step 7: Add five host demo pages, build the host in both modes**

All five need `@rendermode InteractiveServer`.

- [ ] **Step 8: Update `catalog.md` and commit**

Flip rows 021–025 to ✅, set `**Status: 25 ✅ / 75 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex021-ex025"
```

---

## Task 7: ex026–ex030

Multi-selection state and cascading values.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex026_CheckboxGroup.razor` … `Ex030_ComponentComposition.razor`
- Create: `blazor/exercises/01-beginner/Ex028_CascadingValueBasics_Label.razor`, `Ex029_NamedCascadingValue_Label.razor`, `Ex030_ComponentComposition_Item.razor` (and the matching solutions)
- Create: the matching solutions, five test files, five demo pages
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints.
- Produces: nothing later tasks depend on, but Task 8's ex035 follows the same **multi-file exercise convention** established here: an exercise needing more than one component uses `ExNNN_<Slug>.razor` for the entry point and `ExNNN_<Slug>_<Part>.razor` for each additional component, all in the same tier folder and namespace. Record this convention in `blazor/README.md` in Task 9.

- [ ] **Step 1: Write the stubs**

**ex026 `Ex026_CheckboxGroup` — shape B.**
- Parameters: `[Parameter] public IReadOnlyList<string> Features { get; set; } = [];`, `[Parameter] public EventCallback<IReadOnlyList<string>> SelectionChanged { get; set; }`.
- Local state: a set of selected feature names.
- TODO markup comment: render one `<input type="checkbox" id="feature-N" />` per feature (`N` is the zero-based index) with a `<label for="feature-N">@feature</label>`, plus `<span id="selected">…</span>` listing the selected features joined by `", "` **in `Features` order, not click order**. Toggling reports the selection through `SelectionChanged`.
- Throwing `OnParametersSet`, `TODO: Ex026 - track the checkbox selection`.

**ex027 `Ex027_RadioGroup` — shape B.**
- Parameters: `[Parameter] public IReadOnlyList<string> Sizes { get; set; } = [];`.
- Local state: the selected size.
- TODO markup comment: render one `<input type="radio" name="size" id="size-N" />` per size, checked only for the selected one, plus `<span id="chosen">@_selected</span>`. Selecting one must clear the others.
- Throwing `OnParametersSet`, `TODO: Ex027 - track the single radio selection`.

**ex028 — two files, both shape B.**
- `Ex028_CascadingValueBasics.razor`: `[Parameter] public string Theme { get; set; } = "";`, `[Parameter] public RenderFragment? ChildContent { get; set; }`. TODO markup comment: wrap `ChildContent` in a `<CascadingValue Value="Theme">`. Throwing `OnParametersSet`, `TODO: Ex028 - cascade the theme`.
- `Ex028_CascadingValueBasics_Label.razor`: `[CascadingParameter] public string? Theme { get; set; }`, `[Parameter] public string Text { get; set; } = "";`. TODO markup comment: render `<span id="themed" class="theme-@(Theme ?? "none")">@Text</span>`. Throwing `OnParametersSet`, `TODO: Ex028 - consume the cascaded theme`.

**ex029 — two files, both shape B.** Two cascading values of the **same type**, so only `Name` can tell them apart.
- `Ex029_NamedCascadingValue.razor`: `[Parameter] public string Locale { get; set; } = "";`, `[Parameter] public string Currency { get; set; } = "";`, `ChildContent`. TODO markup comment: cascade both as named values, `Name="Locale"` and `Name="Currency"`. Throwing `OnParametersSet`, `TODO: Ex029 - cascade both values by name`.
- `Ex029_NamedCascadingValue_Label.razor`: `[CascadingParameter(Name = "Locale")] public string? Locale { get; set; }`, `[CascadingParameter(Name = "Currency")] public string? Currency { get; set; }`. TODO markup comment: render `<span id="locale">@Locale</span><span id="currency">@Currency</span>`. Throwing `OnParametersSet`, `TODO: Ex029 - consume both named values`.

**ex030 — two files, both shape B.** A child registering itself with its parent.
- `Ex030_ComponentComposition.razor`: `[Parameter] public RenderFragment? ChildContent { get; set; }`, plus `public void Register(string label)` and an ordered list of registered labels. TODO markup comment: cascade `this` so children can find it, render `ChildContent` (so children register), then render `<nav id="crumbs">` with one `<span class="crumb">@label</span>` per registered label separated by `" / "`, where the last one additionally carries the class `current`. Throwing `OnParametersSet`, `TODO: Ex030 - collect and render the registered crumbs`.
- `Ex030_ComponentComposition_Item.razor`: `[CascadingParameter] public Ex030_ComponentComposition? Parent { get; set; }`, `[Parameter] public string Label { get; set; } = "";`. TODO: `protected override void OnInitialized() => throw new NotImplementedException("TODO: Ex030 - register this item with its parent");` — the item renders nothing itself.

  Note for the implementer: registering during a child's `OnInitialized` mutates the parent's state mid-render, so the parent must re-render afterwards. Have `Register` call `StateHasChanged()` on the parent; the reference solution does this and the test's `WaitForAssertion` accommodates the extra render pass.

- [ ] **Step 2: Write the five tests**

**`Ex026_CheckboxGroupTests`** — four facts, post-click assertions inside `cut.WaitForAssertion(...)`:
1. `Features=["a","b","c"]` → three checkboxes, `#selected` text empty.
2. Change `#feature-2` to checked, then `#feature-0` to checked → `#selected` text is `a, c` (declaration order, **not** click order).
3. Uncheck `#feature-0` → `#selected` is `c`.
4. A `SelectionChanged` callback receives `["a","c"]` after fact 2's clicks.

Toggle with `cut.Find("#feature-2").Change(true)`.

Non-vacuity: appending to a `List<string>` on click passes 1, 3, 4 and fails 2.

**`Ex027_RadioGroupTests`** — three facts:
1. `Sizes=["S","M","L"]` → three radios, `#chosen` empty, none checked.
2. `.Change(true)` on `#size-1` → `#chosen` is `M`, and `Find("#size-1").HasAttribute("checked")` is true while `#size-0` and `#size-2` are not checked.
3. Then `.Change(true)` on `#size-2` → `#chosen` is `L` and `#size-1` is no longer checked.

Non-vacuity: independent per-radio booleans pass 1 and 2 and fail 3.

**`Ex028_CascadingValueBasicsTests`** — two facts:
1. Render `Ex028_CascadingValueBasics` with `Theme="dark"` and a `ChildContent` containing `Ex028_CascadingValueBasics_Label` with `Text="hi"` → `Find("#themed")` has class `theme-dark` and text `hi`.
2. Render the label **alone**, with no provider → class is `theme-none`.

Supply the child with `p.AddChildContent<Ex028_CascadingValueBasics_Label>(cp => cp.Add(c => c.Text, "hi"))`.

Non-vacuity: a label hard-coding `theme-dark` fails 2; a provider that renders `ChildContent` without a `CascadingValue` fails 1.

**`Ex029_NamedCascadingValueTests`** — two facts:
1. Provider with `Locale="de-DE"` and `Currency="EUR"` wrapping the label → `#locale` text `de-DE` and `#currency` text `EUR`.
2. Provider with the two values **swapped** (`Locale="EUR"`, `Currency="de-DE"`) → `#locale` text `EUR`. This pins name-matching rather than accidental ordering.

Non-vacuity: two unnamed `CascadingValue`s of type `string` would resolve ambiguously and cannot satisfy both facts.

**`Ex030_ComponentCompositionTests`** — three facts, inside `cut.WaitForAssertion(...)`:
1. Three items with labels `Home`, `Docs`, `Api` → `FindAll("#crumbs span.crumb")` has three entries with those texts in order.
2. The **last** crumb's class list contains `current`; the first two do not.
3. `#crumbs` text contains `Home / Docs / Api`.

Non-vacuity: rendering the `ChildContent` directly instead of collecting registrations produces no `.crumb` elements at all and fails 1.

- [ ] **Step 3: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex026_|FullyQualifiedName~Ex027_|FullyQualifiedName~Ex028_|FullyQualifiedName~Ex029_|FullyQualifiedName~Ex030_"
```

Expected: 0 passed, all failed with `TODO: ExNNN - ` messages.

- [ ] **Step 4: Write the reference solutions**

Points that matter:

- **ex026:** keep a `HashSet<string> _selected` for membership but always project through `Features.Where(_selected.Contains)` when rendering or reporting, so ordering comes from `Features`.
- **ex027:** a single `string? _selected` field; `checked="@(size == _selected)"`.
- **ex028:** `<CascadingValue Value="Theme">@ChildContent</CascadingValue>`.
- **ex029:** two nested `<CascadingValue Value="Locale" Name="Locale">` / `<CascadingValue Value="Currency" Name="Currency">`.
- **ex030:** cascade `this` with `<CascadingValue Value="this" IsFixed="true">`, collect labels in a `List<string>`, and have `Register` append then call `StateHasChanged()`. Render the separator between crumbs, not after the last one.

- [ ] **Step 5: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex026_|FullyQualifiedName~Ex027_|FullyQualifiedName~Ex028_|FullyQualifiedName~Ex029_|FullyQualifiedName~Ex030_"
```

Expected: all passed.

- [ ] **Step 6: Confirm ex026 and ex029 are not vacuous**

- Change ex026's solution to append to a `List<string>` in click order → `Ex026` fact 2 must fail. Restore.
- Drop the `Name` attributes from ex029's solution → `Ex029` must fail (ambiguous or wrong resolution). Restore.

- [ ] **Step 7: Add five host demo pages, build the host in both modes**

- [ ] **Step 8: Update `catalog.md` and commit**

Flip rows 026–030 to ✅, set `**Status: 30 ✅ / 70 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex026-ex030"
```

---

## Task 8: ex031–ex035

Last batch of the tier, ending in the tabs capstone.

**Files:**
- Create: `blazor/exercises/01-beginner/Ex031_ChildToParentCallback.razor` … `Ex035_TabsComposition.razor`
- Create: `blazor/exercises/01-beginner/Ex035_TabsComposition_Tab.razor` (and its solution)
- Create: `blazor/exercises/_support/AddButton.razor`, `blazor/exercises/_support/Level2.razor`, `blazor/exercises/_support/Level3.razor` (and identical copies under `blazor/solutions/_support/`)
- Create: the matching solutions, five test files, five demo pages
- Modify: `blazor/catalog.md`

**Interfaces:**
- Consumes: Global Constraints; the multi-file exercise convention from Task 7.
- Produces: `FeWoLearning.Blazor.Support.AddButton`, `.Level2`, `.Level3` — see Step 1.

- [ ] **Step 1: Add the three `_support` fixtures to both RCLs**

`_support/AddButton.razor` — a stateless child used by ex031:

```razor
@* Test fixture, not an exercise. Raises an EventCallback<int> on click. *@
<button class="add" data-amount="@Amount" @onclick="RaiseAsync">+@Amount</button>

@code {
    [Parameter] public int Amount { get; set; }
    [Parameter] public EventCallback<int> OnAdd { get; set; }

    private Task RaiseAsync() => OnAdd.InvokeAsync(Amount);
}
```

`_support/Level2.razor` and `_support/Level3.razor` — used by ex034 to make the three-level chain observable. `Level3` renders the leaf; `Level2` must pass the parameter on explicitly:

```razor
@* _support/Level2.razor - test fixture, not an exercise. *@
<div class="level-2">
    <Level3 Message="@Message" />
</div>

@code {
    [Parameter] public string Message { get; set; } = "";
}
```

```razor
@* _support/Level3.razor - test fixture, not an exercise. *@
<span id="leaf">@Message</span>

@code {
    [Parameter] public string Message { get; set; } = "";
}
```

- [ ] **Step 2: Write the stubs**

**ex031 `Ex031_ChildToParentCallback` — shape B.** The drill: an `EventCallback` handler re-renders the parent with no manual `StateHasChanged`.
- No parameters. Local `private int _total;`.
- TODO markup comment: render `<span id="total">@_total</span>` and three `<AddButton Amount="N" OnAdd="..." />` children with amounts `1`, `5` and `10`, whose callback adds the reported amount to `_total`. Note in the comment: do **not** call `StateHasChanged()` — an `EventCallback` handler already triggers a re-render, and the test proves it.
- Throwing `OnParametersSet`, `TODO: Ex031 - accumulate the amounts reported by the children`.

**ex032 `Ex032_MarkupStringRendering` — shape A.**
- Parameters: `[Parameter] public string Html { get; set; } = "";`, `[Parameter] public bool AllowHtml { get; set; }`.
- Markup: `<div id="rich">@Rendered</div>`.
- TODO: `private object Rendered => throw new NotImplementedException("TODO: Ex032 - return raw markup only when it is allowed");` — return `new MarkupString(Html)` when `AllowHtml`, otherwise the plain `Html` string so Blazor escapes it. The comment must note that the return type is `object` on purpose, because the two branches have different types.

**ex033 `Ex033_EmptyStateFallback` — shape B.**
- Parameters: `[Parameter] public IReadOnlyList<string> Results { get; set; } = [];`, `[Parameter] public string Query { get; set; } = "";`.
- TODO markup comment: exactly one of three states — `<p id="prompt">Type to search</p>` when `Query` is blank; otherwise `<ul id="results">` with one `<li class="hit">` per result; otherwise `<p id="no-results">No results for "@Query"</p>` when `Query` is set and `Results` is empty. The double quotes around the query are part of the copy.
- Throwing `OnParametersSet`, `TODO: Ex033 - render the right one of three states`.

**ex034 `Ex034_NestedParameterFlow` — shape B.**
- Parameter: `[Parameter] public string Message { get; set; } = "";`.
- TODO markup comment: render `<div class="level-1">` containing a `<Level2 />` so that the leaf `#leaf` shows `Message`. Note in the comment that parameters do **not** flow implicitly: each level passes the value on explicitly, and `Level2` already does its half.
- Throwing `OnParametersSet`, `TODO: Ex034 - pass the message down the chain`.

**ex035 — two files, both shape B.** The capstone.
- `Ex035_TabsComposition.razor`: `[Parameter] public RenderFragment? ChildContent { get; set; }`, plus `public void Register(Ex035_TabsComposition_Tab tab)` and an ordered tab list. TODO markup comment: cascade `this`, render `ChildContent` so the tabs register, then render `<div id="tabs">` with one `<button class="tab" id="tab-N">@tab.Title</button>` per tab (the active one additionally carrying class `active`), and `<div id="tab-panel">` holding **only** the active tab's `ChildContent`. Clicking a header activates that tab. The first registered tab is active initially. Throwing `OnParametersSet`, `TODO: Ex035 - render the tab headers and the active panel`.
- `Ex035_TabsComposition_Tab.razor`: `[CascadingParameter] public Ex035_TabsComposition? Parent { get; set; }`, `[Parameter] public string Title { get; set; } = "";`, `[Parameter] public RenderFragment? ChildContent { get; set; }`. It renders nothing itself; the parent renders its `ChildContent`. TODO: `protected override void OnInitialized() => throw new NotImplementedException("TODO: Ex035 - register this tab with its parent");` — expose `Title` and `ChildContent` publicly so the parent can read them.

- [ ] **Step 3: Write the five tests**

**`Ex031_ChildToParentCallbackTests`** — three facts, inside `cut.WaitForAssertion(...)`:
1. Initially `#total` text is `0`, and `FindAll("button.add")` has three entries with `data-amount` `1`, `5`, `10`.
2. Click the `+5` button → `#total` is `5`.
3. Click `+5`, `+10`, `+1` → `#total` is `16`.

Select a specific child with `cut.Find("button.add[data-amount='5']")`.

Non-vacuity: a parent that replaces rather than accumulates passes 1 and 2 and fails 3.

**`Ex032_MarkupStringRenderingTests`** — three facts:
1. `Html="<b>hi</b>"`, `AllowHtml=false` → `FindAll("#rich b")` is **empty** and `Find("#rich").TextContent` is `<b>hi</b>`.
2. `Html="<b>hi</b>"`, `AllowHtml=true` → `Find("#rich b").TextContent` is `hi`.
3. `Html=""`, `AllowHtml=true` → `#rich` exists and its `TextContent` is empty.

Non-vacuity: always returning `new MarkupString(Html)` passes 2 and 3 and fails 1 — which is the injection mistake this exercise is about.

**`Ex033_EmptyStateFallbackTests`** — four facts:
1. `Query=""` → `#prompt` text `Type to search`; `FindAll("#results")` and `FindAll("#no-results")` empty.
2. `Query="bl"`, `Results=["blazor","blue"]` → two `li.hit` with those texts in order; no `#prompt`, no `#no-results`.
3. `Query="zzz"`, `Results=[]` → `#no-results` text is exactly `No results for "zzz"`.
4. `Query="   "`, `Results=[]` → `#prompt` (blank query counts as no query).

Non-vacuity: checking `Results.Count == 0` before the query state makes fact 1 render `#no-results` and fail.

**`Ex034_NestedParameterFlowTests`** — two facts:
1. `Message="deep"` → `Find("#leaf").TextContent` is `deep`, and `Find(".level-1 .level-2 #leaf")` resolves (the chain is really three levels).
2. Then `cut.Render(p => p.Add(c => c.Message, "deeper"))` → `#leaf` is `deeper`.

Non-vacuity: rendering `Message` directly at level 1 fails the descendant selector in fact 1.

**`Ex035_TabsCompositionTests`** — four facts, inside `cut.WaitForAssertion(...)`:
1. Three tabs (`One`, `Two`, `Three`, with panel contents `first`, `second`, `third`) → `FindAll("#tabs button.tab")` has three entries with those titles in order.
2. Initially `#tab-0` class list contains `active`, `#tab-1` and `#tab-2` do not, and `#tab-panel` text is `first`.
3. `cut.Find("#tab-1").Click()` → `#tab-panel` text is `second`, `#tab-1` is `active`, `#tab-0` is not.
4. After fact 3, `FindAll("#tab-panel")` still has exactly one entry, and its text does **not** contain `first` — only the active panel is rendered.

Compose the tabs in the test with `p.AddChildContent<Ex035_TabsComposition_Tab>(...)` three times, each adding its own `Title` and child content.

Non-vacuity: rendering all panels and merely hiding the inactive ones with CSS fails fact 4.

- [ ] **Step 4: Red check**

```bash
cd blazor
dotnet test --filter "FullyQualifiedName~Ex031_|FullyQualifiedName~Ex032_|FullyQualifiedName~Ex033_|FullyQualifiedName~Ex034_|FullyQualifiedName~Ex035_"
```

Expected: 0 passed, all failed with `TODO: ExNNN - ` messages.

- [ ] **Step 5: Write the reference solutions**

Points that matter:

- **ex031:** `private void Add(int amount) => _total += amount;` wired as `OnAdd="Add"`. No `StateHasChanged()`.
- **ex032:** `private object Rendered => AllowHtml ? new MarkupString(Html) : Html;`
- **ex033:** order the branches query-blank → results-present → no-results.
- **ex034:** `<div class="level-1"><Level2 Message="@Message" /></div>`.
- **ex035:** same registration pattern as ex030 — cascade `this` with `IsFixed="true"`, collect tabs in a `List<Ex035_TabsComposition_Tab>`, `Register` appends and calls `StateHasChanged()`, and the active index defaults to `0`. Render only `_tabs[_active].ChildContent` inside `#tab-panel`.

- [ ] **Step 6: Green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex031_|FullyQualifiedName~Ex032_|FullyQualifiedName~Ex033_|FullyQualifiedName~Ex034_|FullyQualifiedName~Ex035_"
```

Expected: all passed.

- [ ] **Step 7: Confirm ex032 and ex035 are not vacuous**

- Make ex032's solution always return `new MarkupString(Html)` → `Ex032` fact 1 must fail. Restore.
- Make ex035's solution render every tab's panel → `Ex035` fact 4 must fail. Restore.

- [ ] **Step 8: Add five host demo pages, build the host in both modes**

ex035's page composes three `<Ex035_TabsComposition_Tab>` children inside `<Ex035_TabsComposition>` and needs `@rendermode InteractiveServer`.

- [ ] **Step 9: Update `catalog.md` and commit**

Flip rows 031–035 to ✅, set `**Status: 35 ✅ / 65 ⬜**`.

```bash
git add blazor/exercises blazor/solutions blazor/tests blazor/host blazor/catalog.md
git commit -m "blazor: ex031-ex035"
```

---

## Task 9: Tier verification and documentation

Closes out the delivery: the whole tier verified in one run, the host checked over HTTP, and both `README.md` and `CLAUDE.md` written.

**Files:**
- Create: `blazor/README.md`
- Modify: `CLAUDE.md` (repo root)
- Modify: `docs/superpowers/specs/2026-09-04-blazor-track-design.md` (§6 slug list, to match the two deviations)
- Modify: `README.md` (repo root), if it carries the per-track table

**Interfaces:**
- Consumes: everything from Tasks 1–8.
- Produces: the track's documented entry points.

- [ ] **Step 1: Full-tier red check**

Run the whole suite unfiltered:

```bash
cd blazor
dotnet test
```

Expected: **35 test classes, 0 passed, every test failed**, and every failure a `TODO: ExNNN - ` `NotImplementedException`. Zero compile errors. Capture the summary line for the commit message.

- [ ] **Step 2: Full-tier green check**

```bash
cd blazor
dotnet test -p:UseSolutions=true
```

Expected: **all passed, 0 failed.** Capture the counts.

- [ ] **Step 3: Host HTTP spot-check, both modes**

Start the host in solutions mode, fetch three pages, stop it:

```bash
cd blazor
dotnet run --project host -p:UseSolutions=true &
HOST_PID=$!
# wait for the port instead of sleeping blindly
until curl -sf http://localhost:5199/ > /dev/null; do sleep 1; done
curl -s http://localhost:5199/beginner/001 | grep -q "Hello, Blazor!" && echo "ex001 OK"
curl -s http://localhost:5199/beginner/004 | grep -q "blazor" && echo "ex004 OK"
curl -s http://localhost:5199/beginner/033 | grep -q "Type to search" && echo "ex033 OK"
kill $HOST_PID
```

Then the same in exercises mode, where those pages must instead surface the exercise's `NotImplementedException`:

```bash
cd blazor
dotnet run --project host &
HOST_PID=$!
until curl -sf http://localhost:5199/ > /dev/null; do sleep 1; done
curl -s http://localhost:5199/beginner/001 | grep -qi "error" && echo "ex001 stub surfaces an error, as expected"
kill $HOST_PID
```

If a page 500s in solutions mode, fix the demo page — not the solution, whose proof is the test suite.

- [ ] **Step 4: Write `blazor/README.md`**

It must cover, each in its own short section:

1. **What this track is** — 100 Blazor component exercises; Blazor beginner, not C# beginner; plain C# drills live in `dotnet/`.
2. **Prerequisites** — .NET 10 SDK (10.0.400 verified); nuget.org reachable for `bunit` on first restore.
3. **Commands** — the full table from Global Constraints, stressing that everything runs from inside `blazor/`.
4. **How an exercise works** — the two stub shapes, why shape B exists (`throw` is illegal in Razor markup, `CS8115`), and that a solution deletes the throwing member.
5. **Layout** — the per-exercise file layout, the tier-namespace pinning via folder-level `_Imports.razor`, the multi-file exercise convention (`ExNNN_<Slug>_<Part>.razor`), and the `_support/` fixture rule (identical in both RCLs, never a TODO, never in the catalog).
6. **Why `solutions/` is a real project here** — the deviation from `CLAUDE.md`, its justification (no project references both RCLs, so the collision the convention guards against cannot occur), and its benefit (solutions are compile-checked, so the silent-drift failure class that the 2026-08-03 audit found in `vue/` and `go/` cannot occur in this track). Say plainly: do not "fix" this back.
7. **Non-goals** — no real WebAssembly loading, no real SignalR reconnects, no real `focus()`/`scrollIntoView`. JS-dependent exercises assert against bUnit's `JSInterop` mock, never browser behaviour. **A green test here is not evidence of browser behaviour.**
8. **bUnit 2 API notes** — `BunitContext` not `TestContext`; `cut.Render(...)` not `SetParametersAndRender`; `Find`/`FindAll` return wrappers, so prove identity through `FindComponents<T>()[i].Instance`.
9. **Test-quality rules** — the four rules from Global Constraints, including the mandatory non-vacuity question.

- [ ] **Step 5: Update `CLAUDE.md`**

Five edits, all additive:

1. **"What this repository is"** — the opening sentence says "nine independent, self-contained learning tracks" and lists them. A `php/` track also exists and is unlisted; add both `blazor/` and `php/` to the list and correct the count accordingly.
2. **Per-track commands table** — add a `blazor/` row: install `—` (restore on first `dotnet test`), run all tests `dotnet test`, run one `dotnet test --filter FullyQualifiedName~Ex001_`. Add a note under the table that `dotnet test -p:UseSolutions=true` runs the same suite against the reference solutions.
3. **Toolchain status** — add `blazor/` to the verified list with the date, naming .NET 10.0.400 and bUnit 2.9.0, and stating that it is verified end-to-end (stubs red, solutions green) unlike `java/`, `kotlin/`, `flutter/` and `php/`.
4. **Track-specific gotchas** — add a `**Blazor**` bullet: `.slnx`; four projects; the `UseSolutions` switch and why `ArtifactsPath` is required; tier namespaces pinned by folder-level `_Imports.razor` because `01-beginner` is not an identifier; a component's type name is its file name; the two stub shapes and the `CS8115` reason for shape B; bUnit 2's `BunitContext` / `cut.Render` renames; the `FrameworkReference` requirement; and the `_support/` fixture rule.
5. **Current state and Known gaps** — add a `blazor/` row reading `35 / 100 (verified)` with `65` remaining, and note in "Known gaps" that `blazor/` is the one track where `solutions/` **is** in the build and therefore cannot drift silently.

- [ ] **Step 6: Reconcile the spec**

Update §6 of `docs/superpowers/specs/2026-09-04-blazor-track-design.md` so its slug list matches what was built: `022 StopPropagation` instead of `PreventDefault`, and a parenthetical on `023 InputTextBinding` noting it is `@bind` to a local field. Add a line to §6's non-goals recording that `preventDefault` moves to the intermediate tier because it is unobservable in bUnit.

- [ ] **Step 7: Update the root `README.md`**

If the root README carries a per-track table or exercise counts (a recent commit refreshed it for `flutter/`), add `blazor/` with `35 / 100` and mention the `UseSolutions` switch in one line. If it has no such table, skip this step and say so.

- [ ] **Step 8: Final verification, then commit**

Re-run both gates one last time and confirm the numbers match Steps 1 and 2:

```bash
cd blazor
dotnet test
dotnet test -p:UseSolutions=true
```

Then:

```bash
git add blazor/README.md CLAUDE.md README.md docs/superpowers/specs/2026-09-04-blazor-track-design.md
git commit -m "blazor: document track, update CLAUDE.md, close beginner tier at 35/100"
```

Report the exact test counts from both runs. Do not claim the tier is complete without those two numbers in hand.

---

## Self-Review

**Spec coverage.** §1 purpose → Task 1 Step 7 catalog header, Task 9 Step 4 README §1. §2 toolchain → Global Constraints "Versions", Task 1 Steps 1/3. §2.1 both constraints → Global Constraints stub shapes A and B, and the bUnit API table. §3 structure → File Structure, Task 1 Steps 1–5. §4 mechanism → Task 1 Steps 2–4. §4.1 commands → Global Constraints "Commands". §5 deviation → Task 9 Steps 4.6 and 5.4. §6 tier themes → Task 1 Step 7. §6 beginner slugs → Tasks 2–8. §6 non-goals → Task 9 Step 4.7. §7 test rules → Global Constraints "Test-quality rules", plus a named non-vacuity check on every exercise. §8 definition of done → Task 1 Step 6 (1), Task 1 Step 7 and each batch's catalog step (2), Task 9 Steps 1–2 (3, 4), Task 9 Step 3 (5), Task 9 Step 4 (6), Task 9 Step 5 (7), each batch's commit step (8).

**Two spec deviations, recorded rather than hidden:** ex022 `PreventDefault` → `StopPropagation`, and ex023 narrowed to local-field `@bind`. Both are stated in Global Constraints → Deviations, and Task 9 Step 6 reconciles the spec text.

**One spec gap found and closed:** the spec did not say what to do about `php/` missing from `CLAUDE.md`. Task 9 Step 5.1 folds it in, since that edit touches the same sentence.

**Type consistency check.** `Person(int Id, string Name)` — produced in Task 2, consumed by Task 2's ex005 test. `RosterEntry.Person` public — produced in Task 2, read as `.Instance.Person.Id` in the same task. `AlertSeverity { Info, Warning, Danger }` — produced in Task 4 Step 1, used by ex015 and its test. `Ticker.Subscribe/Unsubscribe/Tick/SubscriberCount` — produced in Task 5 Step 1, used by ex020 and its test. `AddButton.Amount/OnAdd` — produced in Task 8 Step 1, used by ex031 and its test. `Level2.Message`/`Level3.Message` — produced in Task 8 Step 1, used by ex034. `Ex030_ComponentComposition.Register(string)` and `Ex035_TabsComposition.Register(Ex035_TabsComposition_Tab)` — each defined and consumed inside its own task. Public members that tests read (`Ex019.FirstRenderCount`/`AfterRenderCount`, `Ex022.OuterClicks`/`InnerClicks`) are declared `public` in their stubs, so the tests compile against the stub and the red run reports failures rather than compile errors.
