"""Exercise 050 — Protocol (reference solution)."""

from typing import Iterable, Protocol, runtime_checkable


@runtime_checkable
class Named(Protocol):
    name: str


@runtime_checkable
class Closeable(Protocol):
    def close(self) -> None: ...


class Sized(Protocol):
    def __len__(self) -> int: ...


class Drawable(Protocol):
    @property
    def area(self) -> float: ...


def names_of(items: Iterable[Named]) -> list[str]:
    return [item.name for item in items]


def close_all(items: Iterable[object]) -> int:
    closed = 0
    for item in items:
        # runtime_checkable only permits this isinstance check, and it only verifies
        # that a `close` attribute exists — not that it takes no arguments.
        if isinstance(item, Closeable):
            item.close()
            closed += 1
    return closed


def total_length(items: Iterable[Sized]) -> int:
    # No isinstance here: Sized is not runtime_checkable, and len() already raises
    # TypeError for anything without __len__.
    return sum(len(item) for item in items)


def largest_area(shapes: Iterable[Drawable]) -> float:
    return max((shape.area for shape in shapes), default=0.0)


def is_named(value: object) -> bool:
    return isinstance(value, Named)
