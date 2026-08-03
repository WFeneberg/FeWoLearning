"""Exercise 056 — functools.reduce (intermediate).

Goal:   Fold a sequence into one value, and know when a built-in beats reduce.
Drills: reduce with and without an initial value, why the initial value is what makes
        the empty case work, folding into a non-scalar accumulator, left-associativity.
Passes: when `pytest exercises/02-intermediate/test_ex056_functools_reduce.py` is green.
"""

from typing import Any, Callable, Iterable


def product(values: Iterable[int]) -> int:
    """Multiply every value together. An empty input yields 1, the identity."""
    raise NotImplementedError


def total(values: Iterable[int], start: int = 0) -> int:
    """Sum with an explicit starting value, via reduce.

    (In real code ``sum(values, start)`` is better; this is about the fold.)
    """
    raise NotImplementedError


def longest(words: Iterable[str]) -> str:
    """Return the longest word, ties going to the first, ``""`` when there are none.

    Doing this with reduce means the accumulator carries the best-so-far.
    """
    raise NotImplementedError


def merge_dicts(mappings: Iterable[dict[str, int]]) -> dict[str, int]:
    """Merge left to right, later values winning. The accumulator is a dict.

    Must not modify any input.
    """
    raise NotImplementedError


def compose(*funcs: Callable[[Any], Any]) -> Callable[[Any], Any]:
    """Compose left to right: ``compose(f, g)(x)`` is ``g(f(x))``.

    With no functions, return the identity.
    """
    raise NotImplementedError


def flatten(nested: Iterable[list[Any]]) -> list[Any]:
    """Concatenate lists with reduce.

    Worth knowing: this is O(n²) because each step builds a new list. A comprehension
    or ``chain.from_iterable`` is the right tool; the exercise is the fold.
    """
    raise NotImplementedError


def running(values: Iterable[int], func: Callable[[int, int], int]) -> list[int]:
    """Return every intermediate accumulator value, not just the final one.

    ``running([1, 2, 3], add)`` -> ``[1, 3, 6]``. That is ``accumulate``, not
    ``reduce`` — reduce discards the intermediates. An empty input yields [].
    """
    raise NotImplementedError


def fold_right(values: list[int], func: Callable[[int, int], int], initial: int) -> int:
    """Fold from the right instead of the left.

    ``fold_right([1, 2, 3], lambda a, b: a - b, 0)`` is ``1 - (2 - (3 - 0))`` = 2,
    whereas folding left gives ``((0 - 1) - 2) - 3`` = -6. reduce only folds left, so
    the input has to be reversed and the arguments swapped.
    """
    raise NotImplementedError
