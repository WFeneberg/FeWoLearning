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

Go and Rust are installed but **not on `PATH`** for a plain shell. They live at:

- `C:\Program Files\Go\bin\go.exe`
- `%USERPROFILE%\.cargo\bin\` (`cargo`, `rustc`, `rustup`, `clippy`, `rustfmt`)

## Rust cannot link yet

`cargo test` fails with `linker 'link.exe' not found`. The `stable-x86_64-pc-windows-msvc`
toolchain is the only one installed, and this machine is missing the pieces it
needs:

- `VC\Tools\MSVC\14.51.36231\lib\x64` — **absent** (only the compiler binaries
  are present, not the libraries)
- `C:\Program Files (x86)\Windows Kits\10\Lib` — **no Windows SDK**
- `VC\Auxiliary\Build\vcvars64.bat` exists but calls a `vcvarsall.bat` that does not

Fix by adding the C++ workload. This **must run elevated** — with `--passive` from
a non-elevated shell the installer exits with code `5007` and does nothing:

```powershell
& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\setup.exe" modify `
  --installPath "C:\Program Files\Microsoft Visual Studio\18\Professional" `
  --add Microsoft.VisualStudio.Workload.NativeDesktop --includeRecommended
```

Verify afterwards with `cargo test` in `rust/`.

The lighter alternative, if you would rather not install the VS workload, is the
GNU toolchain — `rustup` ships its own linker for it, so no MSVC or SDK is needed:

```powershell
rustup toolchain install stable-x86_64-pc-windows-gnu
rustup default stable-x86_64-pc-windows-gnu
```

RustRover then has to be pointed at the `gnu` toolchain as well.

## Per-track test-runner dependencies

| Track     | One-time setup                                              |
|-----------|------------------------------------------------------------|
| `dotnet/` | none — `dotnet test` restores NuGet packages on first run  |
| `python/` | `python -m pip install -e ".[dev]"` (installs pytest, ruff, mypy) |
| `vue/`    | `npm install` — done, `node_modules` present               |
| `angular/`| `npm install` — done, `node_modules` and `package-lock.json` present |
| `go/`     | none — `go mod download` already ran (`golang.org/x/sync`)  |
| `rust/`   | blocked on the linker, see above                            |

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

If you use the unified IDE, install the corresponding language plugins and open
folders as separate windows so each keeps its own SDK/interpreter selection.
