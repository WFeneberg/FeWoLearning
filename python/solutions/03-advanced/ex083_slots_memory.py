"""Exercise 083 — __slots__, attribute restriction and memory (reference solution)."""

from typing import Any


class Point:
    __slots__ = ("x", "y")

    def __init__(self, x: float, y: float) -> None:
        self.x = x
        self.y = y

    def __repr__(self) -> str:
        return f"Point(x={self.x!r}, y={self.y!r})"

    def __eq__(self, other: object) -> bool:
        if other.__class__ is not self.__class__:
            return NotImplemented
        return (self.x, self.y) == (other.x, other.y)


class Point3D(Point):
    __slots__ = ("z",)

    def __init__(self, x: float, y: float, z: float) -> None:
        super().__init__(x, y)
        self.z = z

    def __repr__(self) -> str:
        return f"Point3D(x={self.x!r}, y={self.y!r}, z={self.z!r})"

    def __eq__(self, other: object) -> bool:
        if other.__class__ is not self.__class__:
            return NotImplemented
        return (self.x, self.y, self.z) == (other.x, other.y, other.z)


class DictPoint:
    """An ordinary point with no `__slots__` — the memory-usage baseline. Not part of
    the exercise."""

    def __init__(self, x: float, y: float) -> None:
        self.x = x
        self.y = y


def declared_slots(cls: type) -> set[str]:
    names: set[str] = set()
    for klass in cls.__mro__:
        slots = klass.__dict__.get("__slots__", ())
        if isinstance(slots, str):
            names.add(slots)
        else:
            names.update(slots)
    return names


def has_instance_dict(obj: object) -> bool:
    return hasattr(obj, "__dict__")
