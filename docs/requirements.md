# Requirements & Environment Setup

This document records the toolchains each track needs, what is already present on
this machine, and how to install what is missing. Detected versions were captured
on **2026-08-03**.

## Detected on this machine

| Tool   | Detected version | Used by            |
|--------|------------------|--------------------|
| .NET SDK | `10.0.302`     | `dotnet/`          |
| Python | `3.14.6`         | `python/`          |
| Node.js | `26.5.0`        | `vue/`, `angular/` |
| npm    | `11.17.0`        | `vue/`, `angular/` |
| Git    | `2.55.0`         | all                |
| Go     | `1.26.5`         | `go/`              |
| Rust   | `1.97.1` (cargo `1.97.1`) | `rust/`   |
| JDK    | not installed / unverified | `java/`, `kotlin/` |
| Kotlin | not installed / unverified | `kotlin/` |

Go and Rust are installed but **not on `PATH`** for a plain shell. They live at:

- `C:\Program Files\Go\bin\go.exe`
- `%USERPROFILE%\.cargo\bin\` (`cargo`, `rustc`, `rustup`, `clippy`, `rustfmt`)

## Rust linking (fixed — keep in mind if VS is upgraded)

`cargo test` links correctly now, via [`rust/.cargo/config.toml`](../rust/.cargo/config.toml).
The history matters because the fix is version-pinned.

Two Visual Studio installs are present, and rustc auto-detects the **wrong** one:

| Install                    | MSVC toolset  | Desktop `lib\x64` |
|----------------------------|---------------|-------------------|
| VS 18 Professional (picked by rustc) | `14.51.36231` | ❌ only `lib\onecore\{x64,x86}` |
| VS 2022 Community          | `14.44.35207` | ✅ present        |

So `link.exe` ran with a `LIB` that had no desktop `msvcrt.lib` and failed with:

```
LINK : fatal error LNK1104: cannot open file 'msvcrt.lib'
```

The Windows 10 SDK **is** installed (`10.0.22621.0` and `10.0.26100.0`, with
`ucrt\x64` and `um\x64`) — that part of the earlier diagnosis was wrong.

The fix is to point `LIB` at VS 2022's desktop libs plus the SDK. `rust/.cargo/config.toml`
does this through cargo's `[env]` table, so plain `cargo test` and RustRover both
work with no developer shell. It is deliberately **not** `force = true`, so a real
"x64 Native Tools" prompt (which exports its own `LIB`) still wins.

**If either install is upgraded, refresh the two version numbers in that file.**
Equivalent one-off from a shell:

```powershell
cmd /c 'call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat" && cargo test'
```

Two alternatives, neither needed today: add
`Microsoft.VisualStudio.Workload.NativeDesktop` to the VS 18 install via an
**elevated** `setup.exe modify` (non-elevated `--passive` exits `5007` and does
nothing), or switch to the GNU toolchain, which ships its own linker:

```powershell
rustup toolchain install stable-x86_64-pc-windows-gnu
rustup default stable-x86_64-pc-windows-gnu
```

## Per-track test-runner dependencies

| Track     | One-time setup                                              |
|-----------|------------------------------------------------------------|
| `dotnet/` | none — `dotnet test` restores NuGet packages on first run  |
| `python/` | `python -m pip install -e ".[dev]"` (installs pytest, ruff, mypy) |
| `vue/`    | `npm install` — done, `node_modules` present               |
| `angular/`| `npm install` — done, `node_modules` and `package-lock.json` present |
| `go/`     | none — `go mod download` already ran (`golang.org/x/sync`)  |
| `rust/`   | none — `.cargo/config.toml` supplies `LIB`, see above        |
| `java/`   | planned — add JDK 21 and a Gradle wrapper when the track is scaffolded |
| `kotlin/` | planned — add JDK 21, Kotlin, and a Gradle wrapper when the track is scaffolded |

Set `GOTMPDIR` outside `%TEMP%` when running `go test`: on-access scanning can
remove a freshly built test binary before Go execs it, which surfaces as
`fork/exec …: The system cannot find the file specified`.

## JetBrains IDE mapping

Open **each language folder as its own project**, not the repo root.

| Track     | JetBrains IDE            | Open as                     |
|-----------|--------------------------|-----------------------------|
| `dotnet/` | Rider                    | `dotnet/FeWoLearning.Dotnet.slnx` |
| `python/` | PyCharm                  | `python/` folder            |
| `vue/`    | WebStorm                 | `vue/` folder               |
| `angular/`| WebStorm                 | `angular/` folder           |
| `go/`     | GoLand                   | `go/` folder                |
| `rust/`   | RustRover                | `rust/` folder              |
| `java/`   | IntelliJ IDEA            | `java/` folder              |
| `kotlin/` | IntelliJ IDEA            | `kotlin/` folder            |

If you use the unified IDE, install the corresponding language plugins and open
folders as separate windows so each keeps its own SDK/interpreter selection.

Java and Kotlin are **catalog-only** right now: the track folders exist for the
exercise ledgers, but no Gradle build, stubs, tests, or reference solutions have
been seeded yet.
