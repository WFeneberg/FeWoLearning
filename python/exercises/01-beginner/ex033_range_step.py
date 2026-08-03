"""Exercise 033 — range and reversed (beginner).

Goal:   Generate numeric sequences with range instead of manual counters.
Drills: range(start, stop, step), the exclusive stop, negative steps, reversed(),
        range as a lazy object rather than a list, membership tests.
Passes: when `pytest exercises/01-beginner/test_ex033_range_step.py` is green.
"""


def evens_up_to(limit: int) -> list[int]:
    """Return the even numbers from 0 up to and **including** `limit` when even.

    ``evens_up_to(6)`` -> ``[0, 2, 4, 6]``. A negative limit yields [].
    """
    raise NotImplementedError


def countdown(start: int) -> list[int]:
    """Return `start` down to 1 inclusive, using a negative step.

    ``countdown(3)`` -> ``[3, 2, 1]``. A start of 0 or less yields [].
    """
    raise NotImplementedError


def every_nth(values: list[str], n: int) -> list[str]:
    """Return every `n`-th value starting at index 0, driven by a range of indices.

    An `n` of 0 or less raises ValueError.
    """
    raise NotImplementedError


def indices_reversed(values: list[str]) -> list[int]:
    """Return the valid indices of `values`, highest first, using reversed()."""
    raise NotImplementedError


def arithmetic_series(start: int, step: int, count: int) -> list[int]:
    """Return `count` terms beginning at `start`, each `step` apart.

    A negative `count` yields []. A `step` of 0 gives `count` copies of `start`.
    """
    raise NotImplementedError


def is_in_range(value: int, start: int, stop: int, step: int) -> bool:
    """Report whether `value` would appear in ``range(start, stop, step)``.

    Use the range object's own membership test, which is O(1) for ints rather than
    walking the sequence. A `step` of 0 raises ValueError.
    """
    raise NotImplementedError


def sum_multiples(limit: int, divisor: int) -> int:
    """Sum the multiples of `divisor` strictly below `limit`, starting at `divisor`.

    ``sum_multiples(10, 3)`` -> ``3 + 6 + 9 = 18``. A divisor of 0 or less raises
    ValueError.
    """
    raise NotImplementedError
