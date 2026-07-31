# Requirements & Environment Setup

This document records the toolchains each track needs, what is already present on
this machine, and how to install what is missing. Detected versions were captured
on **2026-07-31**.

## Detected on this machine

| Tool   | Detected version | Used by            |
|--------|------------------|--------------------|
| .NET SDK | `10.0.302`     | `dotnet/`          |
| Python | `3.14.6`         | `python/`          |
| Node.js | `26.5.0`        | `vue/`, `angular/` |
| npm    | `11.17.0`        | `vue/`, `angular/` |
| Git    | `2.55.0`         | all                |
| Go     | **not installed**| `go/`              |
| Rust   | **not installed**| `rust/`            |

## Installing the missing toolchains

The recommended installer on Windows is **winget** (ships with Windows 11).

### Go

```powershell
winget install --id GoLang.Go -e
```

Verify: `go version` (target: Go 1.22+). The `go/` module targets the toolchain
declared in `go/go.mod`.

### Rust

Install via `rustup` (the official toolchain manager):

```powershell
winget install --id Rustlang.Rustup -e
rustup default stable
```

Verify: `rustc --version` and `cargo --version` (target: stable, 1.75+).

## Per-track test-runner dependencies

| Track     | One-time setup                                              |
|-----------|------------------------------------------------------------|
| `dotnet/` | none — `dotnet test` restores NuGet packages on first run  |
| `python/` | `python -m pip install -e ".[dev]"` (installs pytest, ruff, mypy) |
| `vue/`    | `npm install`                                              |
| `angular/`| `npm install`                                             |
| `go/`     | none — `go test ./...` after Go is installed               |
| `rust/`   | none — `cargo test` after Rust is installed                |

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
