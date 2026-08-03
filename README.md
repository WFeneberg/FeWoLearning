# FeWoLearning — A Polyglot Skills Training Environment

A personal, self-paced training ground for practicing and expanding programming
skills across six ecosystems. Each language lives in its own self-contained
folder with an isolated toolchain, its own test runner, and a graded set of
**exercises** paired with reference **solutions**.

## Languages & tracks

| Folder      | Ecosystem                        | Test runner | Exercises | Local toolchain |
|-------------|----------------------------------|-------------|-----------|-----------------|
| `dotnet/`   | C# / .NET (Core, WPF, Avalonia, Uno, Blazor) | xUnit | **100 / 100** | ✅ .NET 10 |
| `go/`       | Go                               | `go test`   | **100 / 100** | ✅ Go 1.26 |
| `vue/`      | Vue 3 (Composition API, TS)      | Vitest      | **100 / 100** | ✅ Node 26 |
| `python/`   | Python 3                         | pytest      | 8 / 100   | ✅ Python 3.14 |
| `angular/`  | Angular (standalone, signals, TS)| Jest        | 2 / 100   | ✅ Node 26 |
| `rust/`     | Rust                             | `cargo test`| 2 / 100   | ⚠️ cannot link — see below |

Each track's `catalog.md` is the authoritative per-exercise ledger: it lists all
100 entries with ✅ (written and verified) or ⬜ (planned).

**Rust is currently blocked.** The toolchain is installed but only for the
`x86_64-pc-windows-msvc` target, and the MSVC libraries plus the Windows SDK are
missing, so nothing links. Add the C++ workload through the Visual Studio
installer, run **elevated**:

```powershell
& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\setup.exe" modify `
  --installPath "C:\Program Files\Microsoft Visual Studio\18\Professional" `
  --add Microsoft.VisualStudio.Workload.NativeDesktop --includeRecommended
```

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
IDE (Rider, PyCharm, GoLand, WebStorm, RustRover, or one unified IDE with plugins).
Do **not** open the repository root as a single project — the toolchains are
deliberately isolated. See the per-language `README.md` for the exact IDE mapping.
