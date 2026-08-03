"""Exercise 005 — Tuple unpacking (beginner).

Goal:   Use tuple packing and unpacking instead of index juggling.
Drills: packing, unpacking, star-targets, swapping, nested unpacking.
Passes: when `pytest exercises/01-beginner/test_ex005_tuple_unpacking.py` is green.
"""


def swap(pair: tuple[int, int]) -> tuple[int, int]:
    """Return the pair with its two items exchanged, using unpacking."""
    raise NotImplementedError


def head_tail(values: list[int]) -> tuple[int, list[int]]:
    """Split into the first item and a list of the rest, via a star-target.

    ``head_tail([1, 2, 3])`` -> ``(1, [2, 3])``. A single-item list yields an empty
    tail. An empty list raises ValueError.
    """
    raise NotImplementedError


def first_last(values: list[int]) -> tuple[int, int]:
    """Return ``(first, last)``. A one-item list returns that item twice.

    An empty list raises ValueError.
    """
    raise NotImplementedError


def min_max(values: list[int]) -> tuple[int, int]:
    """Return ``(smallest, largest)``. An empty list raises ValueError."""
    raise NotImplementedError


def unpack_record(record: tuple[str, tuple[int, int]]) -> tuple[str, int, int]:
    """Flatten a nested record with nested unpacking.

    ``unpack_record(("p", (3, 4)))`` -> ``("p", 3, 4)``.
    """
    raise NotImplementedError


def divmod_pairs(values: list[int], divisor: int) -> list[tuple[int, int]]:
    """Return ``(quotient, remainder)`` for each value.

    A divisor of 0 raises ZeroDivisionError.
    """
    raise NotImplementedError
