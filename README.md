# FeWoLearning — A Polyglot Skills Training Environment

A personal, self-paced training ground for practicing and expanding programming
skills across six ecosystems. Each language lives in its own self-contained
folder with an isolated toolchain, its own test runner, and a graded set of
**exercises** paired with reference **solutions**.

## Languages & tracks

| Folder      | Ecosystem                        | Test runner        | Status of local toolchain |
|-------------|----------------------------------|--------------------|---------------------------|
| `dotnet/`   | C# / .NET (Core, WPF, Avalonia, Uno, Blazor) | xUnit  | ✅ .NET 10 installed       |
| `python/`   | Python 3                         | pytest             | ✅ Python 3.14 installed   |
| `vue/`      | Vue 3 (Composition API, TS)      | Vitest             | ✅ Node 26 installed       |
| `angular/`  | Angular (standalone, TS)         | Jest / Karma       | ✅ Node 26 installed       |
| `go/`       | Go                               | `go test`          | ⚠️ Go not yet installed    |
| `rust/`     | Rust                             | `cargo test`       | ⚠️ Rust not yet installed  |

See [`docs/requirements.md`](docs/requirements.md) for exact versions and
install instructions for the missing toolchains.

## How the exercises work

Every language folder follows the same layout:

```
<lang>/
  exercises/     # your workspace — stubs with TODOs and failing tests
    01-beginner/
    02-intermediate/
    03-advanced/
    04-expert/
  solutions/     # reference implementations (same tiered layout)
  catalog.md     # the full 100-exercise roadmap for this language
```

Each exercise is **test-driven**: a stub ships with failing tests. You make the
tests pass. The matching file under `solutions/` is a worked reference. See
[`docs/exercise-format.md`](docs/exercise-format.md) for the exact convention.

## Working with JetBrains tools

Open each language folder as its **own project/solution** in the matching JetBrains
IDE (Rider, PyCharm, GoLand, WebStorm, RustRover, or one unified IDE with plugins).
Do **not** open the repository root as a single project — the toolchains are
deliberately isolated. See the per-language `README.md` for the exact IDE mapping.
