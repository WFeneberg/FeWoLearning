# Architecture Track Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build `Architecture/`, a 60-exercise graded track teaching application and system architecture in C# on .NET 10, across web, desktop, services+data and cross-cutting concerns.

**Architecture:** Three projects (`exercises/`, `solutions/`, `tests/`) sharing the repo's `UseSolutions` MSBuild mechanism, so `dotnet test` is the red run and `dotnet test -p:UseSolutions=true` the green one, and reference solutions are compile-checked on every green run. Everything targets `net10.0` and runs headless. Infrastructure comes in three tiers: in-memory fakes by default, real-but-in-process SQLite and a real in-process MQTTnet broker for the rows a fake cannot honestly grade, and opt-in Testcontainers behind `-p:Containers=true` for eight rows.

**Tech Stack:** .NET 10.0.400, xunit.v3 3.2.2 on the classic VSTest path (no `global.json`), `Microsoft.Data.Sqlite`, `MQTTnet` 5.x, `Polly.Core`, `Testcontainers.*` (tests only).

**Spec:** [`docs/superpowers/specs/2026-09-06-architecture-track-design.md`](../specs/2026-09-06-architecture-track-design.md)

## Global Constraints

- Folder is `Architecture/` — **capitalised**, deliberately, like `MicroServices/`. Do not "fix" it.
- All three projects target **`net10.0`**. No `net10.0-windows`, no `UseWPF`, no Windows-only API anywhere. The track must run headless.
- Test stack pinned to **xunit.v3 3.2.2**, **`xunit.runner.visualstudio` 3.1.5**, **`Microsoft.NET.Test.Sdk` 17.14.1**, and **no `global.json` in `Architecture/`**. xunit.v3 4.0.0 plus a `Microsoft.Testing.Platform` `global.json` makes `dotnet test` exit 5 with zero tests discovered on this machine. `xunit.runner.visualstudio` has no 3.1.6/3.1.7 — naming one resolves *forward* to 4.0.0 with only an `NU1603` warning.
- `SQLitePCLRaw.lib.e_sqlite3` must be pinned to **2.1.13 or later**; the version `Microsoft.Data.Sqlite` drags in transitively carries GHSA-2m69-gcr7-jv3q and emits `NU1903`.
- Do **not** reference `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Logging` or `Microsoft.Extensions.Configuration` from a project that has `<FrameworkReference Include="Microsoft.AspNetCore.App" />` — they are already in that shared framework and referencing them emits `NU1510`.
- Namespaces are pinned **per block**, not per folder: `FeWoLearning.Architecture.Exercises.Web` / `.Desktop` / `.ServicesData` / `.CrossCutting`. Tests mirror as `FeWoLearning.Architecture.Tests.<Block>`.
- `exercises/` and `solutions/` compile the **same type names into the same namespaces**. `tests/` references exactly one of them. Never both.
- `solutions/` must build with **zero warnings**. A warning there is a finding. `exercises/` may emit `CS0169`/`CS0414`/`CS0649` from stub fields the learner is meant to wire up; those stay unsuppressed.
- Every stub throws `NotImplementedException` at runtime and **compiles** while unfinished.
- Every exercise must satisfy the track's grading rule (spec §6): assert the **mechanism's own side effect**, not just the outcome, and carry at least one **adversarial fact** where a naive implementation demonstrably diverges.
- Commit messages use the form `Architecture: exNNN–exNNN` (en dash). Stage explicit paths — never `git add -A`.

---

### Task 1: Scaffolding — solution, three projects, output redirection

**Files:**
- Create: `Architecture/FeWoLearning.Architecture.slnx`
- Create: `Architecture/Directory.Build.props`
- Create: `Architecture/exercises/FeWoLearning.Architecture.Exercises.csproj`
- Create: `Architecture/solutions/FeWoLearning.Architecture.Solutions.csproj`
- Create: `Architecture/tests/FeWoLearning.Architecture.Tests.csproj`
- Create: `Architecture/.gitignore`

**Interfaces:**
- Consumes: nothing.
- Produces: a solution that builds and a test project that discovers tests; the `UseSolutions` and `Containers` MSBuild properties that every later task depends on.

- [ ] **Step 1: Create the directory tree**

```bash
cd /c/Tools/FeWoLearning
mkdir -p Architecture/exercises/01-web Architecture/exercises/02-desktop \
         Architecture/exercises/03-services-data Architecture/exercises/04-cross-cutting \
         Architecture/exercises/_support \
         Architecture/solutions/01-web Architecture/solutions/02-desktop \
         Architecture/solutions/03-services-data Architecture/solutions/04-cross-cutting \
         Architecture/solutions/_support \
         Architecture/tests/01-web Architecture/tests/02-desktop \
         Architecture/tests/03-services-data Architecture/tests/04-cross-cutting \
         Architecture/tests/_harness
```

- [ ] **Step 2: Write `Architecture/Directory.Build.props`**

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

- [ ] **Step 3: Write `Architecture/FeWoLearning.Architecture.slnx`**

```xml
<Solution>
  <Folder Name="/exercises/">
    <Project Path="exercises/FeWoLearning.Architecture.Exercises.csproj" />
  </Folder>
  <Folder Name="/solutions/">
    <Project Path="solutions/FeWoLearning.Architecture.Solutions.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/FeWoLearning.Architecture.Tests.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 4: Write `Architecture/exercises/FeWoLearning.Architecture.Exercises.csproj`**

`RootNamespace` is the same in both content libraries — that is the whole point. Only `AssemblyName` differs.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Architecture.Exercises</RootNamespace>
    <AssemblyName>FeWoLearning.Architecture.Exercises</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <!-- Brings Microsoft.Extensions.DependencyInjection / Logging / Configuration /
       Options and the ASP.NET Core request pipeline. Do NOT also PackageReference
       any of those - they are in this shared framework and NU1510 follows. -->
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Data.Sqlite" Version="$(SqliteVersion)" />
    <!-- Pinned above Microsoft.Data.Sqlite's own transitive 2.1.11, which carries
         GHSA-2m69-gcr7-jv3q and emits NU1903 on every build. The bundle package
         cannot fix this: bundle and lib versions are decoupled. -->
    <PackageReference Include="SQLitePCLRaw.lib.e_sqlite3" Version="2.1.13" />
    <PackageReference Include="MQTTnet" Version="$(MqttVersion)" />
    <PackageReference Include="Polly.Core" Version="$(PollyVersion)" />
  </ItemGroup>

</Project>
```

Replace `$(SqliteVersion)`, `$(MqttVersion)`, `$(PollyVersion)` with literal versions in Step 6.

- [ ] **Step 5: Write `Architecture/solutions/FeWoLearning.Architecture.Solutions.csproj`**

Identical to Step 4 except `AssemblyName`:

```xml
    <AssemblyName>FeWoLearning.Architecture.Solutions</AssemblyName>
```

- [ ] **Step 6: Resolve and pin exact package versions**

```bash
cd /c/Tools/FeWoLearning/Architecture
for p in Microsoft.Data.Sqlite MQTTnet Polly.Core Testcontainers.PostgreSql \
         Testcontainers.Redis Testcontainers.RabbitMq; do
  echo "=== $p"
  dotnet package search "$p" --take 1 --format json | python -c "import sys,json; d=json.load(sys.stdin); [print(pkg['id'], pkg['latestVersion']) for s in d['searchResult'] for pkg in s['packages']]" 2>/dev/null | head -3
done
```

Write the literal versions into both content `.csproj` files and the test `.csproj`. Do not invent a version — a nonexistent patch resolves forward with only `NU1603`.

- [ ] **Step 7: Write `Architecture/tests/FeWoLearning.Architecture.Tests.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>FeWoLearning.Architecture.Tests</RootNamespace>
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
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  </ItemGroup>

  <!-- Container-gated rows only. Skipped unless -p:Containers=true. -->
  <ItemGroup>
    <PackageReference Include="Testcontainers.PostgreSql" Version="$(TcVersion)" />
    <PackageReference Include="Testcontainers.Redis" Version="$(TcVersion)" />
    <PackageReference Include="Testcontainers.RabbitMq" Version="$(TcVersion)" />
    <PackageReference Include="Npgsql" Version="$(NpgsqlVersion)" />
    <PackageReference Include="StackExchange.Redis" Version="$(RedisVersion)" />
    <PackageReference Include="RabbitMQ.Client" Version="$(RabbitVersion)" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <!-- Exactly one content library, never both: that is what keeps the identical
       namespaces and type names from colliding. `dotnet test` is the red run,
       `dotnet test -p:UseSolutions=true` the green one. -->
  <ItemGroup Condition="'$(UseSolutions)' != 'true'">
    <ProjectReference Include="..\exercises\FeWoLearning.Architecture.Exercises.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(UseSolutions)' == 'true'">
    <ProjectReference Include="..\solutions\FeWoLearning.Architecture.Solutions.csproj" />
  </ItemGroup>

  <!-- `-p:Containers=true` reaches the test process through runtimeconfig.json;
       an MSBuild property is otherwise invisible at runtime. -->
  <ItemGroup>
    <RuntimeHostConfigurationOption Include="FeWoLearning.Architecture.Containers"
                                    Value="$(Containers)"
                                    Condition="'$(Containers)' != ''" />
  </ItemGroup>

</Project>
```

- [ ] **Step 8: Write `Architecture/.gitignore`**

```
bin/
obj/
artifacts-solutions/
TestResults/
```

- [ ] **Step 9: Verify both configurations build**

```bash
cd /c/Tools/FeWoLearning/Architecture
dotnet build 2>&1 | tail -5
dotnet build -p:UseSolutions=true 2>&1 | tail -5
```

Expected: both succeed, `0 Error(s)`. If `CS0579` appears, `Directory.Build.props` is wrong or missing.

- [ ] **Step 10: Commit**

```bash
cd /c/Tools/FeWoLearning
git add Architecture/FeWoLearning.Architecture.slnx Architecture/Directory.Build.props \
        Architecture/.gitignore Architecture/exercises/*.csproj \
        Architecture/solutions/*.csproj Architecture/tests/*.csproj
git commit -m "Architecture: track scaffolding"
```

---

### Task 2: Harness, shared support, and the smoke tests that prove it

**Files:**
- Create: `Architecture/exercises/_support/Clock.cs` and the identical `Architecture/solutions/_support/Clock.cs`
- Create: `Architecture/tests/_harness/ContainerGate.cs`
- Create: `Architecture/tests/_harness/SqliteScratch.cs`
- Create: `Architecture/tests/_harness/MqttBrokerFixture.cs`
- Create: `Architecture/tests/_harness/HarnessSmokeTests.cs`

**Interfaces:**
- Produces, for every later task:
  - `FeWoLearning.Architecture.Exercises.Support.IClock` with `DateTimeOffset UtcNow { get; }`
  - `FeWoLearning.Architecture.Exercises.Support.ManualClock : IClock` with `ManualClock(DateTimeOffset start)`, `void Advance(TimeSpan by)`
  - `FeWoLearning.Architecture.Tests.Harness.ContainerGate.SkipUnlessEnabled()` — call as the first line of a container fact
  - `FeWoLearning.Architecture.Tests.Harness.SqliteScratch : IDisposable` with `string ConnectionString { get; }`, backed by a temp file (not `:memory:`, so a second connection sees the same database)
  - `FeWoLearning.Architecture.Tests.Harness.MqttBrokerFixture : IAsyncLifetime` with `int Port { get; }` and `Task<IMqttClient> ConnectClientAsync(string clientId, Action<MqttClientOptionsBuilder>? configure = null)`

- [ ] **Step 1: Write `_support/Clock.cs` into both content libraries**

```csharp
namespace FeWoLearning.Architecture.Exercises.Support;

/// <summary>The port every time-dependent exercise depends on, so no test sleeps.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

/// <summary>A clock the test drives by hand. Shared fixture - never a TODO.</summary>
public sealed class ManualClock(DateTimeOffset start) : IClock
{
    public DateTimeOffset UtcNow { get; private set; } = start;

    public void Advance(TimeSpan by) => UtcNow = UtcNow.Add(by);
}
```

Copy the identical file to `solutions/_support/Clock.cs`. It must be byte-identical; a divergence here breaks the green run in a way that looks like an exercise bug.

- [ ] **Step 2: Write `tests/_harness/ContainerGate.cs`**

```csharp
namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// Gates the eight container-backed rows. FactAttribute.Skip is not virtual in
/// xunit.v3 3.2.2, so the idiomatic custom [ContainerFact] overriding it fails
/// CS0506 - the gate has to be a call in the test body instead. The MSBuild
/// property reaches the test process through runtimeconfig.json.
/// </summary>
public static class ContainerGate
{
    public static bool Enabled { get; } =
        Environment.GetEnvironmentVariable("FEWO_ARCH_CONTAINERS") == "1"
        || AppContext.GetData("FeWoLearning.Architecture.Containers") as string == "true";

    public static void SkipUnlessEnabled() =>
        Assert.SkipUnless(Enabled,
            "Container tests are off. Enable with: dotnet test -p:Containers=true");
}
```

- [ ] **Step 3: Write `tests/_harness/SqliteScratch.cs`**

A temp **file** database, not `:memory:` — several exercises open a second connection to prove a transaction boundary, and each `:memory:` connection gets its own private database.

```csharp
using Microsoft.Data.Sqlite;

namespace FeWoLearning.Architecture.Tests.Harness;

public sealed class SqliteScratch : IDisposable
{
    private readonly string _path;

    public SqliteScratch()
    {
        _path = Path.Combine(Path.GetTempPath(),
            "fewo-arch-" + Guid.NewGuid().ToString("N") + ".db");
        ConnectionString = new SqliteConnectionStringBuilder { DataSource = _path }.ToString();
    }

    public string ConnectionString { get; }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(_path)) File.Delete(_path);
    }
}
```

- [ ] **Step 4: Write `tests/_harness/MqttBrokerFixture.cs`**

MQTTnet 5 ships the server in the main package. Bind to `IPAddress.Loopback` on port 0 is not available, so pick a free port explicitly.

```csharp
using System.Net;
using System.Net.Sockets;
using MQTTnet;
using MQTTnet.Server;

namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// A real MQTT broker in this process. Real protocol frames, real QoS 1
/// redelivery, real retained messages and last-will delivery, with no container.
/// </summary>
public sealed class MqttBrokerFixture : IAsyncLifetime
{
    private MqttServer? _server;

    public int Port { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Port = FreePort();
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Loopback)
            .WithDefaultEndpointPort(Port)
            .Build();

        _server = new MqttServerFactory().CreateMqttServer(options);
        await _server.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_server is not null)
        {
            await _server.StopAsync();
            _server.Dispose();
        }
    }

    private static int FreePort()
    {
        using var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }
}
```

- [ ] **Step 5: Write `tests/_harness/HarnessSmokeTests.cs`**

These are the only facts in the track that pass in **both** modes. They exist so that a broken harness fails loudly and first, instead of showing up as sixty confusing exercise failures.

```csharp
using FeWoLearning.Architecture.Exercises.Support;
using Microsoft.Data.Sqlite;
using MQTTnet;

namespace FeWoLearning.Architecture.Tests.Harness;

public class HarnessSmokeTests
{
    [Fact]
    public void ManualClock_Advances_Only_When_Told_To()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var clock = new ManualClock(start);

        Assert.Equal(start, clock.UtcNow);
        clock.Advance(TimeSpan.FromMinutes(5));
        Assert.Equal(start.AddMinutes(5), clock.UtcNow);
    }

    [Fact]
    public void SqliteScratch_Is_A_File_Database_Two_Connections_Share()
    {
        using var scratch = new SqliteScratch();

        using (var writer = scratch.OpenConnection())
        {
            var create = writer.CreateCommand();
            create.CommandText = "CREATE TABLE t (v TEXT); INSERT INTO t VALUES ('x');";
            create.ExecuteNonQuery();
        }

        using var reader = scratch.OpenConnection();
        var read = reader.CreateCommand();
        read.CommandText = "SELECT v FROM t";
        Assert.Equal("x", read.ExecuteScalar());
    }

    [Fact]
    public async Task MqttBroker_Accepts_A_Client_And_Round_Trips_A_Message()
    {
        var fixture = new MqttBrokerFixture();
        await fixture.InitializeAsync();
        try
        {
            var factory = new MqttClientFactory();
            using var subscriber = factory.CreateMqttClient();
            using var publisher = factory.CreateMqttClient();

            var received = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            subscriber.ApplicationMessageReceivedAsync += e =>
            {
                received.TrySetResult(e.ApplicationMessage.ConvertPayloadToString() ?? "");
                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", fixture.Port).Build();
            await subscriber.ConnectAsync(options);
            await publisher.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", fixture.Port).WithClientId("pub").Build());

            await subscriber.SubscribeAsync("smoke/#");
            await publisher.PublishStringAsync("smoke/one", "hello");

            var payload = await received.Task.WaitAsync(TimeSpan.FromSeconds(10));
            Assert.Equal("hello", payload);
        }
        finally
        {
            await fixture.DisposeAsync();
        }
    }

    [Fact]
    public void ContainerGate_Is_Off_Unless_Explicitly_Enabled()
    {
        // Documents the default. With -p:Containers=true this asserts the opposite,
        // which is why it reads the gate rather than hard-coding false.
        Assert.Equal(
            Environment.GetEnvironmentVariable("FEWO_ARCH_CONTAINERS") == "1"
            || AppContext.GetData("FeWoLearning.Architecture.Containers") as string == "true",
            ContainerGate.Enabled);
    }
}
```

- [ ] **Step 6: Run the smoke tests in both modes**

```bash
cd /c/Tools/FeWoLearning/Architecture
dotnet test --filter FullyQualifiedName~HarnessSmokeTests 2>&1 | tail -5
dotnet test -p:UseSolutions=true --filter FullyQualifiedName~HarnessSmokeTests 2>&1 | tail -5
```

Expected: 4 passed, 0 failed, **both times**. If the MQTT fact hangs, the API surface of MQTTnet 5.x differs from the code above — check `MqttClientFactory` vs `MqttFactory` and fix before proceeding; every MQTT row depends on this.

- [ ] **Step 7: Commit**

```bash
cd /c/Tools/FeWoLearning
git add Architecture/exercises/_support Architecture/solutions/_support Architecture/tests/_harness
git commit -m "Architecture: harness - manual clock, sqlite scratch, in-process MQTT broker"
```

---

### Task 3: `catalog.md` and `README.md`

**Files:**
- Create: `Architecture/catalog.md`
- Create: `Architecture/README.md`

**Interfaces:**
- Produces: the 60-row work queue every batch task reads. Batch tasks flip rows here and nowhere else.

- [ ] **Step 1: Write `Architecture/catalog.md`**

Header, then the four block tables **copied verbatim from spec §4** with a `Status` column appended, every row `⬜`. Header text:

```markdown
# Architecture (C#) — Exercise Catalog (60)

Blocks: **web** 001–016 · **desktop** 017–028 · **services-data** 029–052 ·
**cross-cutting** 053–060.

Legend: ✅ seeded (stub + test + solution present, red and green both verified) ·
⬜ planned · 🐳 carries extra container-backed facts, skipped unless `-p:Containers=true`.

This track uses four attack-surface-style blocks rather than the repo's usual
100-row / four-difficulty-tier scheme, for the same reason `security/` does:
"beginner architecture" is not a meaningful axis. Difficulty rises *within* each
block. See `docs/superpowers/specs/2026-09-06-architecture-track-design.md` §4.

Stubs live in `exercises/<block>/ExNNN_<Slug>.cs`, their xUnit tests in
`tests/<block>/ExNNN_<Slug>Tests.cs`, and reference implementations in
`solutions/<block>/` at the same relative path.

**Status: 0 ✅ / 60 ⬜**
```

The status cell is written `| ⬜ |` with single spaces — no padding. Record that choice here so batch tasks edit consistently.

- [ ] **Step 2: Write `Architecture/README.md`**

Must cover, each as its own section: what the track is and how it differs from `MicroServices/`; that it is headless `net10.0` and why `02-desktop` is UI-framework-free; the command table (red / green / containers / single exercise); the three infrastructure tiers; the pinned toolchain with the xunit.v3-4.0.0 trap spelled out; and a **"How an architecture test lies"** section reproducing spec §6 in full, since that is the section a future batch author must read before writing a fact.

- [ ] **Step 3: Verify the catalog has exactly 60 rows**

```bash
cd /c/Tools/FeWoLearning/Architecture
grep -cE '^\| [0-9]{3} \|' catalog.md
```

Expected: `60`.

- [ ] **Step 4: Commit**

```bash
cd /c/Tools/FeWoLearning
git add Architecture/catalog.md Architecture/README.md
git commit -m "Architecture: 60-row catalog and README"
```

---

## Batch tasks 4–15: the sixty exercises

Tasks 4 through 15 are the twelve batches. **Every batch task has the identical
step sequence**, given once here; the per-batch sections below give only what
differs — the five rows, the mechanism each row's facts must prove, and the
adversarial fact each row needs.

**The step sequence, for each batch:**

- [ ] **Step A: Read the five rows in `catalog.md`.** Their Slug and Concepts columns are the spec. Do not re-inventory the disk.
- [ ] **Step B: Read one already-finished exercise from the same block** as a style template. Once per block, not once per batch.
- [ ] **Step C: For each of the five, write three files** — `exercises/<block>/ExNNN_<Slug>.cs` (stub with a `Goal:` / `Drills:` / `Passes:` header comment, throwing `NotImplementedException`), `tests/<block>/ExNNN_<Slug>Tests.cs`, `solutions/<block>/ExNNN_<Slug>.cs`. The stub and the solution declare the **same type in the same namespace**.
- [ ] **Step D: Red check, filtered to the five.**

```bash
cd /c/Tools/FeWoLearning/Architecture
dotnet test --filter "FullyQualifiedName~Ex0NN_|FullyQualifiedName~Ex0NM_|..." 2>&1 | tail -15
```

Confirm **no test passes**, and that each failure is the stub's `NotImplementedException` — not a compile error, not a missing import. A stub that fails to build is a bug. Where a fact must go red on an assertion instead (a metadata/reflection row), say so at the fact in a comment.
- [ ] **Step E: Green check.**

```bash
dotnet test -p:UseSolutions=true --filter "<same filter>" 2>&1 | tail -15
```

Expected: all five exercises' facts pass, 0 failed. The build must emit **0 warnings**; a warning from `solutions/` is a finding, fix it.
- [ ] **Step F: The degenerate probe.** For each of the five, temporarily replace the solution body with the laziest thing that could pass — a constant, a no-op, a direct call that skips the pattern. Re-run the green filter. **Facts must fail.** Revert.
- [ ] **Step G: The plausible-wrong probe.** For each row whose subject is a *mechanism* (every row in `03-services-data`, plus 002, 004, 008, 015, 026, 048, 053, 054), temporarily replace the solution with an earnest implementation using the **wrong mechanism** named in that row's section below. Re-run. **Facts must fail.** Revert. This is the probe that catches under-grading the degenerate probe cannot.
- [ ] **Step H: Flip exactly those five `catalog.md` rows** ⬜ → ✅ and update the `**Status:**` line.
- [ ] **Step I: Commit.**

```bash
cd /c/Tools/FeWoLearning
git add Architecture/exercises/<block>/ExNNN_*.cs Architecture/tests/<block>/ExNNN_*Tests.cs \
        Architecture/solutions/<block>/ExNNN_*.cs Architecture/catalog.md
git commit -m "Architecture: exNNN–exNNN"
```

---

### Task 4 — Batch 001–005 (`01-web`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 001 LayeredCompositionRoot | dependency **direction**, read from assembly metadata via reflection — no behavioural fact can prove the domain does not reference infrastructure | a domain type that references an infrastructure type must make the fitness fact fail | domain calling infrastructure through an interface it *defines itself* must pass; through one infrastructure defines must fail |
| 002 ServiceLifetimes | resolved instance identity across and within scopes, and detection of a singleton capturing a scoped dependency | resolving the same scoped service twice in *different* scopes must yield different instances — one scope proves nothing | registering the captive dependency as transient must still be reported as captive |
| 003 OptionsPattern | that `IOptionsSnapshot` re-reads per scope while `IOptions` does not, and that validation runs on first access | change the underlying source *after* first resolution; `IOptions` must still return the old value | eager validation at registration must fail the fact that asserts the failure surfaces on access |
| 004 MiddlewarePipeline | ordering **and** short-circuit: a terminal middleware must prevent later ones running, and the unwind order must be observed | a middleware that short-circuits must leave the downstream marker absent *and* the upstream unwind marker present | reversing registration order must fail |
| 005 VerticalSliceEndpoint | that the slice owns its own request/response types and handler, with no shared service layer between slices | changing one slice's response shape must not require touching the other slice's types | a shared cross-slice service must fail the isolation fact |

Note for 001: this is the first row and it defines the reflection idiom rows 026, 058 and 060 reuse. Put the reusable assertion in the exercise's own file, not in `_harness` — 060's whole subject is writing that assertion.

---

### Task 5 — Batch 006–010 (`01-web`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 006 CqrsCommandQuery | that a command returns no data and a query performs no write — asserted on the store, not on the return type alone | a query handler that mutates must fail | a command handler returning the created entity must fail the "commands do not return data" fact |
| 007 MediatorDispatch | resolution **by request type**, so an unregistered request type fails and a second handler for the same type is detected | dispatching a request with no handler must throw a specific, named error, not `NullReferenceException` | a mediator holding a `Dictionary<string, object>` keyed by type *name* must fail on two same-named types in different namespaces |
| 008 PipelineBehaviors | that behaviours wrap the handler in registration order, observed on both the pre and post side | a behaviour that does not call `next` must prevent the handler running | behaviours running *before* the handler but not *around* it must fail the unwind-order fact |
| 009 ValidationBehavior | that validation runs **before** the handler, proven by the handler's invocation count staying zero | an invalid request must leave handler invocations at 0 — asserting only the error message is satisfied by validating inside the handler | validating inside the handler must fail |
| 010 ResultErrorModel | that failures travel as values, proven by the absence of a thrown exception and by exhaustive mapping | a domain failure must not throw; assert with `Record.Exception(...) is null` | throwing and catching internally, returning a failure Result, must fail a fact asserting no exception was ever constructed (use a counting factory) |

---

### Task 6 — Batch 011–016 (`01-web`, six rows to close the block)

This batch is six, not five, so the block closes on a task boundary.

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 011 DtoBoundaryMapping | that no domain type crosses the boundary, checked by reflection over the DTO's property types | adding a domain-typed property to the DTO must fail | a DTO exposing a domain *enum* must be handled deliberately — decide and document which side it belongs on |
| 012 ApiVersioning | that v1 and v2 coexist and a v1 client keeps working after v2 lands | the v1 response must not gain v2's new required field | v2 implemented by mutating the v1 contract must fail |
| 013 PaginationContract | cursor stability under insertion: a row inserted between page fetches must not shift the second page | offset pagination must visibly skip or duplicate a row; cursor pagination must not | ordering by a non-unique column must fail the stability fact |
| 014 BackendForFrontend | parallel fan-out and **partial failure**: one failing upstream must not fail the whole response | with one upstream throwing, the aggregate must still carry the others' data plus a per-source error | sequential fan-out must fail a fact asserting total elapsed virtual time |
| 015 RateLimitingPolicy | refill over time via `IClock`, and per-client partitioning | one client exhausting its budget must not affect another client | a fixed-window limiter must fail a fact that spans the window boundary the way a token bucket does not |
| 016 HealthReadinessLiveness | that readiness aggregates dependency checks while liveness does not | a failing dependency must flip readiness but leave liveness healthy | one combined check reported for both must fail |

After Step I, additionally update `CLAUDE.md`: add `Architecture/` to the per-track command table, the toolchain-status list, the track-specific-gotchas list and the current-state table. Commit that separately as `docs: register the Architecture track`.

---

### Task 7 — Batch 017–021 (`02-desktop`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 017 MvvmComposition | that `PropertyChanged` fires with the right property name and only on actual change | setting the same value twice must raise the event once | raising unconditionally must fail |
| 018 NavigationService | back-stack depth and lifecycle callback order across a forward-then-back sequence | navigating back must re-activate the *previous instance*, not a new one | recreating the view model on back must fail an instance-identity fact |
| 019 MessengerEventAggregator | that unsubscribe actually stops delivery, and that a collected subscriber does not keep the publisher alive | after unsubscribe, publishing must leave the handler count unchanged | holding subscribers strongly must fail the collection fact (`GC.Collect` plus a `WeakReference`) |
| 020 DialogServiceAbstraction | that the caller's flow branches on the dialog result without referencing any UI type | a "cancel" result must leave the store untouched | a dialog port returning `bool` only must fail a fact needing a three-way result |
| 021 BackgroundJobScheduler | sequencing and cancellation, on virtual time — never `Thread.Sleep` | cancelling a queued job must leave it un-executed *and* leave later jobs running | firing jobs on the thread pool without ordering must fail the sequencing fact |

---

### Task 8 — Batch 022–026 (`02-desktop`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 022 OfflineFirstSync | that local changes survive a restart and that conflict resolution follows the declared policy — real SQLite via `SqliteScratch` | a change made offline on both sides must resolve deterministically, and the loser must be recorded, not silently dropped | last-write-wins by *local* clock must fail against a fact using the server's version |
| 023 SettingsStatePersistence | that a v1 file loads under the v2 schema through a migration step | loading a v1 file must produce v2 defaults for new fields *and* leave a migration marker | ignoring the version field and relying on JSON defaults must fail |
| 024 PluginArchitecture | real isolation: the plugin's own copy of a shared type, and unload actually collecting the context | after `Unload()` plus GC, the `WeakReference` to the context must be dead | loading into the default context must fail the unload fact |
| 025 UndoRedoCommandStack | that redo is invalidated by a new command after undo | undo, then a new command, then redo must be a no-op with an empty redo stack | keeping the redo stack must fail |
| 026 ScopedPerViewDi | disposal at scope end, by reflection **and** behaviour: disposables resolved in a view scope must be disposed exactly once | closing one view must not dispose another view's scoped service | resolving from the root provider must fail both the disposal fact and the captive-dependency reflection fact |

---

### Task 9 — Batch 027–031 (`02-desktop` close, `03-services-data` open)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 027 ThreadMarshallingAbstraction | that work is posted through the port, proven by a recording dispatcher's call log, not by observing the result | code that already runs on the "UI" thread must still be observable as *not* re-posted | invoking inline always must fail the "posted from a background thread" fact |
| 028 TelemetryBoundary | that the domain emits events through a port with structured fields, not formatted strings | the recorded event must expose its fields individually, so asserting on a substring of a rendered message must be impossible | string interpolation into the message must fail the field-level fact |
| 029 RepositoryUnitOfWork | that nothing is persisted until `Commit`, proven against a real SQLite connection opened *outside* the unit of work | before commit, a second connection must not see the row; after commit it must | saving inside the repository method must fail |
| 030 SpecificationPattern | that specifications compose and that the composition is evaluated once as a single query | an `And` of two specs must produce one filter, and the fact asserts the generated predicate, not just the result set | filtering in memory after loading everything must fail a fact asserting rows read |
| 031 AggregateDomainEvents | that events are collected during the operation and dispatched **after** commit, never before | a commit that throws must dispatch nothing | dispatching inside the aggregate method must fail |

---

### Task 10 — Batch 032–036 (`03-services-data`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 032 TransactionalOutbox 🐳 | atomicity: the entity row and the outbox row must appear together or not at all, observed from a second connection | a failure after the entity write and before the outbox write must leave **neither** row | publishing directly to the bus and also inserting an outbox row must fail a fact asserting the bus saw nothing before the relay ran. 🐳 fact: the same against real Postgres |
| 033 IdempotentConsumer | that the *side effect* happens once under duplicate delivery, proven by the effect count, not the inbox row count | deliver the same message twice; the handler's effect must be applied once and the second delivery must still be acknowledged | dedup by message *content* hash must fail against two legitimately identical messages with different ids |
| 034 CacheAside | loader invocation **count** — the only thing that distinguishes a cache from no cache | two reads of the same key must invoke the loader once; a read of a second key must invoke it again | a cache that never expires must fail the TTL fact driven by `ManualClock` |
| 035 WriteThroughWriteBehind | that write-through leaves the store consistent immediately and write-behind does not until flush | after a write-behind write, the store must still hold the old value until flush | write-behind that flushes synchronously must fail |
| 036 CacheStampede 🐳 | single-flight: N concurrent readers of a cold key must invoke the loader exactly once | run the fact with a loader that blocks on a gate until all N callers have arrived, then release — a lock-free implementation must fail | double-checked locking *per call* without a shared in-flight task must fail. 🐳 fact: the same across two Redis connections |

Row 036 is the track's flakiness risk. Gate the loader on a `TaskCompletionSource` released after N arrivals rather than on a sleep, and re-run this batch's green check three times.

---

### Task 11 — Batch 037–041 (`03-services-data`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 037 DistributedLock 🐳 | expiry and the **fencing token**: a token from an expired lease must be rejected by the resource | holder A's lease expires (via `ManualClock`), B acquires, then A writes with its stale token — the write must be rejected | a lock with expiry but no fencing token must fail. 🐳 fact: real Redis `SET NX PX` |
| 038 OptimisticConcurrency 🐳 | that a stale version is rejected, proven with two readers and one writer | both read v1, both write; the second must fail and the store must hold the first writer's value | comparing the whole row instead of a version column must fail when a field is written back unchanged. 🐳 fact: real Postgres |
| 039 PessimisticLocking 🐳 | serialisation: the second writer must block until the first commits | interleave two connections; the final value must reflect both increments | reading, then locking, must fail the lost-update fact. 🐳 fact: `SELECT … FOR UPDATE` on Postgres |
| 040 SagaProcessManager | compensation: a failure at step 3 must run the compensations for steps 2 and 1, in reverse order | assert the compensation call order, not just that state ended "cancelled" | compensating forward-order must fail |
| 041 ChoreographyVsOrchestration | that both topologies reach the same end state but differ in **who knows about whom** — asserted by reflection on each participant's dependencies | the choreographed participants must have no reference to a coordinator type; the orchestrated one must | an orchestrator disguised as a subscriber must fail the dependency fact |

---

### Task 12 — Batch 042–046 (`03-services-data`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 042 EventSourcingAppendStream | append-only-ness and the expected-version check | appending with a stale expected version must be rejected and must leave the stream unchanged | storing only the latest snapshot must fail a rehydration-from-events fact |
| 043 EventSourcedProjection | checkpoint advance and idempotent apply | replaying the same events twice must leave the projection identical, and the checkpoint must not go backwards | rebuilding from scratch every time must fail the checkpoint fact |
| 044 CqrsReadModel | the staleness window: the read model must lag until the projection runs | write, read immediately (stale), run projection, read again (fresh) | reading through to the write store must fail the staleness fact |
| 045 MessageBusAbstraction | topic routing including wildcards, and that a handler for another topic is not invoked | publishing to `orders.created` must not reach an `orders.shipped` subscriber, but must reach `orders.*` | routing by CLR type instead of topic must fail the wildcard fact |
| 046 CompetingConsumers 🐳 | that each message is processed by exactly one consumer, **and** that same-key messages keep their order | N consumers, M messages, keys deliberately colliding: assert per-key sequence and total processed count | round-robin without key affinity must fail the ordering fact. 🐳 fact: real RabbitMQ |

---

### Task 13 — Batch 047–052 (`03-services-data`, six rows to close the block)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 047 DeadLetterQueue 🐳 | attempt counting and the **reason** recorded on the dead-lettered message | a message failing 3 times must land in the DLQ once, with attempt count and exception type recorded; a message failing twice then succeeding must not | dropping the message instead of dead-lettering must fail. 🐳 fact: a real RabbitMQ dead-letter exchange |
| 048 RetryWithBackoff | that delays grow exponentially, asserted on the **recorded delay sequence** from `ManualClock` — never on wall-clock | jitter must keep delays inside declared bounds; assert bounds, not equality | fixed-delay retry must fail the growth fact |
| 049 MqttTelemetryIngest | real broker: topic hierarchy and wildcard subscription (`+` single level vs `#` multi level) | a `+` subscription must not receive a two-level-deeper topic that `#` does receive | string-prefix matching must fail the `+` fact |
| 050 MqttQosRetainedLastWill 🐳 | retained delivery to a *late* subscriber, and last-will delivery on ungraceful disconnect | subscribe after publishing retained — the message must arrive; kill a client's socket without `DisconnectAsync` and the will must be delivered | a graceful disconnect must **not** deliver the will; an implementation that always publishes the will fails. 🐳 fact: broker restart with a persistent session |
| 051 MqttRequestReply | correlation data matching, and that a mismatched correlation is ignored | publish two requests concurrently; each reply must reach its own waiter | matching by response topic alone must fail when both requests share one |
| 052 EventSchemaEvolution | that a v1 payload is upcast to v2 and that an unknown field is tolerated | deserialising a v1 event must produce a v2 object with the declared default, and a v3 payload with an extra field must not throw | strict deserialisation must fail the tolerant-reader fact |

---

### Task 14 — Batch 053–057 (`04-cross-cutting`)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 053 ResiliencePipeline | strategy **ordering**: retry outside timeout retries the timeout; timeout outside retry bounds the whole thing | assert attempt counts for both orderings — they differ, and that difference is the lesson | a single combined strategy must fail one of the two ordering facts |
| 054 CircuitBreakerStates | the half-open probe: exactly one call passes while open-then-elapsed, and success closes while failure re-opens | assert that the *second* concurrent probe is rejected | a breaker that closes on elapsed time alone, without a probe, must fail |
| 055 CorrelationContextPropagation | that the id survives a bus hop, carried in message metadata rather than an ambient static | the consumer must see the producer's id when running on a different thread with no ambient context | an `AsyncLocal`-only implementation must fail the cross-message fact |
| 056 StructuredLoggingBoundary | that scopes nest and fields stay individually addressable | a nested scope's fields must be present on an inner log entry and absent after it exits | pre-rendered message strings must fail |
| 057 ConfigurationLayering | provider precedence and reload | a later provider must win; changing the source must be visible after reload and *not* before | reading providers in registration order without last-wins must fail |

---

### Task 15 — Batch 058–060 (`04-cross-cutting`, three rows to close the track)

| Row | Mechanism the facts must prove | Adversarial fact | Plausible-wrong probe |
|---|---|---|---|
| 058 AntiCorruptionLayer | by reflection: no type from the foreign context appears in the local context's public surface | adding a foreign-typed property anywhere in the local model must fail the fact | a local type that merely *renames* the foreign one field-for-field must still pass — document this as the ACL's honest limit |
| 059 StranglerFigFacade | that the facade routes per-feature and that the legacy path is still reachable for un-migrated features | a migrated feature must never reach legacy; an un-migrated one must never reach the replacement | an all-or-nothing switch must fail the per-feature fact |
| 060 ArchitectureFitnessTests | that a violation is *detected*, not merely absent — the exercise ships a deliberately violating type for the rule to catch | the rule must name the offending type and the offending dependency in its failure message | a rule that scans only the assembly's own namespaces must fail when the violation points at a referenced assembly |

- [ ] **Final step: full-suite verification in all three modes**

```bash
cd /c/Tools/FeWoLearning/Architecture
dotnet test --no-incremental 2>&1 | tail -8
dotnet test -p:UseSolutions=true --no-incremental 2>&1 | tail -8
dotnet test -p:UseSolutions=true -p:Containers=true 2>&1 | tail -8
```

Expected: red run — every exercise fact failed, only the 4 harness smoke facts passed, container facts skipped. Green run — 0 failed, container facts skipped. Container run — 0 failed, 0 skipped (needs Docker). Record the exact totals in `README.md` and in `CLAUDE.md`'s current-state table. **Read the test count, not just the word `Failed`** — the `avalonia/` lesson: a run that stops early still prints a normal-looking summary.

- [ ] **Final commit**

```bash
cd /c/Tools/FeWoLearning
git add Architecture/README.md Architecture/catalog.md CLAUDE.md
git commit -m "Architecture: track complete - 60/60 verified red and green"
```

## Self-review notes

- **Spec coverage:** §2 → Task 1; §3 → Task 1 steps 6–7; §4 → Task 3 plus Tasks 4–15 (all 60 rows appear exactly once across the batch tables); §5 tier 1 and 2 → Task 2; §5 tier 3 → Task 1 step 7 and Task 2 step 2, exercised by the eight 🐳 rows in Tasks 10–13; §6 → steps F and G of every batch, plus a per-row column; §7 → step C; §8 → the batch structure itself.
- **Row count:** the twelve batches (Tasks 4–15) are 5, 5, 6, 5, 5, 5, 5, 5, 5, 6, 5, 3 = 60 — blocks 001–016, 017–028, 029–052, 053–060, each row listed exactly once.
- **Container rows:** 032, 036, 037, 038, 039, 046, 047, 050 — eight, matching spec §5.
