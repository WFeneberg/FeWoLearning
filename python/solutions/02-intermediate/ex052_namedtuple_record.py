"""Exercise 052 — NamedTuple (reference solution)."""

import math
from typing import Any, NamedTuple


class Point(NamedTuple):
    x: int
    y: int = 0


class Segment(NamedTuple):
    start: Point
    end: Point

    @property
    def length(self) -> float:
        # math.hypot takes the deltas directly and avoids overflow on large ones.
        return math.hypot(self.end.x - self.start.x, self.end.y - self.start.y)


def as_dict(point: Any) -> dict[str, Any]:
    return dict(point._asdict())


def with_x(point: Any, x: int) -> Any:
    # _replace returns a new instance; a NamedTuple cannot be assigned to.
    return point._replace(x=x)


def field_names() -> tuple[str, ...]:
    return Point._fields


def from_iterable(values: Any) -> Any:
    # _make consumes any iterable and raises TypeError on the wrong arity.
    return Point._make(values)


def sort_points(points: list[Any]) -> list[Any]:
    # Tuples compare element by element, so this is x-then-y with no key function.
    return sorted(points)


def total_length(segments: list[Any]) -> float:
    return sum(segment.length for segment in segments)
