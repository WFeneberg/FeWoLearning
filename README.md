# FeWoLearning — A Polyglot Skills Training Environment

A personal, self-paced training ground for practicing and expanding programming
skills across nine ecosystems. Each language lives in its own self-contained
folder with an isolated toolchain, its own test runner, and a graded set of
**exercises** paired with reference **solutions**.

## Languages & tracks

| Folder      | Ecosystem                        | Test runner | Exercises | Local toolchain |
|-------------|----------------------------------|-------------|-----------|-----------------|
| `dotnet/`   | C# / .NET (Core, WPF, Avalonia, Uno, Blazor) | xUnit | **100 / 100** | ✅ .NET 10 |
| `go/`       | Go                               | `go test`   | **100 / 100** | ✅ Go 1.26 |
| `vue/`      | Vue 3 (Composition API, TS)      | Vitest      | **100 / 100** | ✅ Node 26 |
| `python/`   | Python 3                         | pytest      | **100 / 100** | ✅ Python 3.14 |
| `angular/`  | Angular (standalone, signals, TS)| Jest        | **100 / 100** | ✅ Node 26 |
| `rust/`     | Rust                             | `cargo test`| **100 / 100** | ✅ Rust 1.97 |
| `avalonia/`| Avalonia 12 (ReactiveUI MVVM, C#)| xUnit v3 + Avalonia.Headless | **10 / 100** | ✅ .NET 10 |
| `blazor/`   | Blazor (Razor components, bUnit) | xUnit + bUnit | **35 / 100** | ✅ .NET 10.0.400 |
| `java/`     | Java                             | JUnit 5     | 100 / 100 (unverified) | planned |
| `kotlin/`   | Kotlin                           | JUnit 5     | 100 / 100 (unverified) | planned |
| `flutter/`  | Flutter / Dart                   | `package:test` / `flutter_test` | 100 / 100 (unverified) | planned |
| `caliburn/` | Caliburn.Micro 5 MVVM on WPF (C#) | xUnit v3 + StaFact | **55 / 100** | ✅ .NET 10 |
| `telemetry/`| Monitoring, Logging, OpenTelemetry (C#) | xUnit v3 + StaFact | **50 / 70** | ✅ .NET 10 |

Each track's `catalog.md` is the authoritative per-exercise ledger: it lists all
100 entries with ✅ (written) or ⬜ (planned).

`blazor/`'s beginner tier (ex001–ex035) is verified end-to-end — 115 test
facts red on the stubs, 115 green on the reference solutions; run
`dotnet test -p:UseSolutions=true` from inside `blazor/` to check the
solutions instead of the stubs.

`avalonia/`'s first ten exercises are verified end-to-end — each proven red as
a stub and green against its reference solution by a real `dotnet test`; the
other ninety are planned, not yet written. See `avalonia/catalog.md` for the
live count.

`java/`, `kotlin/`, and `flutter/` are content-complete — every stub, test, and
reference solution exists — but **unverified**: this machine has no JDK/Gradle
(+ Kotlin), or Flutter/Dart SDK installed, so none of it has ever been
compiled or run. See each track's `README.md` for details and known risk
spots.

Rust needed one machine-local fix to link: rustc auto-detects a Visual Studio
install that has no desktop `lib\x64`, so
[`rust/.cargo/config.toml`](rust/.cargo/config.toml) points `LIB` at the install
that does. Refresh the pinned MSVC/SDK versions there if Visual Studio is
upgraded — details in [`docs/requirements.md`](docs/requirements.md).

See [`docs/requirements.md`](docs/requirements.md) for exact versions and setup
per track.

## How the exercises work

Every language folder follows the same layout:

```
<lang>/
  exercises/     # your workspace — stubs with TODOs and failing tests
    01-beginner/       # 001–035
    02-intermediate/   # 036–070
    03-advanced/       # 071–090
    04-expert/         # 091–100
  solutions/     # reference implementations (same tiered layout)
  catalog.md     # the 100-row progress ledger for this language
```

Each exercise is **test-driven**: a stub ships with a failing test. You make it
pass. The matching file under `solutions/` is a worked reference. Exercises are
named `exNNN_<slug>` (`ExNNN_<Slug>` in .NET) because identifiers cannot start
with a digit in most of these languages. See
[`docs/exercise-format.md`](docs/exercise-format.md) for the exact convention,
including the **known gaps** in how `solutions/` gets verified.

## Working with JetBrains tools

Open each language folder as its **own project/solution** in the matching JetBrains
IDE (Rider, PyCharm, GoLand, WebStorm, RustRover, IntelliJ IDEA, or one unified
IDE with plugins).
Do **not** open the repository root as a single project — the toolchains are
deliberately isolated. See the per-language `README.md` for the exact IDE mapping.
