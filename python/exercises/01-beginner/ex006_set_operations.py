"""Exercise 006 — Set operations (beginner).

Goal:   Answer membership and overlap questions with sets instead of loops.
Drills: set algebra (union/intersection/difference), membership, deduplication,
        frozenset, subset relations.
Passes: when `pytest exercises/01-beginner/test_ex006_set_operations.py` is green.
"""


def dedupe_keep_order(values: list[str]) -> list[str]:
    """Remove duplicates while preserving first-seen order.

    A plain ``set(values)`` would lose the order, so track what has been seen.
    """
    raise NotImplementedError


def common_elements(a: list[int], b: list[int]) -> set[int]:
    """Return the values present in both lists."""
    raise NotImplementedError


def only_in_first(a: list[int], b: list[int]) -> set[int]:
    """Return the values present in `a` but not in `b`."""
    raise NotImplementedError


def symmetric_difference(a: list[int], b: list[int]) -> set[int]:
    """Return the values present in exactly one of the two lists."""
    raise NotImplementedError


def is_subset(small: list[int], large: list[int]) -> bool:
    """Report whether every value of `small` also occurs in `large`.

    An empty `small` is a subset of anything.
    """
    raise NotImplementedError


def has_duplicates(values: list[int]) -> bool:
    """Report whether any value occurs more than once."""
    raise NotImplementedError


def group_key(tags: list[str]) -> frozenset[str]:
    """Return a hashable, order-independent key for a tag list.

    Two lists with the same tags in a different order must produce equal keys, and
    the result must be usable as a dict key.
    """
    raise NotImplementedError
