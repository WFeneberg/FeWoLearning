"""Exercise 039 — yield from (reference solution)."""

import itertools
from typing import Any, Iterable, Iterator


def concat(*iterables: Iterable[int]) -> Iterator[int]:
    for iterable in iterables:
        # `yield iterable` would emit the list itself; delegating emits its items.
        yield from iterable


def flatten(nested: Iterable[Any]) -> Iterator[Any]:
    for item in nested:
        # A str is iterable, so descending into it would recurse to single
        # characters — and a one-character str is still iterable, so it would never
        # bottom out. Treating str as a leaf is what makes this terminate.
        if isinstance(item, (list, tuple)):
            yield from flatten(item)
        else:
            yield item


def flatten_depth(nested: Iterable[Any], depth: int = 1) -> Iterator[Any]:
    if depth < 0:
        raise ValueError("flatten_depth() depth must not be negative")
    for item in nested:
        if depth > 0 and isinstance(item, (list, tuple)):
            yield from flatten_depth(item, depth - 1)
        else:
            yield item


def walk_tree(node: dict[str, Any]) -> Iterator[str]:
    yield node["name"]
    for child in node.get("children", []):
        yield from walk_tree(child)


def interleave(a: Iterable[int], b: Iterable[int]) -> Iterator[int]:
    # zip_longest pads the shorter side with a sentinel; filtering it out lets the
    # longer input's tail through unchanged.
    sentinel = object()
    for first, second in itertools.zip_longest(a, b, fillvalue=sentinel):
        if first is not sentinel:
            yield first  # type: ignore[misc]
        if second is not sentinel:
            yield second  # type: ignore[misc]


def repeat_each(values: Iterable[int], times: int) -> Iterator[int]:
    if times <= 0:
        return
    for value in values:
        yield from itertools.repeat(value, times)
