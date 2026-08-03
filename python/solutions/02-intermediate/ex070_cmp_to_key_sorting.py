"""Exercise 070 — Comparator-based sorting (reference solution)."""

import functools
from typing import Any, Callable

Record = dict[str, Any]


def compare_lengths(a: str, b: str) -> int:
    # Only the sign matters, so the difference is a valid comparator.
    return len(a) - len(b)


def sort_by_comparator(values: list[str], comparator: Callable[[str, str], int]) -> list[str]:
    # Python 3 removed sorted(cmp=…); cmp_to_key builds the key object sort wants.
    return sorted(values, key=functools.cmp_to_key(comparator))


def sort_largest_concatenation(numbers: list[int]) -> str:
    def compare(a: str, b: str) -> int:
        # Whether a belongs before b depends on comparing both concatenations — a
        # property of the *pair*, which no key function can capture.
        if a + b == b + a:
            return 0
        return -1 if a + b > b + a else 1

    ordered = sorted((str(n) for n in numbers), key=functools.cmp_to_key(compare))
    return "".join(ordered)


def sort_version_strings(versions: list[str]) -> list[str]:
    # This one *is* expressible as a key — a tuple of ints — and that is the better
    # tool, so use it rather than a comparator for its own sake.
    return sorted(versions, key=lambda v: tuple(int(part) for part in v.split(".")))


def sort_records(records: list[Record]) -> list[Record]:
    def compare(a: Record, b: Record) -> int:
        # Descending priority: b before a.
        if a["priority"] != b["priority"]:
            return b["priority"] - a["priority"]
        # Ascending name.
        if a["name"] == b["name"]:
            return 0
        return -1 if a["name"] < b["name"] else 1

    return sorted(records, key=functools.cmp_to_key(compare))


def _sign(value: int) -> int:
    return (value > 0) - (value < 0)


def is_valid_comparator(comparator: Callable[[Any, Any], int], samples: list[Any]) -> bool:
    for a in samples:
        for b in samples:
            if _sign(comparator(a, b)) != -_sign(comparator(b, a)):
                return False
    return True
