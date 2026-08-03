"""Exercise 011 — enumerate and zip (reference solution)."""

import itertools


def numbered_lines(lines: list[str]) -> list[str]:
    return [f"{number}: {line}" for number, line in enumerate(lines, start=1)]


def index_of_first(values: list[str], target: str) -> int:
    for index, value in enumerate(values):
        if value == target:
            return index
    return -1


def positions_of(values: list[str], target: str) -> list[int]:
    return [index for index, value in enumerate(values) if value == target]


def sum_products(a: list[int], b: list[int]) -> int:
    # strict=True turns a length mismatch into a ValueError instead of quietly
    # truncating to the shorter input.
    return sum(x * y for x, y in zip(a, b, strict=True))


def merge_labels(names: list[str], values: list[int]) -> list[str]:
    # Here truncation is the documented behaviour, so plain zip is right.
    return [f"{name}={value}" for name, value in zip(names, values)]


def unzip(rows: list[tuple[str, int]]) -> tuple[list[str], list[int]]:
    if not rows:
        # zip(*[]) yields nothing at all, so there is no pair to unpack.
        return [], []
    names, values = zip(*rows)
    return list(names), list(values)


def running_totals(values: list[int]) -> list[tuple[int, int]]:
    return list(zip(values, itertools.accumulate(values)))
