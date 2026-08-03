"""Exercise 024 — collections.Counter (beginner).

Goal:   Count things with Counter instead of hand-rolled dict bookkeeping.
Drills: Counter construction, most_common and its tie behaviour, arithmetic on
        Counters, elements(), subtract vs -.
Passes: when `pytest exercises/01-beginner/test_ex024_counter_most_common.py` is green.
"""

from collections import Counter


def char_counts(text: str) -> dict[str, int]:
    """Count characters, as a plain dict.

    Whitespace counts like any other character.
    """
    raise NotImplementedError


def top_n(values: list[str], n: int) -> list[tuple[str, int]]:
    """Return the `n` most common values as ``(value, count)``, most common first.

    Ties keep first-encountered order, which is what Counter already guarantees.
    An `n` of 0 or less yields an empty list.
    """
    raise NotImplementedError


def most_common_value(values: list[str]) -> str | None:
    """Return the single most common value, or None for an empty input."""
    raise NotImplementedError


def duplicates(values: list[str]) -> list[str]:
    """Return the values occurring more than once, sorted alphabetically."""
    raise NotImplementedError


def merge_counts(a: dict[str, int], b: dict[str, int]) -> dict[str, int]:
    """Add the two count mappings together."""
    raise NotImplementedError


def difference(a: dict[str, int], b: dict[str, int]) -> dict[str, int]:
    """Subtract `b` from `a`, dropping entries that reach zero or below.

    That is what ``Counter.__sub__`` does — unlike ``Counter.subtract``, which keeps
    zero and negative counts.
    """
    raise NotImplementedError


def expand(counts: dict[str, int]) -> list[str]:
    """Turn counts back into a flat list, each key repeated `count` times.

    Keys keep their insertion order; non-positive counts contribute nothing.
    """
    raise NotImplementedError
