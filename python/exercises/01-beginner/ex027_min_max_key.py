"""Exercise 027 — min and max with a key (beginner).

Goal:   Pick extremes by a computed property, and survive the empty case.
Drills: min/max with key=, the `default=` argument, tie behaviour (first wins),
        returning the item rather than the key, min/max over dict items.
Passes: when `pytest exercises/01-beginner/test_ex027_min_max_key.py` is green.
"""


def longest(words: list[str]) -> str | None:
    """Return the longest word, or None when there are none.

    On a tie the **first** one wins — that is what max() already does.
    """
    raise NotImplementedError


def shortest(words: list[str]) -> str | None:
    """Return the shortest word, or None when there are none. First wins on a tie."""
    raise NotImplementedError


def closest_to(numbers: list[int], target: int) -> int | None:
    """Return the number nearest to `target`, or None for an empty list.

    A tie (equally far above and below) resolves to the first one seen.
    """
    raise NotImplementedError


def highest_scorer(scores: dict[str, int]) -> str | None:
    """Return the name with the highest score, or None for an empty mapping.

    Iterate the items, not just the keys, so the score can be the key function.
    """
    raise NotImplementedError


def largest_by_abs(numbers: list[int], default: int = 0) -> int:
    """Return the number furthest from zero, or `default` when there are none.

    Uses ``max(..., default=...)`` rather than a length check.
    """
    raise NotImplementedError


def bounds(numbers: list[int]) -> tuple[int, int] | None:
    """Return ``(min, max)``, or None for an empty list."""
    raise NotImplementedError


def longest_line(text: str) -> str:
    """Return the longest line of `text`, or ``""`` when there are none.

    Lines are split on newlines; a trailing newline does not add an empty line.
    """
    raise NotImplementedError
