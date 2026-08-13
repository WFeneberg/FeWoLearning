"""Exercise 089 — memory-bounded streaming pipelines (reference solution)."""

from collections import deque
from typing import Callable, Iterable, Iterator, TypeVar

T = TypeVar("T")
U = TypeVar("U")


def pipe(source: Iterable[T], *stages: Callable[[Iterable[T]], Iterator[T]]) -> Iterator[T]:
    stream: Iterable[T] = source
    for stage in stages:
        stream = stage(stream)
    return iter(stream)


def filter_stage(predicate: Callable[[T], bool]) -> Callable[[Iterable[T]], Iterator[T]]:
    def _stage(source: Iterable[T]) -> Iterator[T]:
        for item in source:
            if predicate(item):
                yield item

    return _stage


def map_stage(func: Callable[[T], U]) -> Callable[[Iterable[T]], Iterator[U]]:
    def _stage(source: Iterable[T]) -> Iterator[U]:
        for item in source:
            yield func(item)

    return _stage


def moving_average(source: Iterable[float], window: int) -> Iterator[float]:
    if window <= 0:
        raise ValueError(f"window must be positive, got {window}")

    buffer: deque[float] = deque(maxlen=window)
    total = 0.0
    for value in source:
        if len(buffer) == window:
            total -= buffer[0]
        buffer.append(value)
        total += value
        yield total / len(buffer)


def take(source: Iterable[T], n: int) -> list[T]:
    result: list[T] = []
    iterator = iter(source)
    for _ in range(n):
        try:
            result.append(next(iterator))
        except StopIteration:
            break
    return result
