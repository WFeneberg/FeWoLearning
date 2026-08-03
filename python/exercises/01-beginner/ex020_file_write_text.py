"""Exercise 020 — Writing text files (beginner).

Goal:   Write and append text safely, with explicit encoding and newlines.
Drills: open() modes ("w" vs "a" vs "x"), writelines vs write, encoding,
        creating parent directories, round-tripping.
Passes: when `pytest exercises/01-beginner/test_ex020_file_write_text.py` is green.
"""

from pathlib import Path


def write_lines(path: Path, lines: list[str]) -> int:
    """Write each line followed by "\\n", replacing any existing file.

    Return the number of characters written. Use UTF-8 explicitly — the platform
    default would make the file unreadable elsewhere — and pass ``newline="\\n"``
    so Windows does not turn each "\\n" into "\\r\\n" on disk.
    """
    raise NotImplementedError


def append_line(path: Path, line: str) -> None:
    """Append one line plus "\\n", creating the file when it does not exist."""
    raise NotImplementedError


def write_once(path: Path, text: str) -> None:
    """Write `text`, refusing to overwrite an existing file.

    An existing path raises FileExistsError — let the "x" mode do that rather than
    checking with `exists()` first, which would leave a race between the two steps.
    """
    raise NotImplementedError


def write_in_new_dir(path: Path, text: str) -> None:
    """Write `text`, creating any missing parent directories first."""
    raise NotImplementedError


def write_csv_ish(path: Path, rows: list[tuple[str, int]]) -> None:
    """Write ``name,value`` per row, with a ``name,value`` header line."""
    raise NotImplementedError


def round_trip(path: Path, lines: list[str]) -> list[str]:
    """Write `lines`, read them back, and return what came back.

    Proves the write/read pair agree on encoding and newline handling.
    """
    raise NotImplementedError
