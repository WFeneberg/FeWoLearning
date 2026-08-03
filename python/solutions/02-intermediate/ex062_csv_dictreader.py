"""Exercise 062 — csv (reference solution)."""

import csv
from pathlib import Path
from typing import Any, Iterator

_TRUTHY = {"true", "1", "yes"}


def read_rows(path: Path) -> list[dict[str, str]]:
    # newline="" is not optional: the csv module does its own line-ending handling,
    # and Python's universal-newline translation on top of it would corrupt a
    # quoted field that contains a newline.
    with path.open(newline="", encoding="utf-8") as handle:
        return list(csv.DictReader(handle))


def read_rows_typed(path: Path) -> list[dict[str, Any]]:
    rows: list[dict[str, Any]] = []
    for row in read_rows(path):
        rows.append(
            {
                "name": row["name"],
                # CSV carries no types, so every conversion happens here.
                "age": int(row["age"]),
                "active": row["active"].strip().casefold() in _TRUTHY,
            }
        )
    return rows


def write_rows(path: Path, rows: list[dict[str, str]], fieldnames: list[str]) -> None:
    with path.open("w", newline="", encoding="utf-8") as handle:
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        # writeheader() runs regardless of whether there are rows.
        writer.writeheader()
        writer.writerows(rows)


def read_custom(path: Path, delimiter: str) -> list[dict[str, str]]:
    with path.open(newline="", encoding="utf-8") as handle:
        return list(csv.DictReader(handle, delimiter=delimiter))


def iter_rows(path: Path) -> Iterator[dict[str, str]]:
    # The `with` lives inside the generator, so the handle stays open across yields
    # and is closed when the generator finishes or is garbage-collected.
    with path.open(newline="", encoding="utf-8") as handle:
        yield from csv.DictReader(handle)


def column_values(path: Path, column: str) -> list[str]:
    # Subscripting raises KeyError for a column that is not in the header.
    return [row[column] for row in read_rows(path)]


def round_trip(
    path: Path, rows: list[dict[str, str]], fieldnames: list[str]
) -> list[dict[str, str]]:
    write_rows(path, rows, fieldnames)
    return read_rows(path)
