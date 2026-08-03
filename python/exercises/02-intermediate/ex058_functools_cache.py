"""Exercise 058 — functools caching (intermediate).

Goal:   Memoise expensive calls, and know what the cache cannot do.
Drills: @cache vs @lru_cache(maxsize=…), cache_info/cache_clear, why arguments must be
        hashable, cached_property, and why caching a function with side effects lies.
Passes: when `pytest exercises/02-intermediate/test_ex058_functools_cache.py` is green.
"""

from typing import Any, Callable


def make_counted_fib() -> tuple[Callable[[int], int], Callable[[], int]]:
    """Return ``(fib, call_count)`` where `fib` is memoised.

    `fib(0)` is 0, `fib(1)` is 1. Without memoisation the naive recursion calls itself
    exponentially often; with it, computing fib(30) must take at most 31 distinct
    calls. `call_count` returns how many times the **underlying** body ran.
    """
    raise NotImplementedError


def make_bounded(maxsize: int) -> Callable[[int], int]:
    """Return an lru_cache'd identity function with the given `maxsize`.

    Expose the cache statistics as ``func.cache_info()`` — that comes free with the
    decorator. A `maxsize` below 1 raises ValueError.
    """
    raise NotImplementedError


def cache_stats(func: Any) -> tuple[int, int, int]:
    """Return ``(hits, misses, currsize)`` from a cached function's ``cache_info()``."""
    raise NotImplementedError


def normalise_for_cache(values: list[int]) -> tuple[int, ...]:
    """Convert a list into something hashable, so it can be a cache key.

    A list is unhashable, so a cached function cannot take one directly — converting
    at the boundary is the usual fix.
    """
    raise NotImplementedError


class Dataset:
    """Holds numbers and exposes an expensive aggregate as a ``cached_property``.

    ``total`` must be computed only once per instance; ``compute_count`` records how
    often the computation actually ran. Unlike ``@property`` plus ``@cache``, a
    cached_property stores the value on the instance, so it does not keep the object
    alive in a module-level cache.
    """

    def __init__(self, values: list[int]) -> None:
        raise NotImplementedError

    @property
    def total(self) -> int:
        """The sum of the values, computed at most once."""
        raise NotImplementedError


def memoise_with_stats(func: Callable[..., Any]) -> Callable[..., Any]:
    """Hand-rolled memoisation exposing ``wrapper.hits`` and ``wrapper.misses``.

    Writing it once shows what the decorators do: a dict keyed by the arguments. Only
    positional arguments are supported; a keyword call raises TypeError, because
    silently ignoring keywords would return wrong answers.
    """
    raise NotImplementedError
