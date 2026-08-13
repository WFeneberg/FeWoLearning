"""Exercise 084 — custom iterators (advanced).

Goal:   Implement the iterator protocol by hand, and see the difference between an
        *iterator* (exhausts once, `__iter__` returns `self`) and an *iterable
        container* (re-iterable, `__iter__` returns a fresh iterator every time).
Drills: `__iter__`, `__next__`, raising `StopIteration` to signal the end, manual
        `next()` calls, and why a container class should hand out a *new* iterator
        object on every `iter()` rather than being its own iterator.
Passes: when `pytest exercises/03-advanced/test_ex084_custom_iterator.py` is green.

Note:   this is deliberately not a generator function (`ex037`/`ex038` already cover
        that) — the point here is the two dunder methods a generator hides from you.
"""

from typing import Any, Iterator, Sequence


class CountUpTo:
    """An iterator counting from `start` to `end`, inclusive, then exhausted for good.

    It *is* its own iterator: `iter(count_up_to)` returns the same object, so once
    a `for` loop (or manual `next()` calls) drains it, it stays drained — a second
    pass yields nothing.
    """

    def __init__(self, start: int, end: int) -> None:
        raise NotImplementedError

    def __iter__(self) -> "CountUpTo":
        raise NotImplementedError

    def __next__(self) -> int:
        """Return the next value, or raise `StopIteration` once past `end`."""
        raise NotImplementedError


class Batched:
    """A reusable container that yields `data` in fixed-size chunks.

    Unlike `CountUpTo`, this class is *not* its own iterator: every call to
    `iter(batched)` must return a brand-new iterator object, so the same `Batched`
    can be looped over any number of times. `size` must be positive — zero or
    negative raises ValueError.
    """

    def __init__(self, data: Sequence[Any], size: int) -> None:
        raise NotImplementedError

    def __iter__(self) -> Iterator[list[Any]]:
        raise NotImplementedError


class _BatchIterator:
    """The iterator `Batched.__iter__` hands out. Not used directly by callers."""

    def __init__(self, data: Sequence[Any], size: int) -> None:
        raise NotImplementedError

    def __iter__(self) -> "_BatchIterator":
        raise NotImplementedError

    def __next__(self) -> list[Any]:
        """Return the next chunk, or raise `StopIteration` once `data` is consumed.

        The final chunk may be shorter than `size` — do not pad it.
        """
        raise NotImplementedError
