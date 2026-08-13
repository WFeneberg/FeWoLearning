"""Exercise 100 — a tiny property-based testing engine (reference solution)."""

import random
from typing import Any, Callable, Iterator, TypeVar

T = TypeVar("T")

Generator = Callable[[random.Random], T]


class FalsifiedError(Exception):
    def __init__(self, original: Any, shrunk: Any) -> None:
        self.original = original
        self.shrunk = shrunk
        super().__init__(f"property falsified: shrunk example = {shrunk!r} (original: {original!r})")


def ints(min_value: int = -100, max_value: int = 100) -> Generator:
    def _gen(rng: random.Random) -> int:
        return rng.randint(min_value, max_value)

    return _gen


def lists(element_gen: Generator, max_size: int = 10) -> Generator:
    def _gen(rng: random.Random) -> list[Any]:
        size = rng.randint(0, max_size)
        return [element_gen(rng) for _ in range(size)]

    return _gen


def shrink_int(value: int) -> Iterator[int]:
    if value == 0:
        return
    yield 0
    current = value
    while abs(current) > 1:
        current //= 2
        yield current
    yield value - 1 if value > 0 else value + 1


def shrink_list(value: list[Any]) -> Iterator[list[Any]]:
    if not value:
        return
    yield []
    midpoint = len(value) // 2
    yield value[:midpoint]
    yield value[midpoint:]
    for i in range(len(value)):
        yield value[:i] + value[i + 1 :]


def shrink(value: Any) -> Iterator[Any]:
    if isinstance(value, list):
        yield from shrink_list(value)
    elif isinstance(value, int):
        yield from shrink_int(value)


def for_all(
    generator: Generator,
    property_fn: Callable[[Any], bool],
    *,
    examples: int = 100,
    seed: int = 0,
) -> None:
    rng = random.Random(seed)
    for _ in range(examples):
        value = generator(rng)
        if property_fn(value):
            continue

        original = value
        current = value
        improved = True
        while improved:
            improved = False
            for candidate in shrink(current):
                if not property_fn(candidate):
                    current = candidate
                    improved = True
                    break

        raise FalsifiedError(original, current)
