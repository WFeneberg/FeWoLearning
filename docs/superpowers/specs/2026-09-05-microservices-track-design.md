# MicroServices Track — Design

**Date:** 2026-09-05
**Status:** approved
**Track folder:** `MicroServices/`

---

## 1. Purpose

Another self-contained learning track, teaching **microservices on .NET with
Aspire**, following the repo's universal exercise pattern: 100 graded
exercises, each a stub that fails red before implementation and passes green
once it matches its reference solution.

"Beginner" means **Aspire and distributed-systems** beginner, not C# beginner.
`ex001` models a resource in a `DistributedApplication` graph, not a
`FizzBuzz`. Plain C# language drills belong to `dotnet/`; ASP.NET Core
component work belongs to `blazor/`.

The track covers four things the owner asked for together, deliberately in one
track because in practice they are one subject: **Aspire orchestration**,
**polyglot persistence** (SQL Server, PostgreSQL, MongoDB and a spread of other
NoSQL stores), **Docker**, and **Azure** — with everything runnable on a single
developer machine, optionally inside a DevContainer.

The agreed content weighting is **~55 rows Aspire + persistence, ~30 rows
microservice patterns, ~15 rows Docker + Azure**. Section 6 lays out the tier
plan.

## 2. Toolchain

### 2.1 Verified on this machine (2026-09-05)

Everything below was measured by running it, not read from documentation.

- .NET SDK **10.0.400** (`dotnet --version`), with 10.0.303, 9, 8, 7, 6 also
  installed.
- Aspire CLI **13.4.6**. The current NuGet stable for every `Aspire.*` package
  is **13.5.3**; `aspire new --version 13.5.3` pulls matching templates, so the
  CLI being one patch behind does not matter.
- Docker **29.7.2**, Linux containers, daemon reachable
  (`docker info` returns a server version). Docker Compose **v5.5.0**. The
  machine already holds `postgres:18.3/17/16`, `redis:8.6/7`,
  `testcontainers/ryuk` and `dcptun_developer_ms` — Aspire and Testcontainers
  have both run here before.
- Azure CLI: **not installed** — and deliberately not required, see section 6.6.
  The `devcontainer` CLI: **0.89.0**, installed for this track (npm 11.19.0).
  VS Code is installed.
- All required hosting integrations exist at 13.5.3: `SqlServer`,
  `PostgreSQL`, `MongoDB`, `Redis`, `Valkey`, `Garnet`, `Qdrant`, `Milvus`,
  `Oracle`, `MySql`, `Kafka`, `RabbitMQ`, `NATS`, `Seq`, `Azure.CosmosDB`,
  `Azure.Storage`, `Azure.AppContainers`, `Docker`.

### 2.2 The load-bearing measurement: model-only testing

The central risk in a microservices track is that every exercise needs a
container, making the red/green loop unusably slow. It does not.

`DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>()` followed
by `BuildAsync()` produced the complete resource graph in **1.67 s with zero
containers started**. A plain class library — `Microsoft.NET.Sdk`, no
`Aspire.AppHost.Sdk`, no executable — calling
`DistributedApplication.CreateBuilder(...)` did the same in **1.39 s**.

What that graph exposes is rich enough to grade against. Measured output for a
model holding Postgres, SQL Server and MongoDB:

```
pg      :: PostgresServerResource   :: Host={pg.bindings.tcp.host};Port={pg.bindings.tcp.port};Username=postgres;Password={pg-password.value}
orders  :: PostgresDatabaseResource :: {pg.connectionString};Database=orders
sql     :: SqlServerServerResource  :: Server={sql.bindings.tcp.host},{sql.bindings.tcp.port};User ID=sa;Password={sql-password.value};TrustServerCertificate=true
catalog :: SqlServerDatabaseResource:: {sql.connectionString};Initial Catalog=catalog
mongo   :: MongoDBServerResource    :: mongodb://admin:{mongo-password.value}@{mongo.bindings.tcp.host}:{mongo.bindings.tcp.port}/?authSource=admin&authMechanism=SCRAM-SHA-256
reviews :: MongoDBDatabaseResource  :: mongodb://admin:{mongo-password.value}@{...}/reviews?authSource=admin&authMechanism=SCRAM-SHA-256
```

Annotations are enumerable per resource — `WaitAnnotation`,
`HealthCheckAnnotation`, `ContainerImageAnnotation`, `EndpointAnnotation`,
`EnvironmentAnnotation`, `EnvironmentCallbackAnnotation`. A `WithReference` +
`WaitFor` pair was confirmed to leave exactly 2 `WaitAnnotation`s on the
consuming resource.

`ConnectionStringExpression.ValueExpression` differs per database flavour, so a
test can prove the learner wired up *PostgreSQL* rather than *some container*.
Section 8.2 makes that a rule.

### 2.3 The second measurement: offline deployment artifacts

`aspire publish` against a model carrying
`builder.AddDockerComposeEnvironment("compose")` wrote **`docker-compose.yaml`
and `.env`** to the output directory, offline, before any image build. The YAML
is deterministic and fully assertable — service names, pinned images
(`docker.io/library/postgres:18.3`, `mcr.microsoft.com/mssql/server:2022-latest`,
`docker.io/library/mongo:8.3`), `expose` ports, an `aspire` network, and per-service
environment including `ConnectionStrings__orders`, `ConnectionStrings__catalog`,
`ConnectionStrings__reviews`. Secrets are lifted into `.env` as
`${PG_PASSWORD}`, `${SQL_PASSWORD}`, `${MONGO_PASSWORD}`.

So Docker- and deployment-shaped exercises are gradeable without a
subscription and without building images.

One trap found while measuring, itself worth a catalog row: declaring **two**
compute environments (`AddDockerComposeEnvironment` *and*
`AddAzureContainerAppEnvironment`) without assigning resources fails the
`validate-compute-environments` pipeline step with *"Compute resource(s) … are
not assigned to a compute environment, but the model contains multiple compute
environments"*.

A caveat that shapes the implementation: the `aspire publish` **CLI** writes the
artifacts and then does not exit — in a non-interactive shell it drops into
"press CTRL+C to stop the AppHost" and was still running at 200 s and at 600 s.
It is unusable inside a test loop.

**The in-process entry point, measured.** Constructing the builder with publish
arguments and awaiting `RunAsync` returns cleanly in **≈3.7 s** and writes
`aspire-manifest.json` — and, for a model carrying Azure resources, the generated
Bicep alongside it (see the correction below):

```csharp
var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
{
    Args = ["--operation", "publish", "--output-path", dir],
    DisableDashboard = true
});
Ex0NN_Whatever.Configure(builder);
using var app = builder.Build();
await app.RunAsync(cts.Token);
// dir/aspire-manifest.json now exists
```

`PublishingOptions` binds `Publisher` and `OutputPath` from the `Publishing`
configuration section, which is what those arguments set.

The manifest is a rich, deterministic assertion target — per resource it carries
`type` (`container.v0`, `value.v0`, `parameter.v0`), `image` with the pinned tag,
the full `env` map including `ConnectionStrings__*` and the per-flavour
`*_URI` / `*_JDBCCONNECTIONSTRING` forms, `bindings` with `targetPort`, and for
generated secrets the `inputs.value.default.generate` policy. **This is one of the two
things L2 asserts against.**

**Correction, measured after this spec was first written: the manifest is NOT the only
in-process artifact.** An in-process publish of a model carrying
`AddAzureContainerAppEnvironment` plus `AddAzureStorage` writes, in **≈7.5 s**, real
Bicep as well:

```
aspire-manifest.json
aca.module.bicep      aca-acr.module.bicep      storage.module.bicep
aca/aca.bicep         aca-acr/aca-acr.bicep     storage/storage.bicep
```

So the Azure rows (093, 094, 099, 100) assert against **real generated Bicep** in the
fast loop, with no subscription and no golden-file fallback. Docker Compose YAML,
below, is the *single* exception — not the general case. `ManifestHarness.PublishAsync`
is the entry point that exposes these files; `ManifestHarness.GenerateAsync` returns
only the parsed manifest and deletes the output directory before returning.

What is *not* available in-process is the Docker Compose YAML: every argument
combination tried (`--publisher default`, `--publisher compose`, no publisher,
with and without `--operation publish`) produced the manifest, because the
compose file is emitted by a pipeline the CLI drives. Compose-specific rows
therefore assert a **committed golden `docker-compose.yaml`**, generated once at
authoring time with the CLI and checked in — the assertion is that the learner's
model produces a manifest consistent with that golden file, not that the test
re-runs the CLI. Section 6.5 rows are scoped accordingly.

### 2.4 The test runner — and a repo-wide finding

`dotnet test` must work, because that is the command every other track
documents. Two paths were measured:

| Configuration | Result |
|---|---|
| xunit.v3 **4.0.0** + `global.json` `{"test":{"runner":"Microsoft.Testing.Platform"}}` | ❌ exit code 5, "no tests ran" |
| xunit.v3 **3.2.2** + `xunit.runner.visualstudio` **3.1.5**, **no** `global.json` | ✅ tests discovered and passed, 415 ms, no warnings |

Under the failing configuration the xunit MTP runner printed its own **usage
text** — the .NET 10 SDK's `dotnet test` bridge passes it options it does not
accept. The test **executables** run correctly in both configurations; only the
`dotnet test` bridge is affected.

Therefore this track pins **xunit.v3 3.2.2 on the classic VSTest path and ships
no `global.json`** — the same generation `avalonia/` and `caliburn/` run.

A pinning trap worth recording, hit while measuring this: `xunit.runner.visualstudio`
has **no 3.1.6 or 3.1.7** — 3.1.5 is the last 3.x, and the next version is 4.0.0.
Naming a 3.x that does not exist does not fail the build; NuGet resolves *forward*
to 4.0.0 with only an `NU1603` warning, quietly landing the project on the runner
generation this track is avoiding. Pin **3.1.5** exactly and treat `NU1603` here as
an error, not noise.

**Finding outside this track's scope:** `wpf/` currently fails the same way.
Running `dotnet test` in `wpf/` gives exit code 5 and zero tests, while
`wpf/tests/bin/Debug/net10.0-windows/FeWoLearning.Wpf.Tests.exe` runs its 147
tests correctly (143 red, as its 5 seeded exercises expect). `wpf/` is the only
track on xunit.v3 4.0.0 + the MTP `global.json`, which is exactly the broken
combination. `CLAUDE.md` documents `dotnet test` as verified for `wpf/`; that
is no longer true on this machine. Fixing it is a separate task and must not be
folded into this one.

### 2.5 Pinned versions

| Package | Version | Where |
|---|---|---|
| `Aspire.Hosting` + all `Aspire.Hosting.*` integrations | 13.5.3 | `exercises/` + `solutions/` |
| `Aspire.Hosting.Testing` | 13.5.3 | `tests/` |
| `Aspire.Hosting.Elasticsearch` | **13.3.0** | `exercises/` + `solutions/` |
| `xunit.v3` | 3.2.2 | `tests/` |
| `xunit.runner.visualstudio` | 3.1.5 | `tests/` |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | `tests/` |

`Aspire.Hosting.Elasticsearch` is the one deliberate version outlier: its latest
stable is 13.3.0 while every sibling is at 13.5.3. It is pinned at 13.3.0 and
documented in `MicroServices/README.md` as a known lag, not silently bumped and
not silently dropped.

## 3. Folder name

The track folder is **`MicroServices/`**, capitalised, at the repo root. Every
other track folder is lowercase (`blazor/`, `uno/`, `wpf/`). This deviation is
the track owner's explicit choice and is recorded here so a later reader does
not "fix" it.

## 4. Build topology

```
MicroServices/
  FeWoLearning.MicroServices.slnx
  Directory.Build.props
  README.md
  catalog.md
  .devcontainer/
    devcontainer.json
  exercises/                       # FeWoLearning.MicroServices.Exercises.csproj
    01-beginner/ExNNN_Slug.cs
    02-intermediate/ …
    03-advanced/ …
    04-expert/ …
  solutions/                       # FeWoLearning.MicroServices.Solutions.csproj
    <same relative paths>
  tests/                           # FeWoLearning.MicroServices.Tests.csproj
    01-beginner/ExNNN_SlugTests.cs
    _support/                      # shared fixtures; never a catalog row
  services/                        # fixed set of real ASP.NET Core services
    Catalog/ Orders/ Reviews/ Gateway/
  playground/                      # one AppHost, selects an exercise at runtime
```

### 4.1 Why `solutions/` is in the build

`exercises/` and `solutions/` compile **the same type names into the same
namespaces**. `tests/` references exactly one of them, selected by the
`UseSolutions` MSBuild property, so the collision the repo-wide
"`solutions/` outside the build" convention exists to prevent cannot occur:

```
dotnet test                          # red run, against exercises/
dotnet test -p:UseSolutions=true     # green run, against solutions/
```

This is the same deliberate, permanent waiver `blazor/`, `uno/`, `wpf/`,
`caliburn/` and `avalonia/` take. The payoff is that reference solutions are
compile-checked and test-run on every green check and cannot drift silently —
the failure mode `docs/exercise-format.md` records for `vue/` and `go/`.

`MicroServices/Directory.Build.props` must redirect the solutions build via
`UseArtifactsOutput` / `ArtifactsPath`, exactly as its five siblings do. This is
required, not cosmetic: sharing an `obj/` tree between two projects that emit
the same assembly-info attributes fails the build with `CS0579`. It has to live
in `Directory.Build.props`, not the `.csproj` body, because
`BaseOutputPath`/`BaseIntermediateOutputPath` set inside a project are read
after the SDK props import — too late.

### 4.2 Namespaces

Pinned per tier, because `01-beginner` is not a valid C# identifier:

```
FeWoLearning.MicroServices.Exercises.Beginner
FeWoLearning.MicroServices.Exercises.Intermediate
FeWoLearning.MicroServices.Exercises.Advanced
FeWoLearning.MicroServices.Exercises.Expert
```

### 4.3 The shape of an exercise

Each exercise is a static class exposing at minimum:

```csharp
public static class Ex037_EfCoreAgainstPostgres
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException("TODO: …");
}
```

Exercises whose subject includes application code (an EF Core model, a Mongo
aggregation, an outbox dispatcher) expose additional members alongside
`Configure`. Stubs throw `NotImplementedException`; the projects still compile.

### 4.4 `services/` and `playground/`

`services/` holds a small **fixed** set of real ASP.NET Core projects — Catalog,
Orders, Reviews, Gateway — shared by every exercise that needs a genuine HTTP
service to reference. They are not exercises, get no catalog rows, and change
rarely.

`playground/` is a **single** AppHost project that dispatches to one exercise by
name:

```
aspire run --apphost MicroServices/playground -- --exercise ex037
```

This is what buys back the "every exercise is really runnable, in the real
dashboard" property without creating 100 executable AppHost projects. Because
`Configure` takes an `IDistributedApplicationBuilder`, the same code the tests
grade is the code the dashboard runs.

## 5. Test levels

| Level | What it asserts | Cost | How it runs |
|---|---|---|---|
| **L1 model** | resource graph, types, `ConnectionStringExpression`, annotations | ~1.4 s | always |
| **L2 artifact** | `aspire-manifest.json` **and the generated Bicep**, both in-process (§2.3) | ~3.7 s, ~7.5 s with Azure resources; no container | always |
| **L3 container** | a real database starts and a real query runs | minutes | opt-in only |

L1 and L2 are the daily loop and cover the large majority of rows. L3 hangs on a
custom `[ContainerFact]` (and `[ContainerTheory]`) that **skips** unless
containers are enabled, so the default `dotnet test` stays fast and needs no
Docker daemon:

```
dotnet test                          # L1 + L2, L3 skipped
dotnet test -p:Containers=true       # everything
```

Catalog rows backed by L3 carry a 🐳 marker. The estimate is **20–25 rows** —
every place where only a real run proves the thing the exercise is about: EF
Core migrations actually applying, a Mongo aggregation pipeline actually
returning documents, an outbox actually delivering, a Redis pattern actually
expiring.

The skip condition is `Containers=true` **and** a reachable Docker daemon; with
the flag set but no daemon the tests fail rather than silently skipping, so a
broken Docker setup cannot masquerade as a green run.

## 6. Tier plan

### 6.1 Beginner, 001–035 — Aspire model and first persistence

Resource modelling, `WithReference` / `WaitFor` and the difference between
them, endpoints and bindings, health checks, parameters and secrets,
`ServiceDefaults`, OpenTelemetry wiring, and the first real connection to each
of the four everyday stores: SQL Server, PostgreSQL, MongoDB, Redis.

### 6.2 Intermediate A, 036–055 — persistence in depth

EF Core against SQL Server and against PostgreSQL and where they differ,
migrations, seeding, transactions; MongoDB documents, indexes and aggregation;
Redis / Valkey / Garnet caching and expiry patterns; vector search with Qdrant
and Milvus; Elasticsearch; MySQL; Oracle; Azurite for Blob / Queue / Table; the
Cosmos DB emulator.

This is where "diverse Datenbanken" is actually delivered — one flavour is not
one row, and the exercises are about what makes each store *different*, not
about repeating CRUD twelve times.

### 6.3 Intermediate B, 056–070 — communication

Service discovery, typed `HttpClient`s across services, gRPC, RabbitMQ, Kafka,
NATS, resilience pipelines, Seq for structured logs.

### 6.4 Advanced, 071–085 — patterns

Transactional outbox, saga / process manager, CQRS with a separate read model,
idempotent consumers, event versioning, distributed tracing across services,
gateway / BFF, and the consistency models that motivate all of it.

### 6.5 Advanced, 086–090 — Docker in depth

`AddDockerfile`, volumes, bind mounts and networks, multi-stage builds, and
Compose publishing.

### 6.6 Expert, 091–100 — Azure and deployment

Azure emulators inside the model, `AddAzureContainerAppEnvironment` and the
generated Bicep, managed identity and role assignments, multi-environment
models, custom Aspire resources, and the deployment pipeline — including the
two-compute-environment failure recorded in section 2.3.

Azure is taught **offline throughout**: emulators plus generated artifacts. No
subscription, no `az login`, no `azd up`. Every Azure row is red/green testable.

## 7. DevContainer

`MicroServices/.devcontainer/devcontainer.json`, based on the .NET 10
devcontainer image, adding:

- the **docker-outside-of-docker** feature, so Aspire can start sibling
  containers on the host daemon (docker-in-docker would nest them and break
  Aspire's port handling),
- the Aspire CLI,
- forwarding for port 18888, the Aspire dashboard.

It works under Rider as well as VS Code — JetBrains IDEs read
`devcontainer.json`.

**Verification:** the `devcontainer` CLI was missing when this design was
drafted; it has since been installed — **0.89.0**, via
`npm i -g @devcontainers/cli`, on npm 11.19.0 — and is on `PATH`. The
DevContainer is therefore **verified for real**, not shipped on faith:
`devcontainer up` must build the container, and Aspire must start a sibling
database container from inside it, before the track claims DevContainer
support. If that ever stops being provable, the DevContainer ships marked
*unverified* in both `README.md` and `CLAUDE.md`, the way `java/` and `kotlin/`
are — never silently presented as working.

## 8. Grading rules specific to this track

### 8.1 The universal invariant still holds

A stub's test fails **red** before implementation, caused by the TODO and not by
a compile or import error, and passes **green** once the stub matches its
reference solution. A stub that fails to build is a bug.

### 8.2 Rendered connection data does not prove the mechanism

The recurring bug class here. A test asserting only that "a Postgres-ish
container is in the model" is satisfied just as happily by
`AddContainer("pg", "postgres")` as by `AddPostgres("pg")` — so it grades
nothing. Every persistence row must assert **both** the resource *type*
(`PostgresDatabaseResource`, `SqlServerDatabaseResource`,
`MongoDBDatabaseResource`, …) **and** the `ConnectionStringExpression`, which
section 2.2 shows differs per flavour.

The same rule generalises: an exercise about `WaitFor` asserts
`WaitAnnotation`s, not merely that the app started; an exercise about health
checks asserts `HealthCheckAnnotation`, not merely a 200 response.

### 8.3 Always ask what a wrong implementation would do

Before trusting any test, ask whether a naive or wrong implementation would
still pass it. This is the check that caught real bugs in `kotlin/` without a
compiler, and the distributed subject matter here makes it more necessary, not
less: an assertion about eventual consistency that never advances time, or
about a message being consumed that never waits for delivery, will pass against
almost anything.

### 8.4 Do not guess API shapes

The Aspire CLI ships `aspire docs search` and `aspire docs api search
--language csharp`. Use them rather than inventing builder methods, package
names or overloads. Aspire 13's surface has moved considerably and tutorial
material is frequently wrong about it.

## 9. Build order

Step 1 is scaffolding and must land before any exercise:

1. `.slnx`, `Directory.Build.props`, the three projects, the `UseSolutions`
   switch, `services/`, `playground/`, `[ContainerFact]`, and the in-process
   manifest generator of section 2.3. Proven by harness smoke tests that pass in
   both the red and the green run.
2. The DevContainer, verified per section 7.
3. `catalog.md` seeded with all 100 rows as ⬜, following section 6.
4. Exercises in **batches of five**, per `CLAUDE.md`: write stub + test +
   solution, red-check filtered to the five, green-check with
   `-p:UseSolutions=true`, flip exactly those five catalog rows, commit as
   `MicroServices: exNNN–exNNN`.

Full-suite runs happen once per completed tier, not per batch.

## 10. Documentation to update on completion

- `MicroServices/README.md` — setup, commands, the Elasticsearch version lag,
  the L3 opt-in, DevContainer usage, and the traps found while building.
- `MicroServices/catalog.md` — the 100-row ledger and its `**Status:**` line.
- Root `CLAUDE.md` — per-track command table, toolchain status, track-specific
  gotchas, current-state table.
- `docs/requirements.md` — Docker, Aspire CLI and the devcontainer CLI.
- `docs/exercise-format.md` — the naming row for this track, and its
  `solutions/`-in-build exception.
