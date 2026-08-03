"""Exercise 054 — itertools slicing and chaining (reference solution)."""

import itertools
from typing import Any, Callable, Iterable, Iterator


def join(*iterables: Iterable[Any]) -> Iterator[Any]:
    return itertools.chain(*iterables)


def flatten_one(nested: Iterable[Iterable[Any]]) -> Iterator[Any]:
    # from_iterable pulls the outer iterable lazily; chain(*nested) would have to
    # unpack it fully first, which an infinite outer stream never allows.
    return itertools.chain.from_iterable(nested)


def window(values: Iterable[Any], start: int, stop: int) -> list[Any]:
    # islice discards the skipped items rather than seeking — an iterator has no
    # random access.
    return list(itertools.islice(values, start, stop))


def first_n(values: Iterable[Any], count: int) -> list[Any]:
    if count <= 0:
        # islice rejects a negative count with ValueError.
        return []
    return list(itertools.islice(values, count))


def every_nth(values: Iterable[Any], step: int) -> list[Any]:
    if step < 1:
        raise ValueError("every_nth() step must be at least 1")
    return list(itertools.islice(values, 0, None, step))


def while_true(values: Iterable[Any], predicate: Callable[[Any], bool]) -> list[Any]:
    return list(itertools.takewhile(predicate, values))


def after_false(values: Iterable[Any], predicate: Callable[[Any], bool]) -> list[Any]:
    # dropwhile only drops the *leading* run; everything from the first failure on is
    # passed through unfiltered.
    return list(itertools.dropwhile(predicate, values))


def pairs(values: Iterable[Any]) -> list[tuple[Any, Any]]:
    return list(itertools.pairwise(values))


def duplicate(values: Iterable[Any]) -> tuple[list[Any], list[Any]]:
    # tee buffers what one branch has seen and the other has not, so both get the
    # full sequence from a single-pass source.
    a, b = itertools.tee(values, 2)
    return list(a), list(b)
