"""Exercise 010 — Slicing (beginner).

Goal:   Reach into sequences with slices instead of index arithmetic.
Drills: slice syntax, negative indices and steps, out-of-range slices,
        shallow copies, slice assignment.
Passes: when `pytest exercises/01-beginner/test_ex010_slicing.py` is green.
"""


def first_n(values: list[int], n: int) -> list[int]:
    """Return the first `n` items. Fewer than `n` items is fine, not an error.

    A negative `n` returns an empty list.
    """
    raise NotImplementedError


def last_n(values: list[int], n: int) -> list[int]:
    """Return the last `n` items.

    Careful: ``values[-n:]`` returns the *whole* list when n is 0, so handle that.
    """
    raise NotImplementedError


def middle(values: list[int]) -> list[int]:
    """Return everything except the first and last item.

    Lists of two items or fewer yield an empty list.
    """
    raise NotImplementedError


def every_other(values: list[int]) -> list[int]:
    """Return items at index 0, 2, 4, … using a step."""
    raise NotImplementedError


def reversed_copy(values: list[int]) -> list[int]:
    """Return a reversed **copy**, leaving the input untouched, via a step of -1."""
    raise NotImplementedError


def shallow_copy(values: list[int]) -> list[int]:
    """Return a copy that is equal to but not the same object as `values`."""
    raise NotImplementedError


def replace_slice(values: list[int], start: int, stop: int, replacement: list[int]) -> list[int]:
    """Replace ``values[start:stop]`` with `replacement` **in place** and return it.

    The replacement may be a different length than the slice it replaces.
    """
    raise NotImplementedError


def chunk(values: list[int], size: int) -> list[list[int]]:
    """Split into consecutive chunks of `size`; the last one may be shorter.

    A size of 0 or less raises ValueError.
    """
    raise NotImplementedError
