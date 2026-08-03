"""Exercise 008 — List comprehensions (beginner).

Goal:   Replace explicit append loops with comprehensions.
Drills: comprehensions with filters, conditional expressions inside them,
        nested iteration, order of the for/if clauses.
Passes: when `pytest exercises/01-beginner/test_ex008_list_comprehension.py` is green.
"""


def squares(numbers: list[int]) -> list[int]:
    """Return each number squared, in the same order."""
    raise NotImplementedError


def even_squares(numbers: list[int]) -> list[int]:
    """Return the squares of only the even numbers."""
    raise NotImplementedError


def clamp_all(numbers: list[int], ceiling: int) -> list[int]:
    """Return each number, replaced by `ceiling` when it exceeds it.

    This needs a conditional *expression* in the output part, not a filter:
    nothing is dropped. ``clamp_all([1, 9], 5)`` -> ``[1, 5]``.
    """
    raise NotImplementedError


def pairs(xs: list[int], ys: list[str]) -> list[tuple[int, str]]:
    """Return every ``(x, y)`` combination, x varying slowest.

    ``pairs([1, 2], ["a"])`` -> ``[(1, "a"), (2, "a")]``.
    """
    raise NotImplementedError


def flatten_and_filter(rows: list[list[int]], minimum: int) -> list[int]:
    """Flatten one level and keep only values at or above `minimum`."""
    raise NotImplementedError


def word_lengths(sentence: str) -> list[tuple[str, int]]:
    """Return ``(word, length)`` for each whitespace-separated word."""
    raise NotImplementedError


def diagonal(matrix: list[list[int]]) -> list[int]:
    """Return the main diagonal of a square matrix, using the index in the loop.

    ``diagonal([[1, 2], [3, 4]])`` -> ``[1, 4]``. An empty matrix yields ``[]``.
    """
    raise NotImplementedError
