"""Exercise 090 — abc.ABC, abstractmethod and interface checks (advanced).

Goal:   Declare an interface that cannot be half-implemented — a subclass missing
        even one required method stays uninstantiable — and see the other kind of
        interface check abc offers: structural registration, with no inheritance
        at all.
Drills: `abc.ABC`, `@abstractmethod`, why a subclass overriding only *some* of the
        abstract methods is still abstract, and `ABCMeta.register` for making
        `isinstance` recognize an unrelated class as "one of these" on purpose.
Passes: when `pytest exercises/03-advanced/test_ex090_abc_abstract_base.py` is green.

Note:   `Square` below is a plain, fully-implemented class that never inherits from
        `Shape` — it exists only to be registered by `register_virtual_shape`, not
        to be implemented itself.
"""

from abc import ABC


class Shape(ABC):
    """The interface every shape below implements: `area`, `perimeter`, and the
    `describe` this base class builds out of them.

    TODO: mark `area` and `perimeter` as abstract methods, so that neither `Shape`
    itself nor an incomplete subclass (overriding only one of the two) can be
    instantiated.
    """

    def area(self) -> float:
        raise NotImplementedError

    def perimeter(self) -> float:
        raise NotImplementedError

    def describe(self) -> str:
        """Return ``f"{ClassName}: area={area:.2f}, perimeter={perimeter:.2f}"``."""
        raise NotImplementedError


class Rectangle(Shape):
    def __init__(self, width: float, height: float) -> None:
        self.width = width
        self.height = height

    def area(self) -> float:
        raise NotImplementedError

    def perimeter(self) -> float:
        raise NotImplementedError


class Circle(Shape):
    def __init__(self, radius: float) -> None:
        self.radius = radius

    def area(self) -> float:
        raise NotImplementedError

    def perimeter(self) -> float:
        raise NotImplementedError


class IncompleteShape(Shape):
    """Overrides only `area` — still abstract, so still uninstantiable, once
    `Shape` correctly marks both methods as abstract."""

    def area(self) -> float:
        return 0.0


class Square:
    """A duck-typed shape that never inherits from `Shape`. Not part of the
    exercise — `register_virtual_shape` is what connects it to `Shape`."""

    def __init__(self, side: float) -> None:
        self.side = side

    def area(self) -> float:
        return self.side**2

    def perimeter(self) -> float:
        return 4 * self.side


def register_virtual_shape(cls: type) -> None:
    """Register `cls` as a virtual subclass of `Shape`.

    Afterwards `isinstance(instance_of_cls, Shape)` (and `issubclass`) is True even
    though `cls` never inherits from `Shape` — structural registration, not
    inheritance.
    """
    raise NotImplementedError
