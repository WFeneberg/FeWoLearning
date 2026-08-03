"""Exercise 068 — Comprehensions versus generator expressions (intermediate).

Goal:   Choose between building a collection and streaming one.
Drills: list comprehension vs generator expression, memory behaviour, single-use
        generators, short-circuiting with any/next, and where a list is required.
Passes: when `pytest exercises/02-intermediate/test_ex068_comprehension_vs_generator.py` is green.
"""

from typing import Any, Callable, Iterable, Iterator


def squares_list(values: Iterable[int]) -> list[int]:
    """Return the squares as a **list**, because the caller needs len() and indexing."""
    raise NotImplementedError


def squares_lazy(values: Iterable[int]) -> Iterator[int]:
    """Return the squares as a **generator**, so nothing is computed until pulled."""
    raise NotImplementedError


def first_match(values: Iterable[int], predicate: Callable[[int], bool]) -> int | None:
    """Return the first matching value, or None.

    A generator expression plus ``next`` stops at the first hit; a list comprehension
    would evaluate the predicate for every item first.
    """
    raise NotImplementedError


def any_match(values: Iterable[int], predicate: Callable[[int], bool]) -> bool:
    """Report whether anything matches, short-circuiting."""
    raise NotImplementedError


def count_evaluations(values: list[int], predicate: Callable[[int], bool]) -> tuple[bool, int]:
    """Return ``(any_match_result, how_many_times_the_predicate_ran)``.

    Demonstrates the difference: with a generator the predicate stops running at the
    first match, where a list comprehension would have run it on everything.
    """
    raise NotImplementedError


def sum_lazily(values: Iterable[int]) -> int:
    """Sum without building an intermediate list.

    ``sum(x for x in values)`` streams; ``sum([x for x in values])`` allocates the
    whole list first for no benefit.
    """
    raise NotImplementedError


def is_single_use(values: list[int]) -> tuple[list[int], list[int]]:
    """Build a generator over `values`, consume it twice, return both results.

    The second pass is empty: a generator is exhausted after one traversal, where a
    list comprehension could be iterated again.
    """
    raise NotImplementedError


def needs_a_list(values: Iterable[int]) -> tuple[int, int]:
    """Return ``(length, largest)`` from a one-shot iterable.

    Both answers require a full pass, and a generator cannot be traversed twice — so
    materialise once rather than iterating a spent generator.
    """
    raise NotImplementedError
