"""Exercise 011 — enumerate and zip (beginner).

Goal:   Iterate with indices and over several sequences at once, without
        `range(len(...))`.
Drills: enumerate with a start offset, zip, zip(strict=True), unzipping with
        zip(*rows), parallel iteration.
Passes: when `pytest exercises/01-beginner/test_ex011_enumerate_zip.py` is green.
"""


def numbered_lines(lines: list[str]) -> list[str]:
    """Prefix each line with its 1-based number and a colon-space.

    ``numbered_lines(["a"])`` -> ``["1: a"]``. Use enumerate's `start`, not manual
    counting.
    """
    raise NotImplementedError


def index_of_first(values: list[str], target: str) -> int:
    """Return the index of the first item equal to `target`, or -1 when absent."""
    raise NotImplementedError


def positions_of(values: list[str], target: str) -> list[int]:
    """Return every index at which `target` occurs, in ascending order."""
    raise NotImplementedError


def sum_products(a: list[int], b: list[int]) -> int:
    """Return the dot product of the two lists.

    Lists of differing length must raise ValueError rather than silently ignoring
    the extra items — that is what ``zip(strict=True)`` is for.
    """
    raise NotImplementedError


def merge_labels(names: list[str], values: list[int]) -> list[str]:
    """Return ``"name=value"`` for each pair, stopping at the shorter list."""
    raise NotImplementedError


def unzip(rows: list[tuple[str, int]]) -> tuple[list[str], list[int]]:
    """Split a list of pairs into two lists.

    ``unzip([("a", 1), ("b", 2)])`` -> ``(["a", "b"], [1, 2])``. An empty input
    yields two empty lists.
    """
    raise NotImplementedError


def running_totals(values: list[int]) -> list[tuple[int, int]]:
    """Return ``(value, total_so_far)`` for each item.

    ``running_totals([1, 2, 3])`` -> ``[(1, 1), (2, 3), (3, 6)]``.
    """
    raise NotImplementedError
