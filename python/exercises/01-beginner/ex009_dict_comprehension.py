"""Exercise 009 — Dict and set comprehensions (beginner).

Goal:   Build dicts and sets directly instead of filling them in a loop.
Drills: dict/set comprehensions, inverting a mapping, filtering by value,
        zip inside a comprehension, last-wins on duplicate keys.
Passes: when `pytest exercises/01-beginner/test_ex009_dict_comprehension.py` is green.
"""


def lengths_by_word(words: list[str]) -> dict[str, int]:
    """Map each word to its length. A repeated word simply appears once."""
    raise NotImplementedError


def invert(mapping: dict[str, int]) -> dict[int, str]:
    """Swap keys and values.

    When two keys share a value the **last** one wins, matching what a plain
    assignment loop would do.
    """
    raise NotImplementedError


def filter_by_value(scores: dict[str, int], minimum: int) -> dict[str, int]:
    """Keep only the entries whose value is at or above `minimum`."""
    raise NotImplementedError


def zip_to_dict(keys: list[str], values: list[int]) -> dict[str, int]:
    """Pair up the two lists. Extra items in the longer list are dropped."""
    raise NotImplementedError


def upper_keys(mapping: dict[str, int]) -> dict[str, int]:
    """Return the same mapping with uppercased keys."""
    raise NotImplementedError


def unique_lengths(words: list[str]) -> set[int]:
    """Return the distinct word lengths, as a set comprehension."""
    raise NotImplementedError


def index_of_each(values: list[str]) -> dict[str, int]:
    """Map each value to the index of its **first** occurrence.

    ``index_of_each(["a", "b", "a"])`` -> ``{"a": 0, "b": 1}``. Mind that a naive
    comprehension gives last-wins, so this one needs care.
    """
    raise NotImplementedError
