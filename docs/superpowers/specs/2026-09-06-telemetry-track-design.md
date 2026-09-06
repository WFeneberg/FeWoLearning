# Telemetry Track — Design

**Date:** 2026-09-06
**Status:** approved, implementation pending
**Folder:** `telemetry/` (lowercase, like `dotnet/`, `security/`, `python/` — not the
capitalised deviation `MicroServices/` and `Architecture/` take)

## 1. What this track is

70 graded C# exercises in **application instrumentation** on .NET 10: `ILogger`,
`System.Diagnostics.Activity`/`ActivitySource`, `System.Diagnostics.Metrics.Meter`,
the OpenTelemetry SDK, exporters, sampling, propagation — across the four surfaces
the owner ships into: **web** (ASP.NET Core), **services** (hosted services, HTTP
clients, queues, databases), **desktop** (WPF) and **containers** (env-var
configuration, resource detection, graceful shutdown).

It teaches the instrumentation itself, not the topology it runs in and not the
architecture it hangs off. Three existing tracks touch the subject; the boundary is
drawn deliberately in each case:

| Existing | What lives there | What lives here |
|---|---|---|
| `MicroServices/` 021, 022, 023, 067, 068, 070, 081 | OTel *inside Aspire* — what the orchestrator injects on its own, and the deployment topology | The SDK itself: providers, processors, samplers, views, propagators, exporters |
| `Architecture/` 028, 055, 056 | Telemetry as a **port** — keeping the domain logger-free, correlation as an architectural concern | What happens behind the port |
| `security/` 051 | Redaction as an **attack surface** (log injection, secret leakage) | Redaction as a **producer obligation**: structural scrubbing of the log state, not a regex over the rendered text |

Where two tracks touch the same API, this track owns the mechanism and the other
owns its context.

## 2. Repository shape

```
telemetry/
  FeWoLearning.Telemetry.slnx
  Directory.Build.props
  catalog.md                          # 70-row ledger
  README.md
  exercises/FeWoLearning.Telemetry.Exercises.csproj
    01-logging/ 02-diagnostics/ 03-otel-sdk/ 04-web-services/ 05-desktop-ops/ _support/
  solutions/FeWoLearning.Telemetry.Solutions.csproj
    (same five block folders, same relative paths, same type names)
  tests/FeWoLearning.Telemetry.Tests.csproj
    (same five block folders) _harness/
```

Three projects, and `solutions/` is deliberately **in** the build — the same waiver
`avalonia/`, `blazor/`, `uno/`, `caliburn/`, `wpf/`, `security/` and `Architecture/`
take. `exercises/` and `solutions/` compile the same type names into the same
namespaces; `tests/` references **exactly one** of them via the `UseSolutions`
MSBuild property, so the name collision the repo-wide convention exists to prevent
cannot occur, and reference solutions are compile-checked and test-run on every
green check instead of drifting silently.

`Directory.Build.props` must redirect the solutions build's output via
`UseArtifactsOutput`/`ArtifactsPath`. Required, not cosmetic: two projects emitting
the same generated assembly-info attributes into a shared `obj/` tree fail the build
with `CS0579`. It has to live in `Directory.Build.props`, not in the `.csproj` body,
where `BaseOutputPath`/`BaseIntermediateOutputPath` are read after the SDK props
import and therefore too late.

**Namespaces are pinned per block**, because `01-logging` is not a valid C#
identifier:

| Folder | Namespace |
|---|---|
| `01-logging` | `FeWoLearning.Telemetry.Exercises.Logging` |
| `02-diagnostics` | `FeWoLearning.Telemetry.Exercises.Diagnostics` |
| `03-otel-sdk` | `FeWoLearning.Telemetry.Exercises.Otel` |
| `04-web-services` | `FeWoLearning.Telemetry.Exercises.WebServices` |
| `05-desktop-ops` | `FeWoLearning.Telemetry.Exercises.DesktopOps` |

Test namespaces mirror these as `FeWoLearning.Telemetry.Tests.<Block>`.

**Block `03` is `.Otel`, not `.OpenTelemetry`, and that is deliberate.** The
namespace-shadowing trap `avalonia/` and `caliburn/` both record would otherwise land
here in its worst form: inside a namespace ending in `OpenTelemetry`, a fully
qualified `OpenTelemetry.Trace.Sampler` resolves its leading segment to the enclosing
namespace and fails `CS0234`, and the workaround is a `using` alias or
`global::OpenTelemetry.…` on every such reference. Since this is the one block where
learners type that root namespace constantly — including verbatim out of stub TODO
strings — the fix is to not create the collision at all. Do not "tidy" the namespace
back to `.OpenTelemetry` later.

`_support/` (identical in both content libraries) holds fixtures several exercises
share — an in-memory message bus, a controllable clock, a fake outbound HTTP handler.
It is never a TODO and never gets a `catalog.md` row.

## 3. Toolchain

**Target framework `net10.0-windows` with `<UseWPF>true</UseWPF>` for all three
projects, plus `<FrameworkReference Include="Microsoft.AspNetCore.App" />` on the two
content libraries.** This is exactly `security/`'s shape — web surfaces and WPF
surfaces in one project trio — and it is what makes block `05-desktop-ops` able to
drill real WPF mechanisms (`Dispatcher`, `DispatcherUnhandledException`,
`PresentationTraceSources`) instead of a UI-framework-free stand-in.

The cost is stated plainly: **the whole track is Windows-only**, the same trade
`wpf/`, `caliburn/` and `security/` already make. Rows 059–062 additionally need an
STA thread, supplied by `[WpfFact]`; none of them opens a window, so unlike
`caliburn/` this track should not require an interactive desktop session — that
claim is unverified until the first desktop batch runs and must be recorded in
`telemetry/README.md` as measured, not assumed.

**WPF only, no Avalonia.** The `avalonia/` track installs a headless `Application`
process-wide from a `[ModuleInitializer]`; combining that with WPF's `Dispatcher` in
one assembly is a fight over process globals with no upside. An Avalonia block can be
added later as its own project pair if it is ever wanted.

Test stack: **xunit.v3 3.2.2 + `xunit.runner.visualstudio` 3.1.5 +
`Microsoft.NET.Test.Sdk` 17.14.1 + `Xunit.StaFact` 3.0.13, and NO `global.json`** —
the classic VSTest path.

Do **not** copy `security/`'s pin here. xunit.v3 4.0.0 plus a
`Microsoft.Testing.Platform` `global.json` is the combination measured on this
machine to make `dotnet test` exit 5 with zero tests discovered (see CLAUDE.md's
`MicroServices/` toolchain entry). `Xunit.StaFact` 4.x *requires* xunit.v3 4.0.0, so
staying on `Xunit.StaFact` 3.0.13 is what keeps this track on the working path —
`caliburn/`'s measured pairing, for the identical reason. `xunit.runner.visualstudio`
has no 3.1.6 or 3.1.7; naming one resolves *forward* to 4.0.0 with only an `NU1603`
warning, silently landing back on the broken generation without an error to catch it.

Package set (exact versions resolved and pinned at scaffold time by a real restore,
never from memory):

- Content libraries: `OpenTelemetry`, `OpenTelemetry.Extensions.Hosting`,
  `OpenTelemetry.Instrumentation.AspNetCore`, `OpenTelemetry.Instrumentation.Http`,
  `OpenTelemetry.Instrumentation.Runtime`,
  `OpenTelemetry.Exporter.OpenTelemetryProtocol`, `OpenTelemetry.Exporter.Console`,
  `OpenTelemetry.Exporter.Prometheus.AspNetCore`, `Serilog.Extensions.Logging` plus
  the file sink (rows 012–013 only), `Polly.Core` (row 053), and
  `Microsoft.Data.Sqlite` with `SQLitePCLRaw.lib.e_sqlite3` pinned to 2.1.13 or later
  if a row needs it — the transitive 2.1.11 carries GHSA-2m69-gcr7-jv3q and emits
  `NU1903` on every build.
- Tests: `OpenTelemetry.Exporter.InMemory`,
  `Microsoft.Extensions.Diagnostics.Testing` (`FakeLogger`, `FakeLogCollector`,
  `MetricCollector<T>`), `Microsoft.AspNetCore.TestHost`, and the `Testcontainers.*`
  family for the 🐳 rows.
- **Never** `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.Logging`,
  `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Configuration`,
  `Microsoft.Extensions.Options` or `System.Security.Cryptography.ProtectedData` as
  `PackageReference`s: all are already in the shared framework for this TFM once
  `Microsoft.AspNetCore.App` is referenced, and referencing them emits `NU1510`.

Commands, run from inside `telemetry/`:

| Run | Command |
|---|---|
| Stubs (red) | `dotnet test` |
| Solutions (green) | `dotnet test -p:UseSolutions=true` |
| Including container rows | `dotnet test -p:Containers=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |

## 4. Catalog structure — 70 rows in 5 blocks

The track departs from the repo's 100-row / four-difficulty-tier scheme for the same
reason `security/` and `Architecture/` do: "beginner telemetry" is not a meaningful
axis. A histogram bucket boundary is not conceptually harder than a log scope; they
are different concerns. Difficulty rises **within** each block.

Container-gated rows are marked 🐳: they carry *additional* facts needing Docker,
skipped by default. Every such exercise is still fully graded without Docker by its
in-process facts — the container facts add realism, never coverage that would
otherwise be missing.

### `01-logging` (001–014) — `ILogger` and the structured-logging contract

| # | Slug | Concepts |
|---|---|---|
| 001 | StructuredMessageTemplate | message template vs interpolated string; named fields survive into the log state |
| 002 | LogLevelsAndFiltering | `IsEnabled`, filter rules by category and level, per-provider minimum level |
| 003 | CategoriesAndTypedLogger | `ILogger<T>` category naming, `CreateLogger(string)`, category-based filtering |
| 004 | LoggingScopes | `BeginScope`, nesting, scope state on the record, providers must opt in |
| 005 | LoggerMessageSourceGenerator | `[LoggerMessage]` partial method, `EventId`, compile-time template |
| 006 | HighPerformanceGuardClause | `IsEnabled` guard, `LoggerMessage.Define`, avoiding boxing on the disabled path |
| 007 | ExceptionLogging | the exception argument vs `ex.ToString()` in the message; inner and aggregate exceptions |
| 008 | CustomILoggerProvider | a provider + logger + `ISupportExternalScope` written by hand |
| 009 | RedactionAndPii | structural scrubbing of named fields, not a regex over the rendered message |
| 010 | LogEnrichment | ambient properties (version, tenant, machine) attached once, not per call site |
| 011 | EventIdConventions | stable ids, id/name pairing, filtering by `EventId` |
| 012 | SerilogStructuredSink | Serilog behind `ILogger`, destructuring with `@`, properties as fields |
| 013 | SerilogRollingFileAndOverrides | rolling file sink, `MinimumLevel.Override` per source, retention |
| 014 | LogSamplingAndRateLimit | suppressing repetition, first-N-per-window, virtual clock |

### `02-diagnostics` (015–026) — the BCL primitives, before any SDK

| # | Slug | Concepts |
|---|---|---|
| 015 | ActivitySourceAndListener | `StartActivity` returns **null** with no listener; `ActivityListener`, `Sample` → `AllData` |
| 016 | ActivityParentChild | nesting, `Parent`/`ParentId`, `Activity.Current` restored on `Dispose` |
| 017 | TagsBaggageEvents | `SetTag` vs `AddBaggage` (baggage inherits down, tags do not), `AddEvent` |
| 018 | StatusAndException | `SetStatus`, `AddException`, the error tag convention |
| 019 | KindAndLinks | `ActivityKind` Client/Server/Producer/Consumer, `ActivityLink` for fan-in |
| 020 | W3CTraceContext | `traceparent`/`tracestate` parse and format, `ActivityContext` round-trip, id format |
| 021 | MeterAndCounter | `Meter`, `Counter<T>`, `MeterListener`, tags as dimensions |
| 022 | HistogramAndBuckets | `Histogram<T>`, what a distribution can and cannot answer |
| 023 | ObservableInstruments | `ObservableGauge`/`ObservableCounter` pull model, `UpDownCounter` vs `Counter` |
| 024 | MeterListenerLifecycle | `InstrumentPublished`, `EnableMeasurementEvents`, dispose, no double counting |
| 025 | EventSourceAndCounters | a custom `EventSource`, keywords and levels, `EventListener`, runtime counters |
| 026 | DiagnosticSourceListener | `DiagnosticListener`, `IsEnabled`, anonymous payloads, the framework's own events |

### `03-otel-sdk` (027–044) — the OpenTelemetry SDK

| # | Slug | Concepts |
|---|---|---|
| 027 | TracerProviderBuilder | `AddSource`, `Build`, in-memory exporter; an unregistered source produces nothing |
| 028 | ResourceAttributes | `service.name`/`version`/`instance.id`, `ResourceBuilder`, env-var detection |
| 029 | SpanProcessors | simple vs batch export, ordering, a custom processor enriching in `OnStart`/`OnEnd` |
| 030 | Samplers | AlwaysOn/AlwaysOff/TraceIdRatioBased/ParentBased; sampled flag vs recording |
| 031 | CustomSampler | implementing `Sampler`, `Drop` vs `RecordOnly` vs `RecordAndSample` |
| 032 | MeterProviderAndReader | `AddMeter`, periodic vs manual `Collect`, in-memory metric exporter |
| 033 | MetricViews | rename, drop, tag-key selection for cardinality control, explicit histogram bounds |
| 034 | MetricTemporality | Delta vs Cumulative across two collections |
| 035 | Exemplars | a measurement carrying the trace id that produced it, exemplar filtering |
| 036 | OtelLogsPipeline | `AddOpenTelemetry` logging, `LogRecord` fields, formatted message vs state |
| 037 | LogTraceCorrelation | `LogRecord.TraceId` matching `Activity.Current`, `ParseStateValues`, `IncludeScopes` |
| 038 | ContextPropagators | `TextMapPropagator` inject/extract, the composite default |
| 039 | BaggagePropagation | baggage across a boundary, why it is not a span tag until you make it one |
| 040 | SemanticConventions | stable attribute names; why hand-rolled names silently break every dashboard |
| 041 | InstrumentationLibraries | `AddHttpClientInstrumentation`/`AddAspNetCoreInstrumentation`: what is automatic vs yours |
| 042 | OtlpExporterConfiguration 🐳 | endpoint, protocol, headers, env-var precedence; a real collector receives the span |
| 043 | PrometheusScrapeEndpoint 🐳 | the text exposition format, name mangling and the `_total` suffix |
| 044 | ShutdownAndFlush | `ForceFlush`/`Shutdown`, the loss window, disposal order |

### `04-web-services` (045–058) — ASP.NET Core, HTTP clients, queues, databases

| # | Slug | Concepts |
|---|---|---|
| 045 | AspNetCoreServerSpan | a server span per request via `TestHost`; the **route template**, not the raw path, as the span name |
| 046 | SpanEnrichmentAndFiltering | enriching the server span; suppressing `/health` noise at the source |
| 047 | HttpClientPropagation | the outgoing `traceparent`, parent/child across the process boundary, a `DelegatingHandler` |
| 048 | QueuePropagationProducerConsumer | inject into message headers, extract on the consumer, Producer/Consumer kinds and a link |
| 049 | BackgroundServiceInstrumentation | one root activity **per iteration**, not one per service lifetime |
| 050 | RedMetricsAndCardinality | rate/errors/duration by `http.route`; a tag cardinality budget |
| 051 | DatabaseInstrumentation 🐳 | `db.system`/`db.query.text`, and why parameter values must never reach the span |
| 052 | HealthChecksAndProbes | composition, tag-filtered liveness vs readiness, check results as metrics |
| 053 | ResilienceTelemetry | Polly retries visible as events and metrics; the retry-hides-the-failure bug |
| 054 | ErrorStatusAndProblemDetails | exception → span status → correlated log, never swallowed |
| 055 | HttpRequestLogging | request logging duration measured at the right place; query-string PII |
| 056 | CollectorPipeline 🐳 | an OTel Collector container with a processor; what the collector can fix that the app cannot |
| 057 | BackendIngestion 🐳 | structured logs into a real backend; fields are queryable, a baked-in message is not |
| 058 | MultiHopTraceCapstone | HTTP → in-process queue → background worker, one trace across all three hops |

### `05-desktop-ops` (059–070) — WPF, and running in a container

Rows 059–062 are `[WpfFact]` (STA thread). None opens a window.

| # | Slug | Concepts |
|---|---|---|
| 059 | DispatcherLatencyMetric | UI-thread queue latency as a histogram, measured on the `Dispatcher` |
| 060 | UnhandledExceptionCapture | `DispatcherUnhandledException`, `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException` — one record, not three or none |
| 061 | CommandActivityTracing | an activity spanning a user command, surviving the dispatcher hop and `await` |
| 062 | BindingErrorMonitoring | `PresentationTraceSources` binding failures turned into structured logs |
| 063 | StartupPerformanceTracing | cold-start phases as nested activities |
| 064 | OfflineBufferAndReplay | a bounded buffer while the exporter is unreachable, replay on reconnect, explicit drop policy |
| 065 | FlushOnShutdown | flushing on exit; the size of the window that is lost when you do not |
| 066 | PiiScrubbingAndConsent | user names, paths and machine identifiers; an opt-in consent flag that actually gates emission |
| 067 | SessionCorrelation | a stable anonymous install/session id tying one run's logs, traces and metrics together |
| 068 | LocalRollingFileAndSupportBundle | a size-capped rolling local log and a support bundle a user can send |
| 069 | ContainerResourceDetection 🐳 | `OTEL_*` env configuration, container resource attributes, stdout JSON for the log driver |
| 070 | GracefulShutdownFlushInContainer 🐳 | SIGTERM, `IHostApplicationLifetime`, draining the batch processor before the process dies |

## 5. The recurring bug class

`security/`'s lesson was "an attack fact with no paired use fact grades nothing".
This track's is: **a telemetry test that asserts the rendered result instead of the
structure grades nothing.** Six specific ways such a test lies. Each is written out
in `telemetry/README.md` under "How a telemetry test lies", and every row is checked
against the list before its catalog row is flipped.

1. **Text instead of fields.** `$"user {id} failed"` produces a byte-identical
   formatted message to `"user {UserId} failed", id`, and carries not one named
   field. A test asserting the message string grades nothing about structure. Assert
   the structured state — `FakeLogRecord.StructuredState`, `LogRecord` attributes.
2. **Existence instead of lineage.** "A span exists" is satisfied by automatic
   instrumentation the learner never wrote, and by a span with the wrong parent.
   Assert `Source.Name`, `ParentSpanId` and `ActivityKind`, not the count.
3. **Registration instead of emission.** An exporter present in the service
   collection proves nothing; only the in-memory exporter's contents prove data
   flowed. (`MicroServices/` spec §8.2 is the same finding in another key.)
4. **Collecting once.** A single `Collect` cannot distinguish Delta from Cumulative,
   cannot show a counter is monotonic, and cannot catch double counting from a leaked
   listener. Collect twice and compare.
5. **A free pass from automatic instrumentation.** `traceparent` propagates over
   HTTP by itself — `MicroServices/` rows 068 and 081 measured exactly this. Every
   row here grades the learner's **own** `ActivitySource`, `Meter` or handler, and
   any row whose subject is automatic must assert something automation does not do.
6. **Process-global contamination.** An `ActivityListener` sees activities from every
   test running concurrently. `Sdk.SetDefaultTextMapPropagator`,
   `Activity.DefaultIdFormat`, `Activity.ForceDefaultIdFormat`, `Activity.Current`
   and `static Meter` fields are all process-wide.

Two mitigations for (6), both mandatory:

- `[assembly: CollectionBehavior(DisableTestParallelization = true)]` in
  `tests/_harness/AssemblyInfo.cs`. The `avalonia/` track's 2026-09-05 finding
  applies here in full: a parallel run does not error, it silently truncates and
  still prints a normal-looking summary. **Read the test count, not just the word
  `Failed`.**
- A `TelemetryContext`, snapshotting and restoring those globals per test — the same
  construction as `caliburn/`'s `CaliburnCoreContext`, and for the same reason. Its
  pristine snapshot must be taken in an **explicit static constructor**, never a
  static field initializer: a field initializer leaves the type `beforefieldinit`,
  the runtime defers initialization to the first read, that read happens after the
  instance constructor's reset, and the snapshot comes back empty — measured on this
  machine while building `caliburn/`.

Every exercise additionally gets a uniquely named `ActivitySource`/`Meter`, so a
probe scoped to one name cannot see another row's data even if a reset is missed.

**Verification recipe per row**, extending `security/`'s: after the stub is red and
the solution green, build the *reject-everything* variant, then the **plausible wrong
one** — for this track typically "logs, but interpolates", "creates a span, but as a
root instead of a child", "counts, but with unbounded tag cardinality", "registers
the exporter, but never flushes". A row whose facts pass against the plausible wrong
implementation is under-grading and must be tightened before the catalog row flips.

## 6. Test harness

`tests/_harness/`:

| Type | Purpose |
|---|---|
| `AssemblyInfo.cs` | `[assembly: CollectionBehavior(DisableTestParallelization = true)]` |
| `TelemetryContext` | per-test snapshot/restore of the process globals listed above |
| `TraceProbe` | an `ActivityListener` scoped to exactly one source name, collecting completed activities |
| `MetricProbe` | `MetricCollector<T>` plus a manual-collect `MeterProvider` for SDK rows |
| `LogProbe` | `FakeLogger`/`FakeLogCollector` wiring, exposing structured state |
| `WebProbe` | a minimal `TestServer` host the web rows share |
| `ContainerGate` | `Assert.SkipUnless(...)` reading `AppContext.GetData("FeWoLearning.Telemetry.Containers")` |
| `HarnessSmokeTests` | canaries that must pass in **both** modes and fail first when a package bump breaks the harness |

`FactAttribute.Skip` is not virtual in xunit.v3 3.2.2, so the idiomatic custom
`[ContainerFact]` overriding `Skip` fails with `CS0506`. The gate is a call in the
test body instead. `-p:Containers=true` is an MSBuild property and therefore
invisible at runtime; it reaches the test process through a
`RuntimeHostConfigurationOption` read back via `AppContext.GetData` — the mechanism
`MicroServices/` and `Architecture/` already use.

## 7. Stub shape

Stubs throw `NotImplementedException` with a `"TODO: ExNNN - …"` message, so the
project still compiles while unfinished. Each carries the repo's standard header
comment with `Goal:` / `Drills:` / `Passes:`, where `Passes:` states the observable
outcome precisely enough to implement against without reading the test.

Expected warnings from the `exercises/` build (`CS0169`/`CS0414`/`CS0649` for fields
a stub declares for the learner to wire up) are left unsuppressed, as in `blazor/`
and `caliburn/`. `solutions/` must build with **0 warnings** — a warning there is a
finding. `tests/` suppresses `xUnit1051` only, via `NoWarn`; any other warning there
is a finding too.

## 8. Delivery

70 exercises × 3 files = 210 files. This is a multi-session build.

1. **Scaffold session:** this spec, the three projects, `.slnx`,
   `Directory.Build.props`, the harness, all 70 `catalog.md` rows, `README.md`, and a
   real restore that pins every package version. Then batch 1 (ex001–ex005) verified
   red and green, proving the mechanism carries.
2. **Thereafter:** batches of five per CLAUDE.md's "Adding or completing exercises",
   with `catalog.md` as the work queue. Full-suite runs in both modes once per
   completed block.

Commit messages follow the repo convention: `telemetry: exNNN-exNNN`.
