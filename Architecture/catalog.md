# Architecture (C#) — Exercise Catalog (100)

Blocks: **web** 001–016 · **desktop** 017–028 · **services-data** 029–052 ·
**cross-cutting** 053–060 · **scale** 061–073 · **evolution** 074–080 ·
**domain** 081–090 · **runtime** 091–100.

Legend: ✅ seeded (stub + test + solution present, red and green both verified) ·
⬜ planned · 🐳 carries extra container-backed facts, skipped unless `-p:Containers=true`.

This track uses four subject-area blocks rather than the repo's usual 100-row /
four-difficulty-tier scheme, for the same reason `security/` does: "beginner
architecture" is not a meaningful axis. A cache-aside loader is not conceptually
harder than a middleware pipeline; they are different concerns. Difficulty rises
*within* each block. See
`docs/superpowers/specs/2026-09-06-architecture-track-design.md` §4.

Stubs live in `exercises/<block>/ExNNN_<Slug>.cs`, their xUnit tests in
`tests/<block>/ExNNN_<Slug>Tests.cs`, and reference implementations in
`solutions/<block>/` at the same relative path.

**Status: 90 ✅ / 10 ⬜**

## web (001–016) — ASP.NET Core composition and request flow

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 001 | LayeredCompositionRoot | layering, composition root, dependency direction | ✅ |
| 002 | ServiceLifetimes | singleton/scoped/transient, captive dependency detection | ✅ |
| 003 | OptionsPattern | IOptions vs IOptionsSnapshot vs IOptionsMonitor, named options, validation | ✅ |
| 004 | MiddlewarePipeline | chain of responsibility, ordering, short-circuiting | ✅ |
| 005 | VerticalSliceEndpoint | feature slices, slice-local request/response, no shared service layer | ✅ |
| 006 | CqrsCommandQuery | command/query separation, distinct handler contracts | ✅ |
| 007 | MediatorDispatch | mediator, handler resolution by request type, no service locator leak | ✅ |
| 008 | PipelineBehaviors | decorator chain around a handler, ordering, composition | ✅ |
| 009 | ValidationBehavior | validation as a pipeline stage that runs before the handler | ✅ |
| 010 | ResultErrorModel | Result vs exceptions, error-to-status mapping, no control flow by throw | ✅ |
| 011 | DtoBoundaryMapping | domain-to-DTO projection, preventing domain leakage across the boundary | ✅ |
| 012 | ApiVersioning | versioned contracts, v1/v2 coexistence, additive vs breaking change | ✅ |
| 013 | PaginationContract | cursor vs offset, stable ordering, page metadata | ✅ |
| 014 | BackendForFrontend | aggregation, parallel fan-out, partial-failure semantics | ✅ |
| 015 | RateLimitingPolicy | token bucket vs fixed window, per-client partitioning, virtual clock | ✅ |
| 016 | HealthReadinessLiveness | health-check composition, readiness gating vs liveness | ✅ |

## desktop (017–028) — desktop composition, UI-framework-free

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 017 | MvvmComposition | view model as a unit, INotifyPropertyChanged, no framework dependency | ✅ |
| 018 | NavigationService | view-model-first navigation, back stack, lifecycle callbacks | ✅ |
| 019 | MessengerEventAggregator | in-process pub/sub, subscription lifetime, unsubscribe correctness | ✅ |
| 020 | DialogServiceAbstraction | modal interaction as a port, testable without a window | ✅ |
| 021 | BackgroundJobScheduler | queued work, cancellation, sequencing guarantees | ✅ |
| 022 | OfflineFirstSync | local SQLite store, change tracking, conflict resolution policy | ✅ |
| 023 | SettingsStatePersistence | versioned settings, migration on load, forward compatibility | ✅ |
| 024 | PluginArchitecture | AssemblyLoadContext, contract assembly, isolation and unload | ✅ |
| 025 | UndoRedoCommandStack | command pattern, undo/redo invariants, coalescing | ✅ |
| 026 | ScopedPerViewDi | child scope per view, deterministic disposal, no captive dependency | ✅ |
| 027 | ThreadMarshallingAbstraction | dispatcher as a port, testable synchronisation | ✅ |
| 028 | TelemetryBoundary | logging/metrics as a port, keeping the domain logger-free | ✅ |

## services-data (029–052) — databases, caches, buses, MQTT

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 029 | RepositoryUnitOfWork | repository boundary, unit of work, single commit per operation | ✅ |
| 030 | SpecificationPattern | composable specifications, AND/OR/NOT, translation to a query | ✅ |
| 031 | AggregateDomainEvents | aggregate invariants, event collection, dispatch after commit | ✅ |
| 032 | TransactionalOutbox 🐳 | persist + enqueue in one transaction, relay, rollback atomicity | ✅ |
| 033 | IdempotentConsumer | inbox dedup, at-least-once delivery, duplicate suppression | ✅ |
| 034 | CacheAside | hit/miss, loader invocation count, negative caching | ✅ |
| 035 | WriteThroughWriteBehind | write-through vs write-behind, flush semantics, loss window | ✅ |
| 036 | CacheStampede 🐳 | single-flight, concurrent loaders, one load per key | ✅ |
| 037 | DistributedLock 🐳 | lease acquisition, expiry, fencing token | ✅ |
| 038 | OptimisticConcurrency 🐳 | version column, conflict detection, lost-update prevention | ✅ |
| 039 | PessimisticLocking 🐳 | locked read, serialised writers, deadlock avoidance ordering | ✅ |
| 040 | SagaProcessManager | multi-step process, compensation on failure, saga state | ✅ |
| 041 | ChoreographyVsOrchestration | the same flow in two topologies, coupling trade-off | ✅ |
| 042 | EventSourcingAppendStream | append-only stream, rehydration, expected-version check | ✅ |
| 043 | EventSourcedProjection | catch-up projection, idempotent apply, checkpoint | ✅ |
| 044 | CqrsReadModel | separate read store, eventual consistency, staleness window | ✅ |
| 045 | MessageBusAbstraction | publish/subscribe port, topic routing, transport independence | ✅ |
| 046 | CompetingConsumers 🐳 | work distribution, per-key ordering via partition key | ✅ |
| 047 | DeadLetterQueue 🐳 | poison message, max attempts, dead-letter move with reason | ✅ |
| 048 | RetryWithBackoff | exponential backoff, jitter, virtual clock, budget exhaustion | ✅ |
| 049 | MqttTelemetryIngest | real in-process MQTT broker, topic hierarchy, wildcard subscription | ✅ |
| 050 | MqttQosRetainedLastWill 🐳 | QoS semantics, retained messages, last will and testament | ✅ |
| 051 | MqttRequestReply | response topic, correlation data, timeout handling | ✅ |
| 052 | EventSchemaEvolution | versioned events, upcasting, tolerant reader | ✅ |

## cross-cutting (053–060)

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 053 | ResiliencePipeline | retry + timeout + circuit breaker composition, strategy ordering | ✅ |
| 054 | CircuitBreakerStates | closed/open/half-open transitions, virtual clock, probe request | ✅ |
| 055 | CorrelationContextPropagation | correlation id across process and transport boundaries, Activity | ✅ |
| 056 | StructuredLoggingBoundary | log as a port, scopes, no message formatting in the domain | ✅ |
| 057 | ConfigurationLayering | provider precedence, secret sources, reload-on-change | ✅ |
| 058 | AntiCorruptionLayer | translation between bounded contexts, no foreign model leak | ✅ |
| 059 | StranglerFigFacade | routing facade, legacy vs replacement, gradual cutover | ✅ |
| 060 | ArchitectureFitnessTests | dependency direction enforced by reflection over assembly metadata | ✅ |

## scale (061–073) — the system under load, and across instances

Where the first four blocks build one process that is correct, this block asks what
happens when there are many of them, all of them are busy, and the data no longer
fits on one machine.

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 061 | BulkheadIsolation | resource partitioning, one slow dependency not exhausting the pool | ✅ |
| 062 | ConcurrencyLimiter | admission control, load shedding, queue vs reject | ✅ |
| 063 | BackpressureBoundedQueue | bounded buffers, block vs drop vs shed, the unbounded-queue failure | ✅ |
| 064 | CostAwareBatching | batch size vs latency, flush triggers, partial-batch failure | ✅ |
| 065 | IdempotencyKeys 🐳 | HTTP idempotency keys, stored responses, replay vs re-execute | ✅ |
| 066 | LeaderElection 🐳 | lease-based single writer, renewal, loss of leadership mid-work | ✅ |
| 067 | DistributedScheduling | a job that runs once across N instances, missed and overlapping ticks | ✅ |
| 068 | GracefulShutdown | stop accepting, drain in flight, deadline, what to do with the rest | ✅ |
| 069 | StartupReadinessOrdering | dependency probing, fail-fast vs start-degraded, readiness gating | ✅ |
| 070 | ShardingByKey | shard routing, rebalancing, the keys that move | ✅ |
| 071 | ReadReplicaRouting | read/write splitting, replica lag, read-your-writes | ✅ |
| 072 | MultiTenancyIsolation | tenant-scoped access, the cross-tenant leak, the missing filter | ✅ |
| 073 | TenantConfiguration | per-tenant overrides over global defaults, inheritance, unknown tenants | ✅ |

## evolution (074–080) — the system changing while it runs

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 074 | ExpandContractMigration 🐳 | the three-phase schema change, why the middle phase exists | ✅ |
| 075 | IdempotentBackfill | resumable batch repair, checkpoints, re-running safely | ✅ |
| 076 | ConsumerDrivenContract | a contract test that fails when the provider breaks a consumer | ✅ |
| 077 | ApiDeprecationLifecycle | sunset dates, deprecation headers, usage before removal | ✅ |
| 078 | FeatureFlagTargeting | targeting rules, stable percentage bucketing, flag removal | ✅ |
| 079 | CanaryRelease | routing a fraction, comparing outcomes, automatic rollback | ✅ |
| 080 | ObservabilitySpans | parent/child spans, trace context across a boundary, sampling | ✅ |

## domain (081–090) — modelling the work, and the work that takes days

Blocks 01–06 are about the machinery. This one is about what the machinery is FOR:
the types the business talks in, and the processes that outlive a single request.

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 081 | ValueObjectInvariants | self-validating values, equality by value, no invalid instance | ✅ |
| 082 | EntityIdentity | identity vs equality, id generation as a port, unsaved entities | ✅ |
| 083 | AggregateTransactionBoundary | one aggregate per transaction, referencing others by id | ✅ |
| 084 | DomainServicePlacement | logic that belongs to no single entity, and what that costs | ✅ |
| 085 | WorkflowStateMachine | explicit states, illegal transitions refused, not a bool soup | ✅ |
| 086 | WorkflowTimeouts | a step waiting on the world, with a deadline and an escape | ✅ |
| 087 | HumanInTheLoop | pausing for a decision and resuming days later | ✅ |
| 088 | LongRunningOperation | 202 Accepted, a status resource, and where the result lives | ✅ |
| 089 | WebhookDelivery | signing, retries, replay protection, delivery order | ✅ |
| 090 | ChangeDataCapture | turning row changes into an event stream nobody has to write | ✅ |

## runtime (091–100) — resources, degradation, and staying up

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 091 | DeterministicByDesign | clock, ids and randomness as ports - the track's own idiom, taught | ⬜ |
| 092 | ConnectionPooling 🐳 | leasing, exhaustion, returning, detecting a leak | ⬜ |
| 093 | ObjectPoolReuse | reset on return, and the object that remembers the last caller | ⬜ |
| 094 | MultiLevelCache 🐳 | L1 and L2, promotion on hit, coherence between them | ⬜ |
| 095 | TagBasedInvalidation | invalidating by what a thing IS rather than by its key | ⬜ |
| 096 | GracefulDegradation | a fallback chain that reports which quality level answered | ⬜ |
| 097 | TimeoutBudget | one deadline, shrinking as it propagates down the call chain | ⬜ |
| 098 | PoisonPillDetection | telling a bad message apart from a bad day | ⬜ |
| 099 | ActorSingleThreadedOwnership | a mailbox instead of a lock, and the ordering it buys | ⬜ |
| 100 | SupervisionRestartStrategy | restart, backoff, give up, and one-for-one vs one-for-all | ⬜ |
