# Security Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build `security/`, a 60-exercise C# application-security track covering ASP.NET Core, Blazor, desktop class libraries and WPF, following the repo's red/green stub-and-solution pattern.

**Architecture:** Three projects — `exercises/`, `solutions/`, `tests/` — where the two content libraries compile the same type names into the same namespaces and `tests/` references exactly one of them through the `UseSolutions` MSBuild property. `dotnet build` plus running the test executable is the red run; `dotnet build -p:UseSolutions=true` plus the same executable is the green run. One `net10.0-windows` test project hosts all three harnesses (ASP.NET `TestServer`, bUnit, `Xunit.StaFact`), verified by probe before this plan was written.

**Tech Stack:** .NET 10.0.400, `net10.0-windows` with `UseWPF=true`, `Microsoft.NET.Sdk.Razor` for content libraries, xunit.v3 4.0.0, `Xunit.StaFact` 4.0.23, bUnit 2.9.0, `Microsoft.AspNetCore.TestHost` 10.0.0, `Microsoft.Data.Sqlite` 10.0.0, `Microsoft.IdentityModel.JsonWebTokens` 8.16.0.

**Spec:** [`docs/superpowers/specs/2026-09-05-security-track-design.md`](../specs/2026-09-05-security-track-design.md)

## Global Constraints

Every task's requirements implicitly include this section. Values are copied verbatim from the spec.

- **TFM for all three projects:** `net10.0-windows`, with `<UseWPF>true</UseWPF>`, `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`.
- **Content library SDK:** `Microsoft.NET.Sdk.Razor`. **Test project SDK:** `Microsoft.NET.Sdk` — *not* Razor. (Spec §3.)
- **`exercises/*.csproj` and `solutions/*.csproj` are byte-identical except `AssemblyName`.** `RootNamespace` is `FeWoLearning.Security.Exercises` in **both**.
- **Never add `PackageReference` for `Microsoft.Extensions.Hosting` or `System.Security.Cryptography.ProtectedData`.** Both are in the shared framework for this TFM; referencing them emits `NU1510` on every build. (Spec §2.2.)
- **`SQLitePCLRaw.lib.e_sqlite3` is pinned to `2.1.13`** in both content libraries. Without the pin, `Microsoft.Data.Sqlite` 10.0.0 drags in 2.1.11 and every build emits `NU1903` for GHSA-2m69-gcr7-jv3q. (Spec §2.3.)
- **`solutions/` must build with zero warnings.** `exercises/` may emit `CS0169`/`CS0414`/`CS0649` from shape-B stubs; leave those unsuppressed.
- **Verification is `dotnet build`, then run the test executable directly.** `dotnet test` reports "no tests were run" / exit code 5 in this environment, for the pre-existing `wpf/` track too. (Spec §2.4.) The executable is `security/tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe`; filter with `-filter "…"`.
- **Namespaces are per block, not per folder:** `…Exercises.WebAspNet`, `…Exercises.WebBlazor`, `…Exercises.DesktopCore`, `…Exercises.DesktopWpf`. Tests mirror as `FeWoLearning.Security.Tests.<Block>`.
- **Every test file that has `using Bunit;` and touches `TestContext` must add `using TestContext = Xunit.TestContext;`** or it fails `CS0104`. (Spec §3.3.)
- **`ImplicitUsings` here is the minimal set and does NOT include `System.IO`, `System.Net` or `System.Net.Http`.** Measured in Task 2 from the generated `GlobalUsings.g.cs`: the whole set is `System`, `System.Collections.Generic`, `System.Linq`, `System.Threading`, `System.Threading.Tasks`, `Xunit`. So every file touching `HttpClient`/`HttpResponseMessage`/`HttpStatusCode` (all of block 01) needs `using System.Net.Http;` and usually `using System.Net;`, and every file doing file or stream work (most of block 03) needs `using System.IO;`. Expect `CS0246` otherwise — it is a missing using, not a missing package.
- **Every attack fact must be paired with a use fact.** A test that only proves "the attack was rejected" is satisfied by an implementation that rejects everything. This is the rule the track lives or dies by. (Spec §4.2.)
- **Never assert wall-clock timing.** Assert the mechanism and its outcome.
- **Never assert a hard-coded crypto digest.** Assert properties: different salt ⇒ different hash, `Verify` round-trips its own `Hash`, one flipped ciphertext byte is detected.
- **Never use `Assert.Throws<NotImplementedException>`-satisfiable assertions.** Assert rejection *outcomes* (status code, `false`, unchanged state). Where an exception is genuinely the contract, define a local exception type the stub cannot accidentally satisfy.
- **All artifact prose is English** — catalog, README, stub header comments, test names.
- **Stub header comment format** (repo-wide): `Goal:` / `Drills:` / `Passes:`. The `Drills:` line populates the catalog's Concepts column.
- **Commit message format:** `security: exNNN–exNNN` for exercise batches, staging explicit paths. Never `git add -A`.

---

## File Structure

| Path | Responsibility |
|---|---|
| `security/FeWoLearning.Security.slnx` | Solution, three projects. |
| `security/Directory.Build.props` | Redirects the solutions build's output via `UseArtifactsOutput`/`ArtifactsPath`. Required — without it the two content projects share `obj/` and the build fails `CS0579`. |
| `security/global.json` | `{"test":{"runner":"Microsoft.Testing.Platform"}}` — mandatory for xunit.v3 4.0.0 on the .NET 10 SDK. |
| `security/exercises/FeWoLearning.Security.Exercises.csproj` | Stub content library. |
| `security/solutions/FeWoLearning.Security.Solutions.csproj` | Reference content library, same namespaces. |
| `security/tests/FeWoLearning.Security.Tests.csproj` | The single test project; references exactly one content library. |
| `security/{exercises,solutions}/02-web-blazor/_Imports.razor` | `@namespace FeWoLearning.Security.Exercises.WebBlazor` plus shared `@using`s. Folder-level, never project-root. Identical in both libraries. |
| `security/{exercises,solutions}/_support/_Imports.razor` | `@namespace FeWoLearning.Security.Exercises.Support`, so `SmokeGreeter.razor` lands in the namespace the harness smoke test imports. |
| `security/exercises/_support/`, `security/solutions/_support/` | Identical shared fixtures: SQLite seed, key generation, recording logger, attack-payload corpus. Never a catalog row. |
| `security/tests/_harness/WebHarness.cs` | Hosts an exercise's pipeline in `TestServer`, hands back an `HttpClient`. Sole owner of `UseTestServer`. |
| `security/tests/_harness/BlazorHarness.cs` | `BunitContext` plus the services block 02 needs. |
| `security/tests/_harness/WpfPump.cs` | Dispatcher-draining helper for block 04. |
| `security/tests/_harness/HarnessSmokeTests.cs` | Three facts, one per harness. The only tests green on a red run. |
| `security/tests/AssemblyInfo.cs` | `[assembly: Parallelization(Mode = ParallelMode.None)]`. |
| `security/catalog.md` | 60-row ledger. Source of truth for what is done and next. |
| `security/README.md` | Setup, commands, and the accumulated findings. |

---

## Task 1: Scaffolding — three projects that build empty

**Files:**
- Create: `security/FeWoLearning.Security.slnx`
- Create: `security/Directory.Build.props`
- Create: `security/global.json`
- Create: `security/exercises/FeWoLearning.Security.Exercises.csproj`
- Create: `security/exercises/02-web-blazor/_Imports.razor`, `security/exercises/_support/_Imports.razor`
- Create: `security/solutions/FeWoLearning.Security.Solutions.csproj`
- Create: `security/solutions/02-web-blazor/_Imports.razor`, `security/solutions/_support/_Imports.razor`
- Create: `security/tests/FeWoLearning.Security.Tests.csproj`
- Create: `security/tests/AssemblyInfo.cs`
- Create: `security/.gitignore`

**Interfaces:**
- Consumes: nothing.
- Produces: three buildable projects; the `UseSolutions` property switch; assembly names `FeWoLearning.Security.Exercises` (both content libraries share `RootNamespace`, differ only in `AssemblyName`).

- [ ] **Step 1: Create `security/Directory.Build.props`**

```xml
<Project>

  <!-- Redirect the solutions build to its own output tree. Required, not cosmetic:
       exercises/ and solutions/ compile the same type names into the same namespaces,
       so sharing an obj/ tree makes the build fail with CS0579 duplicate-attribute
       errors on the generated assembly info. It has to live here and not in the
       .csproj body - BaseOutputPath/BaseIntermediateOutputPath set inside a project
       are read after the SDK props import, too late to redirect. -->
  <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
    <UseArtifactsOutput>true</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
  </PropertyGroup>

</Project>
```

- [ ] **Step 2: Create `security/global.json`**

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

- [ ] **Step 3: Create `security/.gitignore`**

```gitignore
bin/
obj/
artifacts-solutions/
```

- [ ] **Step 4: Create `security/exercises/FeWoLearning.Security.Exercises.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <OutputType>Library</OutputType>
    <RootNamespace>FeWoLearning.Security.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Security.Exercises</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- Required, or the Razor source generator cannot resolve
       Microsoft.AspNetCore.Components and every .razor file fails CS0234. -->
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <ItemGroup>
    <!-- Row 006 only: SQL injection cannot be honestly proven against a fake.
         Microsoft.Extensions.Hosting and System.Security.Cryptography.ProtectedData
         are deliberately absent - both ship in the shared framework for this TFM and
         referencing them emits NU1510. -->
    <PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.0" />
    <!-- Pinned above Microsoft.Data.Sqlite 10.0.0's own transitive 2.1.11, which
         carries GHSA-2m69-gcr7-jv3q (high severity) and emits NU1903 on every build.
         The bundle package cannot fix this: bundle and lib versions are decoupled. -->
    <PackageReference Include="SQLitePCLRaw.lib.e_sqlite3" Version="2.1.13" />
    <!-- Rows 017, 018. -->
    <PackageReference Include="Microsoft.IdentityModel.JsonWebTokens" Version="8.16.0" />
  </ItemGroup>

</Project>
```

- [ ] **Step 5: Create `security/solutions/FeWoLearning.Security.Solutions.csproj`**

Identical to Step 4 except the one line below. Copy the file and change only `AssemblyName`; `RootNamespace` stays `FeWoLearning.Security.Exercises` so both libraries produce the same namespaces.

```xml
    <AssemblyName>FeWoLearning.Security.Solutions</AssemblyName>
```

- [ ] **Step 6: Create the folder-level `_Imports.razor` files**

`_Imports.razor` must be **per folder**, not at the project root: a root-level
`@namespace` would also capture `_support/SmokeGreeter.razor` (Task 2) and put it
in `…WebBlazor` instead of `…Support`, contradicting the `using` in Task 2 Step 5.

Write these same bytes to **`security/exercises/02-web-blazor/_Imports.razor`**
and **`security/solutions/02-web-blazor/_Imports.razor`**:

```razor
@namespace FeWoLearning.Security.Exercises.WebBlazor
@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
```

And these to **`security/exercises/_support/_Imports.razor`** and
**`security/solutions/_support/_Imports.razor`**:

```razor
@namespace FeWoLearning.Security.Exercises.Support
@using Microsoft.AspNetCore.Components
```

Create the two `02-web-blazor/` directories now even though block 02 has no
exercises until Task 9 — an `_Imports.razor` in a directory that does not exist
cannot be written.

- [ ] **Step 7: Create `security/tests/FeWoLearning.Security.Tests.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Security.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <!-- No OutputType here: xunit.v3 sets it to Exe through its own build props. -->
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="4.0.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="4.0.0" />
    <PackageReference Include="Xunit.StaFact" Version="4.0.23" />
    <PackageReference Include="bunit" Version="2.9.0" />
    <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="10.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one of the two content libraries, never both: that is what keeps the
       identical namespaces and type names from colliding. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Security.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Security.Solutions.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 8: Create `security/tests/AssemblyInfo.cs`**

```csharp
using Xunit.Sdk;
using Xunit.v3;

// Rows 050 (named pipes) and 055 (the system clipboard) touch machine-global and
// process-global state, so the suite must not run in parallel.
// CollectionBehavior(DisableTestParallelization = true) is Obsolete(error: true) in
// xunit.v3 4.0.0 and does not compile - this is the replacement.
[assembly: Parallelization(Mode = ParallelMode.None)]
```

`ParallelizationAttribute` and `ParallelMode` live in `Xunit.Sdk` / `Xunit.v3`,
**not** in the bare `Xunit` namespace — writing `[assembly: Xunit.Parallelization(...)]`
fails `CS0234`. `wpf/tests/_harness/AssemblyInfo.cs` carries the identical shape.

- [ ] **Step 9: Create `security/FeWoLearning.Security.slnx`**

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Security.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Security.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Security.Tests.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 10: Verify the stub build is clean**

Run from `security/`:

```bash
dotnet build 2>&1 | tail -20
```

Expected: `Build succeeded`, **0 errors and 0 warnings**. If `NU1903` appears, the `SQLitePCLRaw.lib.e_sqlite3` pin in Step 4 is missing or was not copied into `solutions/`. If `NU1510` appears, a forbidden `PackageReference` was added.

- [ ] **Step 11: Verify the solutions build is clean and lands in its own tree**

```bash
dotnet build -p:UseSolutions=true 2>&1 | tail -20
ls security/artifacts-solutions 2>/dev/null || ls artifacts-solutions
```

Expected: `Build succeeded`, 0 errors, 0 warnings, and an `artifacts-solutions/` directory exists. If the build fails `CS0579`, `Directory.Build.props` is missing or misplaced.

- [ ] **Step 12: Commit**

```bash
git add security/FeWoLearning.Security.slnx security/Directory.Build.props security/global.json security/.gitignore security/exercises security/solutions security/tests
git commit -m "security: three-project scaffolding with the UseSolutions switch"
```

---

## Task 2: The three harnesses and their smoke tests

This task is the gate for everything after it. Nothing else starts until its three facts are green.

**Files:**
- Create: `security/tests/_harness/WebHarness.cs`
- Create: `security/tests/_harness/BlazorHarness.cs`
- Create: `security/tests/_harness/WpfPump.cs`
- Create: `security/tests/_harness/HarnessSmokeTests.cs`
- Create: `security/exercises/_support/SmokeProbe.cs` and `security/solutions/_support/SmokeProbe.cs` (identical)
- Create: `security/exercises/_support/SmokeGreeter.razor` and `security/solutions/_support/SmokeGreeter.razor` (identical)

**Interfaces:**
- Consumes: Task 1's projects.
- Produces:
  - `WebHarness.StartAsync(Action<IServiceCollection>? services, Action<IApplicationBuilder> configure, CancellationToken ct) → Task<WebHarness>`; instance members `HttpClient Client { get; }`, `IServiceProvider Services { get; }`, `ValueTask DisposeAsync()`.
  - `BlazorHarness : BunitContext` with `BlazorHarness()` constructor.
  - `WpfPump.Pump(DispatcherPriority priority = DispatcherPriority.Loaded) → void`.
  - `_support.SmokeProbe.Configure(IApplicationBuilder app) → void` and `_support.SmokeProbe.MakeTextBox() → System.Windows.Controls.TextBox`.
  - `_support.SmokeGreeter` Razor component with parameter `string Name`.

- [ ] **Step 1: Write the shared smoke fixtures in both content libraries**

`security/exercises/_support/SmokeProbe.cs` — write the identical file to `security/solutions/_support/SmokeProbe.cs`:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.Support;

// Harness canary only. Never a catalog row, never a TODO: if these stop working,
// a package bump broke a harness, not an exercise.
public static class SmokeProbe
{
    public static void Configure(IApplicationBuilder app) =>
        app.Run(async ctx =>
        {
            ctx.Response.Headers["X-Smoke"] = "ok";
            await ctx.Response.WriteAsync("pong");
        });

    // A Button, not a TextBox: Button's default template resolves through
    // SystemResources without an Application, and its DesiredSize is 0x0 when that
    // resolution fails - which is what makes the smoke fact able to fail at all.
    public static System.Windows.Controls.Button MakeButton() => new() { Content = "smoke" };
}
```

`security/exercises/_support/SmokeGreeter.razor` — write the identical file to `security/solutions/_support/SmokeGreeter.razor`:

```razor
<p id="smoke">Hello, @Name</p>

@code {
    [Parameter] public string Name { get; set; } = "";
}
```

- [ ] **Step 2: Write `security/tests/_harness/WebHarness.cs`**

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeWoLearning.Security.Tests.Harness;

// Hosts an exercise's pipeline in an in-memory TestServer and hands back a client.
//
// UseTestServer lives here and ONLY here. The content libraries must never
// reference Microsoft.AspNetCore.TestHost: the whole point of the block-01 shape
// is that the learner configures a pipeline and the harness drives it. An
// exercise that could host itself would let a solution pass by bypassing the
// pipeline entirely.
public sealed class WebHarness : IAsyncDisposable
{
    private readonly IHost _host;

    private WebHarness(IHost host)
    {
        _host = host;
        Client = host.GetTestClient();
    }

    public HttpClient Client { get; }

    public IServiceProvider Services => _host.Services;

    public static async Task<WebHarness> StartAsync(
        Action<IServiceCollection>? services,
        Action<IApplicationBuilder> configure,
        CancellationToken ct = default)
    {
        var host = await new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(s => services?.Invoke(s));
                web.Configure(configure);
            })
            .StartAsync(ct);

        return new WebHarness(host);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
```

- [ ] **Step 3: Write `security/tests/_harness/BlazorHarness.cs`**

```csharp
using Bunit;

namespace FeWoLearning.Security.Tests.Harness;

// Thin wrapper over BunitContext so block-02 tests have one place to add the
// services the block needs (auth state, navigation, persistent component state).
//
// Note for every test file in this project: bUnit 2.9 still ships an obsolete
// Bunit.TestContext, which collides with xunit.v3's Xunit.TestContext. Any file
// that has `using Bunit;` and also touches TestContext fails CS0104. Add
// `using TestContext = Xunit.TestContext;` to those files.
public sealed class BlazorHarness : BunitContext
{
}
```

- [ ] **Step 4: Write `security/tests/_harness/WpfPump.cs`**

```csharp
using System.Windows.Threading;

namespace FeWoLearning.Security.Tests.Harness;

public static class WpfPump
{
    // Drains the dispatcher queue down to `priority`. Bindings update at
    // DispatcherPriority.DataBind, so a test that mutates a source and asserts
    // immediately reads the stale value - call this in between.
    public static void Pump(DispatcherPriority priority = DispatcherPriority.Loaded)
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(
            priority,
            new DispatcherOperationCallback(f => { ((DispatcherFrame)f).Continue = false; return null; }),
            frame);
        Dispatcher.PushFrame(frame);
    }
}
```

- [ ] **Step 5: Write `security/tests/_harness/HarnessSmokeTests.cs`**

```csharp
using System.Net;
using Bunit;
using FeWoLearning.Security.Exercises.Support;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests;

// The canary for a package bump breaking a harness, the same role uno/'s
// HarnessSmokeTests plays. These three are the ONLY tests green on a red run.
public class HarnessSmokeTests
{
    [Fact]
    public async Task Web_Harness_Serves_A_Request()
    {
        await using var harness = await WebHarness.StartAsync(
            services: null,
            configure: SmokeProbe.Configure,
            ct: TestContext.Current.CancellationToken);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ok", Assert.Single(response.Headers.GetValues("X-Smoke")));
        Assert.Equal("pong", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Blazor_Harness_Renders_A_Component()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<SmokeGreeter>(p => p.Add(c => c.Name, "world"));

        Assert.Equal("Hello, world", cut.Find("#smoke").TextContent);
    }

    [WpfFact]
    public void Wpf_Harness_Runs_Sta_And_Resolves_A_Default_Control_Template()
    {
        // The apartment state is what [WpfFact] itself buys; assert it, so a
        // StaFact regression is named rather than showing up as a cast exception
        // somewhere in block 04.
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());

        var button = SmokeProbe.MakeButton();

        button.Measure(new System.Windows.Size(200, 50));
        WpfPump.Pump();

        // Template plus DesiredSize, measured BEFORE any Arrange. Do not assert
        // ActualWidth after Arrange: a FrameworkElement defaults to
        // HorizontalAlignment.Stretch and fills whatever rect it is given, so
        // ActualWidth > 0 holds even with an empty template - the assertion cannot
        // fail, which makes it worthless as a canary. DesiredSize comes from Measure
        // and is 0x0 when template resolution breaks. This is the idiom
        // wpf/tests/_harness/HarnessSmokeTests.cs already uses and has verified.
        Assert.NotNull(button.Template);
        Assert.True(button.DesiredSize.Width > 0, "A templated Button must measure wider than 0.");
        Assert.True(button.DesiredSize.Height > 0, "A templated Button must measure taller than 0.");
    }
}
```

- [ ] **Step 6: Build and run the three smoke facts**

```bash
cd security
dotnet build 2>&1 | tail -10
./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe 2>&1 | tail -20
```

Expected: `Build succeeded`, 0 warnings, and the run reports **Total: 3, Failed: 0**.

If `CS0104` on `TestContext` appears, the `using TestContext = Xunit.TestContext;` alias is missing from Step 5. If the run reports 0 tests, do not switch to `dotnet test` — re-check that `tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe` exists and was rebuilt.

- [ ] **Step 7: Verify the same three facts pass against `solutions/`**

```bash
dotnet build -p:UseSolutions=true 2>&1 | tail -10
./artifacts-solutions/bin/FeWoLearning.Security.Tests/debug/FeWoLearning.Security.Tests.exe 2>&1 | tail -20
```

Expected: **Total: 3, Failed: 0**. If the executable is not at that path, locate it with `find artifacts-solutions -name 'FeWoLearning.Security.Tests.exe'` and **record the real path in `security/README.md`** — later tasks depend on it.

- [ ] **Step 8: Commit**

```bash
git add security/tests/_harness security/tests/AssemblyInfo.cs security/exercises/_support security/solutions/_support
git commit -m "security: web, blazor and wpf harnesses with smoke tests"
```

---

## Task 3: Seed `catalog.md` with all 60 rows

**Files:**
- Create: `security/catalog.md`

**Interfaces:**
- Consumes: nothing.
- Produces: the work queue every later task reads. Column shape `# | Slug | Concepts | Status`, matching every other track's catalog so the CLAUDE.md workflow applies unchanged.

- [ ] **Step 1: Write the header**

```markdown
# Security (C#) — Exercise Catalog (60)

Attack-surface blocks: **web-aspnet** 001–024 · **web-blazor** 025–036 ·
**desktop-core** 037–052 · **desktop-wpf** 053–060.

Legend: ✅ seeded (stub + test + solution present, red and green both verified) ·
⬜ planned.

This track deliberately departs from the repo's 100-row / four-difficulty-tier
scheme. "Beginner" is not a meaningful axis for security: a path-traversal guard
is not conceptually harder than a CSP header, they are different attack surfaces.
Difficulty rises *within* each block. See
`docs/superpowers/specs/2026-09-05-security-track-design.md` §5.

Stubs live in `exercises/<block>/ExNNN_<Slug>.cs` (or `.razor` for block 02),
their xUnit tests in `tests/<block>/ExNNN_<Slug>Tests.cs`, and reference
implementations in `solutions/<block>/` at the same relative path.

**Status: 0 ✅ / 60 ⬜**
```

- [ ] **Step 2: Write the four block tables**

Every row starts ⬜. Use exactly these numbers and slugs; the Concepts column must match each stub's eventual `Drills:` line.

```markdown
## web-aspnet (001–024) — the server-side attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | SecurityHeaders | middleware pipeline, Response.OnStarting, header lifetime | ⬜ |
| 002 | HttpsRedirectAndHsts | HSTS, transport downgrade, redirect status codes | ⬜ |
| 003 | ContentSecurityPolicy | CSP directives, per-request nonce, inline-script blocking | ⬜ |
| 004 | PathTraversalGuard | canonicalisation, root containment, safe static file serving | ⬜ |
| 005 | ModelBindingOverposting | mass assignment, BindNever, explicit DTO projection | ⬜ |
| 006 | SqlInjectionParameterization | parameterised commands, real SQLite, tautology payloads | ⬜ |
| 007 | ContextualOutputEncoding | HtmlEncoder vs JavaScriptEncoder vs UrlEncoder, sink context | ⬜ |
| 008 | AntiforgeryCsrf | antiforgery tokens, cross-origin POST, safe vs unsafe methods | ⬜ |
| 009 | CorsPolicy | origin allowlists, credentials, why wildcard plus credentials fails | ⬜ |
| 010 | CookieSecurityFlags | HttpOnly, Secure, SameSite, cookie scope | ⬜ |
| 011 | SessionFixation | identifier regeneration on privilege change | ⬜ |
| 012 | PasswordHashingPbkdf2 | Rfc2898DeriveBytes, per-user salt, iteration count, fixed-time verify | ⬜ |
| 013 | AuthenticationHandler | AuthenticationHandler, ClaimsPrincipal construction, scheme selection | ⬜ |
| 014 | AuthorizationPolicies | policy-based authorization, requirements, handler registration | ⬜ |
| 015 | ResourceBasedAuthorization | IAuthorizationService on a resource instance, ownership checks | ⬜ |
| 016 | InsecureDirectObjectReference | ownership enforcement, opaque identifiers, enumeration | ⬜ |
| 017 | JwtValidation | issuer, audience, lifetime and signature validation, alg confusion | ⬜ |
| 018 | RefreshTokenRotation | single-use refresh tokens, reuse detection, family revocation | ⬜ |
| 019 | RateLimiting | rate limiter partitions, 429 responses, per-principal keys | ⬜ |
| 020 | JsonDepthAndUnknownMembers | MaxDepth, unmapped member handling, deserialisation resource limits | ⬜ |
| 021 | SsrfOutboundGuard | outbound URL validation, scheme allowlists, private address ranges | ⬜ |
| 022 | OpenRedirectGuard | local-redirect checks, absolute URL rejection, return-URL allowlists | ⬜ |
| 023 | FileUploadValidation | content sniffing, extension allowlists, size limits, safe storage names | ⬜ |
| 024 | ErrorHandlingWithoutLeakage | ProblemDetails, exception middleware, suppressing internals | ⬜ |

## web-blazor (025–036) — the component attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 025 | MarkupStringXss | MarkupString as a sink, sanitisation, when raw HTML is never safe | ⬜ |
| 026 | RenderTreeEncodingDefaults | automatic encoding of text and attributes, attribute injection | ⬜ |
| 027 | CspNonceFlow | nonce propagation to components, eliminating inline handlers | ⬜ |
| 028 | AuthorizeViewAndAuthState | AuthenticationStateProvider, AuthorizeView, cascading auth state | ⬜ |
| 029 | ClientAuthIsNotEnforcement | UI trimming is not authorization, server-side enforcement | ⬜ |
| 030 | AntiforgeryInEditForm | EditForm, antiforgery in interactive and static rendering | ⬜ |
| 031 | SecretsNeverReachClient | configuration surface, what a component may receive | ⬜ |
| 032 | JsInteropInjection | passing untrusted data across JS interop, avoiding eval-shaped calls | ⬜ |
| 033 | NavigationManagerOpenRedirect | client-side redirect validation, external URI rejection | ⬜ |
| 034 | PersistentStateLeak | PersistentComponentState, what must never survive prerendering | ⬜ |
| 035 | ErrorBoundaryLeakage | ErrorBoundary, suppressing exception detail in the render tree | ⬜ |
| 036 | SanitizingComponent | reusable sanitising component, allowlist over denylist | ⬜ |

## desktop-core (037–052) — the local attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 037 | DpapiProtectedData | ProtectedData, DataProtectionScope, optional entropy | ⬜ |
| 038 | CredentialStorage | never plaintext at rest, round-tripping, scope of protection | ⬜ |
| 039 | AesGcmAuthenticatedEncryption | AES-GCM, nonce uniqueness, tag verification, tamper detection | ⬜ |
| 040 | KeyDerivationAndRotation | key derivation, versioned key material, decrypting older versions | ⬜ |
| 041 | FixedTimeComparison | CryptographicOperations.FixedTimeEquals, why length-first exits leak | ⬜ |
| 042 | CryptographicRandomness | RandomNumberGenerator over System.Random, token generation | ⬜ |
| 043 | SignatureVerification | detached signatures, public-key verification, rejecting tampered data | ⬜ |
| 044 | UpdateIntegrityAndRollback | hash manifests, signed manifests, monotonic version enforcement | ⬜ |
| 045 | UnsafeDeserialization | polymorphic type handling, type allowlists, rejecting arbitrary types | ⬜ |
| 046 | XmlExternalEntity | XmlReaderSettings, DtdProcessing, XmlResolver, entity expansion | ⬜ |
| 047 | ZipSlipExtraction | archive entry path containment, absolute and relative escapes | ⬜ |
| 048 | PathCanonicalization | full-path containment, UNC and device-name traps, alternate streams | ⬜ |
| 049 | ProcessArgumentInjection | ProcessStartInfo.ArgumentList over a joined Arguments string | ⬜ |
| 050 | NamedPipeAccessControl | PipeSecurity, ACLs, rejecting unauthorised peers | ⬜ |
| 051 | SecretRedactionInLogs | structured logging, redaction of sensitive values, log injection | ⬜ |
| 052 | RestrictiveFileAcl | file ACLs at creation, inherited permissions, least privilege | ⬜ |

## desktop-wpf (053–060) — the WPF attack surface

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 053 | PasswordBoxNoPlaintextBinding | PasswordBox, why Password is not a DependencyProperty | ⬜ |
| 054 | SensitiveBufferLifetime | clearing sensitive buffers, bounded lifetime of plaintext | ⬜ |
| 055 | ClipboardHygiene | clipboard as shared state, excluding data from history | ⬜ |
| 056 | DragDropUntrustedPayload | validating dropped formats and paths before acting | ⬜ |
| 057 | EmbeddedBrowserNavigationPolicy | navigation allowlists, scheme restrictions, host object exposure | ⬜ |
| 058 | XamlReaderUntrustedMarkup | XamlReader.Parse as code execution, restricting parsed markup | ⬜ |
| 059 | BindingErrorLeakage | binding failure surfaces, tooltips and traces as leak channels | ⬜ |
| 060 | FilePickerResultStillUntrusted | dialog results are user input, post-dialog validation | ⬜ |
```

- [ ] **Step 2a: Verify the row count**

```bash
grep -c '^| 0\|^| [0-9]' security/catalog.md
```

Expected: `60`. If not, a row was dropped or duplicated — fix before committing, because every later task reads this file as the work queue.

- [ ] **Step 3: Commit**

```bash
git add security/catalog.md
git commit -m "security: seed the 60-row catalog"
```

---

## How to execute Tasks 4–17 (the exercise batches)

Tasks 4 through 17 each deliver a batch of exercises. They share one procedure; each task below states only what is specific to its batch.

**For every exercise in a batch, produce three files:**

1. `security/exercises/<block>/ExNNN_<Slug>.cs` — the stub. Header comment with `Goal:` / `Drills:` / `Passes:`. Every member throws `new NotImplementedException("TODO: ExNNN - <what to do>")`.
2. `security/tests/<block>/ExNNN_<Slug>Tests.cs` — the test class, in namespace `FeWoLearning.Security.Tests.<Block>`.
3. `security/solutions/<block>/ExNNN_<Slug>.cs` — the reference implementation, same namespace and type name as the stub.

**The per-batch step sequence:**

- [ ] **Step A: Read the style template.** Read one already-finished exercise from the *same block* — once per block, not once per batch. For the first batch of a block, the worked example is written out in that task.
- [ ] **Step B: Write all three files for each exercise in the batch.**
- [ ] **Step C: Red check.**
  ```bash
  cd security && dotnet build 2>&1 | tail -10
  ./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe -filter "<batch filter>" 2>&1 | tail -30
  ```
  Every fact must **fail**, and each failure's stack must name that exercise's `NotImplementedException`. A failure from a compile error, a missing using, or a fixture is a bug — fix it before continuing. A stub that fails to build is a bug.
- [ ] **Step D: Green check.**
  ```bash
  dotnet build -p:UseSolutions=true 2>&1 | tail -10
  <solutions test exe path from Task 2 Step 7> -filter "<batch filter>" 2>&1 | tail -30
  ```
  Every fact must **pass**, and the solutions build must emit **0 warnings**.
- [ ] **Step E: Pairing audit.** For each exercise in the batch, answer in writing: *would an implementation that rejects everything pass this test?* If yes, the use fact is missing or too weak — add it and redo Steps C and D. This is not optional; it is the rule the track lives or dies by.
- [ ] **Step F: Flip exactly those rows in `catalog.md`** ⬜ → ✅ and update the `**Status: N ✅ / M ⬜**` line.
- [ ] **Step G: Commit** with explicit paths:
  ```bash
  git add security/exercises/<block> security/solutions/<block> security/tests/<block> security/catalog.md
  git commit -m "security: exNNN–exNNN"
  ```

---

## Task 4: Exercises 001–005 (web-aspnet)

**Files:**
- Create: `security/exercises/01-web-aspnet/Ex00{1..5}_*.cs`, matching `security/solutions/01-web-aspnet/`, tests in `security/tests/01-web-aspnet/`.

**Interfaces:**
- Consumes: `WebHarness.StartAsync` from Task 2.
- Produces: the style template for block 01 — every later block-01 batch reads `Ex001_SecurityHeaders.cs` and its test.

**Batch filter:** `-filter "/*/*/Ex001*" -filter "/*/*/Ex002*" -filter "/*/*/Ex003*" -filter "/*/*/Ex004*" -filter "/*/*/Ex005*"`

- [ ] **Step 1: Write `Ex001_SecurityHeaders` as the block's worked example**

Stub — `security/exercises/01-web-aspnet/Ex001_SecurityHeaders.cs`:

```csharp
using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 001 — SecurityHeaders (web-aspnet).
// Goal:   Register middleware that stamps three response headers on every response
//         - X-Content-Type-Options: nosniff, X-Frame-Options: DENY and
//         Referrer-Policy: no-referrer - without overwriting a value a downstream
//         component deliberately set for itself.
// Drills: middleware pipeline, Response.OnStarting, header lifetime.
// Passes: attack facts   - all three headers are present on a plain response, so a
//                          content-sniffing or clickjacking attack has nothing to
//                          work with;
//         use facts      - a handler that set its own Referrer-Policy keeps it, and
//                          the response body is delivered unchanged.
public static class Ex001_SecurityHeaders
{
    public static void Use(IApplicationBuilder app) =>
        throw new NotImplementedException(
            "TODO: Ex001 - add middleware that sets the three security headers without clobbering existing values");
}
```

Test — `security/tests/01-web-aspnet/Ex001_SecurityHeadersTests.cs`:

```csharp
using System.Net;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex001_SecurityHeadersTests
{
    private static Task<WebHarness> StartAsync(RequestDelegate terminal) =>
        WebHarness.StartAsync(
            services: null,
            configure: app =>
            {
                Ex001_SecurityHeaders.Use(app);
                app.Run(terminal);
            },
            ct: TestContext.Current.CancellationToken);

    private static Task PlainBody(HttpContext ctx) => ctx.Response.WriteAsync("body");

    [Theory]
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("X-Frame-Options", "DENY")]
    [InlineData("Referrer-Policy", "no-referrer")]
    public async Task Attack_Response_Always_Carries_The_Hardening_Header(string name, string expected)
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(expected, Assert.Single(response.Headers.GetValues(name)));
    }

    [Fact]
    public async Task Use_A_Deliberate_Downstream_Value_Is_Not_Clobbered()
    {
        await using var harness = await StartAsync(ctx =>
        {
            ctx.Response.Headers["Referrer-Policy"] = "same-origin";
            return ctx.Response.WriteAsync("body");
        });

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal("same-origin", Assert.Single(response.Headers.GetValues("Referrer-Policy")));
    }

    [Fact]
    public async Task Use_The_Response_Body_And_Status_Are_Untouched()
    {
        await using var harness = await StartAsync(PlainBody);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("body", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
```

Solution — `security/solutions/01-web-aspnet/Ex001_SecurityHeaders.cs`:

```csharp
using Microsoft.AspNetCore.Builder;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 001 — SecurityHeaders (reference solution).
public static class Ex001_SecurityHeaders
{
    public static void Use(IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            // OnStarting, not a plain assignment before next(): the downstream handler
            // runs after this line, so anything set here can still be overwritten - and
            // anything set after next() is too late once the body has begun.
            ctx.Response.OnStarting(() =>
            {
                var headers = ctx.Response.Headers;
                if (!headers.ContainsKey("X-Content-Type-Options")) headers["X-Content-Type-Options"] = "nosniff";
                if (!headers.ContainsKey("X-Frame-Options")) headers["X-Frame-Options"] = "DENY";
                if (!headers.ContainsKey("Referrer-Policy")) headers["Referrer-Policy"] = "no-referrer";
                return Task.CompletedTask;
            });

            await next();
        });
}
```

Note the pairing this establishes: the three attack facts alone would be satisfied by middleware that hard-sets the headers and destroys the downstream value; the second use fact is what forces `OnStarting` plus the presence check.

- [ ] **Step 2: Write `Ex002_HttpsRedirectAndHsts`**

Contract — `public static class Ex002_HttpsRedirectAndHsts` with:
```csharp
public static void Use(IApplicationBuilder app, int httpsPort)
```
Attack facts: a plain-HTTP GET is answered with **308** and a `Location` whose scheme is `https` and whose path and query survive verbatim; an HTTPS request carries `Strict-Transport-Security` with `max-age` of at least 31536000 and `includeSubDomains`. Use facts: an HTTPS request is **not** redirected (200, body delivered); a plain-HTTP request does **not** carry HSTS, because a header on a downgradeable channel is worthless.

Trap: assert the `Location` value, not merely that a redirect happened — an implementation redirecting to a fixed `/` would otherwise pass.

- [ ] **Step 3: Write `Ex003_ContentSecurityPolicy`**

Contract — `public static class Ex003_ContentSecurityPolicy` with:
```csharp
public static void Use(IApplicationBuilder app)
public static string GetNonce(HttpContext context)
```
Attack facts: the `Content-Security-Policy` header contains `default-src 'self'` and `object-src 'none'`; it does **not** contain `unsafe-inline` or `unsafe-eval`; two separate requests receive **different** nonces (a fixed nonce is no nonce). Use facts: `GetNonce` returns the exact value that appears in the header's `script-src` directive for that same request, and it is at least 16 bytes of base64.

Trap: the "different nonces" fact is an attack fact and needs its partner — without the "GetNonce matches the header" use fact, returning a fresh random string and never putting it in the header would pass.

- [ ] **Step 4: Write `Ex004_PathTraversalGuard`**

Contract — `public static class Ex004_PathTraversalGuard` with:
```csharp
public static bool TryResolve(string rootDirectory, string requestedPath, out string fullPath)
```
Attack facts, all returning `false` with `fullPath` set to `""`: `../secrets.txt`, `..\\secrets.txt`, a rooted path such as `C:\\Windows\\win.ini`, `subdir/../../outside.txt`, and a path whose canonical form escapes the root only after normalisation. Use facts: `report.txt` and `subdir/report.txt` both return `true` with a `fullPath` under the root that `Path.GetFullPath` agrees with.

Trap: this exercise is the archetype of the reject-everything failure. The two use facts are mandatory. Build the root under a per-test temp directory and delete it in `Dispose`.

- [ ] **Step 5: Write `Ex005_ModelBindingOverposting`**

Contract — in namespace `FeWoLearning.Security.Exercises.WebAspNet`:
```csharp
public sealed class Ex005_UserProfile   // Id, DisplayName, Email, IsAdministrator
public static class Ex005_ModelBindingOverposting
{
    public static Ex005_UserProfile Apply(Ex005_UserProfile existing, string requestJson);
}
```
Attack facts: a request body containing `"isAdministrator": true` leaves `IsAdministrator` **false**; a body containing `"id": 999` leaves `Id` unchanged. Use facts: a body containing `displayName` and `email` updates exactly those two, and a body containing only `displayName` leaves `Email` unchanged.

Trap: do not test this by asserting an exception on unknown members — a stub throwing `NotImplementedException` would satisfy a loose `Assert.ThrowsAny`. Assert the resulting object's property values.

- [ ] **Step 6: Run Steps C through G of the shared procedure**

Batch filter as given at the top of this task. Commit message: `security: ex001–ex005`.

---

## Task 5: Exercises 006–010 (web-aspnet)

**Files:** `security/{exercises,solutions}/01-web-aspnet/Ex00{6..9}_*.cs`, `Ex010_*.cs`, tests in `security/tests/01-web-aspnet/`.

**Interfaces:**
- Consumes: `WebHarness` (Task 2); the block-01 style established by `Ex001` (Task 4).
- Produces: `_support/Ex006_UserDatabase` — an in-memory SQLite fixture reused by no other row.

**Batch filter:** `-filter "/*/*/Ex006*" -filter "/*/*/Ex007*" -filter "/*/*/Ex008*" -filter "/*/*/Ex009*" -filter "/*/*/Ex010*"`

- [ ] **Step 1: Write the SQLite fixture into both content libraries**

`security/exercises/_support/Ex006_UserDatabase.cs`, identical in `solutions/_support/`:

```csharp
using Microsoft.Data.Sqlite;

namespace FeWoLearning.Security.Exercises.Support;

// An in-memory SQLite database seeded with two users. Row 006 is the only row that
// needs a real database: SQL injection cannot be honestly proven against a fake,
// because a test that merely inspects command text is satisfied by a solution that
// builds a right-looking string and concatenates somewhere else.
public sealed class Ex006_UserDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public Ex006_UserDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var seed = _connection.CreateCommand();
        seed.CommandText =
            "create table users (id integer primary key, name text not null, email text not null);" +
            "insert into users (id, name, email) values (1, 'ada', 'ada@example.com'), (2, 'bob', 'bob@example.com');";
        seed.ExecuteNonQuery();
    }

    public SqliteConnection Connection => _connection;

    public void Dispose() => _connection.Dispose();
}
```

- [ ] **Step 2: Write `Ex006_SqlInjectionParameterization`**

Contract:
```csharp
public static class Ex006_SqlInjectionParameterization
{
    public static IReadOnlyList<string> FindEmailsByName(SqliteConnection connection, string name);
}
```
Attack facts: the tautology payload (`x' or '1'='1` and the `--` comment variant) returns an **empty** list — verified during design that the concatenating form returns 2 rows here, so this fact genuinely discriminates; a payload attempting `'; drop table users; --` leaves the `users` table intact and queryable afterwards. Use facts: `"ada"` returns exactly `["ada@example.com"]`, and an unknown name returns an empty list.

Trap: the "unknown name returns empty" use fact does **not** discriminate against reject-everything on its own — the `"ada"` fact is the one that does. Keep both.

- [ ] **Step 3: Write `Ex007_ContextualOutputEncoding`**

Contract:
```csharp
public static class Ex007_ContextualOutputEncoding
{
    public static string ForHtmlBody(string untrusted);
    public static string ForHtmlAttribute(string untrusted);
    public static string ForJavaScriptString(string untrusted);
    public static string ForUrlQuery(string untrusted);
}
```
Attack facts: `<script>alert(1)</script>` through `ForHtmlBody` contains no `<`; `" onmouseover="alert(1)` through `ForHtmlAttribute` contains no raw `"`; `</script>` through `ForJavaScriptString` does not contain the literal `</script>`; `a&b=c` through `ForUrlQuery` contains no raw `&`. Use facts: each method leaves a plain alphanumeric string **unchanged**, and `ForHtmlBody("café")` still yields text a browser renders as `café` (assert it round-trips through `System.Net.WebUtility.HtmlDecode`).

Trap: the "plain string unchanged" use facts are what stop a solution that strips or blanks everything.

- [ ] **Step 4: Write `Ex008_AntiforgeryCsrf`**

Contract:
```csharp
public static class Ex008_AntiforgeryCsrf
{
    public static void AddServices(IServiceCollection services);
    public static void Use(IApplicationBuilder app);
}
```
Attack facts: a POST with no antiforgery token is answered **400**; a POST carrying a token but not its matching cookie is answered **400**. Use facts: a GET is **never** challenged (200) — antiforgery on safe methods breaks every link; and a POST carrying both the token and its cookie succeeds (200) and the handler observed the request body.

Trap: without the "GET is not challenged" and "valid POST succeeds" use facts, middleware that returns 400 for everything passes.

- [ ] **Step 5: Write `Ex009_CorsPolicy`**

Contract:
```csharp
public static class Ex009_CorsPolicy
{
    public static void AddServices(IServiceCollection services, string allowedOrigin);
    public static void Use(IApplicationBuilder app);
}
```
Attack facts: a request with `Origin: https://evil.example` receives **no** `Access-Control-Allow-Origin` header; the response never carries `Access-Control-Allow-Origin: *` together with `Access-Control-Allow-Credentials: true` (the combination browsers reject and servers still ship). Use facts: a request from the allowed origin receives `Access-Control-Allow-Origin` echoing exactly that origin, and a preflight `OPTIONS` from the allowed origin returns 204 with the allowed methods.

- [ ] **Step 6: Write `Ex010_CookieSecurityFlags`**

Contract:
```csharp
public static class Ex010_CookieSecurityFlags
{
    public static void AppendSessionCookie(HttpResponse response, string name, string value);
}
```
Attack facts: the emitted `Set-Cookie` contains `httponly`, `secure`, and `samesite=strict` (compare case-insensitively). Use facts: the cookie's name and value round-trip exactly as given, including a value needing URL encoding; and `Path=/` is present so the cookie is actually usable.

Trap: parse the `Set-Cookie` header rather than asserting the whole string equals a literal — a literal assertion breaks on any legitimate attribute ordering.

- [ ] **Step 7: Run Steps C through G.** Commit message: `security: ex006–ex010`.

---

## Task 6: Exercises 011–015 (web-aspnet)

**Batch filter:** `-filter "/*/*/Ex011*" -filter "/*/*/Ex012*" -filter "/*/*/Ex013*" -filter "/*/*/Ex014*" -filter "/*/*/Ex015*"`

**Interfaces:** Consumes `WebHarness`. Produces `Ex012_PasswordHasher.Hash`/`Verify`, reused conceptually by row 018's test but not referenced by it.

- [ ] **Step 1: `Ex011_SessionFixation`** — `public static class Ex011_SessionFixation` with `public static string SignIn(HttpContext context, string userName)` returning the new session identifier. Attack fact: the identifier held before sign-in is **not** the one returned afterwards, and presenting the pre-sign-in identifier afterwards resolves to an anonymous session. Use facts: the returned identifier resolves to a session carrying `userName`, and two consecutive requests presenting the new identifier see the same session.

- [ ] **Step 2: `Ex012_PasswordHashingPbkdf2`** — contract:
  ```csharp
  public static class Ex012_PasswordHashingPbkdf2
  {
      public static string Hash(string password);
      public static bool Verify(string password, string stored);
  }
  ```
  Attack facts: hashing the same password twice yields **different** stored values (per-hash salt); `Verify` returns `false` for a wrong password; `Verify` returns `false` when the stored value's salt is altered by one byte; the stored value never contains the password as a substring. Use facts: `Verify(p, Hash(p))` is `true` for several passwords including Unicode and an empty string; and the stored format declares an iteration count of at least 100000, read back from the stored value itself.

  Trap: no hard-coded digest anywhere. `Verify` round-tripping its own `Hash` is the property under test.

- [ ] **Step 3: `Ex013_AuthenticationHandler`** — contract: `public sealed class Ex013_ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>` plus `public static class Ex013_AuthenticationHandler { public static void AddServices(IServiceCollection services, string validApiKey); }`. Attack facts: a request with no `X-Api-Key` is unauthenticated (401 from a challenged endpoint); a request with a wrong key is 401; a request whose key differs only in case is 401. Use facts: a request with the valid key reaches the endpoint (200) and the endpoint sees a `ClaimsPrincipal` with `Identity.IsAuthenticated` true and a `NameIdentifier` claim.

- [ ] **Step 4: `Ex014_AuthorizationPolicies`** — contract: `public static class Ex014_AuthorizationPolicies { public static void AddServices(IServiceCollection services); public const string PolicyName = "AdultsOnly"; }` with a requirement over a `dateOfBirth` claim. Attack facts: a principal with no `dateOfBirth` claim fails the policy; a principal 17 years old fails; a principal with a malformed claim value fails rather than throwing. Use facts: a principal 18 years old passes, and one 40 years old passes.

- [ ] **Step 5: `Ex015_ResourceBasedAuthorization`** — contract:
  ```csharp
  public sealed record Ex015_Document(int Id, string OwnerId, string Body);
  public static class Ex015_ResourceBasedAuthorization
  {
      public static void AddServices(IServiceCollection services);
      public const string PolicyName = "DocumentOwner";
  }
  ```
  Attack facts: a principal that is not the owner is denied for read **and** for delete; an anonymous principal is denied. Use facts: the owner is allowed to read and to delete; and a principal holding an `admin` role is allowed to read but **still denied** delete — the fact that stops a blanket-allow-admin implementation.

- [ ] **Step 6: Run Steps C through G.** Commit message: `security: ex011–ex015`.

---

## Task 7: Exercises 016–020 (web-aspnet)

**Batch filter:** `-filter "/*/*/Ex016*" -filter "/*/*/Ex017*" -filter "/*/*/Ex018*" -filter "/*/*/Ex019*" -filter "/*/*/Ex020*"`

**Interfaces:** Consumes `WebHarness`, `Microsoft.IdentityModel.JsonWebTokens`. Produces `Ex017_TokenFactory` (test-side helper for minting tokens with chosen issuer, audience, lifetime and key), which Task 7 Step 3 reuses.

- [ ] **Step 1: `Ex016_InsecureDirectObjectReference`** — contract:
  ```csharp
  public static class Ex016_InsecureDirectObjectReference
  {
      public static IResult GetInvoice(string callerId, int invoiceId, IReadOnlyList<Ex016_Invoice> store);
  }
  public sealed record Ex016_Invoice(int Id, string OwnerId, decimal Amount);
  ```
  Attack facts: requesting another user's existing invoice returns **404**, not 403 — a 403 confirms the id exists and hands the attacker an enumeration oracle; requesting a non-existent id also returns 404, and the two responses are byte-identical. Use facts: the owner requesting their own invoice gets 200 with the amount; an owner with two invoices can fetch both.

- [ ] **Step 2: `Ex017_JwtValidation`** — contract:
  ```csharp
  public static class Ex017_JwtValidation
  {
      public static bool TryValidate(string token, byte[] signingKey, string issuer, string audience, out ClaimsPrincipal? principal);
  }
  ```
  Attack facts, all `false` with `principal` null: a token signed with a different key; a token whose `alg` header is `none`; a token from a different issuer; a token for a different audience; an expired token; a token whose payload was edited after signing. Use fact: a correctly signed, in-date token for the right issuer and audience returns `true` and a principal carrying its `sub` claim.

  Trap: mint the attack tokens with a real handler in the test, not by hand-editing strings, so the only difference from the valid token is the one property under test.

- [ ] **Step 3: `Ex018_RefreshTokenRotation`** — contract:
  ```csharp
  public sealed class Ex018_RefreshTokenStore
  {
      public string Issue(string userId);
      public bool TryRedeem(string refreshToken, out string? replacement);
  }
  ```
  Attack facts: redeeming the same token twice fails the second time; after a reuse attempt, the **replacement** token issued from that token is also refused (family revocation); a token never issued is refused. Use facts: a freshly issued token redeems once and yields a different replacement; the replacement itself redeems once; and a second user's tokens are unaffected by the first user's revocation.

- [ ] **Step 4: `Ex019_RateLimiting`** — contract: `public static class Ex019_RateLimiting { public static void AddServices(IServiceCollection services, int permitsPerWindow); public static void Use(IApplicationBuilder app); }`, partitioned by the `X-Api-Key` header. Attack facts: the request after the permit count is exhausted returns **429**. Use facts: every request up to the limit returns 200; and a different `X-Api-Key` still gets its own full allowance while the first is exhausted — the fact that proves partitioning rather than a global counter.

  Trap: assert only that the limiter rejected, never how long anything took.

- [ ] **Step 5: `Ex020_JsonDepthAndUnknownMembers`** — contract:
  ```csharp
  public static class Ex020_JsonDepthAndUnknownMembers
  {
      public static bool TryParse<T>(string json, out T? value, out string? error);
  }
  ```
  Attack facts: a 200-level nested array fails with a non-null `error` and `value` null; a payload with a member the target type does not declare fails; the failure `error` does **not** contain the target type's full name or any stack detail. Use facts: a well-formed payload at nesting depth 5 parses to a correct value; a payload using different casing for known members still parses.

- [ ] **Step 6: Run Steps C through G.** Commit message: `security: ex016–ex020`.

---

## Task 8: Exercises 021–024 (web-aspnet) — block 01 completes

**Batch filter:** `-filter "/*/*/Ex021*" -filter "/*/*/Ex022*" -filter "/*/*/Ex023*" -filter "/*/*/Ex024*"`

- [ ] **Step 1: `Ex021_SsrfOutboundGuard`** — contract: `public static class Ex021_SsrfOutboundGuard { public static bool IsAllowedTarget(string url); }`. Attack facts, all `false`: `http://127.0.0.1/admin`, `http://localhost/`, `http://169.254.169.254/latest/meta-data/`, `http://10.0.0.5/`, `http://192.168.1.1/`, `file:///C:/Windows/win.ini`, `gopher://example.com/`, and a URL whose host is `[::1]`. Use facts: `https://api.example.com/v1/items` is `true`, and `https://example.com:8443/path?q=1` is `true`.

- [ ] **Step 2: `Ex022_OpenRedirectGuard`** — contract: `public static class Ex022_OpenRedirectGuard { public static string SafeReturnUrl(string? candidate, string fallback); }`. Attack facts, all returning `fallback`: `https://evil.example/`, `//evil.example/`, `/\evil.example`, `http:/\/\evil.example`, a `javascript:` URL, and `null`. Use facts: `/dashboard` returns `/dashboard`; `/reports?year=2026` returns unchanged including the query.

- [ ] **Step 3: `Ex023_FileUploadValidation`** — contract:
  ```csharp
  public static class Ex023_FileUploadValidation
  {
      public static bool TryAccept(string clientFileName, byte[] content, long maxBytes, out string storageName, out string? rejection);
  }
  ```
  Attack facts: a `.exe` is rejected; a file named `report.pdf` whose bytes begin with `MZ` is rejected (extension lies, content decides); a file over `maxBytes` is rejected; `../../evil.png` never yields a `storageName` containing `..` or a directory separator. Use facts: a real PNG named `photo.png` is accepted and `storageName` keeps the `.png` extension while being unpredictable; two uploads of the same name produce **different** storage names.

- [ ] **Step 4: `Ex024_ErrorHandlingWithoutLeakage`** — contract: `public static class Ex024_ErrorHandlingWithoutLeakage { public static void Use(IApplicationBuilder app); }`. Attack facts: when the downstream handler throws with a message containing a connection string, the 500 response body contains neither that message, nor the exception type name, nor the word `at ` followed by a namespace (no stack). Use facts: the response is a `application/problem+json` `ProblemDetails` with `status` 500 and a non-empty stable `title`; and a request that does **not** throw passes through untouched with its own status and body.

- [ ] **Step 5: Run Steps C through G, then run the whole block once**

```bash
./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe -filter "/*/FeWoLearning.Security.Tests.WebAspNet/*" 2>&1 | tail -10
```
Expected on the stub tree: every block-01 fact failed, none passed. Then repeat against the solutions executable: every block-01 fact passed.

Commit message: `security: ex021–ex024 - the web-aspnet block is complete`.

---

## Task 9: Exercises 025–028 (web-blazor)

**Files:** `security/{exercises,solutions}/02-web-blazor/` — `.razor` components plus `.cs` where the exercise is a plain class; tests in `security/tests/02-web-blazor/`.

**Interfaces:** Consumes `BlazorHarness` (Task 2). Produces the block-02 style template.

**Batch filter:** `-filter "/*/*/Ex025*" -filter "/*/*/Ex026*" -filter "/*/*/Ex027*" -filter "/*/*/Ex028*"`

**Every test file in this block needs `using TestContext = Xunit.TestContext;`** alongside `using Bunit;`, or it fails `CS0104`.

- [ ] **Step 1: Write `Ex025_MarkupStringXss` as the block's worked example**

A Razor component's type name **is its file name**. Stub — `security/exercises/02-web-blazor/Ex025_MarkupStringXss.razor`:

```razor
@* Exercise 025 — MarkupStringXss (web-blazor).
   Goal:   Render a comment body that may contain a small amount of formatting
           without handing an attacker a script injection. Implement Sanitize so
           that Rendered returns markup safe to pass to MarkupString.
   Drills: MarkupString as a sink, sanitisation, when raw HTML is never safe.
   Passes: attack facts - script elements, event-handler attributes and
                          javascript: URLs never survive into the rendered output;
           use facts     - <em> and <strong> do survive, and plain text renders
                          unchanged. *@

<div id="comment">@((MarkupString)Rendered)</div>

@code {
    [Parameter] public string Body { get; set; } = "";

    // Shape A: the member the markup calls is the member that throws.
    private string Rendered => Sanitize(Body);

    public static string Sanitize(string untrusted) =>
        throw new NotImplementedException(
            "TODO: Ex025 - return only allowlisted markup; strip everything else");
}
```

Test — `security/tests/02-web-blazor/Ex025_MarkupStringXssTests.cs`:

```csharp
using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex025_MarkupStringXssTests
{
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe>")]
    public void Attack_Injection_Never_Reaches_The_Rendered_Markup(string payload)
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(p => p.Add(c => c.Body, payload));
        var html = cut.Find("#comment").InnerHtml;

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("javascript:", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<iframe", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Allowlisted_Formatting_Survives()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(
            p => p.Add(c => c.Body, "an <em>important</em> and <strong>bold</strong> point"));
        var html = cut.Find("#comment").InnerHtml;

        Assert.Contains("<em>important</em>", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<strong>bold</strong>", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Plain_Text_Renders_Unchanged()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(p => p.Add(c => c.Body, "just a comment"));

        Assert.Equal("just a comment", cut.Find("#comment").TextContent);
    }
}
```

Solution — `security/solutions/02-web-blazor/Ex025_MarkupStringXss.razor`: same markup and header (retitled `(reference solution)`), with `Sanitize` implemented as an **allowlist** — parse the input, keep only `em`, `strong`, `b`, `i` elements with no attributes at all, and HTML-encode every other node's text. Denylisting `<script>` is the wrong answer and the `onerror` and `javascript:` attack facts are there to prove it.

Note the pairing: the four attack facts alone are satisfied by `Sanitize` returning `""`. The two use facts are what make the exercise real.

- [ ] **Step 2: `Ex026_RenderTreeEncodingDefaults`** — a component `Ex026_RenderTreeEncodingDefaults.razor` with parameters `string Text` and `string CssClass`, rendering `<span id="out" class="@CssClass">@Text</span>`. This one is inverted: the learner's job is to explain, via a `public static bool RequiresManualEncoding(string sink)` helper, which sinks Blazor already encodes. Attack facts: rendering `<script>alert(1)</script>` as `Text` produces no `<script>` element (Blazor already handles this — the fact documents it); a `CssClass` of `x" onmouseover="alert(1)` does not produce an `onmouseover` attribute. Use facts: `RequiresManualEncoding("MarkupString")` is `true`, `RequiresManualEncoding("text")` and `RequiresManualEncoding("attribute")` are `false`, and the plain text and class still render correctly.

- [ ] **Step 3: `Ex027_CspNonceFlow`** — contract: a cascading `Ex027_CspNonce` record carrying `string Value`, plus component `Ex027_CspNonceFlow.razor` emitting a `<script id="s" nonce="...">`. Attack facts: the component renders **no** inline event-handler attributes (`onclick=` absent from the rendered HTML); rendering without a cascaded nonce emits **no** `script` element at all rather than an unnonced one. Use facts: with a nonce cascaded, the `script` element's `nonce` attribute equals that exact value; and the script's body content still renders.

- [ ] **Step 4: `Ex028_AuthorizeViewAndAuthState`** — a `Ex028_TestAuthStateProvider : AuthenticationStateProvider` in `_support/`, plus component `Ex028_AuthorizeViewAndAuthState.razor` using `AuthorizeView` with a `Roles="manager"` section. Attack facts: an anonymous state renders neither the manager section nor the authenticated section; a state authenticated without the `manager` role renders the authenticated section but **not** the manager section. Use facts: a `manager`-role state renders both; and the anonymous state renders the `NotAuthorized` content so the page is not simply blank.

- [ ] **Step 5: Run Steps C through G.** Commit message: `security: ex025–ex028`.

---

## Task 10: Exercises 029–032 (web-blazor)

**Batch filter:** `-filter "/*/*/Ex029*" -filter "/*/*/Ex030*" -filter "/*/*/Ex031*" -filter "/*/*/Ex032*"`

- [ ] **Step 1: `Ex029_ClientAuthIsNotEnforcement`** — the block's centrepiece. Contract: `public sealed class Ex029_PayrollService { public bool TryApprove(ClaimsPrincipal caller, int requestId, out string? denial); }` plus a component `Ex029_ClientAuthIsNotEnforcement.razor` that wraps its Approve button in `AuthorizeView Roles="approver"`. Attack facts: calling `TryApprove` **directly**, bypassing the component entirely, with a non-approver principal returns `false` — this is the fact that fails a solution which only hides the button; and with an anonymous principal returns `false`. Use facts: the component renders the button for an approver and hides it for everyone else (so the UI trimming is still implemented), and `TryApprove` returns `true` for an approver.

  This exercise is the reason the plan insists on the pairing rule: hiding the button and enforcing the rule are two separate deliverables, and the test must demand both.

- [ ] **Step 2: `Ex030_AntiforgeryInEditForm`** — component `Ex030_AntiforgeryInEditForm.razor` with an `EditForm` and a model. Attack facts: the rendered static form contains an `input` named `__RequestVerificationToken` with a non-empty value; two renders produce different token values. Use facts: the form's declared fields render with their model values, and submitting valid model data invokes the `OnValidSubmit` callback exactly once.

- [ ] **Step 3: `Ex031_SecretsNeverReachClient`** — contract: `public sealed record Ex031_ApiSettings(string PublicBaseUrl, string ApiKey)` and `public static class Ex031_SecretsNeverReachClient { public static object ToClientView(Ex031_ApiSettings settings); }`, plus a component rendering the client view as JSON into `<pre id="cfg">`. Attack facts: the rendered output contains neither the `ApiKey` value nor the string `ApiKey`; serialising the client view and searching the whole string finds no secret. Use facts: the rendered output contains the `PublicBaseUrl` value, and the client view exposes it under a stable member name the test asserts.

- [ ] **Step 4: `Ex032_JsInteropInjection`** — contract: `public static class Ex032_JsInteropInjection { public static (string Identifier, object?[] Args) BuildCall(string userInput); }`. Attack facts: the returned `Identifier` is a fixed function name and never contains any part of `userInput`; a `userInput` of `'); alert(1); ('` appears only inside `Args`, never in `Identifier`; the identifier never equals `eval`. Use facts: `Args` contains the user input verbatim, unmodified, so the JS function actually receives what the user typed; and the identifier is the documented function name for a benign input too.

- [ ] **Step 5: Run Steps C through G.** Commit message: `security: ex029–ex032`.

---

## Task 11: Exercises 033–036 (web-blazor) — block 02 completes

**Batch filter:** `-filter "/*/*/Ex033*" -filter "/*/*/Ex034*" -filter "/*/*/Ex035*" -filter "/*/*/Ex036*"`

- [ ] **Step 1: `Ex033_NavigationManagerOpenRedirect`** — contract: `public static class Ex033_NavigationManagerOpenRedirect { public static void GoTo(NavigationManager navigation, string? candidate); }`. Attack facts, all landing on the app's own `/` instead: `https://evil.example/`, `//evil.example/`, a `javascript:` URI, `null`. Use facts: `/dashboard` navigates to the app-relative `/dashboard`; `/reports?year=2026` preserves the query. Read the result from bUnit's `BunitNavigationManager.History` — note `History.First()` is the **newest** entry and there is no indexer.

- [ ] **Step 2: `Ex034_PersistentStateLeak`** — contract: a component `Ex034_PersistentStateLeak.razor` persisting a `Ex034_SessionSnapshot`. Attack facts: after `TriggerOnPersisting`, the persisted payload contains neither the user's auth token nor their email; the persisted key does not itself encode a user identifier. Use facts: the persisted payload **does** contain the display name and the last-viewed page, and `TryTake` restores them so the prerender-to-interactive handoff still works. Register the double with `AddBunitPersistentComponentState()`.

- [ ] **Step 3: `Ex035_ErrorBoundaryLeakage`** — contract: component `Ex035_ErrorBoundaryLeakage.razor` wrapping a child that throws, with an `ErrorContent`. Attack facts: the rendered error content contains neither the exception's message, nor its type name, nor a stack frame. Use facts: the rendered error content shows a stable, non-empty operator-facing message and a correlation identifier; and when the child does **not** throw, its normal content renders and no error content appears.

- [ ] **Step 4: `Ex036_SanitizingComponent`** — contract: component `Ex036_SanitizingComponent.razor` with parameters `string Html` and `IReadOnlyCollection<string> AllowedTags`. Attack facts: a tag absent from `AllowedTags` is stripped even if harmless-looking; all attributes are stripped from allowed tags; an unclosed `<em` fragment does not produce broken markup that swallows following content. Use facts: tags present in `AllowedTags` survive with their text; and an empty `AllowedTags` still renders the input's **text** content rather than nothing at all.

- [ ] **Step 5: Run Steps C through G, then run the whole block**

```bash
./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe -filter "/*/FeWoLearning.Security.Tests.WebBlazor/*" 2>&1 | tail -10
```
Expected: all block-02 facts red on stubs, all green on solutions.

Commit message: `security: ex033–ex036 - the web-blazor block is complete`.

---

## Task 12: Exercises 037–041 (desktop-core)

**Files:** `security/{exercises,solutions}/03-desktop-core/`, tests in `security/tests/03-desktop-core/`.

**Interfaces:** Consumes nothing from earlier blocks — these are plain classes with plain `[Fact]` tests, no harness. Produces the block-03 style template.

**Batch filter:** `-filter "/*/*/Ex037*" -filter "/*/*/Ex038*" -filter "/*/*/Ex039*" -filter "/*/*/Ex040*" -filter "/*/*/Ex041*"`

- [ ] **Step 1: `Ex037_DpapiProtectedData`** — contract:
  ```csharp
  public static class Ex037_DpapiProtectedData
  {
      public static byte[] Protect(byte[] plaintext, byte[] entropy);
      public static byte[] Unprotect(byte[] ciphertext, byte[] entropy);
  }
  ```
  Attack facts: for a **non-empty, distinctive** plaintext, the protected bytes never contain it as a contiguous subsequence (scope this fact to non-empty input — every byte array contains the empty one, so the empty case from the use facts would make it vacuous); `Unprotect` with different entropy throws `CryptographicException` rather than returning plaintext; protecting the same plaintext twice yields different ciphertexts. Use facts: `Unprotect(Protect(p, e), e)` equals `p` for several inputs including an empty array; and a 1 MB payload round-trips.

- [ ] **Step 2: `Ex038_CredentialStorage`** — contract:
  ```csharp
  public sealed class Ex038_CredentialStore
  {
      public Ex038_CredentialStore(string directory);
      public void Save(string name, string secret);
      public string? Load(string name);
  }
  ```
  Attack facts: after `Save`, no file under the directory contains the secret in plaintext (read every file's bytes and search); `Load` for an unknown name returns `null` rather than throwing. Use facts: `Load` returns exactly what `Save` stored, including Unicode and a 4 KB secret; and saving twice under one name overwrites rather than appending. Create the directory under a per-test temp path and delete it in `Dispose`.

- [ ] **Step 3: `Ex039_AesGcmAuthenticatedEncryption`** — contract:
  ```csharp
  public static class Ex039_AesGcmAuthenticatedEncryption
  {
      public static byte[] Encrypt(byte[] key, byte[] plaintext);
      public static byte[] Decrypt(byte[] key, byte[] envelope);
  }
  ```
  Attack facts: flipping any single byte of the envelope makes `Decrypt` throw `CryptographicException`; truncating the envelope by one byte throws; decrypting with a different key throws; encrypting the same plaintext twice with the same key yields envelopes with **different** nonces (extract and compare the nonce region). Use facts: `Decrypt(k, Encrypt(k, p))` equals `p` for an empty, a small and a 1 MB plaintext.

- [ ] **Step 4: `Ex040_KeyDerivationAndRotation`** — contract:
  ```csharp
  public sealed class Ex040_KeyRing
  {
      public int CurrentVersion { get; }
      public void Rotate();
      public byte[] Encrypt(byte[] plaintext);
      public byte[] Decrypt(byte[] envelope);
  }
  ```
  Attack facts: an envelope produced *after* a rotation cannot be decrypted by a ring whose key material predates that rotation; two rings built from different master secrets cannot read each other's envelopes at any version. Use facts: after `Rotate`, data encrypted **before** the rotation still decrypts — that is the whole point of versioned key material, and the fact that stops an implementation which simply throws the old key away; `CurrentVersion` increments by exactly one per rotation; and data encrypted after the rotation decrypts too.

- [ ] **Step 5: `Ex041_FixedTimeComparison`** — contract:
  ```csharp
  public static class Ex041_FixedTimeComparison
  {
      public static bool TokensMatch(string presented, string expected);
  }
  ```
  Attack facts: comparing strings of different lengths returns `false` **without** an early length-based exit being observable — assert this structurally, not by timing: the implementation must hash or pad both inputs to a fixed length first, so assert that `TokensMatch` returns `false` for a presented value that is a *prefix* of the expected one and for one that shares its first 31 of 32 characters. Use facts: identical tokens return `true`; the comparison is ordinal, so tokens differing only by case return `false`; and an empty presented token against an empty expected token returns `true`.

  Trap: no timing assertions anywhere. The exercise's `Passes:` line must say so explicitly, so the learner is not tempted to add one.

- [ ] **Step 6: Run Steps C through G.** Commit message: `security: ex037–ex041`.

---

## Task 13: Exercises 042–046 (desktop-core)

**Batch filter:** `-filter "/*/*/Ex042*" -filter "/*/*/Ex043*" -filter "/*/*/Ex044*" -filter "/*/*/Ex045*" -filter "/*/*/Ex046*"`

- [ ] **Step 1: `Ex042_CryptographicRandomness`** — contract: `public static class Ex042_CryptographicRandomness { public static string NewToken(int byteCount); }`. Attack facts: 1000 tokens contain no duplicate; a token is not reproducible by any seeded `System.Random` — assert by generating 1000 tokens and confirming none equals the first token a `new Random(seed)`-driven generator would produce for seeds 0–999; `NewToken(0)` throws `ArgumentOutOfRangeException`. Use facts: `NewToken(32)` decodes to exactly 32 bytes; the encoding is URL-safe (no `+`, `/` or `=` in the output).

- [ ] **Step 2: `Ex043_SignatureVerification`** — contract:
  ```csharp
  public static class Ex043_SignatureVerification
  {
      public static byte[] Sign(byte[] payload, ECDsa privateKey);
      public static bool Verify(byte[] payload, byte[] signature, ECDsa publicKey);
  }
  ```
  Attack facts: `Verify` is `false` when one payload byte is flipped; `false` for a signature from a different key pair; `false` for an empty or truncated signature (and does not throw); `false` when payload and signature are swapped in length-compatible ways. Use facts: `Verify(p, Sign(p, priv), pub)` is `true` for three payloads including an empty one; and the same payload signed twice both verify (ECDSA is randomised).

- [ ] **Step 3: `Ex044_UpdateIntegrityAndRollback`** — contract:
  ```csharp
  public sealed record Ex044_UpdateManifest(string Version, string Sha256, byte[] Signature);
  public static class Ex044_UpdateIntegrityAndRollback
  {
      public static bool ShouldInstall(Ex044_UpdateManifest manifest, byte[] payload, ECDsa publisherKey, string installedVersion, out string? rejection);
  }
  ```
  Attack facts: a manifest whose `Sha256` does not match `payload` is rejected; a manifest signed by another key is rejected; a manifest whose version is **lower** than `installedVersion` is rejected even when perfectly signed (rollback); an equal version is rejected. Use facts: a correctly signed, correctly hashed, higher version returns `true` with `rejection` null; and version comparison is semantic, so `1.10.0` is accepted over `1.9.0`.

- [ ] **Step 4: `Ex045_UnsafeDeserialization`** — contract:
  ```csharp
  public static class Ex045_UnsafeDeserialization
  {
      public static bool TryDeserialize(string json, IReadOnlyCollection<Type> allowedTypes, out object? value, out string? rejection);
  }
  ```
  Attack facts: a payload naming a type outside `allowedTypes` is rejected; a payload naming a type by assembly-qualified name is rejected even if that type exists; the `rejection` message does not echo the attacker-supplied type name back (log injection). Use facts: a payload naming an allowed type deserialises to an instance of it with its properties populated; and two different allowed types both work through the same call.

- [ ] **Step 5: `Ex046_XmlExternalEntity`** — contract: `public static class Ex046_XmlExternalEntity { public static string? ReadTitle(Stream xml); }`. Attack facts: a document declaring an external entity pointing at a local file returns the title **without** the file's contents, or rejects the document — assert the returned string does not contain the sentinel the test wrote into that file; a billion-laughs document does not hang or exhaust memory (it must return or throw within the test, having rejected DTD processing); a document with an external DTD reference does not attempt the fetch. Use facts: a plain well-formed document returns its title; a document with a UTF-8 BOM and a namespace still returns its title.

- [ ] **Step 6: Run Steps C through G.** Commit message: `security: ex042–ex046`.

---

## Task 14: Exercises 047–052 (desktop-core) — block 03 completes

**Batch filter:** `-filter "/*/*/Ex047*" -filter "/*/*/Ex048*" -filter "/*/*/Ex049*" -filter "/*/*/Ex050*" -filter "/*/*/Ex051*" -filter "/*/*/Ex052*"`

This batch is six rather than five so block 03 closes on a task boundary.

- [ ] **Step 1: `Ex047_ZipSlipExtraction`** — contract: `public static class Ex047_ZipSlipExtraction { public static IReadOnlyList<string> ExtractTo(Stream archive, string destinationDirectory); }`. Attack facts: an entry named `../escaped.txt` is not written and no file appears outside `destinationDirectory`; an entry with an absolute path is not written; an entry named `sub/../../escaped.txt` is not written. Use facts: ordinary entries `a.txt` and `sub/b.txt` are written with correct content, and the returned list names exactly the files written.

- [ ] **Step 2: `Ex048_PathCanonicalization`** — contract: `public static class Ex048_PathCanonicalization { public static bool IsInside(string root, string candidate); }`. Attack facts, all `false`: a sibling directory whose name merely *starts with* the root's name (`C:\data-evil` against root `C:\data` — the classic `StartsWith` bug); a `..`-escaping path; a UNC path; a device path (`\\?\C:\...`) escaping the root; a path with a trailing alternate data stream. Use facts: the root itself is inside; a nested file is inside; a nested path written with forward slashes is inside.

- [ ] **Step 3: `Ex049_ProcessArgumentInjection`** — contract: `public static class Ex049_ProcessArgumentInjection { public static ProcessStartInfo BuildStartInfo(string executable, IReadOnlyList<string> arguments); }`. Attack facts: an argument containing `" & del /q *` appears as **one** entry in `ArgumentList` and the `Arguments` string property is empty; `UseShellExecute` is `false`; an argument containing a newline stays one entry. Use facts: three ordinary arguments appear as three `ArgumentList` entries in order, verbatim; and `FileName` equals `executable`.

- [ ] **Step 4: `Ex050_NamedPipeAccessControl`** — contract: `public static class Ex050_NamedPipeAccessControl { public static NamedPipeServerStream CreateServer(string pipeName); }`. Attack facts: the pipe's `PipeSecurity` grants no rights to `WellKnownSidType.WorldSid` or `AuthenticatedUserSid`; it does not grant `PipeAccessRights.ChangePermissions` to anyone but the owner. Use facts: the current user has `ReadWrite`; and a client connecting as the current user actually completes a round-trip message. Use a per-test unique pipe name (`$"fewo-sec-{Guid.NewGuid():N}"`) so a leftover cannot collide, and dispose the server.

- [ ] **Step 5: `Ex051_SecretRedactionInLogs`** — contract: `public static class Ex051_SecretRedactionInLogs { public static string Redact(string message, IReadOnlyDictionary<string, object?> state); }`. Attack facts: values under keys named `password`, `apiKey`, `authorization` and `token` (case-insensitively) never appear in the output; a value appearing inside the message template itself is redacted too; a CR or LF inside any value is neutralised so a forged log line cannot be injected. Use facts: non-sensitive keys and their values do appear; the message's non-sensitive text is preserved verbatim; and a key named `passwordPolicyVersion` is **not** redacted — the fact that stops a naive `Contains("password")` implementation.

- [ ] **Step 6: `Ex052_RestrictiveFileAcl`** — contract: `public static class Ex052_RestrictiveFileAcl { public static void WriteSecret(string path, byte[] content); }`. Attack facts: the created file's ACL grants nothing to `WorldSid` or `AuthenticatedUserSid`; ACL inheritance is disabled on the file so a permissive parent directory does not leak rights. Use facts: the current user can read the file back and the content matches; and writing twice replaces rather than appends. Work under a per-test temp directory, and set that directory's ACL permissively first so the inheritance-disabled fact is meaningful.

- [ ] **Step 7: Run Steps C through G, then the whole block**

```bash
./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe -filter "/*/FeWoLearning.Security.Tests.DesktopCore/*" 2>&1 | tail -10
```

Commit message: `security: ex047–ex052 - the desktop-core block is complete`.

---

## Task 15: Exercises 053–056 (desktop-wpf)

**Files:** `security/{exercises,solutions}/04-desktop-wpf/`, tests in `security/tests/04-desktop-wpf/`.

**Interfaces:** Consumes `WpfPump.Pump` (Task 2). Every test in this block uses `[WpfFact]`, never `[Fact]`.

**Batch filter:** `-filter "/*/*/Ex053*" -filter "/*/*/Ex054*" -filter "/*/*/Ex055*" -filter "/*/*/Ex056*"`

- [ ] **Step 1: `Ex053_PasswordBoxNoPlaintextBinding`** — contract:
  ```csharp
  public sealed class Ex053_LoginViewModel { public bool CanSubmit { get; } /* plus what the exercise needs */ }
  public static class Ex053_PasswordBoxNoPlaintextBinding
  {
      public static void Attach(PasswordBox box, Ex053_LoginViewModel viewModel);
  }
  ```
  Attack facts: the view model exposes **no** public member of type `string` whose name contains `password` — assert by reflection over its public properties and fields, so a solution that just adds a plaintext property fails; the view model's public surface exposes no `PasswordBox` either. Use facts: after `Attach`, typing into the box (set `box.Password`, then `WpfPump.Pump()`) flips `CanSubmit` from `false` to `true`; clearing it flips it back.

  Trap: this is the reflection-graded row. State in `Passes:` that the grading reads metadata, so the learner understands why.

- [ ] **Step 2: `Ex054_SensitiveBufferLifetime`** — contract:
  ```csharp
  public static class Ex054_SensitiveBufferLifetime
  {
      public static T UseThenClear<T>(char[] secret, Func<char[], T> work);
  }
  ```
  Attack facts: after the call returns, every element of the caller's array is `'\0'`; the array is cleared even when `work` throws (assert by catching and then inspecting). Use facts: the value `work` returned is passed through unchanged; and `work` observed the **original** characters, not zeros — the fact that stops an implementation which clears before calling.

- [ ] **Step 3: `Ex055_ClipboardHygiene`** — contract:
  ```csharp
  public static class Ex055_ClipboardHygiene
  {
      public static void CopySecret(string secret);
  }
  ```
  Attack facts: the `DataObject` placed on the clipboard carries the format that excludes it from clipboard history and cloud sync (`"ExcludeClipboardContentFromMonitorProcessing"` and `"CanIncludeInClipboardHistory"` set to false). Use facts: `Clipboard.GetText()` returns the secret, so paste still works.

  **This row touches the real system clipboard, which is shared with the developer's session.** The test must capture the prior clipboard content in its constructor and restore it in `Dispose`, and `security/README.md` must warn that running the suite briefly disturbs the clipboard. Wrap clipboard reads in a retry — the Win32 clipboard can be momentarily locked by another process.

- [ ] **Step 4: `Ex056_DragDropUntrustedPayload`** — contract:
  ```csharp
  public static class Ex056_DragDropUntrustedPayload
  {
      public static IReadOnlyList<string> AcceptableFiles(IDataObject data, string allowedRoot, IReadOnlyCollection<string> allowedExtensions);
  }
  ```
  Attack facts: a `DataObject` carrying a `.exe` yields an empty list; one carrying a path outside `allowedRoot` yields an empty list; one carrying no `FileDrop` format at all yields an empty list rather than throwing. Use facts: a drop of two allowed files under the root yields both, in order; a mixed drop yields only the allowed ones rather than rejecting the whole batch.

- [ ] **Step 5: Run Steps C through G.** Commit message: `security: ex053–ex056`.

---

## Task 16: Exercises 057–060 (desktop-wpf) — block 04 completes, all 60 written

**Batch filter:** `-filter "/*/*/Ex057*" -filter "/*/*/Ex058*" -filter "/*/*/Ex059*" -filter "/*/*/Ex060*"`

- [ ] **Step 1: `Ex057_EmbeddedBrowserNavigationPolicy`** — contract:
  ```csharp
  public sealed record Ex057_NavigationDecision(bool Allow, bool OpenExternally, string? Reason);
  public static class Ex057_EmbeddedBrowserNavigationPolicy
  {
      public static Ex057_NavigationDecision Decide(string targetUri, string appOrigin);
  }
  ```
  No WebView2 reference — this is the decision surface as a standalone class, per spec §6. Attack facts: `javascript:` is denied; `file:///` is denied; a `data:text/html` URI is denied; a plain-`http` URL is denied. Use facts: a URL on `appOrigin` is allowed in-frame (`Allow` true, `OpenExternally` false); an `https` URL on another host is allowed but `OpenExternally` true — the fact that stops a deny-everything policy.

- [ ] **Step 2: `Ex058_XamlReaderUntrustedMarkup`** — contract: `public static class Ex058_XamlReaderUntrustedMarkup { public static bool TryLoadShape(string markup, out System.Windows.Shapes.Shape? shape); }`. Attack facts: markup instantiating a non-`Shape` type (an `ObjectDataProvider`, a `Window`) returns `false` with `shape` null; markup declaring `x:Code` returns `false`; markup referencing an arbitrary CLR namespace via `clr-namespace:` returns `false`. Use facts: a plain `<Rectangle Width="10" Height="4"/>` returns `true` with a `Rectangle` whose `Width` is 10; an `<Ellipse/>` also loads.

- [ ] **Step 3: `Ex059_BindingErrorLeakage`** — contract: `public static class Ex059_BindingErrorLeakage { public static void Bind(TextBlock target, object source, string path, string fallback); }`. Attack facts: when `path` does not exist on `source`, the `TextBlock`'s `Text` is `fallback` and contains neither the type name nor the path; the element's `ToolTip` is null or free of both. Use facts: when the path resolves, `Text` shows the value; and after mutating the source and calling `WpfPump.Pump(DispatcherPriority.DataBind)`, `Text` follows the new value — the fact that proves a live binding rather than a one-time assignment.

  This is the `wpf/`-family trap: an implementation that reads the property once and assigns the string passes every static assertion. The mutate-then-pump fact is mandatory.

- [ ] **Step 4: `Ex060_FilePickerResultStillUntrusted`** — contract:
  ```csharp
  public static class Ex060_FilePickerResultStillUntrusted
  {
      public static bool TryAcceptPickedPath(string pickedPath, string allowedRoot, long maxBytes, out string? rejection);
  }
  ```
  Attack facts: a path outside `allowedRoot` is rejected even though a dialog produced it; a path that is a symbolic link resolving outside the root is rejected; a file exceeding `maxBytes` is rejected; a path naming a directory is rejected. Use facts: an ordinary in-root file under the size limit is accepted with `rejection` null; and a file exactly at `maxBytes` is accepted (boundary, not off-by-one).

- [ ] **Step 5: Run Steps C through G, then the whole suite twice**

```bash
cd security && dotnet build 2>&1 | tail -5
./tests/bin/Debug/net10.0-windows/FeWoLearning.Security.Tests.exe 2>&1 | tail -10
```
Expected: **3 passed** (the harness smoke tests) and every other fact failed.

```bash
dotnet build -p:UseSolutions=true 2>&1 | tail -5
<solutions exe> 2>&1 | tail -10
```
Expected: **0 failed**, and the solutions build emitted 0 warnings.

Record both exact numbers — they go into the README and CLAUDE.md in Tasks 17 and 18.

Commit message: `security: ex057–ex060 - all 60 exercises are written`.

---

## Task 17: `security/README.md`

Write this **as findings accumulate**, not from memory at the end. If earlier tasks discovered anything not predicted here, it belongs in this file.

**Files:**
- Create: `security/README.md`

- [ ] **Step 1: Write the setup and commands section**

Must state: Windows-only; needs an interactive desktop session for block 04; `dotnet build` then run the test executable, **with the spec §2.4 caveat that `dotnet test` reports zero tests in this environment**, including for the pre-existing `wpf/` track; the exact stub and solutions executable paths; and the `-filter` form for one exercise.

- [ ] **Step 2: Write the "how a security test lies" section**

Four entries, each with the concrete symptom:
1. **An attack fact with no use fact grades nothing** — reject-everything passes it. The rule the track lives by.
2. **A hard-coded crypto digest** tests transcription, not behaviour, and breaks on any legitimate parameter change.
3. **A wall-clock timing assertion** is flaky by construction; assert the mechanism.
4. **`Assert.Throws` on a stub that already throws** is a false green.

- [ ] **Step 3: Write the toolchain-traps section**

At minimum: the `Bunit.TestContext` / `Xunit.TestContext` `CS0104` collision and its alias fix; the `NU1903` pin on `SQLitePCLRaw.lib.e_sqlite3`; the `NU1510` trap on `Microsoft.Extensions.Hosting` and `ProtectedData`; why `Directory.Build.props` is required (`CS0579`); why the test project uses the plain SDK and not the Razor SDK.

- [ ] **Step 4: Write the per-row warnings**

Row 055 disturbs the system clipboard while the suite runs. Rows 049, 050 and 052 create real OS objects under per-test temp directories. Row 050 uses a unique pipe name per test. Row 053 is graded by reflection over the view model's public surface. Row 057 deliberately does not reference WebView2.

- [ ] **Step 5: Commit**

```bash
git add security/README.md
git commit -m "security: track README with the harness traps and per-row warnings"
```

---

## Task 18: Register the track in the repo-level docs

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/exercise-format.md`

- [ ] **Step 1: Add `security/` to the per-track command table in `CLAUDE.md`**

Row: install `—`; run all tests — the build-plus-executable form, not `dotnet test`; run one exercise — the `-filter` form. Add a footnote that this track and `wpf/` both hit the zero-discovery behaviour.

- [ ] **Step 2: Add a `security/` entry to the track-specific gotchas section of `CLAUDE.md`**

Cover: 60 rows in four **attack-surface** blocks rather than 100 in four difficulty tiers, and why; the three-project `UseSolutions` layout; the four block namespaces; the `CS0104` bUnit/xunit collision; the `NU1903` pin; the attack-fact/use-fact pairing rule as the track's recurring bug class.

- [ ] **Step 3: Add `security/` to the "Current state" table in `CLAUDE.md`**

Written 60 / 60, using the exact red and green counts recorded in Task 16 Step 5. State that it is verified end-to-end, and by which command.

- [ ] **Step 4: Add `security/` to the naming table in `docs/exercise-format.md`**

Entry: "one file per exercise per block, block-wide namespace, `.razor` for block 02, test in a separate `tests/` project", example `exercises/01-web-aspnet/Ex001_SecurityHeaders.cs`.

- [ ] **Step 5: Record the tier-scheme deviation in `docs/exercise-format.md`**

The numbering-and-tiers section states 001–100 in four difficulty tiers as universal. Add the exception explicitly, or the next reader will assume it holds. Note that `security/` is the only track with 60 rows and that its blocks are attack surfaces, not difficulty levels.

- [ ] **Step 6: Add `security/` to the "Known gaps" note**

`security/` belongs on the list of tracks whose `solutions/` is **in** the build and therefore cannot drift silently — alongside `blazor/`, `avalonia/`, `uno/`, `caliburn/` and `wpf/`.

- [ ] **Step 7: Commit**

```bash
git add CLAUDE.md docs/exercise-format.md
git commit -m "docs: register the security track and its 60-row block scheme"
```

---

## Plan Self-Review

Run against the spec after the plan is written; findings were fixed inline.

**Spec coverage:** §1 → Task 3 header. §2.1–2.2 → Task 1 Steps 4–7. §2.3 → Task 1 Step 4 pin, Task 1 Step 10 check. §2.4 → Global Constraints, every Step C/D, Task 17 Step 1. §2.5 → Task 17 Step 1. §3 → Task 1. §3.1 → Global Constraints, Task 3. §3.2 → Task 2. §3.3 → Global Constraints, Task 2 Step 3, Task 9 preamble. §4.1–4.2 → the shared procedure's Step E, and every batch task's traps. §4.3 → Task 5 Steps 1–2. §5 → Task 3, Task 18 Step 5. §5.1–5.4 → Tasks 4–16. §6 → Task 16 Step 1, Task 17 Step 4. §7 → the shared procedure. §8 → task ordering.

**Type consistency:** `WebHarness.StartAsync(services, configure, ct)` is used with named arguments in Task 2 Step 5 and Task 4 Step 1, matching the signature in Task 2 Step 2. `WpfPump.Pump` is declared in Task 2 Step 4 and used in Tasks 15 and 16. `BlazorHarness` is declared in Task 2 Step 3 and used in Tasks 9–11. `SmokeProbe` and `SmokeGreeter` are declared in Task 2 Step 1 and used only in Task 2 Step 5. Every `ExNNN_` type named in a later task is declared in that same task's contract.

**Batch arithmetic:** 5+5+5+5+4 = 24 (block 01), 4+4+4 = 12 (block 02), 5+5+6 = 16 (block 03), 4+4 = 8 (block 04). Total **60**, matching the catalog seeded in Task 3.
