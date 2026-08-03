"""Exercise 058 — functools caching (reference solution)."""

import functools
from typing import Any, Callable


def make_counted_fib() -> tuple[Callable[[int], int], Callable[[], int]]:
    calls = 0

    @functools.cache
    def fib(n: int) -> int:
        nonlocal calls
        calls += 1
        if n < 2:
            return n
        return fib(n - 1) + fib(n - 2)

    # A fresh cache per factory call, so two fibs never share memoised results.
    return fib, lambda: calls


def make_bounded(maxsize: int) -> Callable[[int], int]:
    if maxsize < 1:
        raise ValueError("make_bounded() maxsize must be at least 1")

    @functools.lru_cache(maxsize=maxsize)
    def identity(n: int) -> int:
        return n

    return identity


def cache_stats(func: Any) -> tuple[int, int, int]:
    info = func.cache_info()
    return info.hits, info.misses, info.currsize


def normalise_for_cache(values: list[int]) -> tuple[int, ...]:
    # A list is unhashable, so it can never be a cache key; a tuple can.
    return tuple(values)


class Dataset:
    def __init__(self, values: list[int]) -> None:
        self.values = values
        self.compute_count = 0

    @functools.cached_property
    def total(self) -> int:
        # cached_property writes the result into the instance __dict__, so the
        # value dies with the object — unlike a module-level cache, which would
        # keep every instance alive.
        self.compute_count += 1
        return sum(self.values)


def memoise_with_stats(func: Callable[..., Any]) -> Callable[..., Any]:
    cache: dict[tuple[Any, ...], Any] = {}

    @functools.wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        if kwargs:
            # Ignoring keywords would hand back a value computed with different
            # ones, so refuse instead of answering wrongly.
            raise TypeError("memoise_with_stats() does not support keyword arguments")
        if args in cache:
            wrapper.hits += 1  # type: ignore[attr-defined]
            return cache[args]
        wrapper.misses += 1  # type: ignore[attr-defined]
        result = func(*args)
        cache[args] = result
        return result

    wrapper.hits = 0  # type: ignore[attr-defined]
    wrapper.misses = 0  # type: ignore[attr-defined]
    return wrapper
