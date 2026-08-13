"""Exercise 083 — __slots__, attribute restriction and memory (advanced).

Goal:   Replace the free-form per-instance `__dict__` with a fixed, declared set of
        attributes — smaller instances, and typo'd attribute names that fail loudly
        instead of silently creating a new attribute.
Drills: declaring `__slots__`, what it removes (`__dict__`), how slots compose across
        a subclass (a subclass only lists its *own* new slots, not the base's), and
        the size difference this makes versus an ordinary class.
Passes: when `pytest exercises/03-advanced/test_ex083_slots_memory.py` is green.

Note:   `DictPoint` below is deliberately *not* part of the exercise — it is an
        ordinary class with no `__slots__`, kept only as the "before" picture the
        tests compare `Point` against.
"""

from typing import Any


# TODO: add `__slots__ = ("x", "y")` and implement the methods below.
class Point:
    """A 2D point restricted to exactly the attributes `x` and `y`."""

    def __init__(self, x: float, y: float) -> None:
        raise NotImplementedError

    def __repr__(self) -> str:
        raise NotImplementedError

    def __eq__(self, other: object) -> bool:
        raise NotImplementedError


# TODO: add `__slots__ = ("z",)` — do not repeat "x", "y", Point already declares them.
class Point3D(Point):
    """A 3D point: everything `Point` has, plus `z`."""

    def __init__(self, x: float, y: float, z: float) -> None:
        raise NotImplementedError

    def __repr__(self) -> str:
        raise NotImplementedError

    def __eq__(self, other: object) -> bool:
        raise NotImplementedError


class DictPoint:
    """An ordinary point with no `__slots__` — the memory-usage baseline. Not part of
    the exercise."""

    def __init__(self, x: float, y: float) -> None:
        self.x = x
        self.y = y


def declared_slots(cls: type) -> set[str]:
    """Every slot name declared anywhere on `cls`'s MRO, flattened into one set.

    `cls.__slots__` alone only lists the slots *that class* added — a subclass's own
    `__slots__` says nothing about its base's. Walk `cls.__mro__`, read each class's
    own `__dict__.get("__slots__", ())`, and flatten: `__slots__` may be a single
    string (one slot) or a tuple/list of strings (several) — normalize both into the
    result set. Classes with no `__slots__` at all (like `object` or `DictPoint`)
    simply contribute nothing.
    """
    raise NotImplementedError


def has_instance_dict(obj: object) -> bool:
    """Whether `obj` has a per-instance `__dict__`.

    True for ordinary instances, False for an instance of a fully-slotted class (and
    everything on its MRO is slotted too).
    """
    raise NotImplementedError
