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

**Status: 45 ✅ / 25 ⬜**

## logging (001–014) — `ILogger` and the structured-logging contract

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 001 | StructuredMessageTemplate | message template vs interpolated string; named fields survive into the log state | ✅ |
| 002 | LogLevelsAndFiltering | `IsEnabled`, filter rules by category and level, per-provider minimum level | ✅ |
| 003 | CategoriesAndTypedLogger | `ILogger<T>` category naming, `CreateLogger(string)`, category-based filtering | ✅ |
| 004 | LoggingScopes | `BeginScope`, nesting, scope state on the record, providers must opt in | ✅ |
| 005 | LoggerMessageSourceGenerator | `[LoggerMessage]` partial method, `EventId`, compile-time template | ✅ |
| 006 | HighPerformanceGuardClause | `IsEnabled` guard, `LoggerMessage.Define`, avoiding boxing on the disabled path | ✅ |
| 007 | ExceptionLogging | the exception argument vs `ex.ToString()` in the message; inner and aggregate exceptions | ✅ |
| 008 | CustomILoggerProvider | a provider + logger + `ISupportExternalScope` written by hand | ✅ |
| 009 | RedactionAndPii | structural scrubbing of named fields, not a regex over the rendered message | ✅ |
| 010 | LogEnrichment | ambient properties (version, tenant, machine) attached once, not per call site | ✅ |
| 011 | EventIdConventions | stable ids, id/name pairing, filtering by `EventId` | ✅ |
| 012 | SerilogStructuredSink | Serilog behind `ILogger`, destructuring with `@`, properties as fields | ✅ |
| 013 | SerilogRollingFileAndOverrides | rolling file sink, `MinimumLevel.Override` per source, retention | ✅ |
| 014 | LogSamplingAndRateLimit | suppressing repetition, first-N-per-window, virtual clock | ✅ |

## diagnostics (015–026) — the BCL primitives, before any SDK

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 015 | ActivitySourceAndListener | `StartActivity` returns **null** with no listener; `ActivityListener`, `Sample` → `AllData` | ✅ |
| 016 | ActivityParentChild | nesting, `Parent`/`ParentId`, `Activity.Current` restored on `Dispose` | ✅ |
| 017 | TagsBaggageEvents | `SetTag` vs `AddBaggage` (baggage inherits down, tags do not), `AddEvent` | ✅ |
| 018 | StatusAndException | `SetStatus`, `AddException`, the error tag convention | ✅ |
| 019 | KindAndLinks | `ActivityKind` Client/Server/Producer/Consumer, `ActivityLink` for fan-in | ✅ |
| 020 | W3CTraceContext | `traceparent`/`tracestate` parse and format, `ActivityContext` round-trip, id format | ✅ |
| 021 | MeterAndCounter | `Meter`, `Counter<T>`, `MeterListener`, tags as dimensions | ✅ |
| 022 | HistogramAndBuckets | `Histogram<T>`, what a distribution can and cannot answer | ✅ |
| 023 | ObservableInstruments | `ObservableGauge`/`ObservableCounter` pull model, `UpDownCounter` vs `Counter` | ✅ |
| 024 | MeterListenerLifecycle | `InstrumentPublished`, `EnableMeasurementEvents`, dispose, no double counting | ✅ |
| 025 | EventSourceAndCounters | a custom `EventSource`, keywords and levels, `EventListener`, runtime counters | ✅ |
| 026 | DiagnosticSourceListener | `DiagnosticListener`, `IsEnabled`, anonymous payloads, the framework's own events | ✅ |

## otel-sdk (027–044) — the OpenTelemetry SDK

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 027 | TracerProviderBuilder | `AddSource`, `Build`, in-memory exporter; an unregistered source produces nothing | ✅ |
| 028 | ResourceAttributes | `service.name`/`version`/`instance.id`, `ResourceBuilder`, env-var detection | ✅ |
| 029 | SpanProcessors | simple vs batch export, ordering, a custom processor enriching in `OnStart`/`OnEnd` | ✅ |
| 030 | Samplers | AlwaysOn/AlwaysOff/TraceIdRatioBased/ParentBased; sampled flag vs recording | ✅ |
| 031 | CustomSampler | implementing `Sampler`, `Drop` vs `RecordOnly` vs `RecordAndSample` | ✅ |
| 032 | MeterProviderAndReader | `AddMeter`, periodic vs manual `Collect`, in-memory metric exporter | ✅ |
| 033 | MetricViews | rename, drop, tag-key selection for cardinality control, explicit histogram bounds | ✅ |
| 034 | MetricTemporality | Delta vs Cumulative across two collections | ✅ |
| 035 | Exemplars | a measurement carrying the trace id that produced it, exemplar filtering | ✅ |
| 036 | OtelLogsPipeline | `AddOpenTelemetry` logging, `LogRecord` fields, formatted message vs state | ✅ |
| 037 | LogTraceCorrelation | `LogRecord.TraceId` matching `Activity.Current`, `ParseStateValues`, `IncludeScopes` | ✅ |
| 038 | ContextPropagators | `TextMapPropagator` inject/extract, the composite default | ✅ |
| 039 | BaggagePropagation | baggage across a boundary, why it is not a span tag until you make it one | ✅ |
| 040 | SemanticConventions | stable attribute names; why hand-rolled names silently break every dashboard | ✅ |
| 041 | InstrumentationLibraries | `AddHttpClientInstrumentation`/`AddAspNetCoreInstrumentation`: what is automatic vs yours | ✅ |
| 042 | OtlpExporterConfiguration | endpoint, protocol, headers, env-var precedence; a real collector receives the span | 🐳 ✅ |
| 043 | PrometheusScrapeEndpoint | the text exposition format, name mangling and the `_total` suffix | 🐳 ✅ |
| 044 | ShutdownAndFlush | `ForceFlush`/`Shutdown`, the loss window, disposal order | ✅ |

## web-services (045–058) — ASP.NET Core, HTTP clients, queues, databases

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 045 | AspNetCoreServerSpan | a server span per request via `TestHost`; the **route template**, not the raw path, as the span name | ✅ |
| 046 | SpanEnrichmentAndFiltering | enriching the server span; suppressing `/health` noise at the source | ⬜ |
| 047 | HttpClientPropagation | the outgoing `traceparent`, parent/child across the process boundary, a `DelegatingHandler` | ⬜ |
| 048 | QueuePropagationProducerConsumer | inject into message headers, extract on the consumer, Producer/Consumer kinds and a link | ⬜ |
| 049 | BackgroundServiceInstrumentation | one root activity **per iteration**, not one per service lifetime | ⬜ |
| 050 | RedMetricsAndCardinality | rate/errors/duration by `http.route`; a tag cardinality budget | ⬜ |
| 051 | DatabaseInstrumentation | `db.system`/`db.query.text`, and why parameter values must never reach the span | 🐳 ⬜ |
| 052 | HealthChecksAndProbes | composition, tag-filtered liveness vs readiness, check results as metrics | ⬜ |
| 053 | ResilienceTelemetry | Polly retries visible as events and metrics; the retry-hides-the-failure bug | ⬜ |
| 054 | ErrorStatusAndProblemDetails | exception → span status → correlated log, never swallowed | ⬜ |
| 055 | HttpRequestLogging | request logging duration measured at the right place; query-string PII | ⬜ |
| 056 | CollectorPipeline | an OTel Collector container with a processor; what the collector can fix that the app cannot | 🐳 ⬜ |
| 057 | BackendIngestion | structured logs into a real backend; fields are queryable, a baked-in message is not | 🐳 ⬜ |
| 058 | MultiHopTraceCapstone | HTTP → in-process queue → background worker, one trace across all three hops | ⬜ |

## desktop-ops (059–070) — WPF, and running in a container

Rows 059–062 are `[WpfFact]` (STA thread). None opens a window.

| # | Slug | Concepts | Status |
|---|------|----------|--------|
| 059 | DispatcherLatencyMetric | UI-thread queue latency as a histogram, measured on the `Dispatcher` | ⬜ |
| 060 | UnhandledExceptionCapture | `DispatcherUnhandledException`, `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException` — one record, not three or none | ⬜ |
| 061 | CommandActivityTracing | an activity spanning a user command, surviving the dispatcher hop and `await` | ⬜ |
| 062 | BindingErrorMonitoring | `PresentationTraceSources` binding failures turned into structured logs | ⬜ |
| 063 | StartupPerformanceTracing | cold-start phases as nested activities | ⬜ |
| 064 | OfflineBufferAndReplay | a bounded buffer while the exporter is unreachable, replay on reconnect, explicit drop policy | ⬜ |
| 065 | FlushOnShutdown | flushing on exit; the size of the window that is lost when you do not | ⬜ |
| 066 | PiiScrubbingAndConsent | user names, paths and machine identifiers; an opt-in consent flag that actually gates emission | ⬜ |
| 067 | SessionCorrelation | a stable anonymous install/session id tying one run's logs, traces and metrics together | ⬜ |
| 068 | LocalRollingFileAndSupportBundle | a size-capped rolling local log and a support bundle a user can send | ⬜ |
| 069 | ContainerResourceDetection | `OTEL_*` env configuration, container resource attributes, stdout JSON for the log driver | 🐳 ⬜ |
| 070 | GracefulShutdownFlushInContainer | SIGTERM, `IHostApplicationLifetime`, draining the batch processor before the process dies | 🐳 ⬜ |
