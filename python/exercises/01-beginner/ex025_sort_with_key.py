"""Exercise 025 — Sorting with a key (beginner).

Goal:   Sort by a computed property instead of rearranging by hand.
Drills: sorted(key=...), reverse=, sort stability, sorted vs list.sort,
        keys that normalise (case-insensitive, absolute value).
Passes: when `pytest exercises/01-beginner/test_ex025_sort_with_key.py` is green.
"""


def by_length(words: list[str]) -> list[str]:
    """Sort by length, shortest first, as a **new** list.

    Words of equal length keep their original relative order — Python's sort is
    stable, so nothing extra is needed for that.
    """
    raise NotImplementedError


def by_length_desc(words: list[str]) -> list[str]:
    """Sort by length, longest first, using `reverse=True`.

    `reverse=True` keeps the sort stable: equal-length words stay in their original
    relative order. That is what makes it different from reversing the result with
    ``[::-1]``, which would flip those groups as well.
    """
    raise NotImplementedError


def case_insensitive(words: list[str]) -> list[str]:
    """Sort alphabetically ignoring case, so "Banana" sorts next to "apple"."""
    raise NotImplementedError


def by_absolute_value(numbers: list[int]) -> list[int]:
    """Sort by distance from zero, keeping the original signs."""
    raise NotImplementedError


def by_last_name(names: list[str]) -> list[str]:
    """Sort ``"First Last"`` strings by the last word.

    Names with a single word sort by that word.
    """
    raise NotImplementedError


def sort_in_place(numbers: list[int]) -> None:
    """Sort ascending **in place**, returning None like list.sort does."""
    raise NotImplementedError


def top_scores(scores: dict[str, int], n: int) -> list[str]:
    """Return the `n` names with the highest scores, highest first.

    Equal scores are broken alphabetically by name, so the result is deterministic.
    """
    raise NotImplementedError
