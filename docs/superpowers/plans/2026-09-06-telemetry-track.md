# Telemetry Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Stand up the `telemetry/` track — three projects, a process-global-safe test harness, a 70-row catalog, and the first batch of exercises verified red and green — so that every later batch is mechanical.

**Architecture:** One project trio (`exercises/`, `solutions/`, `tests/`) on `net10.0-windows` with `UseWPF`. `exercises/` and `solutions/` compile the same type names into the same namespaces; `tests/` references exactly one of them via the `UseSolutions` MSBuild property, so `dotnet test` is the red run and `dotnet test -p:UseSolutions=true` the green one. Every exercise owns a uniquely named `ActivitySource`/`Meter`, and the harness resets the diagnostics process-globals per test.

**Tech Stack:** .NET 10.0.400 · OpenTelemetry 1.18.0 · xunit.v3 3.2.2 on the classic VSTest path · `Xunit.StaFact` 3.0.13 · `Microsoft.Extensions.Diagnostics.Testing` 10.9.0 · Testcontainers 4.14.0

**Spec:** `docs/superpowers/specs/2026-09-06-telemetry-track-design.md`

## Global Constraints

Every task's requirements implicitly include this section.

- **Run every command from inside `telemetry/`**, never the repo root.
- **TFM `net10.0-windows`, `<UseWPF>true</UseWPF>` on all three projects.** The track is Windows-only; that is deliberate (spec §3).
- **`FrameworkReference Include="Microsoft.AspNetCore.App"`** on `exercises/` and `solutions/` only.
- **Never `PackageReference`** `Microsoft.Extensions.Hosting`, `.Logging`, `.DependencyInjection`, `.Configuration`, `.Options`, or `System.Security.Cryptography.ProtectedData` — all are in the shared framework for this TFM and referencing them emits `NU1510`.
- **No `global.json` in `telemetry/`.** xunit.v3 4.0.0 plus a `Microsoft.Testing.Platform` `global.json` makes `dotnet test` exit 5 with zero tests discovered on this machine.
- **Package versions, all measured by a real restore on 2026-09-06 — do not "update" them casually:**
  - `OpenTelemetry`, `.Extensions.Hosting`, `.Instrumentation.AspNetCore`, `.Instrumentation.Http`, `.Instrumentation.Runtime`, `.Exporter.OpenTelemetryProtocol`, `.Exporter.Console`, `.Exporter.InMemory` → **1.18.0**
  - `OpenTelemetry.Exporter.Prometheus.AspNetCore` → **1.18.0-beta.1** (this package has **no stable release**; prerelease is the only option and `dotnet package search` without `--prerelease` returns nothing for it)
  - `Serilog.Extensions.Logging` **10.0.0**, `Serilog.Sinks.File` **7.0.0**, `Polly.Core` **8.7.0**
  - `xunit.v3` **3.2.2**, `xunit.runner.visualstudio` **3.1.5**, `Microsoft.NET.Test.Sdk` **17.14.1**, `Xunit.StaFact` **3.0.13**
  - `Microsoft.Extensions.Diagnostics.Testing` **10.9.0**, `Microsoft.AspNetCore.TestHost` **10.0.11**, `Testcontainers` **4.14.0**
- **`xunit.runner.visualstudio` has no 3.1.6 or 3.1.7.** Naming one resolves *forward* to 4.0.0 with only an `NU1603` warning, silently landing on the broken generation.
- **`Xunit.StaFact` must stay on 3.x.** 4.x requires xunit.v3 4.0.0, which requires the `global.json` this track must not have.
- **Warnings are findings.** `exercises/` may emit `CS0169`/`CS0414`/`CS0649` from stub fields, left unsuppressed. `solutions/` must build with **0 warnings**. `tests/` suppresses `xUnit1051` only.
- **Block `03-otel-sdk`'s namespace is `FeWoLearning.Telemetry.Exercises.Otel`**, never `.OpenTelemetry` — see spec §2 for why. Never write a fully qualified `OpenTelemetry.…` type reference in any track file; use `using` directives.
- **Stubs throw `NotImplementedException("TODO: ExNNN - …")`** and must still compile.

---

## File Structure

| File | Responsibility |
|---|---|
| `telemetry/FeWoLearning.Telemetry.slnx` | the three projects |
| `telemetry/Directory.Build.props` | redirects the solutions build's output (CS0579 guard) |
| `telemetry/exercises/FeWoLearning.Telemetry.Exercises.csproj` | stub content library |
| `telemetry/solutions/FeWoLearning.Telemetry.Solutions.csproj` | reference content library, identical type names |
| `telemetry/tests/FeWoLearning.Telemetry.Tests.csproj` | the one test project, referencing exactly one content library |
| `telemetry/tests/_harness/AssemblyInfo.cs` | serialises the whole suite |
| `telemetry/tests/_harness/TelemetryContext.cs` | per-test reset of diagnostics process-globals |
| `telemetry/tests/_harness/TraceProbe.cs` | an `ActivityListener` scoped to one source name |
| `telemetry/tests/_harness/LogProbe.cs` | `FakeLogger` wiring + structured-field accessors |
| `telemetry/tests/_harness/MetricProbe.cs` | manual-collect `MeterProvider` + in-memory metric exporter |
| `telemetry/tests/_harness/ContainerGate.cs` | `-p:Containers=true` gate |
| `telemetry/tests/_harness/HarnessSmokeTests.cs` | canaries, green in both modes |
| `telemetry/catalog.md` | the 70-row work queue |
| `telemetry/README.md` | setup, commands, and "How a telemetry test lies" |
| `telemetry/{exercises,solutions}/<block>/ExNNN_<Slug>.cs` | one stub / one reference implementation |
| `telemetry/tests/<block>/ExNNN_<Slug>Tests.cs` | that exercise's facts |

---

### Task 1: Project trio that builds and discovers tests

**Files:**
- Create: `telemetry/Directory.Build.props`
- Create: `telemetry/FeWoLearning.Telemetry.slnx`
- Create: `telemetry/exercises/FeWoLearning.Telemetry.Exercises.csproj`
- Create: `telemetry/solutions/FeWoLearning.Telemetry.Solutions.csproj`
- Create: `telemetry/tests/FeWoLearning.Telemetry.Tests.csproj`
- Create: `telemetry/.gitignore`
- Test: `telemetry/tests/_harness/HarnessSmokeTests.cs` (first canary only)

**Interfaces:**
- Consumes: nothing.
- Produces: the `UseSolutions` and `Containers` MSBuild properties; the runtime key `FeWoLearning.Telemetry.Containers` readable via `AppContext.GetData`.

- [ ] **Step 1: Write the failing canary**

`telemetry/tests/_harness/HarnessSmokeTests.cs`:

```csharp
namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Canaries. These must pass in BOTH modes (`dotnet test` and
/// `dotnet test -p:UseSolutions=true`) and are the first thing to fail when a
/// package bump breaks the harness. They are never TODOs and never get a
/// catalog.md row.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void The_test_project_references_exactly_one_content_library()
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .Where(n => n is "FeWoLearning.Telemetry.Exercises" or "FeWoLearning.Telemetry.Solutions")
            .ToArray();

        // Zero is also a failure: it means the content library was never loaded,
        // which is how a "green" run can be green for the wrong reason.
        Assert.Single(loaded);
    }
}
```

- [ ] **Step 2: Run it to confirm it fails**

Run from `telemetry/`: `dotnet test`
Expected: the build fails — no project files exist yet.

- [ ] **Step 3: Create `telemetry/Directory.Build.props`**

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

- [ ] **Step 4: Create `telemetry/exercises/FeWoLearning.Telemetry.Exercises.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Telemetry.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Telemetry.Exercises</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- Brings Microsoft.Extensions.Logging / DependencyInjection / Configuration /
       Options / Hosting and the ASP.NET Core request pipeline. Do NOT also
       PackageReference any of those - they are already in this shared framework
       and NU1510 follows. -->
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <!-- Versions measured by a real restore on 2026-09-06. The Prometheus exporter
       has NO stable release - 1.18.0-beta.1 is the newest thing that exists, and
       `dotnet package search` without --prerelease returns nothing for it. -->
  <ItemGroup>
    <PackageReference Include="OpenTelemetry" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.18.0" />
    <PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.18.0-beta.1" />
    <PackageReference Include="Serilog.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
    <PackageReference Include="Polly.Core" Version="8.7.0" />
  </ItemGroup>

</Project>
```

- [ ] **Step 5: Create `telemetry/solutions/FeWoLearning.Telemetry.Solutions.csproj`**

Byte-identical to Step 4 except these two lines:

```xml
    <RootNamespace>FeWoLearning.Telemetry.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Telemetry.Solutions</AssemblyName>
```

The `RootNamespace` stays `…Exercises` on purpose: solutions compile the *same* namespaces so `tests/` can reference either one without changing a single `using`.

- [ ] **Step 6: Create `telemetry/tests/FeWoLearning.Telemetry.Tests.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <RootNamespace>FeWoLearning.Telemetry.Tests</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <!-- xUnit1051 (pass TestContext.Current.CancellationToken) only - the same single
         suppression caliburn/, security/ and Architecture/ carry. Any OTHER warning
         in this project is a finding, not noise. -->
    <NoWarn>$(NoWarn);xUnit1051</NoWarn>
    <!-- No OutputType here: xunit.v3 sets it to Exe through its own build props. -->
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <!-- xunit.v3 3.2.2 on the classic VSTest path, and NO global.json.
       xunit.v3 4.0.0 + a Microsoft.Testing.Platform global.json makes
       `dotnet test` exit 5 with zero tests discovered on the .NET 10.0.400 SDK.
       runner.visualstudio has no 3.1.6/3.1.7 - 3.1.5 is the last 3.x, and naming
       a nonexistent one resolves FORWARD to 4.0.0 with only an NU1603 warning.
       Xunit.StaFact stays on 3.x for the same reason: 4.x requires xunit.v3 4.0.0. -->
  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="Xunit.StaFact" Version="3.0.13" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <!-- Grading instruments. These belong here and never in a content library:
       an exercise must not be able to see the thing that reads it. -->
  <ItemGroup>
    <PackageReference Include="OpenTelemetry.Exporter.InMemory" Version="1.18.0" />
    <PackageReference Include="Microsoft.Extensions.Diagnostics.Testing" Version="10.9.0" />
    <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="10.0.11" />
  </ItemGroup>

  <!-- Container-backed facts only. Every one of them is behind
       ContainerGate.SkipUnlessEnabled(), so this package is restored always and
       exercised only under -p:Containers=true. -->
  <ItemGroup>
    <PackageReference Include="Testcontainers" Version="4.14.0" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one content library, never both: that is what keeps the identical
       namespaces and type names from colliding. `dotnet test` is the red run,
       `dotnet test -p:UseSolutions=true` the green one. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Telemetry.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Telemetry.Solutions.csproj" />
  </ItemGroup>

  <!-- `-p:Containers=true` reaches the test process through runtimeconfig.json;
       an MSBuild property is otherwise invisible at runtime. -->
  <ItemGroup>
    <RuntimeHostConfigurationOption Include="FeWoLearning.Telemetry.Containers"
                                    Value="$(Containers)"
                                    Condition="'$(Containers)' != ''" />
  </ItemGroup>

</Project>
```

- [ ] **Step 7: Create `telemetry/FeWoLearning.Telemetry.slnx`**

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Telemetry.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Telemetry.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Telemetry.Tests.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 8: Create `telemetry/.gitignore`**

```gitignore
bin/
obj/
artifacts-solutions/
```

- [ ] **Step 9: Give each content library one placeholder file so it compiles**

`telemetry/exercises/_support/Placeholder.cs` and `telemetry/solutions/_support/Placeholder.cs`, identical:

```csharp
namespace FeWoLearning.Telemetry.Exercises.Support;

/// <summary>
/// Keeps the content library non-empty before the first exercise lands. Delete this
/// file as soon as `_support/` holds a real shared fixture.
/// </summary>
internal static class Placeholder
{
    internal const string TrackName = "telemetry";
}
```

- [ ] **Step 10: Run both modes and confirm the canary passes in each**

```bash
dotnet test
dotnet test -p:UseSolutions=true
```

Expected, both times: `Fehler: 0, erfolgreich: 1, gesamt: 1`. If either reports **0 tests discovered**, the `global.json`/runner pin has drifted — re-read Global Constraints before doing anything else.

- [ ] **Step 11: Confirm zero warnings**

```bash
dotnet build --no-incremental
dotnet build --no-incremental -p:UseSolutions=true
```

Expected: `0 Warnung(en)` on both. A `NU1510` here means a shared-framework package was referenced; a `NU1603` means a package version does not exist and NuGet resolved forward.

- [ ] **Step 12: Commit**

```bash
git add telemetry/.gitignore telemetry/Directory.Build.props telemetry/FeWoLearning.Telemetry.slnx telemetry/exercises telemetry/solutions telemetry/tests
git commit -m "telemetry: track scaffolding"
```

---

### Task 2: The harness

**Files:**
- Create: `telemetry/tests/_harness/AssemblyInfo.cs`
- Create: `telemetry/tests/_harness/TelemetryContext.cs`
- Create: `telemetry/tests/_harness/TraceProbe.cs`
- Create: `telemetry/tests/_harness/LogProbe.cs`
- Create: `telemetry/tests/_harness/MetricProbe.cs`
- Create: `telemetry/tests/_harness/ContainerGate.cs`
- Modify: `telemetry/tests/_harness/HarnessSmokeTests.cs`

**Interfaces:**
- Consumes: the project trio from Task 1.
- Produces, all in namespace `FeWoLearning.Telemetry.Tests.Harness`:
  - `sealed class TelemetryContext : IDisposable` — `new TelemetryContext()` resets; `Dispose()` resets again.
  - `sealed class TraceProbe : IDisposable` — `TraceProbe(string sourceName)`; `IReadOnlyList<Activity> Stopped { get; }`; `Activity Single()`.
  - `sealed class LogProbe : IDisposable` — `LogProbe()`; `ILogger<T> For<T>()`; `ILogger For(string category)`; `IReadOnlyList<FakeLogRecord> Records { get; }`; `static string? Field(FakeLogRecord record, string name)`.
  - `sealed class MetricProbe : IDisposable` — `MetricProbe(string meterName)`; `IReadOnlyList<Metric> Collect()`.
  - `static class ContainerGate` — `static void SkipUnlessEnabled()`.

- [ ] **Step 1: Write the failing harness canaries**

Replace `telemetry/tests/_harness/HarnessSmokeTests.cs` with:

```csharp
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Canaries. These must pass in BOTH modes (`dotnet test` and
/// `dotnet test -p:UseSolutions=true`) and are the first thing to fail when a
/// package bump breaks the harness. They are never TODOs and never get a
/// catalog.md row.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void The_test_project_references_exactly_one_content_library()
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .Where(n => n is "FeWoLearning.Telemetry.Exercises" or "FeWoLearning.Telemetry.Solutions")
            .ToArray();

        // Zero is also a failure: it means the content library was never loaded,
        // which is how a "green" run can be green for the wrong reason.
        Assert.Single(loaded);
    }

    [Fact]
    public void TraceProbe_sees_only_its_own_source()
    {
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe("harness.smoke.mine");
        using var mine = new ActivitySource("harness.smoke.mine");
        using var other = new ActivitySource("harness.smoke.other");

        using (mine.StartActivity("kept")) { }
        using (other.StartActivity("ignored")) { }

        Assert.Equal("kept", probe.Single().DisplayName);
    }

    [Fact]
    public void TelemetryContext_clears_the_ambient_activity()
    {
        using var source = new ActivitySource("harness.smoke.ambient");
        using var probe = new TraceProbe("harness.smoke.ambient");
        var leaked = source.StartActivity("leaked");
        Assert.NotNull(Activity.Current);

        using var ctx = new TelemetryContext();

        Assert.Null(Activity.Current);
        leaked?.Dispose();
    }

    [Fact]
    public void LogProbe_exposes_named_fields_not_just_the_message()
    {
        using var logs = new LogProbe();

        logs.For("harness").LogInformation("order {OrderId} shipped", "O-7");

        var record = Assert.Single(logs.Records);
        Assert.Equal("O-7", LogProbe.Field(record, "OrderId"));
    }

    [Fact]
    public void MetricProbe_collects_a_counter_from_its_own_meter()
    {
        using var probe = new MetricProbe("harness.smoke.meter");
        using var meter = new System.Diagnostics.Metrics.Meter("harness.smoke.meter");
        meter.CreateCounter<long>("hits").Add(3);

        var metric = Assert.Single(probe.Collect());
        Assert.Equal("hits", metric.Name);
    }
}
```

- [ ] **Step 2: Run it to confirm it fails**

Run from `telemetry/`: `dotnet test`
Expected: compile errors — `TelemetryContext`, `TraceProbe`, `LogProbe` and `MetricProbe` do not exist.

- [ ] **Step 3: Create `telemetry/tests/_harness/AssemblyInfo.cs`**

```csharp
// The whole suite runs serially, and this is not optional.
//
// Every ActivityListener sees activities from EVERY test running concurrently, and
// Sdk.SetDefaultTextMapPropagator, Activity.DefaultIdFormat and Activity.Current are
// process-wide. A parallel run does not error - it silently produces cross-test
// contamination, and (per avalonia/'s 2026-09-05 finding) can truncate the run while
// still printing a normal-looking summary. Read the test COUNT, not just the word
// "Failed".
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```

- [ ] **Step 4: Create `telemetry/tests/_harness/TelemetryContext.cs`**

```csharp
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Resets the diagnostics process-globals around one test. Construct it first in any
/// test that touches Activity, Baggage or a propagator; the reset runs on both
/// construction and disposal, so a test cannot inherit a neighbour's leak nor leave
/// one behind.
/// </summary>
public sealed class TelemetryContext : IDisposable
{
    private static readonly ActivityIdFormat PristineIdFormat;
    private static readonly bool PristineForceIdFormat;
    private static readonly TextMapPropagator PristinePropagator;

    // An EXPLICIT static constructor, never field initializers. A field initializer
    // leaves the type `beforefieldinit`, which lets the runtime defer initialization
    // until the first read of that field - and that read happens AFTER the instance
    // constructor's reset, so the snapshot captures the already-reset values.
    // Measured on this machine while building caliburn/'s CaliburnCoreContext.
    static TelemetryContext()
    {
        PristineIdFormat = Activity.DefaultIdFormat;
        PristineForceIdFormat = Activity.ForceDefaultIdFormat;
        PristinePropagator = Propagators.DefaultTextMapPropagator;
    }

    public TelemetryContext() => Reset();

    public void Dispose() => Reset();

    private static void Reset()
    {
        Activity.Current = null;
        Activity.DefaultIdFormat = PristineIdFormat;
        Activity.ForceDefaultIdFormat = PristineForceIdFormat;
        Sdk.SetDefaultTextMapPropagator(PristinePropagator);
        Baggage.Current = default;
    }
}
```

- [ ] **Step 5: Create `telemetry/tests/_harness/TraceProbe.cs`**

```csharp
using System.Diagnostics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Collects completed activities from EXACTLY ONE ActivitySource name. Scoping by
/// name is the second line of defence behind the serial run: even if a reset is
/// missed, a probe cannot see another exercise's spans.
/// </summary>
public sealed class TraceProbe : IDisposable
{
    private readonly List<Activity> _stopped = [];
    private readonly ActivityListener _listener;

    public TraceProbe(string sourceName)
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == sourceName,
            // AllDataAndRecorded, not AllData: without the Recorded flag the
            // activity is created but IsAllDataRequested/Recorded is false, and any
            // implementation that guards its tagging on Recorded silently emits
            // nothing - the test would then fail for the wrong reason.
            Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => { lock (_stopped) _stopped.Add(activity); },
        };

        ActivitySource.AddActivityListener(_listener);
    }

    public IReadOnlyList<Activity> Stopped
    {
        get { lock (_stopped) return _stopped.ToArray(); }
    }

    /// <summary>The single completed activity, asserting there is exactly one.</summary>
    public Activity Single() => Assert.Single(Stopped);

    public void Dispose() => _listener.Dispose();
}
```

- [ ] **Step 6: Create `telemetry/tests/_harness/LogProbe.cs`**

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A FakeLogger factory whose records expose STRUCTURED state, not just the rendered
/// message. That distinction is the whole point of block 01: an interpolated string
/// and a message template produce byte-identical text and completely different state.
/// </summary>
public sealed class LogProbe : IDisposable
{
    private readonly FakeLogCollector _collector = new();
    private readonly ILoggerFactory _factory;

    public LogProbe()
    {
        _factory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(new FakeLoggerProvider(_collector));
        });
    }

    public ILogger<T> For<T>() => _factory.CreateLogger<T>();

    public ILogger For(string category) => _factory.CreateLogger(category);

    public IReadOnlyList<FakeLogRecord> Records => _collector.GetSnapshot();

    /// <summary>
    /// The value of one named field, or null when the record carries no such field.
    /// A record produced by an interpolated string carries NO named fields at all -
    /// so a null return is the signal that grades interpolation as wrong.
    /// </summary>
    public static string? Field(FakeLogRecord record, string name) =>
        record.StructuredState?.FirstOrDefault(kv => kv.Key == name).Value;

    /// <summary>
    /// The constant template behind a record. Identical across calls with different
    /// argument values when - and only when - a message template was used.
    /// </summary>
    public static string? OriginalFormat(FakeLogRecord record) =>
        Field(record, "{OriginalFormat}");

    public void Dispose() => _factory.Dispose();
}
```

- [ ] **Step 7: Create `telemetry/tests/_harness/MetricProbe.cs`**

```csharp
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A manual-collect MeterProvider over EXACTLY ONE meter name. Collect() flushes and
/// returns everything gathered since the probe was created.
///
/// Collect() twice when the subject is aggregation: a single collection cannot tell
/// Delta from Cumulative, cannot show a counter is monotonic, and cannot catch double
/// counting from a leaked listener.
/// </summary>
public sealed class MetricProbe : IDisposable
{
    private readonly List<Metric> _exported = [];
    private readonly MeterProvider _provider;

    public MetricProbe(string meterName)
    {
        _provider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(meterName)
            .AddInMemoryExporter(_exported)
            .Build();
    }

    public IReadOnlyList<Metric> Collect()
    {
        _provider.ForceFlush();
        return _exported.ToArray();
    }

    public void Dispose() => _provider.Dispose();
}
```

- [ ] **Step 8: Create `telemetry/tests/_harness/ContainerGate.cs`**

```csharp
namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Gates the container-backed facts. Call it as the FIRST statement of any fact that
/// needs Docker; everything after the call is skipped unless the run passed
/// `-p:Containers=true`.
///
/// It is a call and not a custom [ContainerFact] attribute because FactAttribute.Skip
/// is not virtual in xunit.v3 3.2.2 - overriding it fails with CS0506.
///
/// The MSBuild property reaches the test process through a
/// RuntimeHostConfigurationOption in the .csproj; an MSBuild property is otherwise
/// invisible at runtime.
/// </summary>
public static class ContainerGate
{
    private const string Key = "FeWoLearning.Telemetry.Containers";

    public static bool Enabled =>
        bool.TryParse(AppContext.GetData(Key) as string, out var on) && on;

    public static void SkipUnlessEnabled() =>
        Assert.SkipUnless(Enabled, "Container-backed fact. Re-run with -p:Containers=true.");
}
```

- [ ] **Step 9: Run the canaries in both modes**

```bash
dotnet test
dotnet test -p:UseSolutions=true
```

Expected, both times: `Fehler: 0, erfolgreich: 5, gesamt: 5`.

- [ ] **Step 10: Prove the container gate actually gates**

Add this canary to `HarnessSmokeTests.cs` and re-run:

```csharp
    [Fact]
    public void ContainerGate_skips_by_default_and_runs_under_the_flag()
    {
        ContainerGate.SkipUnlessEnabled();

        // Only reached under -p:Containers=true. If this line ever runs on a plain
        // `dotnet test`, the gate is broken and every container fact is silently
        // executing (or silently passing) in the default run.
        Assert.True(ContainerGate.Enabled);
    }
```

```bash
dotnet test
dotnet test -p:Containers=true
```

Expected: `erfolgreich: 5, übersprungen: 1, gesamt: 6` on the first, `erfolgreich: 6, übersprungen: 0` on the second. **If the first run reports 6 passed and 0 skipped, the gate is not wired** — check the `RuntimeHostConfigurationOption` block.

- [ ] **Step 11: Commit**

```bash
git add telemetry/tests/_harness
git commit -m "telemetry: test harness"
```

---

### Task 3: Catalog and README

**Files:**
- Create: `telemetry/catalog.md`
- Create: `telemetry/README.md`
- Modify: `CLAUDE.md`
- Modify: `README.md`

**Interfaces:**
- Consumes: nothing at runtime.
- Produces: `telemetry/catalog.md` as the work queue every later batch reads. Row numbering, slugs and Concepts are copied **verbatim** from spec §4 — the Slug column determines file names (`ExNNN_<Slug>.cs`), so a typo here becomes a typo in 3 files per row.

- [ ] **Step 1: Write `telemetry/catalog.md`**

Header, then the five block tables exactly as spec §4 lists them, every Status cell `⬜`:

```markdown
# Telemetry (C#) — Exercise Catalog (70)

Blocks: **logging** 001–014 · **diagnostics** 015–026 · **otel-sdk** 027–044 ·
**web-services** 045–058 · **desktop-ops** 059–070.

Legend: ✅ seeded (stub + test + solution present, red and green both verified) ·
⬜ planned · 🐳 carries extra container-backed facts, skipped unless `-p:Containers=true`.

This track uses five subject-area blocks rather than the repo's usual 100-row /
four-difficulty-tier scheme, for the same reason `security/` and `Architecture/` do:
"beginner telemetry" is not a meaningful axis. A histogram bucket boundary is not
conceptually harder than a log scope; they are different concerns. Difficulty rises
*within* each block. See
`docs/superpowers/specs/2026-09-06-telemetry-track-design.md` §4.

Stubs live in `exercises/<block>/ExNNN_<Slug>.cs`, their xUnit tests in
`tests/<block>/ExNNN_<Slug>Tests.cs`, and reference implementations in
`solutions/<block>/` at the same relative path.

**Status: 0 ✅ / 70 ⬜**
```

Then, for each of the five blocks, a `## <block> (NNN–NNN) — <subtitle>` heading and a
table with columns `| # | Slug | Concepts | Status |`, filled from spec §4 verbatim.

- [ ] **Step 2: Write `telemetry/README.md`**

It must contain, at minimum:

1. **Setup and commands** — the four-row command table from spec §3, and the
   Windows-only statement.
2. **"How a telemetry test lies"** — all six items from spec §5, in full prose, with
   the two mandatory mitigations.
3. **The verification recipe** — reject-everything variant, then the *plausible wrong*
   one, with this track's four typical plausible-wrong shapes: "logs, but
   interpolates", "creates a span, but as a root instead of a child", "counts, but
   with unbounded tag cardinality", "registers the exporter, but never flushes".
4. **Measured facts** — a section seeded with what Task 1 and Task 2 established:
   - `FakeLogger`'s record accessor is `logger.Collector.LatestRecord`; there is no
     `logger.Latest`.
   - `[WpfFact]` from `Xunit.StaFact` 3.0.13 supplies an STA thread and a live
     `Dispatcher.CurrentDispatcher`, and needed no interactive desktop session on this
     machine — measured 2026-09-06, unlike `caliburn/`, which does.
   - `OpenTelemetry.Exporter.Prometheus.AspNetCore` has no stable release.
   - `TraceProbe` samples `AllDataAndRecorded`, not `AllData`; with `AllData` alone an
     implementation guarding on `Activity.Recorded` emits nothing and the test fails
     for the wrong reason.
   - `FakeLogger` captures scopes with **no** `IncludeScopes` opt-in, and
     `FakeLogRecord.Scopes` holds the **raw** scope objects rather than flattened
     key/value pairs — a dictionary scope arrives as the dictionary.

- [ ] **Step 3: Add the track to the repo-level tables**

In `CLAUDE.md`: add a `telemetry/` row to the per-track command table, to the
"Current state" table (`0 / 70`, remaining 70), and a `**Telemetry**` bullet to
"Track-specific gotchas" carrying the Global Constraints of this plan in condensed
form. In the repo `README.md`: add the track to whatever track list it holds.

- [ ] **Step 4: Verify the catalog is well-formed**

```bash
grep -c '^| [0-9]' telemetry/catalog.md
```

Expected: `70`. Then confirm numbering has no gaps or repeats:

```bash
grep -o '^| [0-9]\{3\}' telemetry/catalog.md | grep -o '[0-9]\{3\}' | sort | uniq -d
```

Expected: no output.

- [ ] **Step 5: Commit**

```bash
git add telemetry/catalog.md telemetry/README.md CLAUDE.md README.md
git commit -m "telemetry: 70-row catalog and README"
```

---

### Task 4: Ex001 alone — proving the red/green mechanism

Ex001 ships by itself, before the rest of its batch, because it is the first time the
whole loop runs end to end: a stub that compiles and throws, facts that go red on that
throw, a reference implementation that turns them green, and a plausible-wrong
implementation that must **not** turn them green.

**Files:**
- Create: `telemetry/exercises/01-logging/Ex001_StructuredMessageTemplate.cs`
- Create: `telemetry/solutions/01-logging/Ex001_StructuredMessageTemplate.cs`
- Test: `telemetry/tests/01-logging/Ex001_StructuredMessageTemplateTests.cs`
- Modify: `telemetry/catalog.md` (row 001 ⬜ → ✅, status line)

**Interfaces:**
- Consumes: `LogProbe` from Task 2 — `For(string)`, `Records`, `Field(record, name)`, `OriginalFormat(record)`.
- Produces: `FeWoLearning.Telemetry.Exercises.Logging.Ex001_StructuredMessageTemplate`, with
  `public static void LogPaymentFailed(ILogger logger, string orderId, decimal amount, string reason)`.

- [ ] **Step 1: Write the stub**

`telemetry/exercises/01-logging/Ex001_StructuredMessageTemplate.cs`:

```csharp
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.Logging;

// Exercise 001 — StructuredMessageTemplate (logging).
// Goal:   Log one payment failure so that a machine can query it, not only a human
//         read it.
// Drills: message templates vs string interpolation, named placeholders, the
//         {OriginalFormat} entry every template leaves behind.
// Passes: the record carries the named fields OrderId, Amount and Reason, whose
//         values are the arguments;
//         the rendered message reads exactly
//                     "Payment for order O-42 of 19.99 failed: insufficient funds";
//         two calls with DIFFERENT arguments leave the SAME {OriginalFormat} value;
//         and that value contains the literal text "{OrderId}".
//
// The last two clauses are the ones that matter. $"Payment for order {orderId} …"
// renders identical text and carries no named fields at all - so the log is
// unqueryable, and every call site invents its own new "template". Anything that
// aggregates, alerts on, or filters these logs works on the fields and the constant
// template, never on the sentence.
public static class Ex001_StructuredMessageTemplate
{
    /// <summary>
    /// Write ONE Information-level record describing a failed payment.
    ///
    /// The rendered message must read
    /// "Payment for order {OrderId} of {Amount} failed: {Reason}" with the three
    /// values substituted, and the record must carry those three names as fields.
    /// </summary>
    public static void LogPaymentFailed(ILogger logger, string orderId, decimal amount, string reason) =>
        throw new NotImplementedException(
            "TODO: Ex001 - log the failure with a message template, not an interpolated string");
}
```

- [ ] **Step 2: Write the facts**

`telemetry/tests/01-logging/Ex001_StructuredMessageTemplateTests.cs`:

```csharp
using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex001_StructuredMessageTemplateTests
{
    [Fact]
    public void The_record_carries_the_arguments_as_named_fields()
    {
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        var record = Assert.Single(logs.Records);
        Assert.Equal("O-42", LogProbe.Field(record, "OrderId"));
        Assert.Equal("19.99", LogProbe.Field(record, "Amount"));
        Assert.Equal("insufficient funds", LogProbe.Field(record, "Reason"));
    }

    [Fact]
    public void The_rendered_message_still_reads_naturally()
    {
        // The paired "use" fact. Without it, a solution could satisfy the field
        // assertions with a template nobody can read.
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        Assert.Equal(
            "Payment for order O-42 of 19.99 failed: insufficient funds",
            Assert.Single(logs.Records).Message);
    }

    [Fact]
    public void Adversarial_A_Two_calls_share_one_constant_template()
    {
        // THE fact that separates a template from interpolation. Interpolation bakes
        // the values into the format string, so two calls with different arguments
        // produce two different {OriginalFormat} values - and a logging backend then
        // sees two unrelated event types instead of one event with two instances.
        using var logs = new LogProbe();
        var logger = logs.For("payments");

        Ex001_StructuredMessageTemplate.LogPaymentFailed(logger, "O-42", 19.99m, "insufficient funds");
        Ex001_StructuredMessageTemplate.LogPaymentFailed(logger, "O-43", 5.00m, "card expired");

        Assert.Equal(2, logs.Records.Count);
        Assert.Equal(
            LogProbe.OriginalFormat(logs.Records[0]),
            LogProbe.OriginalFormat(logs.Records[1]));
    }

    [Fact]
    public void Adversarial_B_The_template_uses_names_not_positions()
    {
        // "{0} {1} {2}" would satisfy Adversarial_A perfectly well and leave the
        // fields called "0", "1" and "2" - queryable by nothing.
        using var logs = new LogProbe();

        Ex001_StructuredMessageTemplate.LogPaymentFailed(
            logs.For("payments"), "O-42", 19.99m, "insufficient funds");

        var template = LogProbe.OriginalFormat(Assert.Single(logs.Records));
        Assert.NotNull(template);
        Assert.Contains("{OrderId}", template);
        Assert.Contains("{Amount}", template);
        Assert.Contains("{Reason}", template);
    }
}
```

- [ ] **Step 3: Red check**

```bash
dotnet test --filter FullyQualifiedName~Ex001_
```

Expected: `Fehler: 4, erfolgreich: 0, gesamt: 4`, and **every** failure message contains
`TODO: Ex001`. A failure with any other cause means the stub does not compile or a
fact is wrong — fix it before continuing; a stub that fails to build is a bug.

- [ ] **Step 4: Write the reference implementation**

`telemetry/solutions/01-logging/Ex001_StructuredMessageTemplate.cs` — same header
comment as the stub, then:

```csharp
    public static void LogPaymentFailed(ILogger logger, string orderId, decimal amount, string reason) =>
        logger.LogInformation(
            "Payment for order {OrderId} of {Amount} failed: {Reason}",
            orderId, amount, reason);
```

- [ ] **Step 5: Green check**

```bash
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_
```

Expected: `Fehler: 0, erfolgreich: 4, gesamt: 4`.

- [ ] **Step 6: Probe the plausible-wrong implementation**

Temporarily replace the solution body with the interpolated version:

```csharp
    public static void LogPaymentFailed(ILogger logger, string orderId, decimal amount, string reason) =>
        logger.LogInformation($"Payment for order {orderId} of {amount} failed: {reason}");
```

```bash
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~Ex001_
```

Expected: the two `Adversarial_` facts and
`The_record_carries_the_arguments_as_named_fields` fail; only
`The_rendered_message_still_reads_naturally` passes. **If all four pass, the exercise
grades nothing** — tighten the facts before continuing. Then restore Step 4's body and
re-run Step 5.

- [ ] **Step 7: Flip the catalog row**

In `telemetry/catalog.md`, change row `001`'s Status cell from `⬜` to `✅` and the
status line to `**Status: 1 ✅ / 69 ⬜**`. Watch the padding: some catalogs in this repo
pad the cell (`⬜     |`) and some do not — match whatever the neighbouring rows do.

- [ ] **Step 8: Commit**

```bash
git add telemetry/exercises/01-logging telemetry/solutions/01-logging telemetry/tests/01-logging telemetry/catalog.md
git commit -m "telemetry: ex001"
```

---

### Task 5: Ex002–Ex005

**Files:**
- Create: `telemetry/{exercises,solutions}/01-logging/Ex002_LogLevelsAndFiltering.cs`
- Create: `telemetry/{exercises,solutions}/01-logging/Ex003_CategoriesAndTypedLogger.cs`
- Create: `telemetry/{exercises,solutions}/01-logging/Ex004_LoggingScopes.cs`
- Create: `telemetry/{exercises,solutions}/01-logging/Ex005_LoggerMessageSourceGenerator.cs`
- Test: `telemetry/tests/01-logging/Ex00{2,3,4,5}_*Tests.cs`
- Modify: `telemetry/catalog.md`

**Interfaces:**
- Consumes: `LogProbe` (Task 2) and Ex001's established style (Task 4).
- Produces: four public static classes in `FeWoLearning.Telemetry.Exercises.Logging`, each named for its file.

The Concepts column of catalog rows 002–005 is the spec for what each drills.

Ex004 needs scope state, and two things about it are already measured (2026-09-06, on
`Microsoft.Extensions.Diagnostics.Testing` 10.9.0) — build the exercise on these
rather than re-deriving them:

- **`FakeLogger` captures scopes with no `IncludeScopes` opt-in.** After
  `logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = "acme" })`, the
  record's `Scopes` has `Count == 1`. There is no builder flag to set.
- **`FakeLogRecord.Scopes` is `IReadOnlyList<object?>` holding the *raw* scope
  objects, not flattened key/value pairs.** Passing a dictionary puts that dictionary
  in the list; the fact must reach into it. A fact written as though scopes arrive
  pre-flattened into named fields will fail against a correct implementation.

Extend `LogProbe` with the accessor as part of this task rather than inventing a
second probe:

```csharp
    /// <summary>
    /// The raw scope objects active when the record was written, outermost first.
    /// Measured: FakeLogger captures these with no IncludeScopes opt-in, and does NOT
    /// flatten them - a dictionary scope arrives as the dictionary.
    /// </summary>
    public static IReadOnlyList<object?> Scopes(FakeLogRecord record) => record.Scopes;
```

- [ ] **Step 1: For each of the four, write stub + facts, then red-check the batch**

```bash
dotnet test --filter "FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"
```

Expected: every fact red, zero passed, and every failure message naming its own
`TODO: ExNNN`.

- [ ] **Step 2: Write the four reference implementations, then green-check**

```bash
dotnet test -p:UseSolutions=true --filter "FullyQualifiedName~Ex002_|FullyQualifiedName~Ex003_|FullyQualifiedName~Ex004_|FullyQualifiedName~Ex005_"
```

Expected: all facts pass, 0 failed.

- [ ] **Step 3: Probe each with a plausible-wrong implementation**

Per exercise, the shape to try: 002 — a logger that ignores `IsEnabled` and writes
anyway; 003 — a hard-coded category string instead of `ILogger<T>`; 004 — properties
written on every call site instead of a scope; 005 — a hand-written
`logger.LogInformation` instead of the `[LoggerMessage]` partial. Each must leave at
least one fact red. Restore the real solutions afterwards and re-run Step 2.

- [ ] **Step 4: Full-suite sanity run in both modes**

```bash
dotnet test
dotnet test -p:UseSolutions=true
```

Read the **test count**, not just the word `Failed`. Expected red run: 6 harness facts
passed (1 skipped without `-p:Containers=true`) and every exercise fact failed.
Expected green run: everything passed, 1 skipped.

- [ ] **Step 5: Flip rows 002–005 and commit**

```bash
git add telemetry/exercises/01-logging telemetry/solutions/01-logging telemetry/tests/01-logging telemetry/catalog.md
git commit -m "telemetry: ex002-ex005"
```

---

### Task 6 and beyond: the repeating batch procedure

Every remaining batch of five follows CLAUDE.md's "Adding or completing exercises",
with these track-specific additions. This is the whole procedure; there is nothing
further to design.

1. Read `telemetry/catalog.md`. **The next five ⬜ rows are the assignment** — their
   Slug and Concepts columns are the spec. Do not re-inventory the disk.
2. Read **one** already-finished exercise from the same block as a style template —
   once per block, not once per batch.
3. Write stub, facts and reference implementation for each. Give every exercise a
   **uniquely named** `ActivitySource`/`Meter` — `"fewolearning.telemetry.exNNN"` — so
   a `TraceProbe`/`MetricProbe` cannot see a neighbour's data.
4. Red check filtered to the five. Every failure must trace to its own
   `TODO: ExNNN`, and no fact may pass.
5. Green check: `dotnet test -p:UseSolutions=true` filtered to the same five. There is
   no overlay step in this track — the `UseSolutions` property is the mechanism.
6. **Probe the reject-everything variant, then the plausible-wrong one** (spec §5). A
   batch whose facts survive the plausible-wrong implementation is under-grading and
   does not ship.
7. For a 🐳 row, additionally run `dotnet test -p:Containers=true` filtered to it, and
   confirm the same fact **skips** without the flag.
8. Flip exactly those five rows ⬜ → ✅ and update the `**Status:**` line.
9. Record any newly measured behaviour in `telemetry/README.md` under "Measured
   facts". A surprise found and not written down is a surprise the next batch pays for
   again.
10. Commit as `telemetry: exNNN-exNNN`, staging explicit paths — `git add -A` has
    swept up unrelated files in this repo before.

Run the full suite in both modes **once per completed block**, and record the exact
counts in `telemetry/README.md`.

Block boundaries worth planning around, because each introduces a new grading
instrument rather than more of the same:

| Block | First row | What it introduces |
|---|---|---|
| `02-diagnostics` | 015 | `TraceProbe`, and `MeterListener` used directly |
| `03-otel-sdk` | 027 | `MetricProbe`, `InMemoryExporter<Activity>`, and the `.Otel` namespace rule |
| `04-web-services` | 045 | `WebProbe` — a `TestServer` fixture that does not yet exist; build it in the same task as row 045 |
| `05-desktop-ops` | 059 | `[WpfFact]`, STA and `Dispatcher` |
