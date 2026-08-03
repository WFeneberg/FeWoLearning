"""Exercise 050 — Protocol (intermediate).

Goal:   Accept "anything with the right shape" instead of a fixed base class.
Drills: Protocol for structural typing, @runtime_checkable and its limits,
        protocols with properties, why duck typing plus a Protocol beats an ABC when
        you do not own the classes.
Passes: when `pytest exercises/02-intermediate/test_ex050_typing_protocol.py` is green.
"""

from typing import Iterable, Protocol, runtime_checkable


@runtime_checkable
class Named(Protocol):
    """Anything with a `name` string attribute."""

    name: str


@runtime_checkable
class Closeable(Protocol):
    """Anything with a no-argument ``close()``."""

    def close(self) -> None: ...


class Sized(Protocol):
    """Anything with ``__len__``. Deliberately *not* runtime_checkable."""

    def __len__(self) -> int: ...


class Drawable(Protocol):
    """Anything that can report an area, as a property."""

    @property
    def area(self) -> float: ...


def names_of(items: Iterable[Named]) -> list[str]:
    """Return the `name` of each item.

    No shared base class is required — only the attribute.
    """
    raise NotImplementedError


def close_all(items: Iterable[object]) -> int:
    """Call ``close()`` on every item that has one, returning how many were closed.

    Uses ``isinstance(item, Closeable)``, which works because Closeable is
    runtime_checkable. Note the limit: that check verifies the *presence* of the
    method, never its signature.
    """
    raise NotImplementedError


def total_length(items: Iterable[Sized]) -> int:
    """Sum ``len(item)`` over the items.

    Sized is not runtime_checkable, so this must not use isinstance on it — just call
    len() and let a missing ``__len__`` raise TypeError.
    """
    raise NotImplementedError


def largest_area(shapes: Iterable[Drawable]) -> float:
    """Return the largest `area`, or 0.0 when there are no shapes."""
    raise NotImplementedError


def is_named(value: object) -> bool:
    """Report whether `value` satisfies Named at runtime."""
    raise NotImplementedError
