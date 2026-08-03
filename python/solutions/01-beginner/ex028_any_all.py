"""Exercise 028 — any and all (reference solution)."""

from typing import Callable, Iterable


def all_positive(numbers: Iterable[int]) -> bool:
    return all(n > 0 for n in numbers)


def any_negative(numbers: Iterable[int]) -> bool:
    return any(n < 0 for n in numbers)


def none_match(values: Iterable[int], predicate: Callable[[int], bool]) -> bool:
    return not any(predicate(value) for value in values)


def all_unique(values: list[str]) -> bool:
    return len(set(values)) == len(values)


def has_digit(text: str) -> bool:
    return any(char.isdigit() for char in text)


def first_failing(values: list[int], predicate: Callable[[int], bool]) -> int | None:
    # next() over a generator stops at the first hit; the default covers "none".
    return next((value for value in values if not predicate(value)), None)


def count_consumed(numbers: list[int]) -> tuple[bool, int]:
    examined = 0

    def tracked() -> Iterable[bool]:
        nonlocal examined
        for number in numbers:
            examined += 1
            yield number > 10

    # any() stops pulling from the generator as soon as one item is truthy, so
    # `examined` ends up smaller than len(numbers) when there is an early match.
    found = any(tracked())
    return found, examined
