"""Exercise 080 — properties, computed values and caching (reference solution)."""

from functools import cached_property
from typing import Any, Callable

ABSOLUTE_ZERO_CELSIUS = -273.15


def _positive_number(value: Any, name: str) -> float:
    # bool first: it is an int subclass, and `Rectangle(True, 4)` should not be legal.
    if isinstance(value, bool) or not isinstance(value, (int, float)):
        raise TypeError(f"{name} must be a number, got {type(value).__name__}")
    if value <= 0:
        raise ValueError(f"{name} must be greater than zero, got {value!r}")
    return value


class Rectangle:
    def __init__(self, width: float, height: float) -> None:
        # Assigning through the properties means the constructor gets the validation for
        # free — `self._width = width` would have skipped it.
        self.width = width
        self.height = height

    @property
    def width(self) -> float:
        return self._width

    @width.setter
    def width(self, value: float) -> None:
        self._width = _positive_number(value, "width")

    @property
    def height(self) -> float:
        return self._height

    @height.setter
    def height(self, value: float) -> None:
        self._height = _positive_number(value, "height")

    @property
    def area(self) -> float:
        # No setter, so assignment raises AttributeError and the value can never be stale.
        return self._width * self._height

    @property
    def is_square(self) -> bool:
        return self._width == self._height


class Temperature:
    def __init__(self, celsius: float = 0.0) -> None:
        self.celsius = celsius

    @property
    def celsius(self) -> float:
        return self._celsius

    @celsius.setter
    def celsius(self, value: float) -> None:
        if isinstance(value, bool) or not isinstance(value, (int, float)):
            raise TypeError(f"celsius must be a number, got {type(value).__name__}")
        if value < ABSOLUTE_ZERO_CELSIUS:
            raise ValueError(f"{value} °C is below absolute zero")
        self._celsius = float(value)

    @property
    def fahrenheit(self) -> float:
        return self._celsius * 9 / 5 + 32

    @fahrenheit.setter
    def fahrenheit(self, value: float) -> None:
        # Route through the celsius setter rather than touching _celsius: one place holds
        # the invariant, so the two units cannot drift or skip the range check.
        self.celsius = (value - 32) * 5 / 9

    @property
    def kelvin(self) -> float:
        return self._celsius - ABSOLUTE_ZERO_CELSIUS


class Report:
    def __init__(self, values: list[int]) -> None:
        self._values = list(values)
        self._computations = 0

    @cached_property
    def total(self) -> int:
        self._computations += 1
        return sum(self._values)

    def invalidate(self) -> None:
        # cached_property is a *non-data* descriptor: it has no __set__, so the entry it
        # writes into the instance dict under its own name shadows it on later reads.
        # Removing that entry un-shadows the descriptor and the body runs again.
        self.__dict__.pop("total", None)


def lazy_attribute(compute: Callable[[Any], Any]) -> property:
    key = f"_lazy_{compute.__name__}"

    def getter(instance: Any) -> Any:
        if key not in instance.__dict__:
            instance.__dict__[key] = compute(instance)
        return instance.__dict__[key]

    getter.__name__ = compute.__name__
    getter.__doc__ = compute.__doc__
    # A property is a *data* descriptor, so unlike cached_property it stays in charge of
    # every read — which is why the stash needs its own private key.
    return property(getter)
