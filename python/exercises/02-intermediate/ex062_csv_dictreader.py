"""Exercise 062 — csv (intermediate).

Goal:   Read and write CSV correctly, including the cases a naive split() ruins.
Drills: DictReader/DictWriter, quoting and embedded commas/newlines, newline="" when
        opening a file, custom delimiters, type conversion at the boundary.
Passes: when `pytest exercises/02-intermediate/test_ex062_csv_dictreader.py` is green.
"""

from pathlib import Path
from typing import Any, Iterator


def read_rows(path: Path) -> list[dict[str, str]]:
    """Read a CSV with a header row into a list of dicts.

    Open with ``newline=""``: the csv module handles line endings itself, and letting
    Python translate them too corrupts quoted fields containing newlines.
    """
    raise NotImplementedError


def read_rows_typed(path: Path) -> list[dict[str, Any]]:
    """Read a CSV of ``name,age,active`` with converted types.

    `age` becomes an int, `active` becomes a bool ("true"/"1"/"yes" case-insensitively),
    `name` stays a str. CSV has no types — conversion is always the caller's job.
    """
    raise NotImplementedError


def write_rows(path: Path, rows: list[dict[str, str]], fieldnames: list[str]) -> None:
    """Write rows with a header, quoting only where necessary.

    An empty `rows` still writes the header line.
    """
    raise NotImplementedError


def read_custom(path: Path, delimiter: str) -> list[dict[str, str]]:
    """Read a CSV using a non-comma delimiter."""
    raise NotImplementedError


def iter_rows(path: Path) -> Iterator[dict[str, str]]:
    """Yield rows lazily, so a large file is never fully in memory.

    The file must stay open for the whole iteration and be closed afterwards — a
    generator function with the ``with`` inside it does both.
    """
    raise NotImplementedError


def column_values(path: Path, column: str) -> list[str]:
    """Return every value of one column.

    A column that is not in the header raises KeyError.
    """
    raise NotImplementedError


def round_trip(path: Path, rows: list[dict[str, str]], fieldnames: list[str]) -> list[dict[str, str]]:
    """Write then read back, returning what came back.

    Proves the writer's quoting and the reader's parsing agree, even for values
    containing commas, quotes and newlines.
    """
    raise NotImplementedError
