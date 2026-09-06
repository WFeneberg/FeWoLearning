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

**Current measured state** (2026-09-06; `catalog.md` at 15 ✅ / 85 ⬜, so the fifteen
delivered exercises contribute 44 red facts):

```
dotnet test                      →  44 failed, 7 passed, 1 skipped (52 total)
dotnet test -p:UseSolutions=true →   0 failed, 51 passed, 1 skipped (52 total)
```

`-p:Containers=true` unskips the harness's container-gate fact, which then passes; no
🐳 exercise row exists yet, so it has not been re-measured since ex005.

**A correct default run is red, and that is not a broken checkout.** Forty-four
failures is exactly what an untouched tree gives: one `NotImplementedException` per
unimplemented `Configure`, plus the facts that depend on it. The 7 that pass and the 1 that skips are
the harness's own facts, which pass in *both* modes because they grade the harness
rather than an exercise. Update these numbers whenever a batch lands.

The skipped one is the harness's container-gate fact, and it is only **half** the
canary. It fails if `ContainerGate.Require()` ever stops skipping with containers off —
the mutant that would start real containers in the default run. The opposite and more
dangerous mutant, a `Require()` that *always* skips, would silently disable all 25 🐳
rows while every run still reported green; that one is caught by
`ContainerGate_Require_lets_the_test_through_when_containers_are_on`, which forces the
switch on for its own async flow only and **fails** (never skips) if the gate stays
closed. Both mutants were built and observed, not reasoned about. Keep both facts.

### `-p:Containers=true`, and the no-rebuild alternative

`-p:Containers=true` reaches the test process through a
`RuntimeHostConfigurationOption` in `tests/…csproj`, i.e. through
`runtimeconfig.json`, so it requires a build. Setting `FEWO_MS_CONTAINERS=1` in the
environment does the same thing without one.

The gate deliberately checks **only the switch**, never whether Docker is reachable.
With the switch on and no daemon the L3 tests **fail**, loudly. A broken Docker setup
must not be able to masquerade as a green run by silently skipping.

## 4. The three test levels

| Level | Asserts | Cost | Runs |
|---|---|---|---|
| **L1 model** | the resource graph: types, `ConnectionStringExpression`, annotations | ~1.4 s | always |
| **L2 artifact** | `aspire-manifest.json` **and the generated Bicep**, both in-process | ~3.7 s (~7.5 s with Azure resources), no container | always |
| **L3 container** | a real database starts and a real query, message or expiry happens | minutes | opt-in |

**What L1 can prove.** `DistributedApplication.CreateBuilder(...)` + `Build()` produces
the complete model in ~1.4 s with **zero containers started**, and that model is rich
enough to grade against: resource types, per-resource annotations (`WaitAnnotation`,
`HealthCheckAnnotation`, `ContainerImageAnnotation`, `EndpointAnnotation`,
`EnvironmentCallbackAnnotation`, `ContainerMountAnnotation`), and
`ConnectionStringExpression.ValueExpression`, which differs per database flavour. That
last one is what lets a test prove the learner wired up *PostgreSQL* rather than *some
container*. `ModelHarness` in `tests/_support/` is the entry point.

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
- **`NU1603` silently upgrades the test runner.** `xunit.runner.visualstudio` has **no
  3.1.6 and no 3.1.7** — 3.1.5 is the last 3.x and the next version is 4.0.0. Naming a
  3.x that does not exist does not fail the build: NuGet resolves *forward* to 4.0.0 with
  only an `NU1603` warning, quietly landing the project on the runner generation this
  track is avoiding. Treat `NU1603` here as an error, not noise.

## 7. Pinned versions

| Package | Version | Where |
|---|---|---|
| `Aspire.Hosting` + all `Aspire.Hosting.*` integrations | 13.5.3 | `exercises/` + `solutions/` |
| `Aspire.Hosting.Elasticsearch` | **13.3.0** | `exercises/` + `solutions/`, *when row 051 lands* |
| `Aspire.Hosting.Testing` | 13.5.3 | `tests/` |
| `xunit.v3` | 3.2.2 | `tests/` |
| `xunit.runner.visualstudio` | 3.1.5 | `tests/` |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | `tests/` |

This table is the **pinning policy**, not an inventory: a package is added to the two
content libraries when the first row needing it is written. Referenced today:
`Aspire.Hosting`, `.PostgreSQL`, `.SqlServer`, `.MongoDB`, `.Redis`,
`.Azure.AppContainers` and `.Azure.Storage` — the last two because the harness's Bicep
fact needs them and the Azure rows will. Whatever is added next goes into **both**
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
  measurement predates the first exercises; the host now gives 12 failed / 7 passed /
  1 skipped (§3), and the DevContainer has not been re-measured since.

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
Whoever lands the first 🐳 exercise (row 034 is the earliest) should run
`dotnet test -p:Containers=true --filter …Ex034_` inside the container and, if it passes,
upgrade this section to the §7 bar — and if it fails on `localhost`, record which of the
fixes above worked, because every later 🐳 row inherits it.

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
