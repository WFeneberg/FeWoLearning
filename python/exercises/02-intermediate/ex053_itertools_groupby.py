"""Exercise 053 — itertools.groupby (intermediate).

Goal:   Group *consecutive* runs, and know why that is not the same as SQL GROUP BY.
Drills: itertools.groupby, the mandatory sort-before-group, group iterators being
        consumed as you advance, run-length encoding, grouping by a key function.
Passes: when `pytest exercises/02-intermediate/test_ex053_itertools_groupby.py` is green.
"""

from typing import Any, Callable, Iterable


def runs(values: Iterable[Any]) -> list[tuple[Any, int]]:
    """Return ``(value, run_length)`` for each **consecutive** run.

    ``runs("aabba")`` -> ``[("a", 2), ("b", 2), ("a", 1)]`` — the trailing "a" is a
    separate run, which is exactly how groupby differs from a real GROUP BY.
    """
    raise NotImplementedError


def group_sorted(values: Iterable[Any], key: Callable[[Any], Any]) -> dict[Any, list[Any]]:
    """Group by `key` the way a database would: every equal key in one bucket.

    groupby only sees consecutive runs, so the input must be **sorted by the same
    key** first. Forgetting that sort is the classic groupby bug.
    """
    raise NotImplementedError


def group_lengths(words: Iterable[str]) -> dict[int, list[str]]:
    """Group words by length, each bucket in the input's original order."""
    raise NotImplementedError


def compress(text: str) -> str:
    """Run-length encode: ``"aaabb"`` -> ``"a3b2"``.

    A run of one still gets its count: ``"abc"`` -> ``"a1b1c1"``.
    """
    raise NotImplementedError


def longest_run(values: Iterable[Any]) -> tuple[Any, int] | None:
    """Return the longest consecutive run as ``(value, length)``.

    Ties resolve to the first one. An empty input returns None.
    """
    raise NotImplementedError


def first_of_each_run(values: Iterable[Any]) -> list[Any]:
    """Collapse consecutive duplicates, keeping one of each run.

    ``[1, 1, 2, 2, 1]`` -> ``[1, 2, 1]``.
    """
    raise NotImplementedError


def count_groups(values: Iterable[Any], key: Callable[[Any], Any]) -> int:
    """Count the consecutive groups `key` produces, without materialising them.

    Careful: a group's iterator is invalidated as soon as you advance to the next
    group, so counting must not depend on the group contents.
    """
    raise NotImplementedError
