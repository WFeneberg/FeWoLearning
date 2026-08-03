"""Exercise 052 — NamedTuple (intermediate).

Goal:   A record that is still a tuple — indexable, unpackable, immutable.
Drills: typing.NamedTuple, field defaults, _replace/_asdict/_fields, tuple
        behaviour (indexing, unpacking, comparison), when to prefer it over a
        frozen dataclass.
Passes: when `pytest exercises/02-intermediate/test_ex052_namedtuple_record.py` is green.

Note:   the classes carry only annotations. Making them inherit from NamedTuple is
        the exercise — annotations alone give no constructor.
"""

from typing import Any, NamedTuple  # noqa: F401  (available for your solution)


# TODO: make this a NamedTuple.
class Point:
    """A 2-D point where `y` defaults to 0.

    Being a tuple, it must support ``p[0]``, ``x, y = p`` and comparison with a plain
    tuple — that last one is exactly what a frozen dataclass does *not* give you.
    """

    x: int
    y: int


# TODO: make this a NamedTuple with a method.
class Segment:
    """Two points, with a ``length`` property.

    A NamedTuple can carry methods and properties like any class.
    """

    start: Any
    end: Any


def as_dict(point: Any) -> dict[str, Any]:
    """Convert to a dict, using the generated ``_asdict``."""
    raise NotImplementedError


def with_x(point: Any, x: int) -> Any:
    """Return a copy with a different `x`, using the generated ``_replace``.

    A NamedTuple is immutable, so this cannot assign.
    """
    raise NotImplementedError


def field_names() -> tuple[str, ...]:
    """Return Point's field names, from the generated ``_fields``."""
    raise NotImplementedError


def from_iterable(values: Any) -> Any:
    """Build a Point from an iterable of coordinates, using ``_make``.

    Too few or too many values raises TypeError.
    """
    raise NotImplementedError


def sort_points(points: list[Any]) -> list[Any]:
    """Sort points ascending.

    No key needed: tuples compare element by element, so this is x-then-y for free.
    """
    raise NotImplementedError


def total_length(segments: list[Any]) -> float:
    """Sum the `length` of every segment."""
    raise NotImplementedError
