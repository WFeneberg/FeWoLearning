"""Exercise 021 — pathlib (reference solution)."""

from pathlib import Path


def join(base: Path, *parts: str) -> Path:
    result = base
    for part in parts:
        result = result / part
    return result


def split_name(path: Path) -> tuple[str, str, str]:
    # .stem and .suffix split at the *last* dot only, so "report.tar.gz" gives
    # ("report.tar", ".gz") rather than ("report", ".tar.gz").
    return str(path.parent), path.stem, path.suffix


def change_extension(path: Path, suffix: str) -> Path:
    if suffix and not suffix.startswith("."):
        suffix = f".{suffix}"
    # with_suffix("") removes the extension.
    return path.with_suffix(suffix)


def relative(path: Path, base: Path) -> Path:
    # relative_to already raises ValueError for a path outside base.
    return path.relative_to(base)


def list_files(directory: Path) -> list[str]:
    # iterdir() order is filesystem-defined, so sort for a stable answer.
    return sorted(entry.name for entry in directory.iterdir() if entry.is_file())


def find_by_suffix(directory: Path, suffix: str) -> list[str]:
    return sorted(
        entry.name for entry in directory.rglob(f"*{suffix}") if entry.is_file()
    )


def ensure_dir(path: Path) -> Path:
    path.mkdir(parents=True, exist_ok=True)
    return path
