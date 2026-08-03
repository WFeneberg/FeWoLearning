"""Exercise 037 — Generators (reference solution)."""

import itertools
from typing import Iterator

_MISSING = object()


def fibonacci() -> Iterator[int]:
    current, following = 0, 1
    while True:
        yield current
        # Tuple assignment advances both without a temporary.
        current, following = following, current + following


def take(iterator: Iterator[int], count: int) -> list[int]:
    if count <= 0:
        # islice would reject a negative count with ValueError.
        return []
    return list(itertools.islice(iterator, count))


def fib_up_to(limit: int) -> Iterator[int]:
    for value in fibonacci():
        if value > limit:
            return
        yield value


def count_from(start: int, step: int = 1) -> Iterator[int]:
    value = start
    while True:
        yield value
        value += step


def running_max(values: Iterator[int]) -> Iterator[int]:
    largest: int | None = None
    for value in values:
        largest = value if largest is None else max(largest, value)
        yield largest


def is_exhausted(iterator: Iterator[int]) -> bool:
    # A sentinel default turns StopIteration into a value instead of an exception.
    # The pulled item is unavoidably lost; there is no lookahead on a bare iterator.
    return next(iterator, _MISSING) is _MISSING
