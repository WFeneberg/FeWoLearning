# MicroServices — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present, verified red and green) · ⬜ planned ·
🐳 needs a real container (`dotnet test -p:Containers=true`).

"Beginner" means **Aspire and distributed-systems** beginner, not C# beginner: ex001
models a resource graph, not a `FizzBuzz`. Plain C# language drills belong to the
`dotnet/` track; ASP.NET Core component work belongs to `blazor/`.

Weighting, agreed with the track owner: **001–055** Aspire and polyglot persistence,
**056–085** microservice patterns, **086–100** Docker and Azure. Azure is taught
entirely offline — emulators plus generated artifacts, no subscription.

Within that, the six segments are **001–035** the Aspire model and a first connection
to each everyday store, **036–055** persistence in depth, **056–070** communication,
**071–085** patterns, **086–090** Docker in depth, **091–100** Azure and deployment.

**The Concepts column is the spec.** Every row was written by asking what a *wrong*
implementation would do, per spec §8.3. The recurring failure here is a test that
grades a rendered value instead of the mechanism: `AddContainer("pg", "postgres")`
satisfies "there is a Postgres-ish container in the model" exactly as well as
`AddPostgres("pg")` does, and grades nothing. So persistence rows name **both** the
resource type and the shape of the `ConnectionStringExpression`, `WaitFor` rows name
`WaitAnnotation`, health-check rows name `HealthCheckAnnotation`, and rows about
asynchronous behaviour say what must be *waited for* rather than assumed. Where a row
reads oddly specific, that specificity is the grading hook — do not generalise it away
when the exercise is written.

Stubs live in `exercises/<tier>/ExNNN_<Slug>.cs` exposing a static
`Configure(IDistributedApplicationBuilder)` (plus whatever application code the row's
subject needs), their xunit tests in `tests/<tier>/ExNNN_<Slug>Tests.cs`, reference
solutions in `solutions/<tier>/`. Tier namespaces are
`FeWoLearning.MicroServices.Exercises.Beginner/.Intermediate/.Advanced/.Expert`,
because `01-beginner` is not a valid C# identifier.

Most rows are graded at **L1** (the in-process resource graph, ~1.4 s, no containers)
or **L2** (`aspire-manifest.json` generated in-process, ~3.7 s, no containers). The
**25** rows marked 🐳 are graded at **L3** — a real container starts and a real query,
message or expiry happens — and are skipped unless `-p:Containers=true`. See
`README.md` for what each level can and cannot prove.

Two deliberate content gaps, both recorded here so a later reader does not treat them
as oversights:

- **No live Azure.** Rows 091–100 assert emulators, the generated manifest and the
  generated Bicep. Nothing calls a subscription, `az login` or `azd up`, so every
  Azure row is red/green testable on a laptop with no cloud account.
- **No Docker Compose YAML in-process — and that is the *only* artifact missing.** The
  compose file is emitted by a pipeline the `aspire` CLI drives, and the CLI never exits
  in a non-interactive shell. Row 089 therefore asserts against a **committed golden
  `docker-compose.yaml`**, generated once at authoring time — the graded claim is that
  the learner's model is consistent with that file, not that the test re-runs the CLI.
  **Bicep needs no such fallback**: an in-process publish emits the `*.module.bicep`
  and per-resource `*.bicep` files directly (measured, §L2 in `README.md`), so rows
  093/094/099/100 assert on the generated Bicep for real.

**Status: 30 ✅ / 70 ⬜**

## Beginner (001–035) — Aspire model and first persistence

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | ContainerResourceBasics | `AddPostgres` yields a `PostgresServerResource`, `AddDatabase` a child `PostgresDatabaseResource`; assert both **types**, since a bare `AddContainer` also gives you two resources | ✅ |
| 002 | ReferenceVersusWaitFor | `WithReference` adds an `EnvironmentCallbackAnnotation`, `WaitFor` a `WaitAnnotation`; neither implies the other, and a consumer with both carries both | ✅ |
| 003 | EndpointsAndBindings | `WithHttpEndpoint`/`WithEndpoint`, `EndpointAnnotation.TargetPort` vs `Port` vs `IsExternal`; two endpoints on one resource must stay two annotations | ✅ |
| 004 | HealthChecksInTheModel | `WithHttpHealthCheck` writes a `HealthCheckAnnotation` whose key varies with the path; grade the annotation, never a 200 response | ✅ |
| 005 | ParametersAndSecrets | `AddParameter`, `ParameterResource.Secret`, and the `inputs.value.default.generate` policy a generated secret emits into the manifest | ✅ |
| 006 | ImageRegistryTagAndDigest | `WithImage`/`WithImageTag`/`WithImageRegistry`; assert `ContainerImageAnnotation.Image`, `.Tag` and `.Registry` **separately** — a full reference baked into one string is the wrong answer | ✅ |
| 007 | EnvironmentLiteralsAndCallbacks | `WithEnvironment(name, value)` vs the callback overload: a literal is fixed at model time, a callback runs per `EnvironmentCallbackContext` and can read another resource's endpoint | ✅ |
| 008 | ContainerArgsAndEntrypoint | `WithArgs`, `WithEntrypoint`, `CommandLineArgsCallbackAnnotation`; argument **order** is part of the assertion, because a set-equality test grades nothing | ✅ |
| 009 | VolumesAndBindMounts | `WithVolume` (named) vs `WithBindMount` (host path): `ContainerMountAnnotation.Type`, `Source`, `Target`, `IsReadOnly` — the two differ in type, not just in the source string | ✅ |
| 010 | ContainerLifetime | `WithLifetime(ContainerLifetime.Persistent)` and `ContainerLifetimeAnnotation`; why a persistent container survives an AppHost restart and a session one does not | ✅ |
| 011 | ProjectResources | `AddProject<Projects.X>`, `ProjectResource`, `IProjectMetadata.ProjectPath`, and the launch profile that supplies its endpoints | ✅ |
| 012 | ExecutableResources | `AddExecutable`, `ExecutableResource`, working directory and args; an executable carries **no** `ContainerImageAnnotation`, which is how a test tells the two apart | ✅ |
| 013 | ConnectionStringResources | `AddConnectionString` for a store Aspire does not host; `value.v0` in the manifest against a container's `container.v0` | ✅ |
| 014 | ParentAndChildResources | `IResourceWithParent`, a database resource's `Parent`, and why the child's expression interpolates `{parent.connectionString}` rather than repeating the host and port | ✅ |
| 015 | WaitForCompletion | `WaitForCompletion` on a one-shot migration/seed step vs `WaitFor` on a long-running server; assert `WaitAnnotation.WaitType`, since both produce a `WaitAnnotation` | ✅ |
| 016 | CustomAnnotationsAndExtensions | write an `IResourceAnnotation` plus an `IResourceBuilder<T>` extension; `WithAnnotation`, `TryGetLastAnnotation`, and why annotations are a list not a dictionary | ✅ |
| 017 | DashboardUrls | `WithUrl`/`WithUrlForEndpoint`, `ResourceUrlAnnotation`; the display text and the endpoint it decorates are two different fields | ✅ |
| 018 | ReplicasAndEndpointAllocation | `WithReplicas` (**project-only** — there is no container overload), `ReplicaAnnotation`, and what replicating does to endpoint allocation: the scaled resource must leave `EndpointAnnotation.Port` null so the **proxy** owns the one address in front of N instances, while a fixed **proxyless** port belongs only to a single-instance resource. Aspire polices neither, so the row grades the model's shape — and a model that pins no port anywhere passes the scaled half while teaching the wrong lesson, so both halves are named | ✅ |
| 019 | ExcludeFromManifest | a run-mode-only resource: present in the built model, **absent** from `aspire-manifest.json`; the row needs both assertions or it grades nothing | ✅ |
| 020 | RunVersusPublishMode | `builder.ExecutionContext.IsRunMode` / `IsPublishMode` branching one file into two graphs; the exercise fails if both modes produce the same model | ✅ |
| 021 | ServiceDefaults | `AddServiceDefaults`: health endpoints, service discovery, the standard resilience handler and OTel; assert the registrations in the `IServiceCollection`, not that the app started | ✅ |
| 022 | OpenTelemetryRegistration | Aspire injects `OTEL_EXPORTER_OTLP_ENDPOINT`/`OTEL_SERVICE_NAME` on its own, so those keys grade nothing. The learner's part is `WithTracing(t => t.AddSource(…))` / `WithMetrics(m => m.AddMeter(…))` for a **custom** `ActivitySource` and `Meter`: capture with an in-memory exporter, and an unregistered source must produce no spans | ✅ |
| 023 | LivenessVersusReadiness | `/alive` vs `/health`, `AddHealthChecks().AddCheck(..., tags:)` and tag-filtered endpoints; a readiness probe that reports live during startup is the bug being drilled | ✅ |
| 024 | ResourceCommands | `WithCommand`, `ResourceCommandAnnotation`, its `UpdateState` callback; a command whose state never depends on the resource is not doing the exercise | ✅ |
| 025 | EventingAndLifecycleHooks | `builder.Eventing.Subscribe<BeforeStartEvent>` / `ResourceReadyEvent`; where a hook fires relative to `WaitFor`, and why "ready" is not "started" | ✅ |
| 026 | SqlServerFirstConnection | `AddSqlServer(name, password)`, `AddDatabase`, `SqlServerDatabaseResource`; the `Server={host},{port};User ID=sa;…;TrustServerCertificate=true` expression no generic container produces — a COMMA between host and port where Postgres writes `;Port=`, a login fixed to `sa`, and the dev certificate trusted. The password parameter is the one part the learner determines, and the manifest must show it reaching the container as `MSSQL_SA_PASSWORD` with no generated `sqldata-password` left behind | ✅ |
| 027 | PostgresFirstConnection | `AddDatabase(name, databaseName)`: the RESOURCE name is the key a consumer reads (`ConnectionStrings__ordersdb`), `databaseName` is what lands in the string — two different things. Graded through what a referencing consumer actually receives: the seven `ORDERSDB_*` siblings (`_HOST`, `_PORT`, `_USERNAME`, `_PASSWORD`, `_DATABASENAME`, `_URI`, `_JDBCCONNECTIONSTRING`), which a hand-rolled `AddConnectionString` rendering the identical string does **not** produce. **Re-scoped 2026-09-06**: as originally written (`AddPostgres().AddDatabase()`, `PostgresDatabaseResource`, `{pg.connectionString};Database=orders`) this row was a near-duplicate of ex001 and ex014 | ✅ |
| 028 | MongoFirstConnection | `AddMongoDB().AddDatabase()`, `MongoDBDatabaseResource`, and the `mongodb://…?authSource=admin&authMechanism=SCRAM-SHA-256` URI decomposed as a URI — scheme, userinfo, authority, PATH SEGMENT for the database name, query — not a keyed connection string; plus the consequence only a URI has, the `annotated.string` / `filter: "uri"` resource the manifest interpolates in place of the raw password | ✅ |
| 029 | RedisFirstConnection | `AddRedis()` and `RedisResource`: a host:port with no scheme and **no database child resource**, which is exactly how Redis differs from the three above — graded in both directions against a Postgres control in the same model. Comma-separated StackExchange.Redis option syntax, and the scheme that does exist on the BINDING (`UriScheme` "redis") and not in the string | ✅ |
| 030 | DatabaseAdminTools | `WithPgAdmin`, `WithMongoExpress`, `WithRedisInsight`; each adds a *separate* container resource tied to its parent — assert the extra resource and the link, not a port number. The link is a `ResourceRelationshipAnnotation` (never `IResourceWithParent`), whose `Type` differs per integration (`PgAdmin` / `Parent` / `RedisInsight`), and the console names follow three different rules; none of the three reaches `aspire-manifest.json` | ✅ |
| 031 | DataVolumesPerFlavour | `WithDataVolume` vs `WithDataBindMount`, and that the container path is flavour-specific (`/var/lib/postgresql/data`, `/data/db`, `/var/opt/mssql`) — one shared constant is wrong | ⬜ |
| 032 | DatabaseInitScripts | `WithInitBindMount`/`WithInitFiles` targeting `/docker-entrypoint-initdb.d`; the mount annotation plus the `WaitFor` ordering that makes the script run before a consumer connects | ⬜ |
| 033 | ClientIntegrationRegistration | `AddNpgsqlDataSource("orders")` reading `ConnectionStrings:orders`; build the service's configuration with a **sentinel** value and assert the registered data source carries it, then remove the key and assert registration fails — a hardcoded connection string fails both halves | ⬜ |
| 034 | FirstRealQuery | start Postgres for real, `WaitFor` it, and execute a query through the *injected* connection string — proves the expression resolves, which no model-level test can | 🐳 ⬜ |
| 035 | BeginnerCapstoneModel | catalog + orders + reviews + cache in one graph: four resource types, four distinct connection expressions, and consumers carrying both `WithReference` and `WaitFor` | ⬜ |

## Intermediate (036–070) — persistence in depth, then communication

### Persistence in depth (036–055)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | EfCoreAgainstSqlServer | `AddSqlServer` + `AddDatabase` → `SqlServerDatabaseResource` and its `…;Initial Catalog=catalog` expression, wired to `UseSqlServer` over the injected `ConnectionStrings:catalog`; the live SQL Server proof lives in 038 and 040, so this row is the wiring | ⬜ |
| 037 | EfCoreAgainstPostgres | the same `DbContext` on Npgsql; compare the two providers' **generated migration scripts** offline — `nvarchar`/`datetime2`/`IDENTITY` against `text`/`timestamptz`/`GENERATED … AS IDENTITY` | ⬜ |
| 038 | MigrationsOnStartup | `Database.MigrateAsync` from a hosted service, gated on `WaitFor`; the second start must apply **zero** migrations, which is the assertion that catches a naive `EnsureCreated` | 🐳 ⬜ |
| 039 | SeedDataInTheModel | `HasData` in `OnModelCreating` vs an upsert at startup; grade the `INSERT`s in the generated migration and the seed's re-run safety, not the row count after one start | ⬜ |
| 040 | TransactionsAndConcurrency | `IExecutionStrategy` wrapping an explicit transaction, a `rowversion`/`xmin` concurrency token, and a genuine `DbUpdateConcurrencyException` from two interleaved updates | 🐳 ⬜ |
| 041 | MySqlIdentifiersAndCollation | `AddMySql`, `MySqlDatabaseResource` and its own expression; then the schema differences — `utf8mb4` collation, identifier case sensitivity, index prefix lengths — read off the generated DDL | ⬜ |
| 042 | OracleSchemaSemantics | `AddOracle`, `OracleDatabaseResource`; a *schema is a user*, identifiers are folded and length-capped, and keys come from sequences — the relational habits from 036 do not transfer | ⬜ |
| 043 | MongoDocumentModel | embedding vs referencing, `_id`/`ObjectId`, `BsonDocument` against a mapped POCO; the row is graded on the stored **document shape**, so a normalised set of collections fails | ⬜ |
| 044 | MongoIndexesAndExplain | `CreateIndexModel`, a compound index, and `explain()` reporting `IXSCAN` rather than `COLLSCAN` — the query must prove the index was *used*, since a correct result proves nothing | 🐳 ⬜ |
| 045 | MongoAggregationPipeline | `$match`/`$unwind`/`$group`/`$lookup` executed server-side; assert the emitted pipeline stages **and** the documents, because an in-memory LINQ `GroupBy` returns the same answer | 🐳 ⬜ |
| 046 | RedisExpiryAndEviction | `SET … EX`, `TTL`, a key that genuinely disappears, and `maxmemory-policy` deciding what is dropped under pressure; the subject is what Redis *forgets* | 🐳 ⬜ |
| 047 | CacheAsideAndStampede | read-through cache-aside with negative caching for misses and a jittered TTL, plus a `SET NX PX` lock so N **concurrent** misses invoke the loader exactly once and the lock is released; assert the loader's invocation count — naive cache-aside calls it N times | 🐳 ⬜ |
| 048 | ValkeyAndGarnetForks | `AddValkey` → `ValkeyResource` on `valkey/valkey` and `AddGarnet` → `GarnetResource` on Microsoft's image: one `AddRedis` fails the type assertion and the connection expressions cannot tell them apart, so the row also drives both with one client to find where Garnet's command coverage stops short | 🐳 ⬜ |
| 049 | QdrantVectorSearch | a collection with a named vector and an explicit distance metric, a payload filter combined with top-k; changing Cosine to Dot must change the returned order — an unordered assertion grades nothing | 🐳 ⬜ |
| 050 | MilvusCollectionsAndIndexes | schema with a primary key plus a `FloatVector` field, an `IVF_FLAT`/`HNSW` index, and `load()` before search — an unloaded collection returns nothing, which is the trap | 🐳 ⬜ |
| 051 | ElasticsearchAnalyzersAndScoring | a custom analyzer, a `match` query against a `term` query on the same field, and `_score` ordering; full-text search is not `LIKE '%x%'` and the scores must show it | 🐳 ⬜ |
| 052 | AzuriteQueuesAndTables | `AddAzureStorage().RunAsEmulator()` driven for real: a queue message's visibility timeout and `DequeueCount`, and a Table entity found by `PartitionKey`+`RowKey` against a cross-partition scan | 🐳 ⬜ |
| 053 | CosmosContainersAndKeys | `AddAzureCosmosDB().RunAsEmulator()` with database and container children; grade the partition key paths as they reach the **generated Bicep** — a single-path model fails the hierarchical-key case, which is the discriminating half. RU cost is deliberately out of scope: it needs the emulator, which is slow and flaky on Linux | ⬜ |
| 054 | ResilienceAndPooling | provider retry (`EnableRetryOnFailure`) against a Polly pipeline, and `MaxPoolSize` exhaustion; retrying a *non-transient* failure is the bug — the row must distinguish the two | ⬜ |
| 055 | PolyglotPersistenceCapstone | one order flow split across relational, document and cache stores, with a stated owner per fact; the graph carries all three resource types and their three distinct expressions | ⬜ |

### Communication (056–070)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 056 | ServiceDiscoveryBasics | `WithReference` on a project resource, the `services__catalog__https__0` env keys it writes, and `https+http://catalog` resolving through them | ⬜ |
| 057 | TypedHttpClients | `AddHttpClient<CatalogClient>` with a `https+http://` base address; a hand-constructed `HttpClient` bypasses discovery entirely, so the row asserts the resolved handler chain | ⬜ |
| 058 | ResiliencePipelines | `AddStandardResilienceHandler` and its **order**: retry outside a per-attempt timeout behaves differently from retry inside a total timeout, and the row grades which one was built | ⬜ |
| 059 | GrpcServiceAndClient | a `.proto` contract, `AddGrpc`, `AddGrpcClient<T>` over discovery; assert the `StatusCode` (`NotFound` vs `InvalidArgument`), because gRPC's HTTP status is 200 either way | ⬜ |
| 060 | GrpcStreaming | server-streaming and bidirectional calls, `IAsyncStreamReader`, and cancellation **mid-stream** — a test that reads to completion never exercises the interesting half | ⬜ |
| 061 | RabbitMqPublishConsume | `AddRabbitMQ`, exchange/queue/binding declared explicitly, and a message actually consumed; the assertion must await delivery, since a test that never waits passes against a no-op publisher | 🐳 ⬜ |
| 062 | RabbitMqRoutingAndDeadLetters | topic-exchange routing keys with wildcards, `x-dead-letter-exchange`, and a `Nack(requeue:false)` message arriving in the DLQ — requeue-forever looks identical until you look at the DLQ | 🐳 ⬜ |
| 063 | KafkaTopicsAndPartitions | `AddKafka`, an explicit partition count, and key-based placement: the same key must land on one partition and preserve order, while different keys need not — assert partition ids | 🐳 ⬜ |
| 064 | KafkaConsumerGroupsAndOffsets | consumer groups, `enable.auto.commit=false`, an explicit commit, and replay from the committed offset after a restart; at-least-once and at-most-once must be told apart | 🐳 ⬜ |
| 065 | NatsCoreVersusJetStream | `AddNats`: core pub/sub drops messages for an absent subscriber, JetStream replays them; the row is graded by a subscriber that was **offline** during publish | 🐳 ⬜ |
| 066 | MessageContractsAndVersioning | an explicit envelope (type name, version, content type); the consumer is graded against a **JSON literal the test itself builds**, with no reference to the producer's type, so the envelope's `type`+`version` must select the handler — a shared C# class cannot satisfy that setup | ⬜ |
| 067 | SeqStructuredLogging | `AddSeq` plus `WithReference`; log properties must arrive as **fields**, so an interpolated message string with the values baked in fails even though the text matches | ⬜ |
| 068 | BaggageAndSpanEnrichment | `traceparent` propagates on its own once ServiceDefaults is in, so it grades nothing. `Baggage` does **not** propagate usefully by itself: set a tenant id on the caller, read it via `Baggage.GetBaggage` on the callee, and tag a span from the learner's own `ActivitySource` with it — the tag is absent under automatic propagation alone | ⬜ |
| 069 | CancellationPropagation | the request's `CancellationToken` reaching the downstream HTTP call and the database command; a handler that ignores the token still returns the right answer, so assert the downstream *observed* the cancel | ⬜ |
| 070 | CommunicationCapstone | HTTP command → broker event → gRPC read in one flow, with the trace crossing all three hops and the broker hop carrying its context explicitly | ⬜ |

## Advanced (071–090) — patterns, then Docker

### Patterns (071–085)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | TransactionalOutbox | the outbox row written in the **same transaction** as the state change; roll the transaction back and no message may exist — a publish-after-commit implementation passes any weaker test | 🐳 ⬜ |
| 072 | OutboxDispatcherAndOrdering | a dispatcher claiming rows with `FOR UPDATE SKIP LOCKED`, preserving per-aggregate order, with two dispatchers running concurrently and no message sent twice | 🐳 ⬜ |
| 073 | IdempotentConsumer | a dedupe key stored in the *same* transaction as the effect; redeliver the identical message and the effect must not repeat — dedupe committed separately fails under a crash between the two | 🐳 ⬜ |
| 074 | InboxAndTheExactlyOnceIllusion | "exactly once" is at-least-once delivery plus an idempotent effect; the row grades the **effect** being deduplicated, and must reject a solution that claims to deduplicate delivery | ⬜ |
| 075 | SagaChoreography | order → payment → shipping by events with no coordinator; a failing step's **compensating event** must actually be published and consumed, not merely logged | 🐳 ⬜ |
| 076 | SagaOrchestration | a process manager with persisted state, a timeout step, and a retry that resumes at the failed step — assert the state machine's transitions, so a re-run-everything retry fails | ⬜ |
| 077 | CompensatingActions | compensation is not rollback: a refund is a new fact with its own id and timestamp; both the original and the compensating record must survive, so a delete fails the row | ⬜ |
| 078 | CqrsReadModel | writes to the relational store, a projection maintaining a document read model, and read-your-writes **not** holding; assert convergence by bounded polling, never by sleeping past the lag | 🐳 ⬜ |
| 079 | EventVersioning | an upcaster from v1 to v2, a tolerant reader ignoring unknown fields, additive-only schema change; a strict deserializer must fail on the v1 payload the test replays | ⬜ |
| 080 | EventualConsistencyAndTime | convergence asserted with a bounded retry and a deadline, plus a controllable clock; the row exists because a test that never advances time passes against a do-nothing projection | ⬜ |
| 081 | TracingAcrossTheBroker | `traceparent` carried in **message headers** with `ActivityKind.Producer`/`Consumer` and a link; HTTP propagation is automatic, broker propagation is hand-written, and the row grades the hand-written half | ⬜ |
| 082 | CorrelationAndCausation | a correlation id constant across a whole flow against a causation id naming the immediate parent; assert the parent chain, since "every message has some Guid" is satisfied by noise | ⬜ |
| 083 | BulkheadAndIsolation | isolating a slow dependency with a concurrency limiter so it cannot exhaust the shared pool; the assertion is that the *unrelated* endpoint stayed responsive while the slow one saturated | ⬜ |
| 084 | GatewayAndBff | YARP routes and clusters over discovered services, then a BFF shaping one screen's payload; a pass-through proxy is not a BFF, so the row grades the response **shape**, not the routing | ⬜ |
| 085 | ConsistencyModelsCapstone | one flow choosing per step: strong inside an aggregate boundary, eventual across services, with the boundary made explicit in code and asserted at both sides | ⬜ |

### Docker in depth (086–090)

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 086 | AddDockerfileResource | `AddDockerfile` with a build context and `WithBuildArg`; the manifest entry is `dockerfile.v0` with `path`/`context`/`buildArgs`, not `container.v0` — that distinction is the grade | ⬜ |
| 087 | MultiStageBuildAndHardening | an SDK build stage and a runtime stage; ordering `COPY *.csproj` + `restore` before the source copy so a code-only change rebuilds **without** re-restoring, proven by two real builds. Plus the hardening the runtime stage must carry — a non-root `USER` and base images pinned by **digest**, not tag — asserted from the built image's config, because a container that works fine also works fine as root | 🐳 ⬜ |
| 088 | ContainerNetworksAndDns | containers resolving each other by service name on the `aspire` network, and `localhost` inside a container not being the host; the row needs a real run because the model cannot show DNS | 🐳 ⬜ |
| 089 | ComposePublishingGolden | `AddDockerComposeEnvironment`, checked against the committed golden `docker-compose.yaml`: service names, pinned images, `expose` ports, the `aspire` network, `ConnectionStrings__*`, and secrets lifted into `.env` as `${X_PASSWORD}` | ⬜ |
| 090 | VolumeLifetimeAndPersistence | data surviving `docker rm` behind a **named** volume, a bind mount reflecting host edits live, and an anonymous volume lost with `--rm`; only a real restart distinguishes the three | 🐳 ⬜ |

## Expert (091–100) — Azure and deployment

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | AzuriteInTheModel | `AddAzureStorage` with `RunAsEmulator()` and Blob/Queue/Table children: the same `AzureStorageResource` swaps its run-mode container while publish emits Bicep — assert both halves, not just the emulator | ⬜ |
| 092 | AzureContainerAppEnvironment | `AddAzureContainerAppEnvironment`, the `azure.bicep.v0` entries it puts in the manifest, and its module path/parameters; a compute environment with no resources assigned is the failure of row 095 | ⬜ |
| 093 | ManagedIdentityAndRoleAssignments | `WithRoleAssignments` producing a user-assigned identity and a role assignment in the generated Bicep; a connection **secret** still present in the env means the row failed, so assert its absence too | ⬜ |
| 094 | BicepCustomization | `AddAzureInfrastructure` / `ConfigureInfrastructure` mutating the generated resource before emit; the assertion is on the **emitted Bicep**, since C# that compiles proves nothing about the output | ⬜ |
| 095 | TwoComputeEnvironments | compose and ACA declared together, with every compute resource explicitly assigned; unassigned resources fail the `validate-compute-environments` pipeline step, and that failure is the row's negative case | ⬜ |
| 096 | MultiEnvironmentModels | publish-time parameters driving one model into two manifests that differ **only** where intended; a diff-everything assertion is useless, so the row names the fields allowed to change | ⬜ |
| 097 | CustomAspireResource | a bespoke resource implementing `IResourceWithConnectionString` and `IResourceWithEndpoints`, its own `WithX` extension, and a manifest-publishing callback emitting a stable custom type | ⬜ |
| 098 | PublishPipelineSteps | the publish pipeline's steps and their order, adding one of your own, and making it fail deliberately — the row grades where in the pipeline the failure surfaced | ⬜ |
| 099 | SecretsAcrossEnvironments | a generated parameter's `generate` policy in run mode against a Key Vault reference in publish mode; a hard-coded password works locally and must fail this row's publish-mode assertion | ⬜ |
| 100 | DeploymentCapstone | one graph published twice — compose against the golden file, and ACA Bicep — with assignments, identities and per-environment parameters all correct in both | ⬜ |
