"""Exercise 054 — itertools slicing and chaining (intermediate).

Goal:   Cut, join and window iterators without turning them into lists.
Drills: chain / chain.from_iterable, islice, takewhile vs dropwhile, tee,
        pairwise, and the fact that islice consumes what it skips.
Passes: when `pytest exercises/02-intermediate/test_ex054_itertools_chain_islice.py` is green.
"""

from typing import Any, Callable, Iterable, Iterator


def join(*iterables: Iterable[Any]) -> Iterator[Any]:
    """Chain the iterables into one lazy stream."""
    raise NotImplementedError


def flatten_one(nested: Iterable[Iterable[Any]]) -> Iterator[Any]:
    """Flatten one level, using ``chain.from_iterable``.

    Unlike ``chain(*nested)`` this does not need the outer iterable up front, so it
    works on an infinite outer stream too.
    """
    raise NotImplementedError


def window(values: Iterable[Any], start: int, stop: int) -> list[Any]:
    """Return items ``[start:stop)`` of the iterator.

    Note that reaching `start` means consuming everything before it — an iterator has
    no random access.
    """
    raise NotImplementedError


def first_n(values: Iterable[Any], count: int) -> list[Any]:
    """Return the first `count` items. A negative count yields []."""
    raise NotImplementedError


def every_nth(values: Iterable[Any], step: int) -> list[Any]:
    """Return items at index 0, step, 2*step, …

    A `step` below 1 raises ValueError.
    """
    raise NotImplementedError


def while_true(values: Iterable[Any], predicate: Callable[[Any], bool]) -> list[Any]:
    """Return the leading items satisfying `predicate`, stopping at the first that
    does not — even if later items would satisfy it again."""
    raise NotImplementedError


def after_false(values: Iterable[Any], predicate: Callable[[Any], bool]) -> list[Any]:
    """Skip the leading items satisfying `predicate`, then return **everything** else.

    The mirror image of `while_true`: once dropping stops, nothing else is filtered.
    """
    raise NotImplementedError


def pairs(values: Iterable[Any]) -> list[tuple[Any, Any]]:
    """Return overlapping adjacent pairs.

    ``pairs([1, 2, 3])`` -> ``[(1, 2), (2, 3)]``. Fewer than two items yields [].
    """
    raise NotImplementedError


def duplicate(values: Iterable[Any]) -> tuple[list[Any], list[Any]]:
    """Return two independent full copies of a one-shot iterator, using ``tee``.

    Both lists must contain everything, which a plain second pass over an exhausted
    iterator could not deliver.
    """
    raise NotImplementedError
