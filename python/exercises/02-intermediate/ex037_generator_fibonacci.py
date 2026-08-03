"""Exercise 037 — Generators (intermediate).

Goal:   Produce values lazily with `yield` instead of building a list up front.
Drills: generator functions, infinite generators plus islice, generator state,
        why a generator is single-use, StopIteration via next().
Passes: when `pytest exercises/02-intermediate/test_ex037_generator_fibonacci.py` is green.
"""

from typing import Iterator


def fibonacci() -> Iterator[int]:
    """Yield the Fibonacci numbers **forever**, starting 0, 1, 1, 2, 3, 5, …

    This must not terminate on its own — callers bound it with islice or a break.
    Because it is infinite, it also must not build any list internally.
    """
    raise NotImplementedError


def take(iterator: Iterator[int], count: int) -> list[int]:
    """Return the next `count` values from `iterator`.

    Fewer than `count` remaining is fine — return what there is. A negative `count`
    yields [].
    """
    raise NotImplementedError


def fib_up_to(limit: int) -> Iterator[int]:
    """Yield Fibonacci numbers while they are ``<= limit``.

    A negative limit yields nothing.
    """
    raise NotImplementedError


def count_from(start: int, step: int = 1) -> Iterator[int]:
    """Yield `start`, `start + step`, `start + 2 * step`, … forever."""
    raise NotImplementedError


def running_max(values: Iterator[int]) -> Iterator[int]:
    """Yield the largest value seen so far, one output per input.

    ``running_max(iter([1, 3, 2]))`` yields 1, 3, 3.
    """
    raise NotImplementedError


def is_exhausted(iterator: Iterator[int]) -> bool:
    """Report whether `iterator` has nothing left.

    Careful: finding out costs one item. Use ``next(iterator, sentinel)`` and accept
    that a non-empty iterator loses its first value — that is inherent, and naming it
    is part of the exercise.
    """
    raise NotImplementedError
