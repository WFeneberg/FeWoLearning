# MicroServices Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Stand up the `MicroServices/` track — scaffolding, test harness, DevContainer, 100-row catalog and the first five exercises — so that `dotnet test` is red against the stubs and green against the reference solutions.

**Architecture:** Two content libraries (`exercises/`, `solutions/`) compile the same type names into the same namespaces; `tests/` references exactly one of them via the `UseSolutions` MSBuild property. Each exercise is a static class with a `Configure(IDistributedApplicationBuilder)` method. Tests assert on the Aspire application model built in-process — no containers — with an opt-in level that starts real databases.

**Tech Stack:** .NET 10.0.400, Aspire 13.5.3, xunit.v3 3.2.2 on the classic VSTest path, Docker 29.7.2, devcontainer CLI 0.89.0.

**Spec:** [`docs/superpowers/specs/2026-09-05-microservices-track-design.md`](../specs/2026-09-05-microservices-track-design.md)

## Global Constraints

- Track folder is **`MicroServices/`**, capitalised. This deviates from every other track's lowercase name and is deliberate — do not "fix" it.
- Pin exactly: `Aspire.Hosting*` **13.5.3** (except `Aspire.Hosting.Elasticsearch` **13.3.0**), `xunit.v3` **3.2.2**, `xunit.runner.visualstudio` **3.1.5**, `Microsoft.NET.Test.Sdk` **17.14.1**.
- **Never** add `xunit.v3` 4.0.0, `xunit.runner.visualstudio` 4.0.0, or a `global.json` with the `Microsoft.Testing.Platform` runner. That combination makes `dotnet test` exit 5 with zero tests on this machine. Treat `NU1603` as an error: naming a nonexistent `3.1.6`/`3.1.7` silently resolves *forward* to 4.0.0.
- Tier namespaces are pinned because `01-beginner` is not a valid C# identifier: `FeWoLearning.MicroServices.Exercises.{Beginner,Intermediate,Advanced,Expert}`. `solutions/` uses the **same** namespaces and type names.
- Stubs throw `NotImplementedException`. A stub that fails to **compile** is a bug — the learner must get a red test, not a build error.
- `solutions/` builds with **0 warnings**. Warnings from `exercises/` caused by unused stub fields are expected and stay unsuppressed.
- Do not guess Aspire API shapes. Use `aspire docs api search "<concept>" --language csharp` and `aspire docs get "<slug>"`. Aspire 13's surface has moved and tutorials are frequently wrong.
- Every persistence assertion checks **both** the resource type and `ConnectionStringExpression.ValueExpression` (spec §8.2). Asserting only that "a container exists" grades nothing.
- Commit messages for exercise batches: `MicroServices: exNNN–exNNN`. Stage explicit paths; never `git add -A`.

---

### Task 1: Track skeleton that builds and tests in both modes

**Files:**
- Create: `MicroServices/FeWoLearning.MicroServices.slnx`
- Create: `MicroServices/Directory.Build.props`
- Create: `MicroServices/exercises/FeWoLearning.MicroServices.Exercises.csproj`
- Create: `MicroServices/solutions/FeWoLearning.MicroServices.Solutions.csproj`
- Create: `MicroServices/tests/FeWoLearning.MicroServices.Tests.csproj`
- Create: `MicroServices/exercises/01-beginner/_TierMarker.cs`
- Create: `MicroServices/solutions/01-beginner/_TierMarker.cs`
- Test: `MicroServices/tests/_support/HarnessSmokeTests.cs`

**Interfaces:**
- Consumes: nothing.
- Produces: the three project files; the `UseSolutions` switch; namespace `FeWoLearning.MicroServices.Exercises.Beginner` containing `public static class TierMarker { public static string Tier => "01-beginner"; }` in **both** content libraries.

- [ ] **Step 1: Create the solution file**

`MicroServices/FeWoLearning.MicroServices.slnx`:

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.MicroServices.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.MicroServices.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.MicroServices.Tests.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 2: Create `Directory.Build.props`**

This is required, not cosmetic. `exercises/` and `solutions/` emit the same assembly-info attributes; sharing an `obj/` tree fails the build with `CS0579`. It must live here, not in a `.csproj` body — `BaseOutputPath` set inside a project is read after the SDK props import, too late.

```xml
<Project>

  <!-- Redirect the solutions build to its own output tree. Required: exercises/ and
       solutions/ compile the same type names into the same namespaces, so sharing an
       obj/ tree makes the build fail with CS0579 on duplicate generated assembly-info
       attributes. Cannot live in the .csproj body - BaseOutputPath/
       BaseIntermediateOutputPath set there are read after the SDK props import. -->
  <PropertyGroup Condition="'$(UseSolutions)' == 'true'">
    <UseArtifactsOutput>true</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildThisFileDirectory)artifacts-solutions</ArtifactsPath>
  </PropertyGroup>

</Project>
```

- [ ] **Step 3: Create both content libraries**

`MicroServices/exercises/FeWoLearning.MicroServices.Exercises.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.MicroServices.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.MicroServices.Exercises</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- Aspire hosting integrations. Keep this list identical in solutions/.
       All 13.5.3 except Elasticsearch, whose latest stable is 13.3.0 - a known
       lag, documented in MicroServices/README.md, not silently bumped. -->
  <ItemGroup>
    <PackageReference Include="Aspire.Hosting" Version="13.5.3" />
    <PackageReference Include="Aspire.Hosting.PostgreSQL" Version="13.5.3" />
    <PackageReference Include="Aspire.Hosting.SqlServer" Version="13.5.3" />
    <PackageReference Include="Aspire.Hosting.MongoDB" Version="13.5.3" />
    <PackageReference Include="Aspire.Hosting.Redis" Version="13.5.3" />
  </ItemGroup>

</Project>
```

`MicroServices/solutions/FeWoLearning.MicroServices.Solutions.csproj` is **identical except** for these two lines:

```xml
    <RootNamespace>FeWoLearning.MicroServices.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.MicroServices.Solutions</AssemblyName>
```

Note the `RootNamespace` stays `…Exercises` in both — that is what makes the two libraries interchangeable to `tests/`.

Later tasks add integration packages (Kafka, Qdrant, Azure, …) as their tiers need them. Add each to **both** files in the same commit, always.

- [ ] **Step 4: Create the tier marker in both libraries**

`MicroServices/exercises/01-beginner/_TierMarker.cs` and `MicroServices/solutions/01-beginner/_TierMarker.cs`, byte-identical:

```csharp
namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Proves the two content libraries are interchangeable and that the tier
/// namespace is wired up. Not an exercise; never gets a catalog row.
/// </summary>
public static class TierMarker
{
    public static string Tier => "01-beginner";
}
```

- [ ] **Step 5: Create the test project**

`MicroServices/tests/FeWoLearning.MicroServices.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.MicroServices.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- xunit.v3 3.2.2 on the classic VSTest path, and NO global.json.
       xunit.v3 4.0.0 + a Microsoft.Testing.Platform global.json makes
       `dotnet test` exit 5 with zero tests discovered on the .NET 10.0.400 SDK.
       runner.visualstudio has no 3.1.6/3.1.7 - 3.1.5 is the last 3.x, and naming
       a nonexistent one resolves FORWARD to 4.0.0 with only an NU1603 warning. -->
  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.Testing" Version="13.5.3" />
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one content library, never both: that is what keeps the identical
       namespaces and type names from colliding. `dotnet test` is the red run,
       `dotnet test -p:UseSolutions=true` the green one. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.MicroServices.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.MicroServices.Solutions.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 6: Write the harness smoke test**

`MicroServices/tests/_support/HarnessSmokeTests.cs`:

```csharp
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Fails first when the two-library UseSolutions mechanism breaks. These facts
/// must pass in BOTH the red run and the green run - they grade the harness,
/// not an exercise.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void Tier_marker_resolves_from_whichever_library_is_referenced()
        => Assert.Equal("01-beginner", TierMarker.Tier);

    [Fact]
    public void Exactly_one_content_library_is_loaded()
    {
        var names = typeof(TierMarker).Assembly.GetName().Name;
        Assert.True(
            names is "FeWoLearning.MicroServices.Exercises" or "FeWoLearning.MicroServices.Solutions",
            $"Unexpected content assembly: {names}");
    }
}
```

- [ ] **Step 7: Run the red mode**

Run: `cd MicroServices && dotnet test`
Expected: **2 passed, 0 failed.** The smoke tests pass in both modes by design; there are no exercises yet.

If this reports "no tests ran" / exit code 5, the pinning constraint was violated — re-check that there is no `global.json` and that xunit.v3 is 3.2.2.

- [ ] **Step 8: Run the green mode**

Run: `cd MicroServices && dotnet test -p:UseSolutions=true`
Expected: **2 passed, 0 failed**, and `Exactly_one_content_library_is_loaded` sees `FeWoLearning.MicroServices.Solutions`.

- [ ] **Step 9: Confirm the output redirection actually happened**

Run: `ls MicroServices/artifacts-solutions`
Expected: the directory exists. If it does not, `Directory.Build.props` is not being picked up and the two builds are sharing `obj/` — fix before proceeding, or `CS0579` will appear later and be much harder to attribute.

- [ ] **Step 10: Commit**

```bash
git add MicroServices/FeWoLearning.MicroServices.slnx MicroServices/Directory.Build.props \
        MicroServices/exercises MicroServices/solutions MicroServices/tests
git commit -m "MicroServices: track skeleton with UseSolutions switch"
```

---

### Task 2: Test harness — model, manifest and container gate

**Files:**
- Create: `MicroServices/tests/_support/ModelHarness.cs`
- Create: `MicroServices/tests/_support/ManifestHarness.cs`
- Create: `MicroServices/tests/_support/ContainerGate.cs`
- Modify: `MicroServices/tests/FeWoLearning.MicroServices.Tests.csproj` (add `RuntimeHostConfigurationOption`)
- Test: `MicroServices/tests/_support/HarnessSmokeTests.cs` (extend)

**Interfaces:**
- Consumes: Task 1's projects.
- Produces:
  - `ModelHarness.Build(Action<IDistributedApplicationBuilder> configure)` → `DistributedApplicationModel`-like access; concretely returns `ModelHarness.Result` with `IReadOnlyList<IResource> Resources`.
  - `ModelHarness.ConnectionString(IResource)` → `string` (the `ValueExpression`).
  - `ManifestHarness.GenerateAsync(Action<IDistributedApplicationBuilder>)` → `Task<JsonDocument>` of `aspire-manifest.json`.
  - `ContainerGate.Require()` → skips the calling test unless containers are enabled.

- [ ] **Step 1: Write the failing harness tests**

Append to `MicroServices/tests/_support/HarnessSmokeTests.cs`:

```csharp
using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

// ... existing HarnessSmokeTests class stays as-is; add this class below it:

public class HarnessMechanicsTests
{
    [Fact]
    public void ModelHarness_exposes_resources_and_connection_strings()
    {
        var model = ModelHarness.Build(b =>
        {
            var pg = b.AddPostgres("pg").AddDatabase("orders");
            b.AddContainer("worker", "busybox").WithReference(pg).WaitFor(pg);
        });

        var orders = model.Resource("orders");
        Assert.IsType<PostgresDatabaseResource>(orders);
        Assert.Equal("{pg.connectionString};Database=orders", ModelHarness.ConnectionString(orders));
        Assert.Equal(2, model.Resource("worker").Annotations.OfType<WaitAnnotation>().Count());
    }

    [Fact]
    public async Task ManifestHarness_generates_a_manifest_in_process()
    {
        using var manifest = await ManifestHarness.GenerateAsync(b => b.AddPostgres("pg").AddDatabase("orders"));

        var pg = manifest.RootElement.GetProperty("resources").GetProperty("pg");
        Assert.Equal("container.v0", pg.GetProperty("type").GetString());
        Assert.StartsWith("docker.io/library/postgres:", pg.GetProperty("image").GetString());
    }

    [Fact]
    public void ContainerGate_Require_skips_when_containers_are_off()
    {
        ContainerGate.Require();
        Assert.True(ContainerGate.Enabled, "Require() let a test through with containers off.");
    }
}
```

- [ ] **Step 2: Run to verify it fails**

Run: `cd MicroServices && dotnet test`
Expected: FAIL — `ModelHarness`, `ManifestHarness` and `ContainerGate` do not exist (CS0103 / CS0246).

- [ ] **Step 3: Implement `ModelHarness`**

`MicroServices/tests/_support/ModelHarness.cs`:

```csharp
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Builds an Aspire application model in-process and hands back its resources.
/// Measured at ~1.4 s and starts NO containers - this is the workhorse of the
/// track's L1 assertions.
/// </summary>
public static class ModelHarness
{
    public sealed class Result
    {
        private readonly IReadOnlyList<IResource> _resources;

        internal Result(IReadOnlyList<IResource> resources) => _resources = resources;

        public IReadOnlyList<IResource> Resources => _resources;

        /// <summary>Resource by name, with a failure message that lists what IS there.</summary>
        public IResource Resource(string name)
            => _resources.SingleOrDefault(r => r.Name == name)
               ?? throw new InvalidOperationException(
                   $"No resource named '{name}'. Model contains: " +
                   string.Join(", ", _resources.Select(r => $"{r.Name}({r.GetType().Name})")));

        public bool Has(string name) => _resources.Any(r => r.Name == name);
    }

    public static Result Build(Action<IDistributedApplicationBuilder> configure)
    {
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = [],
            DisableDashboard = true
        });
        configure(builder);
        using var app = builder.Build();
        return new Result(builder.Resources.ToList());
    }

    /// <summary>
    /// The connection-string EXPRESSION, not a resolved value. It differs per
    /// database flavour, which is what lets a test prove the learner wired up
    /// PostgreSQL rather than merely some container (spec section 8.2).
    /// </summary>
    public static string ConnectionString(IResource resource)
        => resource is IResourceWithConnectionString cs
            ? cs.ConnectionStringExpression.ValueExpression
            : throw new InvalidOperationException(
                $"Resource '{resource.Name}' ({resource.GetType().Name}) has no connection string.");
}
```

- [ ] **Step 4: Implement `ManifestHarness`**

`MicroServices/tests/_support/ManifestHarness.cs`:

```csharp
using System.Text.Json;
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Runs Aspire's publish operation in-process (~3.7 s) and returns the generated
/// aspire-manifest.json.
///
/// Do NOT shell out to `aspire publish`: it writes its artifacts and then does not
/// exit in a non-interactive shell, dropping into "press CTRL+C to stop the AppHost".
/// Measured still running at 600 s.
///
/// The manifest carries per resource: type (container.v0 / value.v0 / parameter.v0),
/// the pinned image, the full env map including ConnectionStrings__*, bindings with
/// targetPort, and the generated-secret policy. Docker Compose YAML is NOT produced
/// in-process - see the spec for how compose rows are graded instead.
/// </summary>
public static class ManifestHarness
{
    public static async Task<JsonDocument> GenerateAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        var dir = Path.Combine(Path.GetTempPath(), "fewo-ms-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
            {
                Args = ["--operation", "publish", "--output-path", dir],
                DisableDashboard = true
            });
            configure(builder);
            using var app = builder.Build();

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(90));
            await app.RunAsync(timeout.Token);

            var path = Path.Combine(dir, "aspire-manifest.json");
            if (!File.Exists(path))
            {
                throw new InvalidOperationException(
                    $"Publish produced no manifest. Files present: " +
                    (Directory.Exists(dir)
                        ? string.Join(", ", Directory.GetFiles(dir).Select(Path.GetFileName))
                        : "<no directory>"));
            }

            return JsonDocument.Parse(await File.ReadAllTextAsync(path, cancellationToken));
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
        }
    }
}
```

- [ ] **Step 5: Implement `ContainerGate`**

`MicroServices/tests/_support/ContainerGate.cs`:

```csharp
namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// The opt-in for L3 tests that start real database containers.
///
/// FactAttribute.Skip is NOT virtual in xunit.v3 3.2.2, so the usual
/// "custom [ContainerFact] overriding Skip" pattern does not compile (CS0506).
/// Gating happens in the test body via Assert.SkipUnless instead.
/// </summary>
public static class ContainerGate
{
    public static bool Enabled =>
        Environment.GetEnvironmentVariable("FEWO_MS_CONTAINERS") == "1"
        || AppContext.GetData("FeWoLearning.MicroServices.Containers") as string == "true";

    /// <summary>
    /// Call as the first line of any test that needs a real container.
    ///
    /// Deliberately checks only the switch, never whether Docker is reachable: with
    /// the switch ON and no daemon the test must FAIL, loudly, when Aspire cannot
    /// start the container. A broken Docker setup must not be able to masquerade as
    /// a green run by silently skipping (spec section 5).
    /// </summary>
    public static void Require() =>
        Assert.SkipUnless(Enabled,
            "Container tests are off. Enable with: dotnet test -p:Containers=true");
}
```

- [ ] **Step 6: Wire the MSBuild property through to runtime**

An MSBuild property is invisible at runtime. `RuntimeHostConfigurationOption` writes it into `runtimeconfig.json`, where `AppContext.GetData` can read it. Add to `MicroServices/tests/FeWoLearning.MicroServices.Tests.csproj`, before `</Project>`:

```xml
  <!-- `-p:Containers=true` reaches the test process through runtimeconfig.json.
       FEWO_MS_CONTAINERS=1 is the no-rebuild alternative. -->
  <ItemGroup>
    <RuntimeHostConfigurationOption Include="FeWoLearning.MicroServices.Containers"
                                    Value="$(Containers)"
                                    Condition="'$(Containers)' != ''" />
  </ItemGroup>
```

- [ ] **Step 7: Run with containers off**

Run: `cd MicroServices && dotnet test`
Expected: **4 passed, 1 skipped, 0 failed.** The skipped one is `ContainerGate_Require_skips_when_containers_are_off` — proving the gate closes.

- [ ] **Step 8: Run with containers on**

Run: `cd MicroServices && dotnet test -p:Containers=true`
Expected: **5 passed, 0 skipped, 0 failed** — proving the gate opens. (No real container starts yet; only the gate is exercised.)

- [ ] **Step 9: Run the green mode**

Run: `cd MicroServices && dotnet test -p:UseSolutions=true`
Expected: **4 passed, 1 skipped** — harness facts are mode-independent.

- [ ] **Step 10: Commit**

```bash
git add MicroServices/tests
git commit -m "MicroServices: model, manifest and container-gate harness"
```

---

### Task 3: Shared services and the playground AppHost

**Files:**
- Create: `MicroServices/services/Catalog/Catalog.csproj`, `MicroServices/services/Catalog/Program.cs`
- Create: `MicroServices/services/Orders/Orders.csproj`, `MicroServices/services/Orders/Program.cs`
- Create: `MicroServices/playground/Playground.AppHost.csproj`
- Create: `MicroServices/playground/AppHost.cs`
- Create: `MicroServices/playground/ExerciseRegistry.cs`
- Modify: `MicroServices/FeWoLearning.MicroServices.slnx`

**Interfaces:**
- Consumes: `TierMarker` namespace convention from Task 1.
- Produces: `ExerciseRegistry.Lookup(string id)` → `Action<IDistributedApplicationBuilder>?`, keyed by lowercase exercise id such as `"ex001"`.

- [ ] **Step 1: Create the two minimal services**

`MicroServices/services/Catalog/Catalog.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
</Project>
```

`MicroServices/services/Catalog/Program.cs`:

```csharp
// A deliberately tiny service. Exercises reference it when they need a REAL
// HTTP resource in the model; it is not itself an exercise and gets no catalog row.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("healthy"));
app.MapGet("/products", () => new[]
{
    new { Id = 1, Name = "Keyboard", Price = 79.90m },
    new { Id = 2, Name = "Monitor", Price = 329.00m }
});

app.Run();
```

`MicroServices/services/Orders/` is the same shape with `AssemblyName` `Orders`, `/health`, and:

```csharp
app.MapGet("/orders", () => new[] { new { Id = 1001, ProductId = 1, Quantity = 2 } });
```

- [ ] **Step 2: Create the playground AppHost project**

`MicroServices/playground/Playground.AppHost.csproj`:

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.5.3">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <AspireUseCliBundle>true</AspireUseCliBundle>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\services\Catalog\Catalog.csproj" />
    <ProjectReference Include="..\services\Orders\Orders.csproj" />
  </ItemGroup>

  <!-- Always the EXERCISES library: the playground exists so the learner can watch
       their own work run in the dashboard. -->
  <ItemGroup>
    <ProjectReference Include="..\exercises\FeWoLearning.MicroServices.Exercises.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 3: Write the exercise registry**

`MicroServices/playground/ExerciseRegistry.cs`:

```csharp
using Aspire.Hosting;
using FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Maps an exercise id to its Configure method, so one AppHost can run any exercise
/// instead of the repo needing 100 executable AppHost projects.
/// Add one line per exercise, in the same commit as the exercise.
/// </summary>
public static class ExerciseRegistry
{
    private static readonly Dictionary<string, Action<IDistributedApplicationBuilder>> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        // Registered as exercises land. Task 6 adds ex001-ex005.
    };

    public static Action<IDistributedApplicationBuilder>? Lookup(string id)
        => Map.TryGetValue(id, out var configure) ? configure : null;

    public static IEnumerable<string> Known => Map.Keys.Order();
}
```

- [ ] **Step 4: Write the playground AppHost**

`MicroServices/playground/AppHost.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// `aspire run --apphost MicroServices/playground -- --exercise ex001`
var id = builder.Configuration["exercise"];

if (string.IsNullOrWhiteSpace(id))
{
    Console.Error.WriteLine(
        "Pass an exercise, e.g.: aspire run --apphost MicroServices/playground -- --exercise ex001");
    Console.Error.WriteLine("Known: " + string.Join(", ", ExerciseRegistry.Known));
    return;
}

var configure = ExerciseRegistry.Lookup(id)
    ?? throw new InvalidOperationException(
        $"Unknown exercise '{id}'. Known: {string.Join(", ", ExerciseRegistry.Known)}");

configure(builder);

builder.Build().Run();
```

- [ ] **Step 5: Add the new projects to the solution**

Extend `MicroServices/FeWoLearning.MicroServices.slnx`:

```xml
  <Folder Name="/services/">
    <Project Path="services/Catalog/Catalog.csproj" />
    <Project Path="services/Orders/Orders.csproj" />
  </Folder>
  <Folder Name="/playground/">
    <Project Path="playground/Playground.AppHost.csproj" />
  </Folder>
```

- [ ] **Step 6: Verify everything builds**

Run: `cd MicroServices && dotnet build`
Expected: build succeeded, 0 errors.

- [ ] **Step 7: Verify the playground refuses an unknown exercise**

Run: `cd MicroServices && dotnet run --project playground -- --exercise nope`
Expected: exits with `Unknown exercise 'nope'`. It must **not** hang — if it starts the dashboard instead, the `--exercise` argument is not reaching `builder.Configuration` and the argument parsing needs fixing before Task 6.

- [ ] **Step 8: Confirm the test suite is unaffected**

Run: `cd MicroServices && dotnet test`
Expected: still **4 passed, 1 skipped**.

- [ ] **Step 9: Commit**

```bash
git add MicroServices/services MicroServices/playground MicroServices/FeWoLearning.MicroServices.slnx
git commit -m "MicroServices: shared services and playground AppHost"
```

---

### Task 4: DevContainer, verified for real

**Files:**
- Create: `MicroServices/.devcontainer/devcontainer.json`

**Interfaces:**
- Consumes: the track building under Task 3.
- Produces: nothing other tasks depend on.

- [ ] **Step 1: Write the devcontainer definition**

`MicroServices/.devcontainer/devcontainer.json`:

```jsonc
{
  "name": "FeWoLearning MicroServices",
  "image": "mcr.microsoft.com/devcontainers/dotnet:1-10.0",

  // docker-OUTSIDE-of-docker, not docker-in-docker: Aspire starts sibling
  // containers on the host daemon. Nesting them breaks Aspire's port handling.
  "features": {
    "ghcr.io/devcontainers/features/docker-outside-of-docker:1": {},
    "ghcr.io/devcontainers/features/node:1": {}
  },

  // 18888 is the Aspire dashboard.
  "forwardPorts": [18888],
  "portsAttributes": {
    "18888": { "label": "Aspire dashboard", "onAutoForward": "notify" }
  },

  "postCreateCommand": "npm install -g @microsoft/aspire-cli && dotnet restore",

  "customizations": {
    "vscode": {
      "extensions": ["ms-dotnettools.csdevkit"]
    }
  }
}
```

- [ ] **Step 2: Validate the definition parses**

Run: `devcontainer read-configuration --workspace-folder MicroServices`
Expected: JSON configuration echoed, no error.

- [ ] **Step 3: Build and start the container for real**

Run: `devcontainer up --workspace-folder MicroServices`
Expected: `outcome: success` with a container id. This is the step that stops the DevContainer shipping on faith.

- [ ] **Step 4: Prove the toolchain works inside it**

Run: `devcontainer exec --workspace-folder MicroServices dotnet --version`
Expected: `10.x`.

Run: `devcontainer exec --workspace-folder MicroServices docker ps`
Expected: a container listing (proves docker-outside-of-docker reaches the host daemon). If this errors with a socket-permission failure, that is the known rough edge of the feature — fix it here, do not defer it, because every L3 exercise depends on it.

- [ ] **Step 5: Prove the track's tests run inside it**

Run: `devcontainer exec --workspace-folder MicroServices dotnet test`
Expected: **4 passed, 1 skipped** — the same result as on the host.

- [ ] **Step 6: Tear the container down**

Run: `docker ps -a --filter "label=devcontainer.local_folder" --format "{{.ID}}" | xargs -r docker rm -f`
Expected: the devcontainer is removed. Leave the machine as it was found.

- [ ] **Step 7: Commit**

```bash
git add MicroServices/.devcontainer
git commit -m "MicroServices: verified DevContainer with docker-outside-of-docker"
```

If any of steps 3–5 could not be made to pass, **do not** silently commit a broken DevContainer: commit it with a `KNOWN-UNVERIFIED` note at the top of the file and record the failure in `MicroServices/README.md`, the way `java/` and `kotlin/` are recorded.

---

### Task 5: Catalog and README

**Files:**
- Create: `MicroServices/catalog.md`
- Create: `MicroServices/README.md`

**Interfaces:**
- Consumes: the tier plan from spec §6.
- Produces: the work queue every later batch reads.

- [ ] **Step 1: Write the catalog header and all 100 rows**

`MicroServices/catalog.md` starts:

```markdown
# MicroServices — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned ·
🐳 needs a real container (`dotnet test -p:Containers=true`).

"Beginner" means **Aspire and distributed-systems** beginner, not C# beginner: ex001
models a resource graph, not a `FizzBuzz`. Plain C# language drills belong to the
`dotnet/` track.

Weighting, agreed with the track owner: **001–055** Aspire and polyglot persistence,
**056–085** microservice patterns, **086–100** Docker and Azure. Azure is taught
entirely offline — emulators plus generated artifacts, no subscription.

**Status: 0 ✅ / 100 ⬜**

## Beginner (001–035) — Aspire model and first persistence

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | ContainerResourceBasics | `AddPostgres`, `AddDatabase`, resource types in the model | ⬜ |
| 002 | ReferenceVersusWaitFor | `WithReference` injects config, `WaitFor` orders startup | ⬜ |
| 003 | EndpointsAndBindings | `EndpointAnnotation`, target ports, external endpoints | ⬜ |
| 004 | HealthChecksInTheModel | `WithHttpHealthCheck`, `HealthCheckAnnotation` | ⬜ |
| 005 | ParametersAndSecrets | `AddParameter`, secret parameters, generated defaults | ⬜ |
```

Fill rows 006–100 following spec §6.1–6.6, one block heading per tier segment. Each block starts with the same three-column table header. Use these as the shape for each block's opening rows:

```markdown
## Intermediate (036–070) — persistence in depth, then communication

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | EfCoreAgainstSqlServer | `AddSqlServer`, EF Core `DbContext` registration, connection naming | 🐳 ⬜ |
| 037 | EfCoreAgainstPostgres | same model on Npgsql, and where the providers differ | 🐳 ⬜ |
| 038 | MigrationsOnStartup | applying migrations, ordering against `WaitFor` | 🐳 ⬜ |
| …   |
| 056 | ServiceDiscoveryBasics | `WithReference` on a project, `https+http://name` resolution | ⬜ |

## Advanced (071–090) — patterns, then Docker

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | TransactionalOutbox | outbox table, dispatcher, at-least-once delivery | 🐳 ⬜ |
| …   |
| 086 | AddDockerfileResource | `AddDockerfile`, build context, build args | ⬜ |

## Expert (091–100) — Azure and deployment

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | AzuriteInTheModel | `AddAzureStorage` with the emulator, Blob/Queue/Table children | ⬜ |
| …   |
| 095 | TwoComputeEnvironments | assigning resources when compose and ACA both exist | ⬜ |
```

Two rules for filling the rest:

- Mark with 🐳 every row whose subject can only be proven by a real run — EF Core migrations applying, a Mongo aggregation returning documents, an outbox delivering, a Redis key expiring. The estimate is 20–25 rows; the exact set is a judgement call made row by row.
- Before writing a row, ask what a *wrong* implementation would do. If a naive one would satisfy the concept as stated, the row's Concepts cell is too vague to grade — sharpen it now, not when the exercise is written.

- [ ] **Step 2: Verify the catalog has exactly 100 exercise rows**

Run: `grep -cE '^\| [0-9]{3} \|' MicroServices/catalog.md`
Expected: `100`.

- [ ] **Step 3: Write the README**

`MicroServices/README.md` must cover, at minimum:

- setup and the four commands: `dotnet test`, `dotnet test -p:UseSolutions=true`, `dotnet test -p:Containers=true`, `aspire run --apphost playground -- --exercise exNNN`
- the three test levels and what each can and cannot prove
- the pinned versions, and **why** xunit.v3 4.0.0 / MTP `global.json` is forbidden
- the `Aspire.Hosting.Elasticsearch` 13.3.0 version lag
- the DevContainer, with its verification status from Task 4
- the traps measured while building the track: `aspire publish` never exits in a non-interactive shell; two compute environments without assignment fail `validate-compute-environments`; `FactAttribute.Skip` is not virtual; `NU1603` silently upgrades the test runner
- spec §8.2's rule, stated as a rule for whoever adds the next exercise

- [ ] **Step 4: Commit**

```bash
git add MicroServices/catalog.md MicroServices/README.md
git commit -m "MicroServices: 100-row catalog and README"
```

---

### Task 6: Exercises ex001–ex005

**Files:**
- Create: `MicroServices/exercises/01-beginner/Ex001_ContainerResourceBasics.cs` … `Ex005_ParametersAndSecrets.cs`
- Create: `MicroServices/solutions/01-beginner/Ex001_…` … `Ex005_…` (same five names)
- Create: `MicroServices/tests/01-beginner/Ex001_ContainerResourceBasicsTests.cs` … `Ex005_…Tests.cs`
- Modify: `MicroServices/playground/ExerciseRegistry.cs`
- Modify: `MicroServices/catalog.md`

**Interfaces:**
- Consumes: `ModelHarness`, `ManifestHarness` from Task 2; `ExerciseRegistry` from Task 3.
- Produces: the batch template every later batch copies.

- [ ] **Step 1: Write the ex001 stub**

`MicroServices/exercises/01-beginner/Ex001_ContainerResourceBasics.cs`:

```csharp
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a PostgreSQL server with one database on it.
/// Drills: `AddPostgres`, `AddDatabase`, the server/database resource split.
/// Passes: The model contains a PostgresServerResource named "pg" and a
///         PostgresDatabaseResource named "orders" whose connection-string
///         expression composes onto the server's.
/// </summary>
public static class Ex001_ContainerResourceBasics
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: add a Postgres server named 'pg' with a database named 'orders'.");
}
```

- [ ] **Step 2: Write the ex001 test**

`MicroServices/tests/01-beginner/Ex001_ContainerResourceBasicsTests.cs`:

```csharp
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex001_ContainerResourceBasicsTests
{
    [Fact]
    public void Models_a_postgres_server_and_a_database_on_it()
    {
        var model = ModelHarness.Build(Ex001_ContainerResourceBasics.Configure);

        // The TYPE matters, not merely that something called "pg" exists:
        // AddContainer("pg", "postgres") would satisfy a name-only assertion.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
    }

    [Fact]
    public void Database_connection_string_composes_onto_the_server()
    {
        var model = ModelHarness.Build(Ex001_ContainerResourceBasics.Configure);

        Assert.Equal(
            "{pg.connectionString};Database=orders",
            ModelHarness.ConnectionString(model.Resource("orders")));

        Assert.Contains(
            "Username=postgres",
            ModelHarness.ConnectionString(model.Resource("pg")));
    }
}
```

- [ ] **Step 3: Run to verify it fails for the right reason**

Run: `cd MicroServices && dotnet test --filter "FullyQualifiedName~Ex001"`
Expected: FAIL — **2 failed**, both with `NotImplementedException: TODO: add a Postgres server…`.

If the failure is a compile error or a `NullReferenceException`, the stub is wrong. If any test **passes**, the test is wrong — it is not actually grading the TODO.

- [ ] **Step 4: Write the ex001 reference solution**

`MicroServices/solutions/01-beginner/Ex001_ContainerResourceBasics.cs` — same namespace, same type name, same doc comment, working body:

```csharp
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model a PostgreSQL server with one database on it.
/// Drills: `AddPostgres`, `AddDatabase`, the server/database resource split.
/// Passes: The model contains a PostgresServerResource named "pg" and a
///         PostgresDatabaseResource named "orders" whose connection-string
///         expression composes onto the server's.
/// </summary>
public static class Ex001_ContainerResourceBasics
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // AddPostgres models the SERVER (a container). AddDatabase models a logical
        // database ON it - a separate resource whose connection string is composed
        // from the server's, which is why the expression starts {pg.connectionString}.
        builder.AddPostgres("pg")
               .AddDatabase("orders");
    }
}
```

- [ ] **Step 5: Verify the solution turns it green**

Run: `cd MicroServices && dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex001"`
Expected: **2 passed, 0 failed.**

- [ ] **Step 6: Write ex002 — the second worked example**

ex002 carries the trap the whole track turns on, so it is spelled out in full rather than described.

`MicroServices/exercises/01-beginner/Ex002_ReferenceVersusWaitFor.cs`:

```csharp
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Wire a worker to a Postgres database so that it BOTH receives the
///         connection configuration AND starts only after the database is ready.
/// Drills: `WithReference` (injects config) versus `WaitFor` (orders startup) —
///         two different jobs that are easy to confuse for one.
/// Passes: The worker carries a WaitAnnotation for the database AND the manifest
///         shows a ConnectionStrings__orders entry in its environment.
/// </summary>
public static class Ex002_ReferenceVersusWaitFor
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: add Postgres 'pg' with database 'orders', then a container "
            + "'worker' (image busybox) that references AND waits for 'orders'.");
}
```

`MicroServices/tests/01-beginner/Ex002_ReferenceVersusWaitForTests.cs`:

```csharp
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex002_ReferenceVersusWaitForTests
{
    [Fact]
    public void Worker_waits_for_the_database()
    {
        var model = ModelHarness.Build(Ex002_ReferenceVersusWaitFor.Configure);

        // WithReference alone injects configuration but does NOT order startup.
        // Only WaitFor leaves a WaitAnnotation, so this is the fact that separates
        // the two - a solution using only WithReference must fail here.
        Assert.NotEmpty(model.Resource("worker").Annotations.OfType<WaitAnnotation>());
    }

    [Fact]
    public async Task Worker_receives_the_connection_string()
    {
        using var manifest = await ManifestHarness.GenerateAsync(Ex002_ReferenceVersusWaitFor.Configure);

        var env = manifest.RootElement
            .GetProperty("resources").GetProperty("worker").GetProperty("env");

        // Conversely, WaitFor alone orders startup but injects nothing, so this is
        // the fact that a WaitFor-only solution fails.
        Assert.Equal("{orders.connectionString}", env.GetProperty("ConnectionStrings__orders").GetString());
    }
}
```

`MicroServices/solutions/01-beginner/Ex002_ReferenceVersusWaitFor.cs` — same namespace, type name and doc comment, with:

```csharp
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var orders = builder.AddPostgres("pg").AddDatabase("orders");

        // Two separate jobs: WithReference puts ConnectionStrings__orders into the
        // worker's environment; WaitFor holds the worker back until the database
        // reports healthy. Neither implies the other.
        builder.AddContainer("worker", "busybox")
               .WithReference(orders)
               .WaitFor(orders);
    }
```

Red-check `dotnet test --filter "FullyQualifiedName~Ex002"` (expect 2 failed with the TODO), then green-check with `-p:UseSolutions=true` (expect 2 passed).

- [ ] **Step 7: Repeat steps 1–5 for ex003 through ex005**

Same cycle each: stub → test → red-check → solution → green-check. Their subjects, from the catalog:

- **ex003 EndpointsAndBindings** — assert `EndpointAnnotation` on a container, its `TargetPort`, and that an externally-exposed endpoint differs from an internal one.
- **ex004 HealthChecksInTheModel** — assert `HealthCheckAnnotation` is present. A model that merely starts is not evidence; the annotation is.
- **ex005 ParametersAndSecrets** — use `ManifestHarness`, because the generated-secret policy is visible in the manifest (`inputs.value.default.generate`) and not on the model object. Assert `type: "parameter.v0"` and `secret: true`.

Before writing ex005's solution, confirm the parameter API shape:
`aspire docs api search "AddParameter secret" --language csharp`.

- [ ] **Step 8: Register all five in the playground**

In `MicroServices/playground/ExerciseRegistry.cs`, replace the placeholder comment inside `Map`:

```csharp
        ["ex001"] = Ex001_ContainerResourceBasics.Configure,
        ["ex002"] = Ex002_ReferenceVersusWaitFor.Configure,
        ["ex003"] = Ex003_EndpointsAndBindings.Configure,
        ["ex004"] = Ex004_HealthChecksInTheModel.Configure,
        ["ex005"] = Ex005_ParametersAndSecrets.Configure,
```

- [ ] **Step 9: Run the full red check**

Run: `cd MicroServices && dotnet test`
Expected: the five exercises' facts **all fail**, harness facts pass, 1 skipped. Confirm **no exercise fact passes** — a passing stub test is a bug in the test.

- [ ] **Step 10: Run the full green check**

Run: `cd MicroServices && dotnet test -p:UseSolutions=true`
Expected: **everything passes, 0 failed**, 1 skipped.

- [ ] **Step 11: Flip exactly those five catalog rows**

In `MicroServices/catalog.md`, change rows 001–005 from ⬜ to ✅ and update the status line to `**Status: 5 ✅ / 95 ⬜**`. Change nothing else.

- [ ] **Step 12: Commit**

```bash
git add MicroServices/exercises/01-beginner MicroServices/solutions/01-beginner \
        MicroServices/tests/01-beginner MicroServices/playground/ExerciseRegistry.cs \
        MicroServices/catalog.md
git commit -m "MicroServices: ex001–ex005"
```

---

### Task 7: Repo-level documentation

**Files:**
- Modify: `CLAUDE.md`
- Modify: `docs/requirements.md`
- Modify: `docs/exercise-format.md`

**Interfaces:**
- Consumes: everything above.
- Produces: nothing.

- [ ] **Step 1: Add the track to `CLAUDE.md`'s command table**

Add a row to the per-track commands table:

```
| `MicroServices/` | — (restore on first `dotnet test`) | `dotnet test` | `dotnet test --filter FullyQualifiedName~Ex001` |
```

and note that `-p:UseSolutions=true` runs the same suite against the reference solutions, and `-p:Containers=true` additionally runs the container-backed rows.

- [ ] **Step 2: Add the toolchain-status entry**

Record, with the date: Aspire 13.5.3 on .NET 10.0.400, Docker 29.7.2, devcontainer CLI 0.89.0, xunit.v3 3.2.2 pinned on the VSTest path, and the measured red/green counts from Task 6.

- [ ] **Step 3: Add the track-specific gotchas section**

Cover: the capitalised folder name; `solutions/` deliberately in the build; the forbidden xunit.v3 4.0.0 + MTP combination and the `NU1603` forward-resolution trap; `FactAttribute.Skip` not being virtual; `aspire publish` never exiting non-interactively; two compute environments failing `validate-compute-environments`; and spec §8.2's "rendered connection data does not prove the mechanism" rule.

- [ ] **Step 4: Add the track to the current-state table**

```
| `MicroServices/` | 5 / 100 (verified) | 95 |
```

- [ ] **Step 5: Update `docs/requirements.md`**

Add Docker 29.7.2, Aspire CLI 13.4.6, and devcontainer CLI 0.89.0 to the detected-tools table, noting the devcontainer CLI was installed for this track via `npm i -g @devcontainers/cli`.

- [ ] **Step 6: Update `docs/exercise-format.md`**

Add the naming row:

```
| `MicroServices/` | one file per exercise, tier-wide namespace, test in a separate `tests/` project | `exercises/01-beginner/Ex001_ContainerResourceBasics.cs` |
```

and add `MicroServices/` to the list of tracks whose `solutions/` is deliberately inside the build.

- [ ] **Step 7: Commit**

```bash
git add CLAUDE.md docs/requirements.md docs/exercise-format.md
git commit -m "MicroServices: record the track in the repo docs"
```

---

## After this plan

Exercises 006–100 follow the repo's standing batch procedure in `CLAUDE.md`: read `catalog.md` for the next five ⬜ rows, read one finished exercise from the same tier as a style template, write stub + test + solution for each, red-check filtered to the five, green-check with `-p:UseSolutions=true`, register each in `ExerciseRegistry`, flip exactly those five rows, commit as `MicroServices: exNNN–exNNN`.

Three standing additions specific to this track:

1. When a batch needs a new Aspire integration package, add it to **both** content libraries in the same commit, pinned to 13.5.3 (Elasticsearch: 13.3.0).
2. Run the full suite once per completed tier, in both modes, and record the counts in `MicroServices/README.md`.
3. **Rows 086–090 (Docker Compose output) need a golden file.** The compose YAML is not obtainable in-process — every publisher argument combination yields the manifest instead — and the `aspire publish` CLI never exits in a non-interactive shell. So generate `docker-compose.yaml` once at authoring time with the CLI, kill it after the file appears, check the file in beside the exercise, and have the test assert that the learner's model produces a **manifest consistent with that committed golden file**. Never re-run the CLI from a test.
