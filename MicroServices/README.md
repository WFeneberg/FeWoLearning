# MicroServices — Exercise Track

## 1. What this track is

100 graded exercises on **microservices in .NET with Aspire**, following the repo's
universal exercise pattern: a stub that fails red before implementation and passes
green once it matches its reference solution.

**"Beginner" means Aspire and distributed-systems beginner, not C# beginner.** `ex001`
models a resource graph; it is not a `FizzBuzz`. Plain C# language drills belong to
`dotnet/`; ASP.NET Core component work belongs to `blazor/`.

Four subjects the track owner asked for are taught together because in practice they
are one subject: **Aspire orchestration**, **polyglot persistence** (SQL Server,
PostgreSQL, MongoDB, Redis, Valkey, Garnet, Qdrant, Milvus, Elasticsearch, MySQL,
Oracle, Cosmos DB and Azure Storage), **Docker**, and **Azure** — all of it runnable on
one developer machine, optionally inside the DevContainer, and **entirely offline for
the Azure rows**: emulators plus generated artifacts, no subscription, no `az login`,
no `azd up`.

`catalog.md` is the source of truth for what is written and what is next. The folder is
`MicroServices/` — capitalised, unlike every other track folder. That is the owner's
deliberate choice; do not "fix" it.

## 2. Prerequisites

- **.NET 10 SDK** — 10.0.400 verified.
- **nuget.org reachable** on first restore.
- **Docker** — only for the 🐳 rows. The default `dotnet test` needs no daemon at all.
  Docker 29.7.2 (Linux containers) and Compose v5.5.0 verified on this machine.
- **Aspire CLI** — only for `aspire run`. 13.4.6 verified; the CLI being one patch
  behind the 13.5.3 packages does not matter.
- **Azure CLI is not required and not installed.** Nothing in the track calls a
  subscription.

## 3. Commands

Run every command **from inside `MicroServices/`**, not the repo root.

| Command | Effect |
|---|---|
| `dotnet test` | the **red** run: stubs, L1 + L2 only, L3 skipped |
| `dotnet test -p:UseSolutions=true` | the **green** run: the same facts against `solutions/` |
| `dotnet test -p:Containers=true` | adds the L3 rows — real containers, real queries |
| `dotnet test --filter FullyQualifiedName~Ex001_` | one exercise |
| `aspire run --apphost playground -- --exercise ex001` | run that exercise's model in the real Aspire dashboard |
| `dotnet run --project playground -- --exercise ex001` | same, without the Aspire CLI |

`-p:UseSolutions=true` and `-p:Containers=true` apply to the **`dotnet test` rows
only**. Neither reaches the playground, and neither is accepted by `aspire run`:

- `playground/Playground.AppHost.csproj` references `exercises/` **unconditionally** —
  its own comment says so. The playground exists so the learner watches *their own*
  work run, so `UseSolutions` is not wired up there at all. To see a reference solution
  in the dashboard, read it and type it into the stub (or edit that `ProjectReference`
  by hand, and put it back).
- `Containers` reaches only the **test** project, through a
  `RuntimeHostConfigurationOption` in `tests/…csproj` — i.e. through the test
  assembly's `runtimeconfig.json`. The playground starts whatever containers its model
  declares regardless; there is nothing to gate.

There is no separate install step — `dotnet test` restores on first run.

**Current measured state** (2026-09-07; `catalog.md` at 40 ✅ / 60 ⬜). Counted on
disk: **126 exercise facts** across the forty delivered rows, of which three are 🐳,
plus **14 harness facts** in `tests/_support/` — **140** in total.

```
dotnet test                                         → 123 failed,  10 passed, 7 skipped (140 total),      5 s
dotnet test -p:UseSolutions=true                    →   0 failed, 133 passed, 7 skipped (140 total),     10 s
dotnet test -p:UseSolutions=true -p:Containers=true →   0 failed, 140 passed, 0 skipped (140 total), 2 m 31 s
```

**Those numbers moved a long way on 2026-09-07, and the two harness changes behind them
are §6's business, not a mystery to re-derive.** Before them the same suite read 20 s,
1 m 24 s and 5 m 26 s. In short: an in-process publish no longer probes for a container
runtime (~5.1 s → ~0.1 s, sixteen facts), and the 🐳 rows share one database server per
flavour instead of starting one each.

**The seven skips are not all harness facts, and the distinction matters.** Three are
real exercise facts — ex034's, ex038's and ex040's 🐳 rows, gated like every 🐳 row
will be. The other four are harness facts that are *deliberately* gated shut in the
default run: the container-gate canary that proves `Require()` still closes, the
teardown canary that needs a started application, and the two shared-server canaries
(isolation, and surviving a failing test). `-p:Containers=true` unskips all seven. The
10 that pass in the red run are the remaining harness facts, which pass in *both* modes
because they grade the harness rather than an exercise.

The container lane costs the difference between the last two lines, **~2 m 33 s**.
It is dominated by **server start-ups**, and since 2026-09-07 there are only two:
ex034 starts its own Postgres (~51 s, because that row grades the learner's own resource
graph and must), and everything else on SQL Server — ex038, ex040 and the two
shared-server canaries — shares **one** `mcr.microsoft.com/mssql/server:2022-latest`
(~55 s for the first caller, then milliseconds each). Before that change ex038 and
ex040 started a server each and cost ~1 m 25 s apiece; measured after, the two rows'
six facts together run in **1 m 4 s**. The remaining ~28 s is the two applications the
teardown canary starts, which start no containers — see §4.

The rule that follows, and the reason the change was made at 3 🐳 rows rather than at
25: the lane grows with the number of **servers**, not the number of rows, and the
assembly runs serially so it grows by the sum. A new 🐳 row on a flavour that already
has a shared server is nearly free. A worked-out example of why the offline/🐳 split in
`catalog.md` is worth defending is next door — rows 037 and 039 are *about* SQL Server
and Postgres schemas and cost 0 s of the lane, because a generated script needs no
server.

**A correct default run is red, and that is not a broken checkout.** A hundred and
twenty-three failures is exactly what an untouched tree gives: one `NotImplementedException` per
unimplemented `Configure`, plus the facts that depend on it. Update these numbers
whenever a batch lands.

The container-gate canary that skips here is only **one third** of the gate's
protection. It fails if `ContainerGate.Require()` ever stops skipping with containers off —
the mutant that would start real containers in the default run. The opposite and more
dangerous mutant, a `Require()` that *always* skips, would silently disable all 25 🐳
rows while every run still reported green; that one is caught by
`ContainerGate_Require_lets_the_test_through_when_containers_are_on`, which forces the
switch on for its own async flow only and **fails** (never skips) if the gate stays
closed. The third guards the direction neither of those covers: an author who simply
forgets `ContainerGate.Require()` in a future 🐳 row.
`ContainerHarness.RunAsync` throws rather than starting anything when the gate is
closed, and `ContainerHarness_refuses_to_start_anything_when_the_gate_is_closed` grades
that — forcing the gate shut for its own flow so it runs in *both* modes and
`-p:Containers=true` keeps reporting zero skips. All three mutants were built and
observed, not reasoned about. Keep all three facts.

### `-p:Containers=true`, and the no-rebuild alternative

`-p:Containers=true` reaches the test process through a
`RuntimeHostConfigurationOption` in `tests/…csproj`, i.e. through
`runtimeconfig.json`, so it requires a build. Setting `FEWO_MS_CONTAINERS=1` in the
environment opens the **gate** without one.

**But since ex034 the env var is no longer a full substitute, and the difference is
not the gate.** Starting a real `DistributedApplication` needs DCP, and `tests/` pulls
`Aspire.Hosting.AppHost` + `Aspire.Hosting.Orchestration.<rid>` in an item group
conditioned on `'$(Containers)' == 'true'` — precisely so the default restore and build
stay as small and Docker-free as they were. `FEWO_MS_CONTAINERS=1` flips the gate but
cannot add a package reference, so an L3 row let through that way reaches DCP that was
never restored and dies in **161 ms** with

```
Microsoft.Extensions.Options.OptionsValidationException :
  Property CliPath: The path to the DCP executable used for Aspire orchestration is required.
```

Measured 2026-09-07. It is loud, immediate and cannot be mistaken for a pass, which is
why the conditioning was kept rather than making every default restore carry the
orchestrator. Use `FEWO_MS_CONTAINERS=1` to exercise the **gate**; use
`-p:Containers=true` to actually run a 🐳 row.

The gate deliberately checks **only the switch**, never whether Docker is reachable.
With the switch on and no daemon the L3 tests **fail**, loudly. A broken Docker setup
must not be able to masquerade as a green run by silently skipping.

## 4. The three test levels

| Level | Asserts | Cost | Runs |
|---|---|---|---|
| **L1 model** | the resource graph: types, `ConnectionStringExpression`, annotations | ~1.4 s cold, ~10 ms warm | always |
| **L2 artifact** | `aspire-manifest.json` **and the generated Bicep**, both in-process | ~3.7 s (~7.5 s with Azure resources), no container | always |
| **L3 container** | a real database starts and a real query, message or expiry happens | minutes | opt-in |

**What L1 can prove.** `DistributedApplication.CreateBuilder(...)` + `Build()` produces
the complete model with **zero containers started** — **~1.4 s on the first call in a
process, and ~10 ms on every one after it** (measured 2026-09-07, four consecutive calls:
961, 15, 9, 9 ms; the cold figure is JIT and assembly loading, not model building). Quote
whichever number the question is about: a single L1 fact costs the cold one, a hundred of
them do not. That model is rich
enough to grade against: resource types, per-resource annotations (`WaitAnnotation`,
`HealthCheckAnnotation`, `ContainerImageAnnotation`, `EndpointAnnotation`,
`EnvironmentCallbackAnnotation`, `ContainerMountAnnotation`), and
`ConnectionStringExpression.ValueExpression`, which differs per database flavour. That
last one is what lets a test prove the learner wired up *PostgreSQL* rather than *some
container*. `ModelHarness` in `tests/_support/` is the entry point, with **two** build
methods: `Build(configure)` assembles the graph in **run** mode, and
`BuildForPublish(configure)` assembles it in **publish** mode — the same
`--operation publish` arguments `ManifestHarness` passes, but stopping at `Build()`, so
`builder.ExecutionContext.IsPublishMode` is true while the graph is assembled and
nothing is written to disk (measured: the output path is never even created). Rows about
mode-dependent modelling — ex020 is the first — need that third view, because `Build` is
run mode and `ManifestHarness` returns the published artifact rather than the graph
behind it.

**What L1 cannot prove.** That anything resolves, connects, or runs. Every value in the
graph is still an unresolved expression like `{pg.bindings.tcp.host}`.

**What L2 can prove.** The manifest carries, per resource: `type` (`container.v0`,
`value.v0`, `parameter.v0`, `dockerfile.v0`, `azure.bicep.v0`), `image` with the pinned
tag, the full `env` map including `ConnectionStrings__*`, `bindings` with `targetPort`,
and for generated secrets the `inputs.value.default.generate` policy. It is
deterministic, so it is a good assertion target for publish-shaped rows.
`ManifestHarness` is the entry point, with **two** of them:
`GenerateAsync(configure)` returns just the parsed manifest and deletes its output
directory before returning, while `PublishAsync(configure)` returns a disposable
`PublishOutput` that keeps the whole directory alive — `Files`, `ReadText(relative)`,
`BicepFiles`, `Has(relative)` and `Manifest`. Always `using` the latter; `Dispose`
deletes the directory, and every output lives under one temp root that is swept of
anything older than an hour on first use, so a forgotten `using` still cannot pile up.

**The manifest is not the only in-process artifact — Bicep comes out too.** Measured: an
in-process publish of a model carrying `AddAzureContainerAppEnvironment` plus
`AddAzureStorage` writes, in **~7.5 s**, all of

```
aspire-manifest.json
aca.module.bicep      aca-acr.module.bicep      storage.module.bicep
aca/aca.bicep         aca-acr/aca-acr.bicep     storage/storage.bicep
```

So the Azure rows (093 managed identity and role assignments, 094 Bicep customisation,
099 secrets across environments, 100 the capstone) assert on **real generated Bicep**,
in the fast loop, with no subscription and no golden-file fallback. They reach it
through `ManifestHarness.PublishAsync(...)`; `GenerateAsync` alone cannot, because it
deletes the output directory before it returns. `HarnessMechanicsTests
.ManifestHarness_hands_back_the_generated_Bicep_too` is the proof, and it is why
`Aspire.Hosting.Azure.AppContainers` and `Aspire.Hosting.Azure.Storage` are already
referenced by both content libraries.

**What L2 cannot prove.** Docker Compose YAML — and that is the *single* exception, not
a general limitation of in-process publish. See §6.

**What L3 is for.** Only the things a real run proves: migrations actually applying, a
Mongo aggregation actually returning documents, an index actually being *used*, an
outbox actually delivering, a Redis key actually expiring. **25 of the 100 rows** are
L3, marked 🐳 in `catalog.md`. Everything else stays in the fast loop.

### What a 🐳 test may assume — measured on 2026-09-07, when ex034 became the first one

`tests/_support/ContainerHarness.cs` is the only place in the track that touches
Docker. Every 🐳 row goes through `ContainerHarness.RunAsync(configure, body)`, and
every 🐳 test's **first line** is `ContainerGate.Require()`. These are the numbers and
the failure shapes the next twenty-four rows inherit.

- **Images are pulled on demand, and the pull is inside the session's budget.**
  Measured directly: `busybox:1.36` was absent from `docker images` before the run and
  present (6.76 MB) after, with no `docker pull` anywhere in the loop — DCP fetches
  what it does not have. A 🐳 row therefore does **not** need a warm cache to be
  correct, only to be quick, and the harness's 5-minute default budget is sized for a
  cold pull on a slow line rather than for the warm case.
- **A warm Postgres row costs ~57 s wall clock**, of which about ten is teardown.
  ex034's L3 fact was measured at **55 s, 57 s and 60 s** across five runs with
  `postgres:18.3` already local; `dotnet test -p:UseSolutions=true` goes from
  **1 m 27 s to 2 m 18 s** when `-p:Containers=true` adds that single row.
  **Superseded in part on 2026-09-07**: that arithmetic — "budget a minute per 🐳 row,
  so twenty-five of them is a twenty-five-minute lane" — was the reason
  `ContainerHarness.DatabaseAsync` exists. The lane now grows with the number of
  **servers**, not of rows: a 🐳 row on a flavour that already has a shared server costs
  its own work and nothing else. Budget a minute for the first row of each flavour and
  seconds for the rest, and read the entry-point table below before writing one.
- **With the switch ON and Docker unreachable, a 🐳 row FAILS — it does not skip, and
  it does not hang to the deadline.** Measured by poisoning `DOCKER_HOST`: the test
  failed after **28 s** with

  ```
  Aspire.Hosting.DistributedApplicationException : Stopped waiting for resource 'orders'
    to become healthy because it failed to start.
  ```

  Note what that message does *not* say: "Docker". It names the resource, not the
  daemon, so a reader seeing it should check `docker ps` before reading the exercise.
  The important property is the one the gate exists for — a broken Docker setup cannot
  masquerade as a green run by quietly skipping.
- **With the switch OFF, nothing is touched.** `Assert.SkipUnless` in
  `ContainerGate.Require()` skips before a builder exists, and `ContainerHarness.RunAsync`
  additionally **throws** if it is ever reached with the gate closed, so a forgotten
  `Require()` cannot make the default `dotnet test` pull images. That guard has its own
  canary, `ContainerHarness_refuses_to_start_anything_when_the_gate_is_closed`, which
  runs in *both* modes because it forces the gate shut for its own flow.
- **A hung container fails its own test and nothing else.** `RunAsync` links the test's
  token to a **5-minute** deadline and converts the cancellation into a
  `TimeoutException` naming the budget and how far the session got. Measured with the
  budget dialled down to 8 s: it threw at **8.06 s** — *"The container session exceeded
  8 s (it had NOT finished starting after 00:00:08.06). Is Docker running, and is the
  image already pulled?"* — and the whole call returned at 19 s, the balance being
  teardown, which gets its own 2-minute budget precisely because the session token is
  already cancelled by then.
- **Nothing leaks, including when the test fails.** `docker ps -a`, `docker network ls`
  and `docker volume ls` were counted before and after **seven** container runs —
  two green, three deliberately-failing mutants, one Docker-off failure and one forced
  timeout — and came back to the same 82 / 4 / 46 every time. The load-bearing piece is
  `DcpPublisher:WaitForResourceCleanup`, which the harness sets: without it the
  per-session Docker *network* outlives the run, one per test, forever. Any
  pre-existing `dapr_*` containers on this machine belong to other work and are not
  part of that count.
- **A failing teardown must never replace a failing test, and the harness now
  guarantees it.** This is the piece most likely to be copied wrong, so it is stated as
  a contract: `RunAsync` **captures** the session's failure rather than throwing it,
  runs teardown to completion, and only then rethrows — so the real assertion always
  wins. Teardown itself is bounded: `StopAsync` **and** `DisposeAsync` share one
  2-minute budget that is never the session token (when the deadline is what killed the
  test, that token is already cancelled and cleanup is exactly what still needs to
  happen). Both are attempted even if the other threw, and the first failure is kept.
  When the body **succeeded**, a teardown failure *is* reported, wrapped in an
  `InvalidOperationException` telling the reader to check `docker ps -a` — because a
  `StopAsync` that failed is how leftovers start poisoning every test behind it.
  This is a regression, not a hypothetical. The first version tore down inside a plain
  `finally`, where whatever it throws wins; measured against that version, a body
  failing with "THE REAL ASSERTION FAILURE" surfaced as
  `InvalidOperationException: TEARDOWN BLEW UP` instead, and a learner would have gone
  looking in the wrong place. `ContainerHarness_teardown_never_replaces_the_real_failure`
  pins both directions. It needs a started application (DCP), so it is gated — but it
  starts **no containers**: an empty model plus one hosted service that throws on stop.
- **Assume nothing about ports.** DCP publishes on an ephemeral host port, never the
  flavour's default — ex034 asserts `Port != 5432` for exactly that reason, and it is
  the most legible single proof that a hard-coded connection string could not have
  reached the database.
- **Do not assume the resolved connection string is free of braces.** It bit twice: once
  on ex034 while this bullet was being written, and again on 2026-09-07 in the harness's
  own shared-server isolation canary, which asserted `DoesNotContain("{")` on a **SQL
  Server** string and failed roughly one container lane in ten. The rule is not
  flavour-specific and it is not about Postgres. Aspire generates passwords from a
  character set that includes `{`, so a run in ten produces a
  perfectly resolved string containing one. Assert that the named placeholders are gone
  (`{pg.`, `.connectionString}`), never that no brace remains.

### Which container entry point a 🐳 row should use — read this before writing one

`ContainerHarness` has **two** ways in, and the default is the second one.

| | `RunAsync(configure, body)` | `DatabaseAsync(flavour, purpose)` |
|---|---|---|
| starts | a whole application, per test | nothing, after the first caller |
| gives you | a `Session`: the model's resources, `WaitForHealthyAsync`, `ConnectionStringAsync` | a fresh, empty database on the assembly's shared server for that flavour |
| costs | ~55-85 s | ~55 s once, then milliseconds |
| use it when | the row grades the LEARNER's resource graph | the row grades what happens inside a database |

**Use `DatabaseAsync` unless the row's subject is the model itself.** ex034 is the one
delivered row that must use `RunAsync`: its whole point is that the connection string
*Aspire resolved from the learner's own `AddDatabase`* reaches a client, and a
harness-supplied string would grade nothing. ex038 and ex040 want "a real SQL Server and
a database nobody has touched", which is what `DatabaseAsync` is for; their L1 facts
grade the model.

**A database, not a server, is the isolation boundary.** `DatabaseAsync` issues
`CREATE DATABASE` with a GUID-suffixed name and hands back a connection string pointing
at it, so each test gets its own catalogue, its own tables and its own
`__EFMigrationsHistory`. That claim is *proved*, not asserted:
`SharedServer_hands_out_ISOLATED_databases_on_ONE_server` takes two databases, creates
**the same table name** in both with different sentinel rows, and requires each to see
only its own — the `CREATE TABLE` alone would fail with "there is already an object
named 'probe'" if the boundary were cosmetic — while also asserting that both connection
strings name the same host and port, i.e. that this really is one container.

**The teardown contract is unchanged, and unchanged by construction rather than by
resemblance.** Both paths build their application through the same private
`CreateBuilder()` (so the two DCP configuration keys cannot drift) and tear it down
through the same private `TearDownAsync` (so the independent 2-minute budget, "attempt
both `StopAsync` and `DisposeAsync`", and "keep the first failure" are literally the
same lines). Two differences are deliberate:

- The shared server's start is bounded by **its own** deadline and never by the calling
  test's cancellation token. The server outlives the test that happened to trigger it,
  so letting that test's token cancel a half-started server would leave the next test to
  find a broken one. A start that fails is torn down immediately and **not cached**, so
  the next 🐳 test retries rather than inheriting wreckage.
- The stop happens **after the last test in the assembly**, from
  `tests/_support/HarnessLifetime.cs` — an `[assembly: AssemblyFixture(...)]` whose
  `InitializeAsync` deliberately does nothing, because an assembly fixture that started
  a container would start it in the default `dotnet test` too. Everything is lazy; that
  type only closes.

What a shared server changed, and therefore what the existing teardown canary no longer
covers on its own: with one application per test, a failing test tore down its own
server. Now it must leave the server **running and usable** —
`SharedServer_survives_a_failing_test_without_starting_a_second_server` pins that, and
also that no second server was started to service the next request. Both canaries are
🐳-gated; the gate canary itself was extended to grade `DatabaseAsync`'s guard as well
as `RunAsync`'s, because the shared path is a second way into Docker.

**Leaks were re-counted for the new path, not assumed.** `docker ps -a` /
`network ls` / `volume ls` came back to the same **82 / 4 / 46** after: the full
container lane, a deliberately failing container test (ex040's transfer mutated to never
commit — 1 failed, 5 passed, counts unchanged), and a forced timeout (the shared server's
budget dialled down to 8 s, which threw *"The shared SqlServer server exceeded 8 s (it
had NOT finished starting after 00:00:18.12)"* and returned the tree to 82 / 4 / 46).

## 5. How an exercise works

Each exercise is a static class exposing at minimum:

```csharp
public static class Ex037_EfCoreAgainstPostgres
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException("TODO: ex037 — …");
}
```

Rows whose subject includes application code (an EF Core model, a Mongo aggregation, an
outbox dispatcher) expose further members alongside `Configure`. **Stubs throw
`NotImplementedException`; the projects still compile.** A stub that fails to build is a
bug, not an exercise.

Because `Configure` takes an `IDistributedApplicationBuilder`, the code the tests grade
is exactly the code the dashboard runs — `playground/` is one AppHost that dispatches to
an exercise by name via `ExerciseRegistry`, instead of 100 executable AppHost projects.
**Add the registry line in the same commit as the exercise.**

`services/` holds a small fixed set of real ASP.NET Core projects (Catalog, Orders, and
their siblings) shared by rows that need a genuine HTTP service to reference. They are
not exercises, get no catalog rows, and change rarely. `tests/_support/` is the same
kind of thing on the test side: shared fixtures, never a TODO, never a catalog row.

### Project resources: what rows 011 and 056+ must call — measured

`exercises/` is a plain `Microsoft.NET.Sdk` class library, **not** an
`Aspire.AppHost.Sdk` project, so the generated `Projects.Catalog` / `Projects.Orders`
marker classes every Aspire tutorial passes to `AddProject<T>()` **do not exist here**
and never will. That does *not* block project resources. Measured on 2026-09-05 with a
throwaway probe compiled into `exercises/` and driven through `ModelHarness`, in both
the red and the green run:

- **What to call.** The non-generic overload
  `builder.AddProject(string name, string projectPath)` (`Aspire.Hosting`, in
  `Aspire.Hosting` package — already referenced). It works unchanged from the exercises
  library. `AddProject<TProject>()` does not compile here; do not try to make it.
- **How to express the path.** `projectPath` resolves against
  `builder.AppHostDirectory` — and under the harnesses that is **the test assembly's
  own output directory**, not the repo root and not `playground/`. Measured:
  `…/MicroServices/tests/bin/Debug/net10.0` in the red run,
  `…/MicroServices/artifacts-solutions/bin/FeWoLearning.MicroServices.Tests/debug`
  in the green run, and `…\MicroServices\playground` when the same `Configure` runs in
  the playground. So **never hardcode a relative literal**: `..\..\..\..\services\…`
  happens to work in both test modes today only because both output directories are
  coincidentally four levels deep, and it is wrong in the playground. Walk up to the
  track root instead, from a directory the builder hands you:

  ```csharp
  static string TrackRoot(IDistributedApplicationBuilder builder)
  {
      var d = new DirectoryInfo(builder.AppHostDirectory);
      while (d is not null && !File.Exists(Path.Combine(d.FullName, "FeWoLearning.MicroServices.slnx")))
          d = d.Parent;
      return d?.FullName ?? throw new InvalidOperationException("not inside MicroServices/");
  }

  builder.AddProject("catalog",
      Path.Combine(TrackRoot(builder), "services", "Catalog", "Catalog.csproj"));
  ```

  Forward and backward slashes both work. A path that does not resolve throws
  `DistributedApplicationException: The project file "<fully resolved path>" was not
  found` **from `AddProject` itself**, i.e. inside `Configure`, naming the absolute path
  it tried — a loud failure, not a silent one.
- **The walk-up has a measured limit: an output tree outside `MicroServices/`.** Running
  `dotnet test -p:UseSolutions=true -p:ArtifactsPath=<a directory outside the track>`
  produces exactly **7 failures** — ex011's three facts and ex012's four — because the
  walk-up starts from `builder.AppHostDirectory`, which is now outside `MicroServices/`,
  and never reaches `FeWoLearning.MicroServices.slnx`. Every supported configuration —
  the red run, the green run, `-p:Containers=true`, the playground — keeps its output
  directory inside the track, so this is a **limit**, not a bug, and leaving it alone was
  a deliberate choice. A more robust alternative exists — injecting the track root as a
  `RuntimeHostConfigurationOption` from `Directory.Build.props`, the same mechanism
  `Containers` already uses (§3) — and it was consciously not adopted: only a handful of
  rows use the walk-up, and it is correct everywhere the track is actually run. Anyone
  who points `ArtifactsPath` outside `MicroServices/` and sees exactly these 7 failures
  should recognise this paragraph, not go bug-hunting.
- **What you get.** `Aspire.Hosting.ApplicationModel.ProjectResource`, which is both
  `IResourceWithEndpoints` and `IResourceWithServiceDiscovery`. Its annotations at
  `Build()` time are `ProjectMetadata` (the `IProjectMetadata`, whose `ProjectPath` is
  the **fully resolved absolute path** — that is the thing row 011 should assert),
  `ProjectLaunchDefaultsAnnotation`, `SupportsDebuggingAnnotation`,
  `OtlpExporterAnnotation`, `EnvironmentAnnotation`, four `EnvironmentCallbackAnnotation`s,
  `ContainerBuildOptionsCallbackAnnotation`, `PipelineStepAnnotation`,
  `PipelineConfigurationAnnotation`, and the three certificate-trust ones.
- **Endpoints, and how row 011 closed the gap it used to leave.** Until ex011 landed,
  that list carried **no `EndpointAnnotation`** at all: `services/Catalog` and
  `services/Orders` shipped no `launchSettings.json`, so "the launch profile that
  supplies its endpoints" was not observable. Both services now have
  `Properties/launchSettings.json` with the two profiles a `dotnet new webapi` project
  gets, and the resulting endpoints were measured on 2026-09-06:

  | call | annotations added |
  |---|---|
  | `AddProject("catalog", path)` | one `EndpointAnnotation`: `http`, scheme `http`, `Port` 5080, `TargetPort` null, `IsProxied` true |
  | `AddProject("orders", path, launchProfileName: "https")` | a `LaunchProfileAnnotation` (`"https"`) **plus two** endpoints: `https`/7081 and `http`/5081 |
  | `AddProject(…, launchProfileName: null)` | an `ExcludeLaunchProfileAnnotation` and **zero** endpoints |

  Catalog is on 5080/7080 and Orders on 5081/7081. Only the **default** profile — the
  first in the file, `http` — is applied when no name is passed; the `https` profile's
  `applicationUrl` lists two URLs in one string and each becomes its own endpoint.
  The measured trap for anyone grading this: `launchProfileName: null` plus a
  hand-written `WithHttpEndpoint(port: 5080, name: "http")` produces an
  `EndpointAnnotation` **identical in every observable field** to the profile's, so the
  only trace of the difference is that `ExcludeLaunchProfileAnnotation`. ex011 asserts
  its absence for exactly that reason.
- **The two `launchSettings.json` files are now exercise contract, not scaffolding.**
  ex011 asserts directly against the shape measured above: the default profile's single
  `http` endpoint (port 5080 for Catalog, 5081 for Orders), the `https` profile's two
  endpoints from its one two-URL `applicationUrl` (7081 and 5081 for Orders), and
  `launchProfileName: null`'s `ExcludeLaunchProfileAnnotation` with zero endpoints.
  **Editing either file — tidying ports, standardising profiles across `services/` —
  reddens ex011**, with no compile error and nothing in the diff pointing back at the
  exercise. Treat `services/Catalog/Properties/launchSettings.json` and
  `services/Orders/Properties/launchSettings.json` as fixed once ex011 depends on them,
  the same way `tests/_support/` is fixed once exercises depend on it.
- **`AddProject` adds a second, hidden resource.** Measured: each project also brings a
  `<name>-rebuilder` `ProjectRebuilderResource` (a subclass of `ExecutableResource`,
  carrying `HiddenAnnotation`), so a model that declared two projects holds four
  resources. Nobody asked for it, so it grades nothing — but `Assert.Single` over
  `model.Resources.OfType<ExecutableResource>()` in a model that also has projects will
  find it.
- **The alternative, if a future harness wants repo-relative literals.**
  `DistributedApplicationOptions.ProjectDirectory` exists and, when set, *does* become
  `builder.AppHostDirectory` (measured: setting it to the track root makes
  `services/Catalog/Catalog.csproj` resolve). It was deliberately **not** adopted,
  because it would only fix the two harnesses and leave the playground — where
  `AppHostDirectory` is `MicroServices/playground` — disagreeing with them. The walk-up
  above is the one form correct in all three hosts.

### Service-side rows live in the same library pair — deliberately

Rows 021-023 are the first that leave the AppHost model and grade code that runs
*inside a service*: `AddServiceDefaults`, a custom `ActivitySource`/`Meter`
registration, and a pair of health-check probe endpoints. They need ASP.NET Core and
the OpenTelemetry / service-discovery / resilience packages, which the two content
libraries did not carry.

They were given to **`exercises/` and `solutions/`**, not to a third pair of projects.
The reason is the `UseSolutions` switch: it works because `tests/` references *exactly
one* content library, and a second pair would mean a second switch to keep in step -
the same reasoning that keeps `blazor/`'s RCLs at one pair. So both `.csproj` files
gained, identically and in one commit:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />

Microsoft.Extensions.ServiceDiscovery              10.9.0
Microsoft.Extensions.Http.Resilience               10.9.0
OpenTelemetry.Extensions.Hosting                    1.18.0
OpenTelemetry.Instrumentation.AspNetCore            1.18.0
OpenTelemetry.Instrumentation.Http                  1.18.0
OpenTelemetry.Instrumentation.Runtime               1.18.0
OpenTelemetry.Exporter.OpenTelemetryProtocol        1.18.0
```

Two consequences a later author should know.

- **A service-side row exposes something other than `Configure`.** ex021 and ex022 are
  `IHostApplicationBuilder` extension methods; ex023 exposes `ConfigureProbes` plus
  `MapProbes`. That is the same freedom §5 already grants ("rows whose subject includes
  application code expose further members alongside `Configure`"), taken one step
  further: these three have no `Configure` at all, because they have no resource graph.
- **They are in `ExerciseRegistry` but not runnable in the playground.** The registry
  now carries a second dictionary, `WithoutAModel`, mapping those three ids to the
  reason; `AppHost.cs` prints it and exits instead of throwing "Unknown exercise", and
  `Known` lists them alongside the runnable ones. Inventing a fake resource graph purely
  so that `aspire run --exercise ex021` did something would have added ungraded content,
  which is worse than saying so.

**`services/Catalog` and `services/Orders` were deliberately not touched.** ex021 grades
a learner-written `AddServiceDefaults`, not those two projects calling one, for three
reasons: they are fixed fixtures that ex011 already asserts against (§5); they would have
to take a `ProjectReference` on `exercises/` to call the learner's extension, which puts
learner code inside a resource the AppHost launches; and the row's own spec says to
assert the registrations in the `IServiceCollection` rather than that an app started. A
`Host.CreateApplicationBuilder()` inside the test is the whole fixture needed.

### EF Core lives in the same library pair too — rows 036-040

Rows 036-040 are the first that need an ORM, and they follow **exactly** the precedent
rows 021-023 set above: the packages went into `exercises/` **and** `solutions/`,
identically and in one commit, rather than into a third pair of projects. Same reason,
and it is the only reason that matters: `tests/` references exactly one content library,
so `UseSolutions` must stay a single switch over a single pair.

```xml
Microsoft.EntityFrameworkCore.SqlServer        10.0.11
Npgsql.EntityFrameworkCore.PostgreSQL          10.0.3
```

Four notes on that pair, each of which decided something:

- **EF Core is pinned to 10.0.11, the same servicing band as the installed
  `Microsoft.AspNetCore.App` 10.0.11** that `tests/` already pins `Microsoft.AspNetCore.TestHost`
  to. EF ships on the .NET 10 train, so this is the coherence rule §7 already applies to
  OpenTelemetry, not a new one.
- **The Npgsql provider drags `Npgsql` forward, and `tests/` had to follow.**
  `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3 pins `Npgsql` **10.0.3** (10.0.2 of the
  provider pins the same 10.0.3 — checking both was worth the minute), so the content
  libraries now resolve 10.0.3 and `tests/`'s own `Npgsql` pin moved 10.0.2 → 10.0.3 in
  the same commit. ex034's L3 fact seeds a row through its own driver before asking the
  exercise to read it back; the two must never be different drivers. The provider's
  floor on `Microsoft.EntityFrameworkCore.Relational` is `[10.0.4, 11.0.0)`, which
  10.0.11 satisfies, so both providers sit on one EF Core.
- **`Microsoft.EntityFrameworkCore.Design` is deliberately absent.** Nothing here shells
  out to `dotnet ef`, and nothing can: there is no startup project for it to point at.
  The two rows about generated SQL (037, 039) use `DatabaseFacade.GenerateCreateScript()`,
  which is ordinary runtime API in `.Relational`, and row 038 ships **hand-written
  `Migration` classes** — `[DbContext]`, `[Migration]`, `Up`, and nothing else. Measured:
  a hand-written migration with no `ModelSnapshot` class anywhere in the assembly applies
  cleanly and raises no pending-model-changes error.
- **`Microsoft.EntityFrameworkCore.Sqlite` 10.0.11 is in `tests/` and nowhere else.** It
  is test equipment: ex039 grades a startup seed's *re-run* safety, which needs a real
  relational store with real primary keys, and SQLite gives one in memory in
  milliseconds with no container. The exercise never names a provider — its `SeedAsync`
  takes a `DbContext` — so the same code is what would run on SQL Server.

Two consequences for a later author, the same shape as the ASP.NET Core set's:

- **Every one of these five rows exposes more than `Configure`.** A `DbContext` and its
  entities are nested inside the exercise's static class, so five rows can each have a
  `CatalogContext` without colliding, and the service-side entry points sit beside
  `Configure` rather than replacing it.
- **Only 038 and 040 are 🐳, and that was a design constraint, not an accident.** 036 is
  wiring (a resource graph plus a provider registration), 037 compares two providers'
  generated DDL, and 039 reads INSERTs out of a generated script and re-runs a seed
  against SQLite. None of the three needs a server, and each would have cost ~1 m 25 s
  if it had been allowed to start one.

### `solutions/` is in the build here — deliberately

`exercises/` and `solutions/` compile **the same type names into the same namespaces**,
and `tests/` references exactly one of them via the `UseSolutions` MSBuild property,
never both — so the collision the repo-wide "`solutions/` outside the build" convention
exists to prevent cannot occur. This is the same permanent waiver `blazor/`, `uno/`,
`wpf/`, `caliburn/` and `avalonia/` take, and the payoff is that reference solutions are
compile-checked and test-run on every green check and cannot drift silently.

`Directory.Build.props` redirects the solutions build through
`UseArtifactsOutput`/`ArtifactsPath`. That is **required, not cosmetic**: two projects
emitting the same generated assembly-info attributes into one `obj/` tree fails the
build with `CS0579`. It has to live in `Directory.Build.props`, because
`BaseOutputPath`/`BaseIntermediateOutputPath` set inside a `.csproj` body are read after
the SDK props import — too late.

Namespaces are pinned per tier
(`FeWoLearning.MicroServices.Exercises.Beginner/.Intermediate/.Advanced/.Expert`),
because `01-beginner` is not a valid C# identifier.

## 6. Traps measured while building this track

Each of these cost real time. None is a guess.

- **`aspire publish` never exits in a non-interactive shell.** It writes its artifacts
  and then drops into "press CTRL+C to stop the AppHost" — still running at 200 s and at
  600 s. It is unusable inside a test loop. Use the in-process path instead:
  constructing the builder with `Args = ["--operation", "publish", "--output-path", dir]`
  and awaiting `RunAsync` returns cleanly in **≈3.7 s** and writes
  `aspire-manifest.json`. That is what `ManifestHarness` does and what L2 asserts
  against.
- **Docker Compose YAML is not obtainable in-process — and it is the only thing that
  isn't.** Every argument combination tried (`--publisher default`, `--publisher
  compose`, no publisher, with and without `--operation publish`) produced the manifest
  but never the compose file, because that file is emitted by a pipeline the CLI drives.
  Row 089 therefore asserts against a **committed golden `docker-compose.yaml`**,
  generated once at authoring time with the CLI and checked in; the graded claim is that
  the learner's model is consistent with that file, not that the test re-runs the CLI.
  Do **not** generalise this into "publish artifacts need golden files" — Bicep is
  emitted in-process just fine (§4), and the Azure rows rely on that.
- **Two compute environments without assignment is a hard failure.** Declaring both
  `AddDockerComposeEnvironment` and `AddAzureContainerAppEnvironment` without assigning
  resources fails the `validate-compute-environments` pipeline step: *"Compute
  resource(s) … are not assigned to a compute environment, but the model contains
  multiple compute environments"*. Row 095 is built on exactly this.
- **`FactAttribute.Skip` is not virtual in xunit.v3 3.2.2.** The usual custom
  `[ContainerFact] : FactAttribute` that overrides `Skip` does not compile — `CS0506`.
  The gate is therefore a `ContainerGate.Require()` call as the **first line of the test
  body**, using `Assert.SkipUnless`. If a future bump makes `Skip` virtual, the attribute
  form becomes available again; until then, do not try to reintroduce it.
- **`HealthCheckAnnotation.Key` is `{resource}_{endpoint}_{path}_{statusCode}_check`.**
  Measured on Aspire 13.5.3 by dumping a built model, not read anywhere:
  `AddContainer("api","nginx").WithHttpEndpoint(targetPort: 8080).WithHttpHealthCheck("/healthz")`
  yields `api_http_/healthz_200_check`, and calling `WithHttpHealthCheck()` with no path
  yields `api_http_/_200_check`. The format is undocumented and is not part of any
  contract Aspire promises, so **a version bump may change it**. Exercise ex004 fact 1
  pins the two exact keys and is therefore the tripwire: on a bump it fails with a loud
  string-equality diff naming the old and new key, never a silent pass. ex004 fact 2
  asserts only that each key *contains* its own path and not the other's, so it survives
  a reformat and keeps grading the thing that matters. Anyone writing a later
  health-check row should assert the substring, not the whole key, unless they also want
  to own the tripwire.
- **Aspire attaches `HealthCheckAnnotation`s of its own.** `AddPostgres("pg").AddDatabase("orders")`
  arrives with `pg_check` and `orders_check` already present, nobody having asked. A
  health-check exercise written against an *integration* resource therefore grades
  nothing — the annotation is there whether or not the learner did anything. ex004 uses
  bare `AddContainer`s for exactly this reason; a bare `AddContainer` carries none.
- **`EnvironmentAnnotation` derives from `EnvironmentCallbackAnnotation`, and is
  `internal`.** Measured while writing ex007: `WithEnvironment("REGION", "eu-west")` —
  a plain literal — writes an `EnvironmentAnnotation`, which is a *subclass* of
  `EnvironmentCallbackAnnotation`. So `OfType<EnvironmentCallbackAnnotation>().Count()`
  returns the same number for an all-literal resource and an all-callback one, and any
  fact built on that count grades nothing. The type is also `internal`, so a test
  cannot name it (`CS0122`). The way to tell a literal from a callback is to **run**
  the callbacks: construct an `EnvironmentCallbackContext(new
  DistributedApplicationExecutionContext(op), resource, dict, ct)`, invoke every
  `EnvironmentCallbackAnnotation.Callback`, and read the dictionary. **Reading the
  merged dictionary is still not enough**, and this is the sharp edge: writing
  `context.EnvironmentVariables["REGION"] = "eu-west"` from inside a callback lands the
  same `System.String` a literal would, so an implementation using no literal overload
  at all is invisible to any assertion over merged values. Partition first —
  `annotation.GetType() == typeof(EnvironmentCallbackAnnotation)` is the deferred form,
  anything derived is the literal form — then run each group separately and assert
  which variable came from which. Measured composition on 13.5.3:
  `WithEnvironment(name, "s")` → derived; `WithEnvironment(callback)` → exact;
  `WithEnvironment(name, EndpointReference)` → **exact**, i.e. deferred, not a literal.
  Separately, running the callbacks twice under different
  `DistributedApplicationOperation`s is the one assertion no literal can satisfy in any
  spelling. ex007 uses both techniques, one per direction of its subject.
- **`WithPersistentLifetime()` / `WithSessionLifetime()` are experimental.** The Aspire
  API reference says to prefer them over `WithLifetime(ContainerLifetime.…)` "for new
  code". On 13.5.3 both are marked `[Experimental]` with the diagnostic
  `ASPIREPERSISTENCE001` ("for test purposes only"), and **Roslyn reports an
  `[Experimental]` use as an error by default** — that is the compiler's behaviour, not
  a policy of this track. `MicroServices/` sets no `TreatWarningsAsErrors`,
  `WarningsAsErrors` or `AnalysisLevel` anywhere, and a future author should not infer
  one from this entry. The two spellings therefore need an explicit `#pragma warning
  disable` or `NoWarn` to compile at all; `WithLifetime` is the call to use, and ex010
  uses it.
- **A container's `ContainerImageAnnotation` is there before the learner touches it,
  and the manifest cannot grade image pinning.** `AddContainer("api", "nginx")` already
  carries `Image="nginx"`, `Tag="latest"`, `Registry=null`, `SHA256=null` — so the
  annotation's presence proves nothing and only its fields do. Worse for grading:
  `AddContainer("api", "ghcr.io/acme/api:2.4.1")` parses into `Image="ghcr.io/acme/api"`,
  `Tag="2.4.1"`, `Registry=null` and publishes an `"image"` string **byte-identical** to
  the correct three-call answer. ex006 is therefore graded at L1 on the three fields
  separately, with no manifest fact at all. Measured too: `WithImageSHA256` clears
  `Tag`, even when `WithImageTag` was called first.
- **A bind mount's `Source` is resolved to an absolute host path; a volume's is not.**
  `WithBindMount("./seed", …)` stores the path resolved against
  `builder.AppHostDirectory`, which under the harnesses is the **test assembly's output
  directory** — a different absolute path in the red run and the green run. So a mount
  test must assert `Path.IsPathRooted` plus the last segment, never the whole path. In
  the manifest the same source comes back *relative to the publish output directory*
  (a temp folder), i.e. a long `../../..` chain. `WithVolume("pgdata", …)` leaves
  `Source` as the literal name, which is what makes the rooted-path check a second,
  independent way of separating the two mount kinds.
- **Container lifetime does not reach the manifest.** Persistent, session and untouched
  containers publish identically — it is a local run-mode concept. ex010 is L1-only for
  that reason.
- **Only one `AddConnectionString` overload publishes `value.v0`.** Measured while
  writing ex013. `AddConnectionString(name, ReferenceExpression)` and
  `AddConnectionString(name, builder => builder.Append($"…"))` return a public
  `ConnectionStringResource` and publish as **`value.v0`** with the expression inline.
  `AddConnectionString(name)` and `AddConnectionString(name, environmentVariableName)`
  return an **internal** `ConnectionStringParameterResource` and publish as
  **`parameter.v0`** — a different artifact shape for what reads like the same call.
  The internal type is also unnameable from a test (`CS0122`), the same wall ex007 hit
  with `EnvironmentAnnotation`, so `Assert.IsType<ConnectionStringResource>` is how the
  wrong overload gets rejected.
- **A database child does not always interpolate its parent's connection string.**
  Measured across three flavours: `{pg.connectionString};Database=billing` and
  `{sql.connectionString};Initial Catalog=inventory` both defer to the parent, because
  the child's clause goes at the *end*. Mongo's does not — a database name is a path
  segment in the middle of a URI, so `MongoDBDatabaseResource` re-renders
  `mongodb://admin:{mongo-password.value}@{mongo.bindings.tcp.host}:{…port}/reviews?…`
  in full. Parenting (`IResourceWithParent`) and connection-string composition are two
  separate facts about a child, and ex014 grades both. The practical consequence for a
  test: a mutant that replaces `AddDatabase` with
  `AddConnectionString("billing", ReferenceExpression.Create($"{pg};Database=billing"))`
  renders the **byte-identical** expression and is caught only by the `Parent`
  assertion.
- **`WaitAnnotation` carries `WaitType` *and* `ExitCode`, and `WaitFor` on a child emits
  two of them.** Measured: `WaitFor(orders)` where `orders` is a database on `pg` leaves
  a `WaitAnnotation` for **`pg`** as well as one for `orders`, both
  `WaitUntilHealthy` — so any wait assertion has to filter by resource name or it
  either fails against the right answer or is satisfied by the weaker `WaitFor(pg)`.
  `WaitForCompletion(x)` leaves one annotation with `WaitType.WaitForCompletion` and
  `ExitCode` 0 by default (`exitCode:` overrides it). ex002 and ex015 both turn on this.
- **An executable's `WorkingDirectory` is absolutised, and the manifest's copy is not
  usable.** Same shape as a bind mount's `Source` (above): a relative
  `workingDirectory` passed to `AddExecutable` is resolved against
  `builder.AppHostDirectory`, i.e. the test assembly's output folder under the
  harnesses. In `aspire-manifest.json` the same path comes back *relative to the publish
  output directory* — a fresh temp folder — so it is a `../../..` chain that differs
  every run. Grade it at L1 with `Path.IsPathRooted` plus the last segment; do not
  assert `executable.v0`'s `workingDirectory` at all. `command` and `args` publish
  cleanly and are fine to assert.
- **`WithReplicas` exists only on `IResourceBuilder<ProjectResource>`, and Aspire does
  not police what it is combined with.** Measured on 13.5.3 by reflecting over every
  public static `WithReplicas` in `Aspire.Hosting`: there is exactly one, and it takes a
  project. There is no container spelling, so a replica row has to use `AddProject` and
  the walk-up in §5. More importantly for grading, `WithReplicas(3)` next to a **fixed
  host port** neither throws nor warns — and that is correct, not a gap: a proxied
  endpoint has one listener (the proxy) in front of N instances, so one host port there
  is exactly what the proxy is for. What actually breaks is a fixed port on a
  **proxyless** endpoint, and Aspire does not detect that either: the only replica
  combination it rejects in managed code is a persistent container lifetime (*"uses multiple replicas and a persistent lifetime. These features
  do not work together"* — the only replica-related diagnostic string in the assembly).
  ex018 therefore grades the **shape** of the model — `ReplicaAnnotation` on the scaled
  resource only, `Port` null and `IsProxied` true there, a pinned proxyless port on the
  single-instance one — and not a runtime check that does not exist. `catalog.md` row
  018 says all of this; an earlier wording of it ("a single fixed host port and replicas
  are contradictory") was corrected once this was measured, so the row and this entry
  agree and neither is a correction of the other. A second measured
  trap in the same row: omit `launchProfileName: null` and the launch profile supplies
  the endpoint, so `AddProject("catalog", …).WithReplicas(3)` arrives with a **fixed**
  `Port` 5080 and a null `TargetPort` from a file nobody looked at.
- **`WithUrl` writes its annotation now; `WithUrlForEndpoint` writes none at all.**
  Measured on 13.5.3: `WithUrl(url, displayText)` puts a `ResourceUrlAnnotation` on the
  resource immediately, with `Endpoint` null. Both `WithUrlForEndpoint` overloads put a
  **`ResourceUrlsCallbackAnnotation`** there instead and nothing else — the endpoint's
  address does not exist until it is allocated, so there is nothing to decorate at model
  time. A test that only reads `ResourceUrlAnnotation`s therefore sees *nothing* from the
  endpoint half of a correct answer and is satisfied by a second `WithUrl` carrying a
  guessed address. ex017 runs the callbacks by hand — construct a
  `ResourceUrlsCallbackContext(executionContext, resource, urls)` seeded with the
  endpoint's url (`new EndpointReference((IResourceWithEndpoints)resource, name)` is the
  only field the callbacks match on) and invoke every callback. Measured behaviours worth
  knowing: the `Action<ResourceUrlAnnotation>` overload **edits** the matching url in
  place, the `Func<EndpointReference, ResourceUrlAnnotation>` overload **adds** a new one
  and Aspire fills in its `Endpoint` even when the callback left it null, and a name that
  matches no endpoint is a silent no-op (a logged warning, not a throw).
- **`ExcludeFromManifest` and an `if (IsRunMode)` branch are indistinguishable from the
  model and from the manifest.** Measured on 13.5.3 while writing ex019: wrapping the
  `AddContainer` in `if (builder.ExecutionContext.IsRunMode)` produces a run-mode model
  that contains the resource and a manifest that does not — byte-identical, for grading
  purposes, to the correct `ExcludeFromManifest()` answer, because `ModelHarness` builds
  in run mode. The **only** trace of which mechanism was used is that
  `ExcludeFromManifest` leaves a `ManifestPublishingCallbackAnnotation` on the resource
  and the branch leaves nothing (a plain `AddContainer` carries none, so the annotation
  is a real difference and not something everything has). Any future row about
  run-mode-only resources needs that third assertion or it grades the wrong exercise.
- **The four stores in this tier produce four structurally different connection
  strings, and only one of them is keyed the way ADO.NET is.** Measured on 13.5.3
  while writing rows 026-029, and the reason those rows are not the same exercise
  four times:

  ```
  sqldata :: SqlServerServerResource :: Server={sqldata.bindings.tcp.host},{sqldata.bindings.tcp.port};User ID=sa;Password={sa-pw.value};TrustServerCertificate=true
  catalog :: SqlServerDatabaseResource :: {sqldata.connectionString};Initial Catalog=catalog
  pg      :: PostgresServerResource   :: Host={pg.bindings.tcp.host};Port={pg.bindings.tcp.port};Username=postgres;Password={pg-password.value}
  docs    :: MongoDBServerResource    :: mongodb://admin:{docs-password.value}@{docs.bindings.tcp.host}:{docs.bindings.tcp.port}/?authSource=admin&authMechanism=SCRAM-SHA-256
  cache   :: RedisResource            :: {cache.bindings.tcp.host}:{cache.bindings.tcp.port},password={cache-password.value}{cond-cache-bindings-tcp-tlsenabled-<hash>.connectionString}
  ```

  SQL Server joins host and port with a **comma** and fixes the login to `sa`;
  Postgres uses two keyed clauses and fixes it to `postgres`; Mongo is a URI whose
  database name is a **path segment**; Redis is a bare `host:port` with
  comma-separated StackExchange.Redis options and **no scheme** — although its
  *endpoint* does carry `UriScheme` "redis" where Postgres's carries "tcp", so "no
  scheme" is a claim about the string, not about the model. Redis is also the only
  one of the four with **no database child resource at all**.
  Two tails not to pin: Redis's conditional TLS fragment carries a **content hash**
  in its resource name (`cond-cache-bindings-tcp-tlsenabled-3eddb73a` for "cache",
  `…-9058fe65` for "sessions" — deterministic per name, but not a contract), and
  the `cond-…` resource does **not** appear in `builder.Resources` even though the
  expression interpolates it.
- **A referenced database hands its consumer seven variables beside
  `ConnectionStrings__*`, and a hand-rolled connection string hands it none.**
  Measured on 13.5.3 while writing ex027, and it is the sharpest grading hook this
  batch found. `AddContainer("api","nginx").WithReference(db)` where `db` is
  `AddPostgres("pg").AddDatabase("ordersdb", "orders_v2")` writes:

  ```
  ConnectionStrings__ordersdb   = {ordersdb.connectionString}
  ORDERSDB_HOST                 = {pg.bindings.tcp.host}
  ORDERSDB_PORT                 = {pg.bindings.tcp.port}
  ORDERSDB_USERNAME             = postgres
  ORDERSDB_PASSWORD             = {pg-password.value}
  ORDERSDB_DATABASENAME         = orders_v2
  ORDERSDB_URI                  = postgresql://postgres:{pg-password.value}@{pg.bindings.tcp.host}:{pg.bindings.tcp.port}/orders_v2
  ORDERSDB_JDBCCONNECTIONSTRING = jdbc:postgresql://{pg.bindings.tcp.host}:{pg.bindings.tcp.port}/orders_v2
  ```

  Every key is the **resource** name upper-cased; only `_DATABASENAME` and the tail
  of the two URI forms carry the **database** name. SQL Server and MongoDB emit the
  same shape with their own flavours (`mssql://sa:…`, `jdbc:sqlserver://…;
  databaseName=…;trustServerCertificate=true`; Mongo adds
  `_AUTHENTICATIONDATABASE` and `_AUTHENTICATIONMECHANISM`), and Redis emits five
  with `CACHE_URI = {cache.bindings.tcp.scheme}://:{cache-password.value}@…` — the
  scheme coming from the binding, not from the connection string.
  The grading consequence: `AddConnectionString("ordersdb",
  ReferenceExpression.Create($"{pg.Resource};Database=orders_v2"))` renders the
  byte-identical connection string (the mutant ex014 already documents) and its
  consumer receives **exactly one** variable. ex027 was re-scoped onto this, because
  its original spec — `AddPostgres().AddDatabase()` plus
  `{pg.connectionString};Database=orders` — was already covered assertion-for-
  assertion by ex001 and ex014.
- **MongoDB's password is published through a URI-encoding filter; nobody else's
  is.** Measured on 13.5.3. In `aspire-manifest.json` the Mongo connection strings
  interpolate `{docs-password-uri-encoded.value}`, not `{docs-password.value}`, and
  the manifest gains a resource `docs-password-uri-encoded` of type
  **`annotated.string`** with `"filter": "uri"` and `"value":
  "{docs-password.value}"` — a manifest resource type nothing else in this track
  produces. It exists because the password sits in a URI's userinfo rather than in a
  `Password=` clause. Note the model and the manifest therefore **disagree** on
  Mongo's connection-string expression, which is why ex028 pins the model form at L1
  and the encoded form at L2 rather than asserting one string in both places.
- **The three admin-console helpers add a sibling resource and link it with a
  `ResourceRelationshipAnnotation`, and all three differ from each other.** Measured
  on 13.5.3 for row 030:

  | call | resource added | type | relationship `Type` |
  |---|---|---|---|
  | `AddPostgres("pg").WithPgAdmin()` | `pgadmin` | `Aspire.Hosting.Postgres.PgAdminContainerResource` | `"PgAdmin"` |
  | `AddMongoDB("docs").WithMongoExpress()` | `docs-mongoexpress` | `Aspire.Hosting.MongoDB.MongoExpressContainerResource` | `"Parent"` |
  | `AddRedis("cache").WithRedisInsight()` | `redisinsight` | `Aspire.Hosting.Redis.RedisInsightResource` | `"RedisInsight"` |

  None of the three is `IResourceWithParent` — the link is a relationship, not
  parenting — and the store carries no annotation pointing back, so the link is
  one-way. pgAdmin and RedisInsight are **singletons**: two Postgres servers each
  calling `WithPgAdmin()` still yield exactly one `pgadmin`, and measured, it holds a
  relationship to the **first** server only, so do not build a row on the
  two-server case. Mongo Express is per-server and named after its parent. All three
  are present in the **publish-mode model** and absent from `aspire-manifest.json`:
  the helpers call `ExcludeFromManifest` for you rather than branching on
  `IsRunMode`. The relationship `Type` strings are undocumented — ex030 pins them
  and is the tripwire, the same stance ex004 takes on health-check keys.
- **`WithDataVolume(name, …)` and `WithVolume(name, target)` are byte-identical in the
  model, so a flavour-aware helper cannot be graded by the mount it writes.** Measured
  on 13.5.3 while writing ex031: `AddPostgres("pg").WithVolume("pgdata",
  "/var/lib/postgresql")` and `AddPostgres("pg").WithDataVolume("pgdata")` produce
  `ContainerMountAnnotation`s equal in `Type`, `Source`, `Target` and `IsReadOnly`, and
  the resource's annotation list is otherwise identical. An implementation that looks
  all five container paths up once and types them therefore passes every path
  assertion — which it should, because it produces the same container; the thing it has
  not done is use the API the row is about. The **one** observable difference is the
  name the *anonymous* overload generates:
  `fewolearning.microservices.tests-549a2a9f7b-pgauto-data` in the red run and
  `…-9a0e1cce07-…` in the green one, the hash being derived from the AppHost. ex031
  carries a sixth server, `pgauto`, purely so that one resource's volume name is
  something no learner can commit, and grades its shape. Any future row about a
  `With<Something>Volume` helper needs the same trick or it grades a path, not a call.
- **The Postgres data directory depends on the image TAG, and the call order decides
  which one you get.** Measured: `WithDataVolume` reads the tag configured *at the
  moment it runs*. On the default tag (18.3) it lands on `/var/lib/postgresql`; on
  `17.5` it lands on `/var/lib/postgresql/data` — the path every tutorial quotes.
  `WithImageTag("17.5")` **then** `WithDataVolume(…)` gives the 17 path;
  `WithDataVolume(…)` **then** `WithImageTag("17.5")` gives the 18 path on a 17 image,
  silently, and the container initdb's into an empty directory beside the real one on
  every run. Nothing warns. Mongo is `/data/db` and SQL Server `/var/opt/mssql`.
- **SQL Server's two data helpers are asymmetric: one mount versus three.**
  `AddSqlServer("x").WithDataVolume("d")` writes a single volume at `/var/opt/mssql`;
  `AddSqlServer("x").WithDataBindMount("./d")` writes **three** bind mounts, at
  `/var/opt/mssql/data`, `/log` and `/secrets`. A host directory cannot be mounted over
  the whole of `/var/opt/mssql` without hiding the server binaries, so the helper splits
  it. The obvious hand-rolled equivalent — `WithBindMount("./d", "/var/opt/mssql")` —
  produces one mount, a container that will not start, and a model that reads correctly.
- **`WithInitFiles` and `WithInitBindMount` are two different mechanisms, not two
  spellings.** Measured while writing ex032. `WithInitFiles(dir)` writes a
  **`ContainerFileSystemCallbackAnnotation`** (`DestinationPath`
  `/docker-entrypoint-initdb.d`) whose callback enumerates the source folder when
  invoked and yields one `ContainerFile` per script — so a test can *run* the callback
  and discover whether the folder pointed at actually contained anything, which is the
  only way to reject an implementation aimed at an empty directory. Each `ContainerFile`
  carries `SourcePath` and leaves `Contents` **null**: the bytes are streamed at run
  time, not captured at model time, so assert the path and never the SQL.
  `WithInitBindMount(dir)` is `[Obsolete]` and writes an ordinary
  `ContainerMountAnnotation` instead — **`IsReadOnly` true**, unlike the generic
  `WithBindMount`, which defaults to false. `WithInitFiles` also validates its source at
  model-build time, throwing `InvalidOperationException` from inside `Configure` and
  naming the absolute path, so a relative literal is a hard failure rather than a silent
  no-op — and `builder.AppHostDirectory` is three different places (§5), so exercises
  point both helpers at `Path.Combine(AppContext.BaseDirectory, "initdb")` and ship the
  scripts as `Content` from both content libraries.
- **A bare integration resource carries no mount and no file-system callback.**
  Measured on 13.5.3 for `AddPostgres`, `AddSqlServer` and `AddMongoDB`: each arrives
  with `EndpointAnnotation`, `ContainerImageAnnotation`, `ResourceIconAnnotation`, some
  `Environment*` annotations and a `HealthCheckAnnotation` — and **zero**
  `ContainerMountAnnotation` and **zero** `ContainerFileSystemCallbackAnnotation`. So
  unlike the health-check case above, "this resource has no mounts" and "this resource
  has no file callback" are real statements about an answer, and ex031/ex032 grade both
  directions with them.
- **Npgsql strips the password out of `NpgsqlDataSource.ConnectionString`, so a
  sentinel has to live elsewhere in the string.** Measured on Npgsql 10.0.2 while
  writing ex033. `Application Name` survives normalisation and nothing else writes it,
  which makes it the place to put a value the test invented microseconds ago.
  Two more things that decide what a client-integration row can grade: the failure for
  a **missing** `ConnectionStrings:<name>` key is thrown when the data source is
  *resolved*, not when it is registered — `AddNpgsqlDataSource` returns happily against
  an empty configuration — so the negative half has to ask the container for the
  service; and the integration registers `NpgsqlConnection`, `DbDataSource` and
  `DbConnection` alongside the data source plus a health-check registration named
  `PostgreSql`, none of which a hand-rolled
  `AddSingleton(NpgsqlDataSource.Create(config.GetConnectionString(…)!))` produces.
  That hand-rolled version also throws `ArgumentNullException` ("Parameter 'Host'")
  rather than an `InvalidOperationException` naming the key. The sharpest mutant in the
  row is neither of those: `AddNpgsqlDataSource(name, s => s.ConnectionString ??=
  "Host=localhost;…")` — the real integration with a "safe" local fallback bolted on —
  passes the sentinel fact and the health-check fact and dies **only** on the
  missing-key fact. Any row prescribing a sentinel needs the missing-key half too.
- **`NU1603` silently upgrades the test runner.** `xunit.runner.visualstudio` has **no
  3.1.6 and no 3.1.7** — 3.1.5 is the last 3.x and the next version is 4.0.0. Naming a
  3.x that does not exist does not fail the build: NuGet resolves *forward* to 4.0.0 with
  only an `NU1603` warning, quietly landing the project on the runner generation this
  track is avoiding. Treat `NU1603` here as an error, not noise.

- **What a bare host builder already registers, for anyone grading an
  `IServiceCollection`.** Measured on .NET 10.0.400 while writing ex021:
  `Host.CreateApplicationBuilder()` arrives with **52 descriptors over 42 distinct
  service types**, and `WebApplication.CreateBuilder()` with **117 over 95**. None of
  `TracerProvider`, `MeterProvider`, `HealthCheckService`, `ServiceEndpointResolver`
  or `IHttpClientFactory` is among them, so all five are honest assertion targets.
  What *is* free, and therefore grades nothing: `IMeterFactory` on the plain host
  builder, plus `ActivitySource`, `DiagnosticListener` and
  `DistributedContextPropagator` on the web one. ex021 measures the bare builder inside
  its own first fact rather than beside it, so the guard cannot rot.
- **Two service-discovery spellings register the same types; only the options tell them
  apart.** `services.AddServiceDiscovery()` and
  `ConfigureHttpClientDefaults(http => http.AddServiceDiscovery())` both leave a
  `ServiceEndpointResolver` and an `IServiceDiscoveryHttpMessageHandlerFactory`, so
  neither type discriminates. Measured on `Microsoft.Extensions.ServiceDiscovery`
  10.9.0, what does:
  `IOptionsMonitor<HttpClientFactoryOptions>.Get(<any name at all>)
  .HttpMessageHandlerBuilderActions` is **2** for a full ServiceDefaults, **1** if only
  one of the two handlers is on the defaults, and **0** for
  `AddHttpClient("catalog").AddStandardResilienceHandler().AddServiceDiscovery()`, since
  a named client configures only its own name. ex021 asks about a client name nobody
  ever mentioned, for exactly this reason. Separately, the *standard* resilience handler
  is the only one that registers `IValidateOptions<HttpStandardResilienceOptions>`; a
  hand-rolled `AddResilienceHandler("...", p => p.AddRetry(...))` registers none and is
  otherwise indistinguishable.
- **An unregistered `ActivitySource` does not merely go unexported - `StartActivity`
  returns `null`.** Measured on OpenTelemetry 1.18.0. That makes ex022's negative fact
  sharp rather than decorative, and it is also the only thing that rejects
  `AddSource("*")`: under a wildcard the same call returns a real `Activity`, and the
  matching `AddMeter("*")` additionally drags in 18 `System.Runtime` metrics nobody
  asked for.
- **A bare `AddContainer` carries no `ResourceCommandAnnotation`.** Measured on 13.5.3.
  Unlike `HealthCheckAnnotation` on an integration resource (above), nothing is there
  for free - the start/stop/restart buttons the dashboard shows for every resource are
  not model annotations. So a command row *can* grade the annotation's presence. What it
  cannot grade is the presence of `UpdateState`: `WithCommand` supplies a default that
  always answers `Enabled`, so the property is never null, and only **calling** it with
  two different `CustomResourceSnapshot`s separates "reads the resource" from "ignores
  the resource". ex024 calls it three times - `Running`, `Exited`, and no state at all.
- **Publishing `BeforeStartEvent` in-process needs two DCP configuration keys, or
  Aspire's own subscriber throws first.** Measured while writing ex025:
  `BeforeStartEvent` carries a built-in subscription, `InitializeDcpAnnotations`, which
  reads the DCP options and dies with `OptionsValidationException` ("The path to the DCP
  executable ... is required") **before** any learner subscription is reached. Setting
  `DcpPublisher:CliPath` and `DcpPublisher:DashboardPath` on `builder.Configuration` to
  any non-empty placeholder fixes it; nothing ever executes them, because nothing is
  started. That is what `tests/_support/EventingHarness.cs` does, and it is why that
  harness exists rather than a `ModelHarness` overload - `ModelHarness` disposes the app
  before it returns, and an eventing test needs the app's service provider alive.
- **`builder.Eventing` and the app's `IDistributedApplicationEventing` are the same
  instance**, so a subscription made while the graph was assembled is live on the built
  app and a test can publish lifecycle events by hand. Measured, and it is what makes an
  eventing row deterministic with no wall clock: the test *is* the orchestrator. Measured
  too, and the mutant ex025 exists to reject: `Subscribe<T>(handler)` - the app-scoped
  overload - fires for **every** resource when used with a resource-scoped event such as
  `ResourceReadyEvent`, while `Subscribe<T>(resource, handler)` fires only for that one.
  Both compile, both pass every positive assertion, and only publishing the event for a
  resource nobody subscribed to tells them apart.

- **The test assembly runs SERIALLY, and that is load-bearing.**
  `tests/_support/TestParallelism.cs` carries
  `[assembly: CollectionBehavior(DisableTestParallelization = true)]`. Not caution and not
  a performance knob: three exercises grade **process-global** state that cannot be
  isolated by construction — ex022's static `ActivitySource`/`Meter` plus an OpenTelemetry
  `ActivityListener` that is installed process-wide, ex023's two scenario flags, ex025's
  ordered hook log. ex022's negative fact in particular asserts that an *unregistered*
  source yields a **null** `Activity`, which stops being true the instant any other class
  anywhere in the assembly has a `TracerProvider` listening to `"*"`. Under the default
  class-level parallelism those three are correct only while nothing else happens to touch
  the same statics — an assembly-wide invariant that nothing stated and nothing enforced,
  with 75 rows still to be written against it. **The spelling is version-specific.** On
  xunit.v3 3.2.2 `CollectionBehaviorAttribute` is the supported form and is not obsolete
  (verified by reflecting over `xunit.v3.core` 3.2.2:
  `AttributeTargets.Assembly`, settable `DisableTestParallelization`). Do **not** copy
  `wpf/`'s `[assembly: Parallelization(Mode = ParallelMode.None)]` — that is the xunit.v3
  **4.0.0** spelling, and in 4.0.0 this attribute is `Obsolete(error: true)`; the two
  tracks are on different generations on purpose (§7). **Measured cost**, back to back on the
  same tree with only this attribute commented out and back in: the green run goes from
  **26 s to 1 m 8 s** (~2.6x - it is the run where all 87 reference solutions actually
  execute), while the red run is unchanged within noise (19 s against 17 s, because a
  stub throws before it does any work). Worth re-measuring as the catalog fills: if the
  green run ever becomes the bottleneck, the answer is a `[Collection]` per group of
  globally-stateful rows rather than turning this off.
  **Where the green run's time actually goes — profiled 2026-09-07, and it is not where
  it looks.** At 35 delivered rows the green run is 1 m 30 s, up from 26 s at ex025, and
  the obvious explanation (more rows, and rows that build real host builders) is wrong.
  Per-test durations from a `--logger trx` run: **16 of the 120 facts — 13% — account
  for 87.3 s of the 90.2 s of test time. The other 104 facts together take 2.8 s.** Twelve of those
  sixteen have `manifest`, `publish` or `bicep` in their own name, and the rest are the
  same shape. Every one of them is an **L2 in-process publish**, which §6 already
  measures at ~3.7 s (~7.5 s with Azure resources) — a fixed per-*fact* cost, paid once
  per fact and not shared. The whole of rows 031-035, fifteen facts including three
  `Host.CreateApplicationBuilder()` calls, contributes **0.20 s**. So the green run
  tracks the number of **publish-shaped facts**, not the number of rows, and it grows by
  roughly 5 s each time one is added — while the red run stays flat, because a stub
  throws before it reaches the publish. Anyone budgeting a future batch should count its
  L2 facts and ignore everything else; and if this ever does need fixing, the lever is
  sharing one publish across the facts that assert on it, not parallelism.
  **Confirmed by the next batch.** Rows 036-040 added sixteen facts, of which **zero**
  are publish-shaped — five build a resource graph, eight build a `DbContext` or a
  generated script offline, three are 🐳. The green run went from 1 m 30 s to
  **1 m 24 s**, i.e. nowhere, exactly as the model predicts. The container lane is a
  separate budget and grew by ~2 m 43 s; see §3.
- **A count of `HttpMessageHandlerBuilderActions` says how many handlers, never which.**
  Measured while closing a review finding on ex021:
  `ConfigureHttpClientDefaults(h => { h.AddStandardResilienceHandler();
  h.AddHttpMessageHandler(...); })` beside a bare `services.AddServiceDiscovery()` leaves
  **two** actions for every client name and no service-discovery handler within reach of
  any `HttpClient` — so `https+http://catalog` never resolves, while every
  descriptor-level assertion passes. The honest grade is the built chain:
  `IHttpMessageHandlerFactory.CreateHandler(name)` and then walking
  `DelegatingHandler.InnerHandler`. Measured chain for a correct ServiceDefaults, on a
  client name nobody registered:
  `LifetimeTrackingHttpMessageHandler → LoggingScopeHttpMessageHandler →
  Resilience.ResilienceHandler → ServiceDiscovery.Http.ResolvingHttpDelegatingHandler →
  LoggingHttpMessageHandler → SocketsHttpHandler`. Both interesting handler types are
  `internal` and cannot be named from a test, so assert the **assembly** each handler came
  from (`Microsoft.Extensions.Http.Resilience`, `Microsoft.Extensions.ServiceDiscovery`) —
  stable across a rename, and precise. Note what the chain does *not* separate: a
  hand-rolled `AddResilienceHandler(...)` produces the same `ResilienceHandler` type, so
  standard-versus-custom still needs the `IValidateOptions<HttpStandardResilienceOptions>`
  descriptor.
- **A tag-filtered health endpoint cannot be graded by status codes alone.** Measured on
  ex023, and it is the sharpest thing in this batch: an `/alive` mapped with
  `Predicate = r => r.Name is "self" or "event-loop"` — names, not tags, with all the
  registrations' tags spelt correctly — passes *every* scenario the row asserts and *every*
  assertion about the registrations. The whole point of a tag is the check that does not
  exist yet, so the only way to grade it is to create one: after the learner's
  configuration has run, the **test** registers one more `"live"`-tagged check, unhealthy
  and named nothing the exercise mentions, and requires `/alive` to report 503. A
  name-based predicate cannot see it; a tag-based one has no choice. Any future row about
  filtering by metadata needs the same shape — add an item the implementation could not
  have enumerated.
- **An event's `Services` and the builder's own `ExecutionContext` agree in every normal
  run, so "read it off the event" needs an abnormal one.** Measured on ex025: a hook
  written `Record(builder.ExecutionContext.IsPublishMode ? … : …)` — closing over the
  builder instead of resolving `DistributedApplicationExecutionContext` from
  `@event.Services` — answers correctly under both a run-mode and a publish-mode
  application, so building the model twice does not separate the two. Handing a **run-mode**
  application a `BeforeStartEvent` whose service provider reports publish mode does: the
  event is the authority. A one-type-delegating `IServiceProvider` is enough, and Aspire's
  own built-in `BeforeStartEvent` subscriber is unaffected because everything else still
  resolves from the real provider.

- **EF caches the built model per (context type, provider), so a parameterised model
  silently returns the first answer forever.** Measured while writing ex037, and it is
  the reason that row grades anything at all. The row asks for the same `DbContext`'s
  CREATE script under two providers plus a **schema the test invented**, so that a
  hand-written pair of SQL strings can be rejected; the first version read the schema
  from a field, and the second call came back with the *first* call's script, schema and
  all. The fix is EF's own hook — `ReplaceService<IModelCacheKeyFactory, …>` returning a
  key that includes the varying value — and it belongs in the exercise as declared
  scaffolding, not as part of the TODO. Any future row that varies a model at runtime
  needs it. Note what removing it from ex037 would *not* be: silent. Facts 3 and 4 sit in
  one class and share one process-wide model cache, so whichever ran second would read
  the other's script and go red on a string comparison. It is a self-announcing
  dependency, which is why it needs a comment and not a guard.
- **The two providers escape their own delimiter and leave the other's alone, and that
  is the sharpest anti-hardcode hook this batch found.** Measured on EF Core 10.0.11 and
  `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3: a default schema named `a]b"c` comes
  out of SQL Server as `[a]]b"c]` (bracket doubled, quote untouched) and out of Npgsql as
  `"a]b""c"` (quote doubled, bracket untouched). A `$"[{schema}]"` template produces
  `[a]b"c]` and fails. This matters because the *plausible* mutant for a "compare the
  generated SQL" row is a pair of hand-written scripts transcribed from a real run —
  which passes every `nvarchar`/`text`/`datetime2`/`GENERATED … AS IDENTITY` assertion.
  It was built and run, and only the escaping fact rejected it. Two more measured
  spellings from the same probe: Npgsql omits the quotes entirely for an
  all-lowercase schema (`CREATE SCHEMA ex037_plain`) and wraps schema creation in a
  `DO $EF$ … END $EF$;` block, where SQL Server writes
  `IF SCHEMA_ID(N'…') IS NULL EXEC(N'CREATE SCHEMA […];');` followed by `GO`. SQL Server
  batches with `GO`; Npgsql emits none, so `GO` is itself a provider discriminator.
- **`EnsureCreated()` on an existing but empty database DOES create the tables.**
  Relevant because Aspire's `AddDatabase` has already created the database by the time a
  🐳 test connects, and the obvious worry — "EnsureCreated will see the database and do
  nothing" — is wrong on EF Core 10: it falls through to a `HasTables()` check and
  creates them. ex040's harness relies on that. What `EnsureCreated` still does *not* do
  is write `__EFMigrationsHistory`, which is exactly what ex038 grades.
- **Inserting an explicit value into an `IDENTITY` column fails, so a 🐳 test cannot
  choose its own primary keys.** Measured in ex040's first container run: seeding
  `new Account { Id = 1, … }` through EF dies inside `SaveChangesAsync`. Let the database
  generate the ids and read them back off the tracked entities.
- **Once `EnableRetryOnFailure` is on, `BeginTransactionAsync` on its own THROWS**, with
  *"The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not
  support user-initiated transactions. Use the execution strategy returned by
  'DbContext.Database.CreateExecutionStrategy()' …"*. Measured. This is a grading gift:
  "did the learner run the transaction through the execution strategy" needs no clever
  assertion, because without it the operation cannot start. It also means the *offline*
  half must assert `CreateExecutionStrategy().RetriesOnFailure` — the default
  `SqlServerExecutionStrategy` returns false and happily allows the un-wrapped shape, so
  a learner who skipped `EnableRetryOnFailure` would otherwise sail through the container
  fact.
- **A concurrency test needs no threads and no clock — it needs a seam.** ex040 makes the
  interleaving an *ordering*: `AddToBalanceAsync` takes an `afterLoad` callback invoked
  while it is holding a stale copy, and the test's callback is a second, complete
  `AddToBalanceAsync`. The outer save is then guaranteed to be writing against a row
  version that no longer exists, in a single-threaded program with no `Task.Delay`
  anywhere. `TransferAsync` takes the same kind of seam (`betweenSaves`) between its two
  saves. Any future row about ordering — outbox, saga compensation, cache stampede —
  should buy determinism the same way rather than with a sleep.
- **A rolled-back write is invisible afterwards, so proving the transaction was real
  needs a DIRTY read.** The mutant that does one `SaveChanges` for both sides of a
  transfer is genuinely atomic (EF wraps it implicitly) and passes both "the money moved"
  and "nothing survived the failure". What separates it from the correct two-save,
  one-transaction answer is what a *second connection* sees while the transaction is
  still open: `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED` inside the seam reads
  the debited balance from the correct answer and the untouched one from the mutant.
  Both were built and run.
- **`DbContext` and its entities nest inside the exercise's static class.** Five rows in
  this batch each want a `CatalogContext` and a `Product`, in one namespace, in two
  assemblies that compile the same type names. Nesting them inside
  `Ex0NN_Something` makes that free, costs nothing at the call site
  (`using static …Ex0NN_Something;` in the test), and keeps a row self-contained the way
  every other row in the track is.

- **An in-process publish spent 99% of its time asking Docker whether it existed.**
  The single biggest measurement in this track so far, and it was hiding in plain sight
  behind "a publish costs ~5 s". Traced with a logging provider attached to the publish
  builder: `ManifestPublisher` writes `aspire-manifest.json` at **36 ms**, and the
  remaining **~4.5 s** is `Aspire.Hosting.ContainerRuntime` auto-detection — probing
  `podman` (~140 ms, not on PATH), then `docker version` (~2.4 s) and
  `docker container ls -n 1` (~1 s). It is unconditional: an **empty** model costs the
  same 5 s as one with five databases, so the cost is per *publish*, never per resource.
  Setting `builder.Configuration["ASPIRE_CONTAINER_RUNTIME"]` to a name that is not a
  real runtime skips detection entirely. Measured, same model, back to back:
  `<none>` 5262/5020 ms · `"docker"` 951/1094 ms · `"podman"` 141/333 ms ·
  `"none"` 86/129 ms — and the manifest is identical in every case.
  `ManifestHarness.NoContainerRuntime` is now the default for `GenerateAsync`,
  `SharedPublishAsync` and `PublishAsync`; the green run went from **1 m 24 s to 10 s**
  and the red run from 20 s to 5 s. Two things a later author must know: the key is set
  on **that builder's configuration**, never on the process environment, so it cannot
  reach `ContainerHarness`, which runs in *run* mode and needs a real Docker; and a
  future row that genuinely needs image building can pass `containerRuntime: "docker"`
  and pay the ~1 s probe. It also makes §2's promise — "the default `dotnet test` needs
  no daemon at all" — true rather than nearly true: before this, the fast loop shelled
  out to `docker` sixteen times.
- **Because a publish costs a flat rate, the only L2 quantity worth managing is how many
  happen.** `ManifestHarness.SharedPublishAsync` publishes once per distinct model and
  hands the same output to every fact that asks; `GenerateAsync` sits on top of it and
  still returns a **fresh `JsonDocument` per call**, which is what keeps every existing
  `using var manifest = await GenerateAsync(...)` correct — the caller disposes its own
  document, never the shared output. The cache keys on the delegate's **identity**
  (method plus target) plus the container-runtime choice, so a method group such as
  `Ex013_X.Configure` shares across facts and classes.
  `ManifestHarness_publishes_ONCE_per_model_however_many_facts_ask` watches the publish
  counter directly, because a cache that quietly stopped caching would show up nowhere
  until someone profiled the suite again. Shared outputs are deleted by `HarnessLifetime`
  after the last test; the hourly sweep in the static constructor remains the backstop.
- **The sharing rule is NOT "closures are the dangerous ones" — it is the opposite, and
  the first wording of it here was wrong in the dangerous direction.** A *closure* over
  mutable state is harmless: each closure instance is a different `Target`, so it misses
  the cache and pays for its own publish. What is **not** harmless is a **static
  `Configure` that reads static mutable state** — its key is a stable
  `(MethodInfo, null)`, so the second call **hits** and would be handed the first call's
  manifest with nothing said. That is not a hypothetical pattern in this assembly:
  `tests/_support/TestParallelism.cs` names ex023's two static scenario flags and ex025's
  static hook log as the reason the whole assembly runs serially, and sixty rows are still
  to be written against the same freedom. **The rule: a static `Configure` reading static
  mutable state must call `PublishAsync`, which shares nothing, not `GenerateAsync` or
  `SharedPublishAsync`.**
  It is **enforced, not just written down** — for the part that can be enforced cheaply.
  Every cache *hit* rebuilds the model (in publish mode, and under the same
  `ASPIRE_CONTAINER_RUNTIME`, so a `Configure` branching on either is compared like for
  like) and fingerprints it against what was published; a model that changed throws an
  `InvalidOperationException` naming the delegate and pointing at `PublishAsync`.
  **What the fingerprint covers, precisely:** the set of resources and their runtime
  types, their connection-string expressions, which annotations each carries, and every
  annotation property whose type is a string, a primitive, an enum or an `IResource` —
  so `WithImageTag`/`WithImageRegistry`/`WithImageSHA256`, endpoint ports and schemes and
  `IsExternal`/`IsProxied`, mount source/target/type/read-only, `WithReplicas`,
  `WithLifetime`, `WaitAnnotation`'s type and exit code, health-check keys. It is a
  whitelist of value *kinds*, not of annotation *types*, so it covers annotations nobody
  has written yet.
  **What it does NOT cover, stated because the first version of this paragraph
  overclaimed:** a value computed inside a **callback**. Measured —
  `WithEnvironment("MODE", flag ? "a" : "b")` writes an internal `EnvironmentAnnotation`
  whose only public member is a `Func<>`, and `WithArgs` is the same shape — so two models
  differing only there fingerprint identically and the stale manifest comes back. Closing
  that would mean the guard **invoking learner-authored callbacks**, which is a side
  effect a safety net has no business causing: an ex007-shaped row that counted callback
  invocations would be corrupted by the very thing protecting it. **So the RULE above is
  the protection, and the guard is a net under its structural half.**
  Two properties worth knowing: the rebuild costs a model build — ~26 ms warm against a
  ~100 ms publish, of which the fingerprint itself is ~0.18 ms, so the value half is free
  — and `configure` is invoked exactly **once per call** either way, because the publish
  captures its fingerprint from its own builder rather than from a second one. That is the
  count it had before the cache existed, so nothing that was safe before became unsafe.
  `ManifestHarness_REFUSES_to_share_a_stale_publish_with_a_changed_model` is the mutant
  made permanent, twice: a static `Configure` over a static flag with a **structural**
  difference, then one with a **value-only** difference (an image tag). Both were measured
  slipping through before they were closed — with the guard commented out, and then with
  the fingerprint reverted to annotation type names only, the fact fails on a null
  exception because the stale manifest came back. The canary is not decorative.
- **`[assembly: AssemblyFixture(...)]` is the only hook in this suite that runs after the
  last test, and it runs even under `--filter`.** Measured on xunit.v3 3.2.2 with
  `xunit.runner.visualstudio` 3.1.5 by having a probe fixture append to a file:
  `InitializeAsync` ran before the (single, filtered) test and `DisposeAsync` after it.
  That is what the shared container servers and the shared publish outputs are torn down
  by. The corollary is the trap: the fixture is constructed **eagerly**, so anything it
  does in `InitializeAsync` happens in the default `dotnet test` as well —
  `HarnessLifetime.InitializeAsync` is therefore empty on purpose, and must stay that
  way. `Xunit.v3.ITestPipelineStartup` also exists in 3.2.2 and would work; the fixture
  was chosen because it is core xunit rather than runner-adjacent.
- **`AddProject` contributes SEVEN environment variables before anything is referenced,
  so `Assert.NotEmpty(...OfType<EnvironmentCallbackAnnotation>())` grades nothing.**
  Measured while fixing ex038's first fact, which shipped with exactly that assertion and
  passed against a migrator carrying `WaitFor` and no `WithReference` at all. A bare
  project resource already carries `OTEL_SERVICE_NAME`, `OTEL_EXPORTER_OTLP_ENDPOINT`,
  `OTEL_EXPORTER_OTLP_PROTOCOL`, `OTEL_RESOURCE_ATTRIBUTES`,
  `OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY`, `LOGGING__CONSOLE__FORMATTERNAME` and
  `DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION`. Grade the **variable**, not the
  annotation: run the callbacks through an `EnvironmentCallbackContext` (ex007's
  technique) and read `ConnectionStrings__<name>` out of the dictionary. One more
  measured detail decides how to assert it — at model time the value is **not a string**
  but a deferred `IManifestExpressionProvider` whose `ValueExpression` is
  `{catalog.connectionString}`, so `.ToString()` gives you a type name, and a
  hand-written `WithEnvironment("ConnectionStrings__catalog", "Server=…")` lands a plain
  `System.String` there and fails the cast. Both mutants were built and run.

## 7. Pinned versions

| Package | Version | Where |
|---|---|---|
| `Aspire.Hosting` + all `Aspire.Hosting.*` integrations | 13.5.3 | `exercises/` + `solutions/` |
| `Aspire.Hosting.Elasticsearch` | **13.3.0** | `exercises/` + `solutions/`, *when row 051 lands* |
| `Microsoft.Extensions.ServiceDiscovery` | 10.9.0 | `exercises/` + `solutions/` |
| `Microsoft.Extensions.Http.Resilience` | 10.9.0 | `exercises/` + `solutions/` |
| `OpenTelemetry.*` (`.Extensions.Hosting`, `.Instrumentation.AspNetCore` / `.Http` / `.Runtime`, `.Exporter.OpenTelemetryProtocol`) | 1.18.0 | `exercises/` + `solutions/` |
| `Aspire.Npgsql` | 13.5.3 | `exercises/` + `solutions/` |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.11 | `exercises/` + `solutions/` |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.3 | `exercises/` + `solutions/` |
| `Aspire.Hosting.Testing` | 13.5.3 | `tests/` |
| `Aspire.Hosting.AppHost` + `Aspire.Hosting.Orchestration.$(NETCoreSdkRuntimeIdentifier)` | 13.5.3 | `tests/`, **only** under `Condition="'$(Containers)' == 'true'"` |
| `Npgsql` | 10.0.3 | `tests/` |
| `Microsoft.EntityFrameworkCore.Sqlite` | 10.0.11 | `tests/` |
| `OpenTelemetry.Exporter.InMemory` | 1.18.0 | `tests/` |
| `Microsoft.AspNetCore.TestHost` | 10.0.11 | `tests/` |
| `xunit.v3` | 3.2.2 | `tests/` |
| `xunit.runner.visualstudio` | 3.1.5 | `tests/` |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | `tests/` |

This table is the **pinning policy**, not an inventory: a package is added to the two
content libraries when the first row needing it is written. Referenced today:
`Aspire.Hosting`, `.PostgreSQL`, `.SqlServer`, `.MongoDB`, `.Redis`,
`.Azure.AppContainers`, `.Azure.Storage` and the client-side `Aspire.Npgsql` — the last two because the harness's Bicep
fact needs them and the Azure rows will — plus the non-Aspire service-side set rows
021-023 added, and a `FrameworkReference` to `Microsoft.AspNetCore.App` (see §5).
`Aspire.Npgsql` arrived with ex033/ex034 — the first rows that cross from the AppHost
into a **service** — and went into both content libraries at 13.5.3 like every other
Aspire package. `Npgsql` in `tests/` is not an independent choice: ex034's L3
fact seeds a row through its own connection before asking the learner's code to read it
back, so it must be exactly what the two content libraries resolve. That was 10.0.2
until rows 036-040 added `Npgsql.EntityFrameworkCore.PostgreSQL`, which pins
`Npgsql` **10.0.3**; the `tests/` pin moved with it in the same commit, and must keep
moving with it. The **two EF Core providers** are the second structural extension the
content libraries have taken (§5), on the same terms as the ASP.NET Core set: both
`.csproj` files, identically, one commit. `Microsoft.EntityFrameworkCore.Sqlite` is
test equipment for ex039's offline re-run check and lives in `tests/` alone.

The two DCP packages are the track's only **conditional** references. They exist because
starting a real `DistributedApplication` needs the orchestrator that `Aspire.AppHost.Sdk`
normally supplies, and `tests/` is a plain test project. Two things not to do to them:
do **not** set `IsAspireHost=true` "to make it an AppHost" — the .NET 10 SDK's
`_CheckForAspireWorkloadDeprecation` then fires without the `AspireHostingSDKVersion`
only that SDK sets, and the build dies `NETSDK1228`; and do **not** make them
unconditional, because the whole point is that the default `dotnet test` restores and
builds exactly what it did before ex034 landed. The cost of the condition is documented
in §3: `FEWO_MS_CONTAINERS=1` alone can no longer run a 🐳 row.

Those non-Aspire versions are chosen for currency and coherence rather than pinned to
Aspire: OpenTelemetry ships as one release train, so all five of its packages sit on the
same 1.18.0 (the InMemory exporter in `tests/` included, since it shares the SDK's
internals); `Microsoft.Extensions.ServiceDiscovery` and
`Microsoft.Extensions.Http.Resilience` ship together out of `dotnet/extensions` and are
both 10.9.0; and `Microsoft.AspNetCore.TestHost` is 10.0.11 to match the installed
`Microsoft.AspNetCore.App` 10.0.11 exactly, because it substitutes for the server. Whatever is added next goes into **both**
`.csproj` files identically, at 13.5.3, or the two libraries stop being interchangeable.

### The Elasticsearch version lag is deliberate

`Aspire.Hosting.Elasticsearch`'s latest stable is **13.3.0** while every sibling
integration is at 13.5.3. It is pinned at 13.3.0 on purpose. It has not been silently
bumped to a version that does not exist, and the Elasticsearch row (051) has not been
silently dropped to make the version table tidy. If a 13.5.x ever ships, bumping it is a
one-line change — until then this asymmetry is expected, and a reviewer seeing it should
read this paragraph rather than "fix" it.

### Why xunit.v3 4.0.0 plus an MTP `global.json` is forbidden here

`dotnet test` must work, because that is the command every other track documents. Two
configurations were measured:

| Configuration | Result |
|---|---|
| xunit.v3 **4.0.0** + `global.json` `{"test":{"runner":"Microsoft.Testing.Platform"}}` | ❌ exit code 5, zero tests discovered |
| xunit.v3 **3.2.2** + `xunit.runner.visualstudio` **3.1.5**, **no** `global.json` | ✅ discovered and passed, 415 ms, no warnings |

Under the failing configuration the xunit MTP runner printed its own **usage text**: the
.NET 10 SDK's `dotnet test` bridge passes it options it does not accept. The test
*executables* run correctly in both configurations — only the `dotnet test` bridge is
affected, which is precisely the path that matters.

So this track pins **xunit.v3 3.2.2 on the classic VSTest path and ships no
`global.json`**, the same generation `avalonia/` and `caliburn/` run. **Do not add a
`global.json` to `MicroServices/`.**

## 8. DevContainer

`MicroServices/.devcontainer/devcontainer.json`, on
`mcr.microsoft.com/devcontainers/dotnet:1-10.0`. It works under Rider as well as VS
Code — JetBrains IDEs read `devcontainer.json`.

**What was actually measured**, with `@devcontainers/cli` 0.89.0 on a fresh rebuild
(`devcontainer up --remove-existing-container`) — this is the whole of the evidence, not
a summary of it:

- it builds to `{"outcome":"success"}` in **~85 seconds**, almost all of it `dotnet restore`;
- `docker ps` inside works as the non-root `vscode` user **without `sudo`**, and lists the
  host's own pre-existing containers — so the container's Docker client genuinely reaches
  the **host** daemon rather than a nested one;
- `dotnet test` inside gives **4 passed / 1 skipped**, matching the host exactly. That
  measurement predates the first exercises; the host now gives 109 failed / 8 passed /
  3 skipped (§3), and the DevContainer has not been re-measured since.

**What remains unproven, and the specific way it is likely to break.** Spec §7 set the
bar at *Aspire starting a sibling database container from inside the DevContainer*, and
that was never exercised: the track had no 🐳 exercises to run at the time, and the
default `dotnet test` is precisely the run that starts **no** containers.

`docker ps` succeeding proves the socket and the client. It proves **nothing** about the
part that is actually at risk, which is this: the host's socket is bind-mounted and the
devcontainer runs on the default bridge network, with **no host networking**. So every
container Aspire starts is a *sibling* of the devcontainer, created by the **host**
daemon, and its published ports land on the **host's** `localhost` — not on the
devcontainer's. The AppHost, running *inside* the devcontainer, then dials
`localhost:<port>` and finds nothing there. That is the classic
docker-outside-of-docker breakage, and it hits exactly what Aspire does next: the
health-check wait, `WaitFor`, and every connection string handed to a service. It
would also affect any bind-mounted path an exercise passes to a sibling container,
since those paths are resolved by the host daemon against the **host** filesystem, not
against the devcontainer's.

None of that is a reason to expect failure — Docker's `host.docker.internal`, joining
the containers to the devcontainer's own network, or `--network host` are all plausible
fixes, and Aspire may already do the right thing. It is a reason not to claim success
before someone runs it. The honest claim today is **"the DevContainer builds, reaches
the host daemon, and runs the default test suite"** — not "verified end-to-end".
**Row 034 now exists, and this is still unproven.** As of 2026-09-07 the first 🐳 row
is written and green **on the host** (§4), but it has *not* been run inside the
DevContainer — that needs a rebuild and a full restore of the conditional DCP packages,
and it was left to whoever next opens the container rather than claimed untested. The
command is `dotnet test -p:Containers=true --filter …Ex034_`. If it passes, upgrade this
section to the §7 bar; if it fails on `localhost`, record which of the fixes above
worked, because every later 🐳 row inherits it. One extra thing to check that the
paragraph above predates: the conditional `Aspire.Hosting.Orchestration.$(NETCoreSdkRuntimeIdentifier)`
reference resolves a **runtime-identifier-specific** package, so the DevContainer
restores `linux-x64` where the host restores `win-x64` — a first run in there will
download it.

### It does not use the `docker-outside-of-docker` or `node` features

Both fail on this network during the build with `NO_PUBKEY 62D54FD4003F6525` while apt
verifies a third-party repository's signature — corporate TLS interception without the
corporate root CA present inside the build environment. Ubuntu's own repos and plain
HTTPS are unaffected, and the feature's `"moby": false` option fails identically. With
no features at all the same image comes up in 12 seconds, so the CLI, the image tag and
Docker itself are all fine.

The apt-free replacement, in `postCreateCommand`, uses only plain HTTPS downloads —
no apt repo, no GPG keyserver:

- the **host Docker socket bind-mounted** directly (`/var/run/docker.sock`), plus a
  version-pinned **static `docker` client** (29.8.0) from `download.docker.com`'s
  static-binary channel. Client only: no daemon, no docker-in-docker;
- a version-pinned **static Node** (v24.20.0) tarball from `nodejs.org`, extracted into
  `/usr/local`, because the base image ships no node/npm at all;
- `npm install -g @microsoft/aspire-cli` on that npm.

Port **18888** (the Aspire dashboard) is forwarded.

### Caveat: the socket `chmod`

`postStartCommand` runs `sudo chmod 666 /var/run/docker.sock` on **every start**. The
bind-mounted socket arrives `root:root` mode `660`, so `vscode` — and Aspire, and every
exercise, all of which call `docker` without `sudo` — gets "permission denied" without
it. It has to be `postStartCommand` rather than `postCreateCommand` because the file is
the *host's*, and its mode is not owned by this container.

**This is looser than the usual alternative** of adding the user to a docker group and
leaving the socket at 660: mode 666 makes the host's Docker socket world-writable for the
lifetime of the container, and anyone who can write that socket is effectively root on
the host. It is recorded here rather than buried in a comment because a reviewer raised
it and anyone adopting this setup should decide about it consciously. On this machine it
is an accepted trade-off (`vscode` already has passwordless `sudo` from the base image,
so it introduces no new privilege boundary *for that user*); on a shared or multi-tenant
host it is not, and the group-based route should be used instead.

## 9. Rules for whoever adds the next exercise

Read `catalog.md` first — it is the work queue, not the disk. Work in **batches of
five**, per the root `CLAUDE.md`: stub + test + solution, red-check filtered to the five,
green-check with `-p:UseSolutions=true`, register each in `playground/ExerciseRegistry`,
flip exactly those five catalog rows and the `**Status:**` line, commit as
`MicroServices: exNNN–exNNN`. Full-suite runs happen once per completed tier, not per
batch.

**Use the fast paths, and know why they are fast.** Two harness decisions from
2026-09-07 are the difference between a ten-second inner loop and a ninety-second one,
and both are easy to undo by accident:

- L2 goes through `ManifestHarness.GenerateAsync` / `SharedPublishAsync`, which set
  `ASPIRE_CONTAINER_RUNTIME` to a non-runtime so the publish does not probe Docker
  (~5.1 s → ~0.1 s per fact) and publish once per distinct model rather than once per
  fact. Do not call `DistributedApplication.CreateBuilder` with `--operation publish` by
  hand in a test; you will silently pay both costs back.
  **The one exception, and know it before you write a row like ex023 or ex025: if your
  `Configure` is static and reads static mutable state — a scenario flag, a counter, a
  log — its cache key is a stable `(MethodInfo, null)` and it will HIT.** Call
  `PublishAsync`, which shares nothing. The harness detects the mistake and throws rather
  than handing back a stale manifest (§6), but the throw is a safety net, not the
  instruction.
- L3 goes through `ContainerHarness.DatabaseAsync`, which hands out a fresh database on
  one shared server per flavour. Reach for `RunAsync` only when the row grades the
  learner's own resource graph, as ex034 does. §4 has the table.

Three rules are specific to this track.

**1. Rendered connection data does not prove the mechanism.** This is the recurring bug
class here. A test asserting that "a Postgres-ish container is in the model" is satisfied
just as happily by `AddContainer("pg", "postgres")` as by `AddPostgres("pg")` — so it
grades nothing. Every persistence row must assert **both** the resource *type*
(`PostgresDatabaseResource`, `SqlServerDatabaseResource`, `MongoDBDatabaseResource`, …)
**and** the `ConnectionStringExpression`, which differs per flavour:

```
pg      :: PostgresServerResource    :: Host={pg.bindings.tcp.host};Port=…;Username=postgres;Password={pg-password.value}
orders  :: PostgresDatabaseResource  :: {pg.connectionString};Database=orders
sql     :: SqlServerServerResource   :: Server={sql.bindings.tcp.host},…;User ID=sa;…;TrustServerCertificate=true
catalog :: SqlServerDatabaseResource :: {sql.connectionString};Initial Catalog=catalog
mongo   :: MongoDBServerResource     :: mongodb://admin:{mongo-password.value}@…/?authSource=admin&authMechanism=SCRAM-SHA-256
```

The rule generalises: a `WaitFor` row asserts `WaitAnnotation`s, not that the app
started; a health-check row asserts `HealthCheckAnnotation`, not a 200 response; a
Dockerfile row asserts `dockerfile.v0`, not that an image exists.

**2. Always ask what a wrong implementation would do.** Before trusting any test, ask
whether a naive or incorrect implementation would still pass it. The distributed subject
matter makes this *more* necessary, not less: an assertion about eventual consistency
that never advances time, or about a message being consumed that never waits for
delivery, passes against almost anything. Prefer bounded polling with a deadline over
`Thread.Sleep`, and assert the *mechanism* (partition ids, `explain()` output, the
compensating event, the dedupe row) rather than only the answer.

**3. Do not guess API shapes.** Aspire 13's surface has moved considerably and tutorial
material is frequently wrong about it. The CLI ships `aspire docs search` and
`aspire docs api search --language csharp` — use them rather than inventing builder
methods, package names or overloads.
