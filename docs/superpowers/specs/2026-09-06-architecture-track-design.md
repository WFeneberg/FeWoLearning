# Architecture Track — Design

**Date:** 2026-09-06
**Status:** approved, implementation pending
**Folder:** `Architecture/` (capitalised, deliberately — same deviation as `MicroServices/`)

## 1. What this track is

60 graded C# exercises in **application and system architecture** on .NET 10, across
the three areas the owner actually ships into: **web** (ASP.NET Core), **desktop**
(composition and lifecycle patterns behind a UI), and **services + data** (databases,
caches, message buses, MQTT), plus a **cross-cutting** block for the concerns that
cut through all three.

It is not a second `MicroServices/` track. `MicroServices/` teaches *Aspire and the
deployment topology* — how resources are declared, wired, published and started.
This track teaches the *patterns inside the process*: what an outbox is, why a
cache-aside loader must be counted rather than observed, what a saga compensates,
which direction dependencies are allowed to point. Where the two touch (a message
bus, a Postgres row), this track owns the pattern and `MicroServices/` owns the
orchestration.

It is also not a UI track. `wpf/`, `caliburn/`, `avalonia/` and `uno/` cover UI
frameworks. Block `02-desktop` here is deliberately **UI-framework-free**: MVVM
composition, navigation, messaging, plugin loading, offline sync and undo/redo are
all architecture, and all of them are testable without a rendering stack. The
payoff is that the entire track is headless, cross-platform and CI-runnable, unlike
`wpf/`, `caliburn/` and block `04` of `security/`.

## 2. Repository shape

```
Architecture/
  FeWoLearning.Architecture.slnx
  Directory.Build.props
  catalog.md                         # 60-row ledger
  README.md
  exercises/FeWoLearning.Architecture.Exercises.csproj
    01-web/ 02-desktop/ 03-services-data/ 04-cross-cutting/ _support/
  solutions/FeWoLearning.Architecture.Solutions.csproj
    (same four block folders, same relative paths, same type names)
  tests/FeWoLearning.Architecture.Tests.csproj
    (same four block folders) _harness/
```

Three projects, and `solutions/` is deliberately **in** the build — the same waiver
`avalonia/`, `blazor/`, `uno/`, `caliburn/`, `wpf/` and `security/` take. `exercises/`
and `solutions/` compile the same type names into the same namespaces; `tests/`
references **exactly one** of them via the `UseSolutions` MSBuild property, so the
name collision the repo-wide convention exists to prevent cannot occur, and reference
solutions are compile-checked and test-run on every green check instead of drifting
silently.

`Directory.Build.props` must redirect the solutions build's output via
`UseArtifactsOutput`/`ArtifactsPath`. This is required, not cosmetic: sharing an
`obj/` tree between two projects that emit the same generated assembly-info
attributes fails the build with `CS0579`. It has to live in `Directory.Build.props`
and not in the `.csproj` body, where `BaseOutputPath` is read after the SDK props
import and therefore too late.

**Namespaces are pinned per block**, because `01-web` is not a valid C# identifier:

| Folder | Namespace |
|---|---|
| `01-web` | `FeWoLearning.Architecture.Exercises.Web` |
| `02-desktop` | `FeWoLearning.Architecture.Exercises.Desktop` |
| `03-services-data` | `FeWoLearning.Architecture.Exercises.ServicesData` |
| `04-cross-cutting` | `FeWoLearning.Architecture.Exercises.CrossCutting` |

Test namespaces mirror these as `FeWoLearning.Architecture.Tests.<Block>`.

`_support/` (identical in both content libraries) holds fixtures several exercises
share — an in-memory bus, a controllable clock, a counting loader. It is never a
TODO and never gets a `catalog.md` row.

## 3. Toolchain

Target framework **`net10.0`** for all three projects. No `UseWPF`, no
`net10.0-windows`, no Windows-only surface anywhere.

Test stack: **xunit.v3 3.2.2 + `xunit.runner.visualstudio` 3.1.5 +
`Microsoft.NET.Test.Sdk` 17.14.1, and NO `global.json`** — the classic VSTest path,
copied from `MicroServices/` because it is the combination measured to work on this
machine. xunit.v3 4.0.0 plus a `Microsoft.Testing.Platform` `global.json` is the
combination that makes `dotnet test` exit 5 with zero tests discovered here.
`xunit.runner.visualstudio` has no 3.1.6 or 3.1.7; naming one resolves *forward* to
4.0.0 with only an `NU1603` warning, silently landing back on the broken generation.

Exact package versions are resolved and pinned at scaffold time by a real restore,
not from memory. The set is known: `Microsoft.Data.Sqlite` (plus
`SQLitePCLRaw.lib.e_sqlite3` pinned to 2.1.13 or later, since the transitive 2.1.11
carries GHSA-2m69-gcr7-jv3q and emits `NU1903`), `MQTTnet` 5.x, `Polly.Core`,
`Microsoft.Extensions.*` where not already in the shared framework, and — for the
container-gated rows only, in `tests/` — the `Testcontainers.*` family.

Commands, run from inside `Architecture/`:

| Run | Command |
|---|---|
| Stubs (red) | `dotnet test` |
| Solutions (green) | `dotnet test -p:UseSolutions=true` |
| Including container rows | `dotnet test -p:Containers=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |

## 4. Catalog structure — 60 rows in 4 blocks

The track departs from the repo's 100-row / four-difficulty-tier scheme for the same
reason `security/` does: "beginner architecture" is not a meaningful axis. A
cache-aside loader is not conceptually harder than a middleware pipeline; they are
different concerns. Difficulty rises **within** each block.

Container-gated rows are marked 🐳: they carry *additional* facts that need Docker and
are skipped by default. Every such exercise is still fully graded without Docker by
its in-process facts — the container facts add realism, never coverage that would
otherwise be missing.

### `01-web` (001–016) — ASP.NET Core composition and request flow

| # | Slug | Concepts |
|---|---|---|
| 001 | LayeredCompositionRoot | layering, composition root, dependency direction |
| 002 | ServiceLifetimes | singleton/scoped/transient, captive dependency detection |
| 003 | OptionsPattern | IOptions vs IOptionsSnapshot vs IOptionsMonitor, named options, validation |
| 004 | MiddlewarePipeline | chain of responsibility, ordering, short-circuiting |
| 005 | VerticalSliceEndpoint | feature slices, slice-local request/response, no shared service layer |
| 006 | CqrsCommandQuery | command/query separation, distinct handler contracts |
| 007 | MediatorDispatch | mediator, handler resolution by request type, no service locator leak |
| 008 | PipelineBehaviors | decorator chain around a handler, ordering, composition |
| 009 | ValidationBehavior | validation as a pipeline stage that runs before the handler |
| 010 | ResultErrorModel | Result<T> vs exceptions, error-to-status mapping, no control flow by throw |
| 011 | DtoBoundaryMapping | domain-to-DTO projection, preventing domain leakage across the boundary |
| 012 | ApiVersioning | versioned contracts, v1/v2 coexistence, additive vs breaking change |
| 013 | PaginationContract | cursor vs offset, stable ordering, page metadata |
| 014 | BackendForFrontend | aggregation, parallel fan-out, partial-failure semantics |
| 015 | RateLimitingPolicy | token bucket vs fixed window, per-client partitioning, virtual clock |
| 016 | HealthReadinessLiveness | health-check composition, readiness gating vs liveness |

### `02-desktop` (017–028) — desktop composition, UI-framework-free

| # | Slug | Concepts |
|---|---|---|
| 017 | MvvmComposition | view model as a unit, INotifyPropertyChanged, no framework dependency |
| 018 | NavigationService | view-model-first navigation, back stack, lifecycle callbacks |
| 019 | MessengerEventAggregator | in-process pub/sub, subscription lifetime, unsubscribe correctness |
| 020 | DialogServiceAbstraction | modal interaction as a port, testable without a window |
| 021 | BackgroundJobScheduler | queued work, cancellation, sequencing guarantees |
| 022 | OfflineFirstSync | local SQLite store, change tracking, conflict resolution policy |
| 023 | SettingsStatePersistence | versioned settings, migration on load, forward compatibility |
| 024 | PluginArchitecture | AssemblyLoadContext, contract assembly, isolation and unload |
| 025 | UndoRedoCommandStack | command pattern, undo/redo invariants, coalescing |
| 026 | ScopedPerViewDi | child scope per view, deterministic disposal, no captive dependency |
| 027 | ThreadMarshallingAbstraction | dispatcher as a port, testable synchronisation |
| 028 | TelemetryBoundary | logging/metrics as a port, keeping the domain logger-free |

### `03-services-data` (029–052) — databases, caches, buses, MQTT

| # | Slug | Concepts |
|---|---|---|
| 029 | RepositoryUnitOfWork | repository boundary, unit of work, single commit per operation |
| 030 | SpecificationPattern | composable specifications, AND/OR/NOT, translation to a query |
| 031 | AggregateDomainEvents | aggregate invariants, event collection, dispatch after commit |
| 032 | TransactionalOutbox 🐳 | persist + enqueue in one transaction, relay, rollback atomicity |
| 033 | IdempotentConsumer | inbox dedup, at-least-once delivery, duplicate suppression |
| 034 | CacheAside | hit/miss, loader invocation count, negative caching |
| 035 | WriteThroughWriteBehind | write-through vs write-behind, flush semantics, loss window |
| 036 | CacheStampede 🐳 | single-flight, concurrent loaders, one load per key |
| 037 | DistributedLock 🐳 | lease acquisition, expiry, fencing token |
| 038 | OptimisticConcurrency 🐳 | version column, conflict detection, lost-update prevention |
| 039 | PessimisticLocking 🐳 | locked read, serialised writers, deadlock avoidance ordering |
| 040 | SagaProcessManager | multi-step process, compensation on failure, saga state |
| 041 | ChoreographyVsOrchestration | the same flow in two topologies, coupling trade-off |
| 042 | EventSourcingAppendStream | append-only stream, rehydration, expected-version check |
| 043 | EventSourcedProjection | catch-up projection, idempotent apply, checkpoint |
| 044 | CqrsReadModel | separate read store, eventual consistency, staleness window |
| 045 | MessageBusAbstraction | publish/subscribe port, topic routing, transport independence |
| 046 | CompetingConsumers 🐳 | work distribution, per-key ordering via partition key |
| 047 | DeadLetterQueue 🐳 | poison message, max attempts, dead-letter move with reason |
| 048 | RetryWithBackoff | exponential backoff, jitter, virtual clock, budget exhaustion |
| 049 | MqttTelemetryIngest | real in-process MQTT broker, topic hierarchy, wildcard subscription |
| 050 | MqttQosRetainedLastWill 🐳 | QoS semantics, retained messages, last will and testament |
| 051 | MqttRequestReply | response topic, correlation data, timeout handling |
| 052 | EventSchemaEvolution | versioned events, upcasting, tolerant reader |

### `04-cross-cutting` (053–060)

| # | Slug | Concepts |
|---|---|---|
| 053 | ResiliencePipeline | retry + timeout + circuit breaker composition, strategy ordering |
| 054 | CircuitBreakerStates | closed/open/half-open transitions, virtual clock, probe request |
| 055 | CorrelationContextPropagation | correlation id across process and transport boundaries, Activity |
| 056 | StructuredLoggingBoundary | log as a port, scopes, no message formatting in the domain |
| 057 | ConfigurationLayering | provider precedence, secret sources, reload-on-change |
| 058 | AntiCorruptionLayer | translation between bounded contexts, no foreign model leak |
| 059 | StranglerFigFacade | routing facade, legacy vs replacement, gradual cutover |
| 060 | ArchitectureFitnessTests | dependency direction enforced by reflection over assembly metadata |

## 5. Infrastructure strategy — three tiers

**Tier 1 — fakes (default).** An in-memory bus, an in-memory cache whose loader
calls are counted, and a controllable clock, all in `_support/`. Milliseconds,
deterministic, no external process. Most rows live here.

**Tier 2 — real, but in-process (also default, no Docker).**
`Microsoft.Data.Sqlite` backs every transaction, outbox, concurrency and offline-sync
row, because outbox atomicity **cannot be honestly proven against a fake** — a fake
that "rolls back" does so because the fake was written to. And **MQTT runs against a
real MQTTnet 5 broker started in-process on a loopback port**: real protocol frames,
real QoS 1 redelivery, real retained messages and last-will delivery, with no
container. MQTT is therefore fully graded in the default run rather than gated.

**Tier 3 — containers (`-p:Containers=true`, otherwise `Assert.SkipUnless`).**
Testcontainers for Postgres (real optimistic concurrency, real `SELECT … FOR UPDATE`,
a real outbox relay), Redis (a real distributed lock with expiry, a real stampede
across connections), RabbitMQ (real competing consumers, a real dead-letter exchange)
and Mosquitto (broker restart, persistent sessions). Eight rows carry these extra
facts.

The gate copies `MicroServices/tests/_support/ContainerGate.cs`: `FactAttribute.Skip`
is not virtual in xunit.v3 3.2.2, so a custom `[ContainerFact]` overriding it fails
`CS0506`. The gate is `Assert.SkipUnless` in the test body, reading an
`AppContext` value fed by a `RuntimeHostConfigurationOption` in the test `.csproj`,
since an MSBuild property is otherwise invisible at runtime.

## 6. The recurring bug class

This is the track's identity, the way "an attack fact with no paired use fact grades
nothing" is `security/`'s. **Architecture tests lie in two ways:**

**Lie 1 — the outcome is reachable without the pattern.** An outbox test that only
asserts "the message arrived" is satisfied by a direct publish that skips the outbox
entirely. A cache test that only asserts "the value came back" is satisfied by an
implementation with no cache at all. A repository test that only asserts "the entity
was saved" is satisfied by a bare data-context call in the handler. The fix is always
the same: assert the **mechanism's own observable side effect** — the row that was
committed inside the same transaction, the loader's invocation count, the checkpoint
that advanced — not merely the end state.

**Lie 2 — the pattern is asserted but never exercised.** An idempotent-consumer test
that delivers each message exactly once proves nothing, because a consumer with no
dedup passes it. A circuit-breaker test that never trips the breaker grades nothing.
A concurrency test with one writer cannot detect a lost update. The fix: every
mechanism exercise carries an **adversarial fact** in which the naive implementation
demonstrably diverges — a duplicate delivery, a failing commit, a second concurrent
writer, a clock advanced past the break duration.

And the lesson carried over from `security/`'s final review: after building the
**degenerate** implementation (returns a constant, does nothing) to check the facts
catch it, also build the **plausible wrong** one — the pattern implemented earnestly
but with the wrong mechanism — and confirm the facts still go red. That is the bug
class the degenerate probe never finds.

A third, specific to this track: **a test may assert structure that the runtime does
not enforce.** Rows about dependency direction, layering and module boundaries (001,
026, 058, 060) must read assembly metadata via reflection, because no behavioural
assertion can distinguish "the domain does not reference infrastructure" from "the
domain happens not to call it in this test".

## 7. Exercise format

Unchanged from the repo convention. Stubs live at
`exercises/<block>/ExNNN_<Slug>.cs`, tests at `tests/<block>/ExNNN_<Slug>Tests.cs`,
reference implementations at `solutions/<block>/ExNNN_<Slug>.cs`. Each stub carries a
header comment with `Goal:` / `Drills:` / `Passes:` and throws
`NotImplementedException` at runtime so the project still compiles while unfinished.

The invariant that defines a correct exercise holds: **red before implementation,
green once the stub matches its reference solution**, verified by actually running
both commands.

## 8. Delivery

Batches of five, per `CLAUDE.md`. Scaffolding, the 60-row `catalog.md` and
`README.md` land first, then twelve batches: write stub + test + solution, run the
filtered red check, run the green check under `-p:UseSolutions=true`, flip exactly
those five catalog rows, commit as `Architecture: exNNN–exNNN`. Documentation is
English, matching the rest of the repo. `CLAUDE.md` gains an `Architecture/` row in
its command, toolchain and current-state tables once the first batch is verified.


---

## Addendum, 2026-09-06 — blocks 05 and 06 (rows 061–080)

The original 60 were delivered and verified (371 facts, red 359/4/8, green 0/363/8,
containers 371/0). This addendum extends the track to **80 rows** at the owner's
request, with two blocks the first four deliberately did not cover.

Blocks 01–04 build **one process that is correct**. Everything in them can be
reasoned about by a single reader looking at a single instance: the outbox is atomic,
the cache counts its loader, the saga compensates. What none of them ask is what
happens when there are fifty of that process, all of them busy, the data no longer
fits on one machine, and the schema has to change while all of it stays up.

### `05-scale` (061–073) — many instances, all busy

`FeWoLearning.Architecture.Exercises.Scale`, folder `05-scale`.

Bulkhead isolation, admission control, backpressure, batching, HTTP idempotency
keys, leader election, distributed scheduling, graceful shutdown, startup readiness
ordering, sharding, read-replica routing, and the two multi-tenancy rows.

The recurring shape here is **a resource that is finite and shared**. Blocks 01–04
mostly assume the thing you call will answer; this block assumes it sometimes will
not, and asks what the caller does with the queue that builds up behind it.

### `06-evolution` (074–080) — the system changing while it runs

`FeWoLearning.Architecture.Exercises.Evolution`, folder `06-evolution`.

Expand-contract migration, resumable backfills, consumer-driven contracts, API
deprecation, feature-flag targeting, canary releases, and observability spans.

The recurring shape here is **two versions coexisting**. Every row is a case where
the old thing and the new thing are both live and both correct, and the exercise is
the mechanism that keeps them from noticing each other.

### What does not change

The grading rules, the two-probe procedure, the `UseSolutions` mechanism, the
toolchain pins and the container gate all carry over unchanged. Three new rows carry
container facts (065 Redis, 066 Redis, 074 Postgres), bringing the total to eleven.
Everything stays `net10.0` and headless.
