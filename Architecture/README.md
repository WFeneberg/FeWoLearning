# Architecture Track

60 graded C# exercises in **application and system architecture** on .NET 10,
across the three areas this repo's owner actually ships into — **web**
(ASP.NET Core), **desktop** (the composition and lifecycle patterns behind a UI),
and **services + data** (databases, caches, message buses, MQTT) — plus a
**cross-cutting** block for the concerns that run through all three.

See `catalog.md` for the row-by-row ledger, and
`docs/superpowers/specs/2026-09-06-architecture-track-design.md` for the design.

## What this track is not

**Not a second `MicroServices/`.** That track teaches Aspire and the *deployment
topology*: how resources are declared, wired, published and started. This one
teaches the *patterns inside the process*: what an outbox actually guarantees, why
a cache-aside loader must be **counted** rather than observed, what a saga
compensates, which direction dependencies are allowed to point. Where the two
touch — a message bus, a Postgres row — this track owns the pattern and
`MicroServices/` owns the orchestration.

**Not a UI track.** `wpf/`, `caliburn/`, `avalonia/` and `uno/` cover UI
frameworks. Block `02-desktop` here is deliberately **UI-framework-free**: MVVM
composition, navigation, messaging, plugin loading, offline sync and undo/redo are
all architecture, and every one of them is testable without a rendering stack. The
payoff is that this entire track is headless, cross-platform and CI-runnable —
unlike `wpf/`, `caliburn/` and block `04` of `security/`, all of which need an
interactive Windows desktop session.

## Setup and commands

Nothing to install for the default run. Run every command **from inside
`Architecture/`**, not the repo root.

| Run | Command |
|---|---|
| Stubs (red) | `dotnet test` |
| Solutions (green) | `dotnet test -p:UseSolutions=true` |
| Including the container rows | `dotnet test -p:Containers=true` (needs Docker) |
| One exercise | `dotnet test --filter FullyQualifiedName~Ex001_` |

`-p:UseSolutions=true` swaps which content library `tests/` references. Because
`exercises/` and `solutions/` compile **the same type names into the same
namespaces**, exactly one of them is ever referenced — never both — which is what
keeps them from colliding, and which makes the reference solutions compile-checked
and test-run on every green check instead of drifting silently. This is the same
deviation from the repo-wide "`solutions/` stays out of the build" convention that
`avalonia/`, `blazor/`, `uno/`, `caliburn/`, `wpf/` and `security/` take. It is
deliberate and permanent.

`Directory.Build.props` redirects the solutions build to `artifacts-solutions/` via
`UseArtifactsOutput`/`ArtifactsPath`. That is **required, not cosmetic**: two
projects emitting the same generated assembly-info attributes into one `obj/` tree
fail the build with `CS0579`. It also has to live in `Directory.Build.props` and not
in a `.csproj` body, where `BaseOutputPath` is read after the SDK props import and
therefore too late.

## Toolchain

Everything targets **`net10.0`**. No `net10.0-windows`, no `UseWPF`, no Windows-only
API anywhere.

The test project is pinned to **xunit.v3 3.2.2**, **`xunit.runner.visualstudio`
3.1.5** and **`Microsoft.NET.Test.Sdk` 17.14.1** on the classic VSTest path, and this
track ships **no `global.json`**. That combination is copied from `MicroServices/`
because it is the one measured to work here:

- xunit.v3 **4.0.0** plus a `Microsoft.Testing.Platform` `global.json` makes
  `dotnet test` exit 5 with **zero tests discovered** on the .NET 10.0.400 SDK — the
  failure `wpf/` currently has.
- `xunit.runner.visualstudio` has **no 3.1.6 or 3.1.7**. 3.1.5 is the last 3.x and
  the next release is 4.0.0. Naming a nonexistent 3.x patch does not fail the build:
  NuGet resolves *forward* to 4.0.0 with only an `NU1603` warning, landing back on
  the broken generation with no error to catch it.

`MQTTnet` 5 **splits its broker into a separate `MQTTnet.Server` package**. That one
lives in `tests/` only — the exercises are MQTT *clients*; only the harness starts a
broker.

Unlike `security/`, this track does **not** pin `SQLitePCLRaw.lib.e_sqlite3`.
`Microsoft.Data.Sqlite` 10.0.11 no longer drags in the 2.1.11 that carried
GHSA-2m69-gcr7-jv3q, and the build is measured at 0 warnings without the pin. If a
future bump reintroduces `NU1903`, pin it again the way `security/` does.

`tests/` suppresses **`xUnit1051` only**, via `NoWarn`. Any other warning in that
project is a finding. `solutions/` must build with **zero warnings** — a warning
there is a finding too. `exercises/` may emit `CS0169`/`CS0414`/`CS0649` from fields
a stub declares for the learner to wire up; those stay unsuppressed deliberately.

## The three infrastructure tiers

**Tier 1 — fakes (default).** An in-memory bus, caches whose loader calls are
counted, and `ManualClock` from `_support/`. Milliseconds, deterministic, no
external process. Most rows live here.

**Tier 2 — real, but in-process (also default, still no Docker).**
`Microsoft.Data.Sqlite` backs every transaction, outbox, concurrency and
offline-sync row, because **outbox atomicity cannot be honestly proven against a
fake** — a fake that "rolls back" does so because the fake was written to do that.
`SqliteScratch` is a temp **file** database and not `:memory:` on purpose: those
rows prove a transaction boundary by opening a *second* connection, and every
`:memory:` connection gets its own private database, which would make the facts
pass vacuously.

And **MQTT runs against a real MQTTnet 5 broker started in this process** on a
loopback port: real protocol frames, real QoS 1 redelivery, real retained-message
delivery to a late subscriber, a real last will on an ungraceful disconnect. MQTT
is therefore fully graded in the default run rather than gated behind Docker.

**Tier 3 — containers (`-p:Containers=true`).** Eight rows (032, 036, 037, 038,
039, 046, 047, 050) carry *additional* Testcontainers-backed facts against real
Postgres, Redis, RabbitMQ and Mosquitto. They are skipped by default via
`ContainerGate.SkipUnlessEnabled()`. **Every one of those exercises is still fully
graded without Docker** by its in-process facts — the container facts add realism,
never coverage that would otherwise be missing.

The gate is a call in the test body rather than a custom `[ContainerFact]`, because
`FactAttribute.Skip` is not virtual in xunit.v3 3.2.2 and overriding it fails
`CS0506`. The `-p:Containers=true` MSBuild property reaches the test process through
a `RuntimeHostConfigurationOption`, since an MSBuild property is otherwise invisible
at runtime. `FEWO_ARCH_CONTAINERS=1` is the no-rebuild alternative.

## The harness

`tests/_harness/` holds four things and four smoke facts that prove them. Those
smoke facts are **the only tests in this track that pass in both modes**; they exist
so that a broken harness fails loudly and first, instead of surfacing as sixty
confusing exercise failures. If one goes red after a package bump, fix it before
reading anything else in the run.

| Piece | What it gives you |
|---|---|
| `ManualClock` (in `_support/`, both content libraries) | virtual time; no test in this track sleeps |
| `SqliteScratch` | a temp-file SQLite database two connections genuinely share |
| `MqttBrokerFixture` | a real broker in-process, `StartAsync()` + `ConnectClientAsync(id)` |
| `ContainerGate` | `SkipUnlessEnabled()` for the eight 🐳 rows |

`_support/Clock.cs` is **byte-identical** in `exercises/` and `solutions/`. A
divergence there breaks the green run in a way that looks like an exercise bug.

## How an architecture test lies

This is the section to read before writing a fact. It is this track's equivalent of
`security/`'s "an attack fact with no paired use fact grades nothing" — the
recurring bug class that per-exercise review keeps missing.

**Lie 1 — the outcome is reachable without the pattern.** An outbox test that only
asserts "the message arrived" is satisfied by a direct publish that skips the outbox
entirely. A cache test that only asserts "the value came back" is satisfied by an
implementation with no cache at all. A repository test that only asserts "the entity
was saved" is satisfied by a bare data-context call in the handler. The fix is
always the same: assert the **mechanism's own observable side effect** — the row
committed inside the same transaction, the loader's invocation count, the checkpoint
that advanced — never merely the end state.

**Lie 2 — the pattern is asserted but never exercised.** An idempotent-consumer test
that delivers each message exactly once proves nothing: a consumer with no dedup
passes it. A circuit-breaker test that never trips the breaker grades nothing. A
concurrency test with one writer cannot detect a lost update. So every mechanism
exercise carries an **adversarial fact** in which the naive implementation
demonstrably diverges — a duplicate delivery, a failing commit, a second concurrent
writer, a clock advanced past the break duration.

**Lie 3 — the test asserts structure the runtime does not enforce.** No behavioural
assertion can distinguish "the domain does not reference infrastructure" from "the
domain happens not to call it in this test". Rows about dependency direction,
layering and module boundaries (001, 026, 041, 058, 060) therefore read **assembly
metadata by reflection**, and go red on an assertion rather than on a stub's
`NotImplementedException`. That is deliberate and is noted at each such fact.

**Two probes, not one.** Every batch is checked by temporarily replacing each
reference solution with (a) the **degenerate** implementation — a constant, a no-op,
the pattern skipped — and confirming the facts fail; and then with (b) the
**plausible wrong** one — the pattern implemented earnestly but with the wrong
mechanism — and confirming the facts still fail. Probe (b) is the one that matters:
it is the bug class `security/`'s final review found after every per-batch review
had already passed, and the degenerate probe never catches it.

## Adding an exercise

Follow the repo procedure in `CLAUDE.md` ("Adding or completing exercises"):
`catalog.md` is the work queue, work in batches of five, red-check filtered, green-
check with `-p:UseSolutions=true`, flip exactly those rows, commit as
`Architecture: exNNN–exNNN`. Then run both probes above before flipping anything.

Stubs carry a `Goal:` / `Drills:` / `Passes:` header comment, throw
`NotImplementedException` at runtime, and must still **compile** while unfinished.
Namespaces are pinned per block — `FeWoLearning.Architecture.Exercises.Web`,
`.Desktop`, `.ServicesData`, `.CrossCutting` — because `01-web` is not a valid C#
identifier. Tests mirror them as `FeWoLearning.Architecture.Tests.<Block>`.
