# telemetry — Monitoring, Logging and OpenTelemetry for .NET

70 graded C# exercises in **application instrumentation** on .NET 10: `ILogger`,
`Activity`/`ActivitySource`, `Meter`, the OpenTelemetry SDK, exporters, sampling and
propagation — across web, services, desktop and containers.

Design document:
[`docs/superpowers/specs/2026-09-06-telemetry-track-design.md`](../docs/superpowers/specs/2026-09-06-telemetry-track-design.md).
Work queue: [`catalog.md`](catalog.md).

## What this track owns

It teaches the instrumentation itself — not the topology it runs in, and not the
architecture it hangs off. Three sibling tracks touch the subject:

| Sibling | What lives there | What lives here |
|---|---|---|
| `MicroServices/` 021, 022, 023, 067, 068, 070, 081 | OTel *inside Aspire*: what the orchestrator injects on its own, and the deployment topology | The SDK itself: providers, processors, samplers, views, propagators, exporters |
| `Architecture/` 028, 055, 056 | Telemetry as a **port** — keeping the domain logger-free | What happens behind the port |
| `security/` 051 | Redaction as an **attack surface** | Redaction as a **producer obligation** |

## Setup and commands

Run every command **from inside `telemetry/`**, never the repo root.

| Run | Command |
|---|---|
| Stubs (red) | `dotnet test` |
| Solutions (green) | `dotnet test -p:UseSolutions=true` |
| Including the 🐳 container rows | `dotnet test -p:Containers=true` |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |

No install step: the first `dotnet test` restores.

**This track is Windows-only.** It targets `net10.0-windows` with `UseWPF`, because
block `05-desktop-ops` drills real WPF mechanisms (`Dispatcher`,
`DispatcherUnhandledException`, `PresentationTraceSources`) rather than a
UI-framework-free stand-in. The same trade `wpf/`, `caliburn/` and `security/` make.
Unlike `caliburn/`, it does **not** appear to need an interactive desktop session —
see Measured facts below.

## Layout

```
telemetry/
  FeWoLearning.Telemetry.slnx
  Directory.Build.props     # redirects the solutions build's output (CS0579 guard)
  catalog.md                # the 70-row ledger; the work queue
  exercises/  01-logging/ 02-diagnostics/ 03-otel-sdk/ 04-web-services/ 05-desktop-ops/ _support/
  solutions/  (same blocks, same relative paths, same type names)
  tests/      (same blocks) _harness/
```

`exercises/` and `solutions/` compile the **same type names into the same
namespaces**; `tests/` references exactly one of them via the `UseSolutions` MSBuild
property. That is what makes `dotnet test` the red run and
`dotnet test -p:UseSolutions=true` the green one, and it is why `solutions/` is
deliberately *in* the build here — the same waiver `avalonia/`, `blazor/`, `uno/`,
`caliburn/`, `wpf/`, `security/` and `Architecture/` take. Reference solutions are
compile-checked on every green run instead of drifting silently.

Namespaces are pinned per block, because `01-logging` is not a valid C# identifier:

| Folder | Namespace |
|---|---|
| `01-logging` | `FeWoLearning.Telemetry.Exercises.Logging` |
| `02-diagnostics` | `FeWoLearning.Telemetry.Exercises.Diagnostics` |
| `03-otel-sdk` | `FeWoLearning.Telemetry.Exercises.Otel` |
| `04-web-services` | `FeWoLearning.Telemetry.Exercises.WebServices` |
| `05-desktop-ops` | `FeWoLearning.Telemetry.Exercises.DesktopOps` |

**Block 03 is `.Otel`, never `.OpenTelemetry`.** Inside a namespace ending in
`OpenTelemetry`, a fully qualified `OpenTelemetry.Trace.Sampler` resolves its leading
segment to the enclosing namespace and fails `CS0234` — the shadowing trap `avalonia/`
and `caliburn/` both record, in the one block where learners type that root namespace
constantly. Do not "tidy" it back.

`_support/` (identical in both content libraries) holds shared fixtures. It is never a
TODO and never gets a `catalog.md` row.

## How a telemetry test lies

`security/`'s lesson was "an attack fact with no paired use fact grades nothing". This
track's is: **a telemetry test that asserts the rendered result instead of the
structure grades nothing.** Six ways it happens. Check every new row against all six
before flipping its catalog cell.

1. **Text instead of fields.** `$"user {id} failed"` produces a byte-identical
   formatted message to `"user {UserId} failed", id`, and carries not one named field.
   A test asserting the message string grades nothing about structure. Assert the
   structured state — `LogProbe.Field`, `LogRecord` attributes.
2. **Existence instead of lineage.** "A span exists" is satisfied by automatic
   instrumentation the learner never wrote, and by a span with the wrong parent.
   Assert `Source.Name`, `ParentSpanId` and `ActivityKind`, not the count.
3. **Registration instead of emission.** An exporter present in the service collection
   proves nothing; only the in-memory exporter's contents prove data flowed.
   (`MicroServices/`'s §8.2 is the same finding in another key.)
4. **Collecting once.** A single `Collect` cannot distinguish Delta from Cumulative,
   cannot show a counter is monotonic, and cannot catch double counting from a leaked
   listener. Collect twice and compare.
5. **A free pass from automatic instrumentation.** `traceparent` propagates over HTTP
   by itself — `MicroServices/` rows 068 and 081 measured exactly this. Every row here
   grades the learner's **own** `ActivitySource`, `Meter` or handler, and any row whose
   subject *is* automatic must assert something automation does not do.
6. **Process-global contamination.** An `ActivityListener` sees activities from every
   test running concurrently. `Sdk.SetDefaultTextMapPropagator`,
   `Activity.DefaultIdFormat`, `Activity.ForceDefaultIdFormat`, `Activity.Current` and
   `static Meter` fields are all process-wide.
7. **The in-memory exporter does not snapshot.** It stores the `Activity` *object*, so
   anything that mutates a span after it was exported still changes what the test sees
   — measured, with a processor registered after the exporter. An attribute being
   present at assertion time therefore proves nothing about *when* it was set. Grade
   ordering on a call log, never on which tags exist at the end. (This one is an
   artefact of in-memory grading: a real exporter serialises and the question never
   arises — which is exactly why it is easy to miss.)

Two mitigations for (6), both mandatory and both already in place:

- `[assembly: CollectionBehavior(DisableTestParallelization = true)]` in
  `tests/_harness/AssemblyInfo.cs`. `avalonia/`'s 2026-09-05 finding applies here in
  full: a parallel run does not error, it silently truncates and still prints a
  normal-looking summary. **Read the test count, not just the word `Failed`.**
- `TelemetryContext`, which snapshots and restores those globals per test.

Every exercise additionally gets a **uniquely named** `ActivitySource`/`Meter` —
`fewolearning.telemetry.exNNN` — so a probe scoped to one name cannot see another
row's data even if a reset is missed.

## Verifying a new exercise

Beyond CLAUDE.md's batch procedure, and extending `security/`'s recipe:

1. Red check filtered to the batch. Every failure must trace to its own
   `TODO: ExNNN`; no fact may pass. A stub that fails to *build* is a bug.
2. Green check with `-p:UseSolutions=true`. There is no overlay step in this track.
3. Build the **reject-everything** variant. Does any fact still pass that should not?
4. Build the **plausible wrong** one — the earnest implementation that does real work
   with the wrong mechanism. This is what per-batch review misses. On this track the
   four recurring shapes are:
   - *logs, but interpolates* — passes any message-text assertion;
   - *creates a span, but as a root instead of a child* — passes any "a span exists"
     assertion;
   - *counts, but with unbounded tag cardinality* — passes any "the counter went up"
     assertion;
   - *registers the exporter, but never flushes* — passes any DI-registration
     assertion.

   A row whose facts survive its plausible-wrong implementation is under-grading and
   does not ship.
5. For a 🐳 row, run `-p:Containers=true` **and** confirm the same fact skips without
   it.

## The test harness

`tests/_harness/`:

| Type | Purpose |
|---|---|
| `TelemetryContext` | per-test snapshot/restore of the diagnostics process-globals |
| `TraceProbe` | an `ActivityListener` scoped to exactly one source name |
| `LogProbe` | `FakeLogger` wiring, exposing structured state, `{OriginalFormat}` and raw scopes |
| `MetricProbe` | a manual-collect `MeterProvider` over one meter name |
| `ContainerGate` | the `-p:Containers=true` gate |
| `HarnessSmokeTests` | canaries, green in both modes |

`FactAttribute.Skip` is not virtual in xunit.v3 3.2.2, so the idiomatic custom
`[ContainerFact]` overriding `Skip` fails with `CS0506`. The gate is a call in the
test body instead, and `-p:Containers=true` reaches the test process through a
`RuntimeHostConfigurationOption` read back via `AppContext.GetData` — an MSBuild
property is otherwise invisible at runtime.

## Toolchain, and why these exact pins

`net10.0-windows` · `UseWPF` · `FrameworkReference Microsoft.AspNetCore.App` on the
content libraries · **no `global.json`**.

| Package | Pin | Why |
|---|---|---|
| `OpenTelemetry` and its instrumentation/exporter family | `1.18.0` | current stable |
| `OpenTelemetry.Exporter.Prometheus.AspNetCore` | `1.18.0-beta.1` | **has no stable release at all** |
| `xunit.v3` | `3.2.2` | 4.0.0 plus a `Microsoft.Testing.Platform` `global.json` makes `dotnet test` exit 5 with **zero tests discovered** on this machine |
| `xunit.runner.visualstudio` | `3.1.5` | there is no 3.1.6 or 3.1.7; naming one resolves *forward* to 4.0.0 with only an `NU1603` warning |
| `Xunit.StaFact` | `3.0.13` | 4.x requires xunit.v3 4.0.0, which requires the `global.json` this track must not have. `caliburn/`'s measured pairing |
| `Microsoft.NET.Test.Sdk` | `17.14.1` | the version `MicroServices/` and `Architecture/` measured on the VSTest path |
| `Microsoft.Extensions.Diagnostics.Testing` | `10.9.0` | `FakeLogger`, `FakeLogCollector`, `MetricCollector<T>` |

Never `PackageReference` `Microsoft.Extensions.Hosting`, `.Logging`,
`.DependencyInjection`, `.Configuration`, `.Options`, or
`System.Security.Cryptography.ProtectedData`: all are in the shared framework for this
TFM once `Microsoft.AspNetCore.App` is referenced, and referencing them emits
`NU1510`.

**Warnings are findings.** `exercises/` may emit `CS0169`/`CS0414`/`CS0649` from stub
fields, deliberately unsuppressed. `solutions/` must build with 0 warnings.  `tests/`
suppresses `xUnit1051` only.

## Measured facts

Everything here was established by running code on this machine, not recalled. Add to
it whenever a batch surprises you — a surprise found and not written down is a
surprise the next batch pays for again.

**2026-09-06, scaffolding and harness:**

- `FakeLogger`'s record accessor is **`logger.Collector.LatestRecord`**. There is no
  `logger.Latest`.
- `FakeLogger` captures scopes with **no `IncludeScopes` opt-in**, and
  `FakeLogRecord.Scopes` holds the **raw** scope objects rather than flattened
  key/value pairs — a `Dictionary<string, object>` scope arrives as that dictionary.
  A fact written as though scopes arrive pre-flattened into named fields fails against
  a correct implementation.
- `[WpfFact]` from `Xunit.StaFact` 3.0.13 supplies an STA thread and a live
  `Dispatcher.CurrentDispatcher`, and needed **no interactive desktop session** here —
  unlike `caliburn/`, which does, because it opens a real `Window`. Re-measure when
  block 05 lands; no row is supposed to open a window.
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` has **no stable release**;
  `dotnet package search` returns nothing for it unless prereleases are requested.
- `TraceProbe` samples `AllDataAndRecorded`, not `AllData`. With `AllData` alone the
  activity is created but `Activity.Recorded` is false, so an implementation that
  guards its tagging on `Recorded` emits nothing and the test fails for the wrong
  reason.
- **A literal double hyphen is illegal inside an XML comment**, so a `.csproj` comment
  naming a CLI flag like the prerelease switch fails the build with `MSB4025`. The
  rule CLAUDE.md records for `.axaml` applies to project files too.
- **A canary that checks which content library is loaded must first touch a type from
  it, and a `const` does not count.** Referenced assemblies load lazily, so
  `AppDomain.CurrentDomain.GetAssemblies()` returns an empty match; and the compiler
  bakes a `const` into the call site, so reading one never triggers the load. Hence
  `_support/TrackMarker.TrackName` being `static readonly`.
- Harness baseline: **5 passed / 1 skipped** on a plain run, **6 passed / 0 skipped**
  under `-p:Containers=true`, identical in both `UseSolutions` modes, 0 warnings on
  `--no-incremental` builds of both.

**2026-09-06, block `01-logging` rows 001–005:**

- **A generic logger's category drops the type argument entirely.**
  `factory.CreateLogger<Repository<OrderProcessor>>()` produces the category
  `…Logging.Repository` — no `` `1 `` arity marker, no `[OrderProcessor]`. A category
  is a *display* name, not a CLR type name. Operationally that means
  `Repository<Order>` and `Repository<Invoice>` share one category and therefore one
  filter rule, so you cannot raise the level for just one of them.
- **`ILogger<T>` and `CreateLogger("the.same.string")` are behaviourally
  indistinguishable** — same category, same filtering, same records. The difference
  only shows up when someone renames the type and the filter rules silently stop
  matching. Ex003 grades it by asserting the returned instance is generic and closed
  over the type, since no log record can carry that.
- **`[LoggerMessage]` survives into metadata on the partial declaration**, so
  `MethodInfo.GetCustomAttributes` sees it. This is the only honest way to grade
  ex005: a hand-written `logger.LogWarning(new EventId(5001, "CacheMiss"), …)`
  satisfies every behavioural fact, and what the generator actually adds — the guard,
  the allocation-free argument path, one declaration site — leaves no trace in a
  record. `blazor/` ex069 and ex100 are graded on metadata for the same reason.
  Consequence: ex005's `Adversarial_A` goes red on an **assertion** rather than on the
  stub's `NotImplementedException`. That is deliberate and is the only fact in the
  batch that does so.
- The stub class for ex005 is `static` and the solution makes it `static partial`.
  Both compile, so the red run still builds — which is the invariant that matters.
- Batch baseline after rows 001–005: **27 facts total** (21 exercise + 6 harness).
  Red run 21 failed / 5 passed / 1 skipped; green run 26 passed / 1 skipped; green
  with `-p:Containers=true` 27 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, block `01-logging` rows 006–010:**

- **`UseWPF=true` shortens the implicit-using list.** The generated
  `GlobalUsings.g.cs` here is `System`, `System.Collections.Generic`, `System.Linq`,
  `System.Threading`, `System.Threading.Tasks` — the WindowsDesktop SDK's set, which
  drops **`System.IO` and `System.Net.Http`** from the usual `Microsoft.NET.Sdk` list.
  So `IOException`, `Stream`, `Path` and `HttpClient` all need an explicit `using` in
  every file on this track, and the resulting `CS0246` is otherwise baffling.
- **A reflection fact asserting an interface is *declared* grades nothing when the
  stub already declares it.** Ex008's provider must declare `ISupportExternalScope`
  for its `SetScopeProvider` member to make sense, so a fact checking
  `GetInterfaces()` passed against the untouched stub — caught by the red run
  reporting `1 passed`, which is exactly why "no fact may pass on the stub" is checked
  and not assumed. Dropped in favour of the behavioural fact, which cannot be
  satisfied without using the scope provider. This is CLAUDE.md's "a test that asserts
  what the signature produces" in a new disguise: an interface list is part of the
  signature.
- **The scope mechanism has two halves and only one is behavioural.** Once the factory
  finds `ISupportExternalScope` on a provider it pushes scopes into the shared
  `IExternalScopeProvider` and **stops calling that provider's `logger.BeginScope`** —
  so a provider that advertises the interface and ignores the object it is handed sees
  no scopes at all, silently. The reference implementation therefore returns `null`
  from `BeginScope` deliberately, and reads `ForEachScope` per record rather than
  caching.
- **Ex009 is the clearest demonstration so far of why the plausible-wrong probe is not
  optional.** A value-sniffing scrubber — regex for an address or a card number,
  applied to each argument — passes **3 of the 5 facts**: the safe fields keep their
  values, the sensitive ones are redacted, and no secret reaches the message. Only the
  matched adversarial pair catches it: a sensitive field holding `"n/a"` must still be
  redacted, and a safe field holding a real address must not be. Neither fact works
  without the other.
- Batch baseline after rows 001–010: **49 facts total** (43 exercise + 6 harness).
  Red run 43 failed / 5 passed / 1 skipped; green run 48 passed / 1 skipped; green
  with `-p:Containers=true` 49 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 011–015 — block `01-logging` complete, `02-diagnostics` begun:**

- **`Activity.IsAllDataRequested` is a hint to the caller, not a guard the API
  applies.** This corrects a claim repeated all over the internet and, briefly, in this
  track's own first draft of ex015. Under
  `ActivitySamplingResult.PropagationData` the flag is `false` — and `SetTag` **still
  writes**; the tag is present on the activity afterwards. Measured here by running a
  `PropagationData` sampler against ex015's own facts: only the assertion on the flag
  failed, the assertion on the tag passed. So the cost of ignoring the flag is not a
  missing tag, it is building and storing detail the listener said it did not want, for
  an activity an SDK downstream discards anyway. Expensive tagging belongs behind
  `if (activity.IsAllDataRequested)`, and nothing will ever tell you that you forgot.
  **Consequence for grading: a fact that asserts only the tag cannot tell the two
  samplers apart.** It has to assert the flag.
- **`using Serilog;` next to `using Microsoft.Extensions.Logging;` makes the bare name
  `ILogger` ambiguous (`CS0104`)** — both namespaces declare one, and `using Serilog;`
  is unavoidable because `LoggerConfiguration` and the `AddSerilog` extension live
  there. `using ILogger = Microsoft.Extensions.Logging.ILogger;` settles it. The stub
  for ex012 says so, because this is a toolchain wart and not the lesson.
- **Serilog's `@` destructuring hint travels into the stored event's template text.**
  `logEvent.MessageTemplate.Text` reads `"Order {OrderId} placed by {@Customer}"`, not
  the `@`-less form — so a fact asserting the template has to include it.
- **Serilog's file sink buffers**; the logger must be disposed before its files are
  read, which is also the honest lesson (the last records only exist once it closes).
  `ScratchDirectory` in the harness gives the file-based rows a temp directory that
  cleans itself up, best-effort.
- **Ex011's wrong-implementation probe was the sharpest illustration yet of asserting
  too little.** Given an id invented inline at the call site as
  `new EventId(1002, "OrderDeclined")` — right number, drifted name — the fact checking
  each method's level and numeric id **passed**. Only the catalog-consistency fact
  ("every emitted id appears in `All`, by id *and* name") caught it. Two of this
  batch's five wrong implementations were likewise caught by exactly one fact each:
  ex014's fixed-grid window, and ex015's `PropagationData` sampler.
- Batch baseline after rows 001–015: **71 facts total** (65 exercise + 6 harness).
  Red run 65 failed / 5 passed / 1 skipped; green run 70 passed / 1 skipped; green
  with `-p:Containers=true` 71 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 016–020 — `02-diagnostics` under way:**

- **`ActivityContext.TryParse` validates more than its name suggests.** It checks the
  version, the field count, the lengths and lowercase-hex validity — **and rejects the
  all-zero trace and span ids** that the W3C spec forbids despite them being correctly
  shaped. Measured by deleting an explicit all-zero guard from ex020's reference
  solution and watching every fact still pass; the guard was dead code and was removed.
  The all-zero test case still earns its place, because it grades a **hand-rolled**
  parser (`Split('-')` plus `ActivityTraceId.CreateFromString`), which is a plausible
  implementation and which does not get that check for free.
- **`new Activity(name).Start()` needs no listener at all.** It sets `Activity.Current`
  and behaves normally. Only `ActivitySource.StartActivity` returns null unheard — the
  ex015 trap. This is what lets ex016's ambient-context fact set up a caller-side
  activity without registering a second listener.
- **`Activity.Baggage` and `GetBaggageItem` walk the parent chain**, so baggage set on
  a parent is readable from a child regardless of the order the two were created in.
  Tags do not walk anything.
- **A `using var` declared inside a `foreach` body is disposed per iteration**, which
  is the difference between a fan-out and a staircase in ex016. Hoisting it out — or
  dropping the `using` — leaves each step current while the next starts, so the trace
  renders as a chain of dependencies that do not exist, with no error anywhere.
- **Passing an explicit `default` `ActivityContext` as `parentContext` makes a root**,
  overriding `Activity.Current` rather than inheriting it. That is how ex019's batch
  consumer belongs to no single message's trace. Links must be supplied at start.
- Two of this batch's five wrong implementations were again caught by **exactly one**
  fact: ex017's "belt and braces" (baggage *and* a tag, which makes every behavioural
  fact pass while hiding which mechanism did the work) and ex019's "parent the batch on
  the first message".
- Batch baseline after rows 001–020: **102 facts total** (96 exercise + 6 harness).
  Red run 96 failed / 5 passed / 1 skipped; green run 101 passed / 1 skipped; green
  with `-p:Containers=true` 102 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 021–025 — metrics, still before any SDK:**

- **`MeasurementProbe` is the BCL-level probe and is not interchangeable with
  `MetricProbe`.** Block `02-diagnostics` is explicitly about the primitives, so its
  rows are graded on what a raw `MeterListener` delivers, never on what an
  OpenTelemetry pipeline made of it. `MetricProbe` belongs to block `03-otel-sdk`.
- **`EventListener.OnEventSourceCreated` runs during the *base* constructor**, before a
  single field initializer of the derived class. Two consequences, both silent, and
  both hit while writing ex025:
  - a field declared `private readonly List<string> _names = [];` is assigned **after**
    the base constructor, wiping every name collected during construction — so a
    collecting listener must declare the field with **no initializer** and create it
    lazily inside the callback;
  - calling `EnableEvents` from that callback subscribes with whatever the settings
    fields happen to hold, which is `default` — hence the two-step in the test's
    `Subscriber`: remember the source during construction, subscribe once the settings
    exist.
- **A `Meter`'s instruments cannot be removed once published.** Registering a second
  `ObservableGauge` under the same name leaves both alive and every collection then
  reports the value twice — which reads as a doubled queue rather than as a bug. Ex023
  therefore specifies "create the gauge at most once; later calls swap the source", and
  its callback closes over a *field* rather than over the parameter. The same
  constraint is why every instrument in these rows is a `static readonly` field, and
  why ex021's "exactly one published instrument" fact incidentally enforces it.
- **A listener created per test still sees instruments created by earlier tests**,
  because `InstrumentPublished` replays existing instruments on `Start()`. Measurements
  do not replay, so a probe only ever collects its own test's — but a probe that is
  still alive when the *next* batch is recorded collects that too. Ex022's
  distribution-comparison facts dispose each probe before recording the next set for
  exactly this reason.
- **Ex022 grades the lesson rather than an API.** The reference implementation records
  each request both into a histogram and into a sum/count pair, and two facts then show
  `[10,10,10,10,1000]` and `[208,208,208,208,208]` producing an identical mean of 208 ms
  while the histogram separates them. That is the row: an average is not a latency
  measurement, demonstrated on numbers you can check by hand.
- Three of this batch's five wrong implementations were again caught by **exactly one**
  fact: ex022's `Math.Round` before recording, ex023's release decrementing the
  monotonic counter, and ex025's keyword copy-pasted onto the wrong event.
- Batch baseline after rows 001–025: **127 facts total** (121 exercise + 6 harness).
  Red run 121 failed / 5 passed / 1 skipped; green run 126 passed / 1 skipped; green
  with `-p:Containers=true` 127 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 026–030 — `02-diagnostics` complete, `03-otel-sdk` begun:**

- **The in-memory exporter stores the `Activity` object, not a snapshot of it.**
  Measured: a processor registered *after* `AddInMemoryExporter` still mutates the
  exported span in its `OnEnd`, because the test is holding the same reference. So an
  attribute being present at assertion time proves nothing about *when* it was set, and
  any row about processor ordering must be graded on a call log rather than on which
  tags happen to exist at the end. This is a seventh way a telemetry test lies and it
  is specific to in-memory grading — a real exporter serialises and the question never
  arises.
- **Processor chains do not unwind.** Both `OnStart` and `OnEnd` run in *registration*
  order — OTel composes processors into a list and walks it head to tail for both
  hooks, unlike an ASP.NET middleware pipeline, which nests. Measured in ex029:
  `["first:start", "second:start", "first:end", "second:end"]`. Consequence: "the last
  processor gets the final say" is true for `OnEnd` only because it is last in the
  list, and anything added after the exporter runs *after* the export rather than
  around it.
- **`ForceFlush` and `GetResource` live in different namespaces.** `ForceFlush` is an
  extension in `OpenTelemetry.Trace` (`TracerProviderExtensions`); `GetResource` is one
  in the root `OpenTelemetry` namespace. A file using both needs both `using`
  directives, and the `CS1061` you get otherwise names the method rather than the
  namespace.
- **`OpenTelemetry.Exporter.InMemory` had to move into the content libraries.** Block
  03's rows build the pipeline themselves, exporter included, so the package cannot
  live in `tests/` alone. The principle it seemed to violate is narrower than it looks:
  what must stay out of an exercise's reach is the **grading instruments** — `LogProbe`,
  `TraceProbe`, `MetricProbe`, `MeasurementProbe` — not a pipeline component the
  learner is being asked to compose.
- **A fact asserting that the stub's own declarations exist grades nothing**, again.
  Ex027 originally carried "both sources exist and are used", which passed on the
  untouched tree because the stub declares both `ActivitySource` properties. Removed;
  the guarantee it was reaching for comes from the signature instead, since `DoWork`
  takes the source as a *parameter* and therefore cannot treat the two differently.
  This is the third time this shape has appeared (ex008, ex027, and the near-miss in
  ex003) — **check the red run's pass count, every batch.**
- Every one of this batch's five wrong implementations was caught, and every failing
  fact was an `Adversarial_` one: an unconditional `Write` with no `IsEnabled` gate, a
  `AddSource("...ex027*")` wildcard that also catches the unregistered source, a
  forgotten `AddEnvironmentVariableDetector`, processors registered in reverse on the
  middleware intuition, and a `DoWork` that drops the incoming parent context.
- Batch baseline after rows 001–030: **152 facts total** (146 exercise + 6 harness).
  Red run 146 failed / 5 passed / 1 skipped; green run 151 passed / 1 skipped; green
  with `-p:Containers=true` 152 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 031–035 — the metrics half of the SDK; the track is halfway:**

- **The in-memory metric exporter hands back the SAME `Metric` object on every
  collection.** Measured: after two collections the exported list holds two entries that
  are one instance, so the first entry reports the *second* collection's numbers. A
  Delta-vs-Cumulative test that keeps `Metric` objects and reads them at the end agrees
  with itself while measuring nothing. This is lie #7 in its sharpest form, and it is
  why the harness grew `MetricReadout.Of(...)`: **snapshot immediately after every
  collection.** `MetricProbe` now delegates to it.
- **`MetricReadout` had to be named that**, because `OpenTelemetry.Metrics` already
  declares a public `MetricSnapshot` and the obvious name collides (`CS0104`).
- **Reading a `MetricPoint` needs a writable copy and type-aware accessors.**
  `GetHistogramBuckets()` is not declared `readonly`, so calling it on the `ref readonly`
  loop variable is `CS1510`; the buckets enumerator yields `HistogramBucket` **by value**,
  so a `ref readonly` loop variable over it is `CS1510` again; and calling the wrong
  accessor (`GetSumLong` on a histogram) throws, so `metric.MetricType.IsHistogram()` /
  `IsGauge()` / `IsLong()` have to be consulted first.
- **A `Sampler` sees only the tags passed to `StartActivity`.** It runs before the span
  exists — that is the point, since its answer decides whether the span gets built — so
  a tag set afterwards can never influence it. Deciding on a status code or a duration
  is not possible here at all; that is what tail sampling in a collector is for, and why
  row 056 exists.
- **`RecordOnly` is not "half sampled".** Measured: the activity is fully populated
  (`IsAllDataRequested` true) so a processor can read it, `Recorded` is false so
  downstream sees an unsampled traceparent, and it is **never exported**. A `Drop` and a
  `RecordOnly` are indistinguishable by export count alone — `IsAllDataRequested` is
  what separates them, which is exactly what ex031's adversarial fact asserts.
- **There is no metric backlog.** An instrument written to before a `MeterProvider`
  existed had nowhere to aggregate, so those measurements were not buffered, not queued
  and not late — as far as any reader is concerned they never happened. Metrics recorded
  during startup, before the host is built, are lost silently.
- **`ExemplarFilterType.TraceBased` reads `Activity.Current` at `Record` time**, so a
  measurement taken after the span closed carries no exemplar — which is what ex035's
  wrong-implementation probe demonstrates.
- Every wrong implementation was caught; two by exactly one fact (ex031's `RecordOnly`
  collapsed into `Drop`, ex032's wildcard `AddMeter`).
- Batch baseline after rows 001–035: **176 facts total** (170 exercise + 6 harness).
  Red run 170 failed / 5 passed / 1 skipped; green run 175 passed / 1 skipped; green
  with `-p:Containers=true` 176 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 036–040 — logs join the SDK, and context crosses a boundary:**

- **`LogRecord.Body` holds the message TEMPLATE, not the rendered sentence.** Measured:
  a `LogInformation("Order {OrderId} shipped to {City}", …)` produces
  `Body == "Order {OrderId} shipped to {City}"`, and `FormattedMessage` is **null**
  unless the pipeline sets `IncludeFormattedMessage`. Everyone reads "Body" as "the
  message" and an OTLP viewer shows it that way — but this is good news, because
  grouping and alerting by event work on `Body` directly while the varying part lives in
  `Attributes`.
- **`LogRecord.Attributes` needs no opt-in.** The structured fields and the
  `{OriginalFormat}` entry are simply there for a normal message template.
  `ParseStateValues` still exists and is not obsolete, but it is not what makes this
  work — do not reach for it.
- **`LogRecord` objects are NOT reused between exports** (two records are two distinct
  instances), unlike `Metric`. So `LogRecordReadout` is a convenience where
  `MetricReadout` is a necessity. The metric side is the exception, not the rule.
- **Two logging libraries in this repository disagree about scopes, and neither says
  so.** `FakeLogger` (block 01) captures them unconditionally; the OpenTelemetry
  pipeline drops them silently unless built with `IncludeScopes = true`. Ex037 grades
  both directions for exactly this reason.
- **Trace correlation is read from `Activity.Current`, not configured.** Nothing in the
  logging call mentions a trace, and with no span in scope the SDK writes **all-zero**
  ids rather than inventing one — which a backend reads as "unlinked" rather than as a
  trace it has lost.
- **The SDK's default propagator is a composite** (trace context **plus** baggage), and
  that matters: a hand-rolled injector writing only `traceparent` drops every piece of
  baggage at that boundary silently, and nothing downstream can tell "no baggage was
  set" from "the hop threw it away".
- **Ex039 deliberately builds on ex038** and calls its `Inject`/`Extract`. Consequence
  measured during the probe: with ex038 *also* wrong, four of ex039's five facts fail
  instead of the one the row is about. Isolated, its wrong implementation fails exactly
  `Adversarial_A`. The dependency is now stated in ex039's header — do those two rows in
  order.
- Batch baseline after rows 001–040: **200 facts total** (194 exercise + 6 harness).
  Red run 194 failed / 5 passed / 1 skipped; green run 199 passed / 1 skipped; green
  with `-p:Containers=true` 200 passed / 0 skipped. 0 warnings in both modes.

**2026-09-06, rows 041–045 — the first real containers, and `04-web-services` begins:**

- **The container fact earned its place on its first outing, and this is the example to
  point at.** Ex043's four in-process assertions all passed against a Prometheus
  document that a real `promtool check metrics` **rejects**: `orders_processed_total no
  help text`. The exporter emits a `# HELP` line only when the instrument was created
  with a *description*, and nothing in the SDK, the middleware or any reasonable
  assertion says so. The row now requires a description, and carries a fifth in-process
  fact for the `# HELP` line so it is graded without Docker too — but nobody would have
  known to write that fact without the strict reader.
- **`AddHttpClientInstrumentation` cannot be exercised in-memory at all.** Measured: the
  diagnostics handler it listens to is inserted by the real socket handler chain, so a
  client built over any custom handler — which is what `TestServer` hands you — produces
  **zero** spans. That is a property of the transport, not of the instrumentation, and it
  is why row 041 is built on the server side and on `AddRuntimeInstrumentation` instead.
- **`AddAspNetCoreInstrumentation` works fully under `TestServer`**, and the span it
  produces is *already* named after the route template (`GET /orders/{id}`). No
  configuration makes that happen — it is what the instrumentation does.
- **`AddInMemoryExporter` has no options overload for traces**, so a row about batching
  has to construct `new BatchActivityExportProcessor(new InMemoryExporter<Activity>(…),
  scheduledDelayMilliseconds: …)` by hand. No bad thing when the processor is the
  subject.
- **Two Testcontainers details, both measured:** `ContainerBuilder()`'s parameterless
  constructor is `[Obsolete]` in 4.14 (`CS0618`, and this track forbids warnings) — use
  `new ContainerBuilder(image)`. And a container whose command exits immediately never
  satisfies the default "until running" wait strategy, so `StartAsync` fails before
  anything can be read: keep the container idling and use `ExecAsync`, whose
  `ExitCode` is `long?`.
- The two container harnesses are deliberately different shapes, and both are worth
  copying. `CollectorContainer` runs a real OTel Collector with a `debug` exporter and
  asserts against its **logs** — no second protocol, no query API, and nothing
  connecting back into the test host. `PromtoolContainer` needs no ports at all: the
  document goes in as a mapped file and the exit code is the answer.
- Batch baseline after rows 001–045: **227 facts total** (221 exercise + 6 harness), of
  which **3 are container-gated**. Red run 219 failed / 5 passed / 3 skipped; green run
  224 passed / 3 skipped; green with `-p:Containers=true` **227 passed / 0 skipped**.
  0 warnings in both modes.

**2026-09-06, rows 046–050 — and a defect found in already-shipped work:**

- **There is exactly one way to force a root activity, and it is not the obvious one.**
  Measured, under an ambient span:
  - `StartActivity(name, kind, parentContext: default)` → **inherits** the ambient
    parent. Not a root.
  - `StartActivity(name, kind, parentId: null)` → **inherits** too. Not a root.
  - clearing `Activity.Current` around the call → a genuine root
    (`ParentSpanId == default`).

  Both of the first two read as though they should work, and on a bare thread they do -
  which is why this survived review. **Row 019 shipped with the wrong mechanism and a
  header claiming "a ROOT, whatever happens to be ambient".** It has been corrected, and
  its adversarial fact now runs *inside* an ambient span so the row grades what it
  claims. Row 049 was written correctly from the start only because the probe caught it
  first.
- **How it was caught is the point.** Ex049's wrong-implementation probe - dropping the
  explicit `parentContext: default` - changed *nothing*, because the test ran on a bare
  thread where both forms produce roots. A probe that catches nothing is not a passing
  probe; it means the fact is not measuring the mechanism. Chasing that produced the
  measurement above and the fix to row 019.
- **The exported order of ASP.NET Core server spans is NOT request order.** Measured:
  `GetAsync` returns once the response headers arrive while the server span ends a moment
  later, so two sequential requests can interleave in the export list. A test that
  indexes into that list is flaky in the direction that passes locally - look spans up by
  `url.path` instead.
- **`AddAspNetCoreInstrumentation`'s `Filter` runs before the activity exists**, so a
  filtered request costs a predicate rather than a span that is built, serialised and
  discarded downstream. That is the cheapest performance win in this track: a liveness
  probe at fifty replicas is a million spans a day describing nothing that ever varies.
- Row 047 is deliberately a **hand-written `DelegatingHandler`** rather than
  `AddHttpClientInstrumentation`, for the reason row 041 measured - and writing it out is
  also the only way to see *which* context gets injected. Injecting the ambient one
  instead of the client span's own makes the client and server spans siblings, and the
  network hop vanishes from the waterfall.
- Batch baseline after rows 001–050: **257 facts total** (251 exercise + 6 harness), of
  which 3 are container-gated. Red run 249 failed / 5 passed / 3 skipped; green run 254
  passed / 3 skipped; green with `-p:Containers=true` 257 passed / 0 skipped. 0 warnings
  in both modes.
