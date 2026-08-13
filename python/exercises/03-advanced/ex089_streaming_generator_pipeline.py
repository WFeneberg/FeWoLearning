"""Exercise 089 — memory-bounded streaming pipelines (advanced).

Goal:   Process a source that may be far too large (or literally infinite) to hold
        in memory, by chaining generator stages that each pull one item at a time
        instead of materializing a list between steps.
Drills: generator functions as composable pipeline stages, `collections.deque(maxlen=…)`
        for O(window) memory instead of O(n), and proving laziness — that a pipeline
        in front of an infinite source only ever pulls as many items as the
        consumer actually asks for.
Passes: when `pytest exercises/03-advanced/test_ex089_streaming_generator_pipeline.py` is green.

Note:   none of this should call `list(...)` on `source` anywhere — doing so would
        defeat the entire point, and the laziness test below will catch it.
"""

from collections import deque
from typing import Callable, Iterable, Iterator, TypeVar

T = TypeVar("T")
U = TypeVar("U")


def pipe(source: Iterable[T], *stages: Callable[[Iterable[T]], Iterator[T]]) -> Iterator[T]:
    """Thread `source` through `stages` in order, each stage consuming the previous
    one lazily. With no stages, just return an iterator over `source` unchanged."""
    raise NotImplementedError


def filter_stage(predicate: Callable[[T], bool]) -> Callable[[Iterable[T]], Iterator[T]]:
    """Return a pipeline stage that yields only the items matching `predicate`."""
    raise NotImplementedError


def map_stage(func: Callable[[T], U]) -> Callable[[Iterable[T]], Iterator[U]]:
    """Return a pipeline stage that yields `func(item)` for every item."""
    raise NotImplementedError


def moving_average(source: Iterable[float], window: int) -> Iterator[float]:
    """Yield the running average over the last `window` values seen so far.

    Memory use must stay O(window) no matter how long `source` runs — keep only the
    values currently in the window (a `deque(maxlen=window)` is built for this) and
    a running total, rather than re-summing the window on every item. `window <= 0`
    raises ValueError. Before `window` items have arrived, average over however many
    there are so far.
    """
    raise NotImplementedError


def take(source: Iterable[T], n: int) -> list[T]:
    """Materialize only the first `n` items of `source` (fewer if it is shorter)."""
    raise NotImplementedError
