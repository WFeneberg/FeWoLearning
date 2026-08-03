"""Exercise 068 — Comprehensions versus generator expressions (reference solution)."""

from typing import Any, Callable, Iterable, Iterator


def squares_list(values: Iterable[int]) -> list[int]:
    return [value * value for value in values]


def squares_lazy(values: Iterable[int]) -> Iterator[int]:
    # A generator expression: nothing runs until something pulls from it, which is
    # what makes an infinite source survivable.
    return (value * value for value in values)


def first_match(values: Iterable[int], predicate: Callable[[int], bool]) -> int | None:
    # next() over a generator stops at the first hit. A list comprehension would have
    # evaluated the predicate for every item before picking [0].
    return next((value for value in values if predicate(value)), None)


def any_match(values: Iterable[int], predicate: Callable[[int], bool]) -> bool:
    return any(predicate(value) for value in values)


def count_evaluations(values: list[int], predicate: Callable[[int], bool]) -> tuple[bool, int]:
    evaluations = 0

    def counted(value: int) -> bool:
        nonlocal evaluations
        evaluations += 1
        return predicate(value)

    # any() over a generator stops pulling at the first True, so `evaluations` ends up
    # smaller than len(values) when there is an early match.
    found = any(counted(value) for value in values)
    return found, evaluations


def sum_lazily(values: Iterable[int]) -> int:
    return sum(value for value in values)


def is_single_use(values: list[int]) -> tuple[list[int], list[int]]:
    generator = (value for value in values)
    first = list(generator)
    # Exhausted: a second traversal yields nothing, unlike a list.
    second = list(generator)
    return first, second


def needs_a_list(values: Iterable[int]) -> tuple[int, int]:
    # len() and max() each need a full pass, and a one-shot iterator only has one —
    # so materialise once instead of iterating a spent generator.
    items = list(values)
    return len(items), max(items)
