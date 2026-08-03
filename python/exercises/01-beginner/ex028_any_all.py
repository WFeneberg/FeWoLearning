"""Exercise 028 — any and all (beginner).

Goal:   Ask yes/no questions about a whole collection in one expression.
Drills: any/all over generator expressions, their empty-input answers,
        short-circuiting, negating a quantifier, next() to find the offender.
Passes: when `pytest exercises/01-beginner/test_ex028_any_all.py` is green.
"""

from typing import Callable, Iterable


def all_positive(numbers: Iterable[int]) -> bool:
    """Report whether every number is greater than zero.

    An empty input is True — ``all()`` is vacuously true, and that is deliberate.
    """
    raise NotImplementedError


def any_negative(numbers: Iterable[int]) -> bool:
    """Report whether at least one number is below zero.

    An empty input is False — ``any()`` needs a witness.
    """
    raise NotImplementedError


def none_match(values: Iterable[int], predicate: Callable[[int], bool]) -> bool:
    """Report whether *no* value satisfies `predicate`.

    "None match" is ``not any(...)``, not ``all(not ...)`` — both work here, but the
    first reads as the question being asked.
    """
    raise NotImplementedError


def all_unique(values: list[str]) -> bool:
    """Report whether every value occurs exactly once."""
    raise NotImplementedError


def has_digit(text: str) -> bool:
    """Report whether `text` contains at least one decimal digit."""
    raise NotImplementedError


def first_failing(values: list[int], predicate: Callable[[int], bool]) -> int | None:
    """Return the first value that does **not** satisfy `predicate`, else None.

    Where `all()` only says whether something failed, this says which one — use
    ``next()`` with a default over a generator expression.
    """
    raise NotImplementedError


def count_consumed(numbers: list[int]) -> tuple[bool, int]:
    """Return ``(any(n > 10), how_many_items_were_examined)``.

    Proves that ``any()`` short-circuits: it stops at the first match instead of
    walking the whole list.
    """
    raise NotImplementedError
