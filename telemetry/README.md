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
